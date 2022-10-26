using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class StoresTransferDetailsDto
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public int? IsIntial { get; set; }
        public Guid? SerialItemId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public Guid? ProductionOrderId { get; set; }
        public bool? IsItemIntial { get; set; }
        public string DT_Datasource { get; set; }
        public string Actions { get; set; }

    }
}