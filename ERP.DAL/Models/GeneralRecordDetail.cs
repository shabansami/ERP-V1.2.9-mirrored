using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class GeneralRecordDetail:BaseModel
    {
        [ForeignKey(nameof(GeneralRecord))]
        public Guid GeneralRecordId { get; set; }
        [ForeignKey(nameof(AccountTree))]
        public Nullable<Guid> AccountTreeId { get; set; }
        public double Amount { get; set; }
        public bool IsDebit { get; set; } // debit(true), credit(false) حالة القيد مدين/دائن
        public string Notes { get; set; }
        public virtual GeneralRecord GeneralRecord { get; set; }
        public virtual AccountsTree AccountTree { get; set; }

    }
}
