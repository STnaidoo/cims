using CIMS_Datalayer;
using CIMS_V2.AddOn;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIMS_V2.Reports
{
    public partial class ProcessedInstructionReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadPage();
        }

        protected void LoadPage()
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!String.IsNullOrEmpty(Session["UserID"].ToString()))
            {

            }
            else
            {
                Server.Transfer("~Account/Login.aspx");
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            if (drpReportType.SelectedItem.Text.Equals("By User Type"))
            {
                loadProcessedByUserType();
                dgvUnprocessedByUserTypeandInstructionType.Visible = false;
                dgvUnprocessedByUserType.Visible = true;
            }
            else
            {
                loadProcessedByUserTypeAndInstruction();
                dgvUnprocessedByUserTypeandInstructionType.Visible = true;
                dgvUnprocessedByUserType.Visible = false;
            }
        }

        private void loadProcessedByUserType()
        {
            try
            {
                DataSet set = ReportUtility.getProcessedByUserType(drpUserTypes);
                if (set != null)
                {
                    dgvUnprocessedByUserType.DataSource = set.Tables[0].DefaultView;
                    dgvUnprocessedByUserType.DataBind();
                }
            }
            catch(Exception ex)
            {
                errorLog.LogError("ProcessInstructionReport.getProcessedByUserType", ex.StackTrace);  
            }
        }

        private void loadProcessedByUserTypeAndInstruction()
        {
            DataSet set = ReportUtility.getProcessedByUserType(drpUserTypes);
            try
            {
                if (set != null)
                {
                    dgvUnprocessedByUserTypeandInstructionType.DataSource = set.Tables[0].DefaultView;
                    dgvUnprocessedByUserTypeandInstructionType.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("ProcessInstructionReport.getProccessedByUserTypeAndInstruction", ex.StackTrace);
            }
        }
        #region ExcelExportMethods 

        //This method stops the runtime error that asp.net throws because we cannot nest the Gridview inside a <form> tag. Just leave it here doing nothing.
        //NO TOUCHY!
        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }

        //Calls the excel export method
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUnprocessedByUserType.Visible)
                {
                    writeToExcelFile(dgvUnprocessedByUserType);
                }
                else if(dgvUnprocessedByUserTypeandInstructionType.Visible)
                {
                    writeToExcelFile(dgvUnprocessedByUserTypeandInstructionType);
                }
            }

            catch (Exception ex)
            {
                alert.FireAlerts(this.Page, "An error has been encountered during the export process");
                errorLog.LogError("btnExport_Click", ex.StackTrace);
            }
        }

        //Writes to an excel file
        private void writeToExcelFile(GridView gridView)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string filename = "ProcessedInstructionReport" + DateTime.Now + ".xls";
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