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
    
    public partial class tPDFGenerationJobs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tPDFGenerationJobs()
        {
            this.tPDFGenerationTasks = new HashSet<tPDFGenerationTasks>();
            this.tPDFGenerationTasksPackingTicket = new HashSet<tPDFGenerationTasksPackingTicket>();
        }
    
        public int PDFGenerationJobID { get; set; }
        public System.DateTime Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tPDFGenerationTasks> tPDFGenerationTasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tPDFGenerationTasksPackingTicket> tPDFGenerationTasksPackingTicket { get; set; }
    }
}