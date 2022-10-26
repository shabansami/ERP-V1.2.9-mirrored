using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class ProductionOrderReportVM
    {
        public int Id { get; set; }    // رقم امر الانتاج
        public string OrderNumber { get; set; } // 
        public string OrderBarCode { get; set; }// الباركود
        public string OrderColorName { get; set; }//لون امر الانتاج 
        public float OrderQuantity { get; set; } // الكمية المنتجة 
        public int? BranchId { get; set; }//
        public string BranchName { get; set; }// الفرع   
        public int? FinalItemId { get; set; }//
        public string FinalItemName { get; set; }// المنتج النهائى 
        public DateTime? ProductionOrderDate { get; set; }//تاريخ الانتاج 
        public int? ProductionStoreId { get; set; }//
        public string ProductionStoreName { get; set; }// مخزن التصنيع
        public double FinalItemCost { get; set; }//تكلفة المنتج النهائى(الوحدة الواحدة) 
        public double MaterialItemCost { get; set; } //تكلفة المواد الخام المستخدمة فى الانتاج
        public double DamagesCost { get; set; }//تكلفة التوالف 
        public double TotalCost { get; set; }//التكلفة الاجمالية لامر الانتاج 
        public string Notes { get; set; }

        public List<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public List<ProductionOrderExpens> ProductionOrderExpens { get; set; }
        public List<ProductionOrderReceipt> ProductionOrderReceipts { get; set; }
    }
}