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
        [ForeignKey(nameof(AccountsTree))]
        public Nullable<Guid> AccountTreeId { get; set; } //الحساب المدين فى حاالة سند صرف والحساب الدائن فى حالة سند قبض
        public double Amount { get; set; }
        public string Notes { get; set; }

        public virtual Voucher Voucher { get; set; }

        //AccountTree1
        public virtual AccountsTree AccountsTree { get; set; }

    }
}
