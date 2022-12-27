using ERP.DAL;
using ERP.DAL.Models;
using ERP.DAL.Utilites;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class AccountTreeIntialBalancesController : Controller
    {
        // GET: AccountTreeIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public AccountTreeIntialBalancesController()
        {
            db = new VTSaleEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            var list = db.AccountTreeIntialBalances.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, AccountTreeName = x.AccountsTree.AccountName != null , Amount = x.Amount, OperationDate = x.OperationDate.ToString(), IsApproval = x.IsApproval,DebitStatus=x.IsDebit?"مدين":"دائن", Actions = n, Num = n }).ToList();
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }
        public ActionResult CreateEdit()
        {
            IntialBalanceVM vm = new IntialBalanceVM();
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    vm = db.AccountTreeIntialBalances.Where(x => x.Id == id).Select(x => new IntialBalanceVM
                    {
                        Id= x.Id,
                        AccountId=x.AccountTreeId,
                        Amount= x.Amount,
                        DateIntial=x.OperationDate,
                        DebitCredit=x.IsDebit?1:2,
                        Notes= x.Notes,
                        BranchId=x.BranchId,
                    }).FirstOrDefault();
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                  // add
                vm.DateIntial = Utility.GetDateTime();
            }
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };
            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.BranchId == null || vm.DateIntial == null || vm.CustomerId == null || vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;

                if (vm.Id !=Guid.Empty)
                {
                    if (db.AccountTreeIntialBalances.Where(x => !x.IsDeleted && x.AccountTreeId == vm.AccountId && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "تم تسجيل رصيد اول للحساب مسبقا" });

                    var model = db.AccountTreeIntialBalances.FirstOrDefault(x => x.Id == vm.Id);
                    model.BranchId = vm.BranchId;
                    model.AccountTreeId = vm.AccountId;
                    model.OperationDate = vm.DateIntial;
                    model.Amount = vm.Amount??0;
                    model.Notes = vm.Notes;

                }
                else
                {
                    if (db.AccountTreeIntialBalances.Where(x => !x.IsDeleted && x.AccountTreeId == vm.AccountId).Count() > 0)
                        return Json(new { isValid = false, message = "تم تسجيل رصيد اول للحساب مسبقا" });
                    isInsert = true;
                    db.AccountTreeIntialBalances.Add(new AccountTreeIntialBalance
                    {
                        BranchId = vm.BranchId,
                        AccountTreeId = vm.AccountId,   
                        OperationDate=vm.DateIntial,
                        Amount=vm.Amount??0,
                        Notes=vm.Notes
                    }) ;
                }
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
                var accountTreeIntialBalance = db.AccountTreeIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                accountTreeIntialBalance.IsDeleted = true;

                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceAccountTree).ToList();
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        item.IsDeleted = true;
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
            [HttpPost]
            public ActionResult Approval(string id)
            {
                Guid Id;
                if (Guid.TryParse(id, out Id))
                {
                    //    //تسجيل القيود
                    // General Dailies
                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                    {
                        var model = db.AccountTreeIntialBalances.FirstOrDefault(x => x.Id == Id);
                        if (model != null)
                        {
                            // الحصول على حسابات من الاعدادات
                            var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                            if (AccountTreeService.CheckAccountTreeIdHasChilds(model.ExpenseIncomeTypeAccountTreeId))
                                return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });

                            //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(model.Safe.AccountsTreeId))
                                return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                            //التأكد من عدم تكرار اعتماد القيد
                            if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.Expense))
                                return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                            //من ح/المصروف 
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = model.ExpenseIncomeTypeAccountTreeId,
                                Debit = model.Amount,
                                BranchId = model.BranchId,
                                Notes = $"{model.Notes} مصروف من حساب {model.ExpenseIncomeTypeAccountsTree.AccountName}",
                                TransactionDate = model.PaymentDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Expense
                            });

                            // الى ح/ حساب الخزينة
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = model.Safe.AccountsTreeId,
                                Credit = model.Amount,
                                BranchId = model.BranchId,
                                Notes = $"{model.Notes} مصروف من حساب {model.ExpenseIncomeTypeAccountsTree.AccountName}",
                                TransactionDate = model.PaymentDate,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Expense
                            });

                            //تحديث حالة الاعتماد 
                            model.IsApproval = true;
                            db.Entry(model).State = EntityState.Modified;

                            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                                return Json(new { isValid = true, message = "تم اعتماد تسجيل المصروف بنجاح" });
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                        }
                        else
                            return Json(new { isValid = false, message = "تأكد من اختيار العملية " });

                    }
                    else
                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            [HttpPost]
            public ActionResult UnApproval(string id)
            {
                Guid Id;
                if (Guid.TryParse(id, out Id))
                {
                    var model = db.ExpenseIncomes.FirstOrDefault(x => x.Id == Id);
                    if (model != null)
                    {
                        //تحديث حالة الاعتماد 
                        model.IsApproval = false;
                        db.Entry(model).State = EntityState.Modified;
                        //حذف قيود اليومية
                        var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Expense).ToList();
                        if (generalDailies != null)
                        {
                            foreach (var item in generalDailies)
                            {
                                item.IsDeleted = true;
                                db.Entry(item).State = EntityState.Modified;
                            }
                        }
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم فك الاعتماد بنجاح" });
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
























[HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.BranchId == null || vm.DateIntial == null || vm.CustomerId == null || vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                bool saved = false;
                //if (vm.Id > 0)
                //{
                //    if (db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId != vm.CustomerId).Count() > 0)
                //        return Json(new { isValid = false, message = "تم تسجيل رصيد للمورد من قبل" });

                //    var oldGeneralDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && x.TransactionId == vm.CustomerId).ToList();
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
                var customer = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.CustomerId).FirstOrDefault();
                if (db.PersonIntialBalances.Where(x => !x.IsDeleted && x.IsCustomer && x.PersonId == vm.CustomerId).Count() > 0)
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
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(customer.AccountsTreeCustomerId))
                        return Json(new { isValid = false, message = "حساب العميل ليس بحساب فرعى" });

                    saved = AddGeneralDailies(vm, generalSetting, customer);
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

                //}
                if (saved)
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

        bool AddGeneralDailies(IntialBalanceVM vm, List<GeneralSetting> generalSetting, Person customer)
        {
            DateTime dt = new DateTime();
            bool IsDebit;
            dt = vm.DateIntial.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
            if (vm.DebitCredit == 1)
                IsDebit = true;
            else if (vm.DebitCredit == 2)
                IsDebit = false;
            else
                return false;
            using (var context = new VTSaleEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var customerInital = new PersonIntialBalance
                        {
                            PersonId = vm.CustomerId,
                            IsCustomer = true,
                            Amount = vm.Amount ?? 0,
                            BranchId = vm.BranchId,
                            IsDebit = IsDebit,
                            OperationDate = dt,
                            Notes = vm.Notes
                        };
                        context.PersonIntialBalances.Add(customerInital);
                        var aff = context.SaveChanges(auth.CookieValues.UserId);
                        if (aff == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        if (IsDebit) // الرصيد مدين
                        {
                            // حساب العميل
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                Debit = customerInital.Amount,
                                Notes = $"رصيد أول المدة للعميل : {customer.Name}",
                                BranchId = customerInital.BranchId,
                                TransactionDate = customerInital.OperationDate,
                                TransactionId = customerInital.Id,
                                TransactionShared = customerInital.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceCustomer
                            });
                            //رأس المال 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                                Credit = customerInital.Amount,
                                Notes = $"رصيد أول المدة للعميل : {customer.Name}",
                                BranchId = customerInital.BranchId,
                                TransactionDate = customerInital.OperationDate,
                                TransactionId = customerInital.Id,
                                TransactionShared = customerInital.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceCustomer
                            });
                        }
                        else if (!IsDebit) //الرصيد دائن
                        {
                            // حساب العميل
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = customer.AccountsTreeCustomerId,
                                Credit = customerInital.Amount,
                                Notes = $"رصيد أول المدة للعميل : {customer.Name}",
                                BranchId = customerInital.BranchId,
                                TransactionDate = customerInital.OperationDate,
                                TransactionId = customerInital.Id,
                                TransactionShared = customerInital.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceCustomer
                            });
                            //رأس المال 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                                Debit = customerInital.Amount,
                                Notes = $"رصيد أول المدة للعميل : {customer.Name}",
                                BranchId = customerInital.BranchId,
                                TransactionDate = customerInital.OperationDate,
                                TransactionId = customerInital.Id,
                                TransactionShared = customerInital.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceCustomer
                            });
                        }
                        context.SaveChanges(auth.CookieValues.UserId);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
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
                var custInial = db.PersonIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                custInial.IsDeleted = true;
                db.Entry(custInial).State = EntityState.Modified;

                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceCustomer).ToList();
                if (model != null)
                {
                    foreach (var item in model)
                    {
                        item.IsDeleted = true;
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