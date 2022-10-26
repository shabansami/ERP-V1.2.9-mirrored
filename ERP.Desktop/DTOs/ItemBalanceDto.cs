using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.DTOs
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
        public string StoreName { get; set; }
        public int? Num { get; set; }

        //مع تقارير حد الطلب 
        public double Limit { get; set; }//حد الطلب الامن/الخطر

        //مع فواتير الجرد 
        public double BalanceReal { get; set; }
    }
}
