using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PurchaseInvoiceStoresController : Controller
    {
        // GET: PurchaseInvoiceStores
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.PurchaseInvoices.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, IsApprovalStore = x.IsApprovalStore, ApprovalStore = x.IsApprovalStore ? "معتمده" : "غير معتمدة", CaseName = x.Case != null ? x.Case.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ApprovalStore(string invoGuid)
        {
            Guid guid;
            if (Guid.TryParse(invoGuid, out guid))
            {
                return View(db.PurchaseInvoices.Where(x => x.Id == guid).FirstOrDefault());
            }
                return RedirectToAction("Index");
        }

        //اعتماد الفاتور مخزنيا
        public ActionResult ApprovalInvoice(string invoGuid, string data)
        {
            List<ItemDetailsStor> itemDetailsStor = new List<ItemDetailsStor>();
            if (data != null)
                itemDetailsStor = JsonConvert.DeserializeObject<List<ItemDetailsStor>>(data);
            else
                return Json(new { isValid = false, message = "تأكد من وجود اصناف" });

            //if (itemDetailsStor.Sum(x => x.QuantityReal) > itemDetailsStor.Sum(x => x.Quantity))
            //    return Json(new { isValid = false, message = "تأكد من ادخال كميات اقل او تساوى الكمية الاصلية للاصناف" });
            // التأكد من عمدم ادخال صفر فى الكمية المستلمة 
            if (itemDetailsStor.Any(x=>x.QuantityReal==0))
                return Json(new { isValid = false, message = "تأكد من ادخال كميات مستلمة للاصناف" });

            foreach (var item in itemDetailsStor)
            {
                if (item.QuantityReal>item.Quantity)
                    return Json(new { isValid = false, message = "تأكد من ادخال كميات اقل او تساوى الكمية الاصلية للاصناف" });
            }
            //    تحديث الحالة
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.PurchaseInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //اضافة الحالة 
                    var casesPurchaseInvoiceHistory = new CasesPurchaseInvoiceHistory
                    {
                        PurchaseInvoice = model,
                        IsPurchaseInvoice = true,
                    };

                    foreach (var item in itemDetailsStor)
                    {
                        var purchaseDetails = db.PurchaseInvoicesDetails.Where(x => x.Id == item.Id).FirstOrDefault();
                        purchaseDetails.QuantityReal = item.QuantityReal;
                        db.Entry(purchaseDetails).State = EntityState.Modified;
                    }

                    if (itemDetailsStor.Sum(x => x.QuantityReal) != itemDetailsStor.Sum(x => x.Quantity))
                    {
                        model.CaseId = (int)CasesCl.InvoiceQuantityDiffReal;
                        model.IsApprovalStore = false;
                        model.IsApprovalAccountant = false;
                        db.Entry(model).State = EntityState.Modified;

                        casesPurchaseInvoiceHistory.CaseId = (int)CasesCl.InvoiceQuantityDiffReal;
                        db.CasesPurchaseInvoiceHistories.Add(casesPurchaseInvoiceHistory);
                        db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = false, isQuantitDiff=true, message = " لم يتم الاعتماد بسبب اختلاف الكميات..تم ارجاعها للمحاسب" });
                    }
                    else
                    {
                        model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                        model.IsApprovalStore = true;
                        db.Entry(model).State = EntityState.Modified;
                        casesPurchaseInvoiceHistory.CaseId = (int)CasesCl.InvoiceApprovalStore;
                        db.CasesPurchaseInvoiceHistories.Add(casesPurchaseInvoiceHistory);
                        db.SaveChanges(auth.CookieValues.UserId);
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                    }



                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            //    //تسجيل القيود
            //    Guid Id;
            //if (Guid.TryParse(invoGuid, out Id))
            //{
            //    var model = db.PurchaseInvoices.Where(x => x.InvoiceGuid == Id).FirstOrDefault();
            //    if (model != null)
            //    {
            //        if (TempData["userInfo"] != null)
            //            auth = TempData["userInfo"] as VTSAuth;
            //        else
            //            RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            //        // General Dailies
            //        if (Services.CheckGenralSettingValueAccountTree())
            //        {
            //            //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

            //            double debit = 0;
            //            double credit = 0;
            //            // حساب المشتريات
            //            var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue),
            //                BranchId = model.BranchId,
            //                Debit = model.TotalValue,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            debit = debit + model.TotalValue;
            //            // حساب المورد
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountsTreeId,
            //                BranchId = model.BranchId,
            //                Credit = model.Safy,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            credit = credit + model.Safy;

            //            // القيمة المضافة
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
            //                BranchId = model.BranchId,
            //                Debit = model.SalesTax,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            debit = debit + model.SalesTax;

            //            // الخصومات
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
            //                BranchId = model.BranchId,
            //                Credit = model.TotalDiscount,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            credit = credit + model.TotalDiscount;

            //            // ضريبة ارباح تجارية
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
            //                BranchId = model.BranchId,
            //                Credit = model.ProfitTax,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            credit = credit + model.ProfitTax;

            //            //  المبلغ المدفوع من حساب المورد (مدين
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = db.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountsTreeId,
            //                BranchId = model.BranchId,
            //                Debit = model.PayedValue,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            debit = debit + model.PayedValue;

            //            //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
            //            db.GeneralDailies.Add(new GeneralDaily
            //            {
            //                AccountsTreeId = model.SafeId != null ? model.Safe.AccountsTreeId : model.BankAccount.AccountsTreeId,
            //                BranchId = model.BranchId,
            //                Credit = model.PayedValue,
            //                Notes = $"فاتورة توريد رقم : {model.Id}",
            //                PaperNumber = model.InvoiceNumPaper,
            //                TransactionDate = Utility.GetDateTime(),
            //                TransactionId = model.Id,
            //                TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //            });
            //            credit = credit + model.PayedValue;

            //            // المصروفات (مدين
            //            var expenses = db.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == model.Id);
            //            foreach (var expense in expenses)
            //            {
            //                db.GeneralDailies.Add(new GeneralDaily
            //                {
            //                    AccountsTreeId = expense.ExpenseType.AccountsTreeId,
            //                    BranchId = model.BranchId,
            //                    Debit = expense.Amount,
            //                    Notes = $"فاتورة توريد رقم : {model.Id}",
            //                    PaperNumber = model.InvoiceNumPaper,
            //                    TransactionDate = Utility.GetDateTime(),
            //                    TransactionId = model.Id,
            //                    TransactionTypeId = (int)TransactionsTypesCl.Purchases
            //                });
            //                debit = debit + expense.Amount;

            //            }
            //            //التاكد من توازن المعاملة 
            //            if (debit == credit)
            //            {
            //                //حفظ القيود اعتماد المحاسب
            //                model.IsApprovalStore = true;
            //                db.Entry(model).State = EntityState.Modified;

            //                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
            //                {
            //                    return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });

            //                }
            //                else
            //                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            //            }
            //            else
            //            {
            //                return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
            //            }

            //        }
            //        else
            //            return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
            //    }
            //    else
            //        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            //}
            //else
            //    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        //Releases unmanaged resources and optionally releases managed resources.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}