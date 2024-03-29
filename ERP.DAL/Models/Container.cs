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
    using ERP.DAL.Models;

    public partial class Container : BaseModel
    {
        public Container()
        {
            this.PurchaseBackInvoicesDetails = new HashSet<PurchaseBackInvoicesDetail>();
            this.PurchaseInvoicesDetails = new HashSet<PurchaseInvoicesDetail>();
        }
    
        public string Name { get; set; }
        public string ContainerNumber { get; set; }
        public string ShippingCompany { get; set; }
        public Nullable<System.DateTime> ExitDate { get; set; }
        public Nullable<System.DateTime> ArrivalDate { get; set; }
        public string ShippingPort { get; set; }
        public string CompanyName { get; set; }
        public string RespoName { get; set; }
        public string RespoTel { get; set; }
    
        public virtual ICollection<PurchaseBackInvoicesDetail> PurchaseBackInvoicesDetails { get; set; }
        public virtual ICollection<PurchaseInvoicesDetail> PurchaseInvoicesDetails { get; set; }
    }
}
