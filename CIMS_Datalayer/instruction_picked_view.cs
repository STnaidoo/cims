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
    
    public partial class instruction_picked_view
    {
        public Nullable<System.DateTime> instruction_picked_date { get; set; }
        public Nullable<int> instruction_picked_by { get; set; }
        public long instruction_picked_id { get; set; }
        public Nullable<long> instruction_id { get; set; }
        public Nullable<long> instruction_picked_user_type { get; set; }
        public string system_user_fname { get; set; }
        public string system_user_lname { get; set; }
        public Nullable<long> system_user_id { get; set; }
    }
}