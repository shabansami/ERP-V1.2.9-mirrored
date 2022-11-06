using ERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL
{
    public partial class ContractCustomerSupplier : BaseModel
    {
        [ForeignKey(nameof(PersonType))]
        public int PersonTypeId { get; set; }
        [ForeignKey(nameof(Customer))]
        public Nullable<Guid> CustomerId { get; set; }
        [ForeignKey(nameof(Supplier))]
        public Nullable<Guid> SupplierId { get; set; }
        public DateTime FromDate { get; set; } //بداية فترة التعاقد
        public DateTime ToDate { get; set; }//نهاية فترة التعاقد
        public int DayCount { get; set; } //عدد ايام الاستحقاق خلال الفترة 
        public virtual Person Customer { get; set; }
        public virtual Person Supplier { get; set; }
        public virtual PersonType PersonType { get; set; }
    }
}
