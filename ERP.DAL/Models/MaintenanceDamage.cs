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

    public partial class MaintenanceDamage : BaseModel
    {
        public MaintenanceDamage()
        {
            this.MaintenanceDamageDetails = new HashSet<MaintenanceDamageDetail>();
        }

        [ForeignKey(nameof(MaintenanceDetail))]
        public Nullable<Guid> MaintenanceDetailId { get; set; }

        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }

        public virtual MaintenanceDetail MaintenanceDetail { get; set; }
        public virtual Store Store { get; set; }


        public virtual ICollection<MaintenanceDamageDetail> MaintenanceDamageDetails { get; set; }
    }
}
