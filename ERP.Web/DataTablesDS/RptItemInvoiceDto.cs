using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class RptItemInvoiceDto
    {
        public RptItemInvoiceDto()
        {
            InvoicesData = new List<InvoiceData>();
        }
        public Guid? Id { get; set; }//item id
        public string ItemName { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalAmount { get; set; }
        public List<InvoiceData> InvoicesData { get; set; }
    }

    public class InvoiceData
    {
        public Guid? SellInvoiceId { get; set; }
        public Guid? InvoiceNum { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public double ItemAmount { get; set; }
    }
}