using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class AccountTreeIntialBalance: BaseModel
    {
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(AccountsTree))]
        public Nullable<Guid> AccountTreeId { get; set; }
        public bool IsDebit { get; set; }//الحساب مدين/دائن
        public bool IsApproval { get; set; }
        public double Amount { get; set; }
        public Nullable<System.DateTime> OperationDate { get; set; }
        public string Notes { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual AccountsTree AccountsTree { get; set; }
    }
}
