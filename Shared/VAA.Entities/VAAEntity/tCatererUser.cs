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
    
    public partial class tCatererUser
    {
        public long ID { get; set; }
    
        public virtual tCaterer tCaterer { get; set; }
        public virtual tUsers tUsers { get; set; }
    }
}