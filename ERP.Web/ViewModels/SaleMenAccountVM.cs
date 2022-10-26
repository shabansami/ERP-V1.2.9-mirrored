using ERP.Web.DataTablesDS;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SaleMenAccountVM
    {
        public SaleMenAccountVM()
        {
            SellInvoices = new List<RptInvoiceDto>();
            SellBackInvoices = new List<RptInvoiceDto>();
            CustomerPayments = new List<RptInvoiceDto>();
            Cheques = new List<RptInvoiceDto>();
        }
        public Guid? DepartmentId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string InvoiceNum { get; set; }
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
        public double CommissionPayment { get; set; } //العمولة على التحصيل
        public double CommissionSell { get; set; } //العمولة على البيع
        public double CommissionSellBack { get; set; } //العمولة على مرتجع البيع
    }
}