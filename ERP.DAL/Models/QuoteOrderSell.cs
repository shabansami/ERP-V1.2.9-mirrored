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
    public partial class QuoteOrderSell : BaseModel //عرض سعر/امر بيع
    {
        public QuoteOrderSell()
        {
            this.QuoteOrderSellDetails = new HashSet<QuoteOrderSellDetail>();
            this.ProductionOrders = new HashSet<ProductionOrder>();
            this.SellInvoices = new HashSet<SellInvoice>();
            this.QuoteOrderSellsChildren = new HashSet<QuoteOrderSell>();
        }

        public int QuoteOrderSellType { get; set; }//الفاتورة عرض سعر/امر بيع
        [ForeignKey(nameof(Customer))]
        public Nullable<Guid> CustomerId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }     
        [ForeignKey(nameof(Quote))]
        public Nullable<Guid> QuoteId { get; set; }

        public System.DateTime InvoiceDate { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalValue { get; set; }
        public string Notes { get; set; }
        public string InvoiceNumber { get; set; }
        [ForeignKey(nameof(OrderSellCase))]
        public Nullable<int> OrderSellCaseId { get; set; }//حالات امر البيع (تم البيع /تم الانتاج/ جارى التنفيذ/تم الانتهاء)

        public virtual Branch Branch { get; set; }
        public virtual Person Customer { get; set; }
        public virtual QuoteOrderSell Quote { get; set; } //عرض السعر المرتبطه به امر البيع
        public virtual OrderSellCase OrderSellCase { get; set; }

        public virtual ICollection<QuoteOrderSellDetail> QuoteOrderSellDetails { get; set; }
        public virtual ICollection<ProductionOrder> ProductionOrders { get; set; }
        public virtual ICollection<SellInvoice> SellInvoices { get; set; }
        public virtual ICollection<QuoteOrderSell> QuoteOrderSellsChildren { get; set; }
    }
}

