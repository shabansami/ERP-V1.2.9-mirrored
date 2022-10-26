using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ProductionOrderReceiptVM
    {
        //public Guid Id { get; set; }
        public Guid? ProductionOrderId { get; set; }
        public string OrderNumber { get; set; }
        public Guid? FinalItemId { get; set; }
        public string FinalItemName { get; set; }
        public double OrderQuantity { get; set; } //كمية الانتاج
        public double ReceiptQuantity { get; set; } //كمية التسليم
        public double ReceiptQuantityOld { get; set; } //كمية التسليم مسبقا فى حالة تسليها على دفعات
        public Guid? FinalItemStoreId { get; set; } //مخزن المنتج النهائى
        public DateTime? OrderDate { get; set; } // تاريخ امر الانتاج
        public DateTime? ReceiptDate { get; set; } // تاريخ التسليم
        public string Notes { get; set; }
    }
}