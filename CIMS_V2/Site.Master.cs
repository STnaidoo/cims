using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using CIMS_Datalayer;
using System.Data.SqlClient;


namespace CIMS_V2
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        ErrorLogging erl = new ErrorLogging();

        protected void Page_Init(object sender, EventArgs e)
        {
            // The code below helps to protect against XSRF attacks
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Use the Anti-XSRF token from the cookie
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Generate a new Anti-XSRF token and save to the cookie
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                {
                    responseCookie.Secure = true;
                }
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += master_Page_PreLoad;
        }

        protected void master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? string.Empty;
            }
            else
            {
                // Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? string.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hideAllNavButtons();
                setUserTypeNavLinks(Session["UserType"].ToString());
                setActiveNavBar();
                setUserLoggedInName();
                //lbldatetime.Text = Session["LastLogin"].ToString();
            }
        }

        //sets logged in name
        protected void setUserLoggedInName()
        {
            if (!string.IsNullOrEmpty(Session["UserFullName"].ToString()))
              {
                loggedInUserID.InnerText = Session["UserFullName"].ToString();
               }
        }

        //set active links based on user profile settings from DB
        protected void setUserTypeNavLinks(string userTypeVal)
        {
            //tdLinksRates.Visible = true;
            int select = Int32.Parse(userTypeVal);
            switch (select)
            {
                case 1: //Originatior/Customer Care Consultant
                    tdLinksReports.Visible = true;
                    tdTransactions.Visible = true;
                    tdInstructionsView.Visible = true;

                    break;

                case 2: //Originator Team Lead
                    tdLinksReports.Visible = true;
                    tdTransactions.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;

                case 3: //PRMU Officer
                    tdLinksReports.Visible = true;
                    tdTransactions.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;

                //Different user types but view similar screens
                case 4: //Maker
                    tdLinksReports.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;
                case 5: //IBC Operator
                case 6:
                case 7:
                case 13:
                case 14:
                case 15:
                case 17:
                case 22: //Verifier
                    tdLinksReports.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;
                case 23:
                case 24:
                case 25:
                    tdLinksReports.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;

                case 8: //System Admin Account

                    tdLinksUsers.Visible = true;
                    tdLinksInstructionTypes.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdLinksReports.Visible = true;
                    tdLinkActionSetup.Visible = true;
                    tdLinksAdminPanel.Visible = true;
                    tdLinksAccountOpening.Visible = true;
                    //tdLinkAccountManagement.Visible = true;
                    break;

                case 9:
                    tdInstructionsView.Visible = true;
                    tdLinksReports.Visible = false;
                    tdGetNext.Visible = false;
                    break;

                case 10: //Management View
                    //tdLinksReports.Visible = true;
                    //tdManagerView.Visible = true;
                    //tdInstructionsView.Visible = true;
                    tdLinksReports.Visible = true;
                    tdTransactions.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;

                case 11:
                    tdLinksReports.Visible = true;
                    //tdManagerView.Visible = true;
                    tdInstructionsView.Visible = true;
                    tdGetNext.Visible = true;
                    break;

                default:
                    tdInstructionsView.Visible = true;
                    tdLinksReports.Visible = true;
                    break;
            }

            //set additional menu items
            string canViewReturnReport = "", canGetNext = "",
                canModifyUserAllocations = "";
            
            if (Session["can_view_return_report"] !=null)
            {
                canViewReturnReport = Session["can_view_return_report"].ToString();
            }

            if (Session["CanGetNext"] !=null)
            {
                canGetNext = Session["CanGetNext"].ToString();
            }

            if (Session["CanModifyUserAllocations"] != null)
            {
                canModifyUserAllocations = Session["CanModifyUserAllocations"].ToString();
            }
            

            if (canModifyUserAllocations.Equals("1"))
            {
                tdLinksInstructionTypes.Visible = true;
            }

            if (canViewReturnReport.Equals("1"))
            {
                tdLinkReturnReports.Visible = true;
            }

            if (canGetNext.Equals("1"))
            {
                tdGetNext.Visible = true;
            }
            
        }

        //set the active nav bar link
        protected void setActiveNavBar()
        {
            string currentPage = Session["current_page"].ToString();

            switch (currentPage)
            {

                case "instructions.aspx":
                    tdInstructionsView.Attributes["class"] = "active";
                    break;
                case "reports.aspx":
                    tdLinksReports.Attributes["class"] = "active";
                    break;
                case "user_admin.aspx":
                    tdLinksUsers.Attributes["class"] = "active";
                    break;
                case "tl_views.aspx":
                    tdTLView.Attributes["class"] = "active";
                    break;
                case "upload_clients.aspx":
                    tdLinksAccountOpening.Attributes["class"] = "active";
                    break;
                case "managers_view.aspx":
                    tdManagerView.Attributes["class"] = "active";
                    break;
                //case "eod_report.aspx":
                //    tdLinkEODReports.Attributes["class"] = "active";
                //    break;
                case "getnext.aspx":
                    tdGetNext.Attributes["class"] = "active";
                    break;
                case "action_setup.aspx":
                    tdLinksUsers.Attributes["class"] = "active";
                    break;
                case "instruction_type_allocations.aspx":
                    tdLinksInstructionTypes.Attributes["class"] = "active";
                    break;
                //case "accounts_management.aspx":
                //    tdLinkAccountManagement.Attributes["class"] = "active";
                //    break;
                case "return_report.aspx":
                    tdLinkReturnReports.Attributes["class"] = "active";
                    break;
                case "rates.aspx":
                    //tdLinksRates.Attributes["class"] = "active";
                    break;
                case "AdminPanel.aspx":
                    tdLinksAdminPanel.Attributes["class"] = "active";
                    break;
                default:
                    break;
            }
        }
       
        //hides all nav buttons
        protected void hideAllNavButtons()
        {
            tdLinksReports.Visible = false;
            tdTransactions.Visible = false;
            //tdLinkAccountManagement.Visible = false;
            tdManagerView.Visible = false;
            tdTLView.Visible = false;
            tdGetNext.Visible = false;
            tdLinksAccountOpening.Visible = false;
            tdLinksUsers.Visible = false;
            tdLinksInstructionTypes.Visible = false;
            tdInstructionsView.Visible = false;
            tdLinksReports.Visible = false;
            tdLinkActionSetup.Visible = false;
            //tdLinkAccountManagement.Visible = false;
            //tdLinksRates.Visible = false;
            tdLinkChangePass.Visible = false;
            tdLinksReports.Visible = false;
            tdLinksAdminPanel.Visible = false;
        }
        
        /* Onclick methods set the session and redirect to the requested page */
        protected void tdLinksReports_Click(object sender, EventArgs e) 
        {
            Session["current_page"] = "reports.aspx";
            Server.Transfer("~/reports.aspx", false);
           // Response.Redirect("reports.aspx");
        }

        protected void tdTransactions_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "Instructions.aspx";
           // Server.Transfer("~/Instruction/Instructions.aspx", false);

         //   Response.Redirect("instructions.aspx");
        }

        protected void tdLinkAccountManagement_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "accounts_management.aspx";
           // Server.Transfer("~/accounts_management.aspx", false);
          //  Response.Redirect("accounts_management.aspx");
        }

        protected void tdManagerView_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "Managers_View.aspx";
            //Server.Transfer("~/Managers_View.aspx", false);
          //  Response.Redirect("managers_view.aspx");
        }

        protected void tdTLView_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "TL_View.aspx";
            //Server.Transfer("~/TL_View.aspx", false);
          //  Response.Redirect("tl_views.aspx");
        }
        protected void tdGetNext_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "GetNext.aspx";
            //Server.Transfer("~/GetNext.aspx", false);

           // Response.Redirect("getnext.aspx");
        }
        protected void tdLinksCustomers_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "Upload_clients.aspx";
            //Server.Transfer("~/Upload_clients.aspx", false);

            Response.Redirect("~/Upload_clients.aspx");
        }
        protected void tdLinksUsers_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "User_Admin.aspx";
           // Server.Transfer("~/User_Admin.aspx", false);

          //  Response.Redirect("user_admin.aspx");
        }
        protected void tdLinksInstructionTypes_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "InstructionTypeAllocations.aspx";
         //   Server.Transfer("~/Instruction/InstructionTypeAllocations.aspx", false);

          //  Response.Redirect("instruction_type_allocations.aspx");
        }
        protected void tdInstructionsView_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "InstructionView.aspx";
           // Server.Transfer("~/Instruction/InstructionView.aspx", false);

           // Response.Redirect("instruction_view.aspx");
        }
        protected void tdLinkActionSetup_Click(object sender, EventArgs e)
        {

            //Server.Transfer("~/Action_Setup.aspx", false);
            Session["current_page"] = "Action_Setup.aspx";

            //Response.Redirect("~/Action_Setup.aspx");
        }

        //protected void tdLinksRates_Click(object sender, EventArgs e)
        //{
        //    Session["current_page"] = "rates.aspx";
        //    Server.Transfer("~/rates.aspx", false);

        //  //  Response.Redirect("rates.aspx");
        //}

        protected void tdLinkEODReports_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "EndOfDayReport.aspx";
           // Server.Transfer("~/Reports/EndOfDayReport.aspx", false);
        }

        protected void tdLinkReturnReports_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "ReturnReports.aspx";
            //Server.Transfer("~/Reports/ReturnReports.aspx", false);

            //Response.Redirect("/Reports/ReturnReports.aspx");
        }

        protected void tdLinkResourcesReport_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "ResourceReport.aspx";
            //Server.Transfer("~/Reports/ResourceReport.aspx", false);
        }

        protected void tdLinkTransactionSummary_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "TransactionSummaryReport.aspx";
            //Server.Transfer("~/Reports/TransactionSummaryReport.aspx", false);
        }


        protected void tdLinkTransactionReport_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "TransactionReport.aspx";
           // Server.Transfer("~/Reports/TransactionReport.aspx", false);
        }

        protected void tdLinkAdminPanel_Click(object sender, EventArgs e)
        {
            Session["current_page"] = "AdminPanel.aspx";
        }

        protected void logOut(object sender, EventArgs e)
        {
            Context.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            Response.Redirect("~/Account/Login.aspx");
        }
    }

}