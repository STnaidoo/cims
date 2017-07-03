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

namespace CIMS_V2.Unknown_Views
{
    public partial class TL_View : System.Web.UI.Page
    {
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationLog = new OperationsLog();
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
            sharedUtility.LoadDropDownList(
                        drpSearchBy,
                        genericFunctions.GetDropdownListInfo("search_by", new string[] { "search_by_name", "search_by_value" }, new string[] { "search_by_module" }, new string[] { "instructions" }),
                        "search_by_name",
                        "search_by_value");

            //sharedUtility.LoadDropDownList(drpSearchBy, genericFunctions.GetSearchByDropDownListInfo("instructions"), "search_by_name", "search_by_value");
            //"select search_by_value, search_by_name from search_by  where search_by_module = 'instructions' ORDER BY search_by_name "

            //load document status
            sharedUtility.LoadDropDownList(
                drpProcessingStatus, genericFunctions.GetDropdownListInfo("document_status", new string[] { "document_status_id", "document_status" }, null, null),
                "document_status",
                "document_status_id");
            //sharedUtility.LoadDropDownList(drpProcessingStatus, genericFunctions.GetDocumentStatusDropDownListInfo(Convert.ToInt32(Session["UserID"])), "document_status", "document_status_id");
            //"select '-1' AS document_status_id, ' All' AS document_status from document_status UNION select '0' AS document_status_id, ' All I Can Allocate' AS document_status from instructions_types UNION select document_status_id, document_status from document_status  order by document_status"

            //???
            //users
            sharedUtility.LoadDropDownList(
                drpAllocatedTo,
                genericFunctions.GetDropdownListInfo("system_users_view", new string[] { "system_user_id", "names" }, null, null),
                "names",
                "system_user_id");
            //sharedUtility.LoadDropDownList(drpAllocatedTo, genericFunctions.GetSearchByDropDownListInfo("users"), "names", "system_user_id");
            //"select '0' AS system_user_id, ' ' AS names from system_users_view UNION select system_user_id, names from system_users_view "

            //load instructions
            sharedUtility.LoadDropDownList(
                drpTransType,
                genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type_id", "instruction_type" }, null, null),
                "instruction_type",
                "instruction_type_id");
            //sharedUtility.LoadDropDownList(drpTransType, genericFunctions.GetInstructionsTypesDropDownListInfo(Convert.ToInt32(Session["UserID"])), "Instruction_type", "instruction_type_id");
            //"select '0' AS instruction_type_id, ' Select Instruction Type' AS instruction_type from instructions_types UNION select instruction_type_id, instruction_type from instructions_types "

            LoadUsers();
            GetUserProcessingStatus();
            drpProcessingStatus.SelectedIndex = 1;
            drpAllocationStatus.SelectedIndex = 2;
            chkAllocateToMany.Checked = true;
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                dgvInstructions.DataSource = null;
                dgvInstructions.DataBind();

                LoadInstructions();
                CheckWhatCanBeAllocated();
                CheckIfAllocatedAndToggleDeallocationButton();
                GetUserProcessingStatus();
            }
            catch
            {
                //??? log error?
            }
        }

        public void LoadInstructions()
        {
            string userCanAllocateWhatDocumentStatus = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", Session["UserID"].ToString());
            string status = daccess.RunStringReturnStringValue1Where("system_users", "system_user_type", "system_tl_1", Session["UserType"].ToString());


            try
            {
                //??? sharedUtility - dgvInstructions

                //allocation status
                switch (drpAllocationStatus.SelectedItem.Text.ToLower())
                {
                    case "allocated":
                        if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                        DataTable dt = genericFunctions.GetDataTableDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                        status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", drpProcessingStatus.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue);
                                        sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //    sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";

                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue, status, userCanAllocateWhatDocumentStatus);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";

                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "instruction_type_id", drpTransType.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                //addSql = addSql + " AND allocated_to > 0";

                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        else if (drpAllocatedTo.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", drpProcessingStatus.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";

                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, status, userCanAllocateWhatDocumentStatus, null , null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";

                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                if (drpProcessingStatus.SelectedValue == "0")
                                {

                                    if (Session["UserType"].ToString() == "4")
                                    {
                                        DataTable dt = genericFunctions.GetDataTableDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                        status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", "document_status_id", drpProcessingStatus.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue);
                                        sharedUtility.LoadGridView(dgvInstructions, dt);

                                        //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                        //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                        //addSql = addSql + " AND allocated_to > 0";

                                        //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                                        //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                    }
                                    else
                                    {
                                        DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                        "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, status, userCanAllocateWhatDocumentStatus);
                                        sharedUtility.LoadGridView(dgvInstructions, dt);



                                        //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                        //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                        //addSql = addSql + " AND allocated_to > 0";

                                        //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                                        //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                    }
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";

                                    //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                                }

                            }
                        }
                        else if (drpTransType.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", "instruction_type_id", drpTransType.SelectedValue, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", "instruction_type_id", drpTransType.SelectedValue, status, userCanAllocateWhatDocumentStatus, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to > 0";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "instruction_type_id", drpTransType.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        else
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                        status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", null, null, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to > 0";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", status, userCanAllocateWhatDocumentStatus, null, null, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to > 0";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoAnd("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "document_status_id ", drpProcessingStatus.SelectedValue, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to > 0";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        break;

                    case "not allocated":
                        if (drpTransType.SelectedIndex > 0 && drpAllocatedTo.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", "instruction_type_id ", drpTransType.SelectedValue, "allocated_to", drpAllocatedTo.SelectedValue);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);



                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                     "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, status, userCanAllocateWhatDocumentStatus, "instruction_type_id", drpTransType.SelectedValue);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);



                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {

                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, "instruction_type_id", drpTransType.SelectedValue);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";
                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }



                        }
                        else if (drpAllocatedTo.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", "allocated_to", drpAllocatedTo.SelectedValue, status, userCanAllocateWhatDocumentStatus, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "allocated_to", drpTransType.SelectedValue, "document_status_id", drpProcessingStatus.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND allocated_to = '" + drpAllocatedTo.SelectedValue + "'  ";
                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        else if (drpTransType.SelectedIndex > 0)
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", "instruction_type_id ", drpTransType.SelectedValue, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + drpAllocatedTo.SelectedValue.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";
                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", "instruction_type_id", drpTransType.SelectedValue, status, userCanAllocateWhatDocumentStatus, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);


                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";
                                    //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "document_status_id", drpProcessingStatus.SelectedValue, "instruction_type_id", drpTransType.SelectedValue, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";
                                //addSql = addSql + " AND instruction_type_id = '" + drpTransType.SelectedValue + "'  ";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }

                        }
                        else
                        {
                            if (drpProcessingStatus.SelectedValue == "0")
                            {

                                if (Session["UserType"].ToString() == "4")
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoDouble("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by", "allocated_to", null, null, null, null );
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                                }
                                else
                                {
                                    DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                    "allocated_to", status, userCanAllocateWhatDocumentStatus, null, null, null, null);
                                    sharedUtility.LoadGridView(dgvInstructions, dt);

                                    //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                    //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                    //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                                }
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableInfoWithOr("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "allocated_to", "document_status_id", drpProcessingStatus.SelectedValue, null, null, null, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);

                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND (allocated_to = 0 OR allocated_to IS NULL) ";
                                //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                            }
                        }
                        break;
                    default:
                        if (drpProcessingStatus.SelectedValue == "0")
                        {

                            if (Session["UserType"].ToString() == "4")
                            {
                                DataTable dt = genericFunctions.GetDataTableDefault("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                 status, userCanAllocateWhatDocumentStatus, "branch_proccessed_by");
                                sharedUtility.LoadGridView(dgvInstructions, dt);


                                //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";

                                //addSql = addSql + " AND (status = '" + userCanAllocateWhatDocumentStatus + "' AND branch_proccessed_by > 0) ";
                            }
                            else
                            {
                                DataTable dt = genericFunctions.GetDataTableDefault("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                 status, userCanAllocateWhatDocumentStatus, null);
                                sharedUtility.LoadGridView(dgvInstructions, dt);
                                
                                
                                //sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";
                                //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";
                                //addSql = addSql + " AND status = '" + userCanAllocateWhatDocumentStatus + "'  ";
                            }
                        }
                        else
                        {
                            DataTable dt = genericFunctions.GetDataTableDefault("instructions_view", "instruction_type_allocations", drpSearchBy.SelectedValue, "system_user_id", txtSearch.Text, "instruction_type_id", Session["UserID"].ToString(), status, "1",
                                "document_status_id", drpProcessingStatus.SelectedValue, null);
                            sharedUtility.LoadGridView(dgvInstructions, dt);

                            //string sql = "Select * From instructions_view Where " + drpSearchBy.SelectedValue + " LIKE '%" + txtSearch.Text + "%'  AND instruction_type_id IN ( select instruction_type_id from instruction_type_allocations where system_user_id =  '" + Session["UserID"].ToString() + "' AND status = 1)";


                            //addSql = addSql + " AND document_status_id = '" + drpProcessingStatus.SelectedValue + "'  ";
                        }
                        break;
                }




            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error loading instructions");
                erl.LogError("Error loading instructions", ex.Message);
            }
        }

        public void CheckWhatCanBeAllocated()
        {
            string userCanAllocateWhatDocumentStatus = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", Session["UserID"].ToString());

            for (int z = 0; z <= dgvInstructions.Rows.Count - 1; z++)
            {
                CheckBox chk = dgvInstructions.Rows[z].FindControl("chkSelectInstruction") as CheckBox;
                string instructionID = dgvInstructions.Rows[z].Cells[11].Text;
                string documentStatus = daccess.RunStringReturnStringValue1Where("Instructions", "status", "instruction_id", instructionID);
                string allocatedTo = daccess.RunStringReturnStringValue1Where("instructions", "allocated_to", "instruction_id", instructionID);

                double result;
                if (double.TryParse(allocatedTo, out result))
                {
                    allocatedTo = "0";
                }

                //can user allocate an instruction
                if (documentStatus == userCanAllocateWhatDocumentStatus && allocatedTo == "0")
                {
                    chk.Enabled = true;
                }
                else
                {
                    chk.Enabled = false;
                }
            }
        }

        public void CheckIfAllocatedAndToggleDeallocationButton()
        {
            for (int z = 0; z <= dgvInstructions.Rows.Count - 1; z++)
            {
                CheckBox chk = dgvInstructions.Rows[z].FindControl("chkDeallocate") as CheckBox;
                string instructionID = dgvInstructions.Rows[z].Cells[11].Text;
                string allocatedTo = daccess.RunStringReturnStringValue1Where("instructions", "allocated_to", "instruction_id", instructionID);

                double result;
                if (double.TryParse(allocatedTo, out result))
                {
                    allocatedTo = "0";
                }

                //user can allocate an instruction
                if (Convert.ToDouble(allocatedTo) > 0)
                {
                    chk.Enabled = true;
                }
                else
                {
                    chk.Enabled = false;
                }
            }
        }

        public void LoadUsers()
        {
            //??? replace sql
            string sql = "Select system_user_id, officer, system_tl_1, '' As total_pending, '' As total_processed, '' As total From instructios_allocation_view Where system_tl_1 = '" + Session["UserID"].ToString() + "' UNION select system_user_id, officer, system_tl_1, '' As total_pending, '' As total_processed, '' As total from user_team_leader_view where system_tl_1 = '" + Session["UserID"].ToString() + "' ";

            try
            {
                //??? sharedUtility - no getDataSet method - causing other errors in method
                //DataSet myDataSet = sharedUtility.getDataSet(sql, My.Settings.strDSN);

                //if (myDataset != null)
                //{
                //    dgvUsers.DataSource = myDataset.Tables[0].DefaultView;
                //    dgvUsers.DataBind();
                //}
            }
            catch (Exception ex)
            {
                erl.LogError("Error loading users", ex.Message);
            }
        }

        public void GetUserProcessingStatus()
        {
            for (int i = 0; i <= dgvUsers.Rows.Count - 1; i++)
            {
                int total = 0;
                int totalProcessed = 0;
                int totalPending = 0;
                string userID = dgvUsers.Rows[i].Cells[0].Text;

                totalPending = Convert.ToInt32(daccess.RunStringReturnStringValue1Where("instructions", "ISNULL(Count(*), 0)", "allocated_to", userID));
                totalProcessed = Convert.ToInt32(daccess.RunStringReturnStringValue1Where("instructions", "ISNULL(Count(*), 0)", "1", "1"));
                total = totalPending + totalProcessed;

                dgvUsers.Rows[i].Cells[3].Text = totalPending.ToString("N0");
                dgvUsers.Rows[i].Cells[4].Text = totalProcessed.ToString("N0");
                dgvUsers.Rows[i].Cells[5].Text = total.ToString("N0");
            }
        }

        protected void btnAllocate_Click(object sender, EventArgs e)
        {
            if (!CheckIfAUserIsSelected())
            {
                alert.FireAlerts(this.Page, "Please select a user to allocate instructions to");
                return;
            }

            if (!chkAllocateToMany.Checked)
            {
                if (HowManyToAllocateTo() != 1)
                {
                    alert.FireAlerts(this.Page, "Please note that you can only allocate to one user at a time. To allocate to many please check the checkbox - Allocate To Many");
                    return;
                }
            }

            if (!CheckIfInstructionIsSelected())
            {
                alert.FireAlerts(this.Page, "Please select instruction(s) to allocate to users");
                return;
            }

            //get an array of users to allocate to
            List<string> users = new List<string>();

            for (int i = 0; i <= dgvUsers.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvUsers.Rows[i].FindControl("chkAllocate") as CheckBox;
                if (chk.Checked)
                {
                    users.Add(dgvUsers.Rows[i].Cells[0].Text);
                }
            }

            int userToAllocateToPosition = 0;

            for (int z = 0; z <= dgvInstructions.Rows.Count - 1; z++)
            {
                CheckBox chk = dgvInstructions.Rows[z].FindControl("chkSelectInstruction") as CheckBox;
                string userToAllocateToID = users[userToAllocateToPosition];
                string instructionID = dgvInstructions.Rows[z].Cells[11].Text;

                double result;
                if (chk.Checked && double.TryParse(userToAllocateToID, out result) && result > 0)
                {
                    switch (Session["UserType"].ToString())
                    {
                        case "4":
                            if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "allocated_to", "allocated_date", "ftro_allocated_by", "ftro_allocated-date" }, new string[] { userToAllocateToID, "'"+DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")+"'", Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", instructionID))
                            {
                                operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + userToAllocateToID + " allocated ftro Instruction with the following details: Instruction ID:  " + instructionID, "", "0", 0, "Instructions allocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                            }
                            break;
                        case "6":
                            if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "allocated_to", "allocated_date", "processor_allocated_by", "processor_allocated-date" }, new string[] { userToAllocateToID, "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'", Session["UserID"].ToString(), "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" }, "instruction_id", instructionID))
                            {
                                operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + userToAllocateToID + " allocated processor Instruction with the following details: Instruction ID:  " + instructionID, "", "0", 0, "Instructions allocated", HttpContext.Current.Request.ServerVariables["REMOTES_ADDR"], 0);
                            }
                            break;
                        default:
                            break;
                    }
                }

                userToAllocateToPosition = userToAllocateToPosition + 1;

                if (userToAllocateToPosition > users.Count() - 1)
                {
                    userToAllocateToPosition = 0;
                }
            }

            alert.FireAlerts(this.Page, "Allocation successful");
            btnView_Click(null, null);
        }

        public int HowManyToAllocateTo()
        {
            int x = 0;

            for (int i = 0; i <= dgvUsers.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvUsers.Rows[i].FindControl("chkAllocate") as CheckBox;
                if (chk.Checked)
                {
                    x++;
                }
            }

            return x;
        }

        public Boolean CheckIfInstructionIsSelected()
        {
            for (int i = 0; i <= dgvInstructions.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvInstructions.Rows[i].FindControl("chkSelectInstruction") as CheckBox;
                if (chk.Checked)
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean CheckIfInstructionIsDeselected()
        {
            for (int i = 0; i <= dgvInstructions.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvInstructions.Rows[i].FindControl("chkDeallocate") as CheckBox;
                if (chk.Checked)
                {
                    return true;
                }
            }

            return false;
        }

        public Boolean CheckIfAUserIsSelected()
        {
            for (int i = 0; i <= dgvUsers.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvUsers.Rows[i].FindControl("chkAllocate") as CheckBox;
                if (chk.Checked)
                {
                    return true;
                }
            }

            return false;
        }

        protected void btnAllocate0_Click(object sender, EventArgs e)
        {
            if (!CheckIfAUserIsSelected())
            {
                alert.FireAlerts(this.Page, "Please select at least one user to deallocate instructions from");
                return;
            }

            string userCanAllocateWhatDocumentStatus = daccess.RunStringReturnStringValue1Where("system_users", "user_can_allocate_what_document_status", "system_user_id", Session["UserID"].ToString());

            for (int i = 0; i <= dgvUsers.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvUsers.Rows[i].FindControl("chkAllocate") as CheckBox;
                if (chk.Checked)
                {
                    string userToAllocateToID = dgvUsers.Rows[i].Cells[0].Text;
                    if (daccess.RunNonQuery2Wheres("Update", "instructions", new string[] { "allocated_to" }, new string[] { "0" }, "allocated_to", userToAllocateToID, "status", userCanAllocateWhatDocumentStatus))
                    {
                        operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "User " + userToAllocateToID + " deallocated Instruction", "", "0", 0, "Instruction allocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }
            }

            alert.FireAlerts(this.Page, "Deallocation successful");
            btnView_Click(null, null);
        }

        protected void chkSelectAll_CheckedChange(object sender, EventArgs e)
        {
            for (int i = 0; i <= dgvInstructions.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvInstructions.Rows[i].FindControl("chkSelectInstructions") as CheckBox;
                if (chk.Enabled)
                {
                    chk.Checked = chkSelectAll.Checked;
                }
            }
        }

        protected void btnDeallocateSelected_Click(object sender, EventArgs e)
        {
            if (!CheckIfInstructionIsDeselected())
            {
                alert.FireAlerts(this.Page, "Please select instruction(s) to deallocate to users");
                return;
            }

            for (int z = 0; z <= dgvInstructions.Rows.Count - 1; z++)
            {
                CheckBox chk = dgvInstructions.Rows[z].FindControl("chkDeallocate") as CheckBox;
                string instructionID = dgvInstructions.Rows[z].Cells[11].Text;

                if (chk.Checked)
                {
                    if (daccess.RunNonQuery1Where("Update", "instructions", new string[] { "allocated_to" }, new string[] { "0" }, "instruction_id", instructionID))
                    {
                        operationLog.InsertOperationsLog(Convert.ToInt32(Session["UserID"]), "Instruction " + instructionID + " deallocated Instruction", "", "0", 0, "Instruction deallocated", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 0);
                    }
                }
            }

            btnView_Click(null, null);
        }

        protected void chkSelectToDeallocate_CheckedChange(object sender, EventArgs e)
        {
            for (int i = 0; i <= dgvInstructions.Rows.Count - 1; i++)
            {
                CheckBox chk = dgvInstructions.Rows[i].FindControl("chkDeallocate") as CheckBox;
                if (chk.Enabled)
                {
                    chk.Checked = chkSelectToDeallocate.Checked;
                }
            }
        }

        protected void btnAllocate0_Click1(object sender, EventArgs e)
        {

        }
    }
}