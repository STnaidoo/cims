using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System.Globalization;
using System.Data.OleDb;


namespace CIMS_V2
{

    public partial class GetNext : System.Web.UI.Page
    {
        CIMS_Entities _db = new CIMS_Entities();
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        DAccessInfo daccess = new DAccessInfo();
        ErrorLogging erl = new ErrorLogging();

        protected void Page_Load(object sender, EventArgs e)
        {

            //string ddfdd = Session["UserID"].ToString();
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (string.IsNullOrEmpty(Session["UserID"].ToString()))
            {
                Server.Transfer("~/Login.aspx", false);
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadPage();
                }
                if (Session["File_name"] != null)
                {
                    if(Session["File_name"].ToString().Contains(".pdf"))
                    {
                        ShowPdf1.Visible = true;
                        ShowPdf1.FilePath = Session["File_name"].ToString();
                    }
                    else if(Session["File_name"].ToString().Contains(".xls") || Session["File_name"].ToString().Contains(".csv"))
                    {
                        excelGridView.Visible = false;
                        ShowPdf1.Visible = false;
                    }
                }
            }

            btnSubmit.Visible = true;
            
        }

        protected void LoadPage()
        {
            if (Session["File_name"] != null)
            {
                if (Session["File_name"].ToString().Contains(".pdf"))
                {
                    ShowPdf1.Visible = true;
                    ShowPdf1.FilePath = Session["File_name"].ToString();
                }
                else if (Session["File_name"].ToString().Contains(".xls") || Session["File_name"].ToString().Contains(".csv"))
                {
                    excelGridView.Visible = true;
                    ShowPdf1.Visible = false;
                }
            }
            else if (string.IsNullOrEmpty(txtFileName.Text))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
            else
            {
                ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
            }
            
            //disable comments
            txtComments.ReadOnly = true;
            txtPRMO1Comments.ReadOnly = true;
            txtPRMOTLComments.ReadOnly = true;
            txtProcessorComments.ReadOnly = true;
            txtRMComments.ReadOnly = true;
            txtCallBackComment.ReadOnly = true;
            txtPRMO2Comments.ReadOnly = true;
            txtCallBackNos.ReadOnly = true;

            btnView.Visible = false;

            //Duty of care

            if (Session["CanPerformDOC"] != null)
            {

                if (Session["CanPerformDOC"].ToString() == "1")
                {
                    drpDOC.Enabled = true;
                }
                else
                {
                    drpDOC.Enabled = false;
                }
            }

            if(Session["UserType"].ToString() == "15")
            {
                txtCCRate.ReadOnly = false;
            }

            GetNextInfo getNextInfo = new GetNextInfo();
            switch (Session["UserType"].ToString())
            {
                case "1":

                    sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                        "search_by_name",
                        "search_by_value");

                    txtComments.ReadOnly = false;
                    //btnAdd.Visible = true;
                    //FileUpload2.Visible = true;

                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, getNextInfo.GetInstructionsTypesDropDownListInfo(null), "instruction_type", "instruction_type_id");
                    sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");

                    break;

                case "2":

                    btnView_Click(null, null);

                    sharedUtility.LoadDropDownList(drpInstructions, getNextInfo.GetInstructionsTypesDropDownListInfo(null), "instruction_type", "instruction_type_id");
                    sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");

                    break;

                case "44":
                    //load customers search
                    sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "customer" }),
                        "search_by_name",
                        "search_by_value");
                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("customer"), "search_by_name", "search_by_value");
                    rbnSearchInstruction.Checked = true;

                    //instruction_status - Set 

                    //Need to check this during runtime
                    int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));
                    drpValidationStatus.SelectedIndex = indx;

                    rbnProcessed.Visible = true;
                    rbnUnProcessed.Visible = true;

                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, getNextInfo.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                    sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");


                    //set radion button to search instruction
                    rbnSearchInstruction.Checked = true;
                    rbnGetNext.Checked = false;

                    // Need to know what this is for When runnign the application in original code
                    //rbnSearchInstruction_CheckedChanged


                   // btnAdd.Visible = true;
                    //FileUpload2.Visible = true;
                    //'txtPRMO1Comments.ReadOnly = False
                    btnView_Click(null, null);


                    break;

                case "4":

                    //load search by
                    sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                        "search_by_name",
                        "search_by_value");
                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");


                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, getNextInfo.GetInstructionsTypesDropDownListInfo(null), "instruction_type", "instruction_type_id");
                    sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");


                    int instructionTypeId = Convert.ToInt32(drpInstructions.SelectedValue);

                    instructions_types insType = _db.instructions_types.FirstOrDefault(i => i.instruction_type_ID == instructionTypeId);

                    // EFT Makers can upload excel documents 
                    if (insType != null & insType.allow_supporting_documents == 1) 
                    {
                        supportingDocUpload.Visible = true;
                        txtSupportingFileName.Visible = true;
                        lblSupportingDoc.Visible = false;
                        btnSupportingAttach.Visible = false;
                        lblPDFDoc.Visible = false;
                    }

                    enable_disable_addition_controls2(false);

                    break;

                default:

                    //load search by
                    sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                        "search_by_name",
                        "search_by_value");

                    //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");

                    rbnSearchInstruction.Checked = true;

                    //Can the user add an instruction? No!!!.
                    //enable_disable_addition_controls(false);
                    
                    //allow to view others
                    rbnProcessed.Visible = true;
                    rbnUnProcessed.Visible = true;
                    //drpDOC.Enabled = False

                    //load instructions
                    sharedUtility.LoadDropDownList(drpInstructions, getNextInfo.GetInstructionsTypesDropDownListInfo(null), "instruction_type", "instruction_type_id");
                    sharedUtility.LoadDropDownList(drpDOC, genericFunctions.GetDropdownListInfo("duty_of_care_comments", new string[] { "doc_comments_id", "doc_comments" }, null, null), "doc_comments", "doc_comments_id");
                    break;
            }


            //load document status
            sharedUtility.LoadDropDownList(drpValidationStatus, 
                getNextInfo.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserType"])),
                "document_status_action",
                "document_status_id");

            //load currency
            sharedUtility.LoadDropDownList(drpCurrency, getNextInfo.GetCurrencyDropDownListInfo(), "currency_name", "currency_id");

            //load user branch
            sharedUtility.LoadDropDownList(drpBranchs, getNextInfo.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");

            //load rm ro
            sharedUtility.LoadDropDownList(drpRM, getNextInfo.GetRelationshipManagersDropDownListInfo(), "RM_Name", "RM_ID");

            //load DOC
            sharedUtility.LoadDropDownList(drpRM, getNextInfo.GetDutyOfCareCommentsDropDownListInfo(), "doc_comments", "doc_comments_id");

            if (Session["CanGetNext"].ToString() == "1")
            {

                btnView_Click(null, null);
            }

            Page.MaintainScrollPositionOnPostBack = true;

        }

        protected void enable_disable_addition_controls2(bool enbl)
        {
            txtAmount.Enabled = enbl;
            //txtComments.Enabled = enbl;
            drpAccount.Enabled = enbl;
            drpCurrency.Enabled = enbl;
            btnAttach.Enabled = enbl;
            btnGenerate.Enabled = enbl;
            btnNew.Enabled = enbl;

            drpInstructions.Enabled = enbl;
            FileUpload1.Enabled = enbl;
            btnAttach.Enabled = enbl;

           // FileUpload2.Enabled = enbl;
           // btnAdd.Enabled = enbl;
            drpBranchs.Enabled = enbl;
            drpRM.Enabled = enbl;

            txtRelatedTransactionReference.ReadOnly = !enbl;
        }

        protected void enable_disable_addition_controls(bool bl)
        {

            FileUpload1.Visible = bl;
            btnAttach.Visible = bl;
            btnGenerate.Visible = bl;
            btnNew.Visible = bl;

            //txtAmount.ReadOnly = Not bl;
            //txtComments.ReadOnly = Not bl;
            drpInstructions.Enabled = bl;
            drpCurrency.Enabled = bl;
            drpAccount.Enabled = bl;

            //Disallow adding attachments
            //btnAdd.Visible = bl;
            //FileUpload2.Visible = bl;
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            //Cancel all locked instructions
            unlock();
            LoadInstructions();
            MultiView1.SetActiveView(ViewCustomerList);

        }

        protected void unlock() //yeah but what does unlocked and locked even mean tho
        {
            //Check if locked

            GetNextInfo getNextInfo = new GetNextInfo();

            if (txtInstructionID.Text != "" && txtInstructionID.Text != null)
            {

                long? locked_by = getNextInfo.GetLockBy(Convert.ToInt64(txtInstructionID.Text));

                if (rbnUnProcessed.Checked && locked_by > 0 && locked_by.ToString() == Session["UserID"].ToString())
                {
                    //Is it locked
                    //Locked so disable
                    //Unlock
                    //daccess.RunNonQuery("Update instructions set locked_by = '0', locked_date= (Select locked_date from instructions  where instruction_id = -10)  Where instruction_id = '" & txtInstructionID.Text & "'  ", My.Settings.strDSN)
                    getNextInfo.UnlockInstructions(Convert.ToInt64(txtInstructionID.Text));

                    OperationsLog operationLog = new OperationsLog();
                    operationLog.InsertOperationsLog((int)Session["UserID"], "Instruction unlocked with the following details: Instruction ID:  " + txtInstructionID.Text + "  Account: " + drpAccount.SelectedItem.Text, "Reference: " + txtTransactionReference.Text + " Amount: " + txtAmount.Text + " File: " + txtFileName.Text, "", 0, "Instruction Unlocked", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);

                }
            }
        }
        
        protected void LoadInstructions()
        {

            // Dim sql As String = ""
            //Dim addSql As String = ""

            //Load only What is allocated to you
            //sql = "Select * From instructions_view Where " & drpSearchBy.SelectedValue & " LIKE '%" & txtSearch.Text & "%' AND status = '" & Session("UserType") & "' AND (allocated_to =  '" & Session("UserID") & "' OR picked_by =  '" & Session("UserID") & "' ) "

            GetNextInfo getNextInfo = new GetNextInfo();

            string user_type = Session["UserType"].ToString();

            if (dtmFrom.Text != "" && dtmFrom.Text != null)
            {

                if (rbnUnProcessed.Checked)
                {
                    dgvInstructions.DataSource = getNextInfo.GetInstructions(drpSearchBy.SelectedValue, txtSearch.Text.Trim(), user_type, Session["UserID"].ToString(), false, Convert.ToDateTime(dtmFrom.Text.Trim()), Convert.ToDateTime(dtmTo.Text.Trim()));
                    dgvInstructions.DataBind();
                }
                if (rbnProcessed.Checked)
                {
                    dgvInstructions.DataSource = getNextInfo.GetInstructions(drpSearchBy.SelectedValue, txtSearch.Text.Trim(), user_type, Session["UserID"].ToString(), true, Convert.ToDateTime(dtmFrom.Text.Trim()), Convert.ToDateTime(dtmTo.Text.Trim()));
                    dgvInstructions.DataBind();
                }
            }

        }

        protected void btnGetNext_Click(object sender, EventArgs e)
        {
            GetNextInfo getNextInfo = new GetNextInfo();
            AddOn.Alerts alert = new AddOn.Alerts();

            int hw = getNextInfo.how_many_unsubmitted_instructions_exists_for_this_user(Convert.ToInt32(Session["UserID"]));

            if (Session["UserTypePopupAlertTime"] == null)
            {
                Session["UserTypePopupAlertTime"] = "0";
            }
            string PopupAlertTime = Session["UserTypePopupAlertTime"].ToString();

            int result;

            if (!int.TryParse(PopupAlertTime, out result))
            {
                PopupAlertTime = "0";
            }

            if (hw > 0)
            {
                alert.FireAlerts(this.Page, "You have about " + hw + " unsubmitted instructions. Kindly submit them first");

                if (Convert.ToInt32(PopupAlertTime) > 0 && (Session["UserType"].ToString() == "3" || Session["UserType"].ToString() == "5"))
                {
                    alert.FireAlerts(this.Page, "You have some instructions held by you that are approaching cuttoff. Please view the popup for more information.");


                    // Need to do this 
                    //
                    //Dim popup_sql As String = "Select * From Instructions_view Where instruction_id in (" & instructions_approaching_cutoff & ")"
                    //Session("popup_sql") = popup_sql
                    //openpopup("popup_view.aspx")


                    //update that popup has been opened
                    getNextInfo.get_locked_instructions_approaching_cutoff(Session["UserID"].ToString(), Convert.ToInt32(Session["UserType"].ToString()), Convert.ToInt32(PopupAlertTime));
                }
            }
            else
            {
                Get_Next();

                MultiView1.SetActiveView(ViewCustomerList);

                if (dgvInstructions.Rows.Count <= 0)
                {
                    alert.FireAlerts(this.Page, "No Pending Instructions Available");
                }
            }
        }

        protected void Get_Next()
        {
            GetNextInfo getNextInfo = new GetNextInfo();

            dgvInstructions.DataSource = getNextInfo.GetNextInstructionProc(Convert.ToInt32(Session["UserType"].ToString()), Convert.ToInt32(Session["UserID"].ToString()));
            dgvInstructions.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Reset_Instructions();
            btnGenerate.Visible = true;
            btnGenerate.Text = "Get Ref";
        }

        protected void Reset_Instructions()
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
            chkAllowDuplicates.Checked = true;
            //lblLockedBy.Text = "";
            //lblLockedDate.Text = "";
            txtRMComments.Text = "";
            txtRelatedTransactionReference.Text = "";
            drpBranchs.SelectedIndex = -1;
            txtCallBackComment.Text = "";
            txtCallBackNos.Text = "";
            drpDOC.SelectedIndex = -1;
            txtOtherComments.Text = "";

            drpCrossCurrency.SelectedIndex = -1;
            txtCCRate.Text = "";

            //set default image
            ShowPdf1.FilePath = "new.pdf";

            if (Session["UserType"].ToString() == "1")
            {

                //instruction_status

                int indx = drpValidationStatus.Items.IndexOf(drpValidationStatus.Items.FindByValue("3"));

                drpValidationStatus.SelectedIndex = indx;


                //drpValidationStatus_SelectedIndexChanged(null, null);




            }
        }

        protected void drpValidationStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (txtDocumentStatusID.Text == drpValidationStatus.SelectedValue)
            //{
            //    btnSubmit.Visible = false;
            //}
            //else
            //{
            //    btnSubmit.Visible = true;
            //    btnSubmit.Text = drpValidationStatus.SelectedItem.Text;
            //}
            //ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            GetNextInfo getNextInfo = new GetNextInfo();
            AddOn.Alerts alert = new AddOn.Alerts();

            if (string.IsNullOrEmpty(txtTransactionReference.Text))
            {
                alert.FireAlerts(this.Page, "Adding attachment is not possible as the reference is missing.");
                return;
            }

            if (!FileUpload2.HasFile)
            {
                alert.FireAlerts(this.Page, "Please browse a file then attach");
                return;
            }


            //Now attach.
            string fl_name = DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss_") + FileUpload2.FileName.Replace(" ", "_");
            FileUpload2.SaveAs(ConfigurationManager.AppSettings["instructions_location"] + "\\" + fl_name);

            //Load file
            string x = ConfigurationManager.AppSettings["instructions_location"] + "\\" + fl_name;

            if (FileUpload2.FileName.ToLower().Contains(".pdf"))
            {
                ShowPdf1.FilePath = "instructions/" + fl_name;
            }
            
            if (getNextInfo.AddAttachement(fl_name, Convert.ToInt64(txtInstructionID.Text), Convert.ToInt64(Session["UserID"].ToString())))
            {
                alert.FireAlerts(this.Page, "Extra attachment added.Select and click open to view");
            }

            //Load attachements
            sharedUtility.LoadDropDownList(drpAttachments, getNextInfo.GetAttachments(Convert.ToInt64(txtInstructionID.Text)), "file_name", "attachment_id");

        }

        protected void rbnUnProcessed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnUnProcessed.Checked == true)
            {

                enable_disable_editing(true);

                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                MultiView1.SetActiveView(ViewCustomerList);
                lblFrom.Visible = false;
                lblTo.Visible = false;
                dtmFrom.Visible = false;
                dtmTo.Visible = false;

            }
        }

        protected void enable_disable_editing(bool enbl)
        {

            switch (Session["UserType"].ToString())
            {

                case "1":
                    enable_disable_addition_controls2(enbl);
                   // btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;

                case "2":
                case "3":
                case "4":
                    enable_disable_addition_controls2(enbl);
                    // btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;
                case "5":
                case "6":
                case "10":
                case "11":
                case "12":
                case "13":
                case "14":
                case "15":
                case "17":
                case "22":
                case "23":
                case "24":
                case "25":

                    //btnSave.Enabled = enbl;
                    btnSubmit.Enabled = enbl;
                    drpValidationStatus.Enabled = enbl;
                    break;

            }

        }

        protected void rbnProcessed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbnProcessed.Checked == true)
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


        protected void btnOpen_Click(object sender, EventArgs e)
        {
            string filename = ConfigurationManager.AppSettings["instructions_location"].ToString() + "\\" + drpAttachments.SelectedItem.Text;
            FileStream fs = new FileStream(filename, FileMode.Open);

            Byte[] btFile = new byte[fs.Length];
            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));

            fs.Close();
            Response.AddHeader("Content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(btFile);
            Response.End();
        }

        protected void btnDeleteAttachment_Click(object sender, EventArgs e)
        {
            GetNextInfo getNextInfo = new GetNextInfo();
            Alerts alert = new Alerts();

            if (drpAttachments.SelectedIndex == 0)
            {
                alert.FireAlerts(this.Page, "You cannot delete the default document");
            }
            else
            {
                if (drpAttachments.SelectedIndex > 0)
                {

                    getNextInfo.DeleteAttachment(Convert.ToInt64(Session["UserID"].ToString()), Convert.ToInt64(drpAttachments.SelectedValue.ToString()));

                    Load_Attachment();

                    alert.FireAlerts(this.Page, "Document deleted succesfully");
                }
                else
                {
                    alert.FireAlerts(this.Page, "Error deleting attachment");
                }
            }
        }

        protected void Load_Attachment()
        {
            //GetNextInfo getNextInfo = new GetNextInfo();
            //sharedUtility.LoadDropDownList(drpAttachments, getNextInfo.GetAttachments(Convert.ToInt64(txtInstructionID.Text)), "file_name", "attachment_id");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["instruction_path"] = "instructions/" + txtFileName.Text;
            //Need to do the popup page 
            // openpopup("instructions_preview2.aspx")
        }

        protected void drpAttachments_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOpen.Visible = true;

            if (drpAttachments.SelectedItem.Text.ToLower().Contains(".pdf"))
            {
                if (drpAttachments.SelectedIndex == 0)
                {
                    ShowPdf1.FilePath = "instructions/" + txtFileName.Text;
                }
                else
                {
                    ShowPdf1.FilePath = "instructions/" + drpAttachments.SelectedItem.Text;
                }
            }

        }

        private bool validate_generation_of_ref()
        {
            if (drpRM.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the RM");
                return false;
            }

            double result;

            if (!double.TryParse(txtAmount.Text, out result))
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

            if (Session["UserType"].ToString() == "1" && drpBranchs.SelectedValue != Session["UserBranchID"].ToString())
            {

                alert.FireAlerts(this.Page, "Please select the right branch");
                return false;
            }

            var check_dups = getNextInfo.CheckForDuplicates(drpAccount.SelectedItem.Text, Convert.ToInt64(drpInstructions.SelectedValue), Convert.ToInt64(drpCurrency.SelectedValue), Convert.ToDouble(txtAmount.Text), Convert.ToInt64(txtInstructionID.Text), Convert.ToInt32(Session["duplicate_check_days"].ToString()));

            if (check_dups.Count == 0 && !chkAllowDuplicates.Checked)

            {
                alert.FireAlerts(this.Page, "Please note that Duplicate Instruction Exists.To continue anyway check *Allow Duplicates* and click Get Ref again.");


                //Need to do the popup_view page
                //Session("popup_sql") = "Select * From instructions_view Where reference IN (" & check_dups.Replace("*", "'") & ")"
                //openpopup("popup_view.aspx")

                return false;
            }

            return true;
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {

            if (!validate_generation_of_ref())
            {
                return;
            }

            if (btnGenerate.Text.ToLower().Contains("get"))
            {
                int id = 0;
                id = proc_insert_instructions();
                if (id > 0)
                {
                    txtInstructionID.Text = id.ToString();
                    DAccessInfo daccess = new DAccessInfo();
                    string get_code = daccess.RunStringReturnStringValue1Where("instructions_types", "instruction_code", "instruction_type_id", drpInstructions.SelectedValue);
                    string reference = get_code + "/" + Session["UserBranchCode"].ToString() + "/" + id;

                    if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "reference" }, new string[] { reference }, "instruction_id", id.ToString()))
                    {
                        txtTransactionReference.Text = reference;
                        btnGenerate.Text = "Update";
                        btnGenerate.Visible = false;
                        ShowPdf1.FilePath = "new.pdf";
                        drpInstructions.Enabled = false;
                    }
                    else
                    {
                        txtTransactionReference.Text = "Error";
                    }
                }
            }
            else if (btnGenerate.Text.Contains("update"))
            {
                //update
            }
        }

        protected void dgvInstructions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string rw = (e.CommandArgument).ToString();
            Reset_Instructions();
            string id = dgvInstructions.Rows[Convert.ToInt32(rw)].Cells[25].Text;
            int idVal = Convert.ToInt32(id);
            string fl_name = _db.instructions.FirstOrDefault(i => i.instruction_id == idVal).file_name;
            string path = Server.MapPath("~") + fl_name;
            path = path.Replace("\\", "/");

            
            if(_db.instructions.FirstOrDefault(i => i.instruction_id == idVal).instructions_types.instruction_type.Contains("EFT") && Session["UserType"].ToString() == "4") //A maker on an EFT transaction
            {
                
                lblSupportingDoc.Visible = true;
                supportingDocUpload.Visible = true;
                txtSupportingFileName.Visible = true;
                btnSupportingAttach.Visible = true;
            }
            Load_instruction_details(Convert.ToInt32(id));
            load_comments(id);

            //drpValidationStatus_SelectedIndexChanged(null, null);

            if (string.IsNullOrEmpty(fl_name))
            {
                ShowPdf1.FilePath = "new.pdf";
            }
        
            else if (fl_name.Contains(".pdf"))
            {
                if(txtFileName.Text.Contains("instructions/"))
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

            if (_db.instructions_attachment.FirstOrDefault(a => a.instruction_id == idVal) != null)
            {
                downloadButton.Visible = true;

            }
            else
            {
                alert.FireAlerts(this.Page, "There is no downloadable attachment stored for this transaction.");
            }

            MultiView1.SetActiveView(ViewInstructions);

            int extra_documents = how_many_extra_documents(id);

            if (extra_documents > 0)
            {
                if (drpCrossCurrency.SelectedItem.Text == "Yes")
                {
                    alert.FireAlerts(this.Page, "Please note that " + extra_documents.ToString() + " extra document(s) exists for this instructions and It is also a CROSS CURRENCY. You can view the extra documents by selecting the dropdown next to Other attachments above the instruction PDF.");
                }
                else
                {
                    alert.FireAlerts(this.Page, "Please note that " + extra_documents.ToString() + " extra document(s) exists for this instructions. You can download the attachment by clicking the download button below the instruction pdf.");
                }
            }

            if (drpCrossCurrency.SelectedItem.Text == "Yes")
            {
                alert.FireAlerts(this.Page, "Please note that the instruction is a cross currency");
            }


        }

        public void Load_instruction_details(int ID)
        {
            DAccessInfo daccess = new DAccessInfo();
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("instructions_view", "*", "instruction_ID", ID.ToString());
            int indx = 0;

            try
            {
                while (rdr.Read())
                {
                    Reset_Instructions();
                    chkAllowDuplicates.Checked = true;
                    txtStatus.Text = rdr["Status"].ToString();
                    txtDocumentStatusID.Text = rdr["document_status_id"].ToString();
                    txtRelatedTransactionReference.Text = rdr["related_reference"].ToString();

                    indx = drpInstructions.Items.IndexOf(drpInstructions.Items.FindByValue(rdr["instruction_type_id"].ToString()));
                    drpInstructions.SelectedIndex = indx;

                    sharedUtility.LoadDropDownList(drpValidationStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status_action" }, new string[] { "document_status_user_type_who_can_action" }, new string[] { txtStatus.Text }), "document_status_action", "document_status_id");



                    string Rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_id", rdr["currency_id"].ToString());

                    double val;
                    if (!double.TryParse(Rate, out val))
                    {
                        Rate = "1";
                    }

                    string local_equivalance = rdr["Amount"].ToString();

                    if (double.TryParse(local_equivalance, out val) && val > 0)
                    {
                        local_equivalance = (val * Convert.ToDouble(Rate)).ToString();
                    }

                    btnDeleteAttachment.Visible = false;

                    switch (Session["UserType"].ToString())
                    {
                        case "3":
                            enable_disable_addition_controls2(false);
                            if (rdr["Status"].ToString() == "3")
                            {
                                drpInstructions.Enabled = true;
                            }
                            else
                            {
                                drpInstructions.Enabled = false;
                            }
                            break;
                        default:
                            enable_disable_addition_controls2(false);
                            break;
                    }

                    if (rbnUnProcessed.Checked)
                    {
                        if (double.TryParse(rdr["locked_by"].ToString(), out val) && val > 0)
                        {
                            if (rdr["locked_by"].ToString() == Session["UserID"].ToString())
                            {
                                enable_disable_editing(true);
                                if (rdr["inserted_by"].ToString() == Session["UserID"].ToString() && (Session["UserType"].ToString() == "1" || Session["UserType"].ToString() == "3"))
                                {
                                    if (string.IsNullOrEmpty(rdr["reference"].ToString()))
                                    {
                                        drpInstructions.Enabled = true;
                                        btnGenerate.Visible = true;
                                    }
                                    else
                                    {
                                        drpInstructions.Enabled = false;
                                        btnGenerate.Visible = false;
                                    }
                                }
                            }
                            else
                            {
                                enable_disable_editing(false);
                            }
                        }
                        else if (rdr["inserted_by"].ToString() == Session["UserID"].ToString() && (Session["UserType"].ToString() == "1" || Session["UserType"].ToString() == "3"))
                        {
                            enable_disable_editing(true);
                            enable_disable_addition_controls2(true);
                            if (string.IsNullOrEmpty(rdr["reference"].ToString()))
                            {
                                drpInstructions.Enabled = true;
                                btnGenerate.Enabled = true;
                            }
                            else
                            {
                                drpInstructions.Enabled = false;
                                btnGenerate.Enabled = false;
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
                            break;
                        case "2":
                            txtFTRef.ReadOnly = true;
                            break;
                        case "3":
                            btnDeleteAttachment.Visible = false;
                            txtFTRef.ReadOnly = true;
                            break;
                        case "4":
                            btnDeleteAttachment.Visible = true;
                            txtFTRef.ReadOnly = true;
                            break;
                        case "5":
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
                        case "13":
                            txtFTRef.ReadOnly = true;
                            break;
                        case "15":
                            txtFTRef.ReadOnly = true;
                            break;
                        case "22":
                            txtFTRef.ReadOnly = true;
                            break;
                        case "23":
                            txtFTRef.ReadOnly = true;
                            break;
                        default:
                            txtFTRef.ReadOnly = true;
                            break;
                    }

                    txtClient_Name.Text = rdr["Client_Name"].ToString();
                    txtClient_Customer_Number.Text = rdr["Client_Customer_Number"].ToString();
                    txtClientID.Text = rdr["Client_ID"].ToString();
                    txtFileName.Text = rdr["file_name"].ToString();
                    txtComments.Text = rdr["Comments"].ToString();
                    txtInstructionID.Text = rdr["Instruction_ID"].ToString();
                    txtAmount.Text = rdr["Amount"].ToString();

                    if (double.TryParse(txtAmount.Text, out val))
                    {
                        txtAmount.Text = val.ToString("N2");
                    }

                    txtTransactionReference.Text = rdr["reference"].ToString();
                    txtPRMO1Comments.Text = rdr["ftroa_comments"].ToString();
                    txtPRMO2Comments.Text = rdr["prmo2_comments"].ToString();
                    txtPRMOTLComments.Text = rdr["ftrob_comments"].ToString();
                    txtProcessorComments.Text = rdr["processor_comments"].ToString();
                    txtRMComments.Text = rdr["rm_comments"].ToString();
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

                    sharedUtility.LoadDropDownList(drpAccount, genericFunctions.GetDropdownListInfo("client_details", new string[] { "Client_ID", "Client_Account_Number" }, new string[] { "Client_Customer_Number" }, new string[] { txtClient_Customer_Number.Text }), "Client_Account_Number", "Client_ID");

                    indx = drpAccount.Items.IndexOf(drpAccount.Items.FindByText(rdr["account_no"].ToString()));
                    drpAccount.SelectedIndex = indx;

                    indx = drpCurrency.Items.IndexOf(drpCurrency.Items.FindByValue(rdr["currency_id"].ToString()));
                    drpCurrency.SelectedIndex = indx;

                    txtInstructionStatus.Text = rdr["instruction_status"].ToString();

                   // lblLockedBy.Text = " Locked BY: " + rdr["locked_by_name"].ToString();
                   // lblLockedDate.Text = " Date: " + rdr["locked_date"].ToString();

                    indx = drpRM.Items.IndexOf(drpRM.Items.FindByValue(rdr["RM_ID"].ToString()));
                    drpRM.SelectedIndex = indx;

                    indx = drpBranchs.Items.IndexOf(drpBranchs.Items.FindByValue(rdr["branch_id"].ToString()));
                    drpBranchs.SelectedIndex = indx;

                    indx = drpDOC.Items.IndexOf(drpDOC.Items.FindByValue(rdr["doc_comments_id"].ToString()));
                    drpDOC.SelectedIndex = indx;

                    txtCallBackComment.Text = rdr["call_back_comments"].ToString();
                    txtCallBackNos.Text = rdr["call_back_no"].ToString();

                    DateTime d;
                    if (DateTime.TryParse(rdr["delivery_date"].ToString(), out d))
                    {
                        txtDeliveryDat.Text = rdr["delivery_date"].ToString();
                    }

                    //Don't really need this
                    Load_Attachment();

                    hide_unhide_comments_field(Session["UserType"].ToString());

                    //if (Session["processed_at_branch"].ToString() == "1")
                    //{
                    //    chkProcessAtBranch.Checked = true;
                    //}
                    //else
                    //{
                    //    chkProcessAtBranch.Checked = false;
                    //}

                    indx = drpCrossCurrency.Items.IndexOf(drpCrossCurrency.Items.FindByValue(rdr["cross_currency"].ToString()));
                    drpCrossCurrency.SelectedIndex = indx;

                    txtCCRate.Text = rdr["cross_currency_rate"].ToString();

                    if (Session["can_add_attachment"].ToString() == "1")
                    {
                       // btnAdd.Visible = true;
                       // FileUpload2.Visible = true;
                       // btnAdd.Enabled = true;
                       // FileUpload2.Enabled = true;
                    }
                    else
                    {
                       // btnAdd.Visible = false;
                       // FileUpload2.Visible = false;
                       // btnAdd.Enabled = false;
                       // FileUpload2.Enabled = false;
                    }

                    if (Session["CanInsertCoreBankingReference"].ToString() == "1")
                    {
                        txtFTRef.ReadOnly = false;
                    }
                    else
                    {
                        txtFTRef.ReadOnly = true;
                    }

                    toggle_fields();
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
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

        public void load_comments(string id)
        {
            try
            {
                sharedUtility.LoadGridView(dgvComments, genericFunctions.GetDataSourceUserGridViewInfo("instructions_comments_view", "instruction_id", id));

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error loading requirements by user " + Session["UserFullName"].ToString(), ex.Message);
                alert.FireAlerts(this.Page, "Error loading requirements");
            }
        }

        public int how_many_extra_documents(string instruction_id)
        {
            int result = 0;
            DAccessInfo daccess = new DAccessInfo();
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("instructions_attachment", "ISNULL(Count(*),0) AS Counts", "instruction_id", instruction_id);
            try
            {
                while (rdr.Read())
                {
                    result = Convert.ToInt32(rdr["Counts"]);
                }
            }
            catch (Exception ex)
            {

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

            return result;
        }

        public void hide_unhide_comments_field(string user_type)
        {
            if (!string.IsNullOrEmpty(txtComments.Text) || user_type == "1" || user_type == "3")
            {
                trComments.Visible = true;
            }
            else
            {
                trComments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtPRMO1Comments.Text))
            {
                trPRMO1Comments.Visible = true;
            }
            else
            {
                trPRMO1Comments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtPRMO2Comments.Text))
            {
                trPRMO2Comments.Visible = true;
            }
            else
            {
                trPRMO2Comments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtPRMOTLComments.Text))
            {
                trPRMOTLComments.Visible = true;
            }
            else
            {
                trPRMOTLComments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtProcessorComments.Text))
            {
                trProcessorComments.Visible = true;
            }
            else
            {
                trProcessorComments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtRMComments.Text))
            {
                trRMComments.Visible = true;
            }
            else
            {
                trRMComments.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtRelatedTransactionReference.Text))
            {
                txtRelatedTransactionReference.Visible = true;
            }
            else
            {
                txtRelatedTransactionReference.Visible = false;
            }

            if (user_type == "1" || user_type == "3")
            {
                trUploads.Visible = false;
            }
            else if (user_type == "4" && drpInstructions.SelectedItem.Text.Equals("EFT"))
            {
                trUploads.Visible = true;
                FileUpload1.Visible = false;
                btnAttach.Visible = false;
                lblPDFDoc.Visible = false;

            }
            else
            {
                trUploads.Visible = false;
            }

            if (!string.IsNullOrEmpty(txtCallBackComment.Text) || Session["CanPerformCallBack"].ToString() == "1")
            {
                trCallBackComments.Visible = false;
                trCallBackNo.Visible = false;
            }
            else
            {
                trCallBackComments.Visible = false;
                trCallBackNo.Visible = false;
            }

           // trUploads.Visible = false;
        }

        public void toggle_fields()
        {
            DAccessInfo daccess = new DAccessInfo();
            string show_currency = daccess.RunStringReturnStringValue1Where("instructions_types", "show_currency", "instruction_type_id", drpInstructions.SelectedValue);
            string show_cross_currency = daccess.RunStringReturnStringValue1Where("instructions_types", "show_cross_currency", "instruction_type_id", drpInstructions.SelectedValue);
            string show_amount = daccess.RunStringReturnStringValue1Where("instructions_types", "show_amount", "instruction_type_id", drpInstructions.SelectedValue);

            if (show_currency == "1")
            {
                trCurrency.Visible = true;
            }
            else
            {
                trCurrency.Visible = false;
            }

            if (show_amount == "1")
            {
                trAmount.Visible = true;
            }
            else
            {
                trAmount.Visible = false;
            }

            if (show_cross_currency == "1")
            {
                trCrossCurrency.Visible = true;
            }
            else
            {
                trCrossCurrency.Visible = false;
            }
        }

        public int proc_insert_instructions()
        {
            Constants c = new Constants();
            int result;
            try
            {
                using (SqlConnection conn = new SqlConnection(c.getConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("proc_insert_instructions", conn);
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    result = 0;

                    double amount = Convert.ToDouble(txtAmount.Text);
                    string comments = txtComments.Text;
                    string reference = txtTransactionReference.Text;
                    string file_name = txtFileName.Text;
                    int instruction_type_id = Convert.ToInt32(drpInstructions.SelectedValue);
                    DateTime inserted_date = Convert.ToDateTime("1/1/1900");
                    int inserted_by = Convert.ToInt32(Session["UserID"]);
                    DateTime modified_date = Convert.ToDateTime("1/1/1900");
                    int modified_by = Convert.ToInt32(Session["UserID"]);
                    int status = 1;
                    int client_id = Convert.ToInt32(txtClientID.Text);
                    string file_type = "";
                    int allocated_to = Convert.ToInt32(Session["UserID"]);
                    string account_no = drpAccount.SelectedItem.Text;
                    int currency_id = Convert.ToInt32(drpCurrency.SelectedValue);
                    int branch_id = Convert.ToInt32(drpBranchs.SelectedValue);
                    string ft_reference = txtFTRef.Text;
                    int rm_id = Convert.ToInt32(drpRM.SelectedValue);
                    int processed_at_branch = Convert.ToInt16(chkProcessAtBranch.Checked);

                    cmd.Parameters.Add("@amount", SqlDbType.Float);
                    cmd.Parameters.Add("@comments", SqlDbType.VarChar);
                    cmd.Parameters.Add("@reference", SqlDbType.VarChar);
                    cmd.Parameters.Add("@file_name", SqlDbType.VarChar);
                    cmd.Parameters.Add("@instruction_type_id", SqlDbType.Int);
                    cmd.Parameters.Add("@inserted_date", SqlDbType.DateTime);
                    cmd.Parameters.Add("@inserted_by", SqlDbType.Int);
                    cmd.Parameters.Add("@modified_date", SqlDbType.DateTime);
                    cmd.Parameters.Add("@modified_by", SqlDbType.Int);
                    cmd.Parameters.Add("@status", SqlDbType.Int);
                    cmd.Parameters.Add("@client_id", SqlDbType.Int);
                    cmd.Parameters.Add("@allocated_to", SqlDbType.Int);
                    cmd.Parameters.Add("@file_type", SqlDbType.VarChar);
                    cmd.Parameters.Add("@id_out", SqlDbType.Int);
                    cmd.Parameters.Add("@account_no", SqlDbType.VarChar);
                    cmd.Parameters.Add("@currency_id", SqlDbType.Int);
                    cmd.Parameters.Add("@branch_id", SqlDbType.Int);
                    cmd.Parameters.Add("@ft_reference", SqlDbType.VarChar);
                    cmd.Parameters.Add("@rm_id", SqlDbType.Int);
                    cmd.Parameters.Add("@proccessed_at_branch", SqlDbType.Int);

                    cmd.Parameters["@amount"].Value = amount;
                    cmd.Parameters["@comments"].Value = comments;
                    cmd.Parameters["@reference"].Value = reference;
                    cmd.Parameters["@file_name"].Value = file_name;
                    cmd.Parameters["@instruction_type_id"].Value = instruction_type_id;
                    cmd.Parameters["@inserted_date"].Value = inserted_date;
                    cmd.Parameters["@inserted_by"].Value = inserted_by;
                    cmd.Parameters["@modified_date"].Value = modified_date;
                    cmd.Parameters["@modified_by"].Value = modified_by;
                    cmd.Parameters["@status"].Value = status;
                    cmd.Parameters["@client_id"].Value = client_id;
                    cmd.Parameters["@allocated_to"].Value = allocated_to;
                    cmd.Parameters["@file_type"].Value = file_type;
                    cmd.Parameters["@id_out"].Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters["@account_no"].Value = account_no;
                    cmd.Parameters["@currency_id"].Value = currency_id;
                    cmd.Parameters["@branch_id"].Value = branch_id;
                    cmd.Parameters["@ft_reference"].Value = ft_reference;
                    cmd.Parameters["@rm_id"].Value = rm_id;
                    cmd.Parameters["@proccessed_at_branch"].Value = processed_at_branch;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    result = Convert.ToInt32(cmd.Parameters["@id_out"].Value);
                    OperationsLog ol = new OperationsLog();
                    ol.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Instruction inserted successfully", "", "0", 0, "Instruction inserted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                OperationsLog ol = new OperationsLog();
                ol.InsertOperationsLog(Convert.ToInt32(Session["UserID"].ToString()), "Error inserting instruction", "", "0", 0, "Error inserting instruction", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error inserting instruction", ex.Message);
                result = 0;
            }
            //finally
            //{
            //    conn.Close();
            //    cmd.Dispose();
            //    conn = null;
            //    cmd = null;
            //}

            //cmd.Dispose();
            //conn = null;
            //cmd = null;

            return result;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DAccessInfo daccess = new DAccessInfo();
            string user_id = Session["UserID"].ToString();
            string is_referral = daccess.RunStringReturnStringValue1Where("document_status", "is_referral", "document_status_id", drpValidationStatus.SelectedValue);
            string is_document_held = daccess.RunStringReturnStringValue1Where("document_status", "is_document_held", "document_status_id", drpValidationStatus.SelectedValue);
            string is_exception = daccess.RunStringReturnStringValue1Where("document_status", "exception", "document_status_id", drpValidationStatus.SelectedValue);
            string must_comment = daccess.RunStringReturnStringValue1Where("document_status", "must_comment", "document_status_id", drpValidationStatus.SelectedValue);
            string must_email = daccess.RunStringReturnStringValue1Where("document_status", "must_email", "document_status_id", drpValidationStatus.SelectedValue);
            string include_amount_in_checking = daccess.RunStringReturnStringValue1Where("document_status", "include_amount_in_checking", "document_status_id", drpValidationStatus.SelectedValue);
            string doc_is_amust = daccess.RunStringReturnStringValue1Where("document_status", "doc_is_amust", "document_status_id", drpValidationStatus.SelectedValue);
            string ref_is_amust = daccess.RunStringReturnStringValue1Where("document_status", "ref_is_amust", "document_status_id", drpValidationStatus.SelectedValue);
            int status = -1;
            string next_stage = daccess.RunStringReturnStringValue1Where("document_status", "document_status_stage", "document_status_id", drpValidationStatus.SelectedValue);
            string actionName = daccess.RunStringReturnStringValue1Where("document_status", "document_status_action", "document_status_id", drpValidationStatus.SelectedValue);
            bool can_submit = check_if_user_can_submit(user_id, drpValidationStatus.SelectedValue, Session["UserType"].ToString(), Convert.ToInt32(is_referral));

            instruction instruction = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

            string fileName = "";
            if(instruction != null)
            {
                fileName = instruction.file_name;

            }

            if(string.IsNullOrEmpty(fileName))
            {
                alert.FireAlerts(this.Page, "Please upload a file before submitting.");
                return;
            }

            if (string.IsNullOrEmpty(txtOtherComments.Text))
            {
                alert.FireAlerts(this.Page, "Please enter a comment in the 'other comments' box before submitting.");
                return;
            }
            if (!can_submit)
            {
                alert.FireAlerts(this.Page, "You cannot perform the action. Please Log out then Back In and Try Again");
                //??? sharedUtility - no SendMail method
                //sharedUtility.SendMail("", "", "Submit Error", "You cannot perform the action. Please Log out then Back In and Try Again. User " & Session("UserFullName") & ". Action " & drpValidationStatus.SelectedItem.Text & " Reference " & txtTransactionReference.Text & "", "ongadid@stanbic.com", "", "", "html")
                return;
            }

            if (!validate_generation_of_ref())
            {
                if (drpValidationStatus.SelectedIndex <= 1)
                {
                    alert.FireAlerts(this.Page, "Please validate the document");
                    return;
                }
                alert.FireAlerts(this.Page, "Couldn't validate the reference");
                return;
            }

            string check_dups = check_for_duplicates(drpAccount.SelectedItem.Text, drpInstructions.SelectedValue, drpCurrency.SelectedValue, Convert.ToDouble(txtAmount.Text), txtInstructionID.Text);

            if (!string.IsNullOrEmpty(check_dups) && chkAllowDuplicates.Checked == false)
            {
                alert.FireAlerts(this.Page, "Please note that Duplicate Instruction(s) Exists. Refer to the following instruction(s): " + check_dups + ". To continue anyway check *Allow Duplicates* and submit again.");
                OperationsLog o = new OperationsLog();
                o.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Duplicate instruction detected", "", "0", 0, "Duplicate instruction detected", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                return;
            }

            if (drpValidationStatus.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "please select the document status");
                return;
            }

            double Max_amount = 0;
            double Min_amount = 0;
            string Rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_name", drpCurrency.SelectedItem.Text);

            double val = 0;
            if (!double.TryParse(Rate, out val))
            {
                Rate = "1";
            }

            string Max_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "maximum_amount", "User_Type_no", Session["UserType"].ToString());
            string Min_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "minimum_amount", "User_Type_no", Session["UserType"].ToString());

            double val2 = 0;
            if (double.TryParse(Max_amount_text, out val) && double.TryParse(Min_amount_text, out val2))
            {
                Max_amount = val;
                Min_amount = val2;

                if (Session["CheckLimit"].ToString() == "1")
                {
                    if (is_referral != "1" && is_document_held != "1" && is_exception != "1")
                    {
                        double local_equivalance = Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate);

                        if (local_equivalance > Max_amount)
                        {
                            alert.FireAlerts(this.Page, "Please check if your limit allows you to");
                            return;
                        }
                    }
                }
            }

            double next_user_Max_amount = 0;
            double next_user_Min_amount = 0;

            string next_user_max_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "maximum_amount", "User_type_no", Session["UserType"].ToString());
            string next_user_min_amount_text = daccess.RunStringReturnStringValue1Where("user_type", "minimum_amount", "User_type_no", Session["UserType"].ToString());

            if (double.TryParse(next_user_max_amount_text, out val) && double.TryParse(next_user_min_amount_text, out val2))
            {
                next_user_Max_amount = val;
                next_user_Min_amount = val2;


                if (is_referral != "1" && is_document_held != "1" && is_exception != "1" && include_amount_in_checking == "1")
                {
                    double local_equivalance = (Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate));

                    if (local_equivalance > next_user_Max_amount)
                    {
                        alert.FireAlerts(this.Page, "The next user may not be able to act on the instruction because the limit does not allow.");
                    }
                }


                string check_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "check_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                if (check_submission_limit == "1")
                {
                    string maximum_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "maximum_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                    string minimum_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "minimum_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                    string maximum_usd_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "maximum_usd_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);
                    string minimum_usd_submission_limit = daccess.RunStringReturnStringValue1Where("document_status", "maximum_usd_submission_limit", "document_status_id", drpValidationStatus.SelectedValue);

                    if (drpCurrency.SelectedValue == "1")
                    {
                        if (double.TryParse(minimum_submission_limit, out val) && double.TryParse(maximum_submission_limit, out val2))
                        {
                            double minimum_submission_limit_amount = val;
                            double maximum_submission_limit_amount = val2;

                            double local_equivalance = Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate);

                            if (local_equivalance < minimum_submission_limit_amount || local_equivalance > maximum_submission_limit_amount)
                            {
                                alert.FireAlerts(this.Page, "The action is not allowed because of limit constraints. Please submit for callback.");
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (double.TryParse(minimum_usd_submission_limit, out val) && double.TryParse(maximum_usd_submission_limit, out val2))
                        {
                            string usd_rate = daccess.RunStringReturnStringValue1Where("currency", "rate", "currency_id", "2");
                            double local_minimum_usd_submission_limit_amount = Convert.ToDouble(minimum_usd_submission_limit) * Convert.ToDouble(usd_rate);
                            double local_maximum_usd_submission_limit_amount = Convert.ToDouble(maximum_usd_submission_limit) * Convert.ToDouble(usd_rate);

                            double local_equivalance = Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate);

                            if (local_equivalance < local_minimum_usd_submission_limit_amount || local_equivalance > local_maximum_usd_submission_limit_amount)
                            {
                                alert.FireAlerts(this.Page, "The action is not allowed because of limit constraints. Please submit for callback.");
                                return;
                            }
                        }
                    }
                }
            }
            if (Session["CanPerformCallBack"] != null)
            {
                if (Session["CanPerformCallBack"].ToString() == "1")
                {
                    double local_equivalance = Convert.ToDouble(txtAmount.Text) * Convert.ToDouble(Rate);
                    string minimum_call_back_amount = daccess.RunStringReturnStringValue1Where("instructions_types", "minimum_call_back_amount", "instruction_type_ID", drpInstructions.SelectedValue);
                    if (double.TryParse(minimum_call_back_amount, out val) && val <= local_equivalance)
                    {
                        if (is_referral == "0" && is_document_held != "1" && is_exception != "1")
                        {
                            if (string.IsNullOrEmpty(txtCallBackComment.Text))
                            {
                                alert.FireAlerts(this.Page, "Please insert call back comment");
                                return;
                            }

                            if (txtCallBackNos.Text.Length < 7 || !double.TryParse(txtCallBackNos.Text, out val))
                            {
                                alert.FireAlerts(this.Page, "Please insert a valid telephone number");
                            }
                        }
                    }
                }
            }
            if (Session["CanPerformDOC"].ToString() == "1")
            {
                if (drpDOC.SelectedIndex < 0 && is_document_held != "1")
                {
                    alert.FireAlerts(this.Page, "Please select DOC comments");
                    return;
                }
            }

            if (doc_is_amust == "1")
            {
                if (drpDOC.SelectedIndex != 1 || drpDOC.SelectedValue != "1")
                {
                    alert.FireAlerts(this.Page, "DOC must be complete");
                    return;
                }
            }

            if (ref_is_amust == "1")
            {
                if (validate_ft_ref(txtFTRef.Text) == false)
                {
                    return;
                }
            }

            if (!save())
            {
                //just to log what value we get out of the status
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Couldn't save transaction", "Dunno why though");

                alert.FireAlerts(this.Page, "Couldn't save the instruction");
                return;
            }

            if (Session["UserType"].ToString() == "1" || Session["UserType"].ToString() == "3")
            {
                if (drpValidationStatus.SelectedValue == "12")
                {
                    //do nothing
                }
                else if (string.IsNullOrEmpty(txtFileName.Text))
                {
                    alert.FireAlerts(this.Page, "Please attach a file before submitting");
                    return;
                }
            }

            string allocate_to = "0"; //default 

            int instructionType = (int)_db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text).instruction_type_id;
            int userType = Convert.ToInt32(next_stage);


            if (next_stage == Session["UserType"].ToString())
            {
                //allocate_to = Session["UserType"].ToString();
                allocate_to = GetUserWithLeastWork(userType, instructionType).ToString();
                //if (actionName.Contains())
            }
            else if (next_stage == "21")//archive
            {
                allocate_to = "0";
            }
            else
            {
                int userTypeVal = Convert.ToInt32(next_stage);
                allocate_to = GetUserWithLeastWork(userType, instructionType).ToString();

                string usertype = daccess.RunStringReturnStringValue1Where("system_users", "system_user_type", "system_user_id", allocate_to.ToString());

                if (usertype == "1")
                {
                    allocate_to = daccess.RunStringReturnStringValue1Where("instructions", "inserted_by", "instruction_id", txtInstructionID.Text);
                }
                //else if (usertype == "10")
                //{
                //    allocate_to = GetUserWithLeastWork(userTypeVal).ToString();
                //}

                if (allocate_to == "0" && next_stage != "21")
                {
                    alert.FireAlerts(this.Page, "There are no users active to submit this instruction to.");
                    return;
                }
            }

            if (drpValidationStatus.SelectedIndex >= 0)
            {
                string action = daccess.RunStringReturnStringValue1Where("document_status", "document_status", "document_status_id", drpValidationStatus.SelectedValue);
                if (is_exception == "1" || must_comment == "1")
                {
                    if (string.IsNullOrEmpty(txtOtherComments.Text))
                    {
                        alert.FireAlerts(this.Page, "Please add/insert comments");
                        return;
                    }
                }
            }

            if (proc_submit_and_allocate_instructions_enhanced(Convert.ToInt32(next_stage), Convert.ToInt32(Session["UserType"]), Convert.ToInt32(allocate_to), Convert.ToInt32(drpValidationStatus.SelectedValue), Convert.ToInt32(is_referral), Convert.ToInt32(is_document_held)))
            {
                int allocatedUserId = Convert.ToInt32(allocate_to);
                system_users allocatedUser = _db.system_users.FirstOrDefault(u => u.system_user_id == allocatedUserId);
                if (allocatedUser != null) //or archive
                {
                    alert.FireAlerts(this.Page, drpValidationStatus.SelectedItem.Text + ": " + allocatedUser.system_user_login + " (" + allocatedUser.system_user_fname + " " + allocatedUser.system_user_lname + ") " + "successful");
                }
                else if(allocatedUserId == 0)
                {
                    alert.FireAlerts(this.Page, "Instruction archived successfully");
                }
                daccess.RunNonQuery1Where("Update", "instructions", new string[] { "locked_by" }, new string[] { "0" }, "instruction_id", txtInstructionID.Text);
                btnView_Click(null, null);
                MultiView1.SetActiveView(ViewCustomerList);

                string file_ = "instructions/" + txtFileName.Text;
                string comments1 = "";
                string referred_by = Session["UserFullName"].ToString();
                string clients_name = txtClient_Name.Text;
                string instruction_type = drpInstructions.SelectedItem.Text;
                string instruction_date = daccess.RunStringReturnStringValue1Where("instructions", "Convert(nvarchar(20),inserted_date,100)", "instruction_id", txtInstructionID.Text);

                string comments = "  " + Environment.NewLine +
                    " <table border=1> <tr> " + Environment.NewLine +
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
                    " 	<td>Branch Comments</td><td>>" + txtComments.Text + "</td>" + Environment.NewLine +
                    " </tr>" +
                    "<tr>" + Environment.NewLine +
                     "<tr>" + Environment.NewLine +
                    " 	<td>Current Comments</td><td>>" + txtOtherComments.Text + "</td>" + Environment.NewLine +
                    " </tr>" +
                    "<tr>" + Environment.NewLine +
                    " 	<td>Other Comments</td><td>.</td>" + Environment.NewLine +
                    " </tr>" + Environment.NewLine +
                        load_html_comments() + Environment.NewLine +
                    " </table>";

                //??? for mail
                //if (is_exception == "1" || must_email == "1")
                //{
                //    string recipient = get_email_of_user_referred_to(txtInstructionID.Text, drpValidationStatus.SelectedValue);
                //    string referred_to = daccess.RunStringReturnStringValue1Where("user_type", "user_type", "user_type_no IN (SELECT document_status_stage FROM document_status WHERE document_status_id", drpValidationStatus.SelectedValue + ")");
                //}

                try
                {
                    //???
                    //sharedUtility.SenMail
                    //Who to refer to
                    string email_to = get_email_of_user_referred_to(txtInstructionID.Text, drpValidationStatus.SelectedValue);
                    string delivery_date = daccess.RunStringReturnStringValue1Where("instructions", "delivery_date", "instruction_id", txtInstructionID.Text);
                    string reference = daccess.RunStringReturnStringValue1Where("instructions", "reference", "instruction_id", txtInstructionID.Text);

                    string comment = load_html_comments();
                    int index = comment.LastIndexOf(">>") + 2;
                    int length = comment.Length - 13;
                    comment = comment.Substring(index, (length - index));

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
                                               "    Instruction Comment : " + comment + "\r\n" +
                                               "    Refered by          : " + referred_by + "\r\n" +
                                               "    Originating Branch  : " + drpBranchs.SelectedItem.Text);
                    }
                    catch (Exception ex)
                    {
                        //??? log error?
                        erl.LogError("Failed to send email notification to " + email_to, ex.Message);
                        alert.FireAlerts(this.Page, "Failed to send email notification to " + email_to + "\r\nPlease notify them manually.");
                    }
                }
                catch (Exception ex)
                {

                }
                OperationsLog o = new OperationsLog();
                o.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction has been referred", "", "0", 0, "Instruction has been referred", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
            }
            else
            {
                alert.FireAlerts(this.Page, "Error submitting instruction");
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error submitting instruction", "check_for_duplicates()");
            }
        }

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
                    column = "prmu_manager_proccessed_by";
                    break;

                default:
                    column = "allocated_to";
                    break;
            }

            user_email = daccess.RunStringReturnStringValueIN("system_users", "system_user_email", "system_user_id", "instructions", column, "instruction_id", instruction_id);

            return user_email;
        }

        public bool check_if_user_can_submit(string user_id, string document_status_id, string user_type, int is_referral)
        {
            DAccessInfo daccess = new DAccessInfo();
            if (daccess.HowManyRecordsExist2Wheres("document_status", "document_status_user_type_who_can_action", user_type, "document_status_id", "document_status_id") > 0)
            {

            }
            else
            {
                return false;
            }

            return true;
        }

        public string check_for_duplicates(string acc_no, string instruction_type, string currency, double amount, string instruction_id)
        {
            double val;
            if (double.TryParse(Session["duplicate_check_days"].ToString(), out val))
            {
                Session["duplicate_check_days"] = 90;
            }

            int duplicate_check_days = Convert.ToInt16(Session["duplicate_check_days"]);

            string dups = "";

            DAccessInfo d = new DAccessInfo();
            DbDataReader rdr = d.RunNonQueryReturnDataReaderMultiWheres("instructions", "*", new string[] { "account_no", "instruction_type_id", "currency_id", "amount" }, new string[] { acc_no, instruction_type, currency, amount.ToString() });
            if (!string.IsNullOrEmpty(instruction_id))
            {
                rdr = d.RunNonQueryReturnDataReaderMultiWheres("instructions", "*", new string[] { "account_no", "instruction_type_id", "currency_id", "amount", "instruction_id<>" }, new string[] { acc_no, instruction_type, currency, amount.ToString(), instruction_id });
            }

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

                    count++;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error checking for duplicates", ex.Message);
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

        public bool validate_ft_ref(string ft_ref)
        {
            DAccessInfo d = new DAccessInfo();
            string max_reference_chars = d.RunStringReturnStringValue1Where("system_settings", "setting_value", "setting_name", "max_reference_chars");
            string min_reference_chars = d.RunStringReturnStringValue1Where("system_settings", "setting_value", "setting_name", "min_reference_chars");

            double val;
            if (!double.TryParse(min_reference_chars, out val))
            {
                min_reference_chars = "4";
            }

            if (!double.TryParse(max_reference_chars, out val))
            {
                max_reference_chars = "12";
            }

            if (ft_ref.Length < Convert.ToInt32(min_reference_chars))
            {
                alert.FireAlerts(this.Page, "Wrong EE Reference. It should be at least " + min_reference_chars + " characters");
                return false;
            }

            if (ft_ref.Length > Convert.ToInt32(max_reference_chars))
            {
                alert.FireAlerts(this.Page, "Wrong EE Reference. It should be less than" + max_reference_chars + " characters");
                return false;
            }

            if (string.IsNullOrEmpty(txtFTRef.Text))
            {
                alert.FireAlerts(this.Page, "Please type the EE reference");
                return false;
            }

            string allow_dup_reference = d.RunStringReturnStringValue1Where("system_settings", "setting_value", "setting_name", "allow_dup_ref");

            if (allow_dup_reference != "1")
            {
                if (check_for_duplicate_ft_reference(txtFTRef.Text, txtInstructionID.Text) > 0)
                {
                    alert.FireAlerts(this.Page, "The EE Ref already exists");
                    return false;
                }
            }

            return true;
        }

        public bool save()
        {
            if (validate_generation_of_ref() == false)
            {
                return false;
            }

            if (save_comments())
            {
                string status = Session["UserType"].ToString();
                int statusVal = Convert.ToInt32(status);


                if(proc_update_instructions(statusVal))
                {
                    return true;
                }
                else
                {
                    //try to force the save using linq as sometimes weirdness happens with the stored procedure and region-specific values
                    try
                    {
                        instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

                        if (ins != null)
                        {
                            if (ins.instructions_types.allow_supporting_documents == 1) //for any transaction with supporting documents
                            {
                                DateTime modified_date = DateTime.Now;
                          
                                int modified_by = Convert.ToInt32(Session["UserID"]);

                                double ccRate = 0;
                                if(!string.IsNullOrEmpty(txtCCRate.Text))
                                {
                                    ccRate = double.Parse(txtCCRate.Text);
                                }

                                int val;
                                if (!int.TryParse(drpCrossCurrency.SelectedValue, out val))
                                {
                                    val = 0;
                                }

                                //add the current pdf stored in the db to the instructions_attachments for this instruction. 
                                instructions_attachment att = new instructions_attachment();
                                if (!string.IsNullOrEmpty(ins.file_name))
                                {
                                    att.file_name = ins.file_name;
                                    att.inserted_by = ins.inserted_by;
                                    att.date_inserted = ins.inserted_date;
                                    att.instruction_id = ins.instruction_id;
                                }

                                //add the new pdf to the instruction.
                                ins.file_name = txtFileName.Text;

                                ins.cross_currency_rate = ccRate;
                                ins.cross_currency = val;
                                ins.modified_by = modified_by;
                                ins.modified_date = modified_date;
                                ins.rm_id = Convert.ToInt32(drpRM.SelectedValue);
                                ins.doc_comments_id = Convert.ToInt32(drpDOC.SelectedValue);

                                _db.SaveChanges();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch(Exception ex)
                    {
                        alert.FireAlerts(this.Page, "Couldn't force the save " + ex.Message);

                    }
                }
            }
            return false;
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

            
            //get all ACTIVE users with the requested TYPE and BRANCH CODE 
            //List those users firstly by TOTALALLOCATEDWORK and secondly by ID
            IQueryable<system_users> userList = (from users in _db.system_users
                                                 where users.system_user_type == userTypeId &&
                                                 users.system_user_branch_code == branchCode.ToString() &&
                                                 users.system_user_active == 1 && users.system_user_status == 1
                                                 select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);

            //check if the branch's user list has allocated users in it
            int c = 0;
            foreach (system_users user in userList)
            {
                if(allocations.FirstOrDefault(a => a.system_user_id == user.system_user_id) != null)
                {
                    c++;  //lol
                }
            }
   
            //if the user list has no allocated users, look at users at all branches
            if (c == 0)
            {
                userList = (from users in _db.system_users
                            where users.system_user_type == userTypeId &&
                            users.system_user_active == 1 && users.system_user_status == 1
                            select users).OrderBy(u => u.total_work_allocated).ThenByDescending(u => u.system_user_id);
            }

            if (userList.Count() == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }

            //remove the current user from the userList because you don't want to end up sending to yourself RIP segregation of duties

            assignedUserId = Convert.ToInt32(Session["UserID"].ToString());

            List<system_users> userListLessLoggedInUser = new List<system_users>();

            if (userList.Count() > 1)
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
            if(allocatedUserList.Count == 0)
            {
                assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                return assignedUserId;
            }

            int maxTotalWork = (int)allocatedUserList.Max(u => u.total_work_allocated); //paul

            //get a list of all the instructions
            var instructions = from ins in _db.instructions
                               select ins;

            int lowestAllocation = instructions.Count(); //You can't be allocated more work than there are instructions

            //loop through the list to find the user with the least to do or just give work to the first person with nothing to do. 

            //select all users with the lowest allocated work count (from instructions list) and lowest total work count attribute
            List<int> workCountList = new List<int>();

            foreach(system_users user in allocatedUserList)
            {
                int count;

                //get a list of all the different counts of currently allocated work
                count = instructions.Count(i => i.allocated_to == user.system_user_id);
                workCountList.Add(count);
                //so now we have a list of a bunch of different numbers e.g. 100, 85, 103, 141
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

            //find the user with the least work done historically.
            int minTotalWork = (int)allocatedUserList.Min(u => u.total_work_allocated);
           
            //if historical total work is higher in the lowest allocation users than in the bigger allocated user list, use the minimum of the lowest allocation users
            if (lowestAllocationUsers.Min(u => u.total_work_allocated) >= allocatedUserList.Min(u => u.total_work_allocated))
            {
                minTotalWork = (int)lowestAllocationUsers.Min(u => u.total_work_allocated);
            }

            //if the lowest currently allocated users also have the lowest historical work, add them to this list
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
                else if (allocatedUserList.FirstOrDefault() != null)
                {
                    assignedUserId = (int)allocatedUserList.FirstOrDefault().system_user_id;
                }
                else
                {
                    //if there isn't anyone left to assign to, assign the instruction to the current logged in user. :(
                    assignedUserId = Convert.ToInt32(Session["UserID"].ToString());
                    alert.FireAlerts(this.Page, "Couldn't find an acceptable user to assign to. Please try again later.");
                }
            }

            return assignedUserId;

        }

        public int get_user_to_allocate(string status)
        {
            DAccessInfo daccess = new DAccessInfo();
            int least_allocation = 0;
            int least_user_id = 0;
            int count = 0;
            int tot_allocation = 0;
            DbDataReader rdr = daccess.RunNonQueryReturnDataReaderMultiWheres("system_users", "*", new string[] { "system_user_type", "system_user_active" }, new string[] { status, "1" });

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

        public int get_user_to_allocate(string status, string branch)
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

        public bool proc_submit_and_allocate_instructions_enhanced(int status, int user_type, int allocated_to, int document_status_id, int is_referral, int is_document_held)
        {
            bool result;
            try
            {
                 
                Constants c = new Constants();

                    using (SqlConnection conn = new SqlConnection(c.getConnectionString()))
                    {
                        SqlCommand myCommand = new SqlCommand("proc_submit_and_allocate_instructions_enhanced", conn);
                        result = false;
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
                        myCommand.Parameters.Add("@is_document_held", SqlDbType.VarChar);

                        myCommand.Parameters["@instruction_id"].Value = instruction_id;
                        myCommand.Parameters["@status"].Value = status;
                        myCommand.Parameters["@allocated_to"].Value = allocated_to;
                        myCommand.Parameters["@allocated_date"].Value = allocated_date;
                        myCommand.Parameters["@instruction_status"].Value = instruction_status;
                        myCommand.Parameters["@user_type"].Value = user_type;
                        myCommand.Parameters["@user_id"].Value = user_id;
                        myCommand.Parameters["@document_status_id"].Value = document_status_id;
                        myCommand.Parameters["@is_referral"].Value = is_referral;
                        myCommand.Parameters["@is_document_held"].Value = is_document_held;

                        conn.Open();
                        myCommand.ExecuteNonQuery();
                        result = true;
                        //??? insert operations log
                    }
                }
            catch (Exception ex)
            {
                //??? insert operations log
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error submitting instruction", ex.Message);
                alert.FireAlerts(this.Page, "Couldn't submit instruction" + ex.Message);
                result = false;
            }
            //finally
            //{
            //    conn.Close();
            //}

            //conn.Dispose();
            //myCommand.Dispose();
            //conn = null;
            //myCommand = null;

            return result;
        }

        public string load_html_comments()
        {
            DAccessInfo d = new DAccessInfo();
            DbDataReader rdr = d.RunNonQueryReturnDataReader1Where("instructions_comments_view", "*", "instruction_id", txtInstructionID.Text);
            int indx = 0;
            string comments = "";

            try
            {
                while (rdr.Read())
                {
                    comments = comments + "<tr>" + Environment.NewLine +
                        "   <td>" + rdr["instruction_comment_by_name"].ToString() + "</td><td>>" + rdr["instruction_comment"].ToString() + "</td>" + Environment.NewLine +
                        " </tr>";
                }
            }
            catch (Exception ex)
            {

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

            return comments;
        }

        public int check_for_duplicate_ft_reference(string ft, string instruction_id)
        {
            string dups = "";
            DAccessInfo d = new DAccessInfo();
            DbDataReader rdr = d.RunNonQueryReturnDataReaderMultiWheres("instructions", "*", new string[] { "ft_reference", "instruction_id <>" }, new string[] { ft, instruction_id });
            int count = 0;

            try
            {
                while (rdr.Read())
                {
                    count++;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error checking for duplicate reference document", ex.Message);
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

        public bool save_comments()
        {
            if (!string.IsNullOrEmpty(txtOtherComments.Text))
            {
                DAccessInfo d = new DAccessInfo();
                int how_may_such_comments = d.HowManyRecordsExist3Wheres("instruction_comments", "instruction_id", txtInstructionID.Text, "instruction_comment", txtOtherComments.Text.Replace("'", "^"), "instruction_comment_by", Session["UserID"].ToString());

                if (how_may_such_comments == 0)
                {
                    if (!d.RunNonQueryInsert("Insert", "instruction_comments", new string[] { "instruction_id", "instruction_comment_by", "instruction_comment", "instruction_comment_date", "instruction_comment_type_id" }, new string[] { txtInstructionID.Text, Session["UserID"].ToString(), txtOtherComments.Text.Replace("'", "^"), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", "0" }))
                    {
                        alert.FireAlerts(this.Page, "Error saving comments");
                        return false;
                    }
                    else
                    {
                        load_comments(txtInstructionID.Text);
                    }
                  
                }      
            }
            else
            {
                alert.FireAlerts(this.Page, "Please enter a comment before submitting.");
                return false;
            }
            return true;
        }

        public bool proc_update_instructions(int status)
        {
            bool result;
            try
            {

                Constants c = new Constants();
                using (SqlConnection conn = new SqlConnection(c.getConnectionString()))
                {

                    SqlCommand myCommand = new SqlCommand("proc_update_instructions", conn);
                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;
                    result = false;

                    //double amount = double.Parse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                    double amount = 0;

                    string comments = txtComments.Text;
                    string reference = txtTransactionReference.Text;
                    string file_name = txtFileName.Text;
                    //if this txtFileName is blank. then use the one existing in the database as the pdf
                    int instruction_id = Convert.ToInt32(txtInstructionID.Text);

                    if(txtFileName.Text == "new.pdf" || txtFileName.Text == "")
                    {
                        string storedFilename = daccess.RunStringReturnStringValue1Where("instructions", "filename", "reference", txtRelatedTransactionReference.Text);
                        file_name = storedFilename;
                    }
                    
                    int instruction_type_id = Convert.ToInt32(drpInstructions.SelectedValue);//
                    DateTime inserted_date = Convert.ToDateTime("1/1/1900");
                    int inserted_by = Convert.ToInt32(Session["UserID"]);
                    DateTime modified_date = Convert.ToDateTime("1/1/1900");
                    int modified_by = Convert.ToInt32(Session["UserID"]);
                    int client_id = Convert.ToInt32(txtClientID.Text);//
                    string file_type = "";
                    instruction_id = Convert.ToInt32(txtInstructionID.Text);//
                    string ftroa_comments = txtPRMO1Comments.Text.ToString();
                    string prmo2_comments = txtPRMO2Comments.Text.ToString();
                    string ftrob_comments = txtPRMOTLComments.Text.ToString();
                    string processor_comments = txtProcessorComments.Text.ToString();
                    string rm_comments = txtRMComments.Text;
                    string account_no = drpAccount.SelectedItem.Text;//
                    int currency_id = Convert.ToInt32(drpCurrency.SelectedValue);//
                    int branch_id = Convert.ToInt32(drpBranchs.SelectedValue);//this
                    string ft_reference = txtFTRef.Text;
                    string related_reference = txtRelatedTransactionReference.Text;
                    int rm_id = Convert.ToInt32(drpRM.SelectedValue);//this
                    string call_back_comments = txtCallBackComment.Text;
                    string call_back_no = txtCallBackNos.Text;
                    int doc_comments_id = Convert.ToInt32(drpDOC.SelectedValue);//
                    DateTime delivery_date = Convert.ToDateTime("1/1/1900");
                    int processed_at_branch = Convert.ToInt16(chkProcessAtBranch.Checked);
            
                
                    double cr;

                    if(!double.TryParse(txtCCRate.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out cr))
                    {
                        cr = 0;
                    }

                    double a;

                    if (!double.TryParse(txtAmount.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out a))
                    {
                        a = 0;
                    }

                    int val;
                    if (!int.TryParse(drpCrossCurrency.SelectedValue, out val))
                    {
                        val = 0;
                    }

                    DateTime d;
                    if (!DateTime.TryParse(txtDeliveryDat.Text, out d))
                    {
                        d = delivery_date;
                    }

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

                    myCommand.Parameters["@amount"].Value = a;
                    myCommand.Parameters["@comments"].Value = comments;
                    myCommand.Parameters["@reference"].Value = reference;
                    myCommand.Parameters["@file_name"].Value = file_name;
                    myCommand.Parameters["@instruction_id"].Value = instruction_id;
                    myCommand.Parameters["@instruction_type_id"].Value = instruction_type_id;
                    myCommand.Parameters["@modified_date"].Value = modified_date;
                    myCommand.Parameters["@modified_by"].Value = modified_by;
                    myCommand.Parameters["@status"].Value = status;
                    myCommand.Parameters["@client_id"].Value = client_id;
                    myCommand.Parameters["@ftroa_comments"].Value = ftroa_comments;
                    myCommand.Parameters["@ftrob_comments"].Value = ftrob_comments;
                    myCommand.Parameters["@processor_comments"].Value = processor_comments;
                    myCommand.Parameters["@rm_comments"].Value = rm_comments;
                    myCommand.Parameters["@file_type"].Value = file_type;
                    myCommand.Parameters["@account_no"].Value = account_no;
                    myCommand.Parameters["@currency_id"].Value = currency_id;
                    myCommand.Parameters["@branch_id"].Value = branch_id;
                    myCommand.Parameters["@ft_reference"].Value = ft_reference;
                    myCommand.Parameters["@rm_id"].Value = rm_id;
                    myCommand.Parameters["@related_reference"].Value = related_reference;
                    myCommand.Parameters["@call_back_comments"].Value = call_back_comments;
                    myCommand.Parameters["@doc_comments_id"].Value = doc_comments_id;
                    myCommand.Parameters["@prmo2_comments"].Value = prmo2_comments;
                    myCommand.Parameters["@call_back_no"].Value = call_back_no;
                    myCommand.Parameters["@delivery_date"].Value = d;
                    myCommand.Parameters["@processed_at_branch"].Value = processed_at_branch;
                    myCommand.Parameters["@cross_currency"].Value = val;
                    myCommand.Parameters["@cross_currency_rate"].Value = cr;

                    conn.Open();
                    myCommand.ExecuteNonQuery();
                    result = true;
                    //??? insert operations log
            }
        }
            catch (Exception ex)
            {
                //??? insert operations log
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("Error updating instruction", ex.ToString());
                alert.FireAlerts(this.Page, "Error updating instruction" + ex.ToString());
                result = false;
            }
            //finally
            //{
            //    conn.Close();
            //}

            //conn.Dispose();
            //myCommand.Dispose();
            //conn = null;
            //myCommand = null;

            return result;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/InstructionView.aspx");
            //load get next
        }

        protected void downloadButton_Click(object sender, EventArgs e)
        {
            CIMS_Entities _db = new CIMS_Entities();
            instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);
            int instructionId = 0;
            if (ins != null)
            {
                instructionId = (int)ins.instruction_id;
            }

            //get the most recent file
            var files = from attachments in _db.instructions_attachment
                        where attachments.instruction_id == instructionId
                        select attachments;

            DateTime mostRecent = (DateTime)files.Max(f => f.date_inserted);

            instructions_attachment att = files.FirstOrDefault(f => f.date_inserted == mostRecent);

            string fileName = "";

            if(att != null)
            {
                fileName = att.file_name;
            }

            string path = fileName;
            //if (Server.MapPath("~").Contains("D:") && fileName.Contains("instructions/"))
            //{
            //    path = "D:/CIMS/" + fileName;
            //}
            //else if (Server.MapPath("~").Contains("D:") && !fileName.Contains("instructions/"))
            //{
            //    path = "D:/CIMS/instructions/" + fileName;
            //}

            if(fileName.Contains("instructions/"))
            {
                path = Server.MapPath("~") + fileName; 
            }
            else
            {
                path = Server.MapPath("~") + "/instructions/" + fileName;
            }
            path = path.Replace("\\", "/");
            path = path.Replace("//", "/");
            Console.WriteLine("Excel path: " + path);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
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

                ShowPdf1.FilePath = ins.file_name;
            }
            else
            {
                Response.Write("This file does not exist at this path" + path);
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

        private void excelImport(string path) //inserts excel data into the gridview
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
                    alert.FireAlerts(this.Page, "There was an error importing the excel document" + ex.Message);
                    return;
                }
            }
        }
        protected void btnSupportingAttach_Click(object sender, EventArgs e)
        {
            if (supportingDocUpload.HasFile)
            {
                if (supportingDocUpload.FileName.Contains(".xls"))
                {
                    txtSupportingFileName.Text = supportingDocUpload.FileName;
                    try
                    {
                        //get the instruction ID
                        instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);

                        //get the filepath and save it
                        string supportingfl_name = supportingDocUpload.FileName.Replace(" ", "_");
                        if(supportingfl_name.Length > 250)
                        {
                            alert.FireAlerts(this.Page, "The file name is too long, please save the file with a shorter name and try upload it again");
                            return;
                        }
                        string supportingPath = "";
                        if (Server.MapPath("~").Contains("D:"))
                        {
                            supportingPath = "D:/CIMS/Instructions" + supportingfl_name;
                        }
                        else if(supportingfl_name.Contains("/instructions"))
                        {
                            supportingPath = Server.MapPath("~") + supportingfl_name;
                        }
                        else
                        {
                            supportingPath = Server.MapPath("~") + "instructions/" + supportingfl_name;
                        }
                        supportingPath = supportingPath.Replace("\\", "/");
                        supportingDocUpload.SaveAs(supportingPath);
                        txtSupportingFileName.Text = supportingfl_name;

                        //add the new instruction
                        instructions_attachment insAtt = new instructions_attachment();
                        insAtt.file_name = supportingfl_name;
                        insAtt.instruction_id = ins.instruction_id;
                        insAtt.date_inserted = DateTime.Now;

                        _db.instructions_attachment.Add(insAtt);
                        _db.SaveChanges();

                        excelGridView.Visible = true;
                        downloadButton.Visible = true;
                        excelImport(supportingPath);

                        ShowPdf1.FilePath = ins.file_name;
                       
                    }
                    catch (Exception ex)
                    {
                        alert.FireAlerts(this.Page, "Error adding the instruction attachment" + ex.ToString());
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
                if (_db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text) != null)
                {
                    instruction ins = _db.instructions.FirstOrDefault(i => i.reference == txtTransactionReference.Text);
                    int instructionTypeId = (int)ins.instruction_type_id;

                    instructions_types insType = _db.instructions_types.FirstOrDefault(t => t.instruction_type_ID == instructionTypeId);

                    if (string.IsNullOrEmpty(txtSupportingFileName.Text) && insType.allow_supporting_documents == 1)
                    {
                        alert.FireAlerts(this.Page, "Please upload an Excel sheet first using the Excel sheet section above.");
                        return;
                    }
                    if (!string.IsNullOrEmpty(txtFileName.Text))
                    {
                        alert.FireAlerts(this.Page, "Another file/document already exists. It has been replaced");
                    }


                    if (!FileUpload1.FileName.ToLower().Contains(".pdf"))
                    {
                            alert.FireAlerts(this.Page, "Upload error. You can only attach PDF documents in this section.");
                            return;
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

    }
}