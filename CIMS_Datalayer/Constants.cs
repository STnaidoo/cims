using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Web;

namespace CIMS_Datalayer
{
    public class Constants
    {
        public class UserType
        {

        }

        public string getConnectionString()
        {
            string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            string connectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;
            return connectionString;
        }

        public string getEntityConnectionString()
        {
           string entityConnectionString = ConfigurationManager.ConnectionStrings["CIMS_Entities"].ConnectionString;
            return entityConnectionString;
        }

    }
}