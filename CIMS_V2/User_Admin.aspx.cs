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
    public partial class User_Admin : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        RijndaelEnhanced rienha;
        DAccessInfo daccess = new DAccessInfo();
        Constants constants = new Constants();
        static int selected_user_id;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = 0;

            try
            {
                if (Session["UserID"].ToString() == null || Session["UserID"].ToString() == "")
                {
                    Response.Redirect("Login.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Login.aspx");
            }

            if (Session["UserType"].ToString() != "8")
            {
                Response.Redirect("no_rights.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadPage();
                }
            }
        }

        private void LoadPage()
        {
            drpBranchs.SelectedIndex = -1;

          
            //??? dropdowns
            //load search by
            sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "users" }), "search_by_name", "search_by_value");
            //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("users"), "search_by_name", "search_by_value");
            //"select search_by_value, search_by_name from search_by  where search_by_module = 'users' ORDER BY search_by_name "

            //load user type
            sharedUtility.LoadDropDownList(drpsystem_user_type, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type", "user_type_no" }, new string[] { "visible" }, new string[] { "1" }), "user_type", "user_type_no");
            //sharedUtility.LoadDropDownList(drpsystem_user_type, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type "

            //load user status
            sharedUtility.LoadDropDownList(drpsystem_user_status, genericFunctions.GetDropdownListInfo("user_status", new string[] { "user_status_desc", "user_status", "user_status_order" }, null, null), "user_status_desc", "user_status");
            //sharedUtility.LoadDropDownList(drpsystem_user_status, genericFunctions.GetSearchByDropDownListInfo("user_status"), "user_status_desc", "user_status");
            //"select '0' AS user_status, ' ' As user_status_desc, user_status_order as '-1' from user_status UNION select user_status, user_status_desc, user_status_order from user_status order by user_status_order"

            //load user title
            sharedUtility.LoadDropDownList(drpsystem_user_tittle, genericFunctions.GetDropdownListInfo("user_title", new string[] { "title_id", "title_name" }, null, null), "title_name", "title_id");
            //sharedUtility.LoadDropDownList(drpsystem_user_tittle, genericFunctions.GetSearchByDropDownListInfo("user_title"), "title_name", "title_id");
            //"select '0' AS title_id, ' ' AS title_name from user_title UNION select title_id, title_name from user_title "

            //load user branch
            sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");
            //sharedUtility.LoadDropDownList(drpsystem_user_branch, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
            //"select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch "

            //load user branch code
            sharedUtility.LoadDropDownList(drpsystem_user_branch_code, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_code" }, null, null), "branch_code", "branch_id");
            //sharedUtility.LoadDropDownList(drpsystem_user_branch_code, genericFunctions.GetUserBranchDropDownListInfo(), "branch_code", "branch_id");
            //"select '0' AS branch_id, ' ' AS branch_code from user_branch UNION select branch_id, branch_code from user_branch "

            //load can allocate
            sharedUtility.LoadDropDownList(drpuser_can_allocate_what_document_status, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type_no", "user_type" }, new string[] { "is_team_leader", "visible" }, new string[] { "0", "1" }), "user_type", "user_type_no");
            //sharedUtility.LoadDropDownList(drpuser_can_allocate_what_document_status, genericFunctions.GetSearchByDropDownListInfo("user_type"), "user_type", "user_type_no");
            //"select '0' AS user_type_no, ' ' AS user_type from user_type UNION select user_type_no, user_type from user_type where is_team_leader = 0 "

            //user team leader
            sharedUtility.LoadDropDownList(drpsystem_tl_1, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "is_team_leader" }, new string[] { "1" }), "names", "system_user_id");
            //sharedUtility.LoadDropDownList(drpsystem_tl_1, genericFunctions.GetSearchByDropDownListInfo("names"), "names", "system_user_id");
            //"select '-1' AS system_user_id, ' ' AS names from system_users_view UNION select '0' AS system_user_id, ' Not Applicable' AS names from system_users_view UNION  select system_user_id, names from system_users_view where is_team_leader = 1 "

            //user team leader
            sharedUtility.LoadDropDownList(drpTeamAddLeader, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "is_team_leader" }, new string[] { "1" }), "names", "system_user_id");
            //sharedUtility.LoadDropDownList(drpTeamAddLeader, genericFunctions.GetSearchByDropDownListInfo("names"), "names", "system_user_id");
            //"select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view where is_team_leader = 1 "

            //user modified by
            sharedUtility.LoadDropDownList(drpsystem_user_modified_by, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, null, null), "names", "system_user_id");
            //sharedUtility.LoadDropDownList(drpsystem_user_modified_by, genericFunctions.GetSearchByDropDownListInfo("names"), "names", "system_user_id");
            //"select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view "

            //user manager
            sharedUtility.LoadDropDownList(drpManager, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "names", "system_user_id" }, new string[] { "system_user_type" }, new string[] { "10 | 11" }), "names", "system_user_id");
            //sharedUtility.LoadDropDownList(drpManager, genericFunctions.GetSearchByDropDownListInfo("names"), "names", "system_user_id");
            //"select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view where system_user_type = '10' OR system_user_type = '11' "

            //load checklist
            sharedUtility.LoadCheckList(chkBoxInstructions, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "instruction_type" }, new string[] { "Outward Telegraphic Transfer" }), "instruction_type", "instruction_type_id");
            //sharedUtility.LoadCheckList(chkBoxInstructions, " select instruction_type_id, instruction_type from instructions_types ", "instruction_type", "instruction_type_id");

            //sharedUtility.LoadCheckList(CheckBoxInstructions2, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "instruction_type" }, new string[] { "RTGS ZAM" }), "instruction_type", "instruction_type_id");

            //load user branch
            sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetDropdownListInfo("user_branch", new string[] { "branch_id", "branch_name" }, null, null), "branch_name", "branch_id");
            //sharedUtility.LoadDropDownList(drpBranchs, genericFunctions.GetUserBranchDropDownListInfo(), "branch_name", "branch_id");
            //"select '0' AS branch_id, ' ' AS branch_name from user_branch UNION select branch_id, branch_name from user_branch "

            Page.MaintainScrollPositionOnPostBack = true;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadNames();
            MultiView1.SetActiveView(ViewList);
        }

        public void LoadNames() //searches for users
        {
            try
            {
                //if (drpBranchs.SelectedIndex > -1)
                //{
                //    sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, "system_user_branch", drpBranchs.SelectedValue));
                //    //addSql = addSql + " AND system_user_branch  = '" + drpBranchs.SelectedValue + "' ";
                //}
                string adminName = Session["UserLoginX"].ToString();

                string user_type = daccess.RunStringReturnStringValue1Where("system_users_view", "user_type_no", "system_user_login", "'" + adminName + "'");

                string user_branch = daccess.RunStringReturnStringValue1Where("system_users_view", "system_user_branch", "system_user_login", "'" + adminName + "'");
                string searchby = drpSearchBy.SelectedValue;
                string selectedBranch = drpBranchs.SelectedValue;

                if (user_type != "8")
                {
                    sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, "system_user_branch", user_branch));
                    //addSql = addSql + " AND system_user_branch  = '" + drpBranchs.SelectedValue + "' ";
                }
                else
                {
                    //keep the nulls in here
                    //this currently does not include branch searches (nulls them)
                    sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, null, null));

                    //sharedUtility.LoadGridView(dgvNames, genericFunctions.GetDataSourceUserGridViewInfo("system_users_view", drpSearchBy.SelectedValue, txtValue.Text, "system_user_branch", "6874"));
                }

                //??? sharedUtility - no getDataSet method - will fix other errors in method
                //DataSet myDataSet = sharedUtility.getDataSet(sql, My.Settings.strDSN);

                //if (myDataSet != null)
                //{
                //    dgvNames.DataSource = myDataSet.Tables[0].DefaultView;
                //    dgvNames.DataBind();
                //}
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading users", ex.Message);
            }
        }

        public void ResetUsers()
        {
            txtsystem_user_login.Text = "";
            txtsystem_user_id.Text = "";
            drpsystem_user_type.SelectedIndex = -1;
            drpLoginOptions.SelectedIndex = -1;
            txtsystem_user_password.Text = "";
            txtsystem_user_fname.Text = "";
            //txtsystem_user_mname.Text = "";
            txtsystem_user_lname.Text = "";
            txtsystem_user_email.Text = "";
           // txtsystem_user_id_number.Text = "";
            //txtsystem_user_mobile_number.Text = "";
            //txtsystem_user_land_line.Text = "";
            //txtsystem_user_initials.Text = "";
            drpsystem_user_tittle.SelectedIndex = -1;
            drpsystem_user_branch.SelectedIndex = -1;
            drpsystem_tl_1.SelectedIndex = -1;
            drpuser_can_allocate_what_document_status.SelectedIndex = -1;
            drpsystem_user_branch_code.SelectedIndex = -1;
            txtcan_create.Text = "";
            txtcan_submit.Text = "";
            drpis_team_leader.SelectedIndex = -1;
            txtFailedLoginCount.Text = "";
            chkEditInstructionsAlloscations.Checked = false;

            CheckBox1_CheckedChanged(null, null);

            sharedUtility.LoadCheckList(
                chkBoxInstructions,
                genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "active" }, new string[] { "1" }),
                "instruction_type",
                "instruction_type_id");
        }

        public void getInstructionsForUser()
        {
            for (int i = 0; i <= chkBoxInstructions.Items.Count - 1; i++)
            {
                chkBoxInstructions.Items[i].Selected = false;

                string value = chkBoxInstructions.Items[i].Value;
                if (daccess.HowManyRecordsExist3Wheres("instruction_type_allocations", "system_user_id", txtsystem_user_id.Text, "instruction_type_id", value, "status", "1") > 0)
                {
                    chkBoxInstructions.Items[i].Selected = true;
                }
            }

        }

        public void LoadSystemUsers(int ID)
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

                    LoadUsersTeamLeader();

                    //sharedUtility.LoadDropDownList(drpTeamAddLeader, "select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view where is_team_leader = 1 AND user_can_allocate_what_document_status = '" + drpsystem_user_type.SelectedValue + "'  ", "names", "system_user_id");
                    //sharedUtility.LoadDropDownList(drpsystem_tl_1, "select '-1' AS system_user_id, ' ' AS names from system_users_view UNION select '0' AS system_user_id, ' Not Applicable' AS names from system_users_view UNION select system_user_id, names from system_users_view where is_team_leader = 1 AND user_can_allocate_what_document_status = '" + drpsystem_user_type.SelectedValue + "'  ", "names", "system_user_id");

                    txtsystem_user_password.Text = rdr["system_user_password"].ToString();
                    txtsystem_user_fname.Text = rdr["system_user_fname"].ToString();
                   // txtsystem_user_mname.Text = rdr["system_user_mname"].ToString();
                    txtsystem_user_lname.Text = rdr["system_user_lname"].ToString();
                    txtsystem_user_email.Text = rdr["system_user_email"].ToString();
                   // txtsystem_user_id_number.Text = rdr["system_user_id_number"].ToString();
                   // txtsystem_user_mobile_number.Text = rdr["system_user_mobile_number"].ToString();
                   // txtsystem_user_land_line.Text = rdr["system_user_land_line"].ToString();
                    //txtsystem_user_initials.Text = rdr["system_user_initials"].ToString();

                    indx = drpsystem_user_tittle.Items.IndexOf(drpsystem_user_tittle.Items.FindByValue(rdr["system_user_tittle"].ToString()));
                    drpsystem_user_tittle.SelectedIndex = indx;

                    indx = drpsystem_user_branch.Items.IndexOf(drpsystem_user_branch.Items.FindByValue(rdr["system_user_branch"].ToString()));
                    drpsystem_user_branch.SelectedIndex = indx;

                    indx = drpsystem_tl_1.Items.IndexOf(drpsystem_tl_1.Items.FindByValue(rdr["system_tl_1"].ToString()));
                    drpsystem_tl_1.SelectedIndex = indx;

                    indx = drpuser_can_allocate_what_document_status.Items.IndexOf(drpuser_can_allocate_what_document_status.Items.FindByValue(rdr["user_can_allocate_what_document_status"].ToString()));
                    drpuser_can_allocate_what_document_status.SelectedIndex = indx;

                    indx = drpsystem_user_branch_code.Items.IndexOf(drpsystem_user_branch_code.Items.FindByText(rdr["system_user_branch_code"].ToString()));
                    drpsystem_user_branch_code.SelectedIndex = indx;

                    indx = drpLoginOptions.Items.IndexOf(drpLoginOptions.Items.FindByText(rdr["login_option"].ToString()));
                    drpLoginOptions.SelectedIndex = indx;

                    txtcan_create.Text = rdr["can_create"].ToString();
                    txtcan_submit.Text = rdr["can_submit"].ToString();

                    indx = drpManager.Items.IndexOf(drpManager.Items.FindByValue(rdr["system_manager"].ToString()));
                    drpManager.SelectedIndex = indx;

                    indx = drpis_team_leader.Items.IndexOf(drpis_team_leader.Items.FindByValue(rdr["is_team_leader"].ToString()));
                    drpis_team_leader.SelectedIndex = indx;

                    string systemUserActive = rdr["system_user_active"].ToString();
                    int result = 0;
                    if (int.TryParse(systemUserActive, out result))
                    {
                        if (result == 1)
                        {
                            chksystem_user_active.Checked = true;
                        }
                        else
                        {
                            chksystem_user_active.Checked = false;
                        }

                        int changePassword = Convert.ToInt32(rdr["change_password"]);
                        if (changePassword == 1)
                        {
                            Chkchange_password.Checked = true;
                        }
                        else
                        {
                            Chkchange_password.Checked = false;
                        }
                    }

                    drpis_team_leader_SelectedIndexChanged(null, null);
                    ShouldIAllocateInstructionTypes(rdr["system_user_type"].ToString());
                    getInstructionsForUser();

                    chkResetPassword.Checked = false;
                    txtsystem_user_password.ReadOnly = true;

                    indx = drpsystem_user_status.Items.IndexOf(drpsystem_user_status.Items.FindByValue(rdr["system_user_status"].ToString()));
                    drpsystem_user_status.SelectedIndex = indx;

                    indx = drpsystem_user_modified_by.Items.IndexOf(drpsystem_user_modified_by.Items.FindByValue(rdr["system_user_modified_by"].ToString()));
                    drpsystem_user_modified_by.SelectedIndex = indx;

                    if (drpsystem_user_status.SelectedValue == "1")
                    {
                        btnApprove.Visible = false;
                        btnSave.Visible = true;
                    }
                    else
                    {
                        if (drpsystem_user_modified_by.SelectedValue == Session["UserID"].ToString())
                        {
                            btnApprove.Visible = false;
                            btnSave.Visible = true;
                        }
                        else
                        {
                            btnApprove.Visible = true;
                            btnSave.Visible = false;
                        }
                    }

                    txtFailedLoginCount.Text = rdr["failed_login_count"].ToString();
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading users details", ex.Message);
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
            ResetUsers();

            selected_user_id = Convert.ToInt32(dgvNames.Rows[rw].Cells[0].Text);
            LoadSystemUsers(selected_user_id);
            MultiView1.SetActiveView(ViewDetails);
        }

        protected void drpis_team_leader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpis_team_leader.SelectedItem.Text == "Yes")
            {
                drpsystem_tl_1.Enabled = false;
                drpuser_can_allocate_what_document_status.Enabled = true;
                drpManager.Enabled = true;
            }
            else
            {
                drpsystem_tl_1.Enabled = true;
                drpuser_can_allocate_what_document_status.Enabled = false;
                drpManager.Enabled = false;
            }
        }

        public void ShouldIAllocateInstructionTypes(string userType)
        {
            switch (userType)
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateUsers())
            {
                string status = "InActive";
                //???
                //string myEmail = Session["UserEmail"].ToString();

                if (chksystem_user_active.Checked)
                {
                    status = "Active";
                }

                if (daccess.HowManyRecordsExist1Wheres("system_users", "system_user_id", txtsystem_user_id.Text) == 0)
                {
                    if (chkResetPassword.Checked && drpLoginOptions.SelectedItem.Text != "ldap")
                    {
                        if (string.IsNullOrEmpty(txtsystem_user_password.Text))
                        {
                            alert.FireAlerts(this.Page, "Please type the password");
                            return;
                        }
                    }

                    int insertedUserID = ProcInsertSystemUsers();
                    if (insertedUserID > 0)
                    {
                        string recipient = txtsystem_user_email.Text;
                        string comments = "";
                        if (drpsystem_tl_1.SelectedIndex < 0)
                        {
                            comments = "  " + Environment.NewLine +
                        " <table border=1> <tr> " + Environment.NewLine +
                        " 	<td>User Name</td><td>" + txtsystem_user_login.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>Full Name</td><td>" + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>User Type</td><td>" + drpsystem_user_type.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                            " 	<td>User Title</td><td>" + drpsystem_user_tittle.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                        "<tr>" + Environment.NewLine +
                        " 	<td>User Branch</td><td>" + drpsystem_user_branch.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Status</td><td>" + status + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Primary Manager </td><td>" + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Password</td><td>Please ask the creater to advise </td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Created By</td>(<td>" + Session["UserFullName"].ToString() + "</td>" + Environment.NewLine +
                        " </tr></table>";
                        }
                        else
                        {
                            comments = "  " + Environment.NewLine +
                        " <table border=1> <tr> " + Environment.NewLine +
                        " 	<td>User Name</td><td>" + txtsystem_user_login.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>Full Name</td><td>" + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>User Type</td><td>" + drpsystem_user_type.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                            " 	<td>User Title</td><td>" + drpsystem_user_tittle.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                        "<tr>" + Environment.NewLine +
                        " 	<td>User Branch</td><td>" + drpsystem_user_branch.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Status</td><td>" + status + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Primary Team Leader </td><td>" + drpsystem_tl_1.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Password</td><td>Please ask the creater to advise </td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Created By</td>(<td>" + Session["UserFullName"].ToString() + "</td>" + Environment.NewLine +
                        " </tr></table>";
                        }


                        //??? sharedUtility - missing methods
                        //sharedUtility.SendMail(myEmail, "", "USER CREATED", BuildMailHeader() + BuildMailBody(comments) + BuildMailFooter(), recipient, "high", "", "html");
                        alert.FireAlerts(this.Page, "User added successfully");

                        try
                        {
                            sharedUtility.SendMail(txtsystem_user_email.Text,
                                               "USER ADDED - " + System.DateTime.Now,
                                               //email_to + "\r\n\r\n" +
                                               "Hi " + txtsystem_user_fname.Text + " \r\n\r\n" +
                                               "You have been added to the CIMS system as a " + drpsystem_user_type.SelectedItem.Text + " with the following details: \r\n\r\n" +
                                               "    USERNAME    : " + txtsystem_user_login.Text + " \r\n" +
                                               "    PASSWORD      : " + txtsystem_user_password.Text);
                        }
                        catch (Exception ex)
                        {
                            //??? log error?
                            erl.LogError("Failed to send login details to " + txtsystem_user_email.Text, ex.Message);
                            alert.FireAlerts(this.Page, "Failed to send login details to " + txtsystem_user_email.Text + "\r\nPlease notify them manually.");
                        }

                        LoadSystemUsers(insertedUserID);
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "Error adding user");
                    }
                }
                else
                {
                    if (ProcUpdateSystemUsers())
                    {
                        string recipient = txtsystem_user_email.Text;
                        string comments = "  " + Environment.NewLine +
                        " <table border=1> <tr> " + Environment.NewLine +
                        " 	<td>User Name</td><td>" + txtsystem_user_login.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>Full Name</td><td>" + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                         "<tr>" + Environment.NewLine +
                        " 	<td>User Type</td><td>" + drpsystem_user_type.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                          " 	<td>User Title</td><td>" + drpsystem_user_tittle.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                        "<tr>" + Environment.NewLine +
                        " 	<td>User Branch</td><td>" + drpsystem_user_branch.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Status</td><td>" + status + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Primary Team Leader </td><td>" + drpsystem_tl_1.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Primary Manager </td><td>" + drpManager.SelectedItem.Text + "</td>" + Environment.NewLine +
                        " </tr>" +
                              "<tr>" + Environment.NewLine +
                        " 	<td>Password</td><td>Please ask the updator to advise</td>" + Environment.NewLine +
                        " </tr>" +
                          "<tr>" + Environment.NewLine +
                        " 	<td>Modified By</td><td>" + Session["UserFullName"].ToString() + "</td>" + Environment.NewLine +
                        " </tr></table>";

                        if (chkResetPassword.Checked)
                        {
                            if (!string.IsNullOrEmpty(txtsystem_user_password.Text))
                            {
                                string gPassPhrase = "cfcstanbicbank..";
                                string gInitVector = "standardbankgroup";
                                rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);

                                daccess.RunNonQuery1Where("Update", "system_users", new string[] { "system_user_password" }, new string[] { rienha.Encrypt(txtsystem_user_password.Text) }, "system_user_id", txtsystem_user_id.Text);
                                chkResetPassword.Checked = false;
                                txtsystem_user_password.ReadOnly = true;
                            }
                        }

                        //??? sharedUtility = no sendmail method
                        //sharedUtility.SendMail(myEmail, "", "USER UPDATED", BuildMailHeader() + BuildMailBody(comments) + BuildMailFooter(), recipient, "high", "", "html");
                        alert.FireAlerts(this.Page, "User updated successfully");

                        try
                        {
                            sharedUtility.SendMail(txtsystem_user_email.Text,
                                               "USER UPDATED - " + System.DateTime.Now,
                                               //email_to + "\r\n\r\n" +
                                               "Hi " + txtsystem_user_fname.Text + " \r\n\r\n" +
                                               "Your login details have been updated \r\n\r\n");
                        }
                        catch (Exception ex)
                        {
                            //??? log error?
                            erl.LogError("Failed to send update notification to " + txtsystem_user_email.Text, ex.Message);
                            alert.FireAlerts(this.Page, "Failed to send update notification to " + txtsystem_user_email.Text + "\r\nPlease notify them manually.");
                        }
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "Error updating user");
                    }
                }
            }
        }

        public string BuilMailHeader()
        {
            string myString = " <table> <br> <tr>" + Environment.NewLine +
                              "	<td>Dear User," + Environment.NewLine +
                              " </tr>  <table>";
            return myString;
        }

        public string BuildMailBody(string comments)
        {
            string myString = " <table><br> <tr>" + Environment.NewLine +
                              "	<td>Please note that a user has been created / updated .<br> <br>See details below<br> <br>" + comments + "<br> <br>" + Environment.NewLine +
                              " </tr></table>";
            return myString;
        }

        public string BuildMailFooter()
        {
            string myString = "";
            //??? My
            //myString = " <table><br><tr> " + Environment.NewLine +
            //           " <td>Kind Regards,<br> <br>CIMS. &nbsp;<br>. &nbsp;<br>Link: " + My.Settings.link + " &nbsp;" + Environment.NewLine +
            //           " </tr></table> ";
            return myString;
        }

        public void UpdateInstructionTypeAllocation()
        {
            for (int i = 0; i <= chkBoxInstructions.Items.Count - 1; i++)
            {
                string instructionTypeID = chkBoxInstructions.Items[i].Value;
                bool selected = chkBoxInstructions.Items[i].Selected;
                int sel = Convert.ToInt16(selected);

                if (daccess.HowManyRecordsExist2Wheres("instruction_type_allocations", "system_user_id", txtsystem_user_id.Text, "instruction_type_id", instructionTypeID) > 0)
                {
                    daccess.RunNonQuery2Wheres("Update", "instruction_type_allocations", new string[] { "status", "modified_by", "modified_date" }, new string[] { sel.ToString(), Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "system_user_id", txtsystem_user_id.Text, "instruction_type_id", instructionTypeID);
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text + " instruction type allocation updated successfully with the following details: Status: " + sel + " User ID: " + txtsystem_user_id.Text + "' and instruction_type_id: '" + instructionTypeID + "'", "", "0", 0, "User instruction type allocation updated successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
                else
                {
                    daccess.RunNonQueryInsert("Insert", "instruction_type_allocations", new string[] { "instruction_type_id", "system_user_id", "status", "inserted_date", "inserted_by", "modified_date", "modified_by" }, new string[] { instructionTypeID, txtsystem_user_id.Text, sel.ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString() });
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "message", "", "0", 0, "User instruction type allocation inserted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }

                alert.FireAlerts(this.Page, "Operation complete");
            }
        }

        public Boolean ValidateUsers()
        {
            if (string.IsNullOrEmpty(txtsystem_user_login.Text))
            {
                alert.FireAlerts(this.Page, "Please type the login name");
                return false;
            }
            using (CIMS_Entities entities = new CIMS_Entities())
            {
                //check if a user with that login already exists in the database
                //filthy linq hurts my eyes but does the job feelsgoodman
                var duplicateUser = from u in entities.system_users
                                    where u.system_user_login == txtsystem_user_login.Text
                                    select u.system_user_login;

                if (duplicateUser.Contains(txtsystem_user_login.Text))
                {
                    alert.FireAlerts(this.Page, "A user with that login already exists. Please type a different user login.");
                    return false;
                }
            }

            if (drpsystem_user_type.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the user type");
                return false;
            }

            if (chkResetPassword.Checked)
            {
                if (string.IsNullOrEmpty(txtsystem_user_password.Text))
                {
                    alert.FireAlerts(this.Page, "Please type the password");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtsystem_user_fname.Text))
            {
                alert.FireAlerts(this.Page, "Please type the first name");
                return false;
            }

            if (string.IsNullOrEmpty(txtsystem_user_lname.Text))
            {
                alert.FireAlerts(this.Page, "Please type the last name");
                return false;
            }

            //??? sharedutility - missing method
            //if (!sharedUtility.is_this_a_valid_email_address(txtsystem_user_email.Text))
            //{
            //    alert.FireAlerts(this.Page, "Please type a valid email address");
            //    return false;
            //}

            //if (string.IsNullOrEmpty(txtsystem_user_initials.Text))
            //{
            //    alert.FireAlerts(this.Page, "Please type the user initials");
            //    return false;
            //}

            //if (drpsystem_user_tittle.SelectedIndex < 0)
            //{
            //    alert.FireAlerts(this.Page, "Please selecty the user title");
            //    return false;
            //}

            if (drpsystem_user_branch.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the user branch");
                return false;
            }

            if (drpsystem_user_branch_code.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the user branch code");
                return false;
            }

            if (drpLoginOptions.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please select the login option");
                return false;
            }
           // CIMS_Entities branchFinder = new CIMS_Entities();

            //string branchCode = branchFinder.user_branch.FirstOrDefault(b => b.branch_id == Convert.ToInt32(drpsystem_user_branch.SelectedValue)).branch_code;

 //           if (drpsystem_user_branch_code.SelectedValue != drpsystem_user_branch.SelectedValue)
  //          {
  //              alert.FireAlerts(this.Page, "The branch and branch code do no correspond to each other");
   //             return false;
//}

            if (drpis_team_leader.SelectedIndex < 0)
            {
                alert.FireAlerts(this.Page, "Please indicate if the user ia a team leader or not");
                return false;
            }

            if (drpis_team_leader.SelectedIndex == 2)
            {
                if (drpuser_can_allocate_what_document_status.SelectedIndex < 0)
                {
                    alert.FireAlerts(this.Page, "Please select 'Is Team Leader To'");
                    return false;
                }

                if (drpManager.SelectedIndex < 0)
                {
                    alert.FireAlerts(this.Page, "Please select the manager");
                    return false;
                }
            }
            else
            {
                if (drpsystem_tl_1.SelectedIndex < 0 && drpsystem_user_type.SelectedItem.Text != "Systems Admin" && drpsystem_user_type.SelectedItem.Text != "Primary Manager")
                {
                    alert.FireAlerts(this.Page, "Please select the user team leader");
                    return false;
                }
            }

            string duplicateExist = daccess.RunStringReturnStringValue3Wheres("system_users", "count(*)", "system_user_login", txtsystem_user_login.Text, "system_user_id <> " + txtsystem_user_id.Text + " AND 1", "1", "1", "1");
            double result = 0;
            if (double.TryParse(duplicateExist, out result) && result > 0)
            {
                alert.FireAlerts(this.Page, "Cannot save because duplicate user name (" + txtsystem_user_login.Text + ") already exists.");
                return false;
            }



            return true;
        }

        public int ProcInsertSystemUsers()
        {
            int result;
            string systemUserFname = txtsystem_user_fname.Text;
            string systemUserLname = txtsystem_user_lname.Text;

            try
            {


                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {


                    SqlCommand myCommand = new SqlCommand("proc_insert_system_users", myConnection);
                    result = 0;

                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    string systemUserLogin = txtsystem_user_login.Text;
                    int systemUserType = Convert.ToInt32(drpsystem_user_type.SelectedValue);
                    string gPassPhrase = "cfcstanbicbank..";
                    string gInitVector = "standardbankgroup";
                    rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);
                    string systemUserPassword = rienha.Encrypt(txtsystem_user_password.Text);
                    string systemUserMname = "";
                    string systemUserEmail = txtsystem_user_email.Text;
                    string systemUserIdNumber = "";
                    int systemUserActive = Convert.ToInt16(chksystem_user_active.Checked);
                    string systemUserMobileNumber = "";
                    string systemUserLandLine = "";
                    int changePassword = Convert.ToInt16(Chkchange_password.Checked);
                    string systemUserInitials = "";
                    int systemUserTitle = Convert.ToInt16(drpsystem_user_tittle.SelectedValue);
                    int systemUserBranch = Convert.ToInt32(drpsystem_user_branch.SelectedValue);
                    CIMS_Entities _db = new CIMS_Entities();
                    string branchCode = _db.user_branch.FirstOrDefault(b => b.branch_id == systemUserBranch).branch_code;
                    int systemTL1;
                    if (drpsystem_tl_1.SelectedIndex < 0)
                    {
                        systemTL1 = -1;
                    }
                    else
                    {
                        systemTL1 = Convert.ToInt16(drpsystem_tl_1.SelectedValue);
                    }
                    int userCanAllocateWhatDocumentStatus = Convert.ToInt16(drpuser_can_allocate_what_document_status.SelectedValue);

                    //string systemUserBranchCode = drpsystem_user_branch_code.SelectedItem.Text;
                    int isTeamLeader = Convert.ToInt16(drpis_team_leader.SelectedValue);
                    int systemManager = Convert.ToInt16(drpManager.SelectedIndex);
                    string checkSum = "";
                    //??? sharedUtility - missing method
                    //checkSum = sharedUtility.generate_user_cheksum(system_user_login, system_user_type, system_user_active);
                    string loginOption = drpLoginOptions.SelectedItem.Text;
                    int no_of_instructions_i_can_pack = 2;
                    int totalWorkAllocated = 0;

                    myCommand.Parameters.Add("@system_user_login", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_type", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_password", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_fname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_mname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_lname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_email", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_id_number", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_active", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_mobile_number", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_land_line", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@change_password", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_initials", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_tittle", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_branch", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_tl_1", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_can_allocate_what_document_status", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_branch_code", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@is_team_leader", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_manager", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_modified_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_inserted_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@check_sum", SqlDbType.NVarChar);
                    myCommand.Parameters.Add("@login_option", SqlDbType.NVarChar);
                    myCommand.Parameters.Add("@id_out", SqlDbType.Int);
                    myCommand.Parameters.Add("@no_of_instructions_i_can_pack", SqlDbType.Int);
                    myCommand.Parameters.Add("@total_work_allocated", SqlDbType.Int);

                    myCommand.Parameters["@system_user_login"].Value = systemUserLogin;
                    myCommand.Parameters["@system_user_type"].Value = systemUserType;
                    myCommand.Parameters["@system_user_password"].Value = systemUserPassword;
                    myCommand.Parameters["@system_user_fname"].Value = systemUserFname;
                    myCommand.Parameters["@system_user_mname"].Value = systemUserMname;
                    myCommand.Parameters["@system_user_lname"].Value = systemUserLname;
                    myCommand.Parameters["@system_user_email"].Value = systemUserEmail;
                    myCommand.Parameters["@system_user_id_number"].Value = systemUserIdNumber;
                    myCommand.Parameters["@system_user_active"].Value = systemUserActive;
                    myCommand.Parameters["@system_user_mobile_number"].Value = systemUserMobileNumber;
                    myCommand.Parameters["@system_user_land_line"].Value = systemUserLandLine;
                    myCommand.Parameters["@change_password"].Value = changePassword;
                    myCommand.Parameters["@system_user_initials"].Value = systemUserInitials;
                    myCommand.Parameters["@system_user_tittle"].Value = systemUserTitle;
                    myCommand.Parameters["@system_user_branch"].Value = systemUserBranch;
                    myCommand.Parameters["@system_tl_1"].Value = systemTL1;
                    myCommand.Parameters["@user_can_allocate_what_document_status"].Value = userCanAllocateWhatDocumentStatus;
                    myCommand.Parameters["@system_user_branch_code"].Value = branchCode;
                    myCommand.Parameters["@is_team_leader"].Value = isTeamLeader;
                    myCommand.Parameters["@system_manager"].Value = systemManager;
                    myCommand.Parameters["@system_user_modified_by"].Value = Convert.ToInt32(Session["UserID"]);
                    myCommand.Parameters["@system_user_inserted_by"].Value = Convert.ToInt32(Session["UserID"]);
                    myCommand.Parameters["@check_sum"].Value = checkSum;
                    myCommand.Parameters["@login_option"].Value = loginOption;
                    myCommand.Parameters["@no_of_instructions_i_can_pack"].Value = no_of_instructions_i_can_pack;
                    myCommand.Parameters["@id_out"].Direction = ParameterDirection.ReturnValue;
                    myCommand.Parameters["@total_work_allocated"].Value = totalWorkAllocated;

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    result = Convert.ToInt32(myCommand.Parameters["@id_out"].Value);
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User inserted successfully with the following details: Names:  " + systemUserFname + " " + systemUserLname, "", "0", 0, "System user inserted successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error inserting user with the following details: Names: " + systemUserFname + " " + systemUserLname, "", "0", 0, "Error inserting system user", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error inserting user", ex.Message);
                result = 0;
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

        public Boolean ProcUpdateSystemUsers()
        {
            bool result;
            string systemUserFname = txtsystem_user_fname.Text;

            string systemUserLname = txtsystem_user_lname.Text;

            try
            {


                using (SqlConnection myConnection = new SqlConnection(constants.getConnectionString()))
                {



                    SqlCommand myCommand = new SqlCommand("proc_update_system_users", myConnection);
                    result = false;

                    myCommand.CommandTimeout = 0;
                    myCommand.CommandType = CommandType.StoredProcedure;

                    string systemUserLogin = txtsystem_user_login.Text;
                    int systemUserType = Convert.ToInt32(drpsystem_user_type.SelectedValue);
                    string gPassPhrase = "cfcstanbicbank..";
                    string gInitVector = "standardbankgroup";
                    rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);
                    string systemUserPassword = rienha.Encrypt(txtsystem_user_password.Text);
                    string systemUserMname = "";
                    string systemUserEmail = txtsystem_user_email.Text;
                    string systemUserIdNumber = "";
                    int systemUserActive = Convert.ToInt16(chksystem_user_active.Checked); //int16 can't handle large values so be careful
                    string systemUserMobileNumber = "";
                    string systemUserLandLine = "";
                    int changePassword = Convert.ToInt16(Chkchange_password.Checked);
                    string systemUserInitials = "";
                    int systemUserTitle = Convert.ToInt16(drpsystem_user_tittle.SelectedValue);
                    int systemUserBranch = Convert.ToInt16(drpsystem_user_branch.SelectedValue);
                    int systemTL1 = Convert.ToInt16(drpsystem_tl_1.SelectedValue);
                    int userCanAllocateWhatDocumentStatus = Convert.ToInt16(drpuser_can_allocate_what_document_status.SelectedValue);
                    CIMS_Entities _db = new CIMS_Entities();
                    string branchCode = _db.user_branch.FirstOrDefault(b => b.branch_id == systemUserBranch).branch_code;
                    //string systemUserBranchCode = drpsystem_user_branch_code.SelectedItem.Text;
                    int isTeamLeader = Convert.ToInt16(drpis_team_leader.SelectedValue);
                    int systemUserID = Convert.ToInt16(txtsystem_user_id.Text);
                    int systemManager = Convert.ToInt16(drpManager.SelectedValue);
                    string checkSum = "";
                    //??? sharedUtility - missing method
                    //checkSum = sharedUtility.generate_user_cheksum(system_user_login, system_user_type, system_user_active);
                    string loginOption = drpLoginOptions.SelectedItem.Text;

                    myCommand.Parameters.Add("@system_user_login", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_type", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_password", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_fname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_id", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_mname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_lname", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_email", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_id_number", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_active", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_mobile_number", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_land_line", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@change_password", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_initials", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@system_user_tittle", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_branch", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_tl_1", SqlDbType.Int);
                    myCommand.Parameters.Add("@user_can_allocate_what_document_status", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_branch_code", SqlDbType.VarChar);
                    myCommand.Parameters.Add("@is_team_leader", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_manager", SqlDbType.Int);
                    myCommand.Parameters.Add("@system_user_modified_by", SqlDbType.Int);
                    myCommand.Parameters.Add("@check_sum", SqlDbType.NVarChar);
                    myCommand.Parameters.Add("@login_option", SqlDbType.NVarChar);
                    myCommand.Parameters.Add("@id_out", SqlDbType.Int);

                    myCommand.Parameters["@system_user_login"].Value = systemUserLogin;
                    myCommand.Parameters["@system_user_type"].Value = systemUserType;
                    myCommand.Parameters["@system_user_password"].Value = systemUserPassword;
                    myCommand.Parameters["@system_user_fname"].Value = systemUserFname;
                    myCommand.Parameters["@system_user_id"].Value = selected_user_id;
                    myCommand.Parameters["@system_user_mname"].Value = systemUserMname;
                    myCommand.Parameters["@system_user_lname"].Value = systemUserLname;
                    myCommand.Parameters["@system_user_email"].Value = systemUserEmail;
                    myCommand.Parameters["@system_user_id_number"].Value = systemUserIdNumber;
                    myCommand.Parameters["@system_user_active"].Value = systemUserActive;
                    myCommand.Parameters["@system_user_mobile_number"].Value = systemUserMobileNumber;
                    myCommand.Parameters["@system_user_land_line"].Value = systemUserLandLine;
                    myCommand.Parameters["@change_password"].Value = changePassword;
                    myCommand.Parameters["@system_user_initials"].Value = systemUserInitials;
                    myCommand.Parameters["@system_user_tittle"].Value = systemUserTitle;
                    myCommand.Parameters["@system_user_branch"].Value = systemUserBranch;
                    myCommand.Parameters["@system_tl_1"].Value = systemTL1;
                    myCommand.Parameters["@user_can_allocate_what_document_status"].Value = userCanAllocateWhatDocumentStatus;
                    myCommand.Parameters["@system_user_branch_code"].Value = branchCode;
                    myCommand.Parameters["@is_team_leader"].Value = isTeamLeader;
                    myCommand.Parameters["@system_manager"].Value = systemManager;
                    myCommand.Parameters["@system_user_modified_by"].Value = Convert.ToInt32(Session["UserID"]);
                    //myCommand.Parameters["@system_user_inserted_by"].Value = Convert.ToInt32(Session["UserID"]);
                    myCommand.Parameters["@check_sum"].Value = checkSum;
                    myCommand.Parameters["@login_option"].Value = loginOption;
                    myCommand.Parameters["@id_out"].Direction = ParameterDirection.ReturnValue;

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    result = true;
                    operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User updated successfully with the following details: Names:  " + systemUserFname + " " + systemUserLname, "", "0", 0, "System user updated successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                }
            }
            catch (Exception ex)
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error updating user with the following details: Names: " + systemUserFname + " " + systemUserLname, "", "0", 0, "Error updating system user", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                erl.LogError("Error updating user", ex.Message);
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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetUsers();
            chkResetPassword.Enabled = true;
            btnApprove.Visible = false;
            btnSave.Visible = true;
            MultiView1.SetActiveView(ViewDetails);
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEditInstructionsAlloscations.Checked)
            {
                if (!string.IsNullOrEmpty(txtsystem_user_id.Text))
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
            UpdateInstructionTypeAllocation();
        }

        protected void drpTeamAddLeader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (daccess.HowManyRecordsExist2Wheres("user_team_leader", "system_user_id", txtsystem_user_id.Text, "system_tl_1", drpTeamAddLeader.SelectedValue) == 0)
            {
                btnAddTL.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = false;
            }
        }

        protected void btnAddTL_Click(object sender, EventArgs e)
        {
            if (daccess.RunNonQueryInsert("Insert", "user_team_leader", new string[] { "system_user_id", "system_tl_1", "active", "inserted_date", "modified_date", "inserted_by", "modified_by" }, new string[] { txtsystem_user_id.Text, drpTeamAddLeader.SelectedValue, "1", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString(), Session["UserID"].ToString() }))
            {
                alert.FireAlerts(this.Page, "Team leader added usccessfully");
                drpTeamAddLeader_SelectedIndexChanged(null, null);
                LoadUsersTeamLeader();
            }
            else
            {
                alert.FireAlerts(this.Page, "Error adding team leader");
            }
        }

        public void LoadUsersTeamLeader()
        {
            //??? replace sql
            //string sql = "Select * from user_team_leader_view where system_user_id = '" + txtsystem_user_id.Text + "' ";
            //string addSql = "";

            try
            {
                //??? sharedUtility - missing method
                //DataSet myDataSet = sharedUtility.getDataSet(sql, My.Settings.strDSN);

                sharedUtility.LoadGridView(dgvUsers, genericFunctions.GetDataSourceUserGridViewInfo("user_team_leader_view", "system_user_id", txtsystem_user_id.Text));

                //if (myDataSet != null)
                //{
                //    dgvUsers.DataSource = myDataSet.Tables[0].DefaultView;
                //    dgvUsers.DataBind();
                //}
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading users");
                erl.LogError("Error loading users at TL View", ex.Message);
            }
        }

        protected void dgvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rw = Convert.ToInt32(e.CommandArgument);
            string id = dgvUsers.Rows[rw].Cells[2].Text;

            if (daccess.RunNonQueryDelete("Delete", "user_team_leader", "user_team_leaders_id", id))
            {
                alert.FireAlerts(this.Page, "Team leader removed successfully");
                drpTeamAddLeader_SelectedIndexChanged(null, null);
                LoadUsersTeamLeader();
            }
            else
            {
                alert.FireAlerts(this.Page, "Error removing team leader");
            }
        }

        protected void chkResetPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkResetPassword.Checked)
            {
                txtsystem_user_password.ReadOnly = false;
            }
            else
            {
                txtsystem_user_password.ReadOnly = true;
            }
        }

        protected void drpsystem_user_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            sharedUtility.LoadDropDownList(
                drpsystem_tl_1,
                genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "is_team_leader", "user_can_allocate_what_document_status" }, new string[] { "1", drpsystem_user_type.SelectedValue.ToString() }),
                "names",
                "system_user_id");


            //sharedUtility.LoadDropDownList(drpsystem_tl_1, "select '-1' AS system_user_id, ' ' AS names from system_users_view UNION select '0' AS system_user_id, ' Not Applicable' AS names from system_users_view UNION select system_user_id, names from system_users_view where is_team_leader = 1 AND user_can_allocate_what_document_status = '" + drpsystem_user_type.SelectedValue + "'  ", "names", "system_user_id");

            sharedUtility.LoadDropDownList(
                drpTeamAddLeader,
                genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "is_team_leader", "user_can_allocate_what_document_status" }, new string[] { "1", drpsystem_user_type.SelectedValue }),
                "names",
                "system_user_id");



            //sharedUtility.LoadDropDownList(drpTeamAddLeader, "select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view where is_team_leader = 1 AND user_can_allocate_what_document_status = '" + drpsystem_user_type.SelectedValue + "'  ", "names", "system_user_id");
            ShouldIAllocateInstructionTypes(drpsystem_user_type.SelectedValue);
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                //get the approved user's ID
                int userId = Convert.ToInt32(txtsystem_user_id.Text);
                CIMS_Entities _db = new CIMS_Entities();
                system_users tempUser = _db.system_users.FirstOrDefault(u => u.system_user_id == userId);

                if (tempUser.instruction_type_allocations.FirstOrDefault() == null) //if they don't have an instruction type allocation
                {
                    alert.FireAlerts(this.Page, "Please allocate an instruction type to the user before approving them");
                    return;
                }
                else
                {
                    if (daccess.RunNonQuery1Where("Update", "system_users", new string[] { "system_user_active", "system_user_status", "system_user_approved_by", "system_user_approved_date" }, new string[] { "1", "1", Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "system_user_id", txtsystem_user_id.Text))
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User activated successfully with the following details: Names: " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text, "", "0", 0, "User activated successfully", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                        alert.FireAlerts(this.Page, "User activated successfully");
                    }
                    else
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error activating user with the following details: Names: " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text, "", "0", 0, "Error activating user", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }
            }
            catch(Exception ex)
            {
                alert.FireAlerts(this.Page, "Error approving the user " + ex.Message);
            }
        }

        protected void ResetFailedLogin_Click(object sender, EventArgs e)
        {
            if (daccess.RunNonQuery1Where("Update", "system_users", new string[] { "failed_login_count" }, new string[] { "0" }, "system_user_id", txtsystem_user_id.Text))
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User failed logins reset successfully with the following details: Names: " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text, "", "0", 0, "User failed logins reset", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                alert.FireAlerts(this.Page, "User failed logins reset successfully");
            }
            else
            {
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error resetting user failed logins with the following details: Names: " + txtsystem_user_fname.Text + " " + txtsystem_user_lname.Text, "", "0", 0, "Error resetting user failed logins", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
            }
        }

        protected void drpsystem_user_branch_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpsystem_user_branch_code.SelectedIndex = drpsystem_user_branch.SelectedIndex;
        }

        protected void btnActivation_Click(object sender, EventArgs e)
        {
            CIMS_Entities entities = new CIMS_Entities();
            //find the user


            system_users targetUser = entities.system_users.FirstOrDefault(u => u.system_user_login == txtsystem_user_login.Text);

            int userId = Convert.ToInt32(targetUser.system_user_id);
            //change their activation status
            string activeStatus;
            if (targetUser.system_user_active == 0)
            {
                targetUser.system_user_active = 1;
                activeStatus = "Active";
            }
            else
            {
                targetUser.system_user_active = 0;
                activeStatus = "Inactive";
            }

            //update the database
            try
            {
                entities.SaveChanges();

            }
            catch(Exception ex)
            {
                erl.LogError("Failed to change user activation", ex.Message);
                alert.FireAlerts(this.Page, "Failed to update user activation status");
            }


                try
                {
                    //notify the current user
                    alert.FireAlerts(this.Page, "User activation status updated to be " + activeStatus);
                    //notify changed user
                    sharedUtility.SendMail(txtsystem_user_email.Text,
                                       "USER UPDATED - " + System.DateTime.Now,
                                       //email_to + "\r\n\r\n" +
                                       "Hi " + txtsystem_user_fname.Text + " \r\n\r\n" +
                                       "Your activation status has been updated to be \r\n\r\n " + activeStatus);
                }
                catch (Exception ex)
                {
                    //??? log error?
                    erl.LogError("Failed to send update notification to " + txtsystem_user_email.Text, ex.Message);
                    alert.FireAlerts(this.Page, "Failed to send update notification to " + txtsystem_user_email.Text + "\r\nPlease notify them manually.");
                }
            }

        }
        
    
}