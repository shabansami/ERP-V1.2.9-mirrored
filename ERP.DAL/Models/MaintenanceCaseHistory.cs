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

    public partial class MaintenanceCaseHistory : BaseModel
    {
        [ForeignKey(nameof(MaintenanceCas))]
        public Nullable<int> MaintenanceCaseId { get; set; }
        [ForeignKey(nameof(Maintenance))]
        public Nullable<Guid> MaintenanceId { get; set; }
    
        public virtual MaintenanceCas MaintenanceCas { get; set; }
        public virtual Maintenance Maintenance { get; set; }
    }
}
