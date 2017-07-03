using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CIMS_Datalayer;

namespace CIMS_V2.Account
{
    public partial class ManagePassword : System.Web.UI.Page
    {
        protected string SuccessMessage
        {
            get;
            private set;
        }

        //private bool HasPassword(ApplicationUserManager manager)
        //{
        //    return manager.HasPassword(User.Identity.GetUserId());
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            //var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            if (!IsPostBack)
            {
                //// Determine the sections to render
                //if (HasPassword(manager))
                //{
                    changePasswordHolder.Visible = true;
                //}
                //else
                //{
                //    setPassword.Visible = true;
                //    changePasswordHolder.Visible = false;
                //}

                //// Render success message
                //var message = Request.QueryString["m"];
                //if (message != null)
                //{
                //    // Strip the query string from action
                //    Form.Action = ResolveUrl("~/Account/Manage");
                //}

                //lblUserName.Text = Session["UserNameToChangePassword"].ToString();
               // txtUserName.Text = "Gregory";
            }
        }

        protected void ChangePassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
               
                try
                {
                    string gPassPhrase = "cfcstanbicbank..";
                    string gInitVector = "standardbankgroup";
                    string ip_address = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    RijndaelEnhanced rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);

                    LoginInfo logininfo = new LoginInfo();
                    OperationsLog operationLog = new OperationsLog();

                    var getSystemUserInfo = logininfo.GetSystemUser(txtUserName.Text);

                    string old_pass_from_db = rienha.Decrypt(getSystemUserInfo.system_user_password);

                    if(CurrentPassword.Text == old_pass_from_db)
                    {
                        logininfo.UpdatePassword(txtUserName.Text.Trim(), rienha.Encrypt(NewPassword.Text.Trim()));
                        operationLog.InsertOperationsLog(0, "Password changed successfully by user" + txtUserName.Text.Trim(), "", "0", 0, "Password changed successfully ", ip_address, 0);
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "msg", "alert('Password updated successfully.You will be required to log in with the new password.');", true);
                        Response.Redirect("Account/Login.aspx");
                    }    
                }
                catch(Exception ex)
                {
                    ErrorLogging erl = new ErrorLogging();
                    erl.LogError("ChangePassword_Click", ex.Message);
                    throw ex;
                }


            }
        }


        protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (args.IsValid)
            {
                if (NewPassword.Text != CurrentPassword.Text)
                {
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = false;
                }
            }
            else
            {
                args.IsValid = false;
            }
        }

        //protected void SetPassword_Click(object sender, EventArgs e)
        //{
        //    if (IsValid)
        //    {
        //        // Create the local login info and link the local account to the user
        //        var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //        IdentityResult result = manager.AddPassword(User.Identity.GetUserId(), password.Text);
        //        if (result.Succeeded)
        //        {
        //            Response.Redirect("~/Account/Manage?m=SetPwdSuccess");
        //        }
        //        else
        //        {
        //            AddErrors(result);
        //        }
        //    }
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}