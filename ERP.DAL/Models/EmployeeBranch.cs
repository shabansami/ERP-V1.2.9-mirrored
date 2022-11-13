using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class EmployeeBranch : BaseModel
    {
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }
        [ForeignKey(nameof(Branch))]
        public Guid BranchId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Branch Branch { get; set; }
    }
}