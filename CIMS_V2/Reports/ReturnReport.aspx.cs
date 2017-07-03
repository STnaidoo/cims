using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIMS_Datalayer;
using System.IO;
using CIMS_V2.AddOn;
using System.Data;

namespace CIMS_V2.Reports
{
    public partial class ReturnReport : System.Web.UI.Page
    {

        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            try
            {
                if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
                {

                    drpInstructions.Items.Clear();
                    drpBranchs.Items.Clear();
                    drpSearchBy.Items.Clear();
                    //drpStatus.Items.Clear();
                    chkBoxStatus.Items.Clear();

                    ReportUtilityInfo reportUtility = new ReportUtilityInfo();
                    SharedFunctions sharedFunctions = new SharedFunctions();
                    GenericDbFunctions genericDbFunctions = new GenericDbFunctions();
                    //load search by value
                    sharedFunctions.LoadDropDownList(
                        drpSearchBy,
                        genericDbFunctions.GetDropdownListInfo("search_by", new string[] {"search_by_name", "search_by_value"}, new string[] {"search_by_module"}, new string[] {"return_report"}),
                        "search_by_name",
                        "search_by_value");

                    //load instruction types
                    sharedFunctions.LoadDropDownList(
                        drpInstructions,
                        genericDbFunctions.GetDropdownListInfo("instructions_types", new string[] {"instruction_type", "instruction_type_id"}, new string[] { "active" }, new string[] { "1" }),
                        "instruction_type",
                        "instruction_type_id");

                    //load document status
                    //sharedFunctions.LoadDropDownList(
                    //    drpStatus,
                    //    genericDbFunctions.GetDropdownListInfo("document_status", new string[] {"document_status_id", "document_status"}, null, null),
                    //    "document_status",
                    //    "document_status_id");

                    //load check list
                    sharedFunctions.LoadCheckList(
                        chkBoxStatus,
                        genericDbFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                        "document_status",
                        "document_status_id");

                    //load user branches 
                    sharedFunctions.LoadDropDownList(
                        drpBranchs,
                        genericDbFunctions.GetDropdownListInfo("user_branch", new string[] {"branch_name", "branch_id"}, null, null),
                        "branch_name",
                        "branch_id");

                    txtFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    txtTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    drpInstructions.ClearSelection();
                    drpInstructions.Items.FindByText("Outward Telegraphic Transfer").Selected = true;
                    //drpInstructions.Enabled = false;
                    
                }
                else
                {
                    Server.Transfer("~/Account/Login.aspx");
                }
            }
            catch(Exception ex)
            {
                errorLog.LogError("LoadPage", ex.StackTrace);
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

        public void loadTransactions()
        {
            try
            {
                DataSet set = ReportUtility.getReturnsReportByBranch(drpBranchs, drpInstructions, txtFrom.Text, txtTo.Text);
                if (set != null)
                {
                    dgvReturnsReport.DataSource = set.Tables[0].DefaultView;
                    dgvReturnsReport.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("loadTransactions", ex.StackTrace);
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
                writeToExcelFile(dgvReturnsReport);
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
            string filename = "ReturnReport" + DateTime.Now + ".xls";
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