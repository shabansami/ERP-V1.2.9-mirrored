using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class OrderSellCase : BaseModelInt
    {
        public OrderSellCase()
        {
            this.QuoteOrderSells = new HashSet<QuoteOrderSell>();
        }

        public string Name { get; set; }

        public virtual ICollection<QuoteOrderSell> QuoteOrderSells { get; set; }
    }
}
