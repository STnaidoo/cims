using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Data.OleDb;

namespace CIMS_V2
{
    public partial class Instructions : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        DAccessInfo daccess = new DAccessInfo();
        Constants constants = new Constants();
        CIMS_Entities _db = new CIMS_Entities();
        string fileName;
        //insert date format

        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            SharedFunctions sharedFunctions = new SharedFunctions();
            GenericDbFunctions genericFunctions = new GenericDbFunctions();
            InstructionsInfo instructionsUtil = new InstructionsInfo();
            CIMS_Entities excelFinder = new CIMS_Entities();
            //load the current instruction PDF or a place holder
            if (String.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                Session["FilePath"] = ShowPdf1.FilePath;
            }

            //if the reference has been generated and the instruction is an rtgs transaction.
            if (string.IsNullOrEmpty(txtTransactionReference.Text) == false && drpInstructions.SelectedItem.Text.Contains("RTGS") == true)
            {
                string excelFile = excelFinder.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).file_name;
                txtFileName.Text = "instructions/" + excelFile;
                if (excelFile.Contains(".xls") || excelFile.Contains(".csv"))
                {
                    //don't show the pdf viewer if an excel file is loaded
                    ShowPdf1.Visible = false;
                    excelGridView.Visible = true;
                }
            }

            //clear lists
            drpInstructions.Items.Clear();
            drpBranchs.Items.Clear();
            drpSearchBy.Items.Clear();

            drpAttachments.Visible = false;
            btnOpen.Visible = false;
            btnDeleteAttachment.Visible = false;

            dtmFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            dtmTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");

            //dropdown - done, untested
            //load search by customer listing 
            sharedFunctions.LoadDropDownList(
                drpSearchBy,
                genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }),
                "search_by_name",
                "search_by_value");

            //??? dropdown - done, untested
            //load instruction search
            sharedFunctions.LoadDropDownList(
                drpSearchBy,
                genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                "search_by_name",
                "search_by_value");


        }

        private void CutOffInstructionAllocationCheck()
        {
            DateTime today = DateTime.Now;
            try
            {
                var opsLogs = _db.operations_log.ToList().Where(o => o.log_category == "Cutoff Instructions");
                
                List<DateTime> logDates = new List<DateTime>();

                if (opsLogs != null)
                {
                    bool notAllAllocated = false;
                    if (opsLogs.FirstOrDefault(o => o.message == "Not all past cutoff instructions allocated") != null)
                    {
                        notAllAllocated = true;
                    }
                    foreach (operations_log log in opsLogs)
                    {
                        DateTime timeStamp = (DateTime)log.time_stamp;

                        DateTime date = timeStamp.Date; //we don't care about the time, only the date
                        logDates.Add(date);

                    }

                    if (!logDates.Contains(today.Date) || notAllAllocated == true)
                    {

                        bool allocatedInstructions = AllocatePastCutOffInstructions();
                        if (!allocatedInstructions)
                        {
                            alert.FireAlerts(this.Page, "Not all past cutoff instructions could be allocated");
                        }
                    }
                }
                

            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error allocating past cutoff instructions" + ex.Message);
            }
        }

        private void Page_Init(object sender, EventArgs e) //Handles Me.Init
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;

            try
            {
                if (String.IsNullOrEmpty(Session["UserID"].ToString()))
                {
                    Response.Redirect("Login. px");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Login. px");
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["fileName"] != null)
            {
                string fileName = Session["fileName"].ToString();
                //if transaction has been referenced and it is an rtgs
                if (string.IsNullOrEmpty(txtTransactionReference.Text) == false && drpInstructions.SelectedItem.Text.Contains("RTGS") == true)
                {
                    string excelFile = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).file_name;
                    txtFileName.Text = "instructions/" + fileName;
                    if (fileName.Contains(".xls") || fileName.Contains(".csv"))
                    {
                        //don't show the pdf viewer if an excel file is loaded
                        ShowPdf1.Visible = false;
                        //ShowPdf1.Height = 100;
                        excelGridView.Visible = true;
                    }
                }
                else
                {
                    ShowPdf1.FilePath = "instructions/" + fileName;
                    ShowPdf1.Visible = true;
                }

            }
            else if (String.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.Visible = true;
                ShowPdf1.FilePath = "new.pdf";
            }

            else
            {
                if (string.IsNullOrEmpty(txtTransactionReference.Text) == false)
                {
                    string file = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).file_name;
                    txtFileName.Text = "instructions/" + file;
                    if (file.ToLower().Contains(".pdf"))
                    {
                        ShowPdf1.Visible = true;
                        ShowPdf1.FilePath = "~/" + txtFileName.Text;
                    }
                    else
                    {
                        ShowPdf1.Visible = false;
                        excelGridView.Visible = true;

                    }
                }
            }



            if (!IsPostBack)
            {
                if (Session["fileName"] != null)
                {
                    string fileName = Session["fileName"].ToString();
                    ShowPdf1.FilePath = "instructions/" + fileName;
                }
                else
                {
                    try
                    {
                        if (_db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).file_name != null)
                        {
                            string fileName = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).file_name;
                            ShowPdf1.FilePath = "instructions/" + fileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        alert.FireAlerts(this.Page, "Couldn't find the pdf");
                    }
                }
                //  'Duty of care
                if (Session["CanPerformDOC"].Equals(1))
                {
                    drpDOC.Enabled = true;
                }
                else
                {
                    drpDOC.Enabled = false;
                }

                if (Session["UserType"].Equals(1))
                {
                    //drpDOC.Enabled = false

                    rbnSearchCustomer.Visible = true;
                    rbnSearchCustomer.Checked = true;

                    //??? dropdown - done, untested
                    //load customers search
                    sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }), "search_by_name", "search_by_value");
                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("customer"), "search_by_name", "search_by_value");
                    //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'customer' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN);

                    //  ' instruction_status - Set 
                    //int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                    //drpValidationStatus.SelectedIndex = indx;
                    sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "(is_archive is null OR is_archive" }, new string[] { Session["UserType"].ToString(), "0)" }), "document_status_action", "document_status_id");

                    int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("0"));
                    drpValidationStatus.SelectedIndex = indx;
                    btnSubmit.Visible = true;
                    //btnSubmit.Text = drpValidationStatus.SelectedItem.Text;

                    rbnProcessed.Visible = false;
                    rbnUnProcessed.Visible = false;

                    //??? dropdown - done, untested
                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_id" }, new string[] { "instruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocations WHERE status", "system_user_id", "active" }, new string[] { "1", Session["UserID"].ToString() + ")", "1" }), "instruction_type", "instruction_type_id");
                    //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                    //dnx.LoadDropDownListing("", "select '0'   instruction_type_id, ' Select Instruction Type'   instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id = '" & Session("UserID") & "' )  order by instruction_type", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN);



                }

                else if (Session["UserType"].Equals(3))
                {
                    rbnSearchCustomer.Visible = true;
                    rbnSearchCustomer.Checked = true;

                    //??? dropdown - done, untested
                    //load customers search
                    sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }), "search_by_name", "search_by_value");
                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("customer"), "search_by_name", "search_by_value");
                    //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'customer' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN)

                    //' instruction_status - Set 
                    int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                    drpValidationStatus.SelectedIndex = indx;

                    rbnProcessed.Visible = false;
                    rbnUnProcessed.Visible = false;

                    //??? dropdown - done, untested
                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_id" }, new string[] { "instruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocations WHERE status", "system_user_id", "active" }, new string[] { "1", Session["UserID"].ToString() + ")", "1" }), "instruction_type", "instruction_type_id");
                    //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                    //dnx.LoadDropDownListing("", "select '0'   instruction_type_id, ' Select Instruction Type'   instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id = '" & Session("UserID") & "' ) order by instruction_type", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                    //  'set radion button to search instruction
                    rbnSearchInstruction.Checked = true;
                    rbnSearchCustomer.Checked = false;

                    rbnSearchInstruction_CheckedChanged(null, null);
                }
                else
                {
                    //??? dropdown - done, untested
                    //load search by
                    sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }), "search_by_name", "search_by_value");
                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");
                    //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'instructions' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN)

                    rbnSearchCustomer.Visible = true;
                    rbnSearchCustomer.Checked = true;
                    rbnSearchInstruction.Checked = false;

                    //Can the user add an instruction? No!!!.
                    if(Session["UserType"].Equals(4))//Maker
                    {
                        enable_disable_addition_controls(true);
                    }
                    else
                    {
                        enable_disable_addition_controls(false);
                    }

                    //allow to view others
                    rbnProcessed.Visible = true;
                    rbnUnProcessed.Visible = true;

                    //??? dropdown - done, untested
                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "active" }, new string[] { "1" }), "instruction_type", "instruction_type_id");
                    //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                    //dnx.LoadDropDownListing("", "select '0'   instruction_type_id, ' Select Instruction Type'   instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types order by instruction_type", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN);

                }

                //??? dropdown - done, untested
                //load document status
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action" }, new string[] { Session["UserType"].ToString() }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN);

                //For branch user
                if (Session["UserType"].Equals(1))
                {
                    //??? dropdown - done, untested
                    sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "(is_archive is null OR is_archive" }, new string[] { Session["UserType"].ToString(), "0)" }), "document_status_action", "document_status_id");
                    drpValidationStatus.SelectedIndex = 2;
                    //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                    //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND (is_archive is null ||is_archive = 0) ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN);
                }
                else if (Session["UserType"].Equals(4))
                {
                    sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "(is_archive is null OR is_archive" }, new string[] { Session["UserType"].ToString(), "0)" }), "document_status_action", "document_status_id");
                    drpValidationStatus.SelectedIndex = 2;
                }

                //??? dropdown - done, untested
                //load currency
                sharedUtility.LoadDropDownList(drpCurrency, genericFunctions.GetDropdownListInfo("currency", new string[] { "currency_id", "currency_name" }, null, null), "currency_name", "currency_id");
                //sharedUtility.LoadDropDownList(drpCurrency, genericFunctions.GetCurrencyList(), "currency_name", "currency_id");
                //dnx.LoadDropDownListing("", "select '0'   currency_id, ' Select Currency'   currency_name from currency UNION select currency_id, currency_name from currency ", drpCurrency, "currency_name", "currency_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load user branch
                sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");
                //sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
                //dnx.LoadDropDownListing("", "select '0'   branch_id, ' '   branch_name from user_branch UNION select branch_id, branch_name from user_branch ", drpBranchs, "branch_name", "branch_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load rm ro
                sharedUtility.LoadDropDownList(drpRM, genericFunctions.GetDropdownListInfo("relationship_managers", new string[] { "RM_ID", "RM_Name" }, null, null), "RM_Name", "RM_ID");
                //sharedUtility.LoadDropDownList(drpRM, genericFunctions.GetRMDropDownListInfo(), "RM_Name", "RM_ID");
                //dnx.LoadDropDownListing("", "select '0'   RM_ID, ' Select RM'   RM_Name from relationship_managers UNION select RM_ID, RM_Name from relationship_managers ", drpRM, "RM_Name", "RM_ID", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load DOC
                sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");
                //sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDocCommentsDropDownListInfo(), "doc_comments", "doc_comments_id");
                //dnx.LoadDropDownListing("", "select '0'   doc_comments_id, ' Select doc comments'   doc_comments from duty_of_care_comments UNION select doc_comments_id, doc_comments from duty_of_care_comments ", drpDOC, "doc_comments", "doc_comments_id", My.Settings.strDSN)

                switch (Session["UserType"].ToString())
                {
                    case "1":
                        txtComments.ReadOnly = false;
                        btnAdd.Visible = false;
                        FileUpload2.Visible = false;

                        break;
                    case "2":
                        txtComments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                    case "3":
                        btnAdd.Visible = false;
                        FileUpload2.Visible = false;
                        //txtPRMO1Comments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                    case "4":
                        //txtPRMOTLComments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                    case "5":
                        //txtProcessorComments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                    case "6":
                        //txtProcessorComments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                    case "7":
                        //txtRMComments.ReadOnly = false;
                        btnView_Click(null, null);
                        break;
                }

                //check if the branch user can nprocess at branch
                string can_process_instruction = daccess.RunStringReturnStringValue1Where("system_users", "can_process_instruction", "system_user_id", Session["UserID"].ToString());

                if (can_process_instruction.Equals("1"))
                {
                    chkProcessAtBranch.Visible = true;
                }
                else
                {
                    chkProcessAtBranch.Visible = false;
                }

                if (!CutOffCheck()) //only allocate yesterday's past cutoff items if we are within cutoff now. 
                {
                    CutOffInstructionAllocationCheck();
                }
                //??? missing comparison
                //if ()
                //{
                //    btnAdd.Visible = true;
                //    FileUpload2.Visible = true;
                //    btnAdd.Enabled = true;
                //    FileUpload2.Enabled = true;
                //}
                //else
                //{
                //    btnAdd.Visible = false;
                //    FileUpload2.Visible = false;
                //    btnAdd.Enabled = false;
                //    FileUpload2.Enabled = false;
                //}

                //Maintain Scroll Position On Post Back
                this.Page.MaintainScrollPositionOnPostBack = true;

            }

            instructions_types selectedType = _db.instructions_types.FirstOrDefault(i => i.instruction_type == drpInstructions.SelectedItem.Text);

            if (selectedType != null & selectedType.allow_supporting_documents == 1)
            {
                lblSupportingDoc.Visible = true;
                btnSupportingAttach.Visible = true;
                txtSupportingFileName.Visible = true;
                supportingDocUpload.Visible = true;
            }



        }

        public void enable_disable_addition_controls(bool bl)
        {
            FileUpload1.Visible = bl;
            btnAttach.Visible = bl;
            btnGenerate.Visible = bl;
            btnNew.Visible = bl;

            //txtAmount.ReadOnly = Not bl
            //txtComments.ReadOnly = Not bl
            drpInstructions.Enabled = bl;
            chkProcessAtBranch.Enabled = bl;
            drpCurrency.Enabled = bl;
            drpAccount.Enabled = bl;

            //Disallow adding attachments
            btnAdd.Visible = false;
            FileUpload2.Visible = false;
        }

        public void enable_disable_addition_controls2(Boolean enbl)
        {
            txtAmount.Enabled = enbl;
            //txtComments.Enabled = enbl;
            drpAccount.Enabled = enbl;
            drpCurrency.Enabled = enbl;
            btnAttach.Enabled = enbl;
            btnGenerate.Enabled = enbl;
            btnNew.Enabled = enbl;

            drpInstructions.Enabled = enbl;
            chkProcessAtBranch.Enabled = enbl;
            FileUpload1.Enabled = enbl;
            //FileUpload2.Enabled = enbl;
            //btnAdd.Enabled = enbl;
            drpBranchs.Enabled = enbl;
            drpRM.Enabled = enbl;

            drpDeliveryHour.Enabled = enbl;
            drpDeliveryMinute.Enabled = enbl;

            dtmDeliveryTime.Enabled = enbl;

            //txtRelatedTransactionReference.ReadOnly = !enbl;

        }
        protected void btnOpenAccount_Click(object sender, EventArgs e)
        {
            unlock();
            try
            {
                int hm = how_many_unvoidmitted_branch_instructions_exists_for_this_user(Session["UserID"].ToString(), "0");

                //Allow one to delete ||voidmit the very first one ???/
                if (hm >= Int32.Parse(Session["no_of_instructions_i_can_pack"].ToString()) && hm != 0 && drpValidationStatus.SelectedValue.Equals(12))
                {
                    alert.FireAlerts(this.Page, "You seem to have about " + hm + " unsubmitted instructions. You must either Pack, Submit or Delete them before you continue. Please see the list below");
                    rbnSearchInstruction.Checked = true;
                    rbnSearchCustomer.Checked = false;
                    rbnSearchInstruction_CheckedChanged(null, null);
                    btnView_Click(null, null);
                    return;
                }
                reset_client();
                reset_instructions();
                enable_disable_editing(true);

                client_details placeHolderClient = _db.client_details.FirstOrDefault(c => c.Client_Customer_Number == "123456");

                txtClient_Name.Text = placeHolderClient.Client_Name;

                txtClient_Customer_Number.Text = placeHolderClient.Client_Customer_Number;

                txtClientID.Text = placeHolderClient.Client_ID.ToString();

                drpAccount.Visible = true;

                Load_client_details(Convert.ToInt32(placeHolderClient.Client_ID));

                ShowPdf1.FilePath = "new.pdf";
                drpInstructions.Enabled = true;
                drpInstructions.SelectedValue = "13"; //Account Opening
                chkProcessAtBranch.Enabled = true;
                btnGenerate.Visible = true;
                MultiView1.SetActiveView(ViewInstructions);
            }
            catch(Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading page " + ex.ToString());
            }
        
        }
        protected void btnView_Click(object sender, EventArgs e) //Handles btnView.Click
        {
            unlock();

            try
            {
                dgvClients.DataSource = null;
                dgvClients.DataBind();

                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                if (rbnSearchCustomer.Checked)
                {
                    if (txtSearch.Text.Length <= 2)
                    {
                        alert.FireAlerts(this.Page, "Please insert at least three characters to search a customer");
                        return;
                    }
                    int hm = how_many_unvoidmitted_branch_instructions_exists_for_this_user(Session["UserID"].ToString(), "0");

                    //Allow one to delete ||voidmit the very first one ???/
                    if (hm >= Int32.Parse(Session["no_of_instructions_i_can_pack"].ToString()) && hm != 0 && drpValidationStatus.SelectedValue.Equals(12))
                    {
                        alert.FireAlerts(this.Page, "You seem to have about " + hm + " unsubmitted instructions. You must either Pack, Submit or Delete them before you continue. Please see the list below");
                        rbnSearchInstruction.Checked = true;
                        rbnSearchCustomer.Checked = false;
                        rbnSearchInstruction_CheckedChanged(null, null);
                        btnView_Click(null, null);
                        return;
                    }
                    loadClients();
                }
                else if (rbnSearchInstruction.Checked)
                {
                    loadInstructions(-1);
                }

                MultiView1.SetActiveView(ViewCustomerList);
            }
            catch (Exception ex)
            {
                erl.LogError("Instruction.btnViewClick", ex.Message);
            }
        }

        public void unlock()
        {
            //Check if locked
            string locked_by = daccess.RunStringReturnStringValue1Where("instructions", "locked_by", "instruction_id", txtInstructionID.Text);

            if (rbnUnProcessed.Checked)
            {
                int n;
                if (Int32.TryParse(locked_by, out n) && n > 0)
                {
                    if (locked_by.ToString().Equals(Session["UserID"].ToString()))
                    {
                        if (daccess.RunNonQueryEqualsSelect("Update", "instructions", new string[] { "locked_by" }, new string[] { "0" }, "locked_date", "instructions", "locked_date", "instruction_id", "-10", "instruction_id", txtInstructionID.Text))
                        {
                            operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction unlocked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction Unloccked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        }
                    }
                }
            }
        }

        public void loadClients()
        {
            try
            {
                sharedUtility.LoadGridView(dgvClients, genericFunctions.GetDataSourceUserGridViewInfo("client_view", drpSearchBy.SelectedValue, txtSearch.Text, null, null));

            }
            catch (Exception ex)
            {
                erl.LogError("Error loading users.", ex.Message);
                alert.FireAlerts(this.Page, "Error loading users.");
            }
        }

        public void loadInstructions(int PageIndexes)
        {

            string user_type = Session["UserType"].ToString();

            try
            {
                switch (Session["UserType"].ToString())
                {
                    case "1":
                        if (rbnUnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions,
                            genericFunctions.GetDataTableGrid("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text,
                                "allocated_to", Session["UserID"].ToString(), "inserted_by", Session["UserID"].ToString(),
                                "status", Session["UserType"].ToString(), null, null));

                            //"Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND status = '" & Session("UserType") & "'

                            //    AND(allocated_to = '" & Session("UserID") & "'
                            //        OR inserted_by = '" & Session("UserID") & "')  "
                        }
                        else if (rbnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "branch_proccessed_date", "branch_proccessed_by", Session["UserID"].ToString(), null, null));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND branch_proccessed_by = '" & Session("UserID") & "' "

                            // " AND (DateDiff(Day, branch_proccessed_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, branch_proccessed_date, '" & dtmTo.Text & "') >= 0 )"
                        }
                        break;

                    case "2":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions,
                            genericFunctions.getDataGridJoin("instructions_view", "system_users", "instruction_type_allocations", "processor_proccessed_by", "system_user_id", drpSearchBy.SelectedValue, txtSearch.Text,
                            "instruction_type_id", "nstruction_type_id", "system_user_id", Session["UserID"].ToString(),
                            "status", Session["UserType"].ToString(), "system_tl_1", Session["UserID"].ToString()));


                            //sql = "Select * From instructions_view 

                            //    LEFT OUTER JOIN system_users 
                            //    ON branch_proccessed_by = system_users.system_user_id

                            //    Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%'

                            //    AND status = '" & Session("UserType") & "' 
                            //    AND system_tl_1 = '" & Session("UserID") & "'

                            //    AND instruction_type_id IN(select instruction_type_id from instruction_type_allocations
                            //                                                            where system_user_id = '" & Session("UserID") & "' )"

                        }
                        else if (rbnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "branch_approved_date", "branch_approved_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));


                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%' 
                            //    AND branch_approved_by = '" & Session("UserID") & "' " '
                            //    AND allocated_to = '" & Session("UserID") & "'  

                            //    "AND (DateDiff(Day, branch_proccessed_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, branch_approved_date, '" & dtmTo.Text & "') >= 0 )"

                        }
                        break;

                    case "3":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableGrid("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text,
                            "allocated_to", Session["UserID"].ToString(), "ftro_proccessed_by", Session["UserID"].ToString(),
                             "status", "3", "inserted_by", Session["UserID"].ToString()));


                            //sql = "Select * From instructions_view Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%' 
                            //    AND(status = '3')
                            //    AND(inserted_by = '" & Session("UserID") & "') " '
                            //    AND(ftro_proccessed_by = '" & Session("UserID") & "' OR allocated_to = '" & Session("UserID") & "')  "

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate3("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "ftro_proccessed_date",
                            "ftro_proccessed_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString(),
                            "status", "'3'"));


                        }
                        break;

                    case "4":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions,
                            genericFunctions.getDataGridJoinOr("instructions_view", "system_users", "instruction_type_allocations", "processor_proccessed_by", "system_user_id", drpSearchBy.SelectedValue, txtSearch.Text,
                            "instruction_type_id", "instruction_type_id", "system_user_id", Session["UserID"].ToString(),
                            "status", "'4'",
                            "ftro_proccessed_by", Session["UserID"].ToString(),
                            "system_tl_1", Session["UserID"].ToString(),
                            "ftro_allocated_by", Session["UserID"].ToString()
                            ));

                            //sql = "Select * From instructions_view 
                            //    LEFT OUTER JOIN system_users 
                            //    ON ftro_proccessed_by = system_users.system_user_id

                            //    Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%'

                            //AND(status = '4') 
                            //AND(ftro_proccessed_by <> '" & Session("UserID") & "')

                            //AND(system_tl_1 = '" & Session("UserID") & "' OR ftro_allocated_by = '" & Session("UserID") & "')


                            //AND instruction_type_id IN(Select instruction_type_id
                            //    from instruction_type_allocations
                            //    where system_user_id = '" & Session("UserID") & "' ) "

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "ftro_approved_date", "ftro_approved_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND ftro_approved_by = '" & Session("UserID") & "' " ' 
                            //    AND allocated_to = '" & Session("UserID") & "'  "

                            //     " AND (DateDiff(Day, ftro_approved_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, ftro_approved_date, '" & dtmTo.Text & "') >= 0 )"

                        }
                        break;

                    case "5":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.GetDataTableGrid("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text,
                            "allocated_to", Session["UserID"].ToString(), "ftro_proccessed_by", Session["UserID"].ToString(),
                            "status", Session["UserType"].ToString(), null, null));


                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND status = '" & Session("UserType") & "'
                            //    AND(processor_proccessed_by = '" & Session("UserID") & "'
                            //        OR allocated_to = '" & Session("UserID") & "')"

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "processor_approved_date", "processor_proccessed_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND(processor_proccessed_by = '" & Session("UserID") & "') " '
                            //    AND allocated_to = '" & Session("UserID") & "'  "


                            //    AND(DateDiff(Day, processor_proccessed_date, '" & dtmFrom.Text & "') <= 0
                            //        AND DateDiff(Day, processor_proccessed_date, '" & dtmTo.Text & "') >= 0)"

                        }
                        break;

                    case "6":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions,
                              genericFunctions.getDataGridJoinOr("instructions_view", "system_users", "instruction_type_allocations", "processor_proccessed_by", "system_user_id", drpSearchBy.SelectedValue, txtSearch.Text,
                            "system_tl_1", "system_user_id", "system_user_id", Session["UserID"].ToString(),
                            "status", Session["UserID"].ToString(),
                            "system_tl_1", Session["UserID"].ToString(),
                            "processor_allocated_by", Session["UserID"].ToString()));


                            //"Select * From instructions_view  
                            //    LEFT OUTER JOIN system_users 
                            //    ON processor_proccessed_by = system_users.system_user_id

                            //    Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%'

                            //AND status = '" & Session("UserType") & "'
                            //AND(system_tl_1 = '" & Session("UserID") & "'OR processor_allocated_by = '" & Session("UserID") & "')                               

                            //AND instruction_type_id IN(select instruction_type_id
                            //from instruction_type_allocations
                            //where system_user_id = '" & Session("UserID") & "' )"

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "processor_approved_date", "processor_approved_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));


                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND(processor_approved_by = '" & Session("UserID") & "') " '
                            //    AND allocated_to = '" & Session("UserID") & "'  "


                            //     " AND (DateDiff(Day, processor_approved_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, processor_approved_date, '" & dtmTo.Text & "') >= 0 )"

                        }
                        break;

                    case "7":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableAnd("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, "status", Session["UserType"].ToString(), null, null, null, null));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND status = '" & Session("UserType") & "' "

                        }
                        else if (rbnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "rm_proccessed_date", "rm_proccessed_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //    AND(rm_proccessed_by = '" & Session("UserID") & "') " '
                            //    AND allocated_to = '" & Session("UserID") & "'  "

                            // " AND (DateDiff(Day, rm_proccessed_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, rm_proccessed_date, '" & dtmTo.Text & "') >= 0 )"
                        }
                        break;

                    case "10":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions,
                                genericFunctions.getDataGridJoinNot("instructions_view", "system_users", "ftro_proccessed_by", "system_user_id", drpSearchBy.SelectedValue, txtSearch.Text,
                                "system_tl_1", "system_user_id", "system_manager", Session["UserID"].ToString(),
                                "status", "'4'", "ftro_proccessed_by", Session["UserID"].ToString()));


                            //sql = "Select * From instructions_view  
                            //    LEFT OUTER JOIN system_users 
                            //    ON ftro_proccessed_by = system_users.system_user_id 

                            //    Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%' 

                            //    AND (status = '4') 
                            //    AND (ftro_proccessed_by <> '" & Session("UserID") & "')  


                            //    AND (system_tl_1 IN (Select system_user_id from system_users 
                            //                                               Where system_manager = '" & Session("UserID") & "'))"

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "ftro_approved_date", "ftro_approved_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));

                            //   sql = "Select * From instructions_view 
                            //       Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                            //       AND ftro_approved_by = '" & Session("UserID") & "' " ' 
                            //       AND allocated_to = '" & Session("UserID") & "'  "

                            //" AND (DateDiff(Day, ftro_approved_date , '" & dtmFrom.Text & "') <=0  
                            //       AND DateDiff(Day, ftro_approved_date, '" & dtmTo.Text & "') >= 0 )"

                        }
                        break;

                    case "11":
                        if (rbnUnProcessed.Checked)
                        {
                            sharedUtility.LoadGridView(dgvInstructions,
                                genericFunctions.getDataGridJoin("instructions_view", "system_users", "instruction_type_allocations", "processor_proccessed_by", "system_user_id", drpSearchBy.SelectedValue, txtSearch.Text,
                                "system_tl_1", "system_user_id", "system_manager", Session["UserID"].ToString(),
                                "status", "'4'", null, null));


                            //sql = "Select * From instructions_view  
                            //    LEFT OUTER JOIN system_users 
                            //    ON processor_proccessed_by = system_users.system_user_id

                            //    Where " & drpSearchBy.SelectedValue & " 
                            //    LIKE '%" & txtSearch.Text & "%'

                            //    AND status = '6'


                            //    AND(system_tl_1 IN(Select system_user_id
                            //        from system_users
                            //        Where system_manager = '" & Session("UserID") & "'))"

                        }
                        else if (rbnProcessed.Checked)
                        {

                            sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableDate("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, dtmFrom.Text, dtmTo.Text,
                            "processor_approved_date", "processor_approved_by", Session["UserID"].ToString(), "allocated_to", Session["UserID"].ToString()));

                            //sql = "Select * From instructions_view 
                            //    Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%', 
                            //    AND(processor_approved_by = '" & Session("UserID") & "') " '
                            //    AND allocated_to = '" & Session("UserID") & "'  "

                            //" AND (DateDiff(Day, processor_approved_date , '" & dtmFrom.Text & "') <=0  
                            //    AND DateDiff(Day, processor_approved_date, '" & dtmTo.Text & "') >= 0 )"

                        }
                        break;
                    default:
                        sharedUtility.LoadGridView(dgvInstructions, genericFunctions.DataTableAnd("instructions_view ", drpSearchBy.SelectedValue, txtSearch.Text, "status", Session["UserType"].ToString(), "allocated_to", Session["UserID"].ToString(), null, null));

                        //sql = "Select * From instructions_view 
                        //        Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%'
                        //        AND status = '" & Session("UserType") & "'
                        //        AND allocated_to = '" & Session("UserID") & "'  ";
                        //    " ORDER BY inserted_date"
                        break;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Errorloading instructions by user ", ex.Message);
                alert.FireAlerts(this.Page, "Errorloading instructions.");
            }

        }

        protected void dgvClients_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string rw = (e.CommandArgument).ToString();

            reset_client();
            reset_instructions();

            //Add this to ensure that the ftro user can only voidmit to ftro ||delete.
            
            if (Session["UserType"].Equals(3))
            {
                //??? dropdown - done, untested
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "is_new_instruction" }, new string[] { Session["UserType"].ToString(), "1" }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND is_new_instruction = 1 ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)
            }

            try
            {
                int indx = drpBranchs.Items.IndexOf(drpBranchs.Items.FindByValue(Session["UserBranchID"].ToString()));
                drpBranchs.SelectedIndex = indx;
            }
            catch (Exception ex)
            {
                erl.LogError("dgvClientsRowCommand", ex.Message);
            }

            //'enable addition
            enable_disable_editing(true);

            string id = dgvClients.Rows[Int32.Parse(rw)].Cells[4].Text;
            string status = dgvClients.Rows[Int32.Parse(rw)].Cells[5].Text;

            if (status.ToLower().Equals("open") || status.ToLower().Equals("active"))
            {
                Load_client_details(Convert.ToInt32(id));
                ShowPdf1.FilePath = "new.pdf";
                drpInstructions.Enabled = true;
                chkProcessAtBranch.Enabled = true;
                btnGenerate.Visible = true;
                MultiView1.SetActiveView(ViewInstructions);
            }
            else
            {
                alert.FireAlerts(this.Page, "The Account is Not Active");
            }
        }

        public void reset_client()
        {
            txtClient_Customer_Number.Text = "";
            txtClient_Customer_Number.Text = "";
            txtClient_Name.Text = "";
            drpRM.SelectedIndex = -1;
        }

        public void Load_client_details(int ID)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("client_details", "*", "client_ID", ID.ToString());
            int indx = 0;

            try
            {
                while (rdr.Read())
                {
                    txtClient_Name.Text = rdr["Client_Name"].ToString();

                    txtClient_Customer_Number.Text = rdr["Client_Account_Number"].ToString();

                    txtClient_Customer_Number.Text = rdr["Client_Customer_Number"].ToString();

                    txtClientID.Text = rdr["Client_ID"].ToString();

                    //??? dropdown - done, untested
                    sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetDropdownListInfo("client_details", new string[] { "Client_ID", "Client_Account_Number" }, new string[] { "Client_Customer_Number", "Status" }, new string[] { txtClient_Customer_Number.Text, "Open | OPEN" }), "Client_Account_Number", "Client_ID");
                    //sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetClientDropDownListInfo(), "Client_Account_Number", "Client_ID");
                    //dnx.LoadDropDownListing("", "select '0'   Client_ID, ' Select Account'   Client_Account_Number from client_details UNION select Client_ID, Client_Account_Number from client_details where Client_Customer_Number = '" & txtClient_Customer_Number.Text & "' AND Status = 'Open' ", drpAccount, "Client_Account_Number", "Client_ID", My.Settings.strDSN);

                    indx = drpRM.Items.IndexOf(drpRM.Items.FindByValue(rdr["RM_ID"].ToString()));
                    drpRM.SelectedIndex = indx;

                    hide_unhide_comments_field(Session["UserType"].ToString());
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Err||loading clients details by user ", ex.Message);
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

        public void load_flow_dropdown()
        {
            //??? dropdown - done, untested
            sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", 
                new string[] { "document_status_id", "document_status_action" },
                new string[] { "document_status_user_type_who_can_action" },
                new string[] { Session["UserType"].ToString() }),
                "document_status_action",
                "document_status_id");
            //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
            //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "'  AND (is_archive is null ||is_archive = 0) AND ((work_flow_id = 0 ||work_flow_id IS NULL) ||work_flow_id IN (select work_flow_id from instructions_types WHERE instruction_type_id = '" & drpInstructions.SelectedValue & "' ))  ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)
        }

        public void Load_instruction_details(int ID)
        {


            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("instructions_view", "*", "instruction_ID", ID.ToString());
            int indx = 0;

            try
            {
                while (rdr.Read())
                {

                    chkAllowDuplicates.Checked = true;


                    txtStatus.Text = rdr["Status"].ToString();
                    //txtDocumentStatusID.Text = rdr["document_status_id"].ToString();
                    //txtRelatedTransactionReference.Text = rdr["related_reference"].ToString();


                    indx = drpInstructions.Items.IndexOf(drpInstructions.Items.FindByValue(rdr["instruction_type_id"].ToString()));
                    drpInstructions.SelectedIndex = indx;

                    //??? dropdown - done, untested
                    sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action" }, new string[] { txtStatus.Text }), "document_status_action", "document_status_id");
                    //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                    //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & txtStatus.Text & "' ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)


                    if (rdr["exception"].ToString() == "1")
                    {
                        //??? dropdown - done, untested
                        sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_action", "document_status_id" }, new string[] { "document_status_user_type_who_can_action", "document_status_id" }, new string[] { txtStatus.Text, rdr["foward_back_to_after_reversal"].ToString() }), "document_status_action", "document_status_id");
                        //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                        //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & txtStatus.Text & "'  UNION Select document_status_id, document_status_action from document_status where document_status_id = '" & rdr("foward_back_to_after_reversal").ToString() & "'", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)
                    }

                    if ((Session["UserType"].ToString() == "3") && (txtAmount.ReadOnly == false) && (rdr["inserted_by"].ToString() == Session["UserID"].ToString()))
                    {
                        //??? dropdown - done, untested
                        sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "is_new_instruction" }, new string[] { Session["UserType"].ToString(), "1" }), "document_status_action", "document_status_id");
                        //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                        //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND is_new_instruction = 1 ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)
                    }
                    else
                    {
                        load_flow_dropdown();
                    }

                    btnDeleteAttachment.Visible = false;

                    switch (Session["UserType"].ToString())
                    {
                        //??? need to check case 1 - surely one should be true
                        case "1":
                            if ((rdr["inserted_by"].ToString() == Session["UserID"].ToString()) && (txtStatus.Text == "1"))
                            {
                                enable_disable_addition_controls2(false);

                            }
                            else
                            {
                                enable_disable_addition_controls2(false);
                            }
                            break;

                        case "2":
                            enable_disable_addition_controls2(false);
                            break;

                        //??? same issue as case 1
                        case "3":
                            if ((rdr["inserted_by"].ToString() == Session["UserID"].ToString()) && (txtStatus.Text == "3"))
                            {
                                enable_disable_addition_controls2(false);
                            }
                            else
                            {
                                enable_disable_addition_controls2(false);
                            }
                            break;

                        case "4":
                            enable_disable_addition_controls2(false);
                            break;

                        case "5":
                            enable_disable_addition_controls2(false);
                            break;

                        case "6":
                            enable_disable_addition_controls2(false);
                            break;

                        case "7":
                            enable_disable_addition_controls2(false);
                            break;

                        case "10":
                            enable_disable_addition_controls2(false);
                            break;

                        case "11":
                            enable_disable_addition_controls2(false);
                            break;
                    }

                    if (rbnUnProcessed.Checked == true)
                    {
                        double value1 = 0;
                        if ((double.TryParse(rdr["locked_by"].ToString(), out value1)) && (value1 > 0))
                        {
                            if (rdr["locked_by"].ToString() == Session["UserID"].ToString())
                            {
                                enable_disable_editing(true);
                            }

                            if ((rdr["inserted_by"].ToString() == Session["UserID"].ToString()) && ((Session["UserType"].ToString() == "1") || (Session["UserType"].ToString() == "3")))
                            {
                                if (string.IsNullOrEmpty(rdr["reference"].ToString()))
                                {
                                    drpInstructions.Enabled = true;
                                    chkProcessAtBranch.Enabled = true;
                                    btnGenerate.Visible = true;
                                }
                                else
                                {
                                    drpInstructions.Enabled = false;
                                    chkProcessAtBranch.Enabled = false;
                                    btnGenerate.Visible = false;
                                }
                            }
                            else
                            {
                                enable_disable_editing(false);
                            }
                        }
                        else if ((rdr["inserted_by"].ToString() == Session["UserID"].ToString()) && ((Session["UserType"].ToString() == "1") || (Session["UserType"].ToString() == "3")))
                        {
                            enable_disable_editing(true);
                            enable_disable_addition_controls2(true);

                            if (string.IsNullOrEmpty(rdr["reference"].ToString()))
                            {
                                drpInstructions.Enabled = true;
                                chkProcessAtBranch.Enabled = true;
                                btnGenerate.Visible = true;
                            }
                            else
                            {
                                drpInstructions.Enabled = false;
                                chkProcessAtBranch.Enabled = false;
                                btnGenerate.Visible = false;
                            }

                            //daccess.RunNonQuery1Where("Update", "instructions", new string[] { "locked_by", "locked_date" }, new string[] { Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", rdr["Instruction_ID"].ToString());
                        }
                        else
                        {
                            enable_disable_editing(true);
                            //daccess.RunNonQuery1Where("Update", "instructions", new string[] { "locked_by", "locked_date" }, new string[] { Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", rdr["Instruction_ID"].ToString());
                        }
                    }

                    switch (txtStatus.Text)
                    {
                        case "1":
                            btnDeleteAttachment.Visible = true;
                            txtFTRef.ReadOnly = true;

                            if (chkProcessAtBranch.Checked)
                            {
                                txtFTRef.ReadOnly = false;
                            }
                            break;

                        case "2":
                            txtFTRef.ReadOnly = true;
                            break;

                        case "3":
                            //txtPRMO1Comments.ReadOnly = false;
                            //txtPRMOTLComments.ReadOnly = true;
                            //txtProcessorComments.ReadOnly = true;
                            btnDeleteAttachment.Visible = true;
                            txtFTRef.ReadOnly = true;
                            break;

                        case "4":
                            //txtPRMOTLComments.ReadOnly = false;
                            //txtPRMO1Comments.ReadOnly = true;
                            //txtProcessorComments.ReadOnly = true;
                            //txtRMComments.ReadOnly = true;
                            txtFTRef.ReadOnly = true;
                            break;

                        case "5":
                            //txtProcessorComments.ReadOnly = false;
                            //txtPRMOTLComments.ReadOnly = true;
                            //txtPRMO1Comments.ReadOnly = true;
                            //txtRMComments.ReadOnly = true;
                            txtFTRef.ReadOnly = true;

                            if (Session["UserType"].ToString() == "5")
                            {
                                txtFTRef.ReadOnly = false;
                            }
                            else
                            {
                                txtFTRef.ReadOnly = true;
                            }
                            break;

                        case "7":
                            //txtRMComments.ReadOnly = false;
                            //txtPRMOTLComments.ReadOnly = true;
                            //txtPRMO1Comments.ReadOnly = true;
                            //txtProcessorComments.ReadOnly = true;
                            break;
                    }

                    txtClient_Name.Text = rdr["Client_Name"].ToString();

                    txtClient_Customer_Number.Text = rdr["Client_Customer_Number"].ToString();

                    txtClientID.Text = rdr["Client_ID"].ToString();

                    txtFileName.Text = rdr["file_name"].ToString();
                    txtComments.Text = rdr["Comments"].ToString();
                    txtInstructionID.Text = rdr["Instruction_ID"].ToString();

                    txtAmount.Text = rdr["Amount"].ToString();

                    double value = 0;
                    if (double.TryParse(txtAmount.Text, out value))
                    {
                        txtAmount.Text = Convert.ToDouble(txtAmount.Text).ToString("N2");
                    }

                    txtTransactionReference.Text = rdr["reference"].ToString();

                    //txtPRMO1Comments.Text = rdr["ftroa_comments"].ToString();

                    //txtPRMO2Comments.Text = rdr["prmo2_comments"].ToString();

                    //txtPRMOTLComments.Text = rdr["ftrob_comments"].ToString();

                    //txtProcessorComments.Text = rdr["processor_comments"].ToString();

                    //txtRMComments.Text = rdr["rm_comments"].ToString();

                    txtFTRef.Text = rdr["ft_reference"].ToString();

                    indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue(rdr["document_status_id"].ToString()));
                    drpValidationStatus.SelectedIndex = indx;

                    if (txtStatus.Text == "1")
                    {
                        indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("10"));
                        drpValidationStatus.SelectedIndex = indx;
                        btnSubmit.Visible = true;

                        //btnSubmit.Text = drpValidationStatus.SelectedItem.Text;
                    }

                    //??? dropdown - done, untested
                    sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetDropdownListInfo("client_details", new string[] { "Client_ID", "Client_Account_Number" }, new string[] { "Client_Customer_Number" }, new string[] { txtClient_Customer_Number.Text }), "Client_Account_Number", "Clinet_ID");
                    //sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetClientDropDownListInfo(), "Client_Account_Number", "Client_ID");
                    //dnx.LoadDropDownListing("", "select '0'   Client_ID, ' Select Account'   Client_Account_Number from client_details UNION select Client_ID, Client_Account_Number from client_details where Client_Customer_Number = '" & txtClient_Customer_Number.Text & "' ", drpAccount, "Client_Account_Number", "Client_ID", My.Settings.strDSN)

                    indx = drpAccount.Items.IndexOf(drpAccount.Items.FindByText(rdr["account_no"].ToString()));
                    drpAccount.SelectedIndex = indx;

                    indx = drpCurrency.Items.IndexOf(drpCurrency.Items.FindByValue(rdr["currency_id"].ToString()));
                    drpCurrency.SelectedIndex = indx;

                    //txtInstructionStatus.Text = rdr["instruction_status"].ToString();

                    //lblLockedBy.Text = " Locked By: " + rdr["locked_by_name"].ToString();
                    //lblLockedDate.Text = " Date: " + rdr["locked_date"].ToString();

                    indx = drpRM.Items.IndexOf(drpRM.Items.FindByValue(rdr["RM_ID"].ToString()));
                    drpRM.SelectedIndex = indx;

                    indx = drpBranchs.Items.IndexOf(drpBranchs.Items.FindByValue(rdr["branch_id"].ToString()));
                    drpBranchs.SelectedIndex = indx;


                    indx = drpDOC.Items.IndexOf(drpDOC.Items.FindByValue(rdr["doc_comments_id"].ToString()));
                    drpDOC.SelectedIndex = indx;

                    //txtCallBackComment.Text = rdr["call_back_comments"].ToString();

                    //txtCallBackNos.Text = rdr["call_back_no"].ToString();

                    DateTime date;
                    if (DateTime.TryParse(rdr["delivery_date"].ToString(), out date))
                    {
                        dtmDeliveryTime.Text = date.ToString("dd-MM-yyyy");

                        string hour = date.ToString("HH");
                        string min = date.ToString("mm");
                        string tt = date.ToString("tt");

                        indx = drpDeliveryHour.Items.IndexOf(drpDeliveryHour.Items.FindByText(hour));
                        drpDeliveryHour.SelectedIndex = indx;

                        indx = drpDeliveryMinute.Items.IndexOf(drpDeliveryMinute.Items.FindByText(min));
                        drpDeliveryMinute.SelectedIndex = indx;

                        indx = drpAmPms.Items.IndexOf(drpAmPms.Items.FindByText(tt));
                        drpAmPms.SelectedIndex = indx;
                    }

                    load_attachment();

                    hide_unhide_comments_field(Session["UserType"].ToString());

                    if (rdr["processed_at_branch"].ToString() == "1")
                    {
                        chkProcessAtBranch.Checked = true;
                    }
                    else
                    {
                        chkProcessAtBranch.Checked = false;
                    }

                    indx = drpCrossCurrency.Items.IndexOf(drpCrossCurrency.Items.FindByValue(rdr["cross_currency"].ToString()));
                    drpCrossCurrency.SelectedIndex = indx;

                    txtCCRate.Text = rdr["cross_currency_rate"].ToString();

                    if (Session["can_add_attachment"].ToString() == "1")
                    {
                        btnAdd.Visible = false;
                        FileUpload2.Visible = false;
                        btnAdd.Enabled = false;
                        FileUpload2.Enabled = false;
                    }
                    else
                    {
                        btnAdd.Visible = false;
                        FileUpload2.Visible = false;
                        btnAdd.Enabled = false;
                        FileUpload2.Enabled = false;
                    }

                    toggle_fields();
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Err||loading instruction details by user ", ex.Message);
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

        protected void rbnSearchCustomer_CheckedChanged(object sender, EventArgs e)// Handles rbnSearchCustomer.CheckedChanged
        {
            if (rbnSearchCustomer.Checked)
            {
                reset_instructions();

                rbnUnProcessed.Checked = true;
                enable_disable_editing(true);

                //??? dropdown - done, untested
                //load search by
                sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }), "search_by_name", "search_by_value");
                //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("customer"), "search_by_name", "search_by_value");
                //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'customer' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN);

                rbnProcessed.Visible = false;
                rbnUnProcessed.Visible = false;

                dgvClients.DataSource = null;
                dgvClients.DataBind();
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();
                MultiView1.SetActiveView(ViewCustomerList);

                //??? dropdown - done, untested
                //aded because of a bug document status on 28-Sep-2011. This is to reset to the allowable actions f||the user
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action" }, new string[] { Session["UserType"].ToString() }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //dnx.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN);
            }
        }

        protected void rbnSearchInstruction_CheckedChanged(object sender, EventArgs e) //Handles rbnSearchInstruction.CheckedChange
        {
            if (rbnSearchInstruction.Checked)
            {
                //??? dropdown - done, untested
                sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_value", "search_by_name" }, new string[] { "search_by_module" }, new string[] { "instrcutions" }), "search_by_name", "search_by_value");
                //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");
                //dnx.LoadDropDownListing("", "select search_by_value, search_by_name from search_by  where search_by_module = 'instructions' ORDER BY search_by_name ", drpSearchBy, "search_by_name", "search_by_value", My.Settings.strDSN);

                rbnProcessed.Visible = true;
                rbnUnProcessed.Visible = true;

                dgvClients.DataSource = null;
                dgvClients.DataBind();
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();
                MultiView1.SetActiveView(ViewCustomerList);

                txtSearch.Text = "";

                rbnUnProcessed_CheckedChanged(null, null);
                rbnProcessed_CheckedChanged(null, null);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e) //Handles btnNew.Click
        {
            reset_instructions();
            btnGenerate.Visible = true;
            btnGenerate.Text = "Get Ref";
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
            //txtDocumentStatusID.Text = "";
            chkAllowDuplicates.Checked = true;
            //lblLockedBy.Text = "";
            //lblLockedDate.Text = "";
            //txtRMComments.Text = "";
            //txtRelatedTransactionReference.Text = "";
            drpBranchs.SelectedIndex = -1;
            //txtCallBackComment.Text = "";
            //txtCallBackNos.Text = "";
            drpDOC.SelectedIndex = -1;

            // txtPRMO1Comments.Text = "";
            // txtPRMO2Comments.Text = "";
            // txtPRMOTLComments.Text = "";
            // txtProcessorComments.Text = "";

            drpDeliveryHour.SelectedIndex = -1;
            drpDeliveryMinute.SelectedIndex = -1;
            drpAmPms.SelectedIndex = -1;

            dtmDeliveryTime.Text = "";

            chkProcessAtBranch.Checked = false;

            drpCrossCurrency.SelectedIndex = -1;
            txtCCRate.Text = "";

            //'set default image
            ShowPdf1.FilePath = "new.pdf";

            if (Session["UserType"].Equals("1"))
            {
                int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                drpValidationStatus.SelectedIndex = indx;
                //drpValidationStatus_SelectedIndexChanged(null, null);
            }

        }

        private bool AllocatePastCutOffInstructions()
        {
            bool allocatedAll = false;
            operations_log opsLog = new operations_log();
            int userId = Convert.ToInt32(Session["UserId"].ToString());
            system_users currentUser = _db.system_users.FirstOrDefault(u => u.system_user_id == userId);
            try
            {
                var pastCutoffInstructions = _db.instructions.ToList().Where(i => i.past_cutoff_allocation == 1); //create attribute unallocated

                opsLog.time_stamp = DateTime.Now;
                opsLog.message = "Past cutoff allocation started";
                opsLog.user_full_name = currentUser.system_user_fname + " " + currentUser.system_user_lname;
                opsLog.ip_address = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                opsLog.internal_user_id = (int)currentUser.system_user_id;
                opsLog.log_category = "Cutoff Instructions";
                opsLog.opt_id = 0;
                opsLog.user_group = 0;

                if (pastCutoffInstructions.Count() > 0)
                {
                    foreach (instruction i in pastCutoffInstructions) //allocate all of the past cutoff instructions
                    {
                        i.allocated_to = GetUserWithLeastWork((int)i.status, (int)i.instruction_type_id, (int)i.instruction_id);

                        //if we can't find an acceptable user to which to allocate
                        if (i.allocated_to == Convert.ToInt32(Session["UserId"].ToString())) 
                        {
                            i.allocated_to = 0; //keep it in unallocated
                        }
                        else
                        {
                            i.past_cutoff_allocation = 0; 
                        }
                        i.allocated_date = DateTime.Now;                      
                                            
                    }


                    if (_db.instructions.ToList().Where(i => i.past_cutoff_allocation == 1).Count() == 0)
                    {
                        allocatedAll = true; //all have been allocated
                        opsLog.message = "All past cutoff instructions allocated";
                        _db.operations_log.Add(opsLog);


                    }
                    else
                    {
                        allocatedAll = false; //there are some unallocated left
                        opsLog.message = "Not all past cutoff instructions allocated";
                        _db.operations_log.Add(opsLog);
                    }

                }
                else
                {
                    allocatedAll = false; //there aren't any instructions to allocate
                    opsLog.message = "No instructions past cutoff to allocate";
                    _db.operations_log.Add(opsLog);
                }

                _db.SaveChanges();

            }
            catch(Exception ex)
            {
                alert.FireAlerts(this.Page, "Error allocating past cutoff instructions " + ex.Message);
            }
            return allocatedAll;
                                    
        }
        protected void btnGenerate_Click(object sender, EventArgs e) //Handles btnGenerate.Click
        {
            if (!validate_generation_of_ref())
            {
                return;
            }


            if (btnGenerate.Text.ToLower().Contains("get"))
            {
                int id = proc_insert_instructions();

                if (id > 0)
                {
                    //DEV ME
                    txtInstructionID.Text = id.ToString(); //get instruction ID from instruction just inserted into db

                    //gets instruction code e.g. OTT where instruction type ID matches the selected instruction type from the drop down. 
                    string get_code = daccess.RunStringReturnStringValue1Where("instructions_types", "instruction_code", "instruction_type_ID", drpInstructions.SelectedValue);
                    //creates the reference in the form instructiontype/branchcode/instructionID
                    //string rf = get_code + "/" + Session["UserBranchCode"].ToString() + "/" + id.ToString();

                    string currentDate = DateTime.Now.ToString("yyyy-mm-dd");
                    string rf = get_code + "/" + currentDate + "/" + id.ToString();
                    //updates the created instruction to give it its reference. 
                    if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "reference" }, new string[] { rf }, "instruction_id", id.ToString()))
                    {
                        txtTransactionReference.Text = rf;
                        btnGenerate.Text = "Update";
                        btnGenerate.Visible = false;
                        ShowPdf1.FilePath = "new.pdf";
                        drpInstructions.Enabled = false;
                        chkProcessAtBranch.Enabled = false;
                    }
                    else
                    {
                        txtTransactionReference.Text = "Error";
                    }


                }
                else
                {
                    alert.FireAlerts(this.Page, "Could not generate ID so saving was not possible");
                }

                
            }

            if (CutOffCheck())
            {
                alert.FireAlerts(this.Page, "The cutoff time for this instruction type has passed. It can be submitted but will only be processed tomorrow.");
            }
            //else if (btnGenerate.Text.Contains("update"))
            //{

            //}
        }

        public Boolean validate_generation_of_ref()
        {
            if (drpRM.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the RM");
                return false;
            }
            double n;
            if (!double.TryParse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out n))
            {
                alert.FireAlerts(this.Page, "Please type the amount");
                return false;
            }

            if (drpInstructions.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the instruction type");
                return false;
            }

            if (drpAccount.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the account");
                return false;
            }

            if (drpCurrency.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the currency");
                return false;
            }

            if (drpBranchs.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the branch");
                return false;
            }

            if (drpCrossCurrency.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select whether it is cross currency or not");
                return false;
            }

            if (drpCrossCurrency.SelectedValue.Equals("1"))
            {
                double value;
                if (!double.TryParse(txtCCRate.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out value) && value > 0)
                {
                    alert.FireAlerts(this.Page, "Please select the cross currency rate. It should be greater than zero");
                    return false;
                }
            }

            DateTime date;
            if (!DateTime.TryParse(get_deivery_date(), out date) || !DateTime.TryParse(dtmDeliveryTime.Text, out date) || drpDeliveryHour.SelectedIndex <= 0 || drpDeliveryHour.SelectedIndex <= 0)
            {
                if (!drpValidationStatus.SelectedValue.Equals(12))
                {
                    alert.FireAlerts(this.Page, "Please insert the delivery date");
                    return false;
                }
                else
                {
                    dtmDeliveryTime.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    drpDeliveryHour.SelectedIndex = 2;
                    drpDeliveryMinute.SelectedIndex = 2;
                }
            }


            if (Session["UserType"].Equals("1"))
            {
                if (drpBranchs.SelectedValue != Session["UserBranchID"].ToString())
                {
                    alert.FireAlerts(this.Page, "Please select the right branch");
                    return false;
                }

                int hm = how_many_unvoidmitted_branch_instructions_exists_for_this_user(Session["UserID"].ToString(), txtInstructionID.Text);
                string is_document_held = daccess.RunStringReturnStringValue1Where("document_status", "is_document_held", "document_status_id", drpValidationStatus.SelectedValue);
                string is_exception = daccess.RunStringReturnStringValue1Where("document_status", "exception", "document_status_id", drpValidationStatus.SelectedValue);

                //Allow one to delete, refer ||hold if other documents exists
                if (hm >= Convert.ToInt16(Session["no_of_instructions_i_can_pack"].ToString()) && hm != 0 && drpValidationStatus.SelectedValue != "12" && is_exception != "1" && is_document_held != "1")
                {
                    alert.FireAlerts(this.Page, "You seem to have about " + hm + " unvoidmitted nstructions. You must either Pack, voidmit ||Delete them");
                    return false;
                }
            }
            

            string check_dups = check_for_duplicates(drpAccount.SelectedItem.Text, drpInstructions.SelectedValue, drpCurrency.SelectedValue, double.Parse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat), txtInstructionID.Text);
            
            if (!String.IsNullOrEmpty(check_dups) && !chkAllowDuplicates.Checked && drpValidationStatus.SelectedValue != "12")
            {
                alert.FireAlerts(this.Page, "Please note that Duplicate Instruction Exists. Refer to the following instructions: " + check_dups + ". To continue anyway check *Allow Duplicates* and click Get Ref again.");
                //??? should get rid of this but not sure what it is for yet
                //Session["popup_sql"] = "Select * From instructions_view Where reference IN (" & check_dups.Replace("*", "'") & ")"
                openpopup("popup_view.aspx");
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Duplicate instruction detected with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Duplicate instruction detected", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                return false;
            }
            return true;
        }

        public int how_many_unvoidmitted_branch_instructions_exists_for_this_user(string user_id, string instruction_id)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("instructions_view", "ISNULL(Count(*),0) AS Counts", new string[] { "instruction_id <> ", "inserted_by", "status", "(is_document_held <> ", "(exception <> " }, new string[] { instruction_id, user_id, Session["UserType"].ToString(), "1 OR is_document_held IS NULL)", "1 OR exception IS NULL)" });

            int Count = 0;

            if (Session["UserType"].ToString() == "3")
            {
                rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("instructions_view", "ISNULL(Count(*),0) AS Counts", new string[] { "instruction_id <> ", "inserted_by", "status", "(is_document_held <> ", "(exception <> ", "is_not_in_my_queue <> " }, new string[] { instruction_id, user_id, Session["UserType"].ToString(), "1 OR is_document_held IS NULL)", "1 OR exception IS NULL)", "1 AND exception IS NULL)" });
            }

            try
            {
                while (rdr.Read())
                {
                    Count = Convert.ToInt32(rdr["Counts"]);
                }
            }
            catch (Exception ex)
            {
                //??? log error?
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

            return Count;
        }

        public int proc_insert_instructions()
        {
            int proc;
            string account_no = drpAccount.SelectedItem.Text;
            string file_name = txtFileName.Text;
            double amount = double.Parse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat);
            string reference = txtTransactionReference.Text;
            //DEV ME
            //should actually generate the reference here and then just display it to the user instead of using the Get Ref button. 

            try
            {
                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {


                    SqlCommand myCommand = null;
                    proc = 0;
                    myCommand = new SqlCommand("proc_insert_instructions", myConnection);
                    myCommand.CommandTimeout = 0;

                    myCommand.CommandType = CommandType.StoredProcedure;

                    string comments = txtComments.Text;


                    int instruction_type_id = Convert.ToInt32(drpInstructions.SelectedValue);
                    DateTime inserted_date = Convert.ToDateTime("1/1/1900");
                    int inserted_by = Convert.ToInt32(Session["UserID"]);
                    DateTime modified_date = Convert.ToDateTime("1/1/1900");
                    int modified_by = Convert.ToInt32(Session["UserID"]);
                    int status = 1;
                    string client_id = txtClientID.Text;
            

                    string file_type = "";
                    int allocated_to = Convert.ToInt32(Session["UserID"]);

                    int currency_id = Convert.ToInt32(drpCurrency.SelectedValue);
                    int branch_id = Convert.ToInt32(drpBranchs.SelectedValue);
                    string ft_reference = txtFTRef.Text;
                    int rm_id = Convert.ToInt32(drpRM.SelectedValue);
                    DateTime delivery_date = Convert.ToDateTime("1/1/1900");
                    int processed_at_branch = Convert.ToInt16(chkProcessAtBranch.Checked);
                    string cross_currency = drpCrossCurrency.SelectedValue;
                    if (string.IsNullOrWhiteSpace(txtCCRate.Text))
                    {
                        txtCCRate.Text = "0";
                    }

                    double cross_currency_rate = double.Parse(txtCCRate.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat);

                    //delivery_date = get_deivery_date()

                    myCommand.Parameters.Add("@amount", SqlDbType.Float);
                    myCommand.Parameters.Add("@comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@reference", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@file_name", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@instruction_type_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@inserted_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@inserted_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@modified_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@modified_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@status", SqlDbType.Int);
                    myCommand.Parameters.Add("@client_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_to", SqlDbType.Int);
                    myCommand.Parameters.Add("@file_type", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@id_out", SqlDbType.Int);
                    myCommand.Parameters.Add("@account_no", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@currency_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@branch_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@ft_reference", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@rm_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@delivery_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@processed_at_branch", SqlDbType.Int);
                    myCommand.Parameters.Add("@cross_currency", SqlDbType.Int);
                    myCommand.Parameters.Add("@cross_currency_rate", SqlDbType.Float);

                    myCommand.Parameters["@amount"].Value = amount;
                    myCommand.Parameters["@comments"].Value = comments;
                    myCommand.Parameters["@reference"].Value = reference;
                    myCommand.Parameters["@file_name"].Value = file_name;
                    myCommand.Parameters["@instruction_type_id"].Value = instruction_type_id;
                    myCommand.Parameters["@inserted_date"].Value = inserted_date;
                    myCommand.Parameters["@inserted_by"].Value = inserted_by;
                    myCommand.Parameters["@modified_date"].Value = modified_date;
                    myCommand.Parameters["@modified_by"].Value = modified_by;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@client_id"].Value = client_id;
                    myCommand.Parameters["@allocated_to"].Value = allocated_to;
                    myCommand.Parameters["@file_type"].Value = file_type;
                    myCommand.Parameters["@id_out"].Direction = ParameterDirection.ReturnValue;
                    myCommand.Parameters["@account_no"].Value = account_no;
                    myCommand.Parameters["@currency_id"].Value = currency_id;
                    myCommand.Parameters["@branch_id"].Value = branch_id;
                    myCommand.Parameters["@ft_reference"].Value = ft_reference;
                    myCommand.Parameters["@rm_id"].Value = rm_id;
                    myCommand.Parameters["@delivery_date"].Value = delivery_date;
                    myCommand.Parameters["@processed_at_branch"].Value = processed_at_branch;
                    myCommand.Parameters["@cross_currency"].Value = cross_currency;
                    myCommand.Parameters["@cross_currency_rate"].Value = cross_currency_rate;



                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    proc = Convert.ToInt32(myCommand.Parameters["@id_out"].Value);
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Instruction inserted successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + account_no + ", Reference: " + reference + " Amount: " + amount + " File: " + file_name + " ", "", "0", 0, "Instruction inserted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Error inserting instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + account_no + ", Reference: " + reference + " Amount: " + amount + " File: " + file_name + " ", "", "0", 0, "Error inserting instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error inserting instruction", ex.Message);
                proc = 0;
            }
            //finally
            //{
            //    myConnection.Close();
            //}

            ////cleanup 
            //myConnection.Dispose();
            //myCommand.Dispose();

            //myConnection = null;
            //myCommand = null;

            return proc;
        }

        public string get_deivery_date()
        {
            string delivery_date = "";
            delivery_date = dtmDeliveryTime.Text + " " + drpDeliveryHour.SelectedItem.Text + ":" + drpDeliveryMinute.SelectedItem.Text + " " + drpAmPms.SelectedItem.Text;

            return delivery_date;
        }

        protected void btnAttach_Click(object sender, EventArgs e) //Handles btnAttach.Click
        {
            try
            {
                daccess.RunNonQuery1Where("Update", "instructions", new string[] { "start_attachment_date" }, new string[] { "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", txtInstructionID.Text);
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, ex.Message);
            }

            try
            {
                if (string.IsNullOrEmpty(txtTransactionReference.Text))
                {
                    alert.FireAlerts(this.Page, "Please generate ref");
                    return;
                }

                if (!FileUpload1.HasFile)
                {
                    alert.FireAlerts(this.Page, "Please attach a pdf file");
                    return;
                }

                //if(string.IsNullOrEmpty(txtSupportingFileName.Text) && drpInstructions.SelectedItem.Text.Contains("EFT"))
                //{
                //    alert.FireAlerts(this.Page, "Please upload an Excel sheet first using the Excel sheet section above.");
                //    return;
                //}
                if (!string.IsNullOrEmpty(txtFileName.Text))
                {
                    alert.FireAlerts(this.Page, "Another file/document already exists. It has been replaced");
                }


                if (!FileUpload1.FileName.ToLower().Contains(".pdf"))
                {
                    if (!drpInstructions.SelectedItem.Text.Contains("EFT"))
                    {
                        alert.FireAlerts(this.Page, "Upload error. You can only attach PDF documents in this section.");
                        return;
                    }

                }

                //DEV ME

                //only allows storing of one pdf file at a time.

                string fl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss") + FileUpload1.FileName.Replace(" ", "_");
                string path = Server.MapPath("..") + "/instructions/" + fl_name;
                path = path.Replace("\\", "/");
                FileUpload1.SaveAs(path);

                //string supportingfl_name = "";
                //string supportingPath = "";

                //if (supportingDocUpload.HasFile)
                //{
                //    supportingfl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss") + supportingDocUpload.FileName.Replace(" ", "_");
                //    supportingPath = Server.MapPath("..") + "/instructions/" + supportingfl_name;
                //    supportingPath = supportingPath.Replace("\\", "/");
                //    supportingDocUpload.SaveAs(supportingPath);
                //    txtSupportingFileName.Text = supportingfl_name;
                //}

                txtFileName.Text = fl_name;

                if (fl_name.ToLower().Contains(".pdf"))
                {
                    ShowPdf1.FilePath = "../instructions/" + fl_name;
                }

                if (save())
                {
                    //alert.FireAlerts(this.Page, path);
                }
                else
                {
                    alert.FireAlerts(this.Page, "Error attaching document");
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error attaching document");
                erl.LogError("Error attaching document", ex.Message);
            }

            try
            {
                daccess.RunNonQuery1Where("Update", "instructions", new string[] { "end_attachment_date" }, new string[] { "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", txtInstructionID.Text);
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error updating the instruction with the uploaded file " + ex.Message);
            }
        }

        protected void dgvInstructions_RowCommand(object sender, GridViewCommandEventArgs e) //Handles dgvInstructions.RowCommand
        {
            string rw = (e.CommandArgument).ToString();

            reset_client();
            reset_instructions();

            string id = dgvInstructions.Rows[Convert.ToInt32(rw)].Cells[23].Text;

            Load_instruction_details(Convert.ToInt32(id));
            load_comments(id); ;

            //drpValidationStatus_SelectedIndexChanged(null, null);

            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "../instructions/" + txtFileName.Text;
            }

            MultiView1.SetActiveView(ViewInstructions);


        }

        public void load_comments(string id)
        {
            try
            {
                sharedUtility.LoadGridView(dgvComments, genericFunctions.GetDataSourceUserGridViewInfo("instructions_comments", "instruction_id", id));
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading requirements", ex.Message);
                alert.FireAlerts(this.Page, "Error loading requirements.");
            }
        }

        protected void btnSave_Click1(object sender, EventArgs e)
        {
            string check_dups = check_for_duplicates(drpAccount.SelectedItem.Text, drpInstructions.SelectedValue, drpCurrency.SelectedValue, Convert.ToDouble(txtAmount.Text), txtInstructionID.Text);

            if (!string.IsNullOrEmpty(check_dups) && !chkAllowDuplicates.Checked)
            {
                alert.FireAlerts(this.Page, "Please note that Duplicate Instruction Exists. To continue anyway check *Allow Duplicates* and submit again.");

                //??? should get rid of this
                //Session["popup_sql"] = "Select * From instructions_view Where reference IN (" + check_dups.Replace("*", "'") + ")";
                openpopup("popup_view.aspx");
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Duplicate instruction detected with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Duplicate instruction detected", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                return;
            }

            if (save())
            {
                alert.FireAlerts(this.Page, "Save successful");
            }
            else
            {
                alert.FireAlerts(this.Page, "Error saving");
            }
        }

        public Boolean save()
        {
            if (!validate_generation_of_ref())
            {
                return false;
            }

            if (proc_update_instructions(Convert.ToInt32(Session["UserType"])))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string check_for_duplicates(string acc_no, string instruction_type, string currency, double amount, string instruction_id)
        {
            double value;
            if (!double.TryParse(Session["duplicate_check_days"].ToString(), out value))
            {
                Session["duplicate_check_days"] = 90;
            }

            int duplicate_check_days = Convert.ToInt32(Session["duplicate_check_days"]);
            string dups = "";

            DbDataReader rdr = null;
            rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("instructions", "*", new string[] { "account_no", "instruction_type_id", "currency_id", "amount" }, new string[] { acc_no, instruction_type, currency, amount.ToString() });

            int count = 0;
            try
            {
                while (rdr.Read())
                {
                    if (count == 0)
                    {
                        dups = " *" + rdr["reference"].ToString() + "*";
                    }
                    else
                    {
                        dups = dups + ", *" + rdr["reference"].ToString() + "*";
                    }
                    count = count + 1;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error checking for duplicate document", ex.Message);
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

            return dups;
        }

        public int check_for_duplicate_ft_reference(string ft, string instruction_id) //  
        {
            string dups = "";

            DbDataReader rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("instructions", "*", new string[] { "ft_reference", "instruction_id<>" }, new string[] { ft, instruction_id });
            int count = 0;

            try
            {
                while (rdr.Read())
                {
                    count = count + 1;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Err||checking f||duplicate reference document by user", ex.Message);
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

            return count;
        }

        protected void btnSubmit_Click(object sender, EventArgs e) //Handles btnvoidmit.Click
        {

            string is_referral = daccess.RunStringReturnStringValue1Where("document_status", "is_referral", "document_status_id", drpValidationStatus.SelectedValue);
            string is_document_held = daccess.RunStringReturnStringValue1Where("document_status", "is_document_held", "document_status_id", drpValidationStatus.SelectedValue);
            string is_exception = daccess.RunStringReturnStringValue1Where("document_status", "exception", "document_status_id", drpValidationStatus.SelectedValue);
            string must_comment = daccess.RunStringReturnStringValue1Where("document_status", "must_comment", "document_status_id", drpValidationStatus.SelectedValue);
            string is_delete = daccess.RunStringReturnStringValue1Where("document_status", "is_delete", "document_status_id", drpValidationStatus.SelectedValue);



            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                alert.FireAlerts(this.Page, "Please attach a file before submitting");

                return;
            }
            else if (string.IsNullOrEmpty(txtComments.Text))
            {
                Session["fileName"] = txtFileName.Text;
                alert.FireAlerts(this.Page, "Please add a comment before submitting");
                return;
            }

            //Added to fix a bug 28 Sep 2011
            //Get my user id
            string user_id = HttpContext.Current.Session["UserID"].ToString();

            //Check if I can perform this action
            bool can_submit = check_if_user_can_Submit(user_id, drpValidationStatus.SelectedValue);

            if (!can_submit)
            {
                alert.FireAlerts(this.Page, "You cannot perform the action. Please Log out then Back In and Try Again");
                //??? sharedUtility - no SendMail method
                //sharedUtility.SendMail("", "", "Submit Error", "You cannot perform the action. Please Log out then Back In and Try Again. User " & Session("UserFullName") & ". Action " & drpValidationStatus.SelectedItem.Text & " Reference " & txtTransactionReference.Text & "", "ongadid@stanbic.com", "", "", "html")
                return;
            }

            string saved_comments = "";

            if (!txtComments.ReadOnly && (Session["UserType"].ToString() == "1" || Session["UserType"].ToString() == "2"))
            {
                saved_comments = daccess.RunStringReturnStringValue1Where("instructions", "comments", "instruction_id", txtInstructionID.Text);
            }
            /*
            else if (txtPRMO1Comments.ReadOnly)
            {
                saved_comments = daccess.RunStringReturnStringValue1Where("instructions", "ftroa_comments", "instruction_id", txtInstructionID.Text);
            }
            else if (txtPRMOTLComments.ReadOnly == false)
            {
                saved_comments = daccess.RunStringReturnStringValue1Where("instructions", "ftrob_comments", "instruction_id", txtInstructionID.Text);
            }
            else if (txtProcessorComments.ReadOnly)
            {
                saved_comments = daccess.RunStringReturnStringValue1Where("instructions", "processor_comments", "instruction_id", txtInstructionID.Text);
            }
            else if (!txtRMComments.ReadOnly)
            {
                saved_comments = daccess.RunStringReturnStringValue1Where("instructions", "rm_comments", "instruction_id", txtInstructionID.Text);
            } */

            if (!validate_generation_of_ref())
            {
                if (drpValidationStatus.SelectedIndex <= 1)
                {
                    alert.FireAlerts(this.Page, "Please validate the document");
                    return;
                }
                return;
            }

            //The user teller is allowed to process
            if (chkProcessAtBranch.Checked && Session["UserType"].Equals("1"))
            {
                if (!validate_ft_ref(txtFTRef.Text))
                {
                    return;
                }
            }

            //Can I change the reference
            if (Session["UserType"].ToString() == "5" && txtStatus.Text.Equals("5"))
            {
                if (is_exception.Equals(1))
                {
                    //??? below comment already there?
                    //do not validate
                }
                else
                {
                    //??? do something here?
                }
                if (string.IsNullOrEmpty(txtFTRef.Text))
                {
                    alert.FireAlerts(this.Page, "Please type the EE reference");
                    return;
                }

                if (!validate_ft_ref(txtFTRef.Text))
                {
                    return;
                }

                if (check_for_duplicate_ft_reference(txtFTRef.Text, txtInstructionID.Text) > 0)
                {
                    alert.FireAlerts(this.Page, "The EE Ref already exists.");
                    return;
                }
            }

            //check for duplicates
            string check_dups = check_for_duplicates(drpAccount.SelectedItem.Text, drpInstructions.SelectedValue, drpCurrency.SelectedValue, double.Parse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat), txtInstructionID.Text);
            
            if (!string.IsNullOrEmpty(check_dups) && !chkAllowDuplicates.Checked && drpValidationStatus.SelectedValue != "12")
            {
                alert.FireAlerts(this.Page, "Please note that Duplicate Instruction(s) Exists. To continue anyway check *Allow Duplicates* and submit again.");
                //??? should remove this?
                //Session["popup_sql"] = "Select * From instructions_view Where reference IN (" + check_dups.Replace("*", "'") + ")";
                openpopup("popup_view.aspx");
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Duplicate instruction detected with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Duplicate instruction detected", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                return;
            }

            //heckifthe user can submit based on the amount. Otherwise he needs to refer
            double Max_amount = 0;
            double Min_amount = 0;

            string Rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_name", drpCurrency.SelectedItem.Text);

            double value;
            if (!double.TryParse(Rate, out value))
            {
                Rate = "1";
            }

            //Get the min and max amount
            string Max_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "maximum_amount", "User_Type_no", Session["UserType"].ToString());
            string Min_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "minimum_amount", "User_Type_no", Session["UserType"].ToString());

            double min, max;
            if (double.TryParse(Max_amount_text, out max) && double.TryParse(Min_amount_text, out min))
            {
                Max_amount = max;
                Min_amount = min;
            }

            if (Session["CheckLimit"].ToString() == "1")
            {
                if (is_referral != "1" && is_document_held != "1")
                {
                    double local_equivalance = (Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate));

                    if (local_equivalance > Max_amount)
                    {
                        alert.FireAlerts(this.Page, "Please Check if your Limit Allows You To *" + drpValidationStatus.SelectedItem.Text + "*");
                        return;
                    }
                    else
                    {
                        //??? what?
                        //Return ""
                    }
                }
                else
                {
                    //??? what?
                    //Checkifyou can refer To
                }
            }

            string check_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "check_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);

            if (check_submission_limit.Equals("1"))
            {
                string minimum_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "minimum_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                string maximum_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "maximum_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                string minimum_usd_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "minimum_usd_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                string maximum_usd_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "maximum_usd_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);

                if (drpCurrency.SelectedValue.Equals(1))
                {
                    double minsub, maxsub;
                    if (double.TryParse(minimum_submission_limit, out minsub) && double.TryParse(maximum_submission_limit, out maxsub))
                    {
                        double minimum_submission_limit_amount = minsub;
                        double maximum_submission_limit_amount = maxsub;
                        double local_equivalance = (Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate));

                        if (local_equivalance < minimum_submission_limit_amount || local_equivalance > maximum_submission_limit_amount)
                        {
                            alert.FireAlerts(this.Page, "The action is not allowed because of limit constraints. Please try another action.  ");
                            return;
                        }
                        else
                        {
                            //??? what?
                            //Return ""
                        }
                    }
                    else
                    {
                        //??? what?
                        //Check if you can refer To
                    }
                }
                else
                {
                    double minsub, maxsub;
                    if (double.TryParse(minimum_usd_submission_limit, out minsub) && double.TryParse(maximum_usd_submission_limit, out maxsub))
                    {
                        string usd_rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_id", "2");
                        double local_minimum_usd_submission_limit_amount = minsub * Convert.ToDouble(usd_rate);
                        double local_maximum_usd_submission_limit_amount = maxsub * Convert.ToDouble(usd_rate);
                        double local_equivalance = (Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate));

                        if (local_equivalance < local_minimum_usd_submission_limit_amount || local_equivalance > local_maximum_usd_submission_limit_amount)
                        {
                            alert.FireAlerts(this.Page, "The action is not allowed because of limit constraints. Please try another action. 2");
                            return;
                        }
                        else
                        {
                            //??? what?
                            //Return ""
                        }
                    }
                    else
                    {
                        //??? what?
                        //Check if you can refer To
                    }
                }
            }

            //Check if the user needs to insert call back comments
            if (Session["CanPerformCallBack"].Equals("1"))
            {
                if (is_referral == "0" && is_document_held != "1" && is_exception != "1")
                {
                    //if (string.IsNullOrEmpty(txtCallBackComment.Text))
                    {
                        alert.FireAlerts(this.Page, "Please Insert Call Back Comment");
                        return;
                    }

                    double val;
                    //if (txtCallBackNos.Text.Length < 7 || !double.TryParse(txtCallBackNos.Text, out val))
                    {
                        alert.FireAlerts(this.Page, "Please Insert a Valid Telephone No. Remove any non numeric characters that exist.");
                        return;
                    }
                }
                else
                {
                    //??? should something go here?
                }
            }

            //check if DOC is inserted
            if (Session["CanPerformDOC"].Equals(1))
            {
                if (drpDOC.SelectedIndex <= 0 && is_document_held != "1")
                {
                    alert.FireAlerts(this.Page, "Please Select DOC Comments");
                    return;
                }
                else
                {
                    //??? should something go here
                }
            }

            //save any changes
            save();

            //Cheeckif(it is the inserter (hardcoded). This is the branch and ftro user. //should be for anyone who can originate
            if (Session["UserType"].Equals(1) || Session["UserType"].Equals(3))
            {
                if (is_delete.Equals(1))
                {
                    //do nothing. It is being deleted
                }
                else if (string.IsNullOrEmpty(txtFileName.Text))
                {
                    alert.FireAlerts(this.Page, "Please attach a file before submitting");
                    return;
                }
            }

            //Check if(there is next stage inserted
            if (drpValidationStatus.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the document status");
                return;
            }

            //get the next stage
            int status = -1;


            string next_stage = daccess.RunStringReturnStringValue1Where("document_status", "document_status_stage", "document_status_id", drpValidationStatus.SelectedValue);

            string actionStatusText = daccess.RunStringReturnStringValue1Where("document_status", "document_status_action", "document_status_id", drpValidationStatus.SelectedValue);



            //get user to allocate to
            string allocate_to = "0";

            //get the instruction type e.g. RTGS = 1, OTT = 27
            int instructionType = (int)_db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).instruction_type_id;


            int userType = Convert.ToInt32(next_stage);

            //check cutoff time. If past cutoff time, then do not allocate to the next stage's user. Instead add a pastcutoff indicator to the instruction. 
            int pastCutoff; 

            if (CutOffCheck())
            {
                instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

                pastCutoff = 1;
                ins.past_cutoff_allocation = pastCutoff;

                _db.SaveChanges();

                
            }

            else if (actionStatusText.Contains("Return"))
            {
                allocate_to = ReturnToSender(userType).ToString();
            }

            else if (Session["UserType"].Equals(next_stage)) //assigns submission to the current user (must not allocate to themselves though)
            {
                //test
                allocate_to = GetUserWithLeastWork(userType, instructionType).ToString();
            }
            else
            {
                allocate_to = GetUserWithLeastWork(userType, instructionType).ToString(); //document status stage is the same as user type
                string usertype = daccess.RunStringReturnStringValue1Where("system_users", "system_user_type", "system_user_id", allocate_to.ToString());

                if (usertype == "1") //originator
                {
                    allocate_to = daccess.RunStringReturnStringValue1Where("instructions", "inserted_by", "instruction_id", txtInstructionID.Text);
                }
                //else if (usertype == "10") //processing officer team leader
                //{
                //    allocate_to = allocate_to = get_user_to_allocate(next_stage, Session["UserBranchID"].ToString()).ToString();
                //}

                if (allocate_to == "0" && next_stage != "21") //21 is the archive stage
                {
                    alert.FireAlerts(this.Page, "There are no users active to submit this instruction to.");
                    return;
                }
            }

            if (drpValidationStatus.SelectedIndex > 0)
            {
                string action = daccess.RunStringReturnStringValue1Where("document_status", "document_status", "document_status_id", drpValidationStatus.SelectedValue);
                if (is_exception.Equals(1))
                {
                    if (!txtComments.ReadOnly && (string.IsNullOrEmpty(txtComments.Text) || txtComments.Text == saved_comments && Session["UserType"].Equals(1) || Session["UserType"].Equals(2)))
                    {
                        alert.FireAlerts(this.Page, "Please Add/Insert Comments");
                        return;
                    }
                    /*
                    else if (!txtPRMO1Comments.ReadOnly && (string.IsNullOrEmpty(txtPRMO1Comments.Text) || txtPRMO1Comments.Text == saved_comments))
                    {
                        alert.FireAlerts(this.Page, "Please Add/Insert FTRO Comments");
                        return;
                    }
                    else if (!txtPRMOTLComments.ReadOnly && (string.IsNullOrEmpty(txtPRMOTLComments.Text) || txtPRMOTLComments.Text == saved_comments))
                    {
                        alert.FireAlerts(this.Page, "Please Add/Insert FTRO Comments");
                        return;
                    }
                    else if (!txtProcessorComments.ReadOnly && (string.IsNullOrEmpty(txtProcessorComments.Text) || txtProcessorComments.Text == saved_comments))
                    {
                        alert.FireAlerts(this.Page, "Please Add/Insert Process||Comments");
                        return;
                    }
                    else if (!txtRMComments.ReadOnly && (string.IsNullOrEmpty(txtRMComments.Text) || txtRMComments.Text == saved_comments))
                    {
                        alert.FireAlerts(this.Page, "Please Add/Insert RM/RO Comments");
                        return;
                    }
                    */
                }
            }


            if (proc_submit_and_allocate_instructions(Convert.ToInt32(next_stage), Convert.ToInt32(Session["UserType"].ToString()), Convert.ToInt32(allocate_to), Convert.ToInt32(drpValidationStatus.SelectedValue), Convert.ToInt32(is_referral)))
            {
                int allocation = Convert.ToInt32(allocate_to);
                system_users allocatedUser = _db.system_users.FirstOrDefault(u => u.system_user_id == allocation);
                if(allocatedUser != null)
                {
                    alert.FireAlerts(this.Page, drpValidationStatus.SelectedItem.Text + ": " + allocatedUser.system_user_login + " (" + allocatedUser.system_user_fname + " " + allocatedUser.system_user_lname + ")" + " successfully");
                }
                else if(CutOffCheck())
                {
                    alert.FireAlerts(this.Page, drpValidationStatus.SelectedItem.Text + ": submitted after cutoff time. Instruction will be allocated tomorrow.");
                }

                daccess.RunNonQuery1Where("Update", "instructions", new string[] { "locked_by " }, new string[] { "0" }, "instruction_id", txtInstructionID.Text);

                btnView_Click(null, null);
                MultiView1.SetActiveView(ViewCustomerList);

                string receipient = daccess.RunStringReturnStringValue1Where("relationship_managers", "RM_Email", "rm_id", drpRM.SelectedItem.Value);
                string cc = daccess.RunStringReturnStringValue1Where("relationship_managers", "R_Officer", "rm_id", drpRM.SelectedItem.Value);

                //??? sharedUtility - missing method
                //if (!sharedUtility.is_this_a_valid_email_adress(receipient))
                //{
                //    receipient = "ongadid@stanbic.com";
                //}

                //??? sharedutility - missing method
                //if (!sharedUtility.is_this_a_valid_email_adress(cc))
                //{
                //    cc = My.Settings.cc;
                //}

                //??? My
                //string file_ = My.Settings.instructions_location + "\\" + txtFileName.Text;
                string comments1 = "";
                string referred_by = Session["UserFullName"].ToString();
                string clients_name = txtClient_Name.Text;
                string instruction_type = drpInstructions.SelectedItem.Text;
                //string instruction_date = daccess.RunStringReturnStringValue1Where("instructions", "inserted_date", "instruction_id", txtInstructionID.Text);
                string instruction_date = daccess.RunStringReturnStringValue1Where("instructions", "Convert(nvarchar(20),inserted_date,100)", "instruction_id", txtInstructionID.Text);

                if (!txtComments.ReadOnly)
                {
                    comments1 = txtComments.Text; //= action & " by " & Session("UserInitials") & " on " & Format(DateTime.Now(), "dd-MMM-yyyy hh:mm tt");
                }
                /*
                else if ((!txtPRMO1Comments.ReadOnly))
                {
                    comments1 = txtPRMO1Comments.Text; //'= action & " by " & Session("UserInitials") & " on " & Format(DateTime.Now(), "dd-MMM-yyyy hh:mm tt")
                                                       //referred_by = Session("UserFullName")
                }
                else if ((!txtPRMOTLComments.ReadOnly))
                {
                    comments1 = txtPRMOTLComments.Text; //'= action & " by " & Session("UserInitials") & " on " & Format(DateTime.Now(), "dd-MMM-yyyy hh:mm tt")
                }
                else if ((!txtProcessorComments.ReadOnly))
                {
                    comments1 = txtProcessorComments.Text; //'= action & " by " & Session("UserInitials") & " on " & Format(DateTime.Now(), "dd-MMM-yyyy hh:mm tt")
                }
                else if ((!txtRMComments.ReadOnly))
                {
                    comments1 = txtRMComments.Text; //'= action & " by " & Session("UserInitials") & " on " & Format(DateTime.Now(), "dd-MMM-yyyy hh:mm tt")
                } */

                string comments = "  " + Environment.NewLine +
                             " <table border= 1> <tr> " + Environment.NewLine +
                             " 	<td>Client Name</td><td>" + clients_name + "</td>" + Environment.NewLine +
                             " </tr>" +
                              "<tr>" + Environment.NewLine +
                             " 	<td>Instruction Type</td><td>" + instruction_type + "</td>" + Environment.NewLine +
                             " </tr>" +
                              "<tr>" + Environment.NewLine +
                             " 	<td>Instruction Date</td><td>" + instruction_date + "</td>" + Environment.NewLine +
                             " </tr>" +
                               "<tr>" + Environment.NewLine +
                             " 	<td>Referred By</td><td>" + referred_by + "</td>" + Environment.NewLine +
                             " </tr>" +
                               "<tr>" + Environment.NewLine +
                             " 	<td>Referral Reason</td><td>" + comments1 + "</td>" + Environment.NewLine +
                             " </tr></table>";

                //Send Email
                if (is_exception.Equals(1))
                {
                    //Who to refer to

                    //now send email

                    //sharedUtility.SendMail(cc, "", "INSTRUCTION EXCEPTION", build_mail_header() + build_mail_body(comments) + build_mail_footer(), receipient, "high", file_, "html");
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Instruction with the following details haa been referred (" + drpValidationStatus.SelectedItem.Text + ") : Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction referred", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }

                //Who to refer to
                string email_to = get_email_of_user_referred_to(txtInstructionID.Text, drpValidationStatus.SelectedValue);
                string delivery_date = daccess.RunStringReturnStringValue1Where("instructions", "delivery_date", "instruction_id", txtInstructionID.Text);
                string reference = daccess.RunStringReturnStringValue1Where("instructions", "reference", "instruction_id", txtInstructionID.Text);

                //now send email
                try
                {
                    sharedUtility.SendMail(email_to,
                                       "CIMS INSTRUCTION " + reference + " " + System.DateTime.Now,
                                       //email_to + "\r\n\r\n" +
                                       "The instruction below requires your attention before the following delivery date: " + delivery_date + "\r\n\r\n" +

                                       "    Customer No.        : " + txtClient_Customer_Number.Text + "\r\n" +
                                       "    Customer Name       : " + txtClient_Name.Text + "\r\n" +
                                       "    Customer Account No.: " + drpAccount.SelectedItem.Text + "\r\n" +

                                       "    Instruction Type    : " + instruction_type + " - " + drpValidationStatus.SelectedItem.Text + "\r\n" +
                                       "    Instruction ID      : " + txtInstructionID.Text + "\r\n" +
                                       "    Instruction Comment : " + comments1 + "\r\n" +
                                       "    Refered by          : " + referred_by + "\r\n" +
                                       "    Originating Branch  : " + drpBranchs.SelectedItem.Text);
                }
                catch (Exception ex)
                {
                    //??? log error?
                    erl.LogError("Failed to send email notification to " + email_to, ex.Message);
                    alert.FireAlerts(this.Page, "Failed to send email notification to " + email_to + "\r\nPlease notify them manually.");
                }

                //refresh
                btnNew_Click(null, null);
            }
            else
            {
                alert.FireAlerts(this.Page, "Error submitting instruction");
                erl.LogError("", "Error submitting instruction by user: " + Session["UserFullName"] + "Err||submitting instruction" + "check_for_duplicates()");
            }
        }



        public Boolean validate_ft_ref(string ft_ref)   //Validates EE reference
        {
            if (ft_ref.Length < 12)
            {
                alert.FireAlerts(this.Page, "Wrong EE Reference. It should be at le t 12 Characters.");
                return false;
            }

            if (ft_ref.Length > 20)
            {
                alert.FireAlerts(this.Page, "Wrong EE Reference. It should be Less than 20 Characters");
                return false;
            }

            return true;
        }
        #region mail methods

        public string get_email_of_user_referred_to(string instruction_id, string document_status_id)   //String
        {
            string user_email = "";
            string sql = "";
            string stage = daccess.RunStringReturnStringValue1Where("document_status", "document_status_stage", "docuemtn_status_id", document_status_id);
            string column = "";

            switch (stage)
            {

                case "1":
                    column = "branch_proccessed_by";
                    break;

                case "2":
                    column = "branch_approved_by";
                    break;

                case "3":
                    column = "ftro_proccessed_by";
                    break;

                case "4":
                    column = "prmo2_proccessed_by";
                    break;

                case "5":
                    column = "processor_proccessed_by";
                    break;

                case "6":
                    column = "processor_approved_by";
                    break;

                case "10":
                    column = "processor_approved_by";
                    break;

                case "13":
                    column = "prmo2_proccessed_by";
                    break;

                case "14":
                    column = "prmu_tl_proccessed_by";
                    break;

                case "15":
                    column = "POTL_proccessed_by";
                    break;

                default:
                    column = "allocated_to";
                    break;
            }

            user_email = daccess.RunStringReturnStringValueIN("system_users", "system_user_email", "system_user_id", "instructions", column, "instruction_id", instruction_id);

            return user_email;
        }

        public string build_mail_header()   //String
        {
            string myString = "";
            myString = " <table> <br> <tr>" + Environment.NewLine +
                        " 	<td>Dear Team," + Environment.NewLine +
                        " </tr>  <table>";
            return myString;
        }

        public string build_mail_body(string comments)
        {
            string myString = "";
            myString = " <table><br> <tr>" +
                    " 	<td>Please note that there is an issue with the  attached instruction and it has been referred.<br> <br>See details below<br> <br>" + comments + "<br> <br>" +
                    " </tr></table>";
            return myString;
        }

        public string build_mail_footer()
        {
            string myString = "";
            myString = " <table><br><tr> " +
                        " 	<td>Kind Regards,<br> <br>CIMS. &nbsp;" +
                        " </tr></table> ";
            return myString;
        }

        #endregion

        #region Instruction Queueing Methods
        //Previously, the user with the lowest ID value just got assigned instructions as they finished other instructions, leaving
        //users with high ID values without work and other users with lots of work. 
        //There should be a queue for work. When you receive an instruction you go to the back of the queue.
        public int GetUserWithLeastWork(int userTypeId, int instructionTypeId, int instructionId)
        {
            int assignedUserId = 0;

            //get branchcode based on the branch user of the instruction.
            instruction currentIns = _db.instructions.FirstOrDefault(i => i.instruction_id == instructionId);

            int branchIdVal = (int)currentIns.branch_id;

            //get all the allocations (records where a system_user ID is allocated to an instruction type)
            IQueryable<instruction_type_allocations> allocations = from allo in _db.instruction_type_allocations
                                                                   where allo.instruction_type_id == instructionTypeId &&
                                                                   allo.system_users.system_user_type == userTypeId &&
                                                                   allo.status == 1
                                                                   select allo;

            if (allocations.Count() == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }


            //get all ACTIVE users with the requested USERTYPE and BRANCH CODE 
            //List those users firstly by TOTALALLOCATEDWORK and secondly by ID
            IQueryable<system_users> userList = (from users in _db.system_users
                                                 where users.system_user_type == userTypeId &&
                                                 users.system_user_branch == branchIdVal &&
                                                 users.system_user_active == 1
                                                 select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);


            //if there aren't any available users from that branch
            if (userList.Count() == 0)
            {
                userList = (from users in _db.system_users
                            where users.system_user_type == userTypeId &&
                            users.system_user_active == 1
                            select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);
            }

            if (userList.Count() == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }
            //remove the current user from the userList because you don't want to end up sending to yourself RIP segregation of duties

            List<system_users> userListLessLoggedInUser = new List<system_users>();
            assignedUserId = Convert.ToInt32(Session["UserID"].ToString());

            if (userList.Count() > 0)
            {
                foreach (system_users user in userList)
                {
                    if (user.system_user_id != assignedUserId)
                    {
                        userListLessLoggedInUser.Add(user);
                    }
                }
            }
            List<system_users> allocatedUserList = new List<system_users>();

            //now compare the list of users who are active and of the right type with users with the right instruction type allocation. and make a list with the ones that match.
            if (userListLessLoggedInUser.Count() > 0)
            {
                foreach (system_users user in userListLessLoggedInUser)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userListLessLoggedInUser.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }
            else
            {
                foreach (system_users user in userList)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userList.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }

            //count of how many instructions are currently allocated to a user
            int allocatedWorkCount = 0;
            int maxTotalWork = 0;
            if (allocatedUserList.Count() == 0)
            {
                maxTotalWork = (int)allocatedUserList.Max(u => u.total_work_allocated);
            }

            //get a list of all the instructions
            var instructions = from ins in _db.instructions
                               select ins;

            int lowestAllocation = instructions.Count(); //You can't be allocated more work than there are instructions

            //loop through the list to find the user with the least to do or just give work to the first person with nothing to do. 



            //select all users with the lowest allocated work count (from instructions list) and lowest total work count attribute
            List<int> workCountList = new List<int>();

            foreach (system_users user in allocatedUserList) //users are sorted by historical work count
            {
                int count;
                //get all the allocated work counts into a list so we can find the minimum.
                count = instructions.Count(i => i.allocated_to == user.system_user_id);
                workCountList.Add(count);
            }
            //refine the list for those users with the lowest total work allocated
            List<system_users> lowestAllocationUsers = new List<system_users>();

            //add all the users with the lowest allocated work counts to the list
            foreach (system_users user in allocatedUserList)
            {

                //get the count for the current user in the outer loop
                allocatedWorkCount = instructions.Count(i => i.allocated_to == user.system_user_id);

                //then compare it with the list of work counts
                if (allocatedWorkCount == workCountList.Min()) //add the user if their allocated workcount is the minimum. 
                {
                    lowestAllocationUsers.Add(user);
                }
            }

            int minTotalWork = (int)allocatedUserList.Min(u => u.total_work_allocated);

            //if historical total work is higher in the lowest allocation users than in the bigger allocated user list, use the minimum of the lowest allocation users
            if (lowestAllocationUsers.Min(u => u.total_work_allocated) >= allocatedUserList.Min(u => u.total_work_allocated))
            {
                minTotalWork = (int)lowestAllocationUsers.Min(u => u.total_work_allocated);
            }
            //from the users with lowest allocated work, get the users with the lowest historical allocated work
            var usersWithLeastTotalWork = lowestAllocationUsers.ToList().Where(u => u.total_work_allocated == minTotalWork);

            //select the first user in this list as the assigned user for the instruction. 
            if (usersWithLeastTotalWork.Count() > 0)
            {
                assignedUserId = (int)usersWithLeastTotalWork.FirstOrDefault().system_user_id;

            }
            else if (lowestAllocationUsers.Count() > 0)
            {
                assignedUserId = (int)lowestAllocationUsers.FirstOrDefault().system_user_id;
            }
            else if (allocatedUserList.Count() > 0)
            {
                assignedUserId = (int)allocatedUserList.FirstOrDefault().system_user_id;
            }
            else
            {
                //if there isn't anyone left to assign to, assign the instruction to the current logged in user. :(
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
            }

            return assignedUserId;

        }
        public int GetUserWithLeastWork(int userTypeId, int instructionTypeId)
        {
            int assignedUserId = 0;

            //get branchcode based on originator branch code
            string branchId = (Session["UserBranchID"].ToString());

            int branchIdVal = Convert.ToInt32(branchId);
            var branchCode = _db.user_branch.FirstOrDefault(b => b.branch_id == branchIdVal).branch_code;

            //get all the allocations (records where a system_user ID is allocated to an instruction type)
            IQueryable<instruction_type_allocations> allocations = from allo in _db.instruction_type_allocations
                                                                   where allo.instruction_type_id == instructionTypeId &&
                                                                   allo.system_users.system_user_type == userTypeId &&
                                                                   allo.status == 1
                                                                   select allo;

            if (allocations.Count() == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }
               

            //get all ACTIVE users with the requested USERTYPE and BRANCH CODE 
            //List those users firstly by TOTALALLOCATEDWORK and secondly by ID
            IQueryable<system_users> userList = (from users in _db.system_users
                                                 where users.system_user_type == userTypeId &&
                                                 users.system_user_branch_code == branchCode.ToString() &&
                                                 users.system_user_active == 1
                                                 select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);


            //if there aren't any available users from that branch
            if (userList.Count() == 0)
            {
                userList = (from users in _db.system_users
                            where users.system_user_type == userTypeId &&
                            users.system_user_active == 1
                            select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);
            }

            if(userList.Count() == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }
            //remove the current user from the userList because you don't want to end up sending to yourself RIP segregation of duties

            List<system_users> userListLessLoggedInUser = new List<system_users>();
            assignedUserId = Convert.ToInt32(Session["UserID"].ToString());

            if (userList.Count() > 0)
            {
                foreach (system_users user in userList)
                {
                    if (user.system_user_id != assignedUserId)
                    {
                        userListLessLoggedInUser.Add(user);
                    }
                }
            }
            List<system_users> allocatedUserList = new List<system_users>();

            //now compare the list of users who are active and of the right type with users with the right instruction type allocation. and make a list with the ones that match.
            if (userListLessLoggedInUser.Count() > 0)
            {
                foreach (system_users user in userListLessLoggedInUser)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userListLessLoggedInUser.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }
            else
            {
                foreach (system_users user in userList)
                {
                    long userId = user.system_user_id; //get the user's ID
                    long acceptedUserId = 0;
                    if (allocations.FirstOrDefault(a => a.system_user_id == userId) != null)
                    {
                        acceptedUserId = (long)allocations.FirstOrDefault(a => a.system_user_id == userId).system_user_id; //check if the user is allocated to this instruction type
                    }
                    if (acceptedUserId == userId)
                    {
                        //add the allocated, active user of the correct user type to the new list of users
                        allocatedUserList.Add(userList.FirstOrDefault(u => u.system_user_id == acceptedUserId));
                    }
                }
            }

            //count of how many instructions are currently allocated to a user
            int allocatedWorkCount = 0;
            int maxTotalWork = 0;
            if(allocatedUserList.Count() == 0)
            {
                maxTotalWork = (int)allocatedUserList.Max(u => u.total_work_allocated);
            }

            //get a list of all the instructions
            var instructions = from ins in _db.instructions
                               select ins;

            int lowestAllocation = instructions.Count(); //You can't be allocated more work than there are instructions

            //loop through the list to find the user with the least to do or just give work to the first person with nothing to do. 



            //select all users with the lowest allocated work count (from instructions list) and lowest total work count attribute
            List<int> workCountList = new List<int>();

            foreach (system_users user in allocatedUserList) //users are sorted by historical work count
            {
                int count;
                //get all the allocated work counts into a list so we can find the minimum.
                count = instructions.Count(i => i.allocated_to == user.system_user_id);
                workCountList.Add(allocatedWorkCount);
            }
            //refine the list for those users with the lowest total work allocated
            List<system_users> lowestAllocationUsers = new List<system_users>();

            //add all the users with the lowest allocated work counts to the list
            foreach (system_users user in allocatedUserList)
            {

                //get the count for the current user in the outer loop
                allocatedWorkCount = instructions.Count(i => i.allocated_to == user.system_user_id);

                //then compare it with the list of work counts
                if (allocatedWorkCount == workCountList.Min()) //add the user if their allocated workcount is the minimum. 
                {
                    lowestAllocationUsers.Add(user);
                }
            }

            int minTotalWork = (int)allocatedUserList.Min(u => u.total_work_allocated);

            //if historical total work is higher in the lowest allocation users than in the bigger allocated user list, use the minimum of the lowest allocation users
            if (lowestAllocationUsers.Min(u => u.total_work_allocated) >= allocatedUserList.Min(u => u.total_work_allocated))
            {
                minTotalWork = (int)lowestAllocationUsers.Min(u => u.total_work_allocated);
            }
            //from the users with lowest allocated work, get the users with the lowest historical allocated work
            var usersWithLeastTotalWork = lowestAllocationUsers.ToList().Where(u => u.total_work_allocated == minTotalWork);

            //select the first user in this list as the assigned user for the instruction. 
            if (usersWithLeastTotalWork.Count() > 0)
            {
                assignedUserId = (int)usersWithLeastTotalWork.FirstOrDefault().system_user_id;

            }
            else if (lowestAllocationUsers.Count() > 0)
            {
                assignedUserId = (int)lowestAllocationUsers.FirstOrDefault().system_user_id;
            }
            else if (allocatedUserList.Count() > 0)
            {
                assignedUserId = (int)allocatedUserList.FirstOrDefault().system_user_id;
            }
            else
            {
                //if there isn't anyone left to assign to, assign the instruction to the current logged in user. :(
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
            }

            return assignedUserId;

        }

        public int ReturnToSender(int userTypeId)
        {
            int allocate_to = 0;
            //find instruction
            instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);
            int instructionType = (int)ins.instruction_type_id;
            //get a list of users that have acted on the instruction
            //every user that has commented on the instruction has done something in the instruction. so get a list of the commenters

            List<system_users> users = new List<system_users>();
            var idValues = from c in _db.instruction_comments
                           where c.instruction_id == ins.instruction_id
                           select c.instruction_comment_by;

            //find users that are of the correct type and match the ids of the commenters
            foreach (int val in idValues)
            {
                system_users tempUser = new system_users();

                tempUser = _db.system_users.FirstOrDefault(u => u.system_user_id == val && u.system_user_type == Convert.ToInt32(userTypeId));
                if (tempUser != null)
                {
                    users.Add(tempUser);
                }
            }

            //send to the user that matches the required type of the next_stage
            if (users.Count() > 0)
            {
                allocate_to = (int)users.FirstOrDefault().system_user_id;
            }
            else
            {
                allocate_to = GetUserWithLeastWork(userTypeId, instructionType);
            }
            return allocate_to;
        }
        public int get_user_to_allocate(string status) //document status
        {
            //this method loops through active users of the desired type
            //and selects the user with the least amount of work
            int least_allocation = 0;
            int least_user_id = 0;
            int count = 0;
            int tot_allocation = 0;

            //get all users of desired type that are active
            DbDataReader rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("system_users", "*", new string[] { "system_user_type", "system_user_active" }, new string[] { status, "1" });

            try
            {
                while (rdr.Read())
                {
                    //sets current_user_id to the one the reader is reading


                    string current_user_id = rdr["system_user_id"].ToString();

                    //see how many are allocated to that user, if null, use 0. 
                    string allocation = daccess.RunStringReturnStringValue1Where("instructions", "ISNULL(Count(*), 0)", "allocated_to", current_user_id);

                    double value;
                    if (double.TryParse(allocation, out value))
                    {
                        tot_allocation = Convert.ToInt32(value);
                    }

                    //initialize
                    if (count == 0) //wtf. in the lines above we make this value always be zero XD #CIMS thus it will always 
                    {
                        least_allocation = tot_allocation; //
                        least_user_id = Convert.ToInt32(current_user_id); //the current user becomes the user with the least work
                    }

                    //check
                    if (tot_allocation < least_allocation)
                    {
                        least_allocation = tot_allocation;
                        least_user_id = Convert.ToInt32(current_user_id);
                    }

                    count = count + 1;
                }

            }
            catch (Exception ex)
            {
                //??? log error?
            }
            finally
            {
                if (rdr != null)
                {
                    if (!rdr.IsClosed)
                    {
                        rdr.Close();
                    }
                }
            }

            return least_user_id;
        }

        public int get_user_to_allocate(string status, string branch) //gets the active branch user to which to allocate the instruction
        {
            DAccessInfo daccess = new DAccessInfo();
            int least_allocation = 0;
            int least_user_id = 0;
            int count = 0;
            int tot_allocation = 0;
            DbDataReader rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("system_users", "*", new string[] { "system_user_type", "system_user_branch", "system_user_active" }, new string[] { status, branch, "1" });

            try
            {
                while (rdr.Read())
                {
                    string current_user_id = rdr["system_user_id"].ToString();
                    string allocation = daccess.RunStringReturnStringValue1Where("instructions", "ISNULL(Count(*), 0)", "allocated_to", current_user_id);

                    double value;
                    if (double.TryParse(allocation, out value))
                    {
                        tot_allocation = Convert.ToInt32(value);
                    }

                    //initialize
                    if (count == 0)
                    {
                        least_allocation = tot_allocation;
                        least_user_id = Convert.ToInt32(current_user_id);
                    }

                    //check
                    if (tot_allocation < least_allocation)
                    {
                        least_allocation = tot_allocation;
                        least_user_id = Convert.ToInt32(current_user_id);
                    }

                    count = count + 1;
                }
            }
            catch (Exception ex)
            {
                //??? log error?
            }
            finally
            {
                if (rdr != null)
                {
                    if (!rdr.IsClosed)
                    {
                        rdr.Close();
                    }
                }
            }

            return least_user_id;
        }

        #endregion

        public Boolean check_if_user_can_Submit(string user_id, string document_status_id)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("system_users", "system_user_type", "system_user_id", user_id);

            int system_user_type = 0;

            try
            {
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        system_user_type = Convert.ToInt32(rdr["system_user_type"]);

                        if (daccess.HowManyRecordsExist2Wheres("document_status", "document_status_user_type_who_can_action", system_user_type.ToString(), "document_status_id", document_status_id) > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //??? log error?
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


            return false;
        }

        public Boolean proc_submit_and_allocate_instructions(int status, int user_type, int allocated_to, int document_status_id, int is_referral)
        {
            bool proc;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {
                    proc = false;
                    SqlCommand myCommand = null;
                    myCommand = new SqlCommand("proc_submit_and_allocate_instructions", myConnection);
                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    int instruction_id = Convert.ToInt32(txtInstructionID.Text);
                    DateTime allocated_date = Convert.ToDateTime("1/1/1900");
                    string user_id = Session["UserID"].ToString();
                    int instruction_status = Convert.ToInt32(drpValidationStatus.SelectedValue);

                    myCommand.Parameters.Add("@instruction_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@status", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_to", SqlDbType.Int);
                    myCommand.Parameters.Add("@allocated_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@instruction_status", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_type", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@is_referral", SqlDbType.Int);

                    myCommand.Parameters["@instruction_id"].Value = instruction_id;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@allocated_to"].Value = allocated_to;
                    myCommand.Parameters["@allocated_date"].Value = allocated_date;
                    myCommand.Parameters["@instruction_status"].Value = instruction_status;
                    myCommand.Parameters["@user_type"].Value = user_type;
                    myCommand.Parameters["@user_id"].Value = user_id;
                    myCommand.Parameters["@document_status_id"].Value = document_status_id;
                    myCommand.Parameters["@is_referral"].Value = is_referral;


                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    proc = true;
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Instruction submitted (" + drpValidationStatus.SelectedItem.Text + ") successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Instruction submitted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);

                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Err||voidmitting (" + drpValidationStatus.SelectedItem.Text + ") instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text + " ", "", "0", 0, "Error submitting instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error submitting instruction", ex.Message);
                proc = false;
            }
            //finally
            //{
            //    myConnection.Close();
            //}

            ////cleanup 
            //myConnection.Dispose();
            //myCommand.Dispose();

            //myConnection = null;
            //myCommand = null;

            return proc;
        }

        public Boolean proc_update_instructions(int status)  /// Boolean
        {
            bool proc;
            string account_no = "0";
            if (drpInstructions.SelectedValue != "13")
            {
                account_no = drpAccount.SelectedItem.Text;
            }
            double amount = double.Parse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat);
            string reference = txtTransactionReference.Text;
            string file_name = txtFileName.Text;

            try
            {


                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {


                    SqlCommand myCommand = null;
                    proc = false;

                    myCommand = new SqlCommand("proc_update_instructions", myConnection);
                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    string comments = txtComments.Text;

                    int instruction_type_id = Convert.ToInt32(drpInstructions.SelectedValue);
                    DateTime inserted_date = Convert.ToDateTime("1/1/1900");
                    int inserted_by = Convert.ToInt32(Session["UserID"]);
                    DateTime modified_date = Convert.ToDateTime("1/1/1900");
                    int modified_by = Convert.ToInt32(Session["UserID"]);
                    int client_id = Convert.ToInt32(txtClientID.Text);
                    
                    string file_type = "";
                    int instruction_id = Convert.ToInt32(txtInstructionID.Text);
                    //string ftroa_comments = txtPRMO1Comments.Text;
                    //string prmo2_comments = txtPRMO2Comments.Text;
                    //string ftrob_comments = txtPRMOTLComments.Text;
                    string processor_comments = "";
                    string rm_comments = "";

                    int currency_id = Convert.ToInt32(drpCurrency.SelectedValue);
                    int branch_id = Convert.ToInt32(drpBranchs.SelectedValue);
                    string ft_reference = txtFTRef.Text;
                    string related_reference = "";
                    int rm_id = Convert.ToInt32(drpRM.SelectedValue);
                    //string call_back_comments = txtCallBackComment.Text;
                    //string call_back_no = txtCallBackNos.Text;
                    int doc_comments_id = Convert.ToInt32(drpDOC.SelectedValue);
                    DateTime delivery_date = Convert.ToDateTime("1/1/1900");
                    int processed_at_branch = Convert.ToInt16(chkProcessAtBranch.Checked);
                    int cross_currency = Convert.ToInt32(drpCrossCurrency.SelectedValue);
                    double cross_currency_rate = double.Parse(txtCCRate.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat);

                    delivery_date = Convert.ToDateTime(get_deivery_date());

                    myCommand.Parameters.Add("@amount", SqlDbType.Float);
                    myCommand.Parameters.Add("@comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@reference", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@file_name", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@instruction_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@instruction_type_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@modified_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@modified_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@status", SqlDbType.Int);
                    myCommand.Parameters.Add("@client_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@file_type", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@ftroa_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@ftrob_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@processor_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@rm_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@account_no", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@currency_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@branch_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@ft_reference", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@rm_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@related_reference", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@call_back_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@doc_comments_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@prmo2_comments", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@call_back_no", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@delivery_date", SqlDbType.DateTime);
                    myCommand.Parameters.Add("@processed_at_branch", SqlDbType.Int);
                    myCommand.Parameters.Add("@cross_currency", SqlDbType.Int);
                    myCommand.Parameters.Add("@cross_currency_rate", SqlDbType.Float);

                    myCommand.Parameters["@amount"].Value = amount;
                    myCommand.Parameters["@comments"].Value = comments;
                    myCommand.Parameters["@reference"].Value = reference;
                    myCommand.Parameters["@file_name"].Value = file_name;
                    myCommand.Parameters["@instruction_id"].Value = instruction_id;
                    myCommand.Parameters["@instruction_type_id"].Value = instruction_type_id;
                    myCommand.Parameters["@modified_date"].Value = modified_date;
                    myCommand.Parameters["@modified_by"].Value = modified_by;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@client_id"].Value = client_id;
                    myCommand.Parameters["@ftroa_comments"].Value = "";
                    myCommand.Parameters["@ftrob_comments"].Value = "";
                    myCommand.Parameters["@processor_comments"].Value = processor_comments;
                    myCommand.Parameters["@rm_comments"].Value = rm_comments;
                    myCommand.Parameters["@file_type"].Value = file_type;
                    myCommand.Parameters["@account_no"].Value = account_no;
                    myCommand.Parameters["@currency_id"].Value = currency_id;
                    myCommand.Parameters["@branch_id"].Value = branch_id;
                    myCommand.Parameters["@ft_reference"].Value = ft_reference;
                    myCommand.Parameters["@rm_id"].Value = rm_id;
                    myCommand.Parameters["@related_reference"].Value = related_reference;
                    myCommand.Parameters["@call_back_comments"].Value = "";
                    myCommand.Parameters["@doc_comments_id"].Value = doc_comments_id;
                    myCommand.Parameters["@prmo2_comments"].Value = "";
                    myCommand.Parameters["@call_back_no"].Value = "";
                    myCommand.Parameters["@delivery_date"].Value = delivery_date;
                    myCommand.Parameters["@processed_at_branch"].Value = processed_at_branch;
                    myCommand.Parameters["@cross_currency"].Value = cross_currency;
                    myCommand.Parameters["@cross_currency_rate"].Value = cross_currency_rate;


                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    proc = true;
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Instruction updated successfully with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + account_no + ", Reference: " + reference + " Amount: " + amount + " File: " + file_name + " ", "", "0", 0, "Instruction updated successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Err||updating instruction with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + account_no + ", Reference: " + reference + " Amount: " + amount + " File: " + file_name + " ", "", "0", 0, "Error updating instructions", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error updating instruction", ex.Message);
                proc = false;
            }
            //finally
            //{
            //    myConnection.Close();
            //}

            ////cleanup 
            //myConnection.Dispose();
            //myCommand.Dispose();

            //myConnection = null;
            //myCommand = null;

            return proc;

        }
        private Boolean CutOffCheck()
        {
            bool check = false;
            try
            {

                int instructionTypeId = Convert.ToInt32(drpInstructions.SelectedValue);

                DateTime currentTime = DateTime.Parse(DateTime.Now.ToString());

                string cutOffTime = _db.instructions_types.FirstOrDefault(t => t.instruction_type_ID == instructionTypeId).cutt_off_time;

                if (currentTime > DateTime.Parse(cutOffTime))
                {
                    check = true;

                }

            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error checking cutoff " + ex.Message);
            }


            return check;

        }
        protected void drpInstructions_SelectedIndexChanged(object sender, EventArgs e) //Handles drpInstructions.SelectedIndexChanged
        {
            load_flow_dropdown();
            toggle_fields();
            set_default_values();


        }

        public void toggle_fields()
        {
            //string show_currency = daccess.RunStringReturnStringValue1Where("instructions", "show_currency", "instruction_type_id", drpInstructions.SelectedValue);
            //string show_cross_currency = daccess.RunStringReturnStringValue1Where("instructions", "show_cross_currency", "instruction_type_id", drpInstructions.SelectedValue);
            //string show_amount = daccess.RunStringReturnStringValue1Where("instructions_types", "show_amount", "instruction_type_id", drpInstructions.SelectedValue);

            //if (show_currency.Equals("1"))
            //{
            //    trCurrency.Visible = true;
            //}
            //else
            //{
            //    trCurrency.Visible = false;
            //}

            //if (show_amount.Equals("1"))
            //{
            //    trAmount.Visible = true;
            //}
            //else
            //{
            //    trAmount.Visible = false;
            //}

            //if (show_cross_currency.Equals("1"))
            //{
            //    trCrossCurrency.Visible = true;
            //}
            //else
            //{
            //    trCrossCurrency.Visible = false;
            //}

        }

        public void set_default_values()
        {
            string show_currency = daccess.RunStringReturnStringValue1Where("instructions", "show_currency", "instruction_type_id", drpInstructions.SelectedValue);
            string show_cross_currency = daccess.RunStringReturnStringValue1Where("instructions", "show_cross_currency", "instruction_type_id", drpInstructions.SelectedValue);
            string show_amount = daccess.RunStringReturnStringValue1Where("instructions_types", "show_amount", "instruction_type_id", drpInstructions.SelectedValue);

            //if (show_currency.Equals("1"))
            //{
            //    //???
            //}
            //else
            //{
            //    drpCurrency.SelectedValue = "20";
            //}

            //if (show_amount.Equals("1"))
            //{
            //    //???
            //}
            //else
            //{
            //    txtAmount.Text = "0";
            //}

            //if (show_cross_currency.Equals("1"))
            //{
            //    //???
            //}
            //else
            //{
            //    drprossCurrency.SelectedValue = "2";
            //    txtCCRate.Text = "1";
            //}
            if(drpInstructions.SelectedItem.Text.Contains("Outward Telegraphic"))
            {
                supportingDocUpload.Visible = false;
                btnSupportingAttach.Visible = false;
                lblSupportingDoc.Visible = false;
                txtSupportingFileName.Visible = false;
            }
            else if(drpInstructions.SelectedValue == "1" || drpInstructions.SelectedValue == "3")
            {
                supportingDocUpload.Visible = true;
                btnSupportingAttach.Visible = true;
                lblSupportingDoc.Visible = true;
                txtSupportingFileName.Visible = true;
            }
        }

        protected void txtAmount_TextChanged(object sender, EventArgs e) //Handles txtAmount.TextChanged
        {
            string Rate = "";
            double value;
            if (double.TryParse(txtAmount.Text, out value))
            {

                Rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_id", drpCurrency.SelectedValue);
            }

            string local_equivalance = "";
            double val;
            if (!double.TryParse(Rate, out val))
            {
                Rate = "1";
                local_equivalance = txtAmount.Text;
            }

            double local = 0;
            double val2;
            if (double.TryParse(local_equivalance, out val2) && val2 > 0)
            {
                local = Convert.ToDouble(local_equivalance) * Convert.ToDouble(Rate);
            }

            txtAmount.Text = Convert.ToDouble(txtAmount.Text).ToString("N2");

            if (Session["UserType"].ToString() == "3" && txtAmount.ReadOnly == false)
            {
                //??? dropdown - done, untested
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_staus_action" }, new string[] { "document_status_user_type_who_can_action", "is_new_instruction" }, new string[] { Session["UserType"].ToString(), "1" }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND is_new_instruction = 1 ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN);
            }

        }

        protected void drpValidationStatus_SelectedIndexChanged(object sender, EventArgs e) //Handles drpValidationStatus.SelectedIndexChanged
        {
            //if (txtDocumentStatusID.Text == drpValidationStatus.SelectedValue)
            //{
            //    btnSubmit.Visible = false;
            //}
            //else
            //{
            //btnSubmit.Visible = true;
            //btnSubmit.Text = drpValidationStatus.SelectedItem.Text;
            //}
            //ShowPdf1.FilePath = "../instructions/" + txtFileName.Text;
        }

        protected void rbnUnProcessed_CheckedChanged(object sender, EventArgs e) //Handles rbnUnProcessed.CheckedChanged
        {
            if (rbnUnProcessed.Checked)
            {
                enable_disable_editing(true);
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();
                MultiView1.SetActiveView(ViewCustomerList);
                lblFrom.Visible = false;
                lblTo.Visible = false; ;
                dtmFrom.Visible = false;
                dtmTo.Visible = false;
            }
        }

        protected void rbnProcessed_CheckedChanged(object sender, EventArgs e) //Handles rbnProcessed.CheckedChanged
        {
            if (rbnProcessed.Checked)
            {
                enable_disable_editing(false);
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();
                MultiView1.SetActiveView(ViewCustomerList);
                lblFrom.Visible = true;
                lblTo.Visible = true;
                dtmFrom.Visible = true;
                dtmTo.Visible = true;
                dtmFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                dtmTo.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            }
        }

        public void enable_disable_editing(Boolean enbl)
        {
            switch (Session["UserType"].ToString())
            {
                case "1":
                    enable_disable_addition_controls2(enbl);
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "2":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "3":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "4":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "5":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "6":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "10":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "11":
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                default:
                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e) //Handles btnCancel.Click
        {
            unlock();
            //refresh
            btnView_Click(null, null);
        }

        protected void btnAdd_Click(object sender, EventArgs e) //Handles btnAdd.Click
        {
            if (string.IsNullOrEmpty(txtTransactionReference.Text))
            {
                alert.FireAlerts(this.Page, "Please generate ref");
                return;
            }

            if (!FileUpload2.HasFile)
            {
                alert.FireAlerts(this.Page, "Please browse a file { attach");
                return;
            }

            //Now attach.
            string fl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss_") + FileUpload2.FileName.Replace(" ", "_");
            string path = Server.MapPath("..") + "/instructions/" + fl_name;
            FileUpload2.SaveAs(path);


            if (FileUpload2.FileName.ToLower().Contains(".pdf"))
            {
                ShowPdf1.FilePath = "../instructions/" + fl_name;
            }

            if (add_attachment(fl_name, txtInstructionID.Text))
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Attachment added successfully to instructions with the following details: Instruction ID : " + txtInstructionID.Text + ", Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + fl_name + " by user: " + Session["UserFullName"].ToString(), "", "0", 0, "Attachment added successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "Extra attachment added. Select and click open to view");
            }
            else
            {
                alert.FireAlerts(this.Page, "Error adding attachment");
                erl.LogError("Error adding attachment to instruction", "add_attachment returned false");
            }

            //Load attachements
            load_attachment();

            return;
        }

        public void load_attachment()
        {
            //??? dropdown - done, untested
            sharedUtility.LoadDropDownList(drpAttachments, genericFunctions.GetDropdownListInfo("instructions_attachment", new string[] { "attachment_id", "file_name" }, new string[] { "deleted is null || deleted", "instruction_id" }, new string[] { "0)", txtInstructionID.Text }), "file_name", "attachment_id");
            //sharedUtility.LoadDropDownList(drpAttachments, genericFunctions.GetAttachmentDropDownListInfo(), "file_name", "attachment_id");
            //sharedUtility.LoadDropDownListing("", "select '0'   attachment_id, ' Default'   file_name from instructions_attachment UNION select attachment_ID,  file_name  from instructions_attachment where (deleted is null ||deleted =0) and instruction_id = '" & txtInstructionID.Text & "' ", drpAttachments, "file_name", "attachment_id", My.Settings.strDSN);
        }

        public Boolean add_attachment(string name, string instruction_id) //  Boolean
        {
            if (daccess.RunNonQueryInsert("Insert", "instructions_attachment", new string[] { "file_name", "date_inserted", "instruction_id", "inserted_by" }, new string[] { name, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", instruction_id, Session["UserID"].ToString() }))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Boolean delete_attachment(string filename, string instruction_id)//   Boolean
        {
            if (daccess.RunNonQuery1Where("Update", "instructions_attachment", new string[] { "deleted", "deleted_date", "deleted_by" }, new string[] { "1", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString() }, "attachment_id", instruction_id))
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Attachment deleted successfully to instructions with the following details: Instruction ID : " + txtInstructionID.Text + ", Account: " + drpAccount.SelectedItem.Text + ", Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + filename + " by user: " + Session["UserFullName"].ToString(), "", "0", 0, "Attachment deleted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                return true;
            }
            else
            {
                erl.LogError(" delete_attachment", "Err||deleting attachment to instruction with the following details");
                return false;
            }
        }

        protected void drpAttachments_SelectedIndexChanged(object sender, EventArgs e)// Handles drpAttachments.SelectedIndexChanged
        {
            string fl_name = "";
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
                    btnOpen.Visible = false;
                }
            }
        }

        protected void btnDeleteAttachment_Click(object sender, EventArgs e) //Handles btnDeleteAttachment.Click
        {
            if (drpAttachments.SelectedIndex.Equals(0))
            {
                alert.FireAlerts(this.Page, "You cannot delete the default document");
            }
            else if (drpAttachments.SelectedIndex > 0)
            {
                if (delete_attachment("", drpAttachments.SelectedValue))
                {
                    load_attachment();
                    alert.FireAlerts(this.Page, "Document deleted succesfully");
                }
                else
                {
                    alert.FireAlerts(this.Page, "Err||deleting attachment");
                }
            }
        }

        //Because there are so many textboxes. Ple e hide the unecessary ones
        public void hide_unhide_comments_field(string user_type)
        {
            //PRMO1 comments
            //if (!string.IsNullOrEmpty(txtPRMO1Comments.Text) || user_type.Equals("3"))
            //{
            //    trPRMO1Comments.Visible = true;
            //}
            //else
            //{
            //    trPRMO1Comments.Visible = false;
            //}

            //PRMO2 comments
            /* if (!string.IsNullOrEmpty(txtPRMO2Comments.Text) || user_type.Equals("13"))
             {
                 trPRMO2Comments.Visible = true;
             }
             else
             {
                 trPRMO2Comments.Visible = false;
             }

             if (!string.IsNullOrEmpty(txtPRMOTLComments.Text) || user_type.Equals("4"))
             {
                 trPRMOTLComments.Visible = true;
             }
             else
             {
                 trPRMOTLComments.Visible = false;
             }

             if (!string.IsNullOrEmpty(txtProcessorComments.Text) || user_type.Equals("5") || user_type.Equals("6"))
             {
                 trProcessorComments.Visible = true;
             }
             else
             {
                 trProcessorComments.Visible = false;
             }

             //RM comments
             if (!string.IsNullOrEmpty(txtRMComments.Text) || user_type.Equals("7"))
             {
                 trRMComments.Visible = true;
             }
             else
             {
                 trRMComments.Visible = false;

             } */

        }

        protected void chkProcessAtBranch_CheckedChanged() //fix dnx
        {
            if (chkProcessAtBranch.Checked)
            {
                //??? dropdown - done, untested
                //load instructions types\
                sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "allow_branch_to_process", "instruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocations WHERE status", "system_user_id IN (SELECT system_tl_1 FROM system_users WHERE system_user_id", "active" }, new string[] { "1", "1", Session["UserID"].ToString() + "))", "1" }), "instruction_type", "instruction_type_id");
                //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                //sharedUtility.LoadDropDownListing("", "select '0'   instruction_type_id, ' Select Instruction Type'   instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where allow_branch_to_process = '1' AND instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id IN (select system_tl_1 from system_users where system_user_id = '" & Session("UserID") & "' ) ) order by instruction_type", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load document status
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_action", "document_status_id" }, new string[] { "document_status_user_type_who_can_action" }, new string[] { Session["UserType"].ToString() }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)

                //instructon ref
                txtFTRef.ReadOnly = false;
            }
            else
            {
                //??? dropdown - done, untested
                //load instructions
                sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "instruction_type_id in (SELECT instruction_type_id from instruction_type_allocations where status", "system_user_id IN (SELECT system_tl_1 FROM system_users WHERE system_user_id", "active" }, new string[] { "1", Session["UserID"].ToString() + "))", "1" }), "instruction_type", "instruction_type_id");
                //sharedUtility.LoadDropDownList(drpInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                //sharedUtility.LoadDropDownListing("", "select '0'   instruction_type_id, ' Select Instruction Type'   instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id in (select instruction_type_id from instruction_type_allocations where status = 1 AND system_user_id IN (select system_tl_1 from system_users where system_user_id = '" & Session("UserID") & "' ) )", drpInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load document status
                sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action", "(is_archive is null || is_archive" }, new string[] { Session["UserType"].ToString(), "0)" }), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status_action", "document_status_id");
                //sharedUtility.LoadDropDownListing("", "select '0'   document_status_id, ' Select Action'   document_status_action from document_status UNION select document_status_id, document_status_action from document_status where document_status_user_type_who_can_action = '" & Session("UserType") & "' AND (is_archive IS NULL ||is_archive = 0) ", drpValidationStatus, "document_status_action", "document_status_id", My.Settings.strDSN)

                //instructon ref
                txtFTRef.ReadOnly = true;
            }
        }

        public void openpopup(string link)
        {
            StringBuilder scriptString = new StringBuilder();

            scriptString.Append("<script>");
            scriptString.Append("window.open('" + link + "', '', 'toolbar=no,status=no,menubar=no,location=center,scrollbars=no,resizable=yes,Height=500,Width=650');");
            scriptString.Append("</script>");

            ClientScript.RegisterStartupScript(this.GetType(), "test", scriptString.ToString());
            //this.Page.RegisterStartupScript("test", scriptString.ToString());
        }

        protected void btnOpen_Click(object sender, EventArgs e) //Handles btnOpen.Click
        {
            string filename = "";
            //??? My
            //filename = My.Settings.instructions_location + "\\" + drpAttachments.SelectedItem.Text;
            System.IO.FileStream fs = null;
            fs = System.IO.File.Open(filename, System.IO.FileMode.Open);

            Byte[] btFile = new Byte[fs.Length - 1];

            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(btFile);
            Response.End();
        }

        protected void downloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                writeToExcelFile(excelGridView);
                ////Response.ContentType = "instructions/pdf";
                //Response.ContentType = "application/vnd.xls";
                ////Response.AddHeader("content-disposition", "attachment;filename= " + txtFileName.Text);
                //Response.AppendHeader("Content-Dipsosition", "attachment; filename= " + txtFileName.Text);
                //string path = Server.MapPath("..") + "/instructions/" + txtFileName.Text;
                //Response.TransmitFile(path);

                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.SuppressContent = true;
                //HttpContext.Current.ApplicationInstance.CompleteRequest();
                ////Response.End();
            }

            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error exporting the attached excel file." + Environment.NewLine + ex.Message);
                return;
            }

        }
        private void excelImport(string path)
        {
            string connString = "";
            string strFileType = Path.GetExtension(supportingDocUpload.FileName).ToLower();

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
            //string query = "SELECT [ClientAccount],[ClientName],[BeneficiaryAccount],[Amount] FROM [Sheet1$]";
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
                    alert.FireAlerts(this.Page, "There was an error importing the excel document" + ex.Message);
                    return;
                }
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //This needs to be empty for excel exporting to work. Don't change this.
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

        protected void btnSupportingAttach_Click(object sender, EventArgs e)
        {
            if (!drpInstructions.SelectedItem.Text.Equals("Outward Telegraphic Transfer"))
            {
                if (supportingDocUpload.HasFile)
                {
                    if (supportingDocUpload.FileName.Contains(".xls"))
                    {
                        if (!string.IsNullOrEmpty(txtTransactionReference.Text))
                        {


                            txtSupportingFileName.Text = supportingDocUpload.FileName;
                            try
                            {
                                string supportingfl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss") + supportingDocUpload.FileName.Replace(" ", "_");
                                string supportingPath = Server.MapPath("~") + "/instructions/" + supportingfl_name;

                                supportingPath = supportingPath.Replace("\\", "/");
                                supportingPath = supportingPath.Replace("//", "/");
                                txtSupportingFilePath.Visible = true;
                                txtSupportingFilePath.Text = supportingPath;
                                supportingDocUpload.SaveAs(supportingPath);


                                txtSupportingFileName.Text = supportingfl_name;

                                instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

                                instructions_attachment insAtt = new instructions_attachment();
                                insAtt.instruction_id = ins.instruction_id;
                                insAtt.file_name = supportingfl_name;
                                insAtt.date_inserted = DateTime.Now;

                                _db.instructions_attachment.Add(insAtt);

                                _db.SaveChanges();

                                excelGridView.Visible = true;
                                downloadButton.Visible = true;
                                excelImport(supportingPath);

                                //ShowPdf1.FilePath = "instructions/" + ins.file_name;

                            }
                            catch (Exception ex)
                            {
                                alert.FireAlerts(this.Page, "Error adding the instruction attachment" + ex.Message);
                                return;
                            }
                        }
                        else
                        {
                            alert.FireAlerts(this.Page, "Please generate a transaction reference first");
                            return;
                        }
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "Only excel (.xls and .xlsx) files can be uploaded as supporting documents");
                        return;
                    }
                }
                else
                {
                    alert.FireAlerts(this.Page, "Please choose a supporting document with the file browser and then try attach again.");
                    return;
                }

            }
            else
            {
                alert.FireAlerts(this.Page, "Please use the PDF document section for Outward Telegraphic Transfers (OTTs)");
                return;
            }
        }



    } 
    
}