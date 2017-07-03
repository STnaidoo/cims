using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Common;
using CIMS_Datalayer;
using CIMS_V2.AddOn;

namespace CIMS_V2.Unknown_Views
{
    public partial class PRMU_Queue : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationLog = new OperationsLog();
        InstructionsInfo instructionsInfo = new InstructionsInfo();
        DAccessInfo daccess = new DAccessInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            string ddfdd = Session["UserID"].ToString();

            if (!IsPostBack)
            {
                LoadPage();
            }

        }

        protected void LoadPage()
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
            }
            //??? dropdowns
            //load instructions
            sharedUtility.LoadDropDownList(
                drpInstructions,
                genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "instruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocation WHERE status", "system_user_id IN (SELECT system_tl_1 FROM system_users WHERE system_user_id" }, new string[] { "1", Session["UserID"].ToString() + "))" }),
                "instruction_type",
                "instruction_type_id");
            //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
            //"select '0' AS instruction_type_id, ' Select Instruction Type' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id IN (select system_tl_1 from system_users where system_user_id = '" & Session["UserID"] & "' ) )", "instruction_type"

            sharedUtility.LoadDropDownList(
                drpInstructions,
                genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, null, null),
                "instruction_type",
                "instruction_type_id");
            //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
            //"select '0' AS instruction_type_id, ' Select Instruction Type' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types"

            //load document status
            sharedUtility.LoadDropDownList(
                drpValidationStatus,
                genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                "document_status",
                "document_status_id");
            //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status", "document_status_id");
            //"select '0' AS document_status_id, ' Select Action' AS document_status from document_status UNION select document_status_id, document_status from document_status"

            //load currency
            sharedUtility.LoadDropDownList(
                drpCurrency,
                genericFunctions.GetDropdownListInfo("currency", new string[] { "currency_id", "currency_name" }, null, null),
                "currency_name",
                "currency_id");
            //sharedUtility.LoadDropDownList(drpCurrency, genericFunctions.GetCurrencyList(), "currency_name", "currency_id");
            //"select '0' AS currency_id, ' Select Currency' AS currency_name from currency UNION select currency_id, currency_name from currency"

            //load user branch
            sharedUtility.LoadDropDownList(
                drpBranchs, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null),
                "branch_name",
                "branch_id");
            //sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
            //"select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch "

            //load rm ro
            sharedUtility.LoadDropDownList(
                drpRM,
                genericFunctions.GetDropdownListInfo("relationship_managers", new string[] { "RM_ID", "RM_Name" }, null, null),
                "RM_Name",
                "RM_ID");
            //sharedUtility.LoadDropDownList(drpRM, instructionsInfo.GetRM(), "RM_Name", "RM_ID");
            //"select '0' AS RM_ID, ' Select RM' AS RM_Name from relationship_managers UNION select RM_ID, RM_Name from relationship_managers "

            //load user type
            sharedUtility.LoadDropDownList(
                drpUserType,
                genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type_queue" }, new string[] { "display_user_type_queue" }, new string[] { "1" }),
                "user_type_queue",
                "user_type_no");
            //sharedUtility.LoadDropDownList(drpUserType, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type_queue", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type_queue from user_type UNION select user_type_no, user_type_queue from user_type where display_user_type_queue = 1 order by user_type_queue"
        }

        protected void dgvInstructions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt32(e.CommandArgument);
            string id = dgvInstructions.Rows[rw].Cells[2].Text;
            LoadAllDetails(id);
        }

        public void LoadAllDetails(string instruction_id)
        {
            ResetClient();
            LoadInstructionDetails(instruction_id);

            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
            }

            MultiView1.SetActiveView(ViewInstructions);
        }

        public void ResetClient()
        {
            txtClient_Customer_Number.Text = "";
            txtClient_Name.Text = "";
            drpRM.SelectedIndex = -1;
        }

        public void ResetInstructions()
        {
            drpInstructions.SelectedIndex = -1;
            drpRM.SelectedIndex = -1;
            txtAmount.Text = "";
            txtTransactionReference.Text = "";
            txtComments.Text = "";
            txtInstructionID.Text = "";
            txtFileName.Text = "";
            drpValidationStatus.SelectedIndex = -1;
            drpAccount.SelectedIndex = -1;
            drpCurrency.SelectedIndex = -1;
            txtDocumentStatusID.Text = "";
            lblLockedBy.Text = "";
            lblLockedDate.Text = "";
            txtRMComments.Text = "";
            drpBranchs.SelectedIndex = -1;

            ShowPdf1.FilePath = "new.pdf";

            if (Session["UserType"].ToString() == "1")
            {
                int index = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                drpValidationStatus.SelectedIndex = index;
            }
        }

        public void LoadInstructionDetails(string ID)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("instructions", "*", "instruction_ID", ID.ToString());
            int index = 0;

            try
            {
                while (rdr.Read())
                {
                    txtStatus.Text = rdr["Status"].ToString();
                    txtDocumentStatusID.Text = rdr["document_status_id"].ToString();
                    txtClient_Name.Text = rdr["Client_Name"].ToString();
                    txtClient_Customer_Number.Text = rdr["Client_Customer_Number"].ToString();
                    txtClientID.Text = rdr["Client_ID"].ToString();
                    txtFileName.Text = rdr["file_name"].ToString();
                    txtComments.Text = rdr["Comments"].ToString();
                    txtInstructionID.Text = rdr["Instruction_ID"].ToString();
                    txtAmount.Text = rdr["Amount"].ToString();
                    txtTransactionReference.Text = rdr["reference"].ToString();
                    txtFTROAComments.Text = rdr["ftroa_comments"].ToString();
                    txtFTROBComments.Text = rdr["ftrob_comments"].ToString();
                    txtProcessorComments.Text = rdr["processor_comments"].ToString();
                    txtRMComments.Text = rdr["rm_comments"].ToString();
                    txtFTRef.Text = rdr["ft_reference"].ToString();

                    double converted;
                    if (double.TryParse(txtAmount.Text, out converted))
                    {
                        txtAmount.Text = converted.ToString("N2");
                    }

                    index = drpInstructions.Items.IndexOf(drpInstructions.Items.FindByValue(rdr["instruction_type_id"].ToString()));
                    drpInstructions.SelectedIndex = index;

                    index = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue(rdr["document_status_id"].ToString()));
                    drpValidationStatus.SelectedIndex = index;


                    //??? dropdown
                    sharedUtility.LoadDropDownList(
                        drpAccount,
                        genericFunctions.GetDropdownListInfo("client_details", new string[] { "Client_ID", "Client_Account_Number" }, new string[] { "Client_Customer_Number" }, new string[] { txtClient_Customer_Number.Text }),
                        "Client_Account_Number",
                        "Client_ID");
                    //sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetSearchByDropDownListInfo("Client"), "Client_Account_Number", "Client_ID");
                    //"select '0' AS Client_ID, ' Select Account' AS Client_Account_Number from client_details UNION select Client_ID, Client_Account_Number from client_details where Client_Customer_Number = '" + txtClient_Customer_Number.Text + "' "

                    index = drpAccount.Items.IndexOf(drpAccount.Items.FindByText(rdr["account_no"].ToString()));
                    drpAccount.SelectedIndex = index;

                    index = drpCurrency.Items.IndexOf(drpCurrency.Items.FindByValue(rdr["currency_id"].ToString()));
                    drpCurrency.SelectedIndex = index;

                    txtInstructionStatus.Text = rdr["instruction_status"].ToString();

                    lblLockedBy.Text = " Locked By: " + rdr["locked_by_name"].ToString();
                    lblLockedDate.Text = " Date: " + rdr["locked_date"].ToString();

                    index = drpRM.Items.IndexOf(drpRM.Items.FindByValue(rdr["RM_ID"].ToString()));
                    drpRM.SelectedIndex = index;

                    index = drpBranchs.Items.IndexOf(drpBranchs.Items.FindByValue(rdr["branch_id"].ToString()));
                    drpBranchs.SelectedIndex = index;

                    LoadAttachment();

                    if (Session["UserType"].ToString() == "2")
                    {
                        switch (txtStatus.Text)
                        {
                            case "2": //who knows really?
                            case "3":// lmao
                            case "4":// ?
                            case "5":
                                string branchProcessedBy = rdr["branch_processed_by"].ToString();
                                int isTeamMember = daccess.HowManyRecordsExist3Wheres("user_team_leader", "system_tl_1", Session["UserID"].ToString(), "system_user_id", branchProcessedBy, "active", "1");
                                string primaryTeamLeader = daccess.RunStringReturnStringValue1Where("system_users", "system_tl_1", "system_user_id", branchProcessedBy);
                                if (isTeamMember > 0 || primaryTeamLeader == Session["UserID"].ToString())
                                {
                                    btnRecall.Enabled = true;
                                    btnRecall.Visible = true;
                                }
                                else
                                {
                                    btnRecall.Enabled = false;
                                    btnRecall.Visible = false;
                                }
                                break;
                            default:
                                btnRecall.Enabled = false;
                                btnRecall.Visible = false;
                                break;
                        }
                    }
                    else
                    {
                        btnRecall.Enabled = false;
                        btnRecall.Visible = false;
                    }

                    if (Session["UserType"].ToString() == "8")
                    {
                        double result = 0;
                        if (double.TryParse(rdr["locked_by"].ToString(), out result) && result > 0)
                        {
                            btnUnlock.Enabled = true;
                            btnUnlock.Visible = true;
                        }
                        else
                        {
                            btnUnlock.Enabled = false;
                            btnUnlock.Visible = false;
                        }
                    }
                    else
                    {
                        btnUnlock.Enabled = false;
                        btnUnlock.Visible = false;
                    }

                    if (Session["UserType"].ToString() == "15")
                    {
                        if (rdr["is_document_held"].ToString() == "1" && (rdr["Status"].ToString() == "3" || rdr["Status"].ToString() == "13" || rdr["Status"].ToString() == "14"))
                        {
                            btnUnpack.Enabled = true;
                            btnUnpack.Visible = true;
                        }
                        else
                        {
                            btnUnpack.Enabled = false;
                            btnUnpack.Visible = false;
                        }
                    }
                    else
                    {
                        btnUnpack.Enabled = false;
                        btnUnpack.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading instruction details by user", ex.Message);
            }
            finally
            {
                if (rdr != null)
                {
                    if (!rdr.IsClosed)
                    {
                        rdr.Close();
                    }
                    rdr = null;
                }
            }
        }

        public void LoadAttachment()
        {
            //??? dropdown
            //sharedUtility.LoadDropDownList(drpAttachments, "select '0' AS attachment_id, ' Default' AS file_name from instructions_attachment UNION select attachment_ID,  file_name  from instructions_attachment where (deleted is null or deleted =0) and instruction_id = '" + txtInstructionID.Text + "' ", "file_name", "attachment_id");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();
                LoadUnprocessedQueue();
                MultiView1.SetActiveView(ViewCustomerList);
            }
            catch (Exception ex)
            {
                //??? log error?
            }
        }

        public void LoadUnprocessedQueue()
        {
            //??? replace sql
            string sql = "EXEC [dbo].[proc_get_next_instructions] " + drpUserType.SelectedValue;
            string addSql = "";

            double from = 0;
            double to = 0;
            if (double.TryParse(txtMinutesToCuttoffFrom.Text, out from) && double.TryParse(txtMinutesToCuttoffTo.Text, out to))
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_queue] " + drpUserType.SelectedValue + ", " + from.ToString() + ", " + to.ToString() + "  ";
            }

            if (drpStatus.SelectedItem.Text == "Packed")
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_packed_queue] " + drpUserType.SelectedValue;
            }

            if (drpStatus.SelectedItem.Text == "Referrals")
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_referred_queue] " + drpUserType.SelectedValue;
            }

            //??? addSql never used
            sql = sql + addSql;

            try
            {
                //??? sharedUtility doesnt have a getDataSet method
                //DataSet myDataSet = sharedUtility.getDataSet(sql, My.Settings.strDSN);

                //if (myDataset != null)
                //{
                //    dgvInstructions.DataSource = myDataset.Tables[0].DefaultView;
                //    dgvInstructions.DataBind();
                //}
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading queue by user", ex.Message);
            }
        }

        public void LoadProcessedQueue()
        {
            //??? replace sql
            string sql = "EXEC [dbo].[proc_get_next_instructions] " + drpUserType.SelectedValue;
            string addSql = "";

            double from = 0;
            double to = 0;
            if (double.TryParse(txtMinutesToCuttoffFrom.Text, out from) && double.TryParse(txtMinutesToCuttoffTo.Text, out to))
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_queue] " + drpUserType.SelectedValue + ", " + from.ToString() + ", " + to.ToString() + "  ";
            }

            if (drpStatus.SelectedItem.Text == "Packed")
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_packed_queue] " + drpUserType.SelectedValue;
            }

            if (drpStatus.SelectedItem.Text == "Referrals")
            {
                sql = "EXEC [dbo].[proc_get_next_instructions_referred_queue] " + drpUserType.SelectedValue;
            }

            sql = sql + addSql;

            try
            {
                //??? sharedUtility - no getDataSet method
                //DataSet myDataSet = sharedUtility.getDataSet(sql, My.Settings.strDSN);

                //if (myDataset != null)
                //{
                //    dgvInstructions.DataSource = myDataset.Tables[0].DefaultView;
                //    dgvInstructions.DataBind();
                //}
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading queue by user", ex.Message);
            }
        }

        protected void btnRecall_Click(object sender, EventArgs e)
        {
            if (ProcSubmitAllocateInstructions(2, Convert.ToInt32(Session["UserType"]), 0, 25))
            {
                alert.FireAlerts(this.Page, "Recall successful");
            }
        }

        public Boolean ProcSubmitAllocateInstructions(int status, int userType, int allocatedTo, int documentStatusID)
        {
            Constants c = new Constants();
            bool result;
            try
            {


                using (SqlConnection myConnection = new SqlConnection(c.getConnectionString()))
                {

                    //??? My
                    //myConnection = new SqlConnection(My.Settings.strDSN);
                    SqlCommand myCommand = new SqlCommand("proc_submit_and_allocate_instructions", myConnection);
                    int instructionID = Convert.ToInt32(txtInstructionID.Text);
                    DateTime allocatedDate = Convert.ToDateTime("1-1-1900");
                    string userID = Session["UserID"].ToString();
                    int instructionStatus = 25;
                    result = false;

                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;
                    myCommand.Parameters.Add("@instruction_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@status", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_to", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@instruction_status", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_type", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_id", SqlDbType.Int);

                    myCommand.Parameters["@instruction_id"].Value = instructionID;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@allocated_to"].Value = allocatedTo;
                    myCommand.Parameters["@allocated_date"].Value = allocatedDate;
                    myCommand.Parameters["@instruction_status"].Value = instructionStatus;
                    myCommand.Parameters["@user_type"].Value = userType;
                    myCommand.Parameters["@user_id"].Value = userID;
                    myCommand.Parameters["@document_status_id"].Value = documentStatusID;



                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    result = true;
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction submitted (" + drpValidationStatus.SelectedItem.Text + ") successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction submitted ", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error submitting (" + drpValidationStatus.SelectedItem.Text + ") instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error submitting instruction ", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error submitting instruction", ex.Message);
                result = false;
            }
            //finally
            //{
            //    myConnection.Close();
            //}

            //myConnection.Dispose();
            //myCommand.Dispose();
            //myConnection = null;
            //myCommand = null;

            return result;
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            Unlock();
        }

        public void Unlock()
        {
            try
            {
                if (daccess.RunNonQueryEqualsSelect("Update", "instructions", new string[] { "locked_by" }, new string[] { "0" }, "locked_date", "instructions", "locked_date", "instruction_id", "-10", "instruction_id", txtInstructionID.Text))
                {
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unlocked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction unlocked ", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction unlocked successfully");
                    LoadAllDetails(txtInstructionID.Text);
                }
                else
                {
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Ulocking Instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error unlocking ", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Error unlocking instruction");
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error unlocking instruction", ex.Message);
            }
        }

        private Boolean ExportToExcel(GridView gridIn)
        {
            string fileName = "";
            string filePath = "";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter);

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=FileName.xls");
            Response.Charset = "";

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.xls";
            gridIn.RenderControl(htmlTextWriter);

            Response.Write(stringWriter.ToString());
            Response.End();
            Response.Redirect(this.Page.Request.RawUrl.ToString(), true);

            return true;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel(dgvInstructions);
        }

        protected void btnUnpack_Click(object sender, EventArgs e)
        {
            Unpack();
        }

        public void Unpack()
        {
            try
            {
                if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "status", "instruction_status", "document_status_id" }, new string[] { "15", "54", "54" }, "instruction_id", txtInstructionID.Text))
                {
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unpacked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction unpacked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction unpacked successfully");
                    LoadAllDetails(txtInstructionID.Text);
                }
                else
                {
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Unpacking Instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error unpacking", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Error unpacking instruction");
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error unpacking instruction", ex.Message);
            }
        }

        protected void chkCutoffRange_CheckedChanged(object sender, EventArgs e)
        {
            txtMinutesToCuttoffFrom.Visible = chkCutoffRange.Checked;
            txtMinutesToCuttoffTo.Visible = chkCutoffRange.Checked;
            drpStatus.SelectedIndex = -1;
        }

        protected void drpStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpStatus.SelectedIndex > 0)
            {
                chkCutoffRange.Checked = false;
                txtMinutesToCuttoffFrom.Visible = false;
                txtMinutesToCuttoffTo.Visible = false;
            }
        }
    }
}