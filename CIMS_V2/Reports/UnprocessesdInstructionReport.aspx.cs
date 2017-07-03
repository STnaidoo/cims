using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIMS_V2.AddOn;
using CIMS_Datalayer;
using System.Data;
using System.IO;

namespace CIMS_V2.Reports
{
    public partial class UnprocessesdInstructionReport : System.Web.UI.Page
    {
        ErrorLogging errorLog = new ErrorLogging();
        Alerts alert = new Alerts();
        //load user types
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.Contents.Count == 0)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (!IsPostBack)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
                    {
                        //load user type via DNX
                        ReportUtilityInfo reportUtil = new ReportUtilityInfo();
                        SharedFunctions sharedFunctions = new SharedFunctions();
                        sharedFunctions.LoadDropDownList(
                            drpUserTypes,
                            reportUtil.GetUserTypeDropDownList(),
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
                }
            }
        }

        //load unprocessed instructions by user type
        private void loadUnProcessedByUserType()
        {
            try
            {
                DataSet data = ReportUtility.getUnprocessedInstructionsByUserType(drpUserTypes);
                if (data != null)
                {
                    dgvUnprocessedByUserType.DataSource = data.Tables[0].DefaultView;
                    dgvUnprocessedByUserType.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("loadUnProcessedByUserType", ex.StackTrace);
            }
        }


        //load unprocessed instruction by user and instruction type
        private void loadUnProcessedByUserTypeAndInstruction()
        {
            try
            {
                DataSet set = ReportUtility.getUnprocessedInstructionByUserTypeAndInstruction(drpUserTypes);
                if (set != null)
                {
                    dgvUnprocessedByUserTypeandInstructionType.DataSource = set.Tables[0].DefaultView;
                    dgvUnprocessedByUserTypeandInstructionType.DataBind();
                }
            }
            catch (Exception ex)
            {
                errorLog.LogError("loadUnProcessedByUserTypeAndInstruction", ex.StackTrace);

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
                if(dgvUnprocessedByUserType != null)
                {
                    writeToExcelFile(dgvUnprocessedByUserType);
                }
                else if(dgvUnprocessedByUserTypeandInstructionType != null)
                {
                    writeToExcelFile(dgvUnprocessedByUserTypeandInstructionType);
                }
                else
                {
                    alert.FireAlerts(this.Page, "The data grid views have no data in them.");
                }
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
            string filename = "UnprocessedInstructionReport" + DateTime.Now + ".xls";
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
        protected void btnView_Click(object sender, EventArgs e)
        {
            if (drpReportType.SelectedItem.Text.Equals("By User Type"))
            {
                loadUnProcessedByUserType();
                dgvUnprocessedByUserTypeandInstructionType.Visible = false;
                dgvUnprocessedByUserType.Visible = true;
            }
            else
            {
                loadUnProcessedByUserTypeAndInstruction();
                dgvUnprocessedByUserTypeandInstructionType.Visible = true;
                dgvUnprocessedByUserType.Visible = false;
            }
        }
    }
}