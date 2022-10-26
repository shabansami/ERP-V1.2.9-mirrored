using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public class OfferSellInvoice:BaseModel
    {
        [ForeignKey(nameof(Offer))]
        public Nullable<Guid> OfferId { get; set; }      
        [ForeignKey(nameof(SellInvoice))]
        public Nullable<Guid> SellInvoiceId { get; set; }

        /// <summary>
        /// المبلغ المخصوم في العرض
        /// </summary>
        public double OfferDiscount { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual SellInvoice SellInvoice { get; set; }
    }
}
