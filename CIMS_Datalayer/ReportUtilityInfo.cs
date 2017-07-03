using System;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Collections;
using System.Web.UI.WebControls;

namespace CIMS_Datalayer
{
    //Class used to store shared methods used by the reports.
    public class ReportUtilityInfo
    {
        CIMS_Entities _db = new CIMS_Entities();
        DataTable dt = new DataTable();
        Constants con = new Constants();

        public class UserType
        {
            public string user_type_no { get; set; }
            public string user_type { get; set; }
        }

        //returns the user type drow down list
        public List<UserType> GetUserTypeDropDownList()
        {
            try
            {
                var userTypeList = (from data in _db.user_type
                                    select
         new UserType
         {
             user_type_no = data.user_type_no,
             user_type = data.user_type1
         }).ToList();

                return userTypeList;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetSearchByDropDownListInfo", ex.Message);
                throw ex;
            }

        }



        public class ResourcesReportLineItems
        {
            public string user_type { get; set; }
            public long no_of_resources { get; set; }
            public long system_user_type { get; set; }
            public string instruction_type { get; set; }
        }

        public class UnprocessedInstructionReportByUserType
        {
            public string user_type { get; set; }
            public int no_of_instructions { get; set; }
        }

        public class UnprocessedInstructionReportByUserAndInstruction
        {
            public string user_type { get; set; }
            public long user_type_no { get; set; }
            public int no_of_instructions { get; set; }
        }

        public class TransactionViewLineItem
        {
            public long instruction_id { get; set; }
            public long? branch_manual_approved_by { get; set; }
            public string Instruction_Type { get; set; }
            public string Client_Name { get; set; }
            public string branch_proccessed_date { get; set; }
            public string reference { get; set; }
            public double? amount { get; set; }
            public string document_status_name { get; set; }
            public string currency_name { get; set; }
            public string comments { get; set; }
            public string ftroa_comments { get; set; }
            public string processor_comments { get; set; }
            public int? document_status_stage { get; set; }

        }

        public DataTable GetTransactionview(string tableName, string whereclause, string searchText, string fromDate, string toDate, string dateChecker, string AndClause1, string AndValue1)
        {
            try
            {


                SqlCommand command = new SqlCommand();

                using (SqlConnection connection = new SqlConnection(con.getConnectionString()))
                {


                    command = new SqlCommand("proc_instructionViewDateAnd", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tableName", tableName);
                    if (searchText == null)
                    {
                        command.Parameters.AddWithValue("@whereClause", "1");
                        command.Parameters.AddWithValue("@likeValue", "1");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@whereClause", whereclause);
                        command.Parameters.AddWithValue("@likeValue", "'" + searchText + "'");
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
                        command.Parameters.AddWithValue("@AndValue1", "'" + AndValue1 + "'");
                    }
                    connection.Open();

                    dt.Load(command.ExecuteReader());

                    return dt;

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

        //public List<TransactionViewLineItem> GetTransactionview(
        //    string searchBy,
        //    string query,
        //    DateTime dateFrom,
        //    DateTime dateTo,
        //    string branchID,
        //    string instructionTypeID,
        //    int instructionStatus)
        //{
        //    try
        //    {
        //        long branchId = Convert.ToInt32(branchID);
        //        long instructionId = Convert.ToInt32(instructionTypeID);
        //        List<TransactionViewLineItem> search = new List<TransactionViewLineItem>();
        //        if (searchBy == "Client_Name")
        //        {
        //            search = (from data in _db.transactions_view
        //                          where query.Equals(data.Client_Name)
        //                          && dateFrom <= data.inserted_date
        //                          && dateTo >= data.inserted_date
        //                          && branchId == data.branch_id
        //                          && instructionId == data.instruction_type_id
        //                          select new TransactionViewLineItem
        //                          {
        //                              instruction_id = data.instruction_id,
        //                              branch_manual_approved_by = data.branch_manual_approved_by,
        //                              Instruction_Type = data.instruction_type,
        //                              Client_Name = data.Client_Name,
        //                              branch_proccessed_date = data.branch_proccessed_date,
        //                              reference = data.reference,
        //                              amount = data.amount,
        //                              document_status_name = data.document_status_name,
        //                              currency_name = data.currency_name,
        //                              comments = data.comments,
        //                              ftroa_comments = data.ftroa_comments,
        //                              processor_comments = data.processor_comments,
        //                              document_status_stage = data.document_status_stage
        //                          }).ToList();

        //            return search;
        //        }
        //        else if (searchBy == "currency_name")
        //        {
        //            search = (from data in _db.transactions_view
        //                          where query.Equals(data.currency_name)
        //                          && branchId == data.branch_id
        //                          && instructionId == data.instruction_type_id
        //                          && instructionStatus == data.instruction_status
        //                      select new TransactionViewLineItem
        //                          {
        //                              instruction_id = data.instruction_id,
        //                              branch_manual_approved_by = data.branch_manual_approved_by,
        //                              Instruction_Type = data.instruction_type,
        //                              Client_Name = data.Client_Name,
        //                              branch_proccessed_date = data.branch_proccessed_date,
        //                              reference = data.reference,
        //                              amount = data.amount,
        //                              document_status_name = data.document_status_name,
        //                              currency_name = data.currency_name,
        //                              comments = data.comments,
        //                              ftroa_comments = data.ftroa_comments,
        //                              processor_comments = data.processor_comments,
        //                              document_status_stage = data.document_status_stage
        //                          }).ToList();

        //            return search;
        //        }
        //        else if (searchBy == "Client_Customer_Number")
        //        {
        //            search = (from data in _db.transactions_view
        //                          where query.Equals(data.Client_Account_Number)
        //                          && dateFrom <= data.inserted_date
        //                          && dateTo >= data.inserted_date
        //                          && branchId == data.branch_id
        //                          && instructionId == data.instruction_type_id
        //                          select new TransactionViewLineItem
        //                          {
        //                              instruction_id = data.instruction_id,
        //                              branch_manual_approved_by = data.branch_manual_approved_by,
        //                              Instruction_Type = data.instruction_type,
        //                              Client_Name = data.Client_Name,
        //                              branch_proccessed_date = data.branch_proccessed_date,
        //                              reference = data.reference,
        //                              amount = data.amount,
        //                              document_status_name = data.document_status_name,
        //                              currency_name = data.currency_name,
        //                              comments = data.comments,
        //                              ftroa_comments = data.ftroa_comments,
        //                              processor_comments = data.processor_comments,
        //                              document_status_stage = data.document_status_stage
        //                          }).ToList();

        //            return search;
        //        }
        //        else if (searchBy == "Instruction_Type")
        //        {
        //            search = (from data in _db.transactions_view
        //                          where query.Equals(data.instruction_type_id)
        //                          && dateFrom <= data.inserted_date
        //                          && dateTo >= data.inserted_date
        //                          && branchId == data.branch_id
        //                          && instructionId == data.instruction_type_id
        //                          select new TransactionViewLineItem
        //                          {
        //                              instruction_id = data.instruction_id,
        //                              branch_manual_approved_by = data.branch_manual_approved_by,
        //                              Instruction_Type = data.instruction_type,
        //                              Client_Name = data.Client_Name,
        //                              branch_proccessed_date = data.branch_proccessed_date,
        //                              reference = data.reference,
        //                              amount = data.amount,
        //                              document_status_name = data.document_status_name,
        //                              currency_name = data.currency_name,
        //                              comments = data.comments,
        //                              ftroa_comments = data.ftroa_comments,
        //                              processor_comments = data.processor_comments,
        //                              document_status_stage = data.document_status_stage
        //                          }).ToList();

        //            return search;
        //        }
        //        else
        //        {
        //            search = null;
        //            return search;
        //        }
                

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //returns list of transactions to be printed
        public List<TransactionViewLineItem> GetPrintTransactionView(List<long> InstructionIDs)
        {
            try
            {
                var search = (from data in _db.transactions_view
                              where InstructionIDs.Contains(data.instruction_id)
                              select new TransactionViewLineItem
                              {
                                  instruction_id = data.instruction_id,
                                  branch_manual_approved_by = data.branch_manual_approved_by,
                                  Instruction_Type = data.instruction_type,
                                  Client_Name = data.Client_Name,
                                  branch_proccessed_date = data.branch_proccessed_date,
                                  reference = data.reference,
                                  amount = data.amount.GetValueOrDefault(),
                                  document_status_name = data.document_status_name,
                                  currency_name = data.currency_name,
                                  comments = data.comments,
                                  ftroa_comments = data.ftroa_comments,
                                  processor_comments = data.processor_comments,
                                  document_status_stage = data.document_status_stage
                              }).ToList();
                return null;

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetPrintTransactionView", ex.Message);
                throw ex;
            }
        }

        public List<ResourcesReportLineItems> GetResourceReportLineItems(string userType)
        {
            try
            {
                //resources view unavailable in the db
                return null;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetResourceReportLineItems", ex.Message);
                throw ex;
            }
        }


        public class ProcessedByUserType
        {
            public long number_of_instructions { get; set; }
            public long instruction_type { get; set; }

        }
        public List<ProcessedByUserType> GetProcessedByUserType()
        {

            try
            {
                /* var search = (from data in _db.instructions_view
                               where data.)*/
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetResourceReportLineItems", ex.Message);
                throw ex;
            }
            return null;
        }


        public class ResourcesView
        {
            public string instruction_type { get; set; }
            public int? no_of_resources { get; set; }
            public int? system_user_type { get; set; }
            public string user_type { get; set; }
        }


        public List<ResourcesView> GetResourcesView(String userType)
        {
            try
            {
                var search = (from data in _db.proc_resources_view()
                              where userType == data.user_type
                              select new ResourcesView
                              {
                                  instruction_type = data.instruction_type,
                                  no_of_resources = data.no_of_resources,
                                  system_user_type = data.system_user_type,
                                  user_type = data.user_type

                              }).ToList();

                return search;

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetResourcesView", ex.Message);
                throw ex;
            }
        }
        //method returns an SQLDataReader using a query String

        public class TransactionReportView
        {

            public string Client_Name { get; set; }
            public string Instruction_Type { get; set; }
            public double? amount { get; set; }
            public string currency_name { get; set; }
            public DateTime? inserted_date { get; set; }
            public int? Branch_duration { get; set; }
            public int? FTRO_duration { get; set; }
            public int? Processor_duration { get; set; }
            public string document_status_name { get; set; }
            public string comments { get; set; }
            public string ftroa_comments { get; set; }
            public string processor_comments { get; set; }
        }


        //returns a transaction report by field and options selected
        public List<TransactionReportView> processTranReport(
            DateTime? fromDate,
            DateTime to,
            String query,
            String instructionTypeId,
            String instruction_status,
            String searchField,
            DropDownList drpStatus,
            DropDownList drpInstructions)
        {
            int selection = selectOption(drpStatus, drpInstructions);

            if (searchField.Equals("Client_Name"))
            {
                return GetTransactionReportViewByClientName(
                    fromDate, to, query, instructionTypeId, instruction_status, selection);
            }

            if (searchField.Equals("Client_Customer_Number"))
            {
                return GetTransactionReportViewByClientCustNo(
                    fromDate, to, query, instructionTypeId, instruction_status, selection);
            }

            if (searchField.Equals("Instruction_Type"))
            {
                return GetTransactionReportViewByInstructionType(
                    fromDate, to, query, instructionTypeId, instruction_status, selection);
            }

            if (searchField.Equals("currency_name"))
            {
                return GetTransactionReportViewByCurrency(
                    fromDate, to, query, instructionTypeId, instruction_status, selection);
            }

            return new List<TransactionReportView>();
        }

        private int selectOption(DropDownList drpStatus, DropDownList drpInstructions)
        {
            if (drpInstructions.SelectedIndex > 0 &&
                  drpStatus.SelectedIndex > 0)
            {
                return 3;
            }
            else if (drpInstructions.SelectedIndex > 0 &&
                drpStatus.SelectedIndex < 0)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public List<TransactionReportView> GetTransactionReportViewByClientName(
            DateTime? fromDate,
            DateTime to,
            String query,
            String instructionTypeId,
            String instruction_status,
            int selection)
        {
            try
            {
                long iti = Convert.ToInt64(instructionTypeId);
                int? ins = Convert.ToInt32(instruction_status);
                switch (selection)
                {
                    case 1:
                        var search = (from data in _db.transactions_view
                                      where data.Client_Name.Contains(query)
                                     && (data.inserted_date <= fromDate
                                     && data.inserted_date >= to)
                                     && iti == data.instruction_type_id

                                      select new TransactionReportView
                                      {
                                          Client_Name = data.Client_Name,
                                          Instruction_Type = data.instruction_type,
                                          amount = data.amount,
                                          currency_name = data.currency_name,
                                          inserted_date = data.inserted_date,
                                          Branch_duration = data.Branch_duration,
                                          FTRO_duration = data.FTRO_duration,
                                          Processor_duration = data.Processor_duration,
                                          document_status_name = data.document_status_name,
                                          comments = data.comments,
                                          ftroa_comments = data.ftroa_comments,
                                          processor_comments = data.processor_comments
                                      }).ToList();

                        return search;
                    case 2:
                        search = (from data in _db.transactions_view
                                  where data.Client_Name.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                  && iti == data.instruction_type_id
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    case 3:
                        search = (from data in _db.transactions_view
                                  where data.Client_Name.Contains(query)
                                  && (data.inserted_date >= fromDate && data.inserted_date <= to)
                                  && iti == data.instruction_type_id
                                  && ins == data.instruction_status
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    default:
                        return null; ;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<TransactionReportView> GetTransactionReportViewByClientCustNo(
            DateTime? fromDate,
            DateTime to,
            String query,
            String instructionTypeId,
            String instruction_status,
            int selection)

        {
            try
            {
                long iti = Convert.ToInt64(instructionTypeId);
                int? ins = Convert.ToInt32(instruction_status);
                switch (selection)
                {
                    case 1:
                         var search = (from data in _db.transactions_view
                                      where data.Client_Account_Number.Contains(query)
                                     && (data.inserted_date <= fromDate
                                     && data.inserted_date >= to)
                                     && iti == data.instruction_type_id
                                      select new TransactionReportView
                                      {
                                          Client_Name = data.Client_Name,
                                          Instruction_Type = data.instruction_type,
                                          amount = data.amount,
                                          currency_name = data.currency_name,
                                          inserted_date = data.inserted_date,
                                          Branch_duration = data.Branch_duration,
                                          FTRO_duration = data.FTRO_duration,
                                          Processor_duration = data.Processor_duration,
                                          document_status_name = data.document_status_name,
                                          comments = data.comments,
                                          ftroa_comments = data.ftroa_comments,
                                          processor_comments = data.processor_comments
                                      }).ToList();

                        return search;
                    case 2:
                        search = (from data in _db.transactions_view
                                  where data.Client_Account_Number.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                  && iti == data.instruction_type_id
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    case 3:
                        search = (from data in _db.transactions_view
                                  where data.Client_Account_Number.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                  && iti == data.instruction_type_id
                                  && ins == data.instruction_status
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<TransactionReportView> GetTransactionReportViewByInstructionType(
            DateTime? fromDate,
            DateTime to,
            String query,
            String instructionTypeId,
            String instruction_status,
            int selection)
        {
            try
            {
                switch (selection)
                {
                    case 1:
                        var search = (from data in _db.transactions_view
                                      where data.instruction_type.Contains(query)
                                     && (data.inserted_date <= fromDate
                                     && data.inserted_date >= to)
                                     && instructionTypeId.Equals(data.instruction_type_id)


                                      select new TransactionReportView
                                      {
                                          Client_Name = data.Client_Name,
                                          Instruction_Type = data.instruction_type,
                                          amount = data.amount,
                                          currency_name = data.currency_name,
                                          inserted_date = data.inserted_date,
                                          Branch_duration = data.Branch_duration,
                                          FTRO_duration = data.FTRO_duration,
                                          Processor_duration = data.Processor_duration,
                                          document_status_name = data.document_status_name,
                                          comments = data.comments,
                                          ftroa_comments = data.ftroa_comments,
                                          processor_comments = data.processor_comments
                                      }).ToList();

                        return search;
                    case 2:
                        search = (from data in _db.transactions_view
                                  where data.instruction_type.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                     && instructionTypeId.Equals(data.instruction_type_id)
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    case 3:
                        search = (from data in _db.transactions_view
                                  where data.instruction_type.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                  && instructionTypeId.Equals(data.instruction_type_id)
                                  && instruction_status.Equals(data.instruction_status)
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<TransactionReportView> GetTransactionReportViewByCurrency(
             DateTime? fromDate,
             DateTime to,
             String query,
             String instructionTypeId,
             String instruction_status,
             int selection)
        {
            try
            {
                switch (selection)
                {
                    case 1:
                        var search = (from data in _db.transactions_view
                                      where data.currency_name.Contains(query)
                                     && (data.inserted_date <= fromDate
                                     && data.inserted_date >= to)
                                     && instructionTypeId.Equals(data.instruction_type_id)


                                      select new TransactionReportView
                                      {
                                          Client_Name = data.Client_Name,
                                          Instruction_Type = data.instruction_type,
                                          amount = data.amount,
                                          currency_name = data.currency_name,
                                          inserted_date = data.inserted_date,
                                          Branch_duration = data.Branch_duration,
                                          FTRO_duration = data.FTRO_duration,
                                          Processor_duration = data.Processor_duration,
                                          document_status_name = data.document_status_name,
                                          comments = data.comments,
                                          ftroa_comments = data.ftroa_comments,
                                          processor_comments = data.processor_comments
                                      }).ToList();

                        return search;
                    case 2:
                        search = (from data in _db.transactions_view
                                  where data.currency_name.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                     && instructionTypeId.Equals(data.instruction_type_id)
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    case 3:
                        search = (from data in _db.transactions_view
                                  where data.currency_name.Contains(query)
                                  && (data.inserted_date <= fromDate && data.inserted_date >= to)
                                  && instructionTypeId.Equals(data.instruction_type_id)
                                  && instruction_status.Equals(data.instruction_status)
                                  select new TransactionReportView
                                  {
                                      Client_Name = data.Client_Name,
                                      Instruction_Type = data.instruction_type,
                                      amount = data.amount,
                                      currency_name = data.currency_name,
                                      inserted_date = data.inserted_date,
                                      Branch_duration = data.Branch_duration,
                                      FTRO_duration = data.FTRO_duration,
                                      Processor_duration = data.Processor_duration,
                                      document_status_name = data.document_status_name,
                                      comments = data.comments,
                                      ftroa_comments = data.ftroa_comments,
                                      processor_comments = data.processor_comments
                                  }).ToList();
                        return search;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private SqlDataReader executeQuery(string queryString)
        {
            try
            {


                using (SqlConnection conn = new SqlConnection(con.getConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand(queryString, conn);
                    SqlDataReader sqlDataRead = null;

                    //conn =
                    //    new SqlConnection(
                    //        ConfigurationManager.ConnectionStrings["CIMS_Entities"].ToString());
                    conn.Open();
                    sqlDataRead = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return sqlDataRead;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging errorLog = new ErrorLogging();
                errorLog.LogError("ReportUtilityInfo.executeQuery", ex.StackTrace);
            }
            return null;
        }

        public bool SubmitAndAllocateInstrunctions(
            long? instruction_id,
            int? status,
            long? allocated_to,
            DateTime? allocatedDate,
            int? instruction_status,
            int? user_type,
            int? user_id,
            int document_status_id,
            long? is_referall)
        {
            try
            {
                _db.proc_submit_and_allocate_instructions(
                    instruction_id,
                    status,
                    allocated_to,
                    allocatedDate,
                    instruction_status,
                    user_type,
                    user_id,
                    document_status_id,
                    is_referall);

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogging errorLog = new ErrorLogging();
                errorLog.LogError("SubmitAndAllocateInstrunctions", ex.StackTrace);
            }
            return false;
        }
        //method returns a dataset given a query string

        public static DataSet getDataSet(string queryString)
        {
            try
            {


                Constants constants = new Constants();

                SqlCommand cmd = null;
                SqlDataAdapter adapter = null;
                DataSet dataSet = null;
                using (SqlConnection conn = new SqlConnection(constants.getConnectionString()))
                {
                    conn.Open();
                    dataSet = new DataSet();
                    cmd = new SqlCommand(queryString, conn);
                    adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging errorLog = new ErrorLogging();
                errorLog.LogError("ReportUtilityInfo.getDataSet", ex.StackTrace);
            }
            //finally
            //{
            //    conn.Close();
            //}
            return null;
        }
    }
}
