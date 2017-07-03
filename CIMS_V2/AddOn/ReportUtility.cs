using System;
using System.Data;
using CIMS_Datalayer;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Linq; 

namespace CIMS_V2.AddOn
{
    public class ReportUtility
    {
        ReportUtilityInfo reportUtility = new ReportUtilityInfo();
        static ErrorLogging errorLog = new ErrorLogging();
        //load the resource report line items based on the selected user.



        public static DataSet getResourcesReportLineItems(DropDownList list)
        {

            try
            {
             
            }
            catch (Exception)
            {

                throw;
            }

            string sql = "Select * From resources_view  Where 1=1 ";
            string addSql = "";
            if (list!=null && list.SelectedIndex > 0)
            {
                addSql = addSql + " AND user_type  = '" + list.SelectedItem.Text + "' ";
            }
            sql = sql + addSql + " ORDER BY user_type";

            try
            {
                return ReportUtilityInfo.getDataSet(sql);
            }
            catch (Exception ex)
            {
                errorLog.LogError("ReportUtility.getResourcesReportLineItems", ex.StackTrace);
            }
            return null;
        }

        //load procssed by user type
        public static DataSet getProcessedByUserType(DropDownList list)
        {
            string sql = "SELECT COUNT(*) AS number_of_instructions FROM instructions_view Where 1=1 AND " 
                + list.SelectedValue + " > 0 GROUP BY instruction_type  ORDER BY instruction_type";

            try
            {
               return ReportUtilityInfo.getDataSet(sql);
            }
            catch(Exception ex)
            {
                
                errorLog.LogError("ReportUtility.getProcessedByUserType", ex.StackTrace);
            }
            return null;
        }

        public static DataSet getProcessedByUserTypeAndInstructionType(DropDownList list)
        {
            string sql = "SELECT instruction_type, COUNT(*) AS number_of_instructions FROM instructions_view Where 1=1 AND "
                + list.SelectedValue + " > 0 GROUP BY instruction_type  ORDER BY instruction_type";

            try
            {
              return ReportUtilityInfo.getDataSet(sql);
            }
            catch (Exception ex)
            {
                ErrorLogging errorLog = new ErrorLogging();
                errorLog.LogError("ReportUtility.getProcessedByUserType", ex.StackTrace);
            }
            return null;
        }

        public static DataSet getTransactionReport(string inputText , string from, string to,DropDownList searchBy, DropDownList status, DropDownList instructionType)
        {
            string sql = "Select * From transactions_view Where " + searchBy.SelectedValue + " LIKE '%" + inputText + "%' "
                + "AND (DateDiff(Day, inserted_date , '" + from + "') <=0  AND DateDiff(Day, inserted_date , '" + to + "') >=0 )";

            string instruction = "", documentStatus = "";
            if (instructionType.SelectedIndex > 0)
            {
                instruction = " AND Instruction_type_id  = '" + instructionType.SelectedValue + "' ";
            }

            if (status.SelectedIndex > 0)
            {
                documentStatus = " AND instruction_status  = '" + status.SelectedValue + "' ";
            }

            string query = sql + instruction + documentStatus;

            try
            {
                return ReportUtilityInfo.getDataSet(query);
            }
            catch(Exception ex)
            {
                //alert error
                errorLog.LogError("ReportUtility.getTransactionReport", ex.StackTrace);
            }
            return null;
        }

        public static DataSet getUnprocessedInstructionsByUserType(DropDownList userTypes)
        {
            string sql = "Select * From unprocessed_by_user_type_view Where 1 = 1 ";
            string addSql = "";
            if (userTypes!=null && userTypes.SelectedIndex > 0)
            {
                addSql = " AND user_type  = '" + userTypes.SelectedItem.Text + "' ";
            }

            sql = sql + addSql + " ORDER BY user_type";
            try
            {
                return ReportUtilityInfo.getDataSet(sql);
            }
            catch (Exception ex)
            {
                errorLog.LogError("ReportUtility.getUnprocessedInstructions", ex.StackTrace);
            }
            return null;
        }

        //queries are the same for usertype and instruction + usertype TODO
        public static DataSet getUnprocessedInstructionByUserTypeAndInstruction(DropDownList userTypes)
        {
            try
            {
               return ReportUtility.getUnprocessedInstructionsByUserType(userTypes);
            }
            catch(Exception ex)
            {
                errorLog.LogError("ReportUtility.getUnprocessedInstructionByUserTypeAndInstruction", ex.StackTrace);

            }
            return null;
        }

        public static DataSet getReturnsReportByBranch(DropDownList branch, DropDownList instructionType, string from, string to)
        {
            int branchId = Convert.ToInt32(branch.SelectedValue);
            int instructionTypeId = Convert.ToInt32(instructionType.SelectedValue);
            string sql = "SELECT Client_Name, amount, currency_name, reference, document_status_name, delivery_date, allocated_to_name, allocated_date, branch_user, branch_proccessed_date FROM instructions_view WHERE branch_id = " + branchId + "" + "AND instruction_type_id = " + instructionTypeId + "";

            string addSql = " AND (DateDiff(Day, inserted_date , '" + from + "') <=0  AND DateDiff(Day, inserted_date , '" + to + "') >=0 )";

            sql = sql + addSql;

            try
            {
                return ReportUtilityInfo.getDataSet(sql);
            }
            catch (Exception ex)
            {
                errorLog.LogError("ReportUtility.getReturnsReportByBranch", ex.StackTrace);
            }
            return null;


        }
        //load procssed by user type
        public static DataSet getTransactionSummaryReport(DropDownList list, string from, string to)
        {
            CIMS_Entities _db = new CIMS_Entities(); //used for linq queries
            DAccessInfo DAI = new DAccessInfo();


            //select count, SUM(amount) AS Amount from dbo.transactions_view

            //where instructionStatusText = document_status_name
            //where Date Start = date1
            //where Date Start = date2
            string selectedStatus = list.SelectedItem.ToString();


            string sql = "SELECT COUNT(instruction_type_id) AS volume, SUM(amount) AS Amount, instruction_type_id, instruction_type " +
                      "FROM dbo.transactions_view WHERE 1=1";


            string addSql = " AND (DateDiff(Day, inserted_date , '" + from + "') <=0  AND DateDiff(Day, inserted_date , '" + to + "') >=0 )";

            string whereSql = "AND document_status_name = '" + selectedStatus + "'";
            //if (list.SelectedIndex > 0)
            //{
            //    addSql = addSql + " AND instruction_status  = '" + list.SelectedValue + "' ";
            //}

            addSql = addSql + whereSql + "GROUP BY instruction_type_id, instruction_type";

            sql = sql + addSql;

            try
            {
               return ReportUtilityInfo.getDataSet(sql);
            }
            catch (Exception ex)
            {
                errorLog.LogError("ReportUtility.getProcessedByUserType", ex.StackTrace);
            }
            return null;
        }
    }
}