using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public class OfferDetail:BaseModel
    {
        [ForeignKey(nameof(Offer))]
        public Guid OfferId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        public double Quantity { get; set; }

        public virtual Offer Offer { get; set; }
        public virtual Item Item { get; set; }
    }
}
