using ERP.DAL.Models;
using ERP.DAL;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class CustomerIntialsController : Controller
    {
        // GET: CustomerIntials
        VTSaleEntities db;
        VTSAuth auth;
        StoreService storeService;
        public CustomerIntialsController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            var data = db.PersonIntialBalances.Where(x => !x.IsDeleted && x.IsCustomer).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CustomerName = x.Person != null ? x.Person.Name : null, Amount = x.Amount, OperationDate = x.OperationDate.ToString(), IsInstallment = x.Installments.Where(i => !i.IsDeleted).Any(), Actions = n, Num = n }).ToList();
            return Json(new
            {
                data = data
            }, JsonRequestBehavior.AllowGet); ;

        }
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
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = db.Branches.Where(x => !x.IsDeleted);
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };

            return View(new IntialBalanceVM() { DateIntial = Utility.GetDateTime() });
            //}
        }
        public ActionResult CreateEditIntial()
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = db.Branches.Where(x => !x.IsDeleted);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };
            var personIntialBalance = db.PersonIntialBalances.Where(x => !x.IsDeleted && x.IsCustomer);
            IntialBalanceListVM intialBalanceVM = new IntialBalanceListVM();
            intialBalanceVM.Persons = new List<IntialBalanceVM>();
            foreach (var item in db.Persons.Where(x => !personIntialBalance.Any(x2 => x.Id == x2.PersonId) && !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).OrderBy(x => x.CreatedOn).ToList())
            {
                intialBalanceVM.Persons.Add(new IntialBalanceVM { CustomerId = item.Id, PersonName = item.Name });
            }
            intialBalanceVM.DateIntial = Utility.GetDateTime();
            return View(intialBalanceVM);
            //}
        }
        [HttpPost]
        public JsonResult CreateEditIntial(IntialBalanceListVM intialBalanceList)
        {

            bool saved = false;
            bool isAnySaved = false;
            if (intialBalanceList.DateIntial == null || intialBalanceList.BranchId == null)
                return Json(new { isValid = false, message = "تأكد من اختيار الفرع وادخال تاريخ العملية" });

            foreach (var vm in intialBalanceList.Persons)
            {
                if (vm.Amount == 0 || vm.Amount == null  || vm.CustomerId == null || vm.DebitCredit == null)
                {
                    continue;
                }
                vm.BranchId = intialBalanceList.BranchId;
                vm.DateIntial = intialBalanceList.DateIntial;

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                var customer = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.CustomerId).FirstOrDefault();
                if (db.PersonIntialBalances.Where(x => !x.IsDeleted && x.IsCustomer && x.PersonId == vm.CustomerId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للعميل من قبل" });
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
                    if (saved)
                    {
                        isAnySaved = true;
                        continue;
                    }
                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
            }
            if (isAnySaved)
            {
                return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
            }
            else
            {
                return Json(new { isValid = false, message = "لم يتم الاضافة  بنجاح" });

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
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

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
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
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