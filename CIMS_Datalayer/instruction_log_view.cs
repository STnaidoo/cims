//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CIMS_Datalayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class instruction_log_view
    {
        public Nullable<double> amount { get; set; }
        public string reference { get; set; }
        public long instruction_id { get; set; }
        public string instruction_type { get; set; }
        public string Client_Name { get; set; }
        public string account_no { get; set; }
        public string branch_user { get; set; }
        public string document_status_name { get; set; }
        public string ft_reference { get; set; }
        public Nullable<System.DateTime> inserted_date { get; set; }
        public string branch_name { get; set; }
        public string instruction_comments { get; set; }
        public string instruction_log { get; set; }
    }
}
