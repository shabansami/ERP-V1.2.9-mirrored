using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class EmployeeSafe:BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }
        [ForeignKey(nameof(Safe))]
        public Guid SafeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Safe Safe { get; set; }

    }
}
