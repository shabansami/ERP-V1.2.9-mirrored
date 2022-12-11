using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class ProductionLine:BaseModel
    {
        public ProductionLine()
        {
            this.ProductionLineEmployees=new HashSet<ProductionLineEmployee>();
            this.ProductionOrders = new HashSet<ProductionOrder>();
        }
        public string Name { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<ProductionLineEmployee> ProductionLineEmployees { get; set; }
        public virtual ICollection<ProductionOrder> ProductionOrders { get; set; }

    }
}
