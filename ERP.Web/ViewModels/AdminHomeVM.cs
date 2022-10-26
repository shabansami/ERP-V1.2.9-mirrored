using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class AdminHomeVM
    {
        public AdminHomeVM()
        {
            ApprovalInvoices = new List<ElementDetails>();
            LimitedItem = new List<ElementDetails>();
        }
        public int Customers { get; set; } // اجمالى العملاء 
        public int SellInvoicesTotal { get; set; } //اجمالى عدد فواتير البيع
        //public string dayName { get; set; } //اليوم
        public int SellInvoiceDay { get; set; }//اجمالى عدد فواتير البيع اليوم
        public int SellBackInvoices { get; set; }//اجمالى عدد فواتير مرتجع البيع
        //public int CustomerPaymentAmount { get; set; } // اجمالى النقدية المستلمة من العميل فى اليوم

        public int Suppliers { get; set; } // اجمالى الموردين
        public int PurchaseInvoices { get; set; } // اجمالى عدد فواتير التوريد
        public int PurchaseBackInvoices { get; set; } //اجمالى عدد فواتير مرتجع التوريد
        //public int SupplierPaymentAmount { get; set; } // اجمالى النقدية لمورد

        public int Items { get; set; }//عدد الاصناف
        public double ExpenseAmount { get; set; }//اجمالى مصروفات اليوم
        public double IncomeAmount { get; set; } // اجمالى الايرات اليوم
        public int Stores { get; set; } // عدد المخازن
        public int Employees { get; set; }//اجمالى الموظفين المسجلين
        public int ProductionOrders { get; set; }//اجمالى اوامر الانتاج 
        public int UploadFiles { get; set; }//اجمالى الملفات التى تم رفعها 
        public int Notifications { get; set; }//اجمالى الملفات التى تم رفعها 

        public List<ElementDetails> ApprovalInvoices { get; set; }//فواتير تحتاج اى اعتماد
        public List<ElementDetails> LimitedItem { get; set; }//اصناف تحت حد الامان والخطر 
    }

    public class ElementDetails
        {
        public string Name { get; set; }
        public int Count { get; set; }
        public string Url { get; set; }
    }
}