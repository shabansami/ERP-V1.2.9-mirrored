//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.Web.Test
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductionOrderDetail
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ProductionOrderId { get; set; }
        public Nullable<System.Guid> ItemId { get; set; }
        public double Quantity { get; set; }
        public Nullable<int> ItemCostCalculateId { get; set; }
        public double ItemCost { get; set; }
        public double Quantitydamage { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public Nullable<System.Guid> DeletedBy { get; set; }
        public Nullable<System.Guid> ItemProductionId { get; set; }
        public Nullable<int> ProductionTypeId { get; set; }
        public Nullable<int> ComplexProductionIndex { get; set; }
    
        public virtual ItemCostCalculation ItemCostCalculation { get; set; }
        public virtual ItemProduction ItemProduction { get; set; }
        public virtual Item Item { get; set; }
        public virtual ProductionOrder ProductionOrder { get; set; }
        public virtual ProductionType ProductionType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
