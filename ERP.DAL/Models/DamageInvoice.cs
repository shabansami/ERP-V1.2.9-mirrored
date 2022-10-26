using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class DamageInvoice:BaseModel
    {
        public DamageInvoice()
        {
            this.DamageInvoiceDetails = new HashSet<DamageInvoiceDetail>();
        }

        public System.DateTime InvoiceDate { get; set; }
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }
        public double TotalCostQuantityAmount { get; set; }
        [ForeignKey(nameof(ItemCostCalculation))]
        public Nullable<int> ItemCostCalculateId { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<DamageInvoiceDetail> DamageInvoiceDetails { get; set; }
        public virtual ItemCostCalculation ItemCostCalculation { get; set; }
        public virtual Store Store { get; set; }
    }
}
