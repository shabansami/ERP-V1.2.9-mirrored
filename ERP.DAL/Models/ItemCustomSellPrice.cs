using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class ItemCustomSellPrice : BaseModel
    {
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        [ForeignKey(nameof(Group))]
        public Nullable<Guid> GroupBasicId { get; set; }
        [ForeignKey(nameof(Item))]
        public Nullable<Guid> ItemId { get; set; }
        public double ProfitPercentage { get; set; }


        public virtual Branch Branch { get; set; }
        public virtual Group Group { get; set; }
        public virtual Item Item { get; set; }
    }

}
