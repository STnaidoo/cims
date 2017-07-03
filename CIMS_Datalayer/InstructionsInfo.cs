using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMS_Datalayer
{
    public class InstructionsInfo
    {
        CIMS_Entities _db = new CIMS_Entities();

        //returns the search by item given a filtering option
        public class DocComments
        {
            public string doc_comments { get; set; }
            public long doc_coments_id { get; set; }
        }

        //retrurns list of duty of care coments
        public List<DocComments> GetDocComments()
        {
            try
            {
                var search = (from data in _db.duty_of_care_comments
                              select new DocComments
                              {
                                  doc_comments = data.doc_comments,
                                  doc_coments_id = data.doc_comments_id

                              }).ToList();
                return search;
            }
            catch (Exception ex)
            {
                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetDocComments", ex.Message);
                throw ex;
            }
        }

        public class RelationShipManager
        {
            public long RM_ID { get; set; }
            public string RM_Name { get; set; }
        }

        public List<RelationShipManager> GetRM()
        {
            try
            {
                var search = (from data in _db.relationship_managers
                              select new RelationShipManager
                              {
                                  RM_ID = data.RM_ID,
                                  RM_Name = data.RM_Name
                              }).ToList();

                return search;
            }
            catch (Exception ex)
            {

                ErrorLogging erl = new ErrorLogging();
                erl.LogError("GetRM", ex.Message);
                throw ex;
            }
        }

        public class GetInstructionTypeandId
        {
            public long instruction_type_id { get; set; }
            public string instruction_type { get; set; }
        }

        //returns instruction types and ids
        public List<GetInstructionTypeandId> GetInstructionsTypesDropDownListInfo(int? system_user_id)
        {
            try
            {
                if (system_user_id != null)
                {
                    var systemTL1 = (from data in _db.system_users
                                     where data.system_user_id == system_user_id
                                     select data.system_tl_1).FirstOrDefault();

                    var getInstructionTypeId = (from data in _db.instruction_type_allocations
                                                where data.status == 1 && data.system_user_id == systemTL1
                                                select data.instruction_type_id).FirstOrDefault();

                    var search = (from data in _db.instructions_types
                                  where data.instruction_type_ID == getInstructionTypeId
                                  select new GetInstructionTypeandId
                                  {
                                      instruction_type_id = data.instruction_type_ID,
                                      instruction_type = data.instruction_type
                                  }).ToList();

                    return search.OrderBy(x => x.instruction_type).ToList();

                }
                else
                {


                    var search = (from data in _db.instructions_types
                                  select new GetInstructionTypeandId
                                  {
                                      instruction_type_id = data.instruction_type_ID,
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
    }
}
