using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemMovementdDto
    {
        public string ActionType { get; set; }//نوع الحركة
        public string ActionDate { get; set; }//تاريخ الحركة
        public double IncomingQuantity { get; set; }//كمية الوارد 
        public double IncomingCost { get; set; }//تكلفة الوارد 
        public double OutcomingQuantity { get; set; }//كمية الصادر 
        public double OutcomingCost { get; set; }//تكلفة الصادر 
        public double BalanceQuantity { get; set; }//كمية الرصيد 
        public double BalanceCost { get; set; }//تكلفة الرصيد
        public int? Num { get; set; }
    }
}