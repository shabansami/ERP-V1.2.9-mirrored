using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemProductionDetailsDT
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public string Actions { get; set; }
        //public string Num { get; set; } 
}
}