using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;

namespace CIMS_Datalayer
{
    public class ToolsSetInfo
    {
        ErrorLogging logError = new ErrorLogging();

        //extracts excel data using OLEDB Connection
        public DataSet getDataSetFromExcel(string excelFilePath, string excelFileName)
        {
            OleDbCommand oleCommand = new OleDbCommand();
            OleDbConnection oleConnection = new OleDbConnection();
            OleDbDataAdapter oleDataAdapter = new OleDbDataAdapter();
            DataSet oleDataSet = new DataSet();

            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Data Source=" + excelFilePath + ";" + 
                    "Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            try
            {
                oleConnection = new OleDbConnection(connectionString);
                oleConnection.Open();

                oleCommand = new OleDbCommand("Select * From [" + excelFileName + "$]", oleConnection);
                oleDataAdapter.SelectCommand = oleCommand;
                oleDataAdapter.Fill(oleDataSet);
            }
            catch (Exception ex)
            {
                logError.LogError("getDataSetFromExcel", ex.StackTrace);
            }
            finally
            {
                oleConnection.Close();
            }
            return oleDataSet;
        }
       

    }
}
