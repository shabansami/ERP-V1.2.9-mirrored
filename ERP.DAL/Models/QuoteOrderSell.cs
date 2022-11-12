using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class QuoteOrderSell : BaseModel //عرض سعر/امر بيع
    {
        public QuoteOrderSell()
        {
            this.QuoteOrderSellDetails = new HashSet<QuoteOrderSellDetail>();
        }

        public int QuoteOrderSellType { get; set; }//الفاتورة عرض سعر/امر بيع
        [ForeignKey(nameof(Customer))]
        public Nullable<Guid> CustomerId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }

        public System.DateTime InvoiceDate { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalValue { get; set; }
        public string Notes { get; set; }
        public string InvoiceNumber { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual Person Customer { get; set; }

        public virtual ICollection<QuoteOrderSellDetail> QuoteOrderSellDetails { get; set; }
    }
}

