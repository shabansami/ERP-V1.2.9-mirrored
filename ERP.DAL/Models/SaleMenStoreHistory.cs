//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using ERP.DAL.Models;

    public partial class SaleMenStoreHistory : BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Nullable<Guid> EmployeeId { get; set; }
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Store Store { get; set; }
    }
}
