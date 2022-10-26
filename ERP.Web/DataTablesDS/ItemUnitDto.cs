using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemUnitDto
    {
        public Guid Id { get; set; }
        public Guid? UnitNewId { get; set; }
        public string UnitName { get; set; }
        public double Quantity { get; set; }
        public double UnitSellPrice { get; set; }
        public string Actions { get; set; }
        public string Num { get; set; }

    }
}