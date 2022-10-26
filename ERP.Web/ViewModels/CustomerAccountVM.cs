using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class CustomerAccountVM
    {
        public CustomerAccountVM()
        {
            SellInvoices = new List<RptInvoiceDto>();
            SellBackInvoices = new List<RptInvoiceDto>();
            CustomerPayments = new List<RptInvoiceDto>();
            Cheques = new List<RptInvoiceDto>();

            PaidInstallments = new List<SellInvoiceInstallmentScheduleDto>();
            PaidNotInTimeInstallments = new List<SellInvoiceInstallmentScheduleDto>();
            NotPaidInstallments = new List<SellInvoiceInstallmentScheduleDto>();

        }
        public Guid? CustomerId { get; set; }
        public string PesonBalance { get; set; }  //مديونية العميل/المورد

        public string CustomerName { get; set; }
        public string DtFrom { get; set; }
        public string DtTo { get; set; }
        public bool AllTimes { get; set; } //عرض الكل بدون نطاق زمنى
        public bool IsFinalApproval { get; set; } //الفواتير المعتمدة فقط 
        public List<RptInvoiceDto> SellInvoices { get; set; } //فواتير البيع
        public double TotalSellInvoices { get; set; } // صافى فواتير البيع
        public List<RptInvoiceDto> SellBackInvoices { get; set; } //فواتير مرتجع البيع
        public double TotalSellBackInvoices { get; set; } //صافى مرتجع البيع 
        public List<RptInvoiceDto> CustomerPayments { get; set; } //التحصيلات النقدية
        public double TotalCustomerPayments { get; set; } //اجمالى التحصيلات النقدية
        public List<RptInvoiceDto> Cheques { get; set; }  //الشيكات 
        public double TotalCheques { get; set; } // اجمالى الشيكات 
        public double TotalPaidInstallment { get; set; } // اجمالى اقساط تم تحصيلها 
        public double TotalPaidNotInTimeInstallment { get; set; } // اجمالى اقساط تم تحصيلها ولكن متاخرة 
        public double TotalNotPaidInstallment { get; set; } // اجمالى اقساط لم يتم تحصيلها 

        public List<SellInvoiceInstallmentScheduleDto> PaidInstallments { get; set; } //تم تحصيلها 
        public List<SellInvoiceInstallmentScheduleDto> PaidNotInTimeInstallments { get; set; } //تم تحصيلها ولكن متاخرة 
        public List<SellInvoiceInstallmentScheduleDto> NotPaidInstallments { get; set; } //لم يتم تحصيلها 


    }
}
