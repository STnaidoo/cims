using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIMS_Datalayer;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace CIMS_Datalayer
{
    //This page contains generic DB functions used 
    //by many pages for data access
    public class GenericDbFunctions
    {
        CIMS_Entities _db = new CIMS_Entities();
        DataTable dt = new DataTable();
        Constants con = new Constants();
        //SqlConnection connection = new SqlConnection(con.getConnectionString());
        //SqlCommand command = new SqlCommand();


        public DataTable GetDropdownListInfo(string tableName, string[] selectors, string[] wheres, string[] values)
        {
            try
            {
                string sortColumn = null;
                string select = "";
                int count = 0;
                foreach (string s in selectors)
                {
                    select += s + ", ";
                    count++;
                    if ((s.Contains("Name") || s.Contains("name") || s.Contains("action") || s == "user_type") && (string.IsNullOrEmpty(sortColumn)))
                    {
                        sortColumn = s;
                    }
                }
                select = select.Remove(select.Length - 2);

                string where = "";
                if (wheres != null)
                {
                    for (int i = 0; i <= wheres.Count() - 1; i++)
                    {
                        if (wheres[i].Contains("<>"))
                        {
                            if (values[i].Any(char.IsDigit))
                            {
                                where += wheres[i] + " <> " + values[i] + " AND ";
                            }
                            else
                            {
                                where += wheres[i] + " <> '" + values[i] + "' AND ";
                            }
                        }
                        else
                        {
                            if (values[i].Contains("|"))
                            {

                                string[] multiValues = values[i].Split(new char[] { '|' });

 

                                where += "(";

                                for (int x = 0; x <= multiValues.Count()-1; x++)
                                {
                                    if (values[i].Any(char.IsDigit))
                                    {
                                        where += wheres[i] + " = " + multiValues[x].Replace(" ",String.Empty) + " OR ";
                                    }
                                    else
                                    {
                                        where += wheres[i] + " = '" + multiValues[x].Replace(" ", String.Empty) + "' OR ";
                                    }
                                }

                                where = where.Remove(where.Length - 4);

                                where += ") AND ";
                            }
                            else
                            {
                                if (wheres[i] == "Client_Customer_Number")
                                {
                                    where += wheres[i] + " = '" + values[i] + "' AND ";
                                }
                                else
                                {
                                    if (values[i].Any(char.IsLetter))
                                    {
                                        where += wheres[i] + " = '" + values[i] + "' AND ";
                                    }
                                    else
                                    {
                                        where += wheres[i] + " = " + values[i] + " AND ";
                                    }
                                }
                            }
                        }
                    }
                    where = where.Remove(where.Length - 5);
                }
                else
                {
                    where = "1 = 1";
                }
                 
                string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
                string connString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("proc_GetDropDownListInfo");
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    da.SelectCommand = cmd;

                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@select", select);
                    cmd.Parameters.AddWithValue("@where", where);
                    if(string.IsNullOrEmpty(sortColumn)) //if it is null just pass a dbnull value. 
                    {
                        cmd.Parameters.AddWithValue("@sortColumn", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@sortColumn", sortColumn);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);

                    return dt;
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDropDownListInfo", ex.Message + " " + ex.InnerException);
                throw ex;
            }
        }


        //OLD WAY OF DOING DROPDOWNS
        //returns the search by item 
        //public List<search_by> GetSearchByDropDownListInfo(string FilterOption)
        //{
        //    try
        //    {
        //        var search = (from data in _db.search_by
        //                      where data.search_by_module == FilterOption
        //                      select data).ToList();

        //        return search.OrderBy(x => x.search_by_name).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetSearchByDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class Currency
        //{
        //    public long currency_id { get; set; }
        //    public string currency_name { get; set; }
        //}

        ////returns list of currencies
        //public List<Currency> GetCurrencyList()
        //{
        //    try
        //    {
        //        var search = (from data in _db.currencies
        //                      select new Currency
        //                      {
        //                          currency_id = data.currency_id,
        //                          currency_name = data.currency_name

        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetCurrencyList", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class RM
        //{
        //    public string RM_Name { get; set; }
        //    public long RM_ID { get; set; }
        //}

        //public List<RM> GetRMDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.relationship_managers
        //                      select new RM
        //                      {
        //                          RM_Name = data.RM_Name,
        //                          RM_ID = data.RM_ID
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetRMDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class DocComment
        //{
        //    public string doc_comments { get; set; }
        //    public long doc_comments_id { get; set; }
        //}

        //public List<DocComment> GetDocCommentsDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.duty_of_care_comments
        //                      select new DocComment
        //                      {
        //                          doc_comments = data.doc_comments,
        //                          doc_comments_id = data.doc_comments_id
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetDocCommentDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class Client
        //{
        //    public string Client_Account_Number { get; set; }
        //    public long Client_ID { get; set; }
        //}

        //public List<Client> GetClientDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.client_details
        //                      select new Client
        //                      {
        //                          Client_Account_Number = data.Client_Account_Number,
        //                          Client_ID = data.Client_ID
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetDocCommentDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class Attachment
        //{
        //    public string file_name { get; set; }
        //    public long attachment_id { get; set; }
        //}

        //public List<Attachment> GetAttachmentDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.instructions_attachment
        //                      select new Attachment
        //                      {
        //                          file_name = data.file_name,
        //                          attachment_id = data.attachment_id
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetDocCommentDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class UserBranch
        //{
        //    public long branch_id { get; set; }
        //    public string branch_name { get; set; }
        //    public string branch_code { get; set; }
        //}

        ////returns a list of user branches
        //public List<UserBranch> GetUserBranchDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.user_branch
        //                      select new UserBranch
        //                      {
        //                          branch_id = data.branch_id,
        //                          branch_name = data.branch_name,
        //                          branch_code = data.branch_code
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetUserBranchDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class GetDocumentIdandStatus
        //{
        //    public long document_status_id { get; set; }
        //    public string document_status_action { get; set; }
        //    public string document_status { get; set; }
        //}

        ////returns a document status
        //public List<GetDocumentIdandStatus> GetDocumentStatusDropDownListInfo(int? system_user_id)
        //{
        //    try
        //    {
        //        if (system_user_id != null)
        //        {
        //            var search = (from data in _db.document_status
        //                          where data.document_status_user_type_who_can_action == system_user_id
        //                          select new GetDocumentIdandStatus
        //                          {
        //                              document_status_id = data.document_status_id,
        //                              document_status_action = data.document_status_action
        //                          }).ToList();

        //            return search;
        //        }
        //        else
        //        {
        //            var search = (from data in _db.document_status
        //                          select new GetDocumentIdandStatus
        //                          {
        //                              document_status_id = data.document_status_id,
        //                              document_status = data.document_status1
        //                          }).ToList();

        //            return search.OrderBy(x => x.document_status).ToList();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetDocumentStatusDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class UserType
        //{
        //    public string user_type_no { get; set; }
        //    public string user_type { get; set; }
        //}

        //public List<UserType> GetUserTypesDropDownListInfo()
        //{
        //    try
        //    {

        //        var search = (from data in _db.user_type
        //                      select new UserType
        //                      {
        //                          user_type_no = data.user_type_no,
        //                          user_type = data.user_type1
        //                      }).ToList();

        //        return search.OrderBy(x => x.user_type).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetUserTypesDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class UserStatus
        //{
        //    public string user_status_desc { get; set; }
        //    public int user_status { get; set; }
        //}

        //public List<UserStatus> GetUserStatusDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.user_status
        //                      select new UserStatus
        //                      {
        //                          user_status_desc = data.user_status_desc,
        //                          user_status = data.user_status1
        //                      }).ToList();

        //        return search.OrderBy(x => x.user_status_desc).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetUserStatusDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class UserModifiedBy
        //{
        //    public string names { get; set; }
        //    public long system_user_id { get; set; }
        //}

        //public List<UserModifiedBy> GetUserModifiedByDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.system_users_view
        //                      select new UserModifiedBy
        //                      {
        //                          names = data.names,
        //                          system_user_id = data.system_user_id
        //                      }).ToList();

        //        return search.OrderBy(x => x.names).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetUserModifiedByDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class UserTitle
        //{
        //    public string title_name { get; set; }
        //    public int title_id { get; set; }
        //}

        //public List<UserTitle> GetUserTitleDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.user_title
        //                      select new UserTitle
        //                      {
        //                          title_name = data.title_name,
        //                          title_id = data.title_id
        //                      }).ToList();

        //        return search.OrderBy(x => x.title_name).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetUserTitleDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //public class GetInstructionTypeandId
        //{
        //    public long instruction_type_id { get; set; }
        //    public string instruction_type { get; set; }
        //}

        ////returns instruction types and ids
        //public List<GetInstructionTypeandId> GetInstructionsTypesDropDownListInfo(int? system_user_id)
        //{
        //    try
        //    {
        //        if (system_user_id != null)
        //        {
        //            var getInstructionTypeId = (from data in _db.instruction_type_allocations
        //                                        where data.status == 1 && data.system_user_id == system_user_id
        //                                        select data.instruction_type_id).FirstOrDefault();

        //            var search = (from data in _db.instructions_types
        //                          where data.instruction_type_ID == getInstructionTypeId
        //                          select new GetInstructionTypeandId
        //                          {
        //                              instruction_type_id = data.instruction_type_ID,
        //                              instruction_type = data.instruction_type
        //                          }).ToList();

        //            return search.OrderBy(x => x.instruction_type).ToList();

        //        }
        //        else
        //        {


        //            var search = (from data in _db.instructions_types
        //                          select new GetInstructionTypeandId
        //                          {
        //                              instruction_type_id = data.instruction_type_ID,
        //                              instruction_type = data.instruction_type
        //                          }).ToList();

        //            return search.OrderBy(x => x.instruction_type).ToList();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetInstructionsTypesDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        //needs unit test


        //needs unit test all below

        public DataTable GetDataSourceUserGridViewInfo(string tableName, string whereClause, string value)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();

                    command = new SqlCommand("proc_VariableSelectAll", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName", tableName);
                    command.Parameters.AddWithValue("@whereClause", whereClause);
                    command.Parameters.AddWithValue("@value", value);
                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataSource", ex.Message);
                throw ex;
            }
            //finally   //finally is unnecssary because we used the using statement above. Use using statements, they are more dank.
            //{
            //    connection.Close();
            //}
        }

        //
        public DataTable GetDataSourceUserGridViewInfo(string tableName, string whereClause, string value, string whereClause2)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();
                    if (string.IsNullOrEmpty(whereClause2))
                    {
                        command = new SqlCommand("proc_GetDataSource", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", value);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }


                    else
                    {
                        command = new SqlCommand("proc_VariableSelectLikeAnd", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataSource", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataSourceUserGridViewInfo(string tableName, string whereClause, string value, string whereClause2, string value2)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = null;
                    if (string.IsNullOrEmpty(whereClause2))
                    {
                        command = new SqlCommand("proc_GetDataSource", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", "'%" + value + "%'");
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_VariableSelectLikeAnd", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", "'" + value + "'");
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value2", value2);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataSource", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataSourceUserGridViewInfo(string tableName, string whereClause, string value, string andClauseB, string andClauseC, string andClauseD, string andValueD, string andClauseE, string andValueE, string andClauseF, string andValueF)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();
                    if (!string.IsNullOrEmpty(andClauseF))
                    {
                        command = new SqlCommand("proc_VariableAndMultiple", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@andClauseB", andClauseB);
                        command.Parameters.AddWithValue("@andClauseC", andClauseC);
                        command.Parameters.AddWithValue("@andClauseD", andClauseD);
                        command.Parameters.AddWithValue("@andValueD", andValueD);
                        command.Parameters.AddWithValue("@andClauseE", andClauseE);
                        command.Parameters.AddWithValue("@andValueE", andValueE);
                        command.Parameters.AddWithValue("@andClauseF", andClauseE);
                        command.Parameters.AddWithValue("@andValueF", andValueE);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (!string.IsNullOrEmpty(andClauseE) && string.IsNullOrEmpty(andClauseF))
                    {
                        command = new SqlCommand("proc_VariableAndMultiple", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@andClauseB", andClauseB);
                        command.Parameters.AddWithValue("@andClauseC", andClauseC);
                        command.Parameters.AddWithValue("@andClauseD", andClauseD);
                        command.Parameters.AddWithValue("@andValueD", andValueD);
                        command.Parameters.AddWithValue("@andClauseE", andClauseE);
                        command.Parameters.AddWithValue("@andValueE", andValueE);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_VariableAndMultiple", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@andClauseB", andClauseB);
                        command.Parameters.AddWithValue("@andClauseC", andClauseC);
                        command.Parameters.AddWithValue("@andClauseD", andClauseD);
                        command.Parameters.AddWithValue("@andValueD", andValueD);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataSource", ex.Message);
                throw ex;
                //return null;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


        public DataTable getDataTableWithFn(string whereClause, string likeValue, string fromDate, string toDate, string AndClause1, string AndValue1, string AndClause2, string AndValue2)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    SqlCommand command = new SqlCommand();

                    if (string.IsNullOrEmpty(AndClause2))
                    {
                        command = new SqlCommand("proc_TableWithFn", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", "'%" + likeValue + "%'");
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@fromDate", fromDate);
                        command.Parameters.AddWithValue("@toDate", toDate);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_TableWithFn2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@fromDate", fromDate);
                        command.Parameters.AddWithValue("@toDate", toDate);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("getDataTableWithFn", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
             
            
        }



        public DataTable GetDataSourceUserGridViewInfoUnion(string tableName1, string tableName2, string whereClause, string value, string selectA, string selectA1, string selectA2, string selectB, string selectB1, string selectB2)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    SqlCommand command = new SqlCommand();

                    command = new SqlCommand("proc_UnionMultipleSelect", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName1", tableName1);
                    command.Parameters.AddWithValue("@tableName2", tableName2);
                    command.Parameters.AddWithValue("@whereClause", whereClause);
                    command.Parameters.AddWithValue("@value", value);
                    command.Parameters.AddWithValue("@selectA", selectA);
                    command.Parameters.AddWithValue("@selectA1", selectA1);
                    command.Parameters.AddWithValue("@selectA2", selectA2);
                    command.Parameters.AddWithValue("@selectB", selectB);
                    command.Parameters.AddWithValue("@selectB1", selectB1);
                    command.Parameters.AddWithValue("@selectB2", selectB2);

                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
               // erl.LogError("GetDataSource", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }



        public DataTable GetDataTableInfo(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    SqlCommand command = new SqlCommand();

                    command = new SqlCommand("proc_MultipleAndsLikeWhere", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName1", tableName1);
                    command.Parameters.AddWithValue("@tableName2", tableName2);
                    command.Parameters.AddWithValue("@whereClause", whereClause);
                    command.Parameters.AddWithValue("@whereClause2", whereClause2);
                    command.Parameters.AddWithValue("@value", value);
                    command.Parameters.AddWithValue("@likeValue", likeValue);
                    command.Parameters.AddWithValue("@AndClause", AndClause);
                    command.Parameters.AddWithValue("@AndClause1", AndClause1);
                    command.Parameters.AddWithValue("@AndValue1", AndValue1);
                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTable", ex.Message);
                throw ex;
                //return null;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataTableInfo(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    if (string.IsNullOrEmpty(AndClause3))
                    {

                        SqlCommand command = new SqlCommand();

                        command = new SqlCommand("proc_DataTableTwoAndsIn", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {

                        SqlCommand command = new SqlCommand();

                        command = new SqlCommand("proc_DataTableThreeAndsIn", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTable", ex.Message);
                throw ex;
                //return null;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


        public DataTable GetDataTableInfoWithOr(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string orClause, string AndClause2, string AndValue2, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    SqlCommand command = new SqlCommand();

                    if (string.IsNullOrEmpty(AndClause4))
                    {
                        command = new SqlCommand("proc_MultipleAndsLikeWhereOr", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@orClause", orClause);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_TwoAndsLikeWhereOr", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@orClause", orClause);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_threeAndsLikeWhereOr", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@orClause", orClause);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@AndClause4", AndClause4);
                        command.Parameters.AddWithValue("@AndValue4", AndValue4);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableWithOR", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }
        
        public DataTable GetDataTableInfoAnd(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string greater, string AndClause2, string AndValue2, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    SqlCommand command = new SqlCommand();
                    if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_greaterAndMultipleTwo", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@greater", greater);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause4))
                    {
                        command = new SqlCommand("proc_greaterAndMultipleThree", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@greater", greater);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_greaterAndMultiple", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@greater", greater);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@AndClause4", AndClause4);
                        command.Parameters.AddWithValue("@AndValue4", AndValue4);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableAnd", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


        public DataTable GetDataTableDate(string tableName, string whereClause, string likeValue, string fromDate, string toDate, string dateChecker, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();


                    if (string.IsNullOrEmpty(AndClause2))
                    {
                        command = new SqlCommand("proc_instructionViewDateAnd", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);

                        if (likeValue == "")
                        {
                            command.Parameters.AddWithValue("@whereClause", "1");
                            command.Parameters.AddWithValue("@likeValue", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@whereClause", whereClause);
                            command.Parameters.AddWithValue("@likeValue", "'%" + likeValue + "%'");
                        }

                        command.Parameters.AddWithValue("@fromDate", "'" + fromDate + "'");
                        command.Parameters.AddWithValue("@toDate", "'" + toDate + "'");
                        command.Parameters.AddWithValue("@dateCheck", dateChecker);

                        if (AndValue1 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause1", "1");
                            command.Parameters.AddWithValue("@AndValue1", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause1", AndClause1);
                            command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        }
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_instructionViewDateAnd1", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);

                        if (likeValue == "")
                        {
                            command.Parameters.AddWithValue("@whereClause", "1");
                            command.Parameters.AddWithValue("@likeValue", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@whereClause", whereClause);
                            command.Parameters.AddWithValue("@likeValue", "'%" + likeValue + "%'");
                        }

                        command.Parameters.AddWithValue("@fromDate", "'" + fromDate + "'");
                        command.Parameters.AddWithValue("@toDate", "'" + toDate + "'");
                        command.Parameters.AddWithValue("@dateCheck", dateChecker);

                        if (AndValue1 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause1", "1");
                            command.Parameters.AddWithValue("@AndValue1", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause1", AndClause1);
                            command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        }

                        if (AndValue2 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause2", "1");
                            command.Parameters.AddWithValue("@AndValue2", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause2", AndClause2);
                            command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        }

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_instructionViewDateAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);

                        if (likeValue == "")
                        {
                            command.Parameters.AddWithValue("@whereClause", "1");
                            command.Parameters.AddWithValue("@likeValue", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@whereClause", whereClause);
                            command.Parameters.AddWithValue("@likeValue", "'%" + likeValue + "%'");
                        }

                        command.Parameters.AddWithValue("@fromDate", "'" + fromDate + "'");
                        command.Parameters.AddWithValue("@toDate", "'" + toDate + "'");
                        command.Parameters.AddWithValue("@dateCheck", dateChecker);

                        if (AndValue1 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause1", "1");
                            command.Parameters.AddWithValue("@AndValue1", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause1", AndClause1);
                            command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        }

                        if (AndValue2 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause2", "1");
                            command.Parameters.AddWithValue("@AndValue2", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause2", AndClause2);
                            command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        }

                        if (AndValue3 == "")
                        {
                            command.Parameters.AddWithValue("@AndClause3", "1");
                            command.Parameters.AddWithValue("@AndValue3", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@AndClause3", AndClause3);
                            command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        }

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableDate", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataTableDefault(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string proc)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();


                    if (string.IsNullOrEmpty(proc))
                    {
                        command = new SqlCommand("proc_defaultCaseFill", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_defaultCase", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@procc", proc);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }


            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableDefault", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataTableInfoDouble(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string proc, string greater, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {

                    SqlCommand command = new SqlCommand();
                    if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_DoubleCondition", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_DoubleConditionTwo", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_DoubleConditionThree", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@AndClause4", AndClause4);
                        command.Parameters.AddWithValue("@AndValue4", AndValue4);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableWithOR", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable GetDataTableDouble(string tableName1, string tableName2, string whereClause, string whereClause2, string likeValue, string AndClause, string value, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string proc, string greater, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_DoubleCon", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause4))
                    {
                        command = new SqlCommand("proc_DoubleConOne", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_DoubleConTwo", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@value", value);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause", AndClause);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue2);
                        command.Parameters.AddWithValue("@orCLause", greater);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@AndClause4", AndClause4);
                        command.Parameters.AddWithValue("@AndValue4", AndValue4);
                        command.Parameters.AddWithValue("@procc", proc);
                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableDouble", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


        public DataTable DataTableDate(string tableName, string whereClause, string likeValue, string fromDate, string toDate, string dateCheck, string AndClause1, string AndValue1, string AndClause2, string AndValue2)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    if (string.IsNullOrEmpty(AndClause1))
                    {
                        command = new SqlCommand("proc_instructionDate", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@fromDate", fromDate);
                        command.Parameters.AddWithValue("@toDate", toDate);
                        command.Parameters.AddWithValue("@dateCheck", dateCheck);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause2))
                    {
                        command = new SqlCommand("proc_instructionDateAnd1", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@fromDate", fromDate);
                        command.Parameters.AddWithValue("@toDate", toDate);
                        command.Parameters.AddWithValue("@dateCheck", dateCheck);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_instructionDateAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@fromDate", fromDate);
                        command.Parameters.AddWithValue("@toDate", toDate);
                        command.Parameters.AddWithValue("@dateCheck", dateCheck);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("DataTableDate", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable DataTableDate3(string tableName, string whereClause, string likeValue, string fromDate, string toDate, string dateCheck, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3)
        {
            

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();

                    command = new SqlCommand("proc_instructionDateAnd3", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName", tableName);
                    command.Parameters.AddWithValue("@whereClause", whereClause);
                    command.Parameters.AddWithValue("@likeValue", likeValue);
                    command.Parameters.AddWithValue("@fromDate", fromDate);
                    command.Parameters.AddWithValue("@toDate", toDate);
                    command.Parameters.AddWithValue("@dateCheck", dateCheck);
                    command.Parameters.AddWithValue("@AndClause1", AndClause1);
                    command.Parameters.AddWithValue("@AndClause2", AndClause2);
                    command.Parameters.AddWithValue("@AndValue2", AndValue2);
                    command.Parameters.AddWithValue("@AndValue1", AndValue1);
                    command.Parameters.AddWithValue("@AndClaus3", AndClause3);
                    command.Parameters.AddWithValue("@AndValue3", AndValue3);

                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("DataTableDate", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }




        public DataTable DataTableAnd(string tableName, string whereClause, string likeValue, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {

                    SqlCommand command = new SqlCommand();

                    if (string.IsNullOrEmpty(AndClause2))
                    {
                        command = new SqlCommand("proc_instructionDateAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_instructionDateAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_instructionDateAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("DataTableDate", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


        public DataTable GetDataTableGrid(string tableName, string whereClause, string likeValue, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    if (string.IsNullOrEmpty(AndClause3))
                    {
                        command = new SqlCommand("proc_instructionDtAnd", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else if (string.IsNullOrEmpty(AndClause4))
                    {
                        command = new SqlCommand("proc_instructionDtAnd1", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_instructionDtAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName", tableName);
                        command.Parameters.AddWithValue("@whereClause", whereClause);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause3", AndClause3);
                        command.Parameters.AddWithValue("@AndValue3", AndValue3);
                        command.Parameters.AddWithValue("@AndClause4", AndClause4);
                        command.Parameters.AddWithValue("@AndValue4", AndValue4);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableGrid", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }



        public DataTable getDataGridJoin(string tableName1, string tableName2, string tableName3, string tb1Join, string tb2Join, string whereClause1, string likeValue, string inValue, string selector, string whereClause2, string whereValue2, string AndClause1, string AndValue1, string AndClause2, string AndValue2)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    if (string.IsNullOrEmpty(AndClause2))
                    {
                        command = new SqlCommand("proc_instructionJoinAnd1", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@tableName3", tableName3);
                        command.Parameters.AddWithValue("@tb1Join", tb1Join);
                        command.Parameters.AddWithValue("@tb2Join", tb2Join);
                        command.Parameters.AddWithValue("@selector", selector);
                        command.Parameters.AddWithValue("@whereClause1", whereClause1);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@whereValue2", whereValue2);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@inValue", inValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }
                    else
                    {
                        command = new SqlCommand("proc_instructionJoinAnd2", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tableName1", tableName1);
                        command.Parameters.AddWithValue("@tableName2", tableName2);
                        command.Parameters.AddWithValue("@tableName3", tableName3);
                        command.Parameters.AddWithValue("@tb1Join", tb1Join);
                        command.Parameters.AddWithValue("@tb2Join", tb2Join);
                        command.Parameters.AddWithValue("@selector", selector);
                        command.Parameters.AddWithValue("@whereClause1", whereClause1);
                        command.Parameters.AddWithValue("@whereClause2", whereClause2);
                        command.Parameters.AddWithValue("@whereValue2", whereValue2);
                        command.Parameters.AddWithValue("@likeValue", likeValue);
                        command.Parameters.AddWithValue("@inValue", inValue);
                        command.Parameters.AddWithValue("@AndClause1", AndClause1);
                        command.Parameters.AddWithValue("@AndValue1", AndValue1);
                        command.Parameters.AddWithValue("@AndClause2", AndClause2);
                        command.Parameters.AddWithValue("@AndValue2", AndValue2);

                        connection.Open();

                        dt.Load(command.ExecuteReader());

                        return dt;
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableGridJoin", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable getDataGridJoinNot(string tableName1, string tableName2, string tb1Join, string tb2Join, string whereClause1, string likeValue, string inValue, string selector, string whereClause2, string whereValue2, string AndClause1, string AndValue1, string AndClause2, string AndValue2)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    command = new SqlCommand("proc_instructionJoinAndNot", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName1", tableName1);
                    command.Parameters.AddWithValue("@tableName2", tableName2);
                    command.Parameters.AddWithValue("@tb1Join", tb1Join);
                    command.Parameters.AddWithValue("@tb2Join", tb2Join);
                    command.Parameters.AddWithValue("@whereClause1", whereClause1);
                    command.Parameters.AddWithValue("@whereClause2", whereClause2);
                    command.Parameters.AddWithValue("@whereValue2", whereValue2);
                    command.Parameters.AddWithValue("@likeValue", likeValue);
                    command.Parameters.AddWithValue("@AndClause1", AndClause1);
                    command.Parameters.AddWithValue("@AndValue1", AndValue1);
                    command.Parameters.AddWithValue("@AndClause2", AndClause2);
                    command.Parameters.AddWithValue("@AndValue2", AndValue2);

                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableGridJoin", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable getDataGridJoinOr(string tableName1, string tableName2, string tableName3, string tb1Join, string tb2Join, string whereClause1, string likeValue, string inValue, string selector, string whereClause2, string whereValue2, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    command = new SqlCommand("proc_instructionJoinAndOr", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName1", tableName1);
                    command.Parameters.AddWithValue("@tableName2", tableName2);
                    command.Parameters.AddWithValue("@tableName3", tableName3);
                    command.Parameters.AddWithValue("@tb1Join", tb1Join);
                    command.Parameters.AddWithValue("@tb2Join", tb2Join);
                    command.Parameters.AddWithValue("@whereClause1", whereClause1);
                    command.Parameters.AddWithValue("@whereClause2", whereClause2);
                    command.Parameters.AddWithValue("@whereValue2", whereValue2);
                    command.Parameters.AddWithValue("@inValue", inValue);
                    command.Parameters.AddWithValue("@likeValue", likeValue);
                    command.Parameters.AddWithValue("@AndClause1", AndClause1);
                    command.Parameters.AddWithValue("@AndValue1", AndValue1);
                    command.Parameters.AddWithValue("@AndClause2", AndClause2);
                    command.Parameters.AddWithValue("@AndValue2", AndValue2);
                    command.Parameters.AddWithValue("@AndClause3", AndClause3);
                    command.Parameters.AddWithValue("@AndValue3", AndValue3);
                    command.Parameters.AddWithValue("@selector", selector);

                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableGridJoinAndOr", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }

        public DataTable getDataGridJoinOr(string tableName1, string tableName2, string tableName3, string tb1Join, string tb2Join, string whereClause1, string likeValue, string inValue, string selector, string whereClause2, string whereValue2, string AndClause1, string AndValue1, string AndClause2, string AndValue2, string AndClause3, string AndValue3, string AndClause4, string AndValue4)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand command = new SqlCommand();



                    command = new SqlCommand("proc_instructionJoinAndOr2", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName1", tableName1);
                    command.Parameters.AddWithValue("@tableName2", tableName2);
                    command.Parameters.AddWithValue("@tableName3", tableName3);
                    command.Parameters.AddWithValue("@tb1Join", tb1Join);
                    command.Parameters.AddWithValue("@tb2Join", tb2Join);
                    command.Parameters.AddWithValue("@whereClause1", whereClause1);
                    command.Parameters.AddWithValue("@whereClause2", whereClause2);
                    command.Parameters.AddWithValue("@whereValue2", whereValue2);
                    command.Parameters.AddWithValue("@inValue", inValue);
                    command.Parameters.AddWithValue("@likeValue", likeValue);
                    command.Parameters.AddWithValue("@AndClause1", AndClause1);
                    command.Parameters.AddWithValue("@AndValue1", AndValue1);
                    command.Parameters.AddWithValue("@AndClause2", AndClause2);
                    command.Parameters.AddWithValue("@AndValue2", AndValue2);
                    command.Parameters.AddWithValue("@AndClause3", AndClause3);
                    command.Parameters.AddWithValue("@AndValue3", AndValue3);
                    command.Parameters.AddWithValue("@AndClause4", AndClause4);
                    command.Parameters.AddWithValue("@AndValue4", AndValue4);
                    command.Parameters.AddWithValue("@selector", selector);

                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDataTableGridJoinAndOr", ex.Message);
                throw ex;
            }
            //finally
            //{
            //    connection.Close();
            //}
        }


    }
}
