using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIMS_V2
{
    public partial class no_rights : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

        }
    }
}