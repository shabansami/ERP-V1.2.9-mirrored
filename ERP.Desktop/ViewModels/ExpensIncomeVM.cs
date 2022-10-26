using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    internal class ExpensIncomeVM
    {
        public Guid ID { get; set; }
        public string TypeName { get; set; }
        public Guid TypeID { get; set; }
        public double Amount { get; set; }
    }
}
