using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemProductionOrderDetailsDT
    {
        public Guid? ItemProductionId { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public double? ItemCost { get; set; }
        public Guid? StoreUnderId { get; set; }
        public int? ItemCostCalculateId { get; set; }
        public double? QuantityRequired { get; set; }
        public double? QuantityAvailable { get; set; }

        //public double QuantityNew { get; set; } // االكمية الجديدة المضافة للتوليفة 
        public bool IsAllQuantityDone { get; set; } // فى حالة الترو تكون كلاس success else dengar
        public int? Actions { get; set; }
        public int? ActionStatus { get; set; } // تستخدم فى الجريد لعرض (رصيد يكفى او لا
        public string DT_DatasourceIn { get; set; }
        public string DT_DatasourceOut { get; set; }
        public double Quantitydamage { get; set; } //الكمية التالفة 

    }
    public class ItemProductionQuantity
    {
        //مع تقرير متوسط انتاج منتج من المواد الخام المتاحة
        public Guid? ItemId { get; set; }
        public double? QuantityAvailable { get; set; }
        //public double QuantityExpected { get; set; } //الكمية المتوقع انتاجها  
        public double? QuantityItem { get; set; } //كمية التوليفة  
    }
}


        //notes 18-10-2021

        //old notes 18-10-2021
        //public int? ItemProductionId { get; set; }
        //public int? ItemId { get; set; }
        //public string ItemName { get; set; }
        //public double? QuantityRequired { get; set; }
        //public double? QuantityAvailable { get; set; }
        //public int? Actions { get; set; }
        //public bool IsAllQuantityDone { get; set; } // فى حالة الترو تكون كلاس success else dengar
