using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System.Data.OleDb;

namespace CIMS_V2.Instruction
{
    public partial class InstructionView : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        DAccessInfo daccess = new DAccessInfo();        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        protected void LoadPage()
        {
            try
            {

                SharedFunctions sharedFunctions = new SharedFunctions();
                GenericDbFunctions genericFunctions = new GenericDbFunctions();
                InstructionsInfo instructionsUtil = new InstructionsInfo();
                ReportUtilityInfo reportUtils = new ReportUtilityInfo();
                CIMS_Entities _db = new CIMS_Entities();
                //load the current instruction PDF or a place holder
                if (String.IsNullOrEmpty(txtFileName.Text))
                {
                    ShowPdf1.FilePath = "new.pdf";
                }
                else if (txtFileName.Text.Contains(".pdf"))
                {
                    ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                }
                else if (txtFileName.Text.Contains(".xls") || txtFileName.Text.Contains(".csv"))
                {
                    ShowPdf1.Visible = false;
                    excelGridView.Visible = false;
                    downloadButton.Visible = true;
                    
                }

                //clear lists
                drpInstructions.Items.Clear();
                drpBranchs.Items.Clear();
                drpSearchBy.Items.Clear();
                drpUserList.Items.Clear();

                dtmFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                dtmTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");


                //load instructions with TL
                sharedFunctions.LoadDropDownList(drpInstructions, instructionsUtil.GetInstructionsTypesDropDownListInfo( Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");

                //load search by customer listing 
                sharedFunctions.LoadDropDownList(
                    drpSearchBy,
                    genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }),
                    "search_by_name",
                    "search_by_value");

                
                //load instruction search
                sharedFunctions.LoadDropDownList(
                    drpSearchBy,
                    genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                    "search_by_name",
                    "search_by_value");

                //load currency list
                //sharedFunctions.LoadDropDownList(drpCurrency, genericFunctions.GetDropdownListInfo("currency", new string[] {"currency_id", "currency_name"}, null, null), "currency_id", "currency_name");

                //load currency
                GetNextInfo getNextInfo = new GetNextInfo();
                sharedUtility.LoadDropDownList(drpCurrency, getNextInfo.GetCurrencyDropDownListInfo(), "currency_name", "currency_id");

                //load user branches 
                sharedFunctions.LoadDropDownList(
                    drpBranchs,
                    genericFunctions.GetDropdownListInfo("user_branch", new string[] {"branch_name", "branch_id"}, null, null),
                    "branch_name",
                    "branch_id");

                //load duty of care comments
                sharedFunctions.LoadDropDownList(
                    drpDOC,
                    genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] {"doc_comments_id", "doc_comments"}, null, null),
                    "doc_comments",
                    "doc_comments_id");

                //load document status
                sharedFunctions.LoadDropDownList(
                    drpStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] {"document_status_id", "document_status"}, null, null),
                    "document_status",
                    "document_status_id");

                //load document validation status list
                sharedFunctions.LoadDropDownList(
                    drpValidationStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                    "document_status",
                    "document_status_id");

                sharedFunctions.LoadCheckList(
                    chkBoxStatus,
                    genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                    "document_status",
                    "document_status_id");

                //load instructionn search 
                //sharedFunctions.LoadDropDownList(drpInstructions0,genericFunctions.GetDropdownListInfo("instructions_types", new string[] {"instruction_type_id", "instruction_type"}, null, null),"instruction_type","instruction_type_id");
                sharedUtility.LoadDropDownList(drpInstructions0,genericFunctions.GetDropdownListInfo("instructions_types", new string[] {"instruction_type_id", "instruction_type" }, new string[] { "active" }, new string[] { "1" }), "instruction_type", "instruction_type_id");

                //load instruction
                sharedFunctions.LoadDropDownList(
                  drpInstructions,
                  genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, null, null),
                  "instruction_type",
                  "instruction_type_id");



                drpUserList.SelectedIndex = -1;
                drpInstructions.SelectedIndex = -1;
                drpBranchs.SelectedIndex = -1;
                drpCrossCurrency.SelectedIndex = -1;
                drpCurrency.SelectedIndex = -1;
                drpDOC.SelectedIndex = -1;
                drpInstructions0.SelectedIndex = -1;
                drpPriority.SelectedIndex = -1;
                drpRM.SelectedIndex = -1;
                drpSearchBy.SelectedIndex = -1;
                drpStatus.SelectedIndex = -1;
                drpUnpackStatuss.SelectedIndex = -1;
                drpValidationStatus.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ErrorLogging error = new ErrorLogging();
                error.LogError("LoadPage", ex.StackTrace);
            }

            Page.MaintainScrollPositionOnPostBack = true;
        }

        private void instruction_view_Init(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Session["UserID"] as string))
                {
                    Response.Redirect("Login.aspx");
                }
            }
            catch
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void dgvInstructions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt16(e.CommandArgument);
            int check = dgvInstructions.Rows.Count;
            string id = dgvInstructions.Rows[rw].Cells[29].Text;
            CIMS_Entities _db = new CIMS_Entities();
            int idVal = Convert.ToInt32(id);
            load_all_details(id);
            load_comments(id);

            if (can_I_recall(id))
            {
                btnRecall.Visible = true;
                btnRecall.Enabled = true;
            }
            else
            {
                btnRecall.Visible = false;
                btnRecall.Enabled = false;
            }

            if (_db.instructions_attachment.FirstOrDefault(i => i.instruction_id == idVal) != null)
            {
                excelGridView.Visible = false;
                downloadButton.Visible = true;
                //problems with importing excel files from the server because Microsoft. 


                //string excelFile = _db.instructions_attachment.FirstOrDefault(i => i.instruction_id == idVal).file_name;
                //string excelPath = Server.MapPath("~") + excelFile;
                //excelPath = excelPath.Replace("\\", "/");
                //try
                //{
                //    excelImport(excelPath);
                //}
                //catch (Exception ex)
                //{
                //    alert.FireAlerts(this.Page, "Error importing excel file " + ex.Message);
                //}
            }


        }

        public Boolean can_I_recall(string instruction_id)
        {
            string is_it_archived = daccess.RunStringReturnStringValue1Where("document_status", "is_archived", "document_status_id", txtDocumentStatusID.Text);
            string instruction_submission_by = daccess.RunStringReturnStringValue1Where("instruction_submission", "instruction_submission_by", "instruction_id", instruction_id);
            string instruction_picked_by = daccess.RunStringReturnStringValue1Where("instruction_picked", "instruction_picked_by", "instruction_id", instruction_id);
            string instruction_currently_picked_by = daccess.RunStringReturnStringValue1Where("instructions", "picked_by", "instruction_id", instruction_id);
            string instruction_inserted_by = daccess.RunStringReturnStringValue1Where("instructions", "inserted_by", "instruction_id", instruction_id);
            string branch_recall_allowed = daccess.RunStringReturnStringValue1Where("instructions_view", "branch_recall_allowed", "instruction_id", instruction_id);

            if (is_it_archived != "1" && instruction_submission_by == Session["UserID"].ToString() && instruction_picked_by == Session["UserID"].ToString() && instruction_currently_picked_by != Session["UserID"].ToString())
            {
                return true;
            }
            else
            {
                if (is_it_archived != "1" && (Session["UserType"].ToString() == "1" || Session["UserType"].ToString() == "3") && instruction_inserted_by == Session["UserID"].ToString() && branch_recall_allowed == "1" && (instruction_currently_picked_by == "" || instruction_currently_picked_by == "0"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


        }

        public void load_comments(string id)
        {
            //string sql, addSql = "";


            //sql = " SELECT * From instructions_comments_view " +
            //  " Where instruction_id = '" + id;

            try
            {
                //??? sharedUtility - no getdataset method
                //DataSet myDataset = sharedUtility.getDataset(sql, My.Settings.strDSN);

                sharedUtility.LoadGridView(dgvComments, genericFunctions.GetDataSourceUserGridViewInfo("instructions_comments_view", "instruction_id", id));
                /*if (myDataset != null)
                {
                    dgvComments.DataSource = myDataset.Tables[0].DefaultView;
                    dgvComments.DataBind();
                }*/

            }
            catch (Exception ex)
            {
                erl.LogError("Error loading requirements by user ", ex.Message);
                alert.FireAlerts(this.Page, "Error loading requirements.");
            }
        }

        public void load_all_details(string instruction_id)
        {
            CIMS_Entities _db = new CIMS_Entities();
            SharedFunctions sharedFunctions = new SharedFunctions();
            reset_client();
            Load_instruction_details(Int32.Parse(instruction_id));
            int idVal = Convert.ToInt32(instruction_id);
            instruction currentInstruction = _db.instructions.FirstOrDefault(i => i.instruction_id == idVal);
            string fl_name = currentInstruction.file_name; 

            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else if (fl_name.Contains(".pdf"))
            {
                if (txtFileName.Text.Contains("instructions/"))
                {
                    ShowPdf1.FilePath = txtFileName.Text;
                    //ShowPdf1.Width = 567;
                }
                else
                {
                    ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                    //ShowPdf1.Width = 567;

                }
            }
            //get user type value
            string userType = currentInstruction.status.ToString();

            //get all users of that type with that instruction type allocation.
            string instructionTypeId = currentInstruction.instruction_type_id.ToString();

            try
            {

                var users = from u in _db.system_users
                            where u.system_user_type == currentInstruction.status &
                            u.system_user_active == 1 & u.system_user_status == 1 &
                            u.instruction_type_allocations.FirstOrDefault(i => i.instruction_type_id == currentInstruction.instruction_type_id).instruction_type_id == currentInstruction.instruction_type_id
                            select u;

                foreach (system_users allocatedUser in users)
                {
                    ListItem item = new ListItem();
                    item.Value = allocatedUser.system_user_id.ToString();
                    item.Text = allocatedUser.system_user_fname + " " + allocatedUser.system_user_lname;
                    drpUserList.Items.Add(item);
                }

            }
            catch(Exception ex)
            {
                alert.FireAlerts(this.Page, "Couldn't populate the user list dropdown" + ex.Message);

            }
            //sharedFunctions.LoadDropDownList(
            //    drpUserList,
            //    genericFunctions.GetDropdownListInfo("system_users", new string[] { "system_user_id", "system_user_fname", "system_user_lname" }, new string[] { "system_user_type", "system_user_active" }, new string[] { userType, "1" }),
            //    "system_user_fname",
            //    "system_user_id");
            
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

            drpCrossCurrency.SelectedIndex = -1;
            txtCCRate.Text = "";

            //set default image
            ShowPdf1.FilePath = "new.pdf";

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

                    //??? dropdownlist query
                    //load users to allocate to
                    sharedUtility.LoadDropDownList(
                        drpAllocatedTo,
                        genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "user_type_no", "system_user_id IN (SELECT system_user_id FROM instruction_type_allocations WHERE instruction_type_id", "instruction_type_allocations.status" }, new string[] { rdr["Status"].ToString(), rdr["instruction_type_id"].ToString(), "1)" }),
                        "names",
                        "system_user_id");
                    //sharedUtility.LoadDropDownList(drpAllocatedTo, "select '0' AS system_user_id, ' No One' AS names from system_users_view UNION select system_user_id, names from system_users_view where user_type_no = '" + rdr["Status"].ToString() + "' AND system_user_id IN (SELECT system_user_id From instruction_type_allocations Where  instruction_type_id = '" + rdr["instruction_type_id"].ToString() + "' AND instruction_type_allocations.status = 1 ) ", "names", "system_user_id");
                
                    // 'load document status
                    sharedUtility.LoadDropDownList(
                        drpUnpackStatuss,
                        genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_action", "document_status_id" }, new string[] { "document_status_user_type_who_can_action", "is_document_held" }, new string[] { Session["UserType"].ToString(), "-1" }),
                        "document_status_action",
                        "document_status_id");

                    //"select '0' AS document_status_id, ' Select Action' AS document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND is_document_held = -1 "


                    txtDocumentStatusID.Text = rdr["document_status_id"].ToString();
                    txtClient_Name.Text = rdr["Client_Name"].ToString();
                    txtClient_Customer_Number.Text = rdr["Client_Customer_Number"].ToString();
                    txtClientID.Text = rdr["Client_ID"].ToString();

                    txtFileName.Text = rdr["file_name"].ToString();
                    txtComments.Text = rdr["Comments"].ToString();
                    txtInstructionID.Text = rdr["Instruction_ID"].ToString();
                    txtAmount.Text = rdr["Amount"].ToString();

                    double result = 0;
                    if (double.TryParse(txtAmount.Text, out result))
                    {
                        txtAmount.Text = Convert.ToDouble(txtAmount.Text).ToString("N2");
                    }

                    txtTransactionReference.Text = rdr["reference"].ToString();
                    txtProcessorComments.Text = rdr["processor_comments"].ToString();
                    txtRMComments.Text = rdr["rm_comments"].ToString();
                    txtPRMO1Commentss.Text = rdr["ftroa_comments"].ToString();
                    txtPRMO2Commentss.Text = rdr["prmo2_comments"].ToString();
                    txtPRMOTLCommentss.Text = rdr["ftrob_comments"].ToString();
                    txtProcessorComments.Text = rdr["processor_comments"].ToString();
                    txtRMComments.Text = rdr["rm_comments"].ToString();
                    txtFTRef.Text = rdr["ft_reference"].ToString();

                    indx = drpInstructions.Items.IndexOf(drpInstructions.Items.FindByValue(rdr["instruction_type_id"].ToString()));
                    drpInstructions.SelectedIndex = indx;
                    indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue(rdr["document_status_id"].ToString()));
                    drpValidationStatus.SelectedIndex = indx;

                    //??? dropdownlist query
                    //sharedUtility.LoadDropDownList(drpAccount, "select '0' AS Client_ID, ' Select Account' AS Client_Account_Number from client_details UNION select Client_ID, Client_Account_Number from client_details where Client_Customer_Number = '" + txtClient_Customer_Number.Text + "' ", "Client_Account_Number", "Client_ID");
                    sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetDropdownListInfo("client_details", new string[] { "Client_ID", "Client_Account_Number" }, new string[] { "Client_Customer_Number" }, new string[] { txtClient_Customer_Number.Text }), "Client_Account_Number", "Client_ID");

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

                    indx = drpPriority.Items.IndexOf(drpPriority.Items.FindByText(rdr["manual_priority"].ToString()));
                    drpPriority.SelectedIndex = indx;

                    indx = drpDOC.Items.IndexOf(drpDOC.Items.FindByValue(rdr["doc_comments_id"].ToString()));
                    drpDOC.SelectedIndex = indx;

                    txtCallBackComment.Text = rdr["call_back_comments"].ToString();

                    txtCallBackNos.Text = rdr["call_back_no"].ToString();

                    DateTime date;
                    if (DateTime.TryParse(rdr["delivery_date"].ToString(), out date))
                    {
                        txtDeliveryDat.Text = rdr["delivery_date"].ToString();
                    }

                    load_attachment();

                    if (Session["CanChangePriority"].Equals(1))
                    {
                        drpPriority.Enabled = true;
                        btnChangePriority.Enabled = true;
                    }
                    else
                    {
                        drpPriority.Enabled = false;
                        btnChangePriority.Enabled = false;
                    }


                    if (Session["UserType"].Equals(2))
                    {
                        switch (txtStatus.Text)
                        {
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                                string branch_proccessed_by = rdr["branch_proccessed_by"].ToString();
                                int is_this_my_team_member = daccess.HowManyRecordsExist3Wheres("user_team_leader", "system_tl_1", Session["UserID"].ToString(), "system_user_id", branch_proccessed_by, "active", "1");
                                string primarry_team_leader = daccess.RunStringReturnStringValue1Where("system_users", "system_tl_1", "system_user_id", branch_proccessed_by);

                                if (is_this_my_team_member > 0 || primarry_team_leader == Session["UserID"].ToString())
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

                    if (Session["UserType"].Equals(8))
                    {
            
                        drpUserList.Visible = true;
                        Allocation.Visible = true;
                        Allocation.Enabled = true;
                        AllocationLbl.Visible = true;
                    }

                    double resultout = 0;
                    double resultout2 = 0;
                    double resultout3 = 0;
                    if ((double.TryParse(rdr["referred_to"].ToString(), out resultout) && result > 0) || (double.TryParse(rdr["picked_by"].ToString(), out resultout2) && resultout2 > 0) || (double.TryParse(rdr["locked_by"].ToString(), out resultout3) && resultout3 > 0))
                    {
                        btnUnlock.Enabled = true;
                        btnUnlock.Visible = true;
                    }
                    else
                    {
                        btnUnlock.Enabled = false;
                        btnUnlock.Visible = false;
                    }


                    if (Session["UserType"].Equals(15))
                    {
                        if (rdr["is_document_held"].ToString() == "1" && (rdr["Status"].ToString() == "3" || rdr["Status"].ToString() == "13" || rdr["Status"].ToString() == "14"))
                        {
                            btnUnpack.Enabled = true;
                            btnUnpack.Visible = true;
                        }
                    }
                    else
                    {
                        btnUnpack.Enabled = false;
                        btnUnpack.Visible = false;
                    }

                    //if (Session["UserCanUnpack"].Equals(1))
                    //{
                    //    double outresult;
                    //    if (rdr["is_document_held"].ToString() == "1" || (double.TryParse(rdr["picked_by"].ToString(), out outresult) && outresult > 0))
                    //    {
                    //        btnUnpackAndAllocate.Enabled = true;
                    //        btnUnpackAndAllocate.Visible = true;
                    //        drpAllocatedTo.Enabled = true;
                    //        drpAllocatedTo.Visible = true;
                    //        drpUnpackStatuss.Enabled = true;
                    //        drpUnpackStatuss.Visible = true;
                    //    }
                    //    else
                    //    {
                    //        btnUnpackAndAllocate.Enabled = false;
                    //        btnUnpackAndAllocate.Visible = false;
                    //        drpAllocatedTo.Enabled = false;
                    //        drpAllocatedTo.Visible = false;
                    //        drpUnpackStatuss.Enabled = false;
                    //        drpUnpackStatuss.Visible = false;
                    //    }
                    //}
                    //else
                    //{
                    //    btnUnpackAndAllocate.Enabled = false;
                    //    btnUnpackAndAllocate.Visible = false;
                    //    drpAllocatedTo.Enabled = false;
                    //    drpAllocatedTo.Visible = false;
                    //    drpUnpackStatuss.Enabled = false;
                    //    drpUnpackStatuss.Visible = false;
                    //}

                    double outresult1 = 0;
                    double outresult2 = 0;
                    double outresult3 = 0;
                    if ((double.TryParse(rdr["referred_to"].ToString(), out outresult1) && outresult1 > 0) || (rdr["picked_by"].ToString() != Session["UserID"].ToString() && double.TryParse(rdr["picked_by"].ToString(), out outresult2) && outresult2 > 0) || (rdr["locked_by"].ToString() != Session["UserID"].ToString() && double.TryParse(rdr["locked_by"].ToString(), out outresult3) && outresult3 > 0))
                    {
                        btnTakeOver.Visible = true;
                        if (rdr["Status"].ToString() == Session["UserType"].ToString())
                        {
                            btnTakeOver.Visible = false;
                        }
                        else
                        {
                            btnTakeOver.Visible = false;
                        }
                    }
                    else
                    {
                        btnTakeOver.Visible = false;
                    }

                    indx = drpCrossCurrency.Items.IndexOf(drpCrossCurrency.Items.FindByValue(rdr["cross_currency"].ToString()));


                    drpCrossCurrency.SelectedIndex = indx;
                    txtCCRate.Text = rdr["cross_currency_rate"].ToString();
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
                    else
                    {
                        rdr = null;
                    }
                }
            }
        }

        public void load_attachment()
        {
            //??? dropdownlist query
            //sharedUtility.LoadDropDownList(drpAttachments, "select '0' AS attachment_id, ' Default' AS file_name from instructions_attachment UNION select attachment_ID,  file_name  from instructions_attachment where (deleted is null or deleted =0) and instruction_id = '" & txtInstructionID.Text & "' ", "file_name", "attachment_id");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                loadInstructions();

                MultiView1.SetActiveView(ViewCustomerList);
            }
            catch (Exception ex)
            {
                erl.LogError("btnViewClick", ex.Message);
            }
        }

        public void loadInstructions()
        {
            string instruction_status = "";

            if (chkMultiple.Checked) //shows all the checkboxes for differe
            {

                for (int i = 0; i < chkBoxStatus.Items.Count - 1; i++)
                {
                    string instruction_state = chkBoxStatus.Items[i].Value;
                    bool selected = chkBoxStatus.Items[i].Selected;

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

            try
            {
                if (chkShowComments.Checked && drpInstructions0.SelectedIndex > 0 && drpStatus.SelectedIndex > 0)
                {

                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.getDataTableWithFn(drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "instruction_status", drpStatus.SelectedValue, "Instruction_type_id", drpInstructions0.SelectedValue));

                       //"Select instructions_view.* , [dbo].[fn_get_instruction_comments](instruction_id) As instruction_comments From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  ";
                    //"AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //AND instruction_status = '" + drpStatus.SelectedValue + "'
                    //AND Instruction_type_id = '" + drpInstructions0.SelectedValue +

                    //    order by instruction_id

                }
                else if (chkShowComments.Checked && drpInstructions0.SelectedIndex > 0)
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.getDataTableWithFn(drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "Instruction_type_id", drpInstructions0.SelectedValue, null, null));

                    //"Select instructions_view.* , [dbo].[fn_get_instruction_comments](instruction_id) As instruction_comments From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  ";
                    //"AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //AND Instruction_type_id = '" + drpInstructions0.SelectedValue +
                    //    order by instruction_id
                }
                else if (chkShowComments.Checked && drpStatus.SelectedIndex > 0)
                {
                   
                   sharedUtility.LoadGridView(dgvInstructions, genericFunctions.getDataTableWithFn(drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "instruction_status", drpStatus.SelectedValue, null, null));
   
                    //"Select instructions_view.* , [dbo].[fn_get_instruction_comments](instruction_id) As instruction_comments From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  ";
                    //"AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"


                    //AND instruction_status = '" + drpStatus.SelectedValue + "'


                    //    order by instruction_id
                }
                else if (drpStatus.SelectedIndex > 0 && drpInstructions0.SelectedIndex >= 0)
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "inserted_date",
                    "instruction_status", instruction_status, "instruction_status", drpStatus.SelectedValue, "Instruction_type_id", drpInstructions0.SelectedValue));

                    //"Select instructions_view.* , '' As instruction_comments From instructions_view Where " +
                    //     drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  "AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //    order by instruction_id
                    //AND instruction_status = '" + drpStatus.SelectedValue + "'
                    //AND Instruction_type_id = '" + drpInstructions0.SelectedValue +

                }
                else if (drpStatus.SelectedIndex > 0)
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "inserted_date",
                    "instruction_status", instruction_status, "instruction_status", drpStatus.SelectedValue, null, null));

                    //"Select instructions_view.* , '' As instruction_comments From instructions_view Where " +
                    //    drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  "AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //    AND instruction_status = '" + drpStatus.SelectedValue + "'
                }
                else if (chkShowComments.Checked)
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "inserted_date", 
                    "instruction_status", instruction_status, "instruction_status", drpStatus.SelectedValue, null, null));

                    //"Select instructions_view.* , '' As instruction_comments From instructions_view Where " +
                    //    drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  "AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //    AND instruction_status = '" + drpStatus.SelectedValue + "'


                }
                else if (drpInstructions0.SelectedIndex > 0 && drpInstructions0.SelectedValue != "13")
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "inserted_date", 
                        "instruction_status", instruction_status, 
                        "instruction_status", drpStatus.SelectedValue, "Instruction_type_id", drpInstructions0.SelectedValue));

                    //"Select instructions_view.* , '' As instruction_comments From instructions_view Where " +
                    //drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  "AND
                    //    (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"

                    //    AND instruction_status = '" + drpStatus.SelectedValue + "'
                    //    AND Instruction_type_id = '" + drpInstructions0.SelectedValue +
                }
                else if(drpInstructions0.SelectedValue == "13")
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions", "", "", dtmFrom.Text, dtmTo.Text, "inserted_date",
                        "instruction_type_id", drpInstructions0.SelectedValue, null, null, null, null));
                }
                else
                {
                    sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text, "inserted_date", 
                        "instruction_status", instruction_status, null, null, null , null));
                    //"Select instructions_view.* , '' As instruction_comments 
                    // From instructions_view Where " + 
                    //    drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  
                    //   "AND (DateDiff(Day, inserted_date, '" + dtmFrom.Text + "') <= 0  
                    //    AND DateDiff(Day, inserted_date, '" + dtmTo.Text + "') >= 0)"
                    //    AND instruction_status  IN(" + instruction_status + ")"
                    //    order by instruction_id

                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading instructions. ");
                erl.LogError("Error loading instructions.", ex.Message);
            }
        }

        protected void btnRecall_Click(object sender, EventArgs e)
        {
            if (can_I_recall(txtInstructionID.Text))
            {
                string instruction_submission_id = daccess.RunStringReturnStringValue1Where("instruction_submission", "instruction_submission_id", "instruction_id", txtInstructionID.Text);
                string document_status_id = daccess.RunStringReturnStringValue3Wheres("instruction_submission", "document_status_id", "instruction_id", txtInstructionID.Text, "instruction_submission_id", instruction_submission_id, "instruction_submission_status", Session["UserType"].ToString());
                string instruction_submission_status = daccess.RunStringReturnStringValue3Wheres("instruction_submission", "instruction_submission_status", "instruction_id", txtInstructionID.Text, "instruction_submission_id", instruction_submission_id, "instruction_submission_status", Session["UserType"].ToString());
                string s = Session["UserType"].ToString();

                if (s == instruction_submission_status)
                {
                    string cc = daccess.RunStringReturnStringValue1Where("system_users", "system_user_email", "system_user_id", Session["UserID"].ToString());
                    //???
                    //string file_ = My.Settings.instructions_location + "\ " + txtFileName.Text;

                    if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "status", "document_status_id", "instruction_status", "referred_to", "locked_by", "picked_by" }, new string[] { instruction_submission_status, document_status_id, document_status_id, "0", "0", Session["UserID"].ToString() }, "instruction_id", txtInstructionID.Text))
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction recalled successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction recalled", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        alert.FireAlerts(this.Page, "Instruction Recalled Successfully.");

                        try
                        {
                            string comments = "  \r\n" +
                                  " <table border=1> <tr> \r\n" +
                                  " 	<td>Client Name</td><td>" + txtClient_Name.Text + "</td>" +
                                  " </tr>" +
                                   "<tr>" +
                                  " 	<td>Instruction Type</td><td>" + drpInstructions.SelectedItem.Text + "</td>" + Environment.NewLine +
                                 " </tr>" +
                                  "<tr>" + Environment.NewLine +
                                 " 	<td>Instruction Date</td><td>" + txtDeliveryDat.Text + "</td>" + Environment.NewLine +
                                 " </tr></table>";

                            //??? sharedUtility - no sendmail method
                            //sharedUtility.SendMail(cc, "", "INSTRUCTION RECALLED", build_takeover_mail_header() & build_recalled_mail_body(comments) & build_takeover_mail_footer(), "", "high", file_, "html");
                            

                        }
                        catch
                        {
                            //??? log error?
                        }
                    }
                    else
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Changing Priority for instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error Taking Over Instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        alert.FireAlerts(this.Page, "Error Recalling Instruction.");
                    }

                    if (can_I_recall(txtInstructionID.Text))
                    {
                        btnRecall.Visible = true;
                        btnRecall.Enabled = true;
                    }
                    else
                    {
                        btnRecall.Visible = false;
                        btnRecall.Enabled = false;
                    }
                }
                else if (s == "1")
                {
                    string query = "Update instructions set status = '1', document_status_id = '98', instruction_status = '98' ,referred_to = '0', locked_by = '0', picked_by = '0'  Where instruction_id = '" + txtInstructionID.Text + "' ";
                    string cc = daccess.RunStringReturnStringValue1Where("system_users", "system_user_email", "system_user_id", Session["UserID"].ToString());
                    //???
                    //string file = My.Settings.instructions_location + "\ " + txtFileName.Text;

                    if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "status", "document_status_id", "instruction_status", "referred_to", "locked_by", "picked_by" }, new string[] { "1", "98", "98", "0", "0", "0" }, "instruction_id", txtInstructionID.Text))
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction recalled successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction recalled", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        alert.FireAlerts(this.Page, "Instruction recalled successfully");

                        try
                        {
                            string comments = "  " + Environment.NewLine +
                            " <table border=1> <tr> " + Environment.NewLine +
                            " 	<td>Client Name</td><td>" + txtClient_Name.Text + "</td>" + Environment.NewLine +
                            " </tr>" +
                             "<tr>" + Environment.NewLine +
                            " 	<td>Instruction Type</td><td>" + drpInstructions.SelectedItem.Text + "</td>" + Environment.NewLine +
                            " </tr>" +
                             "<tr>" + Environment.NewLine +
                            " 	<td>Instruction Date</td><td>" + txtDeliveryDat.Text + "</td>" + Environment.NewLine +
                            " </tr></table>";

                            //??? sharedUtility - no send mail method
                            //sharedUtility.SendMail(cc, "", "INSTRUCTION RECALLED", build_takeover_mail_header() & build_recalled_mail_body(comments) & build_takeover_mail_footer(), "", "high", file, "html");
                        }
                        catch
                        {
                            //??? log error?
                        }
                    }
                    else
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Changing Priority for instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error taking over instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        alert.FireAlerts(this.Page, "Error recalling instruction");
                    }

                    if (can_I_recall(txtInstructionID.Text))
                    {
                        btnRecall.Visible = true;
                        btnRecall.Enabled = true;
                    }
                    else
                    {
                        btnRecall.Visible = false;
                        btnRecall.Enabled = false;
                    }
                }
                else
                {
                    alert.FireAlerts(this.Page, "Recall is not possible. Error in the instruction status");
                }
            }
            else
            {
                alert.FireAlerts(this.Page, "Recall is not possible. Maybe the instruction status has changed.");
                btnRecall.Enabled = false;
            }
        }

        public Boolean proc_submit_and_allocate_instructions(int status, int user_type, int allocated_to, int document_status_id)
        {

            //SqlConnection myConnection;
            //SqlCommand myCommand;
            Boolean procSubmit = false;

            //??? all below commented out because of the "My" in myConnection - fix that fix it all
            //myConnection = new SqlConnection(My.Settings.strDSN);
            //myCommand = new SqlCommand("proc_submit_and_allocate_instructions", myConnection);
            //myCommand.CommandTimeout = 0;

            //myCommand.CommandType = CommandType.StoredProcedure;

            //int instruction_id = Convert.ToInt32(txtInstructionID.Text);
            //DateTime allocated_date = Convert.ToDateTime("1-1-1900");
            //string user_id = Session["UserID"].ToString();
            //int instruction_status = 25;

            //myCommand.Parameters.Add("@instruction_id", SqlDbType.Int);
            //myCommand.Parameters.Add("@status", SqlDbType.Int);
            //myCommand.Parameters.Add("@allocated_to", SqlDbType.Int);
            //myCommand.Parameters.Add("@allocated_date", SqlDbType.DateTime);
            //myCommand.Parameters.Add("@instruction_status", SqlDbType.Int);
            //myCommand.Parameters.Add("@user_type", SqlDbType.Int);
            //myCommand.Parameters.Add("@user_id", SqlDbType.Int);
            //myCommand.Parameters.Add("@document_status_id", SqlDbType.Int);

            //myCommand.Parameters["@instruction_id"].Value = instruction_id;
            //myCommand.Parameters["@status"].Value = status;
            //myCommand.Parameters["@allocated_to"].Value = allocated_to;
            //myCommand.Parameters["@allocated_date"].Value = allocated_date;
            //myCommand.Parameters["@instruction_status"].Value = instruction_status;
            //myCommand.Parameters["@user_type"].Value = user_type; ;
            //myCommand.Parameters["@user_id"].Value = user_id;
            //myCommand.Parameters["@document_status_id"].Value = document_status_id;

            try
            {
                //myConnection.Open();
                //myCommand.ExecuteNonQuery();
                //procSubmit = true;
                //operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction submitted (" + drpValidationStatus.SelectedItem.Text + ") successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction submitted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);

            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error submitting (" + drpValidationStatus.SelectedItem.Text + ") instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error submitting instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error submitting instruction with the following details ", ex.Message);
                procSubmit = false;
            }
            finally
            {
                //myConnection.Close();
            }

            //myConnection.Dispose();
            //myCommand.Dispose();

            //myConnection = null;
            //myCommand = null;
            return procSubmit;
        }

        protected void chkMultiple_CheckedChanged(object sender, EventArgs e)
        {
            chkBoxStatus.Visible = chkMultiple.Checked;
            drpStatus.Enabled = !chkMultiple.Checked;
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            unlock();
        }

        public void unlock()
        {
            try
            {
                if (daccess.RunNonQueryEqualsSelect("Update", "instructions", new string[] { "locked_by", "picked_by" }, new string[] { "0", "0" }, "locked_date", "instructions", "locked_date", "instruction_id", "-10", "instruction_id", txtInstructionID.Text))
                {
                    //operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unlocked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction unlocked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction UnLocked Successfully.");
                    load_all_details(txtInstructionID.Text);
                }
                else
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Unlocking Instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error unlocking", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Error unlocking instruction");
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error Unlocking Instruction.");
                erl.LogError("Error Unlocking Instruction.", ex.Message);
            }
        }

        private Boolean export_to_excel(GridView GridView_in)
        {

            StringWriter oStringWriter = new StringWriter();
            HtmlTextWriter oHtmlTextWriter = new HtmlTextWriter(oStringWriter);

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=FileName.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.xls";
            GridView_in.RenderControl(oHtmlTextWriter);

            Response.Write(oStringWriter.ToString());
            Response.End();
            Response.Redirect(this.Page.Request.RawUrl.ToString(), true);
            return true;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
           // export_to_excel(dgvInstructions);
        }

        public void unpack()
        {
            try
            {
                if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "status", "instruction_status", "instruction_status_id" }, new string[] { "15", "54", "54" }, "instruction_id", txtInstructionID.Text))
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unpacked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction unpacked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Unpacked successfully");
                    load_all_details(txtInstructionID.Text);
                }
                else
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Unpacking Instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error unpacking", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Error Unpacking Instruction.");
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error Unpacking Instruction.");
                erl.LogError("Error Unpacking Instruction.", ex.Message);
            }
        }

        public void unpack_and_allocateto(string allocate_to)
        {
            if (drpUnpackStatuss.SelectedIndex <= 0)
            {
                alert.FireAlerts(this.Page, "Please Select the Unpack Status");
                return;
            }

            try
            {
                if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "locked_by", "instruction_status", "document_status_id", "picked_by", "allocated_to", "allocated_date", "allocated_by" }, new string[] { "0", drpUnpackStatuss.SelectedValue, drpUnpackStatuss.SelectedValue, "0", allocate_to, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString() }, "instruction_id", txtInstructionID.Text))
                {
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction Unpacked and Allocated To " + drpAllocatedTo.SelectedItem.Text + " with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction unpacked and allocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    alert.FireAlerts(this.Page, "Instruction Unpacked and Allocated To " + drpAllocatedTo.SelectedItem.Text + " Successfully.");
                    load_all_details(txtInstructionID.Text);
                }
                else
                {
                    alert.FireAlerts(this.Page, "Error Unpacking and Allocating Instruction ");
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error Unpacking and Allocating Instruction ");
                erl.LogError("Error Unpacking and Allocating Instruction ", ex.Message);
            }
        }

        protected void btnUnpack_Click(object sender, EventArgs e)
        {
            unpack();
        }

        protected void btnChangePriority_Click(object sender, EventArgs e)
        {
            if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "manual_priority", "manual_priority_by", "manual_priority_date" }, new string[] { drpPriority.SelectedItem.Text, Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", txtInstructionID.Text))
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Changed Priority for instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Changed priority", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "Priority Changed Successfully.");
            }
            else
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Changing Priority for instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error changing priority", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "Error Changing Priority.");
            }
        }

        public string get_user_who_locked_email(string instruction_id)
        {
            string user_email = "";
            user_email = daccess.RunStringReturnStringValueIN("system_users", "system_user_email", "system_user_id", "instructions", "referred_to", "instruction_id", instruction_id);

            //??? sharedUtility - missing method
            //if (!sharedUtility.is_this_a_valid_email_adress(user_email))
            //{
            //    user_email = daccess.RunStringReturnStringValueIN("system_users", "system_user_email", "system_user_id", "instructions", "picked_by", "instruction_id", instruction_id);
            //}

            return user_email;
        }

        protected void btnUnpackAndAllocate_Click(object sender, EventArgs e)
        {
            unpack_and_allocateto(drpAllocatedTo.SelectedValue);
        }

        public void openpopup(string link)
        {
            StringBuilder scriptString = new StringBuilder();
            scriptString.Append("<script>");
            scriptString.Append("window.open('" + link + "', '', 'toolbar=no,status=no,menubar=no,location=center,scrollbars=yes,resizable=yes,Height=600,Width=1000');");
            scriptString.Append("</script>");

            ClientScript.RegisterStartupScript(this.GetType(), "test", scriptString.ToString());
            //Page.RegisterStartupScript("test", scriptString.ToString());
        }

        protected void btnPreviewFull_Click(object sender, EventArgs e)
        {
            Session["instruction_path"] = "instructions/" + txtFileName.Text;
            openpopup("instructions_preview2.aspx");
        }

        protected void btnTakeOver_Click(object sender, EventArgs e)
        {
            string receipient = get_user_who_locked_email(txtInstructionID.Text);
            string cc = daccess.RunStringReturnStringValue1Where("system_users", "system_user_email", "system_user_id", Session["UserID"].ToString());
            //???
            //string file = My.Settings.instructions_location + "\ " + txtFileName.Text;

            if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "referred_to", "locked_by", "picked_by" }, new string[] { "0", "0", Session["UserID"].ToString() }, "instruction_id", txtInstructionID.Text))
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction taken over successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction taken over", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "Instruction Taken Over Successfully.");

                try
                {
                    string comments = "  \r\n" +
                    " <table border=1> <tr> \r\n" +
                    " 	<td>Client Name</td><td>" + txtClient_Name.Text + "</td>  \r\n" +
                    " </tr>" +
                     "<tr>\r\n" +
                        " 	<td>Instruction Type</td><td>" + drpInstructions.SelectedItem.Text + "</td> \r\n" +
                    " </tr> \r\n" +
                     "<tr> \r\n" +
                    " 	<td>Instruction Date</td><td>" + txtDeliveryDat.Text + "</td> \r\n" +
                    " </tr></table>";

                    //??? sharedUtility - no SendMail method
                    //sharedUtility.SendMail(cc, "", "INSTRUCTION TAKEN OVER", build_takeover_mail_header() + build_takeover_mail_body(comments) + build_takeover_mail_footer(), receipient, "high", file, "html");
                }
                catch (Exception ex)
                {
                    erl.LogError("btnTakeOver_Click", ex.Message);
                }
            }
            else
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error Changing Priority for instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error taking over instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "Error Taking Over Instruction.");
            }
        }

        public string build_takeover_mail_body(string comments)
        {
            string myString = "";
            myString = " <table><br> <tr>" + Environment.NewLine +
                        " 	<td>Please note that the following instruction has been taken over by " + Session["UserFullName"].ToString() + ".<br> <br>See details below<br> <br>" + comments + "<br> <br>" + Environment.NewLine +
                        " </tr></table>";

            return myString;
        }

        public string build_recalled_mail_body(string comments)
        {
            string myString = "";
            myString = " <table><br> <tr> " +
                            " 	<td>Please note that the following instruction has been recalled by " + Session["UserFullName"].ToString() + ".<br> <br>See details below<br> <br>" + comments + "<br> <br>" + Environment.NewLine +
                            " </tr></table>";
            return myString;
        }

        public string build_takeover_mail_footer()
        {
            string myString = " ";
            myString = " <table><tr> " +
                        " 	<td>Kind Regards,<br> <br>CIMS. &nbsp;" +
                        " </tr></table> ";
            return myString;
        }

        public string build_takeover_mail_header()
        {
            string myString = "";
            myString = " <table> <br> <tr> \r\n" + " 	<td>Dear Team,\r\n" + " </tr>  <table>";

            return myString;
        }

        protected void chkShowComments_CheckedChanged(object sender, EventArgs e)
        {
            btnView_Click(null, null);
        }

        protected void btnOpen_Click(object sender, EventArgs e)
        {
            //???
            //string filename = My.Settings.instructions_location + "\ " + drpAttachments.SelectedItem.Text;
            //FileStream fs = File.Open(filename, FileMode.Open);

            //Byte[] btFile = new Byte[fs.Length - 1];
            //fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            //fs.Close();
            //Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
            //Response.ContentType = "application/octet-stream";
            //Response.BinaryWrite(btFile);
            //Response.End();
        }

        protected void drpAttachments_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOpen.Visible = false;

            if (drpAttachments.SelectedItem.Text.Contains(".pdf"))
            {
                if (drpAttachments.SelectedIndex.Equals(0))
                {
                    ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                }
                else
                {
                    ShowPdf1.FilePath = "instructions/" + drpAttachments.SelectedItem.Text;
                }

            }
            else
            {
                if (drpAttachments.SelectedIndex.Equals(0))
                {
                    ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                }
                else
                {
                    btnOpen.Visible = true;
                }
            }

        }
        protected void drpUserList_SelectedIndexChanged(object sender, EventArgs e)
        { 
     
        }

        protected void Allocation_Click(object sender, EventArgs e)
        {
            try
            {
                CIMS_Entities entities = new CIMS_Entities();

                instruction targetInstruction = entities.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

                targetInstruction.allocated_to = Convert.ToInt32(drpUserList.SelectedValue);

                entities.SaveChanges();

                system_users allocatedUser = entities.system_users.FirstOrDefault(u => u.system_user_id == targetInstruction.allocated_to);

                try
                {
                    sharedUtility.SendMail(allocatedUser.system_user_email,
                       "CIMS INSTRUCTION " + targetInstruction.reference + " " + System.DateTime.Now,
                       //email_to + "\r\n\r\n" +
                       "The instruction below requires your attention before the following delivery date: " + targetInstruction.delivery_date + "\r\n\r\n" +

                       "    Customer No.        : " + txtClient_Customer_Number.Text + "\r\n" +
                       "    Customer Name       : " + txtClient_Name.Text + "\r\n" +
                       "    Customer Account No.: " + drpAccount.SelectedItem.Text + "\r\n" +

                       "    Instruction Type    : " + targetInstruction.instructions_types.instruction_type + " - " + drpValidationStatus.SelectedItem.Text + "\r\n" +
                       "    Instruction ID      : " + txtInstructionID.Text + "\r\n" +
                       "    Instruction Comment : " + targetInstruction.comments + "\r\n" +
                       "    Refered by          : " + targetInstruction.modified_by + "\r\n" +
                       "    Originating Branch  : " + drpBranchs.SelectedItem.Text);
                }
                catch (Exception ex)
                {
                    //??? log error?
                    erl.LogError("Failed to send email notification to " + allocatedUser.system_user_email, ex.Message);
                    alert.FireAlerts(this.Page, "Failed to send email notification to " + allocatedUser.system_user_email + "\r\nPlease notify them manually.");
                }

                ShowPdf1.FilePath = "instructions/" + targetInstruction.file_name; 

                alert.FireAlerts(this.Page, "The instruction has been allocated to " + drpUserList.SelectedItem.Text + "." + Environment.NewLine + 
                    "An email has been sent to the user.");

            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Couldn't change the instruction allocation" + ex.Message.ToString());
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            //This needs to be empty for excel exporting to work. Don't change this.
        }
        private void excelImport(string path)
        {
            string connString = "";
            string strFileType = Path.GetExtension(path).ToLower();

            // string excelPath = FileUpload1.PostedFile.FileName;

            //connect to excel workbook
            if (strFileType.Trim() == ".xls")
            {
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source =" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";

            }
            else if (strFileType.Trim() == ".xlsx")
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0; HDR=Yes;IMEX=2\"";
            }

            //NEED TO FIND THE CORRECT COLUMNS TO GET FROM THE FILE
            string query = "SELECT * FROM [Sheet1$]";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "tbl_temp");

                    excelGridView.DataSource = ds.Tables["tbl_temp"];
                    excelGridView.DataBind();
                    da.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    alert.FireAlerts(this.Page, "There was an error importing the excel document " + ex.Message);
                    return;
                }
            }
        }

        protected void downloadButton_Click(object sender, EventArgs e)
        {
            CIMS_Entities _db = new CIMS_Entities();
            
            int instructionId = (int)_db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).instruction_id;
            string fileName = _db.instructions_attachment.FirstOrDefault(a => a.instruction_id == instructionId).file_name;
            string path = fileName;
            if (Server.MapPath("~").Contains("D:"))
            {
                path = "D:/CIMS/" + fileName;
            }
            else if (Server.MapPath("~").Contains("C:"))
            {
                path = Server.MapPath("~") + fileName;
            }
            path = path.Replace("\\", "/");
            FileInfo file = new FileInfo(path);
            if(file.Exists)
            {
                try
                {
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    Response.AddHeader("Content-Length", file.Length.ToString());
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.WriteFile(file.FullName);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                        
                }
                catch (Exception ex)
                {
                    alert.FireAlerts(this.Page, "Error downloading excel file from server " + ex.Message);
                    return;
                }
            }
            else
            {
                Response.Write("This file does not exist");
            }
            
            //try
            //{
            //    writeToExcelFile(excelGridView);
            //}

            //catch (Exception ex)
            //{
            //    alert.FireAlerts(this.Page, "Error exporting the attached excel file." + Environment.NewLine + ex.Message);
            //    return;
            //}
        }
        private void DownloadExcelFile(string path)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Charset = "";
                string filename = "RTGSInstruction" + DateTime.Now + ".xls";
                // string filename = "ReturnReport" + DateTime.Now + ".xls";
                StringWriter writer = new StringWriter();
                HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.xls";
                Response.AddHeader("content-disposition", "attachment;filename= " + filename);

                path = writer.ToString();

                Response.Write(path);

                HttpContext.Current.Response.End();
            }
            catch
            {

            }
        }

        private void writeToExcelFile(GridView gridView)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Charset = "";
                string filename = "RTGSInstruction" + DateTime.Now + ".xls";
                // string filename = "ReturnReport" + DateTime.Now + ".xls";
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

                HttpContext.Current.Response.End();
                //Response.End();
            }

            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Excel export error" + ex.Message);
                return;
            }

        }

        public int GetUserWithLeastWork(int userTypeId, int allocatedUserId)
        {
            CIMS_Entities _db = new CIMS_Entities();
            int assignedUserId = 0;

            //get branchcode based on originator branch code
            string branchCodeVal = _db.system_users.FirstOrDefault(s => s.system_user_id == allocatedUserId).system_user_branch_code;

            //get all ACTIVE users with the requested TYPE and BRANCH CODE 
            //List those users firstly by TOTALALLOCATEDWORK and secondly by ID
            IQueryable<system_users> userList = (from users in _db.system_users
                                                 where users.system_user_type == userTypeId &&
                                                 users.system_user_branch_code == branchCodeVal.ToString() &&
                                                 users.system_user_active == 1 && users.system_user_status == 1
                                                 select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);

            if (userList.Count() == 0)
            {
                userList = (from users in _db.system_users
                            where users.system_user_type == userTypeId &&
                            users.system_user_active == 1 && users.system_user_status == 1
                            select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);
            }
            //count of how many instructions are currently allocated to a user
            int allocatedWorkCount = 0;


            int maxTotalWork = (int)userList.Max(u => u.total_work_allocated);

            //get a list of all the instructions
            var instructions = from ins in _db.instructions
                               select ins;

            int lowestAllocation = instructions.Count(); //You can't be allocated more work than there are instructions

            //loop through the list to find the user with the least to do or just give work to the first person with nothing to do. 

            int minTotalWork = (int)userList.Min(u => u.total_work_allocated);


            //select all users with the lowest allocated work count (from instructions list) and lowest total work count attribute
            List<int> workCountList = new List<int>();

            //refine the list for those users with the lowest total work allocated
            List<system_users> lowestAllocationUsers = new List<system_users>();

            //add all the users with the lowest allocated work counts to the list
            foreach (system_users user in userList)
            {
                foreach (system_users insideUser in userList) //users are sorted by historical work count
                {
                    int count;
                    //get all the allocated work counts into a list so we can find the minimum.
                    count = instructions.Count(i => i.allocated_to == user.system_user_id);
                    workCountList.Add(allocatedWorkCount);
                }
                //get the count for the current user in the outer loop
                allocatedWorkCount = instructions.Count(i => i.allocated_to == user.system_user_id);

                if (allocatedWorkCount == workCountList.Min()) //add the user if their allocated workcount is the minimum. 
                {
                    lowestAllocationUsers.Add(user);
                }
            }
            //from the users with lowest allocated work, get the users with the lowest historical allocated work
            var usersWithLeastTotalWork = lowestAllocationUsers.ToList().Where(u => u.total_work_allocated == minTotalWork);

            //select the first user in this list as the assigned user for the instruction. 
            if (usersWithLeastTotalWork.FirstOrDefault() != null)
            {
                assignedUserId = (int)usersWithLeastTotalWork.FirstOrDefault().system_user_id;

            }
            else
            {
                if (lowestAllocationUsers.FirstOrDefault() != null)
                {
                    assignedUserId = (int)lowestAllocationUsers.FirstOrDefault().system_user_id;
                }
                else
                {
                    assignedUserId = (int)userList.FirstOrDefault().system_user_id;
                }
            }

            return assignedUserId;

        }


    }
}