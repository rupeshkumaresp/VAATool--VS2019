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
    
    public partial class tMenuApprovalStage
    {
        public long ID { get; set; }
        public long MenuID { get; set; }
        public int ApprovalStatusID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
    
        public virtual tApprovalStatuses tApprovalStatuses { get; set; }
        public virtual tMenu tMenu { get; set; }
        public virtual tUsers tUsers { get; set; }
    }
}
