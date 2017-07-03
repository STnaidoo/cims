using System;
using System.Collections.Generic;
using System.Collections;
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
    public class GetNextInfo
    {
        CIMS_Entities _db = new CIMS_Entities();
        ErrorLogging erl = new ErrorLogging();

        public class GetInstructionTypeandId
        {
            public long instruction_type_ID { get; set; }
            public string instruction_type { get; set; }
        }




        public List<GetInstructionTypeandId> GetInstructionsTypesDropDownListInfo(int? system_user_id)
        {
            try
            {
                if (system_user_id != null)
                {

                    var getInstructionTypeId = (from data in _db.instruction_type_allocations
                                                where data.status == 1 && data.system_user_id == system_user_id
                                                select data.instruction_type_id).FirstOrDefault();

                    var search = (from data in _db.instructions_types
                                  where data.instruction_type_ID == getInstructionTypeId
                                  select new GetInstructionTypeandId
                                  {
                                      instruction_type_ID = data.instruction_type_ID,
                                      instruction_type = data.instruction_type
                                  }).ToList();

                    return search.OrderBy(x => x.instruction_type).ToList();
               
                }
                else
                {


                    var search = (from data in _db.instructions_types
                                  select new GetInstructionTypeandId
                                  {
                                      instruction_type_ID = data.instruction_type_ID,
                                      instruction_type = data.instruction_type
                                  }).ToList();

                    return search.OrderBy(x => x.instruction_type).ToList();                  

                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetInstructionsTypesDropDownListInfo", ex.Message);
                throw ex;
            }
        }


        public class GetDocumentIdandStatus
        {
            public long document_status_id { get; set; }
            public string document_status_action { get; set; }
        }


        public List<GetDocumentIdandStatus> GetDocumentStatusDropDownListInfo(int system_user_id)
        {
            try
            {
                var search = (from data in _db.document_status
                              where data.document_status_user_type_who_can_action == system_user_id
                              
                              select new GetDocumentIdandStatus
                              {
                                  document_status_id = data.document_status_id,
                                  document_status_action = data.document_status_action
                              }).ToList();

                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDocumentStatusDropDownListInfo", ex.Message);
                throw ex;
            }
        }

        //public List<currency> GetCurrencyDropDownListInfo()
        //{
        //    try
        //    {
        //        var search = (from data in _db.currencies
        //                      select new currency
        //                      {
        //                          currency_id = data.currency_id,
        //                          currency_name = data.currency_name
        //                      }).ToList();

        //        return search;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetCurrencyDropDownListInfo", ex.Message);
        //        throw ex;
        //    }
        //}

        public List<currency> GetCurrencyDropDownListInfo()
        {
            try
            {
                var search = (from data in _db.currencies
                              select data).ToList();

                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetCurrencyDropDownListInfo", ex.Message);
                throw ex;
            }
        }



        public List<user_branch> GetUserBranchDropDownListInfo()
        {
            try
            {
                var search = (from data in _db.user_branch
                              select data).ToList();

                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetUserBranchDropDownListInfo", ex.Message);
                throw ex;
            }
        }

        public List<relationship_managers> GetRelationshipManagersDropDownListInfo()
        {
            try
            {
                var search = (from data in _db.relationship_managers
                              select data).ToList();

                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetRelationshipManagersDropDownListInfo", ex.Message);
                throw ex;
            }
        }

        public List<duty_of_care_comments> GetDutyOfCareCommentsDropDownListInfo()
        {
            try
            {
                var search = (from data in _db.duty_of_care_comments
                              select data).ToList();

                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDutyOfCareCommentsDropDownListInfo", ex.Message);
                throw ex;
            }
        }


        public long? GetLockBy(long txtInstructionIDText)
        {
            try
            {
                var get = (from data in _db.instructions
                           where data.instruction_id == txtInstructionIDText
                           select data.locked_by).FirstOrDefault();

                return get;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetLockBy", ex.Message);
                throw ex;
            }
        }


        public void UnlockInstructions(long txtInstructionIDText)
        {
            try
            {
                instruction inst = (from us in _db.instructions
                                    where us.instruction_id == txtInstructionIDText
                                    select us).First();
                inst.locked_by = 0;
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("UnlockInstructions", ex.Message);
            }
        }


        public List<instructions_view> GetInstructions(string drpSearchByText, string txtSearchText, string sessionUserType, string sessionUserID, bool rbnProcessed, DateTime dtmFrom, DateTime dtmTo)
        {
            try
            {
                var instructionsViewList = new List<instructions_view>();

                switch (sessionUserType)
                {

                    case "1":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText)) && us.status == Convert.ToInt32(sessionUserType) && (us.allocated_to == Convert.ToInt64(sessionUserID) || us.inserted_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText)) && us.branch_proccessed_by == Convert.ToInt64(sessionUserID) && (Convert.ToDateTime(us.branch_proccessed_date).Subtract(dtmFrom).Days <= 0) && (Convert.ToDateTime(us.branch_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }


                    case "2":

                        if (!rbnProcessed)
                        {


                            var instructionTypeAllocations = (from ins in _db.instruction_type_allocations
                                                              where ins.system_user_id == Convert.ToInt64(sessionUserID)
                                                              select ins.instruction_type_id).ToList();


                            var items = (from us in _db.instructions_view
                                         join sys in _db.system_users on us.branch_proccessed_by equals sys.system_user_id into ussys
                                         from sys in ussys.DefaultIfEmpty()

                                         where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                         && us.status == Convert.ToInt32(sessionUserType)
                                         && (sys.system_tl_1 == Convert.ToInt64(sessionUserID))
                                         && instructionTypeAllocations.Contains(us.instruction_type_id)
                                         select us).OrderBy(x => x.inserted_date).ToList();

                            return items;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.branch_approved_by == Convert.ToInt64(sessionUserID)
                                          && us.allocated_to == Convert.ToInt64(sessionUserID)
                                          && (Convert.ToDateTime(us.branch_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.branch_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "3":


                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.ftro_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.ftro_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.ftro_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }


                    case "13":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.prmo2_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.prmo2_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.prmo2_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }


                    case "14":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.prmu_tl_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.prmu_tl_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.prmu_tl_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "15":
                    case "22":
                    case "23":
                    case "24":
                    case "25":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.prmu_manager_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.prmu_manager_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.prmu_manager_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "4":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.ftro_approved_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.ftro_approved_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.ftro_approved_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "5":
                    case "6":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.processor_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.ftro_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.ftro_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }


                    case "7":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.rm_proccessed_by == Convert.ToInt64(sessionUserID)
                                          && us.status != Convert.ToInt32(sessionUserType)
                                          && (Convert.ToDateTime(us.rm_proccessed_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.rm_proccessed_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "10":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.ftro_approved_by == Convert.ToInt64(sessionUserID)
                                          && (Convert.ToDateTime(us.ftro_approved_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.ftro_approved_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    case "11":
                    case "17":

                        if (!rbnProcessed)
                        {

                            var itemsins = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && us.status == Convert.ToInt32(sessionUserType)
                                            && (us.picked_by == Convert.ToInt64(sessionUserID))
                                            select us).OrderBy(x => x.inserted_date).ToList();

                            return itemsins;
                        }
                        else
                        {
                            var items2 = (from us in _db.instructions_view
                                          where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                          && us.processor_approved_by == Convert.ToInt64(sessionUserID)
                                          && (Convert.ToDateTime(us.processor_approved_date).Subtract(dtmFrom).Days <= 0)
                                          && (Convert.ToDateTime(us.processor_approved_date).Subtract(dtmTo).Days >= 0)
                                          select us).OrderBy(x => x.inserted_date).ToList();

                            return items2;
                        }

                    default:

                        var itemsdefault = (from us in _db.instructions_view
                                            where (GetColumn(instructionsViewList, drpSearchByText).Contains(txtSearchText))
                                            && (us.allocated_to == Convert.ToInt64(sessionUserID) || (us.picked_by == Convert.ToInt64(sessionUserID)))
                                            && us.status != Convert.ToInt32(sessionUserType)
                                            select us).OrderBy(x => x.inserted_date).ToList();

                        return itemsdefault;

                }


            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetInstructions", ex.Message);
                throw ex;
            }
        }

        // Need to check this function 
        private string GetColumn(List<instructions_view> items, string columnName)
        {
            var values = items.Select(x => x.GetType().GetProperty(columnName).GetValue(x).ToString());
            return values.FirstOrDefault();
        }

        public int how_many_unsubmitted_instructions_exists_for_this_user(int user_id)
        {
            try
            {
                var counts = (from us in _db.instructions_view
                              where (us.is_document_held != 1 || us.is_document_held == null)
                              where us.picked_by.ToString().Contains(user_id.ToString())
                              select us).Count();

                return counts;

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("how_many_unsubmitted_instructions_exists_for_this_user", ex.Message);
                return 0;
            }
        }

        public void get_locked_instructions_approaching_cutoff(string user_id, int user_type, int popup_time)
        {
            try
            {

                switch (user_type)
                {
                    case 3:

                        var instr1 = (from us in _db.instructions_view
                                      where us.is_document_held == 1
                                      && us.status == user_type
                                      && (us.prmo1_popup_alert_status == 0 || us.prmo1_popup_alert_status == null)
                                      && DateTime.Now.Minute + Convert.ToDateTime(us.cutt_off_time).Minute < popup_time
                                      select us).ToList();



                        instr1.ForEach(x =>
                        {
                            x.prmo1_popup_alert_status = 1;
                            x.prmo1_popup_alert_time = DateTime.Now;
                        });
                        _db.SaveChanges();

                        break;



                    case 5:

                        var instr = (from us in _db.instructions_view
                                     where us.is_document_held == 1
                                     && us.status == user_type
                                     && (us.processor_popup_alert_status == 0 || us.processor_popup_alert_status == null)
                                     && DateTime.Now.Minute + Convert.ToDateTime(us.cutt_off_time).Minute < popup_time
                                     select us).ToList();

                        instr.ForEach(x =>
                        {
                            x.processor_popup_alert_status = 1;
                            x.processor_popup_alert_time = DateTime.Now;
                        });
                        _db.SaveChanges();

                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("get_locked_instructions_approaching_cutoff", ex.Message);
                throw ex;
            }
        }

        public DataTable GetNextInstructionProc(int userType, int userId)
        {
            DataTable result = null;
            try
            {


                Constants constants = new Constants();

                using (SqlConnection conn = new SqlConnection(constants.getConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("proc_get_next_instruction");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@status", userType);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    cmd = new SqlCommand("proc_VariableSelectLikeAnd", conn);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tableName", "instructions_view");
                    cmd.Parameters.AddWithValue("@whereClause", "status");
                    cmd.Parameters.AddWithValue("@value", userType);
                    cmd.Parameters.AddWithValue("@whereClause2", "allocated_to");
                    cmd.Parameters.AddWithValue("@value2", userId + " AND document_status_name IS NOT NULL AND branch_proccessed_date IS NOT NULL AND branch_user IS NOT NULL ");
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    da.Fill(dt);
                    result = dt;
                }
            }
            catch (Exception ex)
            {
                erl.LogError("Query could not be executed", ex.Message);
                result = null;
            }

            return result;
        }

        //public List<proc_get_next_instruction_Result> GetNextInstructionProc(int userType, int userId)
        //{
        //    try
        //    {
        //        var items = (from db in _db.proc_get_next_instruction(userType, userId)
        //                     select db).ToList();
        //        return items;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogging erl = new ErrorLogging();
        //        erl.LogError("GetNextInstructionProc", ex.Message);
        //        throw ex;
        //    }
        //}

        public bool AddAttachement(string fileName, Int64 instructionId, Int64 userId)
        {
            try
            {
                var add = new instructions_attachment();
                add.file_name = fileName;
                add.date_inserted = DateTime.Now;
                add.instruction_id = instructionId;
                add.inserted_by = userId;
                _db.instructions_attachment.Add(add);
                _db.SaveChanges();

                return true;

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("AddAttachement", ex.Message);
                return false;
            }
        }


        //dnx.LoadDropDownListing("", "select '0' AS attachment_id, ' Default' AS file_name from instructions_attachment UNION 
        //select attachment_ID,  file_name  from instructions_attachment where (deleted is null or deleted =0) and instruction_id = '" & txtInstructionID.Text & "' ", drpAttachments, "file_name", "attachment_id", My.Settings.strDSN)
   

        public List<instructions_attachment> GetAttachments(Int64 instructionId)
        {
            try
            {
                List<instructions_attachment> items = _db.instructions_attachment.ToList().Where(i => i.instruction_id == instructionId).ToList();

                return items;
            }
            //try
            //{
            //    var items = (from db in _db.instructions_attachment
            //                 where db.instruction_id == instructionId
            //                 && db.deleted == null || db.deleted == 0

            //                 select new instructions_attachment
            //                 {
            //                     attachment_id = db.attachment_id,
            //                     file_name = db.file_name
            //                 }).ToList();
            //    return items;
            //}
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetAttachments", ex.Message);
                throw ex;
            }
        }


        public void DeleteAttachment(long deletedByUserId, long instructionId)
        {
            try
            {
                instructions_attachment inst = (from us in _db.instructions_attachment
                                    where us.attachment_id == instructionId
                                    select us).First();
                inst.deleted = 1;
                inst.deleted_date = DateTime.Now;
                inst.deleted_by = deletedByUserId;
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("DeleteAttachment", ex.Message);
            }
        }

        public long? AtachmentInsertBy(long instructionId)
        {
            try
            {
                var inst = (from us in _db.instructions_attachment
                                                where us.instruction_id == instructionId
                                                select us.inserted_by).First();

                return inst;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("AtachmentInsertBy", ex.Message);
                throw ex;
            }
        }

        public List<instruction> CheckForDuplicates(string acc_no, long instruction_type, long currency, double amount, long? instruction_id, int duplicate_check_days)
        {
            try
            {

                if (instruction_id == null)
                {

                    var items = (from us in _db.instructions
                                 where us.account_no == acc_no &&
                                 us.instruction_type_id == instruction_type &&
                                 us.currency_id == currency &&
                                 us.amount == amount &&
                                 ((DateTime.Now.Day - us.inserted_date.Value.Day) < duplicate_check_days)

                                 select us).ToList();

                    return items;
                }
                else
                {
                    var items = (from us in _db.instructions
                                 where us.account_no == acc_no &&
                                 us.instruction_type_id == instruction_type &&
                                 us.currency_id == currency &&
                                 us.amount == amount &&
                                 us.instruction_id != instruction_id &&
                                 ((DateTime.Now.Day - us.inserted_date.Value.Day) < duplicate_check_days)

                                 select us).ToList();

                    return items;
                }
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("CheckForDuplicates", ex.Message);
                throw ex;
            }
        }
    }
}
