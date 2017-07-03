using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIMS_V2.AddOn;
using CIMS_Datalayer;
using System.Data;
using System.IO;

namespace CIMS_V2.Reports
{
    public partial class TransactionSummaryReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();
        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
            {
                drpStatus.Items.Clear();
     

                ReportUtilityInfo reportUtil = new ReportUtilityInfo();
                SharedFunctions sharedFunctions = new SharedFunctions();
                GenericDbFunctions genericDbFunctions = new GenericDbFunctions();



                sharedFunctions.LoadDropDownList(
                       drpStatus,
                       genericDbFunctions.GetDropdownListInfo("document_status", new string[] {"document_status", "document_status_id"}, null, null),
                       "document_status",
                       "document_status_id");



                txtFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                txtTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            }
            else
            {
                Server.Transfer("~/Account/Login.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            bool shouldLoad = true;
            if (string.IsNullOrEmpty(txtFrom.Text))
            {
                alert.FireAlerts(this.Page, "Please enter a from value");
                shouldLoad = false;
            }
            if (string.IsNullOrEmpty(txtTo.Text))
            {
                alert.FireAlerts(this.Page, "Please enter a from value");
                shouldLoad = false;
            }

            if (shouldLoad)
            {
                loadTransactions();
            }
        }

        private void loadTransactions()
        {
            try
            {
                DataSet set = ReportUtility.getTransactionSummaryReport(drpStatus, txtFrom.Text, txtTo.Text);
                if (set!=null)
                {
                    dgvTransactionsReport.DataSource = set.Tables[0].DefaultView;
                    dgvTransactionsReport.DataBind();
                }
            }
            catch(Exception ex)
            {
                errorLog.LogError("loadTransactions", ex.StackTrace);
            }
        }

        #region ExcelExportMethods 

        //This method stops the runtime error that asp.net throws because we cannot nest the Gridview inside a <form> tag. Just leave it here doing nothing.
        //NO TOUCHY!
        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }

        //Calls the excel export method
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                writeToExcelFile(dgvTransactionsReport);
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "An error has been encountered during the export process");
                errorLog.LogError("btnExport_Click", ex.StackTrace);
            }
        }

        //Writes to an excel file
        private void writeToExcelFile(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string filename = "TransactionSummaryReport" + DateTime.Now + ".xls";
            StringWriter writer = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("content-disposition", "attachment;filename= " + filename);
            gridView.GridLines = GridLines.Both;
            gridView.HeaderStyle.Font.Bold = true;
            gridView.RenderControl(htmlWriter);
            string renderedView = writer.ToString();

            Response.Write(renderedView);
            Response.End();

        }
        #endregion
    }
}