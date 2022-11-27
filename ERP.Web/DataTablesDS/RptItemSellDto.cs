using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptItemSellDto
    {
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public int? Num { get; set; }
    }
}