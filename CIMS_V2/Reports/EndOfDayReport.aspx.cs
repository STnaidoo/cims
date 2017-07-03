using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIMS_V2.Reports
{
    public partial class EndOfDayReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        OperationsLog logOperations = new OperationsLog();
        Alerts alert = new Alerts();

        ReportUtilityInfo reportUtility = new ReportUtilityInfo();
        GenericDbFunctions genericDbFunctions = new GenericDbFunctions();
        SharedFunctions sharedFunctions = new SharedFunctions();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        //page init
        protected void LoadPage()
        {
            try
            {
                if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
                {

                    //clear lists
                    drpInstructions.Items.Clear();
                    drpBranchs.Items.Clear();
                    drpSearchBy.Items.Clear();
                    //drpStatus.Items.Clear();
                    chkBoxStatus.Items.Clear();



                    //load search by value
                    sharedFunctions.LoadDropDownList(
                        drpSearchBy,
                        genericDbFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "transaction" }),
                        "search_by_name",
                        "search_by_value");

                    //load all instruction types
                    sharedFunctions.LoadDropDownList(
                        drpInstructions,
                        genericDbFunctions.GetDropdownListInfo("instructions_types", 
                                                             new string[] { "instruction_type_id", "instruction_type" }, 
                                                             new string[] { "active" }, 
                                                             new string[] { "1" }), 
                        "instruction_type", 
                        "instruction_type_id");

                    //load all document statuses
                    //sharedFunctions.LoadDropDownList(
                    //   drpStatus,
                    //   genericDbFunctions.GetDropdownListInfo("document_status", new string[] {"document_status_id", "document_status"}, null, null),
                    //   "document_status",
                    //   "document_status_id");

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

                    drpBranchs.SelectedValue = Session["UserBranch"].ToString();

                    txtFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    txtTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                }
                else
                {
                    Server.Transfer("~/Account/Login.aspx");
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("LoadPage", ex.StackTrace);
                throw ex;
            }
        }

        protected void loadTransactions()
        {
            ReportUtilityInfo reportUtil = new ReportUtilityInfo();

            //dgvTransactionsReport.DataSource =
            //    reportUtil.GetTransactionview(
            //        drpSearchBy.SelectedValue,
            //        txtSearch.Text,
            //        Convert.ToDateTime(txtFrom.Text),
            //        Convert.ToDateTime(txtTo.Text),
            //        drpBranchs.SelectedValue,
            //        drpInstructions.SelectedValue,
            //        Convert.ToInt32(drpStatus.SelectedValue));

            sharedFunctions.LoadGridView(
                dgvTransactionsReport,
                reportUtility.GetTransactionview("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, txtFrom.Text, txtTo.Text, "inserted_date", "instruction_type", drpInstructions.SelectedItem.Text));

            //dgvTransactionsReport.DataBind();
     

            checkAndDisableAuthorized();
        }

        //loads transaction
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

            if (drpBranchs.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select a valid branch");
                shouldLoad = false;
            }
            if (drpSearchBy.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select a Filter By option");
                shouldLoad = false;
            }

            if (shouldLoad)
            {
                loadTransactions();
            }
        }

        //select all
        protected void checkAll()
        {
            for (int i = 0; i < dgvTransactionsReport.Rows.Count; i++)
            {

                CheckBox check = (CheckBox)dgvTransactionsReport.Rows[i].FindControl("chkVerified");
                if (check.Enabled)
                {
                    check.Checked = true;
                }
            }
        }

        //check all approved transactions and disable control
        protected void checkAndDisableAuthorized()
        {
            for (int i = 0; i < dgvTransactionsReport.Rows.Count; i++)
            {
                String branchManualApprovedBy = dgvTransactionsReport.Rows[i].Cells[1].Text;
                int approved;
                if (!String.IsNullOrEmpty(branchManualApprovedBy))
                {
                    String documentStage = dgvTransactionsReport.Rows[i].Cells[13].Text;
                    CheckBox check = (CheckBox)dgvTransactionsReport.Rows[i].FindControl("chkVerified");

                    if (int.TryParse(branchManualApprovedBy, out approved))
                    {
                        check.Checked = true;
                        check.Enabled = false;
                    }

                    if (int.TryParse(documentStage, out approved) && approved > 2)
                    {
                        check.Checked = true;
                        check.Enabled = false;
                    }
                }
            }
        }


        protected void chkMultiple_CheckedChanged(object sender, EventArgs e)
        {
            //chkBoxStatus.Visible = chkMultiple.Checked;
            //drpStatus.Enabled = (!chkMultiple.Checked);
        }


        //prints selected transactions
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string instructionID = "", instructionIDs = "";

            for (int i = 0; i < dgvTransactionsReport.Rows.Count; i++)
            {
                instructionID = dgvTransactionsReport.Rows[i].Cells[0].Text;
                CheckBox check = (CheckBox)dgvTransactionsReport.Rows[i].FindControl("chkVerified");
                if (check.Checked)
                {
                    if (string.IsNullOrEmpty(instructionIDs))
                    {
                        instructionIDs = instructionID;
                    }
                    else
                    {
                        instructionIDs = instructionIDs + " ; " + instructionID;
                    }
                }
            }
        }

        //check transactions
        protected void btnCheckAll_Click(object sender, EventArgs e)
        {
            checkAll();
        }

        //uncheck transactions 
        protected void btnUncheck_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvTransactionsReport.Rows.Count; i++)
            {

                CheckBox check = (CheckBox)dgvTransactionsReport.Rows[i].FindControl("chkVerified");
                if (check.Enabled)
                {
                    check.Checked = false;
                }
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvTransactionsReport.Rows.Count; i++)
            {
                CheckBox check = (CheckBox)dgvTransactionsReport.Rows[i].FindControl("chkVerified");
                if (check.Enabled && check.Checked)
                {
                    String instruction_id = dgvTransactionsReport.Rows[i].Cells[0].Text;

                    try
                    {
                        ReportUtilityInfo reportUtil = new ReportUtilityInfo();
                        bool submit = reportUtil.SubmitAndAllocateInstrunctions(
                            Convert.ToInt64(instruction_id),
                            3,
                            0,
                            DateTime.Now,
                            9,
                            Convert.ToInt32(Session["UserType"].ToString()),
                            Convert.ToInt32(Session["UserID"].ToString()),
                            9,
                            null);

                        if (submit)
                        {
                            logOperations.InsertOperationsLog(
                                Convert.ToInt32(Session["userID"].ToString()),
                                "Instruction submitted successfully with the following details: Instruction ID:  " + instruction_id + " ",
                                "", "0", 0, "Instruction submitted successfully","",0);
                        }
                        else
                        {
                            logOperations.InsertOperationsLog(
                              Convert.ToInt32(Session["userID"].ToString()),
                              "Error submitting instruction with the following details: Instruction ID:  " + instruction_id + " ",
                              "", "0", 0, "Error submitting instruction", "", 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLog.LogError("btnApprove", ex.StackTrace);
                    }

                }
            }
        }

        // enables approve button when user branch id is equal to instruction branch id
        protected void drpBranchs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Session["UserBranchID"].ToString())
                && drpBranchs.SelectedValue.Equals((Session["UserBranchID"].ToString())))
            {
                btnApprove.Enabled = true;
            }
            else
            {
                btnApprove.Enabled = true;
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
            string filename = "EndOfDayReport" + DateTime.Now + ".xls";
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