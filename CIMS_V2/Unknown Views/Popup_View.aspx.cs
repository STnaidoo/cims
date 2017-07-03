using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.Common;
using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System.Text.RegularExpressions;

namespace CIMS_V2.Unknown_Views
{
    public partial class Popup_View : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        InstructionsInfo instructionsInfo = new InstructionsInfo();
        DAccessInfo daccess = new DAccessInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
            }

            if (!Page.IsPostBack)
            {
                dtmFrom.Text = DateTime.Now.ToString("dd/MM/yyyy"); //Format(DateTime.Now(), "dd-MMM-yyyy");
                dtmTo.Text = DateTime.Now.ToString("dd/MM/yyyy");  //Format(DateTime.Now(), "dd-MMM-yyyy");

                //??? dropdowns
                sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }),
                        "search_by_name",
                        "search_by_value");
                //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("customer"), "search_by_value", "search_by_name");

                //load instructions
                sharedUtility.LoadDropDownList(
                    drpInstructions,
                    genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "indtruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocations WHERE status", "system_user_id IN (SELECT system_tl_1 FROM system_users WHERE system_user_id" }, new string[] { "1", Session["UserID"].ToString() + "))" }),
                    "instruction_type",
                    "instruction_types");
                //dnx.LoadDropDownListing("", "select '0' AS instruction_type_id, ' Select Instruction Type' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id IN (select system_tl_1 from system_users where system_user_id = '" & Session("UserID") & "' ) )", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //load search by
                sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                        "search_by_name",
                        "search_by_value");
                //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'instructions' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN)

                //load instructions
                sharedUtility.LoadDropDownList(
                    drpInstructions,
                    genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, null, null),
                    "instruction_type",
                    "instruction_type_id");
                //dnx.LoadDropDownListing("", "select '0' AS instruction_type_id, ' Select Instruction Type' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types ", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //load document status
                sharedUtility.LoadDropDownList(
                    drpValidationStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                    "document_status",
                    "document_status_id");
                //dnx.LoadDropDownListing("", "select '0' AS document_status_id, ' Select Action' AS document_status from document_status UNION select document_status_id, document_status from document_status ", drpValidationStatus, "document_status", "document_status_id", My.Settings.strDSN)

                //load currency
                sharedUtility.LoadDropDownList(
                    drpCurrency,
                    genericFunctions.GetDropdownListInfo("currency", new string[] { "currency_id", "currency_name" }, null, null),
                    "currency_name",
                    "currency_id");
                //dnx.LoadDropDownListing("", "select '0' AS currency_id, ' Select Currency' AS currency_name from currency UNION select currency_id, currency_name from currency ", drpCurrency, "currency_name", "currency_id", My.Settings.strDSN)

                //load user branch
                sharedUtility.LoadDropDownList(
                    drpBranchs,
                    genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_name", "branch_id" }, null, null),
                    "branhc_name",
                    "branch_id");
                //dnx.LoadDropDownListing("", "select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch ", drpBranchs, "branch_name", "branch_id", My.Settings.strDSN)

                //load rm ro
                sharedUtility.LoadDropDownList(
                    drpRM,
                    genericFunctions.GetDropdownListInfo("relationship_managers", new string[] { "RM_ID", "RM_Name" }, null, null),
                    "RM_Name",
                    "RM_ID");
                //dnx.LoadDropDownListing("", "select '0' AS RM_ID, ' Select RM' AS RM_Name from relationship_managers UNION select RM_ID, RM_Name from relationship_managers ", drpRM, "RM_Name", "RM_ID", My.Settings.strDSN)


                //load instructions search
                sharedUtility.LoadDropDownList(
                    drpInstructions0,
                    genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, null, null),
                    "instruction_type",
                    "instruction_type_id");
                //dnx.LoadDropDownListing("", "select '0' AS instruction_type_id, ' All' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types ", drpInstructions0, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //load document status search
                sharedUtility.LoadDropDownList(
                    drpStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                    "document_status",
                    "document_status_id");
                //dnx.LoadDropDownListing("", "select '0' AS document_status_id, ' All' AS document_status from document_status UNION select document_status_id, document_status from document_status ", drpStatus, "document_status", "document_status_id", My.Settings.strDSN)

                //load check list of document_status
                sharedUtility.LoadCheckList(
                    chkBoxStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                    "document_status",
                    "document_status_id");
                //dnx.LoadCheckList("", " select document_status_id, document_status from document_status order by document_status", chkBoxStatus, "document_status", "document_status_id", My.Settings.strDSN)

                try
                {
                    load_all(Session["popup_sql"].ToString());
                }
                catch (Exception ex)
                {
                    erl.LogError("popUpView, PageLoad", ex.Message);
                }

            }

            Page.MaintainScrollPositionOnPostBack = true;
        }

        private void instruction_view_Init(object sender, EventArgs e) //Handles Me.Init
        {
            try
            {
                if (string.IsNullOrEmpty(Session["UserID"] as string))
                {
                    Response.Redirect("Login.aspx");
                }

            }
            catch (Exception ex)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void dgvInstructions_RowCommand(object sender, EventArgs e) //Handles dgvInstructions.RowCommand
        {
            string rw = e.ToString();
            string id = dgvInstructions.Rows[Int32.Parse(rw)].Cells[15].Text;

            load_all_details(id);
        }

        public void load_all_details(string instruction_id)
        {
            reset_client();
            Load_instruction_details(Int32.Parse(instruction_id));

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

        public void reset_client()
        {
            txtClient_Customer_Number.Text = "";
            txtClient_Name.Text = "";
            drpRM.SelectedIndex = -1;
        }

        public void reset_instructions()
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

            //'set default image
            ShowPdf1.FilePath = "new.pdf";

            //'Set default instruction
            if (Session["UserType"].Equals(1))
            {
                int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                drpValidationStatus.SelectedIndex = indx;
            }
        }

        public void Load_instruction_details(int ID)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("instructions_view", "*", "instruction_ID", ID.ToString());
            int indx = 0;

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

                    double n;
                    if (double.TryParse(txtAmount.Text, out n))
                    {
                        txtAmount.Text = n.ToString();
                    }

                    txtTransactionReference.Text = rdr["reference"].ToString();
                    txtFTROAComments.Text = rdr["ftroa_comments"].ToString();
                    txtFTROBComments.Text = rdr["ftrob_comments"].ToString();
                    txtProcessorComments.Text = rdr["processor_comments"].ToString();
                    txtRMComments.Text = rdr["rm_comments"].ToString();
                    txtFTRef.Text = rdr["ft_reference"].ToString();

                    indx = drpInstructions.Items.IndexOf(drpInstructions.Items.FindByValue(rdr["instruction_type_id"].ToString()));
                    drpInstructions.SelectedIndex = indx;

                    indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue(rdr["document_status_id"].ToString()));
                    drpValidationStatus.SelectedIndex = indx;

                    //??? dropdown
                    //dnx.LoadDropDownListing("", "select '0' AS Client_ID, ' Select Account' AS Client_Account_Number from client_details UNION select Client_ID, Client_Account_Number from client_details where Client_Customer_Number = '" & txtClient_Customer_Number.Text & "' ", drpAccount, "Client_Account_Number", "Client_ID", My.Settings.strDSN)
                    //sharedUtility.LoadDropDownList(drpAccount,,,)

                    indx = drpAccount.Items.IndexOf(drpAccount.Items.FindByText(rdr["account_no"].ToString()));
                    drpAccount.SelectedIndex = indx;

                    indx = drpCurrency.Items.IndexOf(drpCurrency.Items.FindByValue(rdr["currency_id"].ToString()));
                    drpCurrency.SelectedIndex = indx;

                    txtInstructionStatus.Text = rdr["instruction_status"].ToString();

                    lblLockedBy.Text = " Locked By: " + rdr["locked_by_name"].ToString();
                    lblLockedDate.Text = " Date: " + rdr["locked_date"].ToString();

                    indx = drpRM.Items.IndexOf(drpRM.Items.FindByValue(rdr["RM_ID"].ToString()));
                    drpRM.SelectedIndex = indx;

                    indx = drpBranchs.Items.IndexOf(drpBranchs.Items.FindByValue(rdr["branch_id"].ToString()));
                    drpBranchs.SelectedIndex = indx;

                    load_attachment();

                    if (Session["UserType"].ToString() == "2")
                    {
                        switch (txtStatus.Text)
                        {
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                                string branch_proccessed_by = rdr["branch_proccessed_by"].ToString();
                                int is_this_my_team_member = daccess.HowManyRecordsExist3Wheres("user_team_leader", "system_tl_1", Session["UserID"].ToString(), "system_user_id", branch_proccessed_by, "active", "1");
                                string primary_team_leader = daccess.RunStringReturnStringValue1Where("system_users", "system_tl_1", "system_user_id", branch_proccessed_by);
                                break;
                            default:
                                break;
                        }
                    }

                    //Enable or disable locking / unlocking
                    if (Session["UserType"].Equals(8))
                    {
                        double result = 0;
                        if (double.TryParse(rdr["locked_by"].ToString(), out result) && result > 0)
                        {
                            //btnUnlock.Enabled = true;
                            //btnUnlock.Visible = true;
                        }
                        else
                        {
                            //btnUnlock.Enabled = false;
                            //btnUnlock.Visible = false ;                       
                        }
                    }

                    //Pack / unpack . check if the status is PRMO, PRMO2 OR PRMOTL
                    if (Session["UserType"].Equals(15))
                    {
                        if (rdr["is_document_held"].ToString() == "1" && (rdr["Status"].ToString() == "3" || rdr["Status"].ToString() == "13" || rdr["Status"].ToString() == "14"))
                        {
                            //btnUnpack.Enabled = true;
                            //btnUnpack.Visible = true;
                        }
                        else
                        {
                            //btnUnpack.Enabled = false;
                            //btnUnpack.Visible = false;
                        }
                    }
                    else
                    {
                        //btnUnpack.Enabled = false;
                        //btnUnpack.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading instruction details by user " + Session["UserFullName"].ToString(), ex.Message);
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

        public void load_attachment()
        {
            //??? dropdown
            //dnx.LoadDropDownListing("", "select '0' AS attachment_id, ' Default' AS file_name from instructions_attachment UNION select attachment_ID,  file_name  from instructions_attachment where (deleted is null or deleted =0) and instruction_id = '" & txtInstructionID.Text & "' ", drpAttachments, "file_name", "attachment_id", My.Settings.strDSN);
            //sharedUtility.LoadDropDownList(drpAttachments, genericFunctions.GetDocumentStatusDropDownListInfo(), , );
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            load_all(Session["popup_sql"].ToString());
        } //pop up sql

        public void load_all(string sql)
        {
            try
            {
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                loadInstructions(sql);

                MultiView1.SetActiveView(ViewInstructions);
            }
            catch (Exception ex)
            {
                erl.LogError("loadall", ex.Message);
            }
        }

        public void loadInstructions(string sql)
        {

            if (chkMultiple.Checked)
            {
                string instruction_status = "";
                for (int i = 0; i < chkBoxStatus.Items.Count - 1; i++)
                {
                    string instruction_state = chkBoxStatus.Items[i].Value;
                    Boolean selected = chkBoxStatus.Items[i].Selected;

                    if (selected)
                    {
                        if (string.IsNullOrEmpty(instruction_status))
                        {
                            instruction_status = instruction_state;
                        }
                        else
                        {
                            instruction_status = instruction_status + " , " + instruction_state;
                        }
                    }

                }

                if (string.IsNullOrEmpty(instruction_status))
                {
                    alert.FireAlerts(this.Page, "Please Select atleast one status");
                    return;
                }
            }
            else
            {
                if (drpStatus.SelectedIndex > 0)
                {
                    //addSql = addSql & " AND instruction_status  = '" & drpStatus.SelectedValue & "' "
                }
            }

            try
            {
                //??? dgv
                //DataSet myDataset = dnx.getDataset(Sql, My.Settings.strDSN);

                //if (myDataset != null)
                //{
                //    dgvInstructions.DataSource = myDataset.Tables(0).DefaultView;
                //    dgvInstructions.DataBind();
                //}
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading instructions by user ", ex.Message);
                alert.FireAlerts(this.Page, "Error loading instructions by user");
            }
        }

        public Boolean proc_submit_and_allocate_instructions(int status, int user_type, int allocated_to, int document_status_id)
        {
            Constants c = new Constants();
            SqlCommand myCommand = null;
            bool proc_submit_and_allocate_instructions = false;
            try
            {

                using (SqlConnection myConnection = new SqlConnection(c.getConnectionString()))
                {


                    //??? My
                    //myConnection = new SqlConnection(My.Settings.strDSN);
                    myCommand = new SqlCommand("proc_submit_and_allocate_instructions", myConnection);
                    myCommand.CommandTimeout = 0;

                    myCommand.CommandType = CommandType.StoredProcedure;

                    int instruction_id = Convert.ToInt32(txtInstructionID.Text);
                    DateTime allocated_date = Convert.ToDateTime("1/1/1900");
                    string user_id = Session["UserID"].ToString();
                    int instruction_status = 25;

                    myCommand.Parameters.Add("@instruction_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@status", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_to", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@instruction_status", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_type", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_id", SqlDbType.Int);

                    myCommand.Parameters["@instruction_id"].Value = instruction_id;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@allocated_to"].Value = allocated_to;
                    myCommand.Parameters["@allocated_date"].Value = allocated_date;
                    myCommand.Parameters["@instruction_status"].Value = instruction_status;
                    myCommand.Parameters["@user_type"].Value = user_type;
                    myCommand.Parameters["@user_id"].Value = user_id;
                    myCommand.Parameters["@document_status_id"].Value = document_status_id;



                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    proc_submit_and_allocate_instructions = true;
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction submitted (" + drpValidationStatus.SelectedItem.Text + ") successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction submitted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Error submitting (" + drpValidationStatus.SelectedItem.Text + ") instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error submitting instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error submitting instruction with the following details", ex.Message);
                proc_submit_and_allocate_instructions = false;
            }
            //finally
            //{
            //    myConnection.Close();
            //}
            //myConnection.Dispose();
            //myCommand.Dispose();

            //myConnection = null;
            //myCommand = null;
            return proc_submit_and_allocate_instructions;

        }

        protected void chkMultiple_CheckedChanged(object sender, EventArgs e) //Handles chkMultiple.CheckedChanged
        {
            chkBoxStatus.Visible = chkMultiple.Checked;
            drpStatus.Enabled = (!chkMultiple.Checked);
        }

        public void unlock()
        {
            try
            {
                if (daccess.RunNonQueryEqualsSelect("Update", "instructions", new string[] { "locked_by" }, new string[] { "0" }, "locked_date", "instructions", "locked_date", "instruction_id", "-10", "instruction_id", txtInstructionID.Text))
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unlocked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction Unlocked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction UnLocked Successfully.");
                    load_all_details(txtInstructionID.Text);
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error Unlocking Instruction.");
            }
        }

        private Boolean export_to_excel(GridView GridView_in)
        {
            //export_grid_to_excel()
            string strFileName = "";
            string strFilePath = "";

            StringWriter oStringWriter = new StringWriter();
            HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=FileName.xls");
            Response.Charset = "";

            //'If you want the option to open the Excel file without saving then
            //comment out the line below
            //'Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.xls";

            // export the grid view
            GridView_in.RenderControl(oHtmlTextWriter);

            Response.Write(oStringWriter.ToString());
            Response.End();
            Response.Redirect(this.Page.Request.RawUrl.ToString(), true);

            return true;
        }

        protected void btnExport_Click(object sender, EventArgs e) //Handles btnExport.Click
        {
            export_to_excel(dgvInstructions);
        }

        public void unpack()
        {
            try
            {
                if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "status", "instruction_status", "document_status_id" }, new string[] { "15", "54", "54" }, "instruction_id", txtInstructionID.Text))
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unpacked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction Unpacked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction Unpacked Successfully.");
                    load_all_details(txtInstructionID.Text);
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, " Error Unlocking Instruction.");
                erl.LogError("Error Unlocking Instruction.", ex.Message);
            }
        }

        protected void btnView_Click1(object sender, EventArgs e)
        {
            load_all(Session["popup_sql"].ToString());
        }
    }
}