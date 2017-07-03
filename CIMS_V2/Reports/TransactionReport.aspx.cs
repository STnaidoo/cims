using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Data;
using CIMS_V2.AddOn;
using CIMS_Datalayer;
using System.Web.UI.WebControls;

namespace CIMS_V2.Reports
{
    public partial class TransactionReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();

        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            try
            {

                drpInstructions.Items.Clear();
                drpSearchBy.Items.Clear();
                drpStatus.Items.Clear();

                ReportUtilityInfo reportUtilityInfo = new ReportUtilityInfo();
                SharedFunctions sharedFunction = new SharedFunctions();
                GenericDbFunctions genericDbFunctions = new GenericDbFunctions();

                //load transaction  search options
                sharedFunction.LoadDropDownList(
                    drpSearchBy,
                    genericDbFunctions.GetDropdownListInfo("search_by", new string[] {"search_by_name", "search_by_value"}, new string[] {"search_by_module"}, new string[] {"transaction"}),
                    "search_by_name",
                    "search_by_value");

                //load instruction types
                sharedFunction.LoadDropDownList(
                    drpInstructions,
                    genericDbFunctions.GetDropdownListInfo("instructions_types", new string[] {"instruction_type_id", "instruction_type"}, null, null),
                    "instruction_type",
                    "instruction_type_id");

                //load document status
                sharedFunction.LoadDropDownList(
                    drpStatus,
                    genericDbFunctions.GetDropdownListInfo("document_status", new string[] {"document_status_id", "document_status"}, null, null),
                    "document_status",
                    "document_status_id");

                txtFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                txtTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            }
            catch (Exception ex)
            {
                errorLog.LogError("Page_Load", ex.StackTrace);
                throw ex;
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
                alert.FireAlerts(this.Page, "Please enter a valid from date");
                shouldLoad = false;
            }
            if (string.IsNullOrEmpty(txtTo.Text))
            {
                alert.FireAlerts(this.Page, "Please enter a valid to date");
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
                ReportUtilityInfo reportUtil = new ReportUtilityInfo();
                var search = reportUtil.processTranReport(
                    Convert.ToDateTime(txtFrom.Text), 
                    Convert.ToDateTime(txtTo.Text), 
                    txtSearch.Text,
                    drpInstructions.SelectedValue,
                    drpStatus.SelectedValue,
                    drpSearchBy.SelectedValue,
                    drpStatus,
                    drpInstructions);
                
                if (search != null)
                {
                    dgvTransactionsReport.DataSource = search;
                    dgvTransactionsReport.DataBind();
                }
            }
            catch(Exception ex)
            {
                errorLog.LogError("loadTransactions", ex.StackTrace);
                throw ex;
            }
            
        }

        #region ExcelExportMethods 
        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }
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

        private void writeToExcelFile(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string filename = "TransactionReport" + DateTime.Now + ".xls";
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