using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class PrintInvoiceDto
    {
        public PrintInvoiceDto()
        {
            ItemDetails = new List<ItemDetailsDT>();
        }
        //بيانات الجهة 
        public string EntityDataName { get; set; }//اسم الجهة
        public string EntityCommercialRegisterNo { get; set; } //رقم السجل التجارى للجهة
        public string EntityDataTaxCardNo { get; set; }//رقم السجل الضريبى للجهة
        public string Logo { get; set; }//لوجو الجهة فى الطباعه
        public string QRCode { get; set; }//Qr code الفاتورة 
        //بيانات الفاتورة 
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string SuppCustomerName { get; set; }
        public string SuppCustomerCommercialRegisterNo { get; set; } //رقم السجل التجارى للعميل 
        public string SuppCustomerTaxNo { get; set; }//رقم السجل الضريبى للعميل
        public string SuppCustomerAddress { get; set; }//عنوان العميل
        public string SuppCustomerAddress2 { get; set; }//عنوان التسليم
        public string BranchName { get; set; }
        public string PaymentTypeName { get; set; }
        public string SafeBankName { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalValue { get; set; }
        public double TotalIncomExpenses { get; set; }
        public double PayedValue { get; set; }
        public double RemindValue { get; set; }
        public double TotalDiscount { get; set; }
        public double Safy { get; set; }
        public double PersonBalance { get; set; } //مديونية العميل/المورد
        public double SalesTax { get; set; } //ضريبة قيمة مضافة بالزيادة على الفاتورة
        public double ProfitTax { get; set; }//ضريبة ارباح تجارية بنقص من الفاتورة 
        public string RptTitle { get; set; }
        public string PersonTypeName { get; set; }// مسمى العميل او المورد 
        public string Notes { get; set; }
        public string CustmerTaxNumber { get; set; }//الرقم الضريبى للعميل
        public string CustomerCommercialRegistrationNo { get; set; }//رقم السجل التجارى للعميل
        //تفاصيل الفاتورة 
        public List<ItemDetailsDT> ItemDetails { get; set; }
        public List<InvoiceExpensesDT> InvoiceExpenses { get; set; }

        public string LanguagePage { get; set; } //لغة التقرير (عربى//انجليزى)
        public string ToggleUrl { get; set; }//رابط التغيير بين اللغة العربية والانجليزية 
    }
    public class PrintPaymentDto
    {
        public Guid Id { get; set; }
        public string SaleMenName { get; set; }
        public string SellInvoiceId { get; set; }
        public string SuppCustomerName { get; set; }
        public string BranchName { get; set; }
        public string SafeBankName { get; set; }
        public double Amount { get; set; }
        public string PaymentDate { get; set; }
        public double TotalAmount { get; set; }
        public object PersonDebit { get; set; }
        public string Notes { get; set; }
        //بيانات الجهة 
        public string EntityDataName { get; set; }//اسم الجهة
        public string EntityCommercialRegisterNo { get; set; } //رقم السجل التجارى للجهة
        public string EntityDataTaxCardNo { get; set; }//رقم السجل الضريبى للجهة
        public string Logo { get; set; }//لوجو الجهة فى الطباعه
        public string QRCode { get; set; }//Qr code الفاتورة 

    }
}