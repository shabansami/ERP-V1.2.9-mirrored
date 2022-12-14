using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class ProductionLineEmployee:BaseModel
    {
        [ForeignKey(nameof(ProductionLine))]
        public Guid ProductionLineId { get; set; }
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }
        public bool IsProductionEmp { get; set; }//موظف بالانتاج ام لا 
        public bool CalculatingHours { get; set; }//احتساب عدد ساعات الانتاج لراتب الموظف ام لا
        public double HourlyWage { get; set; } //اجر الساعه
        public virtual Employee Employee { get; set; }
        public virtual ProductionLine ProductionLine { get; set; }
    }
}
