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

namespace CIMS_V2
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        
        CIMS_Entities _db = new CIMS_Entities();
        GenericDbFunctions genericFunctions = new GenericDbFunctions();
        GetNextInfo getNextInfo = new GetNextInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        ErrorLogging erl = new ErrorLogging();
        OperationsLog operationsLog = new OperationsLog();
        RijndaelEnhanced rienha;
        DAccessInfo daccess = new DAccessInfo();
        Constants constants = new Constants();

        #region Page Load Stuff
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

        private void populateYesNoDRP(DropDownList drp, string yes, string no)
        {
            ListItem on = new ListItem();
            on.Text = yes;
            on.Value = "1";

            ListItem off = new ListItem();
            off.Text = no;
            off.Value = "0";

            drp.Items.Add(on);
            drp.Items.Add(off);
        }
        private void LoadPage()
        {

            //get all active instruction types 
            sharedUtility.LoadDropDownList(drpInstructionTypes, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_ID" }, new string[] { "active" }, new string[] { "1" }), "instruction_type", "instruction_type_id");
            //get all visible user types
            sharedUtility.LoadDropDownList(drpUserTypes, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type", "user_type_id" }, new string[] { "visible" }, new string[] { "1" }), "user_type", "user_type_id");

            populateYesNoDRP(drpActiveStatus, "Active", "Inactive");
            populateYesNoDRP(drpUserActive, "Active", "Inactive");
            populateYesNoDRP(drpSupportingDocumentsEnable, "Enabled", "Disabled");
            populateYesNoDRP(drpCanGetNext, "Yes", "No");
            populateYesNoDRP(drpCanOriginate, "Yes", "No");
            populateYesNoDRP(drpUserTypeVisible, "Yes", "No");

            //default values for dropdowns
            drpInstructionTypes.Items.Insert(0, new ListItem("Please select", string.Empty));
            drpActiveStatus.Items.Insert(0, new ListItem("Please select", string.Empty));
            drpCutOffHour.Items.Insert(0, new ListItem("HH", string.Empty));
            drpCutOffMinute.Items.Insert(0, new ListItem("MM", string.Empty));
            drpSupportingDocumentsEnable.Items.Insert(0, new ListItem("Please select", string.Empty));
            drpCanGetNext.Items.Insert(0, new ListItem("Please select", string.Empty));
            drpCanOriginate.Items.Insert(0, new ListItem("Please select", string.Empty));
            drpUserTypeVisible.Items.Insert(0, new ListItem("Please select", string.Empty));
            //toggle the menu
            MenuManagement("All");
            
        }

        public void MenuManagement(string message)
        {
            btnViewInstructionTypesManagement.Enabled = true;
            btnViewUserTypesManagement.Enabled = true;
            btnViewUserManagement.Enabled = true; 

            switch (message) //controls the menu items alternating visibility
            {
                case "All":
                    btnViewInstructionTypesManagement.Enabled = true;
                    btnViewUserManagement.Enabled = true;
                    btnViewUserTypesManagement.Enabled = true;

                    instructionTypesDiv.Visible = false;
                    userManagementDiv.Visible = false;
                    userTypeManagementDiv.Visible = false;
                    break;

                case "ITM":
                    btnViewInstructionTypesManagement.Enabled = false;
                    break;

                case "UM":
                    btnViewUserManagement.Enabled = false;
                    break;

                case "UTM":
                    btnViewUserTypesManagement.Enabled = false;
                    break;

                case "None":
                    btnViewInstructionTypesManagement.Enabled = false;
                    btnViewUserManagement.Enabled = false;
                    btnViewUserTypesManagement.Enabled = false;
                    break;
            }
        }

        #endregion

        #region MenuStuff
        protected void btnViewInstructionTypesManagement_Click(object sender, EventArgs e)
        {
            userManagementDiv.Visible = false;
            instructionTypesDiv.Visible = true;
            userTypeManagementDiv.Visible = false;



            MenuManagement("ITM");
        }


        protected void btnViewUserTypesManagement_Click(object sender, EventArgs e)
        {

            userManagementDiv.Visible = false;
            instructionTypesDiv.Visible = false;
            userTypeManagementDiv.Visible = true;

            
            populateUserTypeFields(); 

            MenuManagement("UTM");
        }

        protected void btnViewUserManagement_Click(object sender, EventArgs e)
        {
            userManagementDiv.Visible = true;
            instructionTypesDiv.Visible = false;
            userTypeManagementDiv.Visible = false;

            btnViewUserTypesManagement.Enabled = false;

            MenuManagement("UM");

        }

        #endregion

        #region Instruction Type Management

        public void populateInstructionTypeFields()
        {
            if (!string.IsNullOrEmpty(drpInstructionTypes.SelectedValue))
            {
                drpActiveStatus.Visible = true;

                int instructionId = Convert.ToInt32(drpInstructionTypes.SelectedValue);

                instructions_types insType = _db.instructions_types.FirstOrDefault(i => i.instruction_type_ID == instructionId);

                if (insType != null)
                {

                    int activeStatus = (int)insType.active;
                    int supportingDoc = 0;
                    if (insType.allow_supporting_documents != null)
                    {
                        supportingDoc = (int)insType.allow_supporting_documents;
                        drpSupportingDocumentsEnable.SelectedValue = supportingDoc.ToString();
                    }
                    else
                    {
                        drpSupportingDocumentsEnable.SelectedIndex = 0;
                    }
                    if (activeStatus == 1)
                    {
                        drpActiveStatus.SelectedValue = "1";
                    }
                    else if (activeStatus == 0)
                    {
                        drpActiveStatus.SelectedValue = "0";
                    }

                    string hours = insType.cutt_off_time.Substring(0, 2);
                    drpCutOffHour.SelectedValue = hours;
                    string minutes = insType.cutt_off_time.Substring(3, 2);
                    drpCutOffMinute.SelectedValue = minutes;
                    string amPm = "";
                    if (insType.cutt_off_time.Length == 8)
                    {
                        amPm = insType.cutt_off_time.Substring(6, 2);
                    }
                    else if (insType.cutt_off_time.Length == 7)
                    {
                        amPm = insType.cutt_off_time.Substring(5, 2);
                    }

                    drpAmPms.SelectedValue = amPm;
                }
                else
                {
                    alert.FireAlerts(this.Page, "Can't find that instruction type! Please contact your database administrator");

                }
            }

            MenuManagement("ITM");
        }
        protected void drpInstructionTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateInstructionTypeFields();
            
        }
        protected void chkViewInactiveInstructions_CheckedChanged(object sender, EventArgs e)
        {
            if (chkViewInactiveInstructions.Checked == true)
            {
                //load inactive and active instructions
                sharedUtility.LoadDropDownList(drpInstructionTypes, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_ID" }, new string[] { "active" }, new string[] { "0" }), "instruction_type", "instruction_type_id");
            }
            else
            {
                sharedUtility.LoadDropDownList(drpInstructionTypes, genericFunctions.GetDropdownListInfo("instructions_types", new string[] { "instruction_type", "instruction_type_ID" }, new string[] { "active" }, new string[] { "1" }), "instruction_type", "instruction_type_id");
            }

            populateInstructionTypeFields(); 

            MenuManagement("ITM");
        }
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (drpInstructionTypes.SelectedIndex != 0 && drpActiveStatus.SelectedIndex != 0 && drpSupportingDocumentsEnable.SelectedIndex != 0)
                {

                    int id = Convert.ToInt32(drpInstructionTypes.SelectedValue);
                    int active = Convert.ToInt32(drpActiveStatus.SelectedValue);
                    int supporting = Convert.ToInt32(drpSupportingDocumentsEnable.SelectedValue);

                    instructions_types ins = _db.instructions_types.FirstOrDefault(t => t.instruction_type_ID == id);

                    string cutOff = "";

                    if (drpCutOffHour.SelectedIndex != 0 && drpCutOffMinute.SelectedIndex != 0)
                    {

                        string hours = drpCutOffHour.SelectedValue;
                        string minutes = drpCutOffMinute.SelectedValue;
                        string amPm = drpAmPms.SelectedValue;

                        cutOff = hours + ":" + minutes + " " + amPm;
                    }
                    if (ins != null) //if there is an instruction type like that
                    {
                        //update cutoff time
                        if (!string.IsNullOrEmpty(cutOff))
                        {
                            ins.cutt_off_time = cutOff;

                        }

                        //update values

                        ins.active = active;
                        ins.allow_supporting_documents = supporting;

                        _db.SaveChanges();
                    }
                    alert.FireAlerts(this.Page, "Instruction updated with the following info. Name: " + ins.instruction_type + ", Active Status (1 is active): " + ins.active + " and Cutoff: " + ins.cutt_off_time);
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Couldn't update the instruction type " + ex.ToString());
            }
            MenuManagement("ITM");
        }
        #endregion

        #region User Management

        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            string searchTerm = "";
            if (!String.IsNullOrEmpty(txtSearchUser.Text))
            {
                searchTerm = txtSearchUser.Text;
            }
            var users = _db.system_users.ToList().Where(u => u.system_user_login.Contains(searchTerm) || u.system_user_fname.Contains(searchTerm) || u.system_user_lname.Contains(searchTerm));
            if (users != null && users.Count() > 0)
            {
                foreach (system_users u in users)
                {
                    ListItem userListItem = new ListItem(u.system_user_login, u.system_user_id.ToString());
                    drpUserList.Items.Add(userListItem);
                }

                drpUserList.Enabled = true;
                btnSaveUser.Enabled = true;

                drpUserActive.Enabled = true;
                txtUserLogin.Enabled = true;

                int idVal = Convert.ToInt32(drpUserList.SelectedValue);
                var selectedUser = _db.system_users.FirstOrDefault(u => u.system_user_id == idVal);

                txtUserLogin.Text = selectedUser.system_user_login;

                drpUserActive.SelectedValue = selectedUser.system_user_active.ToString();
            }
            else
            {
                alert.FireAlerts(this.Page, "No users found");
            }

            

            MenuManagement("UM");
        }
        protected void drpUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idVal = Convert.ToInt32(drpUserList.SelectedValue);
            var selectedUser = _db.system_users.FirstOrDefault(u => u.system_user_id == idVal);

            txtUserLogin.Text = selectedUser.system_user_login;
            drpUserActive.SelectedValue = selectedUser.system_user_active.ToString();

            MenuManagement("UM");
        }
        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            string message = "";
            try
            {
                int idVal = Convert.ToInt32(drpUserList.SelectedValue);
                system_users user = _db.system_users.FirstOrDefault(u => u.system_user_id == idVal);

                if (user != null)
                {
                    int active = (int)user.system_user_active;
                    int status = (int)user.system_user_status;

                    active = Convert.ToInt32(drpUserActive.SelectedValue);

                    user.system_user_active = active;
                    user.system_user_login = txtUserLogin.Text;

                    _db.SaveChanges();

                    message = "User saved! Login: " + txtUserLogin.Text + ", Active: " + drpUserActive.SelectedItem.Text;
                    alert.FireAlerts(this.Page, message);

                    drpUserList.Items.Clear();

                    string searchTerm = "";
                    if (!String.IsNullOrEmpty(txtSearchUser.Text))
                    {
                        searchTerm = txtSearchUser.Text;
                    }
                    var users = _db.system_users.ToList().Where(u => u.system_user_login.Contains(searchTerm) || u.system_user_fname.Contains(searchTerm) || u.system_user_lname.Contains(searchTerm));
                    if (users != null && users.Count() > 0)
                    {
                        foreach (system_users u in users)
                        {
                            ListItem userListItem = new ListItem(u.system_user_login, u.system_user_id.ToString());
                            drpUserList.Items.Add(userListItem);
                        }

                        var selectedUser = _db.system_users.FirstOrDefault(u => u.system_user_login == txtUserLogin.Text);
                        drpUserList.SelectedItem.Text = selectedUser.system_user_login;
                    }
                    else
                    {
                        alert.FireAlerts(this.Page, "User not found");
                    }
                }
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "Error saving user " + ex.Message);
            }
            MenuManagement("UM");
        }

        private void populateUserManagementFields()
        {


            MenuManagement("UM");
        }

        #endregion

        #region User Type Management

        protected void btnSaveUserType_Click(object sender, EventArgs e)
        {
            int idVal = Convert.ToInt32(drpUserTypes.SelectedValue);
            
            user_type selectedType = _db.user_type.FirstOrDefault(t => t.user_type_id == idVal);

            if(!string.IsNullOrEmpty(txtUserTypeName.Text))
            {
                selectedType.user_type1 = txtUserTypeName.Text;               
            }
            if (drpCanGetNext.SelectedIndex > 0)
            {
                selectedType.can_get_next = Convert.ToInt32(drpCanGetNext.SelectedValue);
            }
            if(drpCanOriginate.SelectedIndex > 0)
            {
                selectedType.can_add_attachment = Convert.ToInt32(drpCanOriginate.SelectedValue);
            }
            if(drpUserTypeVisible.SelectedIndex > 0)
            {
                selectedType.visible = Convert.ToInt32(drpUserTypeVisible.SelectedValue);
            }

            _db.SaveChanges();

            txtUserTypeName.Text = selectedType.user_type1; 

            loadUserTypeListDropDown();
            
            if(selectedType.user_type1 != null)
            {

                drpUserTypes.SelectedItem.Text = selectedType.user_type1;
            }
            if(selectedType.can_add_attachment != null)
            {
                drpCanOriginate.SelectedValue = selectedType.can_add_attachment.ToString();
            }
            if(selectedType.can_get_next != null)
            {
                drpCanGetNext.SelectedValue = selectedType.can_get_next.ToString();
            }
            if (selectedType.visible != null)
            {
                drpUserTypeVisible.SelectedValue = selectedType.visible.ToString();
            }



            MenuManagement("UTM");
        }

        protected void drpUserTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateUserTypeFields(); 

            MenuManagement("UTM");

        }
        private void loadUserTypeListDropDown()
        {
            //view all inactive user types
            if (chkViewInactiveUserTypes.Checked == true)
            {
                sharedUtility.LoadDropDownList(drpUserTypes, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type", "user_type_id" }, new string[] { "visible" }, new string[] { "0" }), "user_type", "user_type_id");
            }
            else
            {
                sharedUtility.LoadDropDownList(drpUserTypes, genericFunctions.GetDropdownListInfo("user_type", new string[] { "user_type", "user_type_id" }, new string[] { "visible" }, new string[] { "1" }), "user_type", "user_type_id");
            }
        }
        protected void chkViewInactiveUserTypes_CheckedChanged(object sender, EventArgs e)
        {
            loadUserTypeListDropDown();
            populateUserTypeFields(); 
            MenuManagement("UTM");
        }

        private void populateUserTypeFields()
        {
            int idVal = Convert.ToInt32(drpUserTypes.SelectedValue);

            user_type selectedType = _db.user_type.FirstOrDefault(t => t.user_type_id == idVal);

            if (selectedType != null)
            {
                drpCanGetNext.Enabled = true;
                drpCanOriginate.Enabled = true;
                drpUserTypeVisible.Enabled = true;
                txtUserTypeName.Enabled = true;
                

                txtUserTypeName.Text = selectedType.user_type1;

                if (selectedType.can_get_next.ToString() != null)
                {
                    drpCanGetNext.SelectedValue = selectedType.can_get_next.ToString();
                }

                if (selectedType.can_add_attachment.ToString() != null)
                {
                    drpCanOriginate.SelectedValue = selectedType.can_add_attachment.ToString();
                }

                if(selectedType.visible.ToString() != null)
                {
                    drpUserTypeVisible.SelectedValue = selectedType.visible.ToString(); 
                }
            }

        }
        #endregion
    }
}


        