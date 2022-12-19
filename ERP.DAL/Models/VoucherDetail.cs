using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class VoucherDetail:BaseModel
    {
        [ForeignKey(nameof(Voucher))]
        public Guid VoucherId { get; set; }
        [ForeignKey(nameof(AccountsTreeFrom))]
        public Nullable<Guid> AccountTreeFromId { get; set; }
        [ForeignKey(nameof(AccountsTreeTo))]
        public Nullable<Guid> AccountTreeToId { get; set; }
        public double Amount { get; set; }

        public virtual Voucher Voucher { get; set; }
        //AccountTree
        public virtual AccountsTree AccountsTreeFrom { get; set; }

        //AccountTree1
        public virtual AccountsTree AccountsTreeTo { get; set; }

    }
}
