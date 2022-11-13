using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Utilites;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class CustomerIntialBalancesController : Controller
    {
        // GET: CustomerIntialBalances
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            // اسماء العملاء من جدول بيرسون
            var persons = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer));
            //العملاء من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل عميل عمليتين يتم تسجيلهم فى جدول القيود اليومية
            var generalGrouping = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem && persons.Any(p => p.AccountsTreeCustomerId == x.TransactionId && !p.IsDeleted)).Select(x => new { TransactionId = x.TransactionId, CustomerName = persons.Where(p => p.AccountsTreeCustomerId == x.TransactionId).FirstOrDefault().Name, Amount = x.Credit + x.Debit, CreatedOn =x.TransactionDate.ToString().Substring(0,11), Actions = n, Num = n }).GroupBy(x => new { x.TransactionId, x.CustomerName, x.Num, x.Actions, x.Amount, x.CreatedOn }).ToList();
            var data = generalGrouping.Select(x => new
            {
                TransactionId = x.FirstOrDefault().TransactionId,
                CustomerName = x.FirstOrDefault().CustomerName,
                Amount = x.FirstOrDefault().Amount,
                CreatedOn = x.FirstOrDefault().CreatedOn,
                Actions = n,
                Num = n
            }).ToList();
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //if (TempData["model"] != null) //edit
            //{
            //    int id;
            //    if (int.TryParse(TempData["model"].ToString(), out id))
            //    {
            //        var model = db.GeneralDailies.FirstOrDefault(x=>x.Id==id);
            //        ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", model.TransactionId);
            //        return View(model);
            //    }
            //    else
            //        return RedirectToAction("Index");
            //}
            //else
            //{                   // add
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text="مدين",Value="1",Selected=true},new SelectListItem { Text="دائن",Value="2"} };

            return View(new IntialBalanceVM() { DateIntial = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null|| vm.BranchId == null|| vm.DateIntial == null || vm.CustomerId == null|| vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                //if (vm.Id != Guid.Empty)
                //{
                //    if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem && x.TransactionId != vm.CustomerId).Count() > 0)
                //        return Json(new { isValid = false, message = "تم تسجيل رصيد للمورد من قبل" });

                //    var oldGeneralDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem && x.TransactionId == vm.CustomerId).ToList();
                //    foreach (var item in oldGeneralDailies)
                //    {
                //        item.IsDeleted = true;
                //        db.Entry(item).State = EntityState.Modified;
                //    }

                //    //add new general dailies
                //    // الحصول على حسابات من الاعدادات
                //    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                //    AddGeneralDailies(vm, generalSetting);

                //}
                //else
                //{
                var accountTreeCust = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.CustomerId).FirstOrDefault().AccountsTreeCustomerId;
                if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem && x.TransactionId == accountTreeCust).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للعميل من قبل" });
                isInsert = true;
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });

                    AddGeneralDailies(vm, generalSetting, accountTreeCust);
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                //}
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    if (isInsert)
                        return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
                    else
                        return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });

                }
                else
                    return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        void AddGeneralDailies(IntialBalanceVM vm, List<GeneralSetting> generalSetting, Guid? accountTreeCust)
        {
            DateTime dt = new DateTime();
            dt = vm.DateIntial.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
            if (vm.DebitCredit==1) // الرصيد مدين
            {
                // حساب العميل
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = accountTreeCust,
                    Debit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة للعميل : {db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault().Name}",
                    BranchId = vm.BranchId,
                    TransactionDate = dt,
                    TransactionId = accountTreeCust,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                });
                //رأس المال 
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                    Credit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة للعميل : {db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault().Name}",
                    BranchId = vm.BranchId,
                    TransactionDate = dt,
                    TransactionId = accountTreeCust,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                });
            }else if(vm.DebitCredit==2) //الرصيد دائن
            {
                // حساب العميل
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = accountTreeCust,
                    Credit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة للعميل : {db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault().Name}",
                    BranchId = vm.BranchId,
                    TransactionDate = dt,
                    TransactionId = accountTreeCust,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                });
                //رأس المال 
                db.GeneralDailies.Add(new GeneralDaily
                {
                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                    Debit = vm.Amount ?? 0,
                    Notes = $"رصيد أول المدة للعميل : {db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault().Name}",
                    BranchId = vm.BranchId,
                    TransactionDate = dt,
                    TransactionId = accountTreeCust,
                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                });
            }

        }
        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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