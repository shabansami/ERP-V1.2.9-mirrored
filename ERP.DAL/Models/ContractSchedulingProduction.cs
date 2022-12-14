using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class ContractSchedulingProduction:BaseModel
    {
        [ForeignKey(nameof(ContractScheduling))]
        public Guid ContractSchedulingId { get; set; }
        [ForeignKey(nameof(ProductionOrder))]
        public Guid ProductionOrderId { get; set; }

        public ContractScheduling ContractScheduling { get; set; }
        public ProductionOrder ProductionOrder { get; set; }

    }
}
