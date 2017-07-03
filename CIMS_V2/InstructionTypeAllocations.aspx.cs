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

namespace CIMS_V2.Instruction
{
    public partial class InstructionTypeAllocations : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        DAccessInfo daccess = new DAccessInfo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            LoadPage();
        }

        protected void LoadPage()
        {
            if (!Page.IsPostBack)
            {
                //??? dropdown - done, untested
                //load search by
                //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("users"), "search_by_name", "search_by_value");
                sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_value", "search_by_name" }, new string[] { "search_by_module" }, new string[] { "users" }), "search_by_name", "search_by_value");


                //??? dropdown - done, untested
                //load user type
                sharedUtility.LoadDropDownList(drpsystem_user_type, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type", "user_type_no" }, null, null), "user_type", "user_type_no");
                //sharedUtility.LoadDropDownList(drpsystem_user_type, genericFunctions.GetUserTypesDropDownListInfo(), "user_type", "user_type_no");
                //sharedUtility.LoadDropDownList(drpsystem_user_type, "select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type ", "user_type", "user_type_no");

                //??? dropdown - done, untested
                //load user status
                sharedUtility.LoadDropDownList(drpsystem_user_status, genericFunctions.GetDropdownListInfo("user_status", new string[] { "user_status", "user_status_desc", "user_status_order" }, null, null), "user_status_desc", "user_status");
                //sharedUtility.LoadDropDownList(drpsystem_user_status, genericFunctions.GetUserStatusDropDownListInfo(), "user_status_desc", "user_status");
                //sharedUtility.LoadDropDownList(drpsystem_user_status, "select '0' AS user_status, ' ' As user_status_desc, user_status_order as '-1' from user_status UNION select user_status, user_status_desc, user_status_order from user_status order by user_status_order", "user_status_desc", "usre_status");

                //??? dropdown - done, untested
                //load user title
                sharedUtility.LoadDropDownList(drpsystem_user_tittle, genericFunctions.GetDropdownListInfo("user_title", new string[] { "title_id", "title_name" }, null, null), "title_name", "title_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_tittle, genericFunctions.GetUserTitleDropDownListInfo(), "title_name", "title_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_tittle, "select '0' AS title_id, ' ' AS title_name from user_title UNION select title_id, title_name from user_title ", "title_name", "title_id");

                //??? dropdown - done, untested
                //load user branch
                sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
                //dnx.LoadDropDownListing("", "select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch ", drpsystem_user_branch, "branch_name", "branch_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load user branch code
                sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_code" }, null, null), "branch_code", "branch_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetUserBranchDropDownListInfo(), "branch_code", "branch_id");
                //dnx.LoadDropDownListing("", "select '0' AS branch_id, ' ' AS branch_code from user_branch UNION select branch_id, branch_code from user_branch ", drpsystem_user_branch_code, "branch_code", "branch_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //user modified by
                sharedUtility.LoadDropDownList(drpsystem_user_modified_by, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, null, null), "names", "system_user_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_modified_by, genericFunctions.GetUserModifiedByDropDownListInfo(), "names", "system_user_id");
                //sharedUtility.LoadDropDownList(drpsystem_user_modified_by, "select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view ", "names", "system_user_id");

                //??? dropdown - done, untested
                //load check list
                sharedUtility.LoadCheckList(chkBoxInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_ID", "instruction_type" }, null, null), "instruction_type", "instruction_type_ID");
                //sharedUtility.LoadCheckList(chkBoxInstructions, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
                //dnx.LoadCheckList("", " select instruction_type_id, instruction_type from instructions_types ", chkBoxInstructions, "instruction_type", "instruction_type_id", My.Settings.strDSN)

                //??? dropdown - done, untested
                //load user branch
                sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");
                //sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
                //dnx.LoadDropDownListing("", "select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch ", drpBranchs, "branch_name", "branch_id", My.Settings.strDSN) 
            }

            Page.MaintainScrollPositionOnPostBack = true;
        } //dnx.dropdownlist

        private void userAdminInit(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;

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

            if (Session["CanModifyUserAllocations"].ToString() != "1")
            {
                Response.Redirect("no_rights.aspx");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            loadNames();
            MultiView1.SetActiveView(ViewList);
        }

        public void loadNames()
        {

            try
            {
                if (drpBranchs.SelectedIndex > -1) //fix genericfunction method 
                {
                    //    sql = "Select * From system_users_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtValue.Text + "%' "
                    //        " AND system_user_branch  = '" + drpBranchs.SelectedValue + "'   ORDER BY names";
                    sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, "system_user_branch", drpBranchs.SelectedValue));

                }
                else
                {
                    sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, null, null));
                    //"Select * From system_users_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtValue.Text + "%' "; " ORDER BY names"
                }

            }
            catch (Exception ex)
            {
                erl.LogError("Error loading system users", ex.Message);
                alert.FireAlerts(this.Page, "Error loading system users");
            }

        }

        public void reset_users()
        {
            txtsystem_user_login.Text = "";
            txtsystem_user_id.Text = "";
            drpsystem_user_type.SelectedIndex = -1;
            drpLoginOptions.SelectedIndex = -1;
            txtsystem_user_fname.Text = "";
            txtsystem_user_mname.Text = "";
            txtsystem_user_lname.Text = "";
            txtsystem_user_email.Text = "";
            txtsystem_user_initials.Text = "";
            drpsystem_user_tittle.SelectedIndex = -1;
            drpsystem_user_branch.SelectedIndex = -1;
            drpsystem_user_branch_code.SelectedIndex = -1;
            txtFailedLoginCount.Text = "";

            chkEditInstructionsAlloscations.Checked = false;
            CheckBox1_CheckedChanged(null, null);

            sharedUtility.LoadCheckList(chkBoxInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_id" }, null, null), "instruction_type", "instruction_type_id");
        }

        public void getInstructionsForUser()
        {
            for (int i = 0; i < chkBoxInstructions.Items.Count - 1; i++)
            {
                chkBoxInstructions.Items[i].Selected = false;
            }

            for (int j = 0; j < chkBoxInstructions.Items.Count - 1; j++)
            {
                string value = chkBoxInstructions.Items[j].Value;
                if (daccess.HowManyRecordsExist3Wheres("instruction_tyoe_allocations", "system_user_id", txtsystem_user_id.Text, "instruction_type_id", value, "status", "1") > 0)
                {
                    chkBoxInstructions.Items[j].Selected = true;
                }
            }
        }

        public void loadSystemUsers(int ID)
        {
            DbDataReader rdr = daccess.RunNonQueryReturnDataReader1Where("system_users", "*", "system_user_id", ID.ToString());
            int indx = 0;

            try
            {
                while (rdr.Read())
                {
                    txtsystem_user_login.Text = rdr["system_user_login"].ToString();
                    txtsystem_user_id.Text = rdr["system_user_id"].ToString();
                    indx = drpsystem_user_type.Items.IndexOf(drpsystem_user_type.Items.FindByValue(rdr["system_user_type"].ToString()));
                    drpsystem_user_type.SelectedIndex = indx;

                    loadUsersTeamLeader();

                    txtsystem_user_fname.Text = rdr["system_user_fname"].ToString();
                    txtsystem_user_mname.Text = rdr["system_user_mname"].ToString();
                    txtsystem_user_lname.Text = rdr["system_user_lname"].ToString();
                    txtsystem_user_email.Text = rdr["system_user_email"].ToString();
                    txtsystem_user_initials.Text = rdr["system_user_initials"].ToString();
                    indx = drpsystem_user_tittle.Items.IndexOf(drpsystem_user_tittle.Items.FindByValue(rdr["system_user_tittle"].ToString()));
                    drpsystem_user_tittle.SelectedIndex = indx;
                    indx = drpsystem_user_branch.Items.IndexOf(drpsystem_user_branch.Items.FindByValue(rdr["system_user_branch"].ToString()));
                    drpsystem_user_branch.SelectedIndex = indx;

                    indx = drpsystem_user_branch_code.Items.IndexOf(drpsystem_user_branch_code.Items.FindByText(rdr["system_user_branch_code"].ToString()));
                    drpsystem_user_branch_code.SelectedIndex = indx;

                    indx = drpLoginOptions.Items.IndexOf(drpLoginOptions.Items.FindByText(rdr["login_option"].ToString()));
                    drpLoginOptions.SelectedIndex = indx;


                    string system_user_active = rdr["system_user_active"].ToString();
                    string change_password = rdr["change_password"].ToString();
                    int m, n;

                    m = Int32.Parse(system_user_active);
                    n = Int32.Parse(change_password);

                    if (m == 1)
                    {
                        chksystem_user_active.Checked = true;
                    }
                    else
                    {
                        chksystem_user_active.Checked = false;
                    }

                    if (m == 1 && n == 1)
                    {

                        Chkchange_password.Checked = true;
                    }
                    else
                    {
                        Chkchange_password.Checked = false;
                    }

                    allocateInstructionType(rdr["system_user_type"].ToString());
                    getInstructionsForUser();

                    indx = drpsystem_user_status.Items.IndexOf(drpsystem_user_status.Items.FindByValue(rdr["system_user_status"].ToString()));
                    drpsystem_user_status.SelectedIndex = indx;

                    indx = drpsystem_user_modified_by.Items.IndexOf(drpsystem_user_modified_by.Items.FindByValue(rdr["system_user_modified_by"].ToString()));
                    drpsystem_user_modified_by.SelectedIndex = indx;

                    txtFailedLoginCount.Text = rdr["failed_login_count"].ToString();
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading system users details", ex.Message);
                alert.FireAlerts(this.Page, "Error loading users details.");
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

        protected void dgvNames_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt32(e.CommandArgument);
            reset_users();

            string id = dgvNames.Rows[rw].Cells[0].Text;
            loadSystemUsers(Int32.Parse(id));
            MultiView1.SetActiveView(ViewDetails);
        }

        public void allocateInstructionType(string UserType)
        {
            switch (UserType)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "10":
                case "11":
                case "13":
                case "14":
                case "15":
                case "16":
                case "17":
                case "22":
                case "23":
                case "24":
                case "25":
                    chkEditInstructionsAlloscations.Visible = true;
                    chkBoxInstructions.Visible = true;
                    btnSaveInstructionAllocations.Visible = true;
                    break;
                default:
                    chkEditInstructionsAlloscations.Visible = false;
                    chkBoxInstructions.Visible = false;
                    btnSaveInstructionAllocations.Visible = false;
                    break;

            }

        }

        public string build_mail_header()
        {
            string myString = " <table> <br> <tr>" +
                              " 	<td>Dear User," +
                              " </tr>  <table>";
            return myString;
        }

        public string build_mail_body(string comments)
        {
            string body = " <table><br> <tr> \r\n" +
                        " 	<td>Please note that a user has been created / updated .<br> <br>See details below<br> <br>" + comments +
                       "<br> <br> \r\n" +
                        " </tr></table>";
            return body;
        }

        public string build_mail_footer()
        {

            string myString = "";
            //??? My
            //myString = " <table><br><tr> \r\n " +
            //            " 	<td>Kind Regards,<br> <br>CIMS. &nbsp;<br>. &nbsp;<br>Link: " + My.Settings.link + " &nbsp; \r\n" +
            //            " </tr></table> ";
            return myString;
        }

        public void update_instructioon_type_allocation()
        {
            for (int i = 0; i < chkBoxInstructions.Items.Count - 1; i++)
            {

                string instruction_type_id = chkBoxInstructions.Items[i].Value;
                Boolean selectd = chkBoxInstructions.Items[i].Selected;
                int sel = Convert.ToInt16(selectd);

                if (daccess.HowManyRecordsExist2Wheres("instruction_type_allocations", "system_user_id", txtsystem_user_id.Text, "instruction_type_id", instruction_type_id) > 0)
                {
                    bool successful = daccess.RunNonQuery2Wheres("Update", "instruction_type_allocations", new string[] { "status", "modified_by", "modified_date" }, new string[] { sel.ToString(), Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "system_user_id", txtsystem_user_id.Text, "instruction_type_id", instruction_type_id);
                    if (successful)
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + " instruction type allocation updated successfully with the following details: Status: " + sel + " User ID: " + txtsystem_user_id.Text + "' and instruction_type_id: '" + instruction_type_id + " ", "", "0", 0, "User Instruction type allocation updated successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }
                else
                {
                    bool successful = daccess.RunNonQueryInsert("Insert", "instruction_type_allocations", new string[] { "instruction_type_id", "system_user_id", "status", "inserted_date", "inserted_by", "modified_date", "modified_by" }, new string[] { instruction_type_id, txtsystem_user_id.Text, sel.ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" });
                    if (successful)
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + " instruction type allocation inserted successfully with the following details: Status: " + sel + " User ID: " + txtsystem_user_id.Text + "' and instruction_type_id: '" + instruction_type_id + " ", "", "0", 0, "User Instruction type allocation inserted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }

                alert.FireAlerts(this.Page, "Operation complete");
            }

        }

        public Boolean validate_users()
        {
            if (String.IsNullOrEmpty(txtsystem_user_login.Text))
            {
                alert.FireAlerts(this.Page, "Please type the login name");
                return false;
            }

            if (drpsystem_user_type.SelectedIndex <= 0)
            {
                alert.FireAlerts(this.Page, "Please select the user type");
                return false;
            }

            if (String.IsNullOrEmpty(txtsystem_user_fname.Text))
            {
                alert.FireAlerts(this.Page, "Please type the first name");
                return false;
            }

            if (String.IsNullOrEmpty(txtsystem_user_lname.Text))
            {
                alert.FireAlerts(this.Page, "Please type the last name");
                return false;
            }

            if (!emailIsValid(txtsystem_user_email.Text))
            {
                alert.FireAlerts(this.Page, "Please type a valid email address");
                return false;
            }

            if (String.IsNullOrEmpty(txtsystem_user_initials.Text))
            {
                alert.FireAlerts(this.Page, "Please type the users initials");
                return false;
            }

            if (drpsystem_user_tittle.SelectedIndex <= 0)
            {
                alert.FireAlerts(this.Page, "Please select the user title");
                return false;
            }

            if (drpBranchs.SelectedIndex <= 0)
            {
                alert.FireAlerts(this.Page, "Please select the user branch");
                return false;
            }

            if (drpLoginOptions.SelectedIndex <= 0)
            {
                alert.FireAlerts(this.Page, "Please select the login option");
                return false;
            }

            if (drpsystem_user_branch_code.SelectedValue != drpsystem_user_branch.SelectedValue)
            {
                alert.FireAlerts(this.Page, "The branch and branch code do not correspond to each other");
                return false;
            }

            return true;
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEditInstructionsAlloscations.Checked)
            {
                if (String.IsNullOrEmpty(txtsystem_user_id.Text))
                {
                    btnSaveInstructionAllocations.Enabled = true;
                    chkBoxInstructions.Enabled = true;
                }
                else
                {
                    chkEditInstructionsAlloscations.Checked = false;
                    btnSaveInstructionAllocations.Enabled = false;
                    chkBoxInstructions.Enabled = false;
                }

            }
            else
            {
                chkEditInstructionsAlloscations.Checked = false;
                btnSaveInstructionAllocations.Enabled = false;
                chkBoxInstructions.Enabled = false;
            }
        }

        protected void btnSaveInstructionAllocations_Click(object sender, EventArgs e)
        {
            update_instructioon_type_allocation();
        }

        public void loadUsersTeamLeader()
        {

            try
            {

                sharedUtility.LoadGridView(dgvUsers, genericFunctions.GetDataSourceUserGridViewInfo("user_team_leader_view", "system_user_id", txtsystem_user_id.Text));

            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading users." + ex.Message);
                erl.LogError("Error loading users at TL View by user ", ex.Message);
            }
        }

        protected void dgvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt32(e.CommandArgument);
            string id = dgvUsers.Rows[rw].Cells[2].Text;

            if (daccess.RunNonQueryDelete("Delete", "user_team_leader", "user_team_leaders_id", id))
            {
                alert.FireAlerts(this.Page, "Team leader removed successfully");
                loadUsersTeamLeader();
            }
            else
            {
                alert.FireAlerts(this.Page, "Error removing Team leader.");
            }
        }

        protected void drpsystem_user_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            allocateInstructionType(drpsystem_user_type.SelectedValue);
        }

        public bool emailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
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
    }
}