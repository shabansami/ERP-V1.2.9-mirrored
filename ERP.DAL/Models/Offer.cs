using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DAL.Models
{
    public class Offer:BaseModel
    {
        public Offer()
        {
            this.OfferDetails = new HashSet<OfferDetail>();
            this.OfferSellInvoices = new HashSet<OfferSellInvoice>();
        }
        [ForeignKey(nameof(OfferType))]
        public int OfferTypeId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double AmountBefore { get; set; }//فى حالة نوع العرض (عرض على صنف/اكتر) القيمة الحقيقية للاصناف
        public double AmountAfter{ get; set; }//قيمة الاصناف بعد العرض 
        public int Limit { get; set; }
        [ForeignKey(nameof(Branch))]
        public Nullable<Guid> BranchId { get; set; }
        public string Notes { get; set; }
        /// <summary>
        /// اقل قيمة للفاتورة لتطبيق عليها العرض --- مطلوب تغير تسميتها
        /// </summary>
        public double InvoiceAmountFrom { get; set; }//فى حالة نوع العرض (عرض على فاتورة) قيمة الفاتورة من 
        /// <summary>
        /// قيمة غير مستخدمة في المعالجة --- مطلوب ازالتها
        /// </summary>
        public double InvoiceAmountTo { get; set; }//قيمة الفاتورة الى 
        public double DiscountAmount { get; set; } //قيمة الخصم
        public double DiscountPercentage { get; set; }



        public virtual OfferType OfferType { get; set; }
        public virtual Branch Branch { get; set; }

        public virtual ICollection<OfferDetail> OfferDetails { get; set; }
        public virtual ICollection<OfferSellInvoice> OfferSellInvoices { get; set; }

    }
}
