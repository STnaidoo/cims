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
    
    public partial class document_status33
    {
        public Nullable<int> document_status_flow { get; set; }
        public string document_status { get; set; }
        public Nullable<int> document_status_stage { get; set; }
        public string document_status_action { get; set; }
        public Nullable<int> document_status_user_type_who_can_action { get; set; }
        public long document_status_id { get; set; }
        public Nullable<int> exception { get; set; }
        public Nullable<int> foward_back_to_after_reversal { get; set; }
        public Nullable<int> is_document_held { get; set; }
        public string document_status_user_type_who_can_action_name { get; set; }
        public Nullable<int> is_referral { get; set; }
        public Nullable<int> include_amount_in_checking { get; set; }
        public Nullable<int> must_comment { get; set; }
        public Nullable<int> is_archive { get; set; }
        public Nullable<int> is_new_instruction { get; set; }
        public Nullable<int> is_not_in_my_queue { get; set; }
        public Nullable<int> is_archived { get; set; }
        public Nullable<int> doc_is_amust { get; set; }
        public Nullable<int> is_rework { get; set; }
        public Nullable<int> branch_recall_allowed { get; set; }
    }
}