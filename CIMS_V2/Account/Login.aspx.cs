using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using CIMS_V2.Models;
using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System.Configuration;
using System.Web.Security;
using System.Data.SqlClient;
using System.Linq;


namespace CIMS_V2.Account
{
    public partial class Login : Page
        
    {
        DAccessInfo daccess = new DAccessInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();

        protected void Page_Load(object sender, EventArgs e)
        {

            

            //  RegisterHyperLink.NavigateUrl = "Register";
            // Enable this once you have account confirmation enabled for password reset functionality
            //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                // RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Session.Abandon();
                string status = InitiateLogin(txtUserName.Text, txtPassword.Text);

                // Time to Check Duplicates 
                LoginInfo loginifo = new LoginInfo();
                Session["duplicate_check_days"] = loginifo.GetDuplicateDays("duplicate_check_days");

                if (status == "")
                {

                    CIMS_Datalayer.Constants c = new CIMS_Datalayer.Constants();
                    using (SqlConnection con = new SqlConnection(c.getConnectionString()))
                    //you must use a using to open and close the connection or else there will be 
                    //too many connections open and you will get connection leaks and the application will become slow and crash
                    //thus make sure you close connections!!!
                    {
                        SqlCommand cmd = new SqlCommand("proc_LastLogin");
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@whereClause", "system_user_id");
                        cmd.Parameters.AddWithValue("@whereValue", Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@lastlogin", "'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'");

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                


                    if (Session["CanGetNext"].ToString() == "1")
                    {
                        Session["current_page"] = "getnext.aspx";
                        // Server.Transfer("~/GetNext.aspx");
                        Response.Redirect("~/GetNext.aspx");
                        //Server.Transfer("~/getnext.aspx",false);
                    }
                    else
                    {
                        Session["current_page"] = "instruction_view.aspx";
                        //Server.Transfer("~/instruction_view.aspx", false);
                        Response.Redirect("~/InstructionView.aspx");
                    }
                }
                else
                {
                    lblError.Text = "Error Logging in. Please contact administrator for assistance.";
                }


            }
        }

        private string InitiateLogin(string userName, string password)
        {
            LoginInfo logininfo = new LoginInfo();
            OperationsLog operationLog = new OperationsLog();

            var getSystemUserInfo = logininfo.GetSystemUser(txtUserName.Text);
            string status;
            string ip_address = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (getSystemUserInfo != null)
            {

                string gPassPhrase = "cfcstanbicbank..";
                string gInitVector = "standardbankgroup";

                RijndaelEnhanced rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);

                string check_sum = rienha.Decrypt(getSystemUserInfo.system_user_password);

                //lmao
                string check_sum_bro = getSystemUserInfo.system_user_login + getSystemUserInfo.system_user_type + getSystemUserInfo.system_user_active;

                int? state = getSystemUserInfo.system_user_active;
                if (state == 0)
                {
                    getSystemUserInfo.system_user_active = 1;
                    state = 1;
                }
                string db_password = getSystemUserInfo.system_user_password;
                int? change_password = getSystemUserInfo.change_password;

                //string newpassword = rienha.Encrypt("Gregory");

                string db_password_dec = rienha.Decrypt(getSystemUserInfo.system_user_password);
                int? system_user_status = getSystemUserInfo.system_user_status;
                int? failed_login_count = getSystemUserInfo.failed_login_count;
                string login_option = getSystemUserInfo.login_option;

                int? no_of_instructions_i_can_pack = getSystemUserInfo.no_of_instructions_i_can_pack;


                //dont know why this is here
                DateTime? change_password_date = DateTime.Now.AddDays(-20);
                change_password_date = getSystemUserInfo.change_password_date;

                //Inactive or otherwise
                if (state != 1)
                {
                    status = "User is inactive";
                    //reactivate user
                    //get user ID
                    //bool reActivated = false;
                    //reActivated = daccess.RunNonQuery1Where("Update", "system_users", new string[] { "system_user_active", "system_user_status" }, new string[] { "1", "1" }, "system_user_login", getSystemUserInfo.system_user_login);
                    operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Error logging in. User is not active.", "", "0", 0, "Error logging in ", ip_address, 0);
                }
                else
                {
                    if (!logininfo.Check_Password(userName, db_password_dec, password, login_option))
                    {
                        status = "Wrong password";
                        logininfo.UpdateFailedLoginCounter((int)getSystemUserInfo.system_user_id, failed_login_count);
                        operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Error logging in. Wrong password.", "", "0", 0, "Error logging in ", ip_address, 0);
                    }
                    else
                    {
                        if (system_user_status != 1)
                        {
                            status = "Error in user status";
                            operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Error logging in. Error in user status.", "", "0", 0, "Error logging in ", ip_address, 0);
                        }
                        else
                        {
                            //if (check_sum != check_sum_bro && getSystemUserInfo.ignore_checksum != 1)
                            //{
                            //    status = "Error in check sum";
                            //    operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Error logging in. Error in checksum.", "", "0", 0, "Error logging in ", ip_address, 0);
                            //}
                            //else
                            //{
                            if (change_password != 0)
                            {
                                status = "Change password";
                                Session["UserNameToChangePassword"] = userName;
                                //Dev ME
                                Response.Redirect("change_password.aspx"); // Need to do this page 
                            }
                            else
                            {
                                if (failed_login_count > Convert.ToInt32(ConfigurationManager.AppSettings["max_failed_login_count"]))
                                {
                                    //what really needs to be done here
                                    status = "User is Locked.";
                                    operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Error logging in. User is Locked.", "", "0", 0, "Error logging in ", ip_address, 0);
                                    status = "Change password";
                                }
                                else
                                {
                                    var getUserTypes = logininfo.UserTypeDetails(getSystemUserInfo.system_user_type.ToString());

                                    //set variables
                                    Session["CanChangePriority"] = getUserTypes.can_change_priority;
                                    Session["UserID"] = getSystemUserInfo.system_user_id;
                                    Session["UserLoginX"] = getSystemUserInfo.system_user_login;
                                    Session["UserFullName"] = getSystemUserInfo.system_user_fname + " " + getSystemUserInfo.system_user_lname;
                                    Session["LoggedIn"] = "True";
                                    Session["UserType"] = getSystemUserInfo.system_user_type;
                                    Session["UserTypeName"] = getUserTypes.user_type1;
                                    if (getSystemUserInfo.last_successful_login == null)
                                    {
                                        Session["LastLogin"] = "First Successful Login";

                                    }
                                    else

                                    {
                                        Session["LastLogin"] = "Last Successful Login: " + getSystemUserInfo.last_successful_login.ToString();
                                    }


                                    //Uganda changes
                                    Session["CanPerformCallBack"] = getUserTypes.can_perform_callback;
                                    Session["CanPerformDOC"] = getUserTypes.can_perform_doc;
                                    Session["CheckLimit"] = getUserTypes.check_limit;
                                    Session["CanGetNext"] = getUserTypes.can_get_next;
                                    Session["can_add_attachment"] = getUserTypes.can_add_attachment;
                                    Session["can_view_return_report"] = getUserTypes.can_view_return_report;

                                    Session["UserTypePopupAlertTime"] = getUserTypes.popup_alert_time;
                                    Session["UserTittle"] = logininfo.GetUserTitle(getSystemUserInfo.system_user_tittle);
                                    Session["UserBranch"] = logininfo.GetUserBranch(getSystemUserInfo.system_user_branch);
                                    Session["UserBranchID"] = getSystemUserInfo.system_user_branch;
                                    Session["UserInitials"] = getSystemUserInfo.system_user_initials;
                                    Session["UserBranchCode"] = getSystemUserInfo.system_user_branch_code;
                                    Session["IsTeamLeader"] = getSystemUserInfo.is_team_leader;
                                    Session["SystemHeaderView"] = getSystemUserInfo.system_header_view;
                                    Session["no_of_instructions_i_can_pack"] = getSystemUserInfo.no_of_instructions_i_can_pack;
                                    Session["UserCanUnpack"] = getSystemUserInfo.can_unpack_and_allocate;
                                    Session["CanModifyUserAllocations"] = getSystemUserInfo.can_modify_user_allocations;

                                    //can_insert_core_banking_reference
                                    Session["CanInsertCoreBankingReference"] = getUserTypes.can_insert_core_banking_reference;

                                    logininfo.UpdateFailedLoginCounter((int)getSystemUserInfo.system_user_id, null);

                                    //user is logged in.
                                    operationLog.InsertOperationsLog((int)getSystemUserInfo.system_user_id, "Login successful", "", "0", 0, "Login successful", ip_address, 0);

                                    status = "";
                                }
                            }
                            
                        }
                    } 
                }
            }
            else
            {
                status = "User does not exist";
                operationLog.InsertOperationsLog(0, "Login successful", "", "0", 0, "User " + userName + " does not exist", ip_address, 0);
            }

            return status;
        }

        protected void forgotpasswordLink_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/Forgot.aspx");
        }
    }
}