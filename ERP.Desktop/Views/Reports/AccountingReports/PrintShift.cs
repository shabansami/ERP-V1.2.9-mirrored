using ERP.DAL;
using ERP.Desktop.Views._Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Desktop.Utilities.CommonMethods;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using PrintEngine.HTMLPrint;
using ERP.Desktop.Services.Settings;
using ERP.Desktop.Views.Settings.Shift;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using ERP.Desktop.Services;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Views.Reports.AccountingReports
{
    public partial class PrintShift : BaseForm
    {
        ShiftServices _services;
        VTSaleEntities db;

        public PrintShift()
        {
            InitializeComponent();
            db = new VTSaleEntities();
            _services=new ShiftServices();
        }

        #region Load and fill
        private void PrintShift_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            dtpTo.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }
        #endregion

     

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {

            if (dtpFrom.Value > dtpTo.Value)
            {
                AlrtMsgs.ShowMessageError("الرجاء اخيار تواريخ صحيحة");
                dtpTo.Focus();
                return;
            }

            dgv_shift.AutoGenerateColumns = false;
            dgv_shift.DataSource = _services.GetAllClosedShifts(dtpFrom.Value, dtpTo.Value);
        }
        #endregion

        private void dgv_shift_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            Guid? id;
            if (e.ColumnIndex == dgv_shift.Columns[nameof(print)].Index)
            {
                id = Guid.Parse(dgv_shift.Rows[e.RowIndex].Cells[nameof(Shift_ID)].Value.ToString());
                printDgv(id);
            }
        }

        void printDgv(Guid? shiftId)
        {
            var shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftId);
            if (shift==null)
            {
                AlrtMsgs.ShowMessageError("خطأ فى رقم الورية");
                return;
            }
                PrintRpt(shift,true);
        }

        #region Print Report
        void PrintRpt(ShiftsOffline shift,bool isPreviewDialog = false )
        {
            Report report = new Report();

            report.ReportTitle = "تقرير وردية رقم " + shift.ShiftNumber;


            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();

            var sell = Math.Round(shift.SellInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy), 2, MidpointRounding.ToEven);
            var sellBack = Math.Round(shift.SellBackInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy), 2, MidpointRounding.ToEven) ;
            var purchase = Math.Round(shift.PurchaseInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy), 2, MidpointRounding.ToEven) ;
            var purchaseBack = Math.Round(shift.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy), 2, MidpointRounding.ToEven) ;
            var expense = Math.Round(shift.ExpenseIncomes.Where(x => !x.IsDeleted && x.IsExpense && x.IsApproval).Sum(x => x.Amount), 2, MidpointRounding.ToEven) + Math.Round(shift.ContractLoans.Where(x => !x.IsDeleted && x.IsApproval).Sum(x => x.Amount), 2, MidpointRounding.ToEven);
            var income = Math.Round(shift.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense && x.IsApproval).Sum(x => x.Amount), 2, MidpointRounding.ToEven);
            var GetSafy = Math.Round((sell + purchaseBack + income) - (sellBack + purchase + expense), 2, MidpointRounding.ToEven);
            
            var headerTextsTop = new List<(string, string)>();
            var headerTexts = new List<(string, string)>();
            headerTextsTop.Add(("رقم الوردية", shift.ShiftNumber));
            headerTextsTop.Add(("نقطة البيع", shift.PointOfSale.Name));
            headerTextsTop.Add(("تاريخ الوردية", shift.Date.ToString("yyyy/MM/dd")));
            headerTextsTop.Add(("الكاشير", shift.Employee.Person.Name));
            report.AddStrings(headerTextsTop, 1, StringLocation.AfterTitle, true, 12);
            headerTexts.Add(("عدد فواتير المبيعات", shift.SellInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + ""));
            headerTexts.Add(("اجمالي المبيعات", sell + ""));
            headerTexts.Add(("عدد فواتير مرتجع المبيعات", shift.SellBackInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + ""));
            headerTexts.Add(("اجمالي مرتجع المبيعات", sellBack + ""));
            headerTexts.Add(("عدد فواتير التوريد", shift.PurchaseInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + ""));
            headerTexts.Add(("اجمالي التوريد", purchase + ""));
            headerTexts.Add(("عدد فواتير مرتجع التوريد", shift.PurchaseBackInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + ""));
            headerTexts.Add(("اجمالي مرتجع التوريد",purchaseBack + ""));
            headerTexts.Add(("اجمالي المصروفات", expense + "")); //===========
            headerTexts.Add(("اجمالي الايرادات", income + ""));
            headerTexts.Add(("صافى الوردية", GetSafy.ToString()));//==========
            headerTexts.Add(("تاريخ اغلاق الفاتورة", shift.ClosedOn.Value.ToString("yyyy/MM/dd HH:mm")));

            //انواع الدفع 
            var sellInvoives = shift.SellInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval);
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.Cash).Any())
                headerTexts.Add(("اجمالى المبالغ النقدية", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Cash).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.Partial).Any())
                headerTexts.Add(("اجمالى المبالغ الجزئية", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Partial).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.Deferred).Any())
                headerTexts.Add(("اجمالى المبالغ الآجله", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Deferred).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.Installment).Any())
                headerTexts.Add(("اجمالى المبالغ التقسيط", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Installment).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.BankWallet).Any())
                headerTexts.Add(("اجمالى مبالغ المحافظ", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankWallet).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x=>x.PaymentTypeId==(int)PaymentTypeCl.BankCard).Any())
                headerTexts.Add(("اجمالى مبالغ الفيزا", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankCard).DefaultIfEmpty().Sum(x=>x.Safy),2,MidpointRounding.ToEven).ToString()));
            

            report.AddStrings(headerTexts, 1, StringLocation.AfterTitle, true, 12);

            if (isPreviewDialog && System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().Contains(Properties.Settings.Default.DefaultPrinter))
                report.Print(Properties.Settings.Default.DefaultPrinter);
            else
                report.ShowPrintPreviewDialog();
        }

        
        #endregion
    }

}
