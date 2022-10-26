using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DAL;

namespace ERP.Desktop.ViewModels
{
    public class ItemProductionVM
    {
        public ItemProductionVM()
        {
            itemProductionDetails = new List<ItemProductionDetails>();
        }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMob { get; set; }
        public string CustomerAddress { get; set; }

        public string ItemProductionName { get; set; }
        public bool IsCustomerNew { get; set; }
        public bool IsNewItemProduction { get; set; } //التوليفة تم انشاءها فى حالة عميل جديد ام تم انشائها مع عميل موجود مسبقا
        public List<ItemProductionDetails> itemProductionDetails { get; set; }
    }

    public class ItemProductionDetails
    {
        public Guid? ItemId { get; set; }
        public string ItemBarcode { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
    }
}
