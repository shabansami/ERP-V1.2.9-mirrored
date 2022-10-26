using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
   public class PersonIntialBalance : BaseModel
    {
        public PersonIntialBalance()
        {
            this.Installments = new HashSet<Installment>();
        }

        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(Person))]
        public Nullable<Guid> PersonId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsDebit { get; set; }
        public double Amount { get; set; }
        public Nullable<System.DateTime> OperationDate { get; set; }
        public string Notes { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual Person Person { get; set; }

        public virtual ICollection<Installment> Installments { get; set; }
    }
}
