using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class ProductionType : BaseModelInt
    {
        public ProductionType()
        {
            ItemProduction = new HashSet<ItemProduction>();
            ItemProductionDetails = new HashSet<ItemProductionDetail>();
            ProductionOrderDetails = new HashSet<ProductionOrderDetail>();
        }
        public string Name { get; set; }
        public virtual ICollection<ItemProduction> ItemProduction { get; set; }

        public virtual ICollection<ItemProductionDetail> ItemProductionDetails { get; set; }
        public virtual ICollection<ProductionOrderDetail> ProductionOrderDetails { get; set; }
    }
}