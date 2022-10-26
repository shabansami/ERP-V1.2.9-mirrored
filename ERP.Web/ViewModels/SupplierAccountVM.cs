using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class SupplierAccountVM
    {
        public SupplierAccountVM()
        {
            PurchaseInvoices = new List<RptInvoiceDto>();
            PurchaseBackInvoices = new List<RptInvoiceDto>();
            SupplierPayments = new List<RptInvoiceDto>();
            Cheques = new List<RptInvoiceDto>();
        }
        public Guid? SupplierId { get; set; } //المورد 
        public string SupplierName { get; set; }//المورد 
        public string DtFrom { get; set; }
        public string DtTo { get; set; }
        public bool AllTimes { get; set; } //عرض الكل بدون نطاق زمنى
        public bool IsFinalApproval { get; set; } //الفواتير المعتمدة فقط 
        public List<RptInvoiceDto> PurchaseInvoices { get; set; } //فواتير التوريد
        public double TotalPurchaseInvoices { get; set; } // صافى فواتير التوريد
        public List<RptInvoiceDto> PurchaseBackInvoices { get; set; } //فواتير مرتجع التوريد
        public double TotalPurchaseBackInvoices { get; set; } //صافى مرتجع التوريد 
        public List<RptInvoiceDto> SupplierPayments { get; set; } //التحصيلات النقدية
        public double TotalSupplierPayments { get; set; } //اجمالى التحصيلات النقدية
        public List<RptInvoiceDto> Cheques { get; set; }  //الشيكات 
        public double TotalCheques { get; set; } // اجمالى الشيكات 
    }
}