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
using CIMS_Datalayer;
using CIMS_V2.AddOn;

namespace CIMS_V2
{
    public partial class Managers_View : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
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

            if (!Page.IsPostBackEventControlRegistered)
            {
                sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_value", "search_by_name" }, new string[] { "search_by_module" }, new string[] { "instructions" }), "search_by_name", "search_by_value");
                //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");

                //load document status
                sharedUtility.LoadDropDownList(drpProcessingStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null), "document_status", "document_status_id");
                //sharedUtility.LoadDropDownList(drpProcessingStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status", "document_status_id");

                //??? dropdown query
                //load users
                sharedUtility.LoadDropDownList(drpTeamLeaders, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, new string[] { "system_user_type", "system_manager", "is_team_leader" }, new string[] { "4 | 6 | 10", Session["UserID"].ToString(), "1" }), "names", "system_user_id");
                //sharedUtility.LoadDropDownList(drpTeamLeaders, "select '0' AS system_user_id, AS names from system_users_view UNION select system_user_id, names from system_users_view Where (user_type_no = 4 or user_type_no = 6) AND system_manager = '" + Session["UserID"].ToString() + "' AND is_team_leader = 1 order by names ", "names", "system_user_id");

                //??? dropdown query
                //load users
                sharedUtility.LoadDropDownList(drpAllocatedTo, genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, null, null), "names", "system_user_id");
                //sharedUtility.LoadDropDownList(drpAllocatedTo, "select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view ", "names", "system_user_id");

                //' Dim indx As Integer = drpProcessingStatus.Items.IndexOf(drpProcessingStatus.Items.FindByValue(user_can_allocate_what_document_status_id))
                drpProcessingStatus.SelectedIndex = 1; //indx
                drpAllocationStatus.SelectedIndex = 2;
            }
        }

        protected void btnViewClick(object sender, EventArgs e)
        {
            try
            {
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                loadInstructions();
                checkAllocation();
            }
            catch (Exception ex)
            {
                erl.LogError("btnViewClick", ex.Message);
            }
        }

        protected void drpTeamleaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            sharedUtility.LoadDropDownList(drpTransType, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, new string[] { "instruction_type_id IN (SELECT instruction_type_id FROM instruction_type_allocations WHERE system_user_id" }, new string[] { drpTeamLeaders.SelectedValue + ")" }), "instruction_type", "instruction_type_id");
            //"select '-1' AS instruction_type_id, ' All' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types where instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" & drpTeamLeaders.SelectedValue & "' )"
            //sharedUtility.LoadDropDownList(drpTransType, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "instruction_type", "instruction_type_id");
            loadUsers();
            btnViewClick(null, null);
        }

        protected void loadInstructions()
        {
            string status = daccess.RunStringReturnStringValue1Where("system_users", "system_user_type", "system_tl_1", drpTeamLeaders.SelectedValue);//"Select Top 1 system_user_type From system_users Where system_tl_1 = '" + drpTeamLeaders.SelectedValue + "' ", My.Settings.strDSN
            string user_can_allocate_what_document_status = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", drpTeamLeaders.SelectedValue);

            try
            {

                switch (drpAllocationStatus.SelectedItem.Text.ToLower())
                {
                    case "allocated":
                        if (drpProcessingStatus.SelectedIndex == 0)
                        {
                            if (drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", status, user_can_allocate_what_document_status, "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND status = '" + user_can_allocate_what_document_status + "'  ";
                            }
                            else if (drpTransType.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", status, user_can_allocate_what_document_status, "instruction_type_id", drpTransType.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND status = '" + user_can_allocate_what_document_status + "'  ";
                            }
                            else if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", status, user_can_allocate_what_document_status, "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);



                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND status = '" + user_can_allocate_what_document_status + "'  ";
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", status, user_can_allocate_what_document_status, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND status = '" + user_can_allocate_what_document_status + "'  ";
                            }

                        }
                        else if (drpProcessingStatus.SelectedIndex > 0)
                        {
                            if (drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", "document_status_id ", drpProcessingStatus.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }
                            else if (drpTransType.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                               "allocated_to", "document_status_id ", drpProcessingStatus.SelectedValue, "instruction_type_id", drpTransType.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }
                            else if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", "document_status_id ", drpProcessingStatus.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", "document_status_id ", drpProcessingStatus.SelectedValue, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }
                        }

                        break;
                    case "not allocated":
                        if (drpProcessingStatus.SelectedIndex == 0)
                        {
                            if (drpTransType.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "instruction_type_id", drpTransType.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                            }
                            else if (drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);
                            }
                            else if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                    "instruction_type_id", drpTransType.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", status, user_can_allocate_what_document_status, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND status = '" + user_can_allocate_what_document_status + "'  ";
                            }
                        }
                        else if (drpProcessingStatus.SelectedIndex > 0)
                        {
                            if (drpTransType.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", "instruction_type_id", drpTransType.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";


                            }
                            else if (drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                    "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);
                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";


                            }
                            else if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                            {
                                DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                    "instruction_type_id", drpTransType.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", "document_status_id", drpProcessingStatus.SelectedValue, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + drpTeamLeaders.SelectedValue + "' AND status = 1)";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        break;
                    default:
                        if (drpAllocatedTo.SelectedIndex > 0)
                        {
                            DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                            "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                            sharedUtility.LoadGridView(dgvInstructions, dt);

                        }
                        else if (drpTransType.SelectedIndex > 0)
                        {
                            DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "instruction_type_id", drpTransType.SelectedValue, null, null);
                            sharedUtility.LoadGridView(dgvInstructions, dt);

                        }
                        else if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                        {
                            DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1",
                                "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue);
                            sharedUtility.LoadGridView(dgvInstructions, dt);

                        }
                        else
                        {
                            DataTable dt = genericFunctions.GetDataTableInfo("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", drpTeamLeaders.SelectedValue, status, "1");
                            sharedUtility.LoadGridView(dgvInstructions, dt);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading instructions", ex.Message);
                alert.FireAlerts(this.Page, "Error loading instructions.");
            }

        }

        protected void checkAllocation()
        {
            //Loop through instructions to allocate. Allocate as you move down.

            string user_can_allocate_what_document_status = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", drpTeamLeaders.SelectedValue);

            foreach (GridViewRow row in dgvInstructions.Rows)
            {
                CheckBox chk = row.Cells[0].FindControl("chkSelectInstruction") as CheckBox;
                string instruction_id = row.Cells[11].Text;  //dgvInstructions.Rows(i).Cells(11).Text;

                string document_status = daccess.RunStringReturnStringValue1Where("instructions", "status", "instruction_id", instruction_id);
                string allocated_to = daccess.RunStringReturnStringValue1Where("instructions", "allocated_to", "instruction_id", instruction_id);

                double num;
                int allocate;
                if (!double.TryParse(allocated_to, out num))//might need to delete this
                {
                    allocate = 0;
                }
                else
                {
                    Int32.TryParse(allocated_to, out allocate); //convert string to int
                }
                //checking if user can allocate an instruction
                if (document_status.Equals(user_can_allocate_what_document_status) && allocate.Equals(0))
                {
                    chk.Enabled = true;
                }
                else
                {
                    chk.Enabled = false;
                }
            }
        }

        protected void loadUsers()
        {

            try
            {
                DataTable dt = genericFunctions.GetDataSourceUserGridViewInfoUnion("instructios_allocation_view", "user_team_leader_view", "system_tl_1", drpTeamLeaders.SelectedValue, "system_user_id", "officer", "system_tl_1", "system_user_id", "officer", "system_tl_1");
                sharedUtility.LoadGridView(dgvUsers, dt);
            }
            catch (Exception e)
            {
                erl.LogError("Error loading users.", e.Message);
                alert.FireAlerts(this.Page, "Error loading users.");
                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Error loading users at TL View by user " + Session["UserFullName"].ToString(), "", "0", 0, "Error loading users", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);

            }
        }

        protected void btnAllocateClick(object sender, EventArgs e)
        {
            CheckBox chk;
            List<string> userToAllocateList = new List<string>();

            if (!isUserSelected())
            {
                alert.FireAlerts(this.Page, "Please select atleast a user to allocate instrctions to");
            }

            if (!isInstructionSelected())
            {
                alert.FireAlerts(this.Page, "Please select instruction(s) to allocate to users");
            }

            //Get an array of users to allocate to
            foreach (GridViewRow row in dgvUsers.Rows)
            {
                chk = row.Cells[0].FindControl("chkAllocate") as CheckBox;
                if (chk.Checked)
                {
                    userToAllocateList.Add(row.Cells[0].Text);
                }
            }

            int p = 0;
            //Loop through instructions to allocate. Allocate as you move down.
            foreach (GridViewRow row in dgvInstructions.Rows)
            {
                chk = row.Cells[0].FindControl("chkSelectInstruction") as CheckBox;

                string user_to_alocate_to_id = userToAllocateList[p];
                string instruction_id = row.Cells[11].Text; //dgvInstructions.Rows(j).Cells(11).Text;//why 11

                int num, checker;
                Int32.TryParse(user_to_alocate_to_id, out num);
                if (chk.Checked && num > 0)
                {
                    checker = Convert.ToInt32(Session["UserType"]);
                    switch (checker)
                    {
                        case 10:
                            if (daccess.RunNonQuery1Where("UPDATE", "instructions", new string[] { "allocated_to", "allocated_date", "ftro_allocated_by", "ftro_allocated_date" }, new string[] { user_to_alocate_to_id, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", drpTeamLeaders.SelectedValue, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", instruction_id))
                            {
                                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + user_to_alocate_to_id + " allocated ftro Instruction with the following details: Instruction ID:  " + instruction_id, "", "0", 0, "Insytruction alloacted", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                            }
                            break;
                        case 11:
                            if (daccess.RunNonQuery1Where("UPDATE", "instructions", new string[] { "allocated_to", "allocated_date", "processor_allocated_by", "processor_allocated_date" }, new string[] { user_to_alocate_to_id, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", drpTeamLeaders.SelectedValue, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", instruction_id))
                            {
                                operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + user_to_alocate_to_id + " allocated processor Instruction with the following details: Instruction ID:  " + instruction_id, "", "0", 0, "Instruction allocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                            }
                            break;
                        default: //insert error message
                            break;
                    }

                    p = p + 1;
                }
                alert.FireAlerts(this.Page, "allocation successfull");
                btnViewClick(null, null);
            }
            userToAllocateList.Clear();
        }

        public Boolean isInstructionSelected()
        {
            CheckBox chk;

            foreach (GridViewRow row in dgvInstructions.Rows)
            {
                chk = row.Cells[0].FindControl("chkSelectInstruction") as CheckBox;
                if (chk.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        protected Boolean isUserSelected()
        {
            CheckBox chk;

            foreach (GridViewRow row in dgvUsers.Rows)
            {
                chk = (row.Cells[0].FindControl("chkAllocate") as CheckBox);
                if (chk.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        protected void btnAllocate0_Click(object sender, EventArgs e)
        {
            CheckBox chk;
            string user_to_alocate_to_id;

            if (!isUserSelected())
            {
                alert.FireAlerts(this.Page, "Please select atleast a user to deallocate instructions from.");
                return;
            }

            string user_can_allocate_what_document_status = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", drpTeamLeaders.SelectedValue);

            foreach (GridViewRow row in dgvUsers.Rows)
            {
                chk = (row.Cells[0].FindControl("chkAllocate") as CheckBox);

                if (chk.Checked)
                {
                    user_to_alocate_to_id = row.Cells[0].Text;
                    if (daccess.RunNonQuery2Wheres("Update", "instructions", new string[] { "allocated_to" }, new string[] { "0" }, "allocated_to", user_to_alocate_to_id, "status", user_can_allocate_what_document_status))
                    {
                        operationsLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + user_to_alocate_to_id + " deallocated Instruction", "", "0", 0, "Instruction allocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }
            }

            alert.FireAlerts(this.Page, "Deallocation successfull");
            btnViewClick(null, null);
        }

        protected void checkSelectAll_CheckedAgain()
        {
            CheckBox chk;

            foreach (GridViewRow row in dgvInstructions.Rows)
            {
                chk = (row.Cells[0].FindControl("chkSelectInstruction") as CheckBox);
                if (chk.Enabled)
                {
                    chk.Checked = chkSelectAll.Checked;
                }

            }
        }

    }
}