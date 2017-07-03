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
    public partial class Action_Setup : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationLog = new OperationsLog();
        DAccessInfo daccess = new DAccessInfo();
        Constants constants = new Constants();

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
            //??? dropdowns
            //load search by
            sharedUtility.LoadDropDownList(
                drpSearchBy,
                genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_value", "search_by_name" }, new string[] { "search_by_module" }, new string[] { "actions" }),
                "search_by_name",
                "search_by_value");
            //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("actions"), "search_by_name", "search_by_value");
            //"select search_by_value, search_by_name from search_by  where search_by_module = 'actions' ORDER BY search_by_name "

            //load user type - drpUserTypeSearch
            sharedUtility.LoadDropDownList(
                drpUserTypeSearch,
                genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type" }, null, null),
                "user_type",
                "user_type_no");
            //sharedUtility.LoadDropDownList(drpUserTypeSearch, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type ORDER BY user_type"

            //load user type - drpdocument_status_user_type_who_can_action
            sharedUtility.LoadDropDownList(
                drpdocument_status_user_type_who_can_action,
                genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type" }, null, null),
                "user_type",
                "user_type_no");
            //sharedUtility.LoadDropDownList(drpdocument_status_user_type_who_can_action, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type ORDER BY user_type"

            //load user type drpdocument_status_stage
            sharedUtility.LoadDropDownList(drpdocument_status_stage,
                genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type" }, null, null),
                "user_type",
                "user_type_no");
            //sharedUtility.LoadDropDownList(drpdocument_status_stage, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type ORDER BY user_type"

            //load user type - foward_back_to_after reversal
            sharedUtility.LoadDropDownList(
                drpfoward_back_to_after_reversal,
                genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type" }, null, null),
                "user_type",
                "user_type_no");

            sharedUtility.LoadDropDownList(
                drpInstructionTypes,
                genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_ID" }, new string[] { "active" }, new string[] { "1" }),
                "instruction_type",
                "instruction_type_ID");            //sharedUtility.LoadDropDownList(drpfoward_back_to_after_reversal, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type ORDER BY user_type"
        }

        public Boolean ValidateActions()
        {
            return true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateActions())
            {
                if (daccess.HowManyRecordsExist1Wheres("document_status", "document_status_id", txtdocument_status_id.Text).Equals(0))
                {
                    if (ProcInsertDocumentStatus())
                    {
                        alert.FireAlerts(this.Page, "Action added successfully");
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "Error adding action");
                    }
                }
                else
                {
                    if (ProcUpdateDocumentStatus())
                    {
                        alert.FireAlerts(this.Page, "Action updated successfully");
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "Error updating action");
                    }
                }
            }

            btnSearch_Click(null, null);
        }

        public void LoadDocumentStatus(int ID)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("document_status", "*", "document_status_id", ID.ToString());
            int index = 0;

            try
            {
                while (rdr.Read())
                {
                    txtdocument_status_id.Text = rdr["document_status_id"].ToString();
                    txtdocument_status.Text = rdr["document_status"].ToString();
                    txtdocument_status_action.Text = rdr["document_status_action"].ToString();

                    index = drpdocument_status_stage.Items.IndexOf(drpdocument_status_stage.Items.FindByValue(rdr["document_status_stage"].ToString()));
                    drpdocument_status_stage.SelectedIndex = index;

                    index = drpdocument_status_user_type_who_can_action.Items.IndexOf(drpdocument_status_user_type_who_can_action.Items.FindByValue(rdr["document_status_user_type_who_can_action"].ToString()));
                    drpdocument_status_user_type_who_can_action.SelectedIndex = index;

                    index = drpexception.Items.IndexOf(drpexception.Items.FindByValue(rdr["exception"].ToString()));
                    drpexception.SelectedIndex = index;

                    index = drpfoward_back_to_after_reversal.Items.IndexOf(drpfoward_back_to_after_reversal.Items.FindByValue(rdr["foward_back_to_after_reversal"].ToString()));
                    drpfoward_back_to_after_reversal.SelectedIndex = index;

                    index = drpis_document_held.Items.IndexOf(drpis_document_held.Items.FindByValue(rdr["is_document_held"].ToString()));
                    drpis_document_held.SelectedIndex = index;

                    index = drpis_referral.Items.IndexOf(drpis_referral.Items.FindByValue(rdr["is_referral"].ToString()));
                    drpis_referral.SelectedIndex = index;

                    if (rdr["include_amount_in_checking"].ToString() == "1")
                    {
                        chknclude_amount_in_checking1.Checked = true;
                    }
                    else
                    {
                        chknclude_amount_in_checking1.Checked = false;
                    }

                    if (rdr["must_comment"].ToString() == "1")
                    {
                        chkmust_comment.Checked = true;
                    }
                    else
                    {
                        chkmust_comment.Checked = false;
                    }
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
        }

        public void ResetTextboxes()
        {
            txtdocument_status_id.Text = "";
            txtdocument_status.Text = "";
            txtdocument_status_action.Text = "";
            drpdocument_status_stage.SelectedIndex = 0;
            drpdocument_status_user_type_who_can_action.SelectedIndex = 0;
            drpexception.SelectedIndex = 0;
            drpfoward_back_to_after_reversal.SelectedIndex = 0;
            drpis_document_held.SelectedIndex = 0;
            drpis_referral.SelectedIndex = 0;
            chknclude_amount_in_checking1.Checked = false;
            chkmust_comment.Checked = false;
            chkSpecificInstructionType.Checked = false;
            drpInstructionTypes.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadActions();
        }

        public void LoadActions()
        {
            //??? replace sql
            //string sql = "Select * From document_status_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtValue.Text + "%' ";
            //string addSql = "";

            //if (drpUserTypeSearch.SelectedIndex > 0)
            //{
            //    addSql += " AND user_type_no = '" + drpUserTypeSearch.SelectedValue + "' ";
            //}

            //sql += addSql + " ";

            try
            {


                //we want an exact match here (only the actions for ONE selected user type) so we call a stored procedure that doesn't use LIKE
                sharedUtility.LoadGridView(dgvActions, genericFunctions.GetDataSourceUserGridViewInfo("document_status_view", "user_type_no", drpUserTypeSearch.SelectedValue));
                
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading actions");
                erl.LogError("Error loading actions", ex.Message);
            }
        }

        public Boolean ProcInsertDocumentStatus()
        {
            bool result; 
            try
            {


                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {


                    SqlCommand myCommand = new SqlCommand("proc_insert_document_status", myConnection);
                    result = false;

                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    string documentStatus = txtdocument_status.Text;
                    int documentStatusStage = Convert.ToInt32(drpdocument_status_stage.SelectedValue);
                    string documentStatusAction = txtdocument_status_action.Text;
                    int documentStatusUserTypeWhoCanAction = Convert.ToInt32(drpdocument_status_user_type_who_can_action.SelectedValue);
                    int exception = Convert.ToInt32(drpexception.SelectedValue);

                    int forwardBackToAfterReversal = 0;
                    if (drpfoward_back_to_after_reversal.SelectedValue != "")
                    //The default XXX option creates an empty ("") selected value which breaks the method.
                    //so now if the default value is selected, we just pass through a forwardBackToAfterReversal value of 0. 
                    {
                        forwardBackToAfterReversal = Convert.ToInt32(drpfoward_back_to_after_reversal.SelectedValue);
                    }

                    int isDocumentHeld = Convert.ToInt32(drpis_document_held.SelectedValue);
                    int isReferral = Convert.ToInt32(drpis_referral.SelectedValue);
                    int includeAmountInChecking = Convert.ToInt32(chknclude_amount_in_checking1.Checked);
                    int mustComment = Convert.ToInt32(chkmust_comment.Checked);
                    long instructionTypeId = 0;
                    if (chkSpecificInstructionType.Checked == true)
                    {
                        instructionTypeId = Convert.ToInt64(drpInstructionTypes.SelectedValue);
                    }
                    myCommand.Parameters.Add("@document_status", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@document_status_stage", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_action", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@document_status_user_type_who_can_action", SqlDbType.Int);
                    myCommand.Parameters.Add("@exception", SqlDbType.Int);
                    myCommand.Parameters.Add("@foward_back_to_after_reversal", SqlDbType.Int);
                    myCommand.Parameters.Add("@is_document_held", SqlDbType.Int);
                    myCommand.Parameters.Add("@is_referral", SqlDbType.Int);
                    myCommand.Parameters.Add("@include_amount_in_checking", SqlDbType.Int);
                    myCommand.Parameters.Add("@must_comment", SqlDbType.Int);
                    myCommand.Parameters.Add("@instruction_type_id", SqlDbType.BigInt);

                    myCommand.Parameters["@document_status"].Value = documentStatus;
                    myCommand.Parameters["@document_status_stage"].Value = documentStatusStage;
                    myCommand.Parameters["@document_status_action"].Value = documentStatusAction;
                    myCommand.Parameters["@document_status_user_type_who_can_action"].Value = documentStatusUserTypeWhoCanAction;
                    myCommand.Parameters["@exception"].Value = exception;
                    myCommand.Parameters["@foward_back_to_after_reversal"].Value = forwardBackToAfterReversal;
                    myCommand.Parameters["@is_document_held"].Value = isDocumentHeld;
                    myCommand.Parameters["@is_referral"].Value = isReferral;
                    myCommand.Parameters["@include_amount_in_checking"].Value = includeAmountInChecking;
                    myCommand.Parameters["@must_comment"].Value = mustComment;
                    myCommand.Parameters["@instruction_type_id"].Value = instructionTypeId;



                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Document Status Saved successfully", "", "0", 0, "Document status saved successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), ex.Message, "", "0", 0, "Error saving document status", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
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

        public Boolean ProcUpdateDocumentStatus()
        {
            bool result;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {

                    //??? My
                    //myConnection = new SqlConnection(My.Settings.strDSN);
                    SqlCommand myCommand = new SqlCommand("proc_update_document_status", myConnection);
                    result = false;

                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    string documentStatus = txtdocument_status.Text;
                    int documentStatusStage = Convert.ToInt32(drpdocument_status_stage.SelectedValue);
                    string documentStatusAction = txtdocument_status_action.Text;
                    int documentStatusUserTypeWhoCanAction = Convert.ToInt32(drpdocument_status_user_type_who_can_action.SelectedValue);
                    int documentStatusId = Convert.ToInt32(txtdocument_status_id.Text);
                    int exception = Convert.ToInt32(drpexception.SelectedValue);
                    int fowardBackToAfterReversal = Convert.ToInt32(drpfoward_back_to_after_reversal.SelectedValue);
                    int isDocumentHeld = Convert.ToInt32(drpis_document_held.SelectedValue);
                    int isReferral = Convert.ToInt32(drpis_referral.SelectedValue);
                    int includeAmountInChecking = Convert.ToInt32(chknclude_amount_in_checking1.Checked);
                    int mustComment = Convert.ToInt32(chkmust_comment.Checked);
                    long instructionTypeId = 0;
                    if (chkSpecificInstructionType.Checked == true)
                    {
                        instructionTypeId = Convert.ToInt64(drpInstructionTypes.SelectedValue);
                    }

                    myCommand.Parameters.Add("@document_status", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@document_status_stage", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_action", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@document_status_user_type_who_can_action", SqlDbType.Int);
                    myCommand.Parameters.Add("@document_status_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@exception", SqlDbType.Int);
                    myCommand.Parameters.Add("@foward_back_to_after_reversal", SqlDbType.Int);
                    myCommand.Parameters.Add("@is_document_held", SqlDbType.Int);
                    myCommand.Parameters.Add("@is_referral", SqlDbType.Int);
                    myCommand.Parameters.Add("@include_amount_in_checking", SqlDbType.Int);
                    myCommand.Parameters.Add("@must_comment", SqlDbType.Int);
                    myCommand.Parameters.Add("@instruction_type_id", SqlDbType.BigInt);

                    myCommand.Parameters["@document_status"].Value = documentStatus;
                    myCommand.Parameters["@document_status_stage"].Value = documentStatusStage;
                    myCommand.Parameters["@document_status_action"].Value = documentStatusAction;
                    myCommand.Parameters["@document_status_user_type_who_can_action"].Value = documentStatusUserTypeWhoCanAction;
                    myCommand.Parameters["@document_status_id"].Value = documentStatusId;
                    myCommand.Parameters["@exception"].Value = exception;
                    myCommand.Parameters["@foward_back_to_after_reversal"].Value = fowardBackToAfterReversal;
                    myCommand.Parameters["@is_document_held"].Value = isDocumentHeld;
                    myCommand.Parameters["@is_referral"].Value = isReferral;
                    myCommand.Parameters["@include_amount_in_checking"].Value = includeAmountInChecking;
                    myCommand.Parameters["@must_comment"].Value = mustComment;
                    myCommand.Parameters["@instruction_type_id"].Value = instructionTypeId;


                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    result = true;
                    operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Document Status (" + documentStatusId + ") Updated successfully", "", "0", 0, "Document status saved siccessfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), ex.Message, "", "0", 0, "Error updating document status", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
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

        protected void dgvActions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(dgvActions.Rows[rw].Cells[0].Text);

            ResetTextboxes();
            LoadDocumentStatus(id);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetTextboxes();
        }

        protected void chkSpecificInstructionType_CheckedChanged(object sender, EventArgs e)
        {
            
            if (chkSpecificInstructionType.Checked == true)
            {
                drpInstructionTypes.Enabled = true;
            }
            else
            {
                drpInstructionTypes.Enabled = false;
            }
        }
    }
}