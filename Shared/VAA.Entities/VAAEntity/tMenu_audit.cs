//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VAA.Entities.VAAEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tMenu_audit
    {
        public long ID { get; set; }
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public Nullable<int> MenuTypeID { get; set; }
        public string ChiliPDFName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> LanguageID { get; set; }
        public Nullable<long> CycleID { get; set; }
        public Nullable<int> CurrentApprovalStatusID { get; set; }
        public Nullable<bool> IsMovedToLiveOrder { get; set; }
        public Nullable<int> ReorderCount { get; set; }
    }
}