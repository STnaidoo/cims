using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using CIMS_V2.Models;
using System.Linq;
using CIMS_V2.AddOn;
using CIMS_Datalayer;

namespace CIMS_V2.Account
{
    public partial class Forgot : Page
    {
        DAccessInfo daccess = new DAccessInfo();
        Alerts alert = new Alerts();
        SharedFunctions sharedUtility = new SharedFunctions();
        protected void Page_Load(object sender, EventArgs e)
        {
     
        }

        protected void ForgotPassword(object sender, EventArgs e)
        {
            if (IsValid)
            {

                CIMS_Entities _db = new CIMS_Entities();
                try
                {
                    //get the user with that user name address.
                    system_users user = _db.system_users.FirstOrDefault(u => u.system_user_login == Email.Text);

                    if (user == null)
                    {
                        alert.FireAlerts(this.Page, "A user with that username does not exist");
                        return;
                    }
                    string encryptedPassword = user.system_user_password;
                    string gPassPhrase = "cfcstanbicbank..";
                    string gInitVector = "standardbankgroup";

                    RijndaelEnhanced rienha = new RijndaelEnhanced(gPassPhrase, gInitVector);

                    string decryptedPassword = rienha.Decrypt(encryptedPassword);
                    string emailBody = "Your password for CIMS is: " + decryptedPassword + ", please use this password to login.";


                    user.system_user_status = 1;
                    _db.SaveChanges();

                    alert.FireAlerts(this.Page, "Please check your email for your password.");
                    DisplayEmail.Visible = true;

                    sharedUtility.SendMail(user.system_user_email, "CIMS Password", emailBody);

                }
                catch (Exception ex)
                {
                    alert.FireAlerts(this.Page, "Couldn't decrypt password for that user" + ex.Message);
                }
            }
        }

    }
}