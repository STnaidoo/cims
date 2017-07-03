
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
namespace CIMS_Datalayer
{
    public class UploadClientsInfo
    {
        public bool proc_insert_client_details()
        {
            //  SqlConnection conn = new SqlConnection()
            // SqlCommand command;
            //  Boolean inserted = false;

            // return inserted;

            string test = ConfigurationManager.ConnectionStrings.ToString();
            Console.Write(test);

            return true;
        }
    }
}
