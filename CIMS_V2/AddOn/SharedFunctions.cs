using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Text;
using System.Configuration;

namespace CIMS_V2.AddOn
{
    public class SharedFunctions
    {
        
        public void LoadDropDownList(DropDownList dropDownL, Object dataSource, string TextField, string ValueField)
        {
            dropDownL.DataSource = dataSource;
            dropDownL.DataTextField = TextField;
            dropDownL.DataValueField = ValueField;
            dropDownL.DataBind();
        }

        public void LoadCheckList(CheckBoxList checkList, Object dataSource, string TextField, string ValueField)
        {
            checkList.DataSource = dataSource;
            checkList.DataTextField = TextField;
            checkList.DataValueField = ValueField;
            checkList.DataBind();
        }

        public void LoadGridView(GridView gridV, DataTable dataSource)
        {
            if(dataSource != null)
            {
                gridV.DataSource = dataSource; //check if works
                gridV.DataBind();
            }                
        }

        public void SendMail(String to, String subject, String body)
        {
            MailMessage objecto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            client.Port             = Convert.ToInt32(ConfigurationManager.AppSettings["port"].ToString());
            client.Host             = ConfigurationManager.AppSettings["host"].ToString();
            client.Timeout          = Convert.ToInt32(ConfigurationManager.AppSettings["timeout"].ToString());

            client.DeliveryMethod   = SmtpDeliveryMethod.Network;
            //objecto_mail.IsBodyHtml = true;
            
            string currentUrl = ""; //will use this to adapt to which UAT/Prod instance is running on a particular server

            objecto_mail.From       = new MailAddress("noreply@cimsadmin.co.za");            
            objecto_mail.Subject    = subject;
            objecto_mail.Body       = body + "\r\n\r\nFor your reference, here is the link to CIMS:\r\n"
                                           + "http://10.248.171.59/CIMS_UAT/"
                                           + "\r\n\r\nRegards\r\nCIMS Admin";

            objecto_mail.To.Add(new MailAddress(to));

            client.Send(objecto_mail);
        }
    }
}