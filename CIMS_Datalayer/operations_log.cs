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
    
    public partial class operations_log
    {
        public int internal_log_id { get; set; }
        public Nullable<System.DateTime> time_stamp { get; set; }
        public string message { get; set; }
        public string user_full_name { get; set; }
        public string internal_user_category { get; set; }
        public string ip_address { get; set; }
        public Nullable<int> @private { get; set; }
        public Nullable<int> internal_user_id { get; set; }
        public string log_category { get; set; }
        public int opt_id { get; set; }
        public Nullable<int> user_group { get; set; }
        public string user_branch { get; set; }
    }
}