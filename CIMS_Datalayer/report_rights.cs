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
    
    public partial class report_rights
    {
        public long report_rights_id { get; set; }
        public Nullable<int> user_type { get; set; }
        public Nullable<int> sub_menu_id { get; set; }
    
        public virtual sub_menu sub_menu { get; set; }
    }
}