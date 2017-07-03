using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMS_Datalayer
{
    public class ErrorLogging
    {
        CIMS_Entities _db = new CIMS_Entities();

        public void LogError(string source, string stackTrace)
        {
            var addError = new ErrorLog();
            addError.Source = source;
            addError.ErrorDescription = stackTrace;
            addError.DateCreated = DateTime.Now;
            _db.ErrorLogs.Add(addError);
            _db.SaveChanges();            
        }

    }
}