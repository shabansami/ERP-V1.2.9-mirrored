using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.DataTablesDS
{
    public class PrintGeneralRecordDto
    {
        public PrintGeneralRecordDto()
        {
            AccountDebitList = new List<AccountDetails>();
            AccountCreditList = new List<AccountDetails>();
        }
        //بيانات الجهة 
        public string EntityDataName { get; set; }//اسم الجهة
        public string EntityCommercialRegisterNo { get; set; } //رقم السجل التجارى للجهة
        public string EntityDataTaxCardNo { get; set; }//رقم السجل الضريبى للجهة
        public string Logo { get; set; }//لوجو الجهة فى الطباعه
        public string QRCode { get; set; }//Qr code الفاتورة 

        public string RptTitle { get; set; }
        public string BranchName  { get; set; }
        public string OperationDate { get; set; }
        public string Notes { get; set; }
        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public bool IsDebitFirstRpt { get; set; } //يتم عرض الحسابات المدينة اولا فى اعيى التقرير ثم الدائنة
        public List<AccountDetails> AccountDebitList { get; set; }
        public List<AccountDetails> AccountCreditList { get; set; }
    }

    public class AccountDetails
    {
        public string AccountName { get; set; }
        public double Amount { get; set; }
    }
}