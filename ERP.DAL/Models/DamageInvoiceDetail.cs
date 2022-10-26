using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class DamageInvoiceDetail:BaseModel
    {
        [ForeignKey(nameof(DamageInvoice))]
        public Nullable<Guid> DamageInvoiceId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        public double Quantity { get; set; }
        public double CostQuantity { get; set; }

        public virtual DamageInvoice DamageInvoice { get; set; }
        public virtual Item Item { get; set; }
    }

}
