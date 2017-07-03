using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIMS_Datalayer;
namespace CIMS_V2
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            //Default error handling
            if (!IsPostBack)
            {
                string id = "";
                ErrorLogging erl = new ErrorLogging();
                OperationsLog logOps = new OperationsLog();
                if (string.IsNullOrEmpty(Session["userID"].ToString()))
                {
                        id = Session["userID"].ToString();
                }
                erl.LogError("System Error", "Unexpected System Error Encountered By: " + Session["UserFullName"]);
                logOps.InsertOperationsLog(Convert.ToInt32(id) , "Unexpected System Error", "", "",0, "System Error","",0);
            }
        }
    }
}