using ERP.DAL;
using ERP.Desktop.Services;
using ERP.Desktop.Utilities;
using ERP.Desktop.Views._Main;
using ERP.Desktop.Views.Transactions.Purchases;
using ERP.Desktop.Views.Transactions.PurchasesBack;
using ERP.Desktop.Views.Transactions.Sales;
using ERP.Desktop.Views.Transactions.SalesBack;
using PrintEngine.HTMLPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;

namespace ERP.Desktop.Views.Settings.Shift
{
    public partial class ShiftClose : BaseForm
    {
        public bool IsShiftClosed { get; internal set; }
        Guid shiftID;
        VTSaleEntities db;
        ShiftsOffline shift;

        public ShiftClose(Guid shiftID) : base()
        {
            InitializeComponent();
            this.shiftID = shiftID;
            db = DBContext.UnitDbContext;
        }

        #region Load Shift Details
        private void ShiftClose_Load(object sender, EventArgs e)
        {
            shift = db.ShiftsOfflines.Where(x => !x.IsDeleted && x.Id == shiftID).FirstOrDefault();
            if (shift == null)
            {
                AlrtMsgs.ShowMessageError("خطأ في رقم الفاتروة");
                Close();
                return;
            }
            if (shift.IsClosed)
            {
                AlrtMsgs.ShowMessageError("الفاتورة مغلقة بالفعل");
                Close();
                return;
            }
            //Fill shift details
            lblCashier.Text = shift.Employee.Person.Name;
            lblPOS.Text = shift.PointOfSale.Name;
            lblShiftId.Text = shift.ShiftNumber + "";
            lblShiftDate.Text = shift.Date.ToString("yyyy/MM/dd");

            //expenses and employee loan
            lbl_expenses.Text = Math.Round(shift.ExpenseIncomes.Where(x => !x.IsDeleted && x.IsExpense && x.IsApproval).Sum(x => x.Amount),2,MidpointRounding.ToEven)+Math.Round(shift.ContractLoans.Where(x => !x.IsDeleted && x.IsApproval).Sum(x => x.Amount), 2, MidpointRounding.ToEven) + "";
            lbl_incomes.Text = Math.Round(shift.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense && x.IsApproval).Sum(x => x.Amount),2,MidpointRounding.ToEven) + "";
            FillSellInvoices(shift);

            FillSellBackInvoices(shift);

            FillPurchaseInvoices(shift);

            FillPurchaseBackInvoices(shift);

            lblSafy.Text = GetSafy().ToString();
        }


        private void FillSellInvoices(ShiftsOffline shift)
        {

            //fill sell invoices
            lblSellCount.Text = shift.SellInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + "";
            lblSellTotal.Text =Math.Round(shift.SellInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            //Open have items
            var invoices = shift.SellInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval);

            //var sellOpenCount = shift.SellInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            var sellOpenCount = invoices.Where(x => x.SellInvoicesDetails.Where(y => !y.IsDeleted).Count() >0).Count();
            lblSellOpenCount.Text = sellOpenCount + "";
            lblSellOpenTotal.Text =Math.Round(shift.SellInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            btnSellOpenShow.Visible = sellOpenCount > 0;

            //open no items
            var countNoItem = invoices.Where(x => x.SellInvoicesDetails.Where(y => !y.IsDeleted).Count() == 0).Count();
            lbl_invosNoItems.Text = countNoItem.ToString();
        }

        private void FillSellBackInvoices(ShiftsOffline shift)
        {

            //fill sell Back invoices
            lblSellBackCount.Text = shift.SellBackInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + "";
            lblSellBackTotal.Text =Math.Round(shift.SellBackInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            //Open
            var sellBackOpenCount = shift.SellBackInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            lblSellBackOpenCount.Text = sellBackOpenCount + "";
            lblSellBackOpenTotal.Text =Math.Round(shift.SellBackInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            btnSellBackOpenShow.Visible = sellBackOpenCount > 0;
        }

        private void FillPurchaseInvoices(ShiftsOffline shift)
        {
            //fill Purchase invoices
            lblPurchaseCount.Text = shift.PurchaseInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + "";
            lblPurchaseTotal.Text =Math.Round(shift.PurchaseInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            //Open
            var PurchaseOpenCount = shift.PurchaseInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            lblPurchaseOpenCount.Text = PurchaseOpenCount + "";
            lblPurchaseOpenTotal.Text =Math.Round(shift.PurchaseInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            btnPurchaseOpenShow.Visible = PurchaseOpenCount > 0;
        }
        private void FillPurchaseBackInvoices(ShiftsOffline shift)
        {

            //fill Purchase back invoices
            lblPurchaseBackCount.Text = shift.PurchaseBackInvoices.Count(x => !x.IsDeleted && x.IsFinalApproval) + "";
            lblPurchaseBackTotal.Text =Math.Round(shift.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            //Open
            var PurchaseBackOpenCount = shift.PurchaseBackInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            lblPurchaseBackOpenCount.Text = PurchaseBackOpenCount + "";
            lblPurchaseBackOpenTotal.Text =Math.Round(shift.PurchaseBackInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval).Sum(x => x.Safy),2,MidpointRounding.ToEven) + "";
            btnPurchaseBackOpenShow.Visible = PurchaseBackOpenCount > 0;
        }
        #endregion

        #region Open 
        private void btnSellOpenShow_Click(object sender, EventArgs e)
        {
            var form = new SellInvoicesCashier();
            form.currentShift = shift;
            form.currentPOS = shift.PointOfSale;
            form.IsOpenFromShiftCloseFrom = true;
            form.ShowDialog();
            shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftID);
            FillSellInvoices(shift);
        }

        private void btnSellBackOpenShow_Click(object sender, EventArgs e)
        {
            var form = new SellBackInvoicesCashier();
            form.currentShift = shift;
            form.currentPOS = shift.PointOfSale;
            form.IsOpenFromShiftCloseFrom = true;
            form.ShowDialog();
            shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftID);
            FillSellBackInvoices(shift);
        }

        private void btnPurchaseOpenShow_Click(object sender, EventArgs e)
        {
            var form = new PurchaseInvoicesCashier();
            form.currentShift = shift;
            form.currentPOS = shift.PointOfSale;
            form.IsOpenFromShiftCloseFrom = true;
            form.ShowDialog();
            shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftID);
            FillPurchaseInvoices(shift);
        }

        private void btnPurchaseBackOpenShow_Click(object sender, EventArgs e)
        {
            var form = new PurchaseBackInvoicesCashier();
            form.currentShift = shift;
            form.currentPOS = shift.PointOfSale;
            form.IsOpenFromShiftCloseFrom = true;
            form.ShowDialog();
            shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftID);
            FillPurchaseBackInvoices(shift);
        }
        #endregion

        #region Close The shift
        private void btnCloseShift_Click(object sender, EventArgs e)
        {
            shift = db.ShiftsOfflines.FirstOrDefault(x => !x.IsDeleted && x.Id == shiftID);

            //Validiate that there is no open invoices
            //حذف فواتير بدون الاصناف
            var invoices = shift.SellInvoices.Where(x => !x.IsDeleted && !x.IsFinalApproval).ToList();

            //var sellOpenCount = shift.SellInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            var sellOpenCount = invoices.Where(x => x.SellInvoicesDetails.Where(y => !y.IsDeleted).Count()> 0).Count();
            if (sellOpenCount > 0)
            {
                AlrtMsgs.ShowMessageError("لا يمكن غلق الوردية و هناك فواتير مبيعات مفتوحة\nالرجاء اغلاق جميع الفواتير المفتوحة اولاً");
                return;
            }
            var invoicesNoItems = invoices.Where(x => x.SellInvoicesDetails.Where(y => !y.IsDeleted).Count() == 0).ToList();
            foreach (var invo in invoicesNoItems)
            {
                //if(invo.SellInvoicesDetails.Where(x=>!x.IsDeleted).Count()==0)
                DeleteInvoicesNoItems(invo.Id);
            }

            var sellBackOpenCount = shift.SellBackInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            if (sellBackOpenCount > 0)
            {
                AlrtMsgs.ShowMessageError("لا يمكن غلق الوردية و هناك فواتير مرتجع مبيعات مفتوحة\nالرجاء اغلاق جميع الفواتير المفتوحة اولاً");
                return;
            }
            var PurchaseOpenCount = shift.PurchaseInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            if (PurchaseOpenCount > 0)
            {
                AlrtMsgs.ShowMessageError("لا يمكن غلق الوردية و هناك فواتير توريد مفتوحة\nالرجاء اغلاق جميع الفواتير المفتوحة اولاً");
                return;
            }
            var PurchaseBackOpenCount = shift.PurchaseBackInvoices.Count(x => !x.IsDeleted && !x.IsFinalApproval);
            if (PurchaseBackOpenCount > 0)
            {
                AlrtMsgs.ShowMessageError("لا يمكن غلق الوردية و هناك فواتير مرتجع توريد مفتوحة\nالرجاء اغلاق جميع الفواتير المفتوحة اولاً");
                return;
            }

            shift.IsClosed = true;
            shift.ClosedOn = CommonMethods.TimeNow;
            shift.ClosedBy = UserServices.UserInfo.UserId;
           var aff= db.SaveChanges(UserServices.UserInfo.UserId);
            if (aff>0)
            {
                if (ckPrintReport.Checked)
                {
                    PrintReport(true);
                }
                IsShiftClosed = true;
                MessageBox.Show("تم اغلاق الوردية بنجاح", "اغلاق وردية", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }else
            {
                AlrtMsgs.ShowMessageError("حدث خطأ اثناء تنفيذ العملية");
                return;
            }


        }

        void DeleteInvoicesNoItems(Guid invId)
        {
            var model = db.SellInvoices.Where(x => x.Id == invId).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                model.CaseId = (int)CasesCl.InvoiceDeleted;
                db.Entry(model).State = EntityState.Modified;

                var details = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == model.Id).ToList();
                //details.ForEach(x => x.IsDeleted = true);
                foreach (var detail in details)
                {
                    // حذف كل السيريالات الاصناف ان وجدت 
                    //var itemSerial = db.ItemSerials.Where(x => details.Any(y => y.ItemSerialId == x.Id)).ToList();
                    if (detail.ItemSerial != null)
                    {
                        var serial = detail.ItemSerial;

                        serial.IsDeleted = true;
                        db.Entry(serial).State = EntityState.Modified;
                        var itemSerialsHistories = db.CasesItemSerialHistories.Where(x => x.ItemSerialId == serial.Id).ToList();
                        foreach (var itemSerialHis in itemSerialsHistories)
                        {
                            itemSerialHis.IsDeleted = true;
                            db.Entry(itemSerialHis).State = EntityState.Modified;
                        }
                    }
                    detail.IsDeleted = true;
                    db.Entry(detail).State = EntityState.Modified;
                }

                var expenses = db.SellInvoiceIncomes.Where(x => x.SellInvoiceId == model.Id).ToList();
                //expenses.ForEach(x => x.IsDeleted = true);
                foreach (var expense in expenses)
                {
                    expense.IsDeleted = true;
                    db.Entry(expense).State = EntityState.Modified;
                }

                //حذف الحالات 
                var casesSellInvoiceHistories = db.CasesSellInvoiceHistories.Where(x => x.SellInvoiceId == model.Id).ToList();
                foreach (var cases in casesSellInvoiceHistories)
                {
                    cases.IsDeleted = true;
                    db.Entry(cases).State = EntityState.Modified;
                }
                //db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                //{
                //    SellInvoice = model,
                //    IsSellInvoice = true,
                //    CaseId = (int)CasesCl.InvoiceDeleted
                //});
                var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
                // حذف كل القيود 
                foreach (var generalDay in generalDalies)
                {
                    generalDay.IsDeleted = true;
                    db.Entry(generalDay).State = EntityState.Modified;
                }
                // حذف  اشعار الاستحقاق ان وجد 
                var notifications = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceDueDateClient).ToList();
                foreach (var notify in notifications)
                {
                    notify.IsDeleted = true;
                    db.Entry(notify).State = EntityState.Modified;
                }



                //db.SaveChanges(UserServices.UserInfo.UserId);
            }
        }
        #endregion

        #region Print Report
        protected override void PrintReport(bool isPreviewDialog = false)
        {
            Report report = new Report();

            report.ReportTitle = "تقرير وردية رقم " + lblShiftId.Text;


            report.FontSize = 12;
            //Generate the prepReport tables
            report.GenerateReport();

            //var sellPayed = Math.Round(shift.SellInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval && x.PayedValue > 0).Sum(x => x.Safy), 2, MidpointRounding.ToEven) + "";
            GetSafy();

            var headerTextsTop = new List<(string, string)>();
            var headerTexts = new List<(string, string)>();
            headerTextsTop.Add(("رقم الوردية", lblShiftId.Text));
            headerTextsTop.Add(("نقطة البيع", lblPOS.Text));
            headerTextsTop.Add(("تاريخ الوردية", lblShiftDate.Text));
            headerTextsTop.Add(("الكاشير", lblCashier.Text));
            report.AddStrings(headerTextsTop, 1, StringLocation.AfterTitle, true, 12);

            headerTexts.Add(("عدد فواتير المبيعات", lblSellCount.Text));
            headerTexts.Add(("اجمالي المبيعات", lblSellTotal.Text));
            headerTexts.Add(("عدد فواتير مرتجع المبيعات", lblSellBackCount.Text));
            headerTexts.Add(("اجمالي مرتجع المبيعات", lblSellBackTotal.Text));
            headerTexts.Add(("عدد فواتير التوريد", lblPurchaseCount.Text));
            headerTexts.Add(("اجمالي التوريد", lblPurchaseTotal.Text));
            headerTexts.Add(("عدد فواتير مرتجع التوريد", lblPurchaseBackCount.Text));
            headerTexts.Add(("اجمالي مرتجع التوريد", lblPurchaseBackTotal.Text));
            headerTexts.Add(("اجمالي المصروفات", lbl_expenses.Text)); //===========
            headerTexts.Add(("اجمالي الايرادات", lbl_incomes.Text));
            headerTexts.Add(("صافى الوردية", GetSafy().ToString()));//==========
            headerTexts.Add(("تاريخ اغلاق الفاتورة", CommonMethods.TimeNow.ToString("yyyy/MM/dd HH:mm")));
            //انواع الدفع 
            var sellInvoives = shift.SellInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval);
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Cash).Any())
                headerTexts.Add(("اجمالى المبالغ النقدية", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Cash).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Partial).Any())
                headerTexts.Add(("اجمالى المبالغ الجزئية", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Partial).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Deferred).Any())
                headerTexts.Add(("اجمالى المبالغ الآجله", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Deferred).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Installment).Any())
                headerTexts.Add(("اجمالى المبالغ التقسيط", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.Installment).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankWallet).Any())
                headerTexts.Add(("اجمالى مبالغ المحافظ", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankWallet).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));
            if (sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankCard).Any())
                headerTexts.Add(("اجمالى مبالغ الفيزا", Math.Round(sellInvoives.Where(x => x.PaymentTypeId == (int)PaymentTypeCl.BankCard).DefaultIfEmpty().Sum(x => x.Safy), 2, MidpointRounding.ToEven).ToString()));


            report.AddStrings(headerTexts, 1, StringLocation.AfterTitle, true, 12);

            if (isPreviewDialog && System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().Contains(Properties.Settings.Default.DefaultPrinter))
                report.Print(Properties.Settings.Default.DefaultPrinter);
            else
                report.ShowPrintPreviewDialog();
        }

        private double GetSafy()
        {
            double sell, sellBack, purchase, purchaseBack, expense, income;

            sell = double.Parse(lblSellTotal.Text);
            sellBack = double.Parse(lblSellBackTotal.Text);
            purchase = double.Parse(lblPurchaseTotal.Text);
            purchaseBack = double.Parse(lblPurchaseBackTotal.Text);
            expense = double.Parse(lbl_expenses.Text);
            income = double.Parse(lbl_incomes.Text);
            return Math.Round((sell + purchaseBack + income) - (sellBack + purchase + expense), 2, MidpointRounding.ToEven);
        }
        #endregion


    }
}
