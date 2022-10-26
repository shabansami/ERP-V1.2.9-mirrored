using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class PrintBarcodeVM
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public double TotalQuantity { get; set; }
        public DateTime? InvoDate { get; set; }
        public List<ItemSerialM> ItemSerial { get; set; }
    }
    public class ItemSerialM
    {
        public string ItemName { get; set; }
        public string SerialNumber { get; set; }

    }
}