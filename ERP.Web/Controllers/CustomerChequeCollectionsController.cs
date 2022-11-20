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

    public class CustomerChequeCollectionsController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        // GET: CustomerChequeCollections
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
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted&&x.Id==cheque.BankAccountId).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName",cheque.BankAccountId);
                    cheque.CollectionDate = Utility.GetDateTime();
                    cheque.Notes = "تم تحصيل الشيك ";

                    return View(cheque);
                }
                else
                    return RedirectToAction("Index", "CustomerCheques");
            }
            else
                return RedirectToAction("Index", "CustomerCheques");

        }

        [HttpPost]
        public JsonResult ChequeCollection(Cheque vm)
        {
            if (ModelState.IsValid)
            {
                if ( vm.CollectionDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال تاريخ التحصيل" });

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
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.PersonCustomer.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                        // التأكد من عدم وجود حساب فرعى من الحساب البنك
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(model.BankAccount.AccountsTreeId))
                            return Json(new { isValid = false, message = "حساب البنك ليس بحساب فرعى" });

                    //    //تسجيل القيود
                    // General Dailies
                    // من حـ/ البنك    الى حـ/ شيكات تحت التحصيل
                    // حساب  البنك
                    db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.BankAccount.AccountsTreeId,
                            Debit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = vm.Notes,
                            TransactionDate = vm.CollectionDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.ChequeIn
                        });
                 
                        // حساب  شيكات تحت التحصيل
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue),
                            Credit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = vm.Notes,
                            TransactionDate = vm.CollectionDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.ChequeIn
                        });

                    // غلق  اشعار الاستحقاق ان وجد 
                    var notify = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id && x.NotificationTypeId == (int)NotificationTypeCl.ChequesClients).FirstOrDefault();
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
                return RedirectToAction("Index", "CustomerCheques");

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