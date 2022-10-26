using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.DTOs
{
    public class ItemsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? GroupBasicId { get; set; }
        public string GroupBasicName { get; set; }
        public string GroupSellId { get; set; }
        public string GroupSellName { get; set; }
        public Guid? UnitId { get; set; }
        public string UnitName { get; set; }
        public Guid? ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }

        public string BarCode { get; set; }
        public string ItemCode { get; set; }

        public float SellPrice { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string AvailableToSell { get; set; }
        public string TechnicalSpecifications { get; set; }

    }
}
