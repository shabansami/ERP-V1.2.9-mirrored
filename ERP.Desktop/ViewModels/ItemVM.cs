using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.ViewModels
{
    public class ItemVM
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string ItemCode { get; set; }
        public double? Price { get; set; }
        public Guid GroupBasicId { get; set; }
        public string GroupBasicName { get; set; }
        public Guid GroupSellId { get; set; }
        public string GroupSellName { get; set; }
    }
}
