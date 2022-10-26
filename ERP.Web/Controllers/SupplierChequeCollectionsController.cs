using ERP.DAL;
using ERP.Web.Services;
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
    public class SupplierChequeCollectionsController : Controller
    {
        // GET: SupplierChequeCollections
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            return View();
        }

        #region تحصيل شيك
        public ActionResult ChequeCollection()
        {
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var cheque = db.Cheques.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefault();
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted && x.Id == cheque.BankAccountId).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", cheque.BankAccountId);
                    cheque.CollectionDate = Utility.GetDateTime();
                    cheque.Notes = "تم تحصيل الشيك ";

                    return View(cheque);
                }
                else
                    return RedirectToAction("Index", "SupplierCheques");
            }
            else
                return RedirectToAction("Index", "SupplierCheques");

        }

        [HttpPost]
        public JsonResult ChequeCollection(Cheque vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.CollectionDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال تاريخ التحصيل" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                var model = db.Cheques.FirstOrDefault(x=>x.Id==vm.Id);
                model.CollectionDate = vm.CollectionDate;
                model.IsCollected = true;
                db.Entry(model).State = EntityState.Modified;

                //تسجيل القيود
                List<GeneralSetting> generalSetting;
                //التأكد من عدم وجود حساب فرعى من الحسابات المستخدمة
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من حساب العميل
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.PersonSupplier.AccountTreeSupplierId))
                        return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                    // التأكد من عدم وجود حساب فرعى من الحساب البنك
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.BankAccount.AccountsTreeId))
                        return Json(new { isValid = false, message = "حساب البنك ليس بحساب فرعى" });

                    //    //تسجيل القيود
                    // General Dailies
                    // من حـ/شيكات تحت التحصيل  الى حـ البنك/ 
                    // حساب  البنك
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = model.BankAccount.AccountsTreeId,
                        Credit = model.Amount,
                        BranchId = model.BranchId,
                        Notes = vm.Notes,
                        TransactionDate = vm.CollectionDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.ChequeOut
                    });

                    // حساب  شيكات تحت التحصيل
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue),
                        Debit = model.Amount,
                        BranchId = model.BranchId,
                        Notes = vm.Notes,
                        TransactionDate = vm.CollectionDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.ChequeOut
                    });

                    // غلق  اشعار الاستحقاق ان وجد 
                    var notify = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id && x.NotificationTypeId == (int)NotificationTypeCl.ChequesSuppliers).FirstOrDefault();
                    if (notify != null)
                    {
                        notify.IsClosed = true;
                        db.Entry(notify).State = EntityState.Modified;
                    }

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم التحصيل بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }
        public ActionResult Collection(string id)
        {
            Guid Id;
            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index", "SupplierCheques");

            TempData["model"] = Id;
            return RedirectToAction("ChequeCollection");

        }

        #endregion
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