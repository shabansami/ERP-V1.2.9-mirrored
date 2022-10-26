using System;

namespace ERP.Desktop.ViewModels
{
    public class InvoiceDetailVM
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public float Quantity { get; set; }
        public float Amount { get; set; }
    }
}
