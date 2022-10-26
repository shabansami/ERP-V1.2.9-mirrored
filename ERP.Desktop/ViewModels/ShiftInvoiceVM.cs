using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class ShiftInvoiceVM
    {
        public Guid Id { get; set; }
        public string InvoiceNumPaper { get; set; }
        public string InvoKind { get; set; }
        public string ClintName { get; set; }
        public float Safy { get; set; }
        public string InvoType { get; set; }
        public bool IsFinalApproved { get; set; }
        public string Status { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
