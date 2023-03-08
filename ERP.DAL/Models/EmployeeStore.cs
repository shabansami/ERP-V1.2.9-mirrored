using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class EmployeeStore : BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }
        [ForeignKey(nameof(Store))]
        public Guid StoreId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Store Store { get; set; }
    }
}
