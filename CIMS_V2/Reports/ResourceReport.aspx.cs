
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System.IO;

namespace CIMS_V2.Reports
{
    public partial class ResourceReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPage();
            }
        }

        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            try
            {
                if (!String.IsNullOrEmpty(Session["UserID"].ToString()))
                {

                    //clear lists
                    drpUserTypes.Items.Clear();

                    SharedFunctions sharedFunctions = new SharedFunctions();
                    ReportUtilityInfo report = new ReportUtilityInfo();

                    sharedFunctions.LoadDropDownList(
                        drpUserTypes,
                        report.GetUserTypeDropDownList(),
                        "user_type",
                        "user_type_no");
                }
                else
                {
                    Server.Transfer("~/Account/Login.aspx");
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("Page_Load", ex.StackTrace);
                throw ex;
            }
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            loadResources();
        }

        //returns dataset based on selected user and loads lineitems to table.
        private void loadResources()
        {
            try
            {
                ReportUtilityInfo reportUtil = new ReportUtilityInfo();
                if (drpUserTypes.SelectedIndex >0)
                {
                    var list = reportUtil.GetResourcesView(drpUserTypes.SelectedItem.Text);

                    if (list !=null)
                    {
                        dgvResources.DataSource = list;
                        dgvResources.DataBind();
                    }
                }
            }
            catch(Exception ex)
            {
                errorLog.LogError("ResourceReport.loadResources", ex.StackTrace);
                throw ex;
            }
        }

        #region ExcelExportMethods 
        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                writeToExcelFile(dgvResources);
            }
            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "An error has been encountered during the export process");
                errorLog.LogError("btnExport_Click", ex.StackTrace);
            }
        }

        private void writeToExcelFile(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string filename = "ResourceReport" + DateTime.Now + ".xls";
            StringWriter writer = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(writer);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.xls";
            Response.AddHeader("content-disposition", "attachment;filename= " + filename);
            gridView.GridLines = GridLines.Both;
            gridView.HeaderStyle.Font.Bold = true;
            gridView.RenderControl(htmlWriter);
            string renderedView = writer.ToString();

            Response.Write(renderedView);
            Response.End();

        }
        #endregion
    }
}