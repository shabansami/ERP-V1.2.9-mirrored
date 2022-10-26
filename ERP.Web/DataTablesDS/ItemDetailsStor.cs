using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class ItemDetailsStor
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemName { get; set; }
        public string ContainerName { get; set; }
        public double Quantity { get; set; }
        public double QuantityReal { get; set; }

    }
}