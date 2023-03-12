using ERP.Web.Identity;
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
    [Authorization]

    public class IncomesController : Controller
    {
        // GET: Incomes
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ExpenseIncomeService expenseIncomeService;
        StoreService storeService;
        public IncomesController()
        {
            db = new VTSaleEntities();
            expenseIncomeService = new ExpenseIncomeService();
            storeService=new StoreService();    
        }
        public ActionResult Index()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion
            return View();
        }
        public ActionResult GetAll(string incomeTypeId, string isApprovalStatus, string dFrom, string dTo)
        {
            var list = expenseIncomeService.GetIncomes(db, incomeTypeId, isApprovalStatus, dFrom, dTo,auth);
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid guid;
                if (Guid.TryParse(TempData["model"].ToString(), out guid))
                {
                    var model = db.ExpenseIncomes.FirstOrDefault(x=>x.Id==guid);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    ViewBag.SafeId = new SelectList(EmployeeService.GetSafesByUser(model.BranchId.ToString(), auth.CookieValues.UserId.ToString()), "Id", "Name", model.SafeId);
                    //ViewBag.IncomeTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name", model.IncomeTypeId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");

            }
            else
            {                   // add

                //var defaultStore = storeService.GetDefaultStore(db);
                //var branchId = defaultStore != null ? defaultStore.BranchId : null;
                ViewBag.BranchId = new SelectList(branches, "Id", "Name");
                ViewBag.Branchcount = branches.Count();
                ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
                //ViewBag.IncomeTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.ExpenseIncomes.Where(x => !x.IsDeleted && !x.IsExpense).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new ExpenseIncome() { PaymentDate = Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ExpenseIncome vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ExpenseIncomeTypeAccountTreeId == null || vm.Amount == 0 || vm.BranchId == null || vm.PaymentDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد بشكل صحيح" });

                var isInsert = false;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseIncomeTypeAccountTreeId))
                    return Json(new { isValid = false, message = "حساب الايراد ليس بحساب فرعى" });
                if (vm.Id != Guid.Empty)
                {
                    //if (db.IncomeIncomes.Where(x => !x.IsDeleted && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.ExpenseIncomes.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Amount = vm.Amount;
                    model.Notes = vm.Notes;
                    model.BranchId = vm.BranchId;
                    model.SafeId = vm.SafeId;
                    model.PaymentDate = vm.PaymentDate;
                    model.PaidTo = vm.PaidTo;
                    model.ExpenseIncomeTypeAccountTreeId = vm.ExpenseIncomeTypeAccountTreeId;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    var model = new ExpenseIncome
                    {
                        BranchId = vm.BranchId,
                        SafeId = vm.SafeId,
                        Amount = vm.Amount,
                        Notes = vm.Notes,
                        PaymentDate = vm.PaymentDate,
                        PaidTo = vm.PaidTo,
                        ExpenseIncomeTypeAccountTreeId = vm.ExpenseIncomeTypeAccountTreeId,
                        IsExpense = false,
                    };
                    //            //اضافة رقم العملية
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    model.OperationNumber = codePrefix + (db.ExpenseIncomes.Count(x => model.OperationNumber.StartsWith(codePrefix) && !x.IsExpense) + 1);
                    db.ExpenseIncomes.Add(model);

                }
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
                var model = db.ExpenseIncomes.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
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
                    var model = db.ExpenseIncomes.FirstOrDefault(x=>x.Id==Id);
                    if (model != null)
                    {
                        // الحصول على حسابات من الاعدادات
                        var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

                        if (AccountTreeService.CheckAccountTreeIdHasChilds(model.ExpenseIncomeTypeAccountTreeId))
                            return Json(new { isValid = false, message = "حساب الايرادات ليس بحساب فرعى" });

                        //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(model.Safe.AccountsTreeId))
                            return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                        //التأكد من عدم تكرار اعتماد القيد
                        if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.Income))
                            return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                        // من ح/ حساب الخزينة
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.Safe.AccountsTreeId,
                            Debit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"{model.Notes} ايراد الى حساب {model.ExpenseIncomeTypeAccountsTree.AccountName}",
                            TransactionDate = model.PaymentDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Income
                        });

                        //الى ح/الايراد 
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.ExpenseIncomeTypeAccountTreeId,
                            Credit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"{model.Notes} ايراد الى حساب {model.ExpenseIncomeTypeAccountsTree.AccountName}",
                            TransactionDate = model.PaymentDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Income
                        });


                        //تحديث حالة الاعتماد 
                        model.IsApproval = true;
                        db.Entry(model).State = EntityState.Modified;

                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم اعتماد تسجيل الإيراد بنجاح" });
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
                var model = db.ExpenseIncomes.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Income).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
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