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

    public partial class ProductionOrder : BaseModel
    {
        public ProductionOrder()
        {
            this.ItemSerials = new HashSet<ItemSerial>();
            this.ProductionOrderDetails = new HashSet<ProductionOrderDetail>();
            this.ProductionOrderExpenses = new HashSet<ProductionOrderExpens>();
            this.ProductionOrderReceipts = new HashSet<ProductionOrderReceipt>();
            this.StoresTransferDetails = new HashSet<StoresTransferDetail>();
        }
    
        public string OrderBarCode { get; set; }

        [ForeignKey(nameof(ProductionOrderColor))]
        public Nullable<Guid> OrderColorId { get; set; }

        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }


        [ForeignKey(nameof(ProductionStore))]
        public Nullable<Guid> ProductionStoreId { get; set; }
        [ForeignKey(nameof(ProductionUnderStore))]
        public Nullable<Guid> ProductionUnderStoreId { get; set; }    
        [ForeignKey(nameof(OrderSell))]
        public Nullable<Guid> OrderSellId { get; set; }

        public double TotalCost { get; set; }//������ ����� ��� ������� ������ ������ �����+������ �������+����� �������
        public string Notes { get; set; }
        public DateTime ProductionOrderDate { get; set; }
        public bool IsDone { get; set; }
        public string OrderNumber { get; set; }
        [ForeignKey(nameof(EmployeeProduction))]
        public Nullable<Guid> EmployeeProductionId { get; set; }
        [ForeignKey(nameof(EmployeeOperation))]
        public Nullable<Guid> EmployeeOperationId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Employee EmployeeProduction { get; set; }//������� �� �������
        public virtual Employee EmployeeOperation { get; set; }//������� �� �������

        //Item

        //Store
        public virtual Store ProductionStore { get; set; }
        public virtual Store ProductionUnderStore { get; set; }
        public virtual ProductionOrderColor ProductionOrderColor { get; set; }
        public virtual QuoteOrderSell OrderSell { get; set; }


        public virtual ICollection<ItemSerial> ItemSerials { get; set; }
        public virtual ICollection<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual ICollection<ProductionOrderExpens> ProductionOrderExpenses { get; set; }
        public virtual ICollection<ProductionOrderReceipt> ProductionOrderReceipts { get; set; }
        public virtual ICollection<StoresTransferDetail> StoresTransferDetails { get; set; }
    }
}
