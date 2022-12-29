using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public partial class ItemIntialBalanceDetail:BaseModel
    {

        [ForeignKey(nameof(ItemIntialBalance))]
        public Nullable<Guid> ItemIntialBalanceId { get; set; } 
        [ForeignKey(nameof(AccountTree))]
        public Nullable<Guid> AccountTreeId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        [ForeignKey(nameof(Store))]
        public Nullable<Guid> StoreId { get; set; }

        public virtual AccountsTree AccountTree { get; set; }
        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
        public virtual ItemIntialBalance ItemIntialBalance { get; set; }
    }
}
