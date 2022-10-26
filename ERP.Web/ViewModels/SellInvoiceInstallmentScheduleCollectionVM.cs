using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SellInvoiceInstallmentScheduleCollectionVM //تستخدم لعرض الاقساط الغير محصلة/محلة فى موعدها/المحصلة بعد موعدها متأخرة
    {
        public SellInvoiceInstallmentScheduleCollectionVM()
        {
            PaidInstallments = new List<SellInvoiceInstallmentScheduleDto>();
            PaidNotInTimeInstallments = new List<SellInvoiceInstallmentScheduleDto>();
            NotPaidInstallments = new List<SellInvoiceInstallmentScheduleDto>();
        }

        public Nullable<Guid> SellInvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public double Safy { get; set; } //صافى الفاتورة 
        public double RemindValue { get; set; }
        public double TotalValue { get; set; }//المبلغ الاجمالى عبارة عن المتبقى والعمولة
        public int Duration { get; set; }


        public List<SellInvoiceInstallmentScheduleDto> PaidInstallments { get; set; } //تم تحصيلها 
        public List<SellInvoiceInstallmentScheduleDto> PaidNotInTimeInstallments { get; set; } //تم تحصيلها ولكن متاخرة 
        public List<SellInvoiceInstallmentScheduleDto> NotPaidInstallments { get; set; } //لم يتم تحصيلها 
    }

}