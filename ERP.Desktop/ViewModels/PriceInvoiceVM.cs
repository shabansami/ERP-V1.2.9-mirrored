using System;

namespace ERP.Desktop.ViewModels
{
    public class PriceInvoiceVM
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public double Total { get; set; }
        public float Safy { get; set; }
        public string InvoiceNumber { get; set; }
    }
}