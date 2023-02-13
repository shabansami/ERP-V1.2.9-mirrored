using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemBalanceDto
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string ItemTypeName { get; set; }
        public string GroupName { get; set; }
        public double Balance { get; set; }
        public Nullable<Guid> StoreId { get; set; }
        public string StoreName { get; set; }
        public string DT_Datasource { get; set; }
        public int? Num { get; set; }
        public int? Actions { get; set; }

        //مع تقارير حد الطلب 
        public double Limit { get; set; }//حد الطلب الامن/الخطر

        //مع فواتير الجرد 
        public double BalanceReal { get; set; }
    }
}