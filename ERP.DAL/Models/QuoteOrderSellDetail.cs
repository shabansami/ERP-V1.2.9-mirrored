using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;

namespace ERP.DAL
{
    public partial class QuoteOrderSellDetail : BaseModel
    {
        [ForeignKey(nameof(QuoteOrderSell))]
        public Nullable<Guid> QuoteOrderSellId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        [ForeignKey(nameof(ItemProduction))]
        public Nullable<Guid> ItemProductionId { get; set; }//التوليفة المحددة فى حالة تجهيز لانتاج مجمع 
        public Nullable<int> OrderSellItemType { get; set; }//نوع اصناف امر البيع (اصناف للبيع/اصناف للانتاج
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }//المخزن المحدد فى حالة تجهيز لفاتورة بيع 
        [ForeignKey(nameof(Unit))]
        public Nullable<Guid> UnitId { get; set; }

        /// <summary>
        /// الكمية
        /// </summary>
        public double Quantity { get; set; }
        public double QuantityUnit { get; set; }
        /// <summary>
        /// سعر الصنف
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// اجمالي سعر الكمية
        /// </summary>
        public double Amount { get; set; }


        public virtual Item Item { get; set; }
        public virtual QuoteOrderSell QuoteOrderSell { get; set; }
        public virtual ItemProduction ItemProduction { get; set; }//التوليفة المحددة فى حالة تجهيز لانتاج مجمع 
        public virtual Store Store { get; set; } //المخزن المحدد فى حالة تجهيز لفاتورة بيع 
        public virtual Unit Unit { get; set; }

    }
}
