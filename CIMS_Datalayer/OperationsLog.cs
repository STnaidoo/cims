using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMS_Datalayer
{
    public class OperationsLog
    {
        public bool InsertOperationsLog(int user_id_in, string message_in ,string internal_user_category_in, string private_in , int opt_id_in , string log_category_in , string ipAddress, int user_group)
        {
            try {
                CIMS_Entities _db = new CIMS_Entities();
                _db.proc_insert_operations_log(user_id_in, message_in, internal_user_category_in, ipAddress, 0, opt_id_in == null ? 0 : opt_id_in, log_category_in, user_group == null ? 0 : user_group);

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
