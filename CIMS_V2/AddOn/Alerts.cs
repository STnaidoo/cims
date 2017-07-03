using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CIMS_V2.AddOn
{
    public class Alerts
    {
            public void FireAlerts(Page page, string message)
        {
            ScriptManager.RegisterStartupScript(page, page.GetType(), "msg", "alert('" + message + "');", true);
        }
    }
}