using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace CIMS_Datalayer
{
    public class DAccessInfo
    {
        CIMS_Entities _db = new CIMS_Entities();
        ErrorLogging erl = new ErrorLogging();

        public int HowManyRecordsExist1Wheres(string tableName, string whereClause1, string whereValue1)
        {
            int result = 0;

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using(SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_HowManyRecordsExist1Where");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@whereClause1", whereClause1);
                    cmd.Parameters.AddWithValue("@whereValue1", whereValue1);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows.Count;
                }
              
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = 0;
            }

            return result;
        }
        //always close your connections otherwise the terrorists win. best to use a using statement to do so. 
        public int HowManyRecordsExist2Wheres(string tableName, string whereClause1, string whereValue1, string whereClause2, string whereValue2)
        {
            int result = 0;

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_HowManyRecordsExist2Wheres");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@whereClause1", whereClause1);
                    cmd.Parameters.AddWithValue("@whereValue1", whereValue1);
                    cmd.Parameters.AddWithValue("@whereClause2", whereClause2);
                    cmd.Parameters.AddWithValue("@whereValue2", whereValue2);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows.Count;
                }
           
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = 0;
            }

            return result;
        }

        public int HowManyRecordsExist3Wheres(string tableName, string whereClause1, string whereValue1, string whereClause2, string whereValue2, string whereClause3, string whereValue3)
        {
            int result = 0;

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_HowManyRecordsExist3Wheres");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    double val;
                    if (!double.TryParse(whereValue1, out val))
                    {
                        whereValue1 = "'" + whereValue1 + "'";
                    }
                    if (!double.TryParse(whereValue2, out val))
                    {
                        whereValue2 = "'" + whereValue2 + "'";
                    }
                    if (!double.TryParse(whereValue3, out val))
                    {
                        whereValue3 = "'" + whereValue3 + "'";
                    }

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@whereClause1", whereClause1);
                    cmd.Parameters.AddWithValue("@whereValue1", whereValue1);
                    cmd.Parameters.AddWithValue("@whereClause2", whereClause2);
                    cmd.Parameters.AddWithValue("@whereValue2", whereValue2);
                    cmd.Parameters.AddWithValue("@whereClause3", whereClause3);
                    cmd.Parameters.AddWithValue("@whereValue3", whereValue3);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows.Count;
                }
                
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = 0;
            }

            return result;
        }

        public string RunStringReturnStringValue1Where(string tableName, string selector, string whereClause, string value)
        {
            string result = "";

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_RunStringReturnStringValue");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@whereCaluse", whereClause);
                    cmd.Parameters.AddWithValue("@selector", selector);
                    cmd.Parameters.AddWithValue("@value", value);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows[0][0].ToString();
                }

            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = "";
            }

            return result;
        }

        public string RunStringReturnStringValue3Wheres(string tableName, string selector, string whereClause1, string value1, string whereClause2, string value2, string whereClause3, string value3)
        {
            string result = "";

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_RunStringReturnStringValue3Wheres");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@whereClause1", whereClause1);
                    cmd.Parameters.AddWithValue("@whereClause2", whereClause2);
                    cmd.Parameters.AddWithValue("@whereClause3", whereClause3);
                    cmd.Parameters.AddWithValue("@selector", selector);
                    cmd.Parameters.AddWithValue("@value1", value1);
                    cmd.Parameters.AddWithValue("@value2", value2);
                    cmd.Parameters.AddWithValue("@value3", value3);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows[0][0].ToString();
                }
                
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = "";
            }

            return result;
        }

        public string RunStringReturnStringValueIN(string mainTableName, string mainSelector, string mainWhereClause, string secondaryTableName, string secondarySelector, string secondaryWhereClause, string secondaryWhereValue)
        {
            string result = "";

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_RunStringReturnStringValueIN");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@mainTableName", mainTableName);
                    cmd.Parameters.AddWithValue("@mainSelector", mainSelector);
                    cmd.Parameters.AddWithValue("@mainWhereClause", mainWhereClause);
                    cmd.Parameters.AddWithValue("@secondaryTableName", secondaryTableName);
                    cmd.Parameters.AddWithValue("@secondarySelector", secondarySelector);
                    cmd.Parameters.AddWithValue("@secondaryWhereClause", secondaryWhereClause);
                    cmd.Parameters.AddWithValue("@secondaryWhereValue", secondaryWhereValue);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.Rows[0][0].ToString();
                }
                    
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = "";
            }

            return result;
        }

        public bool RunNonQuery1Where(string queryType, string tableName, string[] columns, string[] values, string whereClause, string whereValue)
        {
            bool result = false;
            string update = "";
            try
            {
                for (int i = 0; i <= columns.Count() - 1; i++)
                {
                    char singleQuote = '\'';
                    if (Char.IsDigit(values[i][0]) || values[i][0] == singleQuote)
                    {
                        update += columns[i] + "=" + values[i] + ", ";
                    }
                    else
                    {
                        update += columns[i] + " = '" + values[i] + "', ";
                    }
                    
                }
                update = update.Remove(update.Length - 2);

                int check = _db.proc_RunNonQuery1Where(tableName, whereClause, update, whereValue);
                if (check > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query did not execute correctly", ex.Message);
                result = false;
            }

            return result;
        }

        public bool RunNonQuery2Wheres(string queryType, string tableName, string[] columns, string[] values, string whereClause1, string whereValue1, string whereClause2, string whereValue2)
        {
            bool result = false;
            string update = "";
            try
            {
                for (int i = 0; i <= columns.Count() - 1; i++)
                {
                    update += columns[i] + "=" + values[i] + ", ";
                }
                update = update.Remove(update.Length - 2);

                int check = _db.proc_RunNonQuery2Wheres(tableName, update, whereClause1, whereValue1, whereClause2, whereValue2);
                if (check > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query did not execute correctly", ex.Message);
                result = false;
            }

            return result;
        }

        public bool RunNonQueryEqualsSelect(string queryType, string tableName, string[] columns, string[] values, string equalsSelectColumn, string equalsSelectTableName, string equalsSelectSelector, string equalsSelectWhereClause, string equalsSelectWhereValue, string whereClause, string whereValue)
        {
            bool result = false;

            string update = "";
            try
            {
                for (int i = 0; i <= columns.Count() - 1; i++)
                {
                    update += columns[i] + "=" + values[i] + ", ";
                }
                update = update.Remove(update.Length - 2);

                int check = _db.proc_RunNonQueryEqualsSelect(tableName, update, equalsSelectColumn, equalsSelectTableName, equalsSelectSelector, equalsSelectWhereClause, equalsSelectWhereValue, whereClause, whereValue);
                if (check > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query did not execute correctly", ex.Message);
                result = false;
            }

            return result;
        }

        public bool RunNonQueryInsert(string queryType, string tableName, string[] columns, string[] values)
        {
            bool result = false;

            try
            {
                string cols = "";
                foreach (string c in columns)
                {
                    cols += c + ", ";
                }
                cols = cols.Remove(cols.Length - 2);

                string vals = "";
                double val;
                char singleQuote = '\'';
                foreach (string v in values)
                {
                    if (double.TryParse(v, out val))
                    {
                        vals += v + ", ";
                    }
                    else
                    {

                        if (v[0] != singleQuote)
                        {
                            vals += "'" + v + "', ";
                        }
                        else
                        {
                            vals += v + ", ";
                        }
                    }
                }
                vals = vals.Remove(vals.Length - 2);

                int check = _db.proc_RunNonQueryInsert(tableName, cols, vals);
                if (check > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query did not execute correctly", ex.Message);
                result = false;
            }

            return result;
        }

        public bool RunNonQueryDelete(string queryType, string tableName, string whereClause, string whereValue)
        {
            bool result = false;

            try
            {
                int check = _db.proc_RunNonQueryDelete(tableName, whereClause, whereValue);
                if (check > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query did not execute correctly", ex.Message);
                result = false;
            }

            return result;
        }

        public DbDataReader RunNonQueryReturnDataReader1Where(string tableName, string selector, string whereClause, string whereValue)
        {
            DbDataReader result = null;

            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_RunNonQueryReturnDataReader1Where");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@selector", selector);
                    cmd.Parameters.AddWithValue("@whereClause", whereClause);
                    cmd.Parameters.AddWithValue("@whereValue", whereValue);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.CreateDataReader();
                }
                
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = null;
            }

            return result;
        }

        public DbDataReader RunNonQueryReturnDataReaderMultiWheres(string tableName, string selector, string[] whereClauses, string[] whereValues)
        {
            DbDataReader result = null;

            try
            {
                string where = "";
                for (int i = 0; i <= whereClauses.Count() - 1; i++)
                {
                    if (whereClauses[i].Contains("<>"))
                    {
                        where += whereClauses[i] + whereValues[i] + " AND ";
                    }
                    else
                    {
                        where += whereClauses[i] + "=" + whereValues[i] + " AND ";
                    }
                }

                where = where.Remove(where.Length - 5);

                string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
                string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_RunNonQueryReturnDataReaderMultiWheres");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@selector", selector);
                    cmd.Parameters.AddWithValue("@where", where);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt.CreateDataReader();
                }
                    
                
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = null;
            }

            return result;
        }
    }
}


