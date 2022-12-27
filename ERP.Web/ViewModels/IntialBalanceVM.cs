using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class IntialBalanceVM
    {
        public Guid Id { get; set; }
        public Guid? SupplierId { get; set; } //رقم  المورد 
        public Guid? CustomerId { get; set; } //رقم العميل  
        public string PersonName { get; set; } //اسم الشخص  
        public Guid? SafeId { get; set; } //رقم الخزنة  
        public Guid? BankAccountId { get; set; } //رقم الخزنة  
        public double? Amount { get; set; } = 0;// الرصيد 
        public Guid? BranchId { get; set; }
        public int? DebitCredit { get; set; } //حالة الرصيد للعميل والمورد (دائن/مدين) لعكس القيد

        public Guid? AccountId { get; set; }//مع شاشة ارصدة الاصول وحقوق الملكية
        public DateTime? DateIntial { get; set; }
        public string Notes { get; set; }
    }
}