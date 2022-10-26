using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class AccountingVM
    {
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string Note { get; set; }
        public DateTime? Date { get; set; }
        public string TransName { get; set; }
    }
}
