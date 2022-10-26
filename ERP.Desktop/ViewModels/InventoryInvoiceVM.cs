using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class InventoryInvoiceVM
    {
        public Guid Id { get; set; }
        public Guid InvoiceGuid { get; set; }
        public string InvoiceDate { get; set; }
        public string BranchName { get; set; }
        public string StoreName { get; set; }
        public double TotalDifferenceAmount { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
