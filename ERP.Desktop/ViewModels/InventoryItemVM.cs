using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class InventoryItemVM
    {
        public Guid Id { get; set; }
        public double Balance { get; set; }
        public double BalanceReal { get; set; }
        public string ItemName { get; set; }
        public double DifferenceAmount { get; set; }
        public double DifferenceCount { get; set; }
    }
}
