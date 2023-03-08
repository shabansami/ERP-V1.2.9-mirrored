using ERP.DAL.Models;
using ERP.DAL;
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
using ERP.Web.Identity;
using System.Windows.Documents;
using ERP.Web.DataTablesDS;
using System.Runtime.Remoting.Contexts;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class SupplierIntialsController : Controller
    {
        // GET: SupplierIntials
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public SupplierIntialsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            //بيانات الفرع/اكتر المحددة لليوزر
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            var list = db.PersonIntialBalances.Where(x => !x.IsDeleted && !x.IsCustomer);
            var branchesList = list.ToList().Where(x => branches.Any(b => b.Id == x.BranchId)).ToList();
           
            return Json(new
            {
                data = branchesList.OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, SupplierName = x.Person != null ? x.Person.Name : null, Amount = x.Amount, IsApproval=x.IsApproval, OperationDate = x.OperationDate.Value.ToString("yyyy-MM-dd"), Actions = n, Num = n }).ToList()
        }, JsonRequestBehavior.AllowGet); ;

        }

        #region     اضافة رصيد اول المدة للمورد 
        [HttpGet]
        public ActionResult CreateEdit()
        {
          
            IntialBalanceVM vm = new IntialBalanceVM();
            var debitCreditList = new List<DropDownListInt> { new DropDownListInt { Name = "مدين", Id = 1 }, new DropDownListInt { Name = "دائن", Id = 2 } };
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.PersonIntialBalances.FirstOrDefault(x => x.Id == id);
                    vm = new IntialBalanceVM()
                    {
                        Id=model.Id,
                        BranchId=model.BranchId,
                        SupplierId=model.PersonId,
                        Amount = model.Amount,
                        DateIntial=model.OperationDate,
                        DebitCredit=model.IsDebit?1:2,
                        Notes=model.Notes,
                    };
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                // add
            var defaultStore = storeService.GetDefaultStore(db);
            vm.BranchId = defaultStore != null ? defaultStore.BranchId : null;
            vm.DateIntial = Utility.GetDateTime();
            vm.DebitCredit = 2;
            }
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.Branchcount = branches.Count();
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name",vm.SupplierId);
            ViewBag.DebitCredit = new SelectList(debitCreditList, "Id", "Name", vm.DebitCredit);
            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.BranchId == null || vm.DateIntial == null || vm.SupplierId == null || vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
               var dateIntial = vm.DateIntial.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                var isInsert = false;
                if (vm.Id!=Guid.Empty)
                {
                    if (db.PersonIntialBalances.Where(x => !x.IsDeleted && !x.IsCustomer && x.Id != vm.Id && x.PersonId == vm.SupplierId).Count() > 0)
                        return Json(new { isValid = false, message = "تم تسجيل رصيد للمورد من قبل" });
                    var model=db.PersonIntialBalances.Where(x=>x.Id==vm.Id).FirstOrDefault();
                    if (model != null)
                    {
                        model.BranchId = vm.BranchId;
                        model.Amount = vm.Amount??0;
                        model.PersonId = vm.SupplierId;
                        model.OperationDate= dateIntial;
                        model.Notes = vm.Notes;
                        model.IsDebit = vm.DebitCredit == 1 ? true : false;

                    } else
                        return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                {
                if (db.PersonIntialBalances.Where(x => !x.IsDeleted && !x.IsCustomer && x.PersonId == vm.SupplierId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للمورد من قبل" });
                isInsert = true;
                   db.PersonIntialBalances.Add( new PersonIntialBalance
                    {
                        PersonId = vm.SupplierId,
                        BranchId = vm.BranchId,
                        Amount = vm.Amount ?? 0,
                        OperationDate = dateIntial,
                        Notes = vm.Notes,
                        IsCustomer=false,
                        IsDebit=vm.DebitCredit==1?true:false,
                    });
                }
                if (db.SaveChanges(auth.CookieValues.UserId)>0)
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
                var supptInial = db.PersonIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                supptInial.IsDeleted = true;
                db.Entry(supptInial).State = EntityState.Modified;

                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceSupplier).ToList();
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

        #endregion

        #region اعتماد وفك الاعتماد 
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
                    var supplierInital = db.PersonIntialBalances.FirstOrDefault(x => x.Id == Id);
                    var supplier = db.Persons.Where(x => x.Id == supplierInital.PersonId).FirstOrDefault();
                    if (supplier==null)
                        return Json(new { isValid = false, message = "تأكد من وجود حساب للمورد" });

                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(supplier.AccountTreeSupplierId))
                        return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                    if (supplierInital.IsDebit) // الرصيد مدين
                    {
                        // حساب المورد
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = supplier.AccountTreeSupplierId,
                            Debit = supplierInital.Amount,
                            Notes = $"رصيد أول المدة للمورد : {supplier.Name} , {supplierInital.Notes}",
                            BranchId = supplierInital.BranchId,
                            TransactionDate = supplierInital.OperationDate,
                            TransactionId = supplierInital.Id,
                            TransactionShared = supplierInital.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSupplier
                        });
                        //رأس المال 
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                            Credit = supplierInital.Amount,
                            BranchId = supplierInital.BranchId,
                            Notes = $"رصيد أول المدة للمورد : {supplier.Name} , {supplierInital.Notes}",
                            TransactionDate = supplierInital.OperationDate,
                            TransactionId = supplierInital.Id,
                            TransactionShared = supplierInital.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSupplier
                        });
                    }
                    else if (!supplierInital.IsDebit) //الرصيد دائن
                    {
                        // حساب المورد
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = supplier.AccountTreeSupplierId,
                            Credit = supplierInital.Amount,
                            Notes = $"رصيد أول المدة للمورد : {supplier.Name} , {supplierInital.Notes}",
                            BranchId = supplierInital.BranchId,
                            TransactionDate = supplierInital.OperationDate,
                            TransactionId = supplierInital.Id,
                            TransactionShared = supplierInital.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSupplier
                        });
                        //رأس المال 
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                            Debit = supplierInital.Amount,
                            BranchId = supplierInital.BranchId,
                            Notes = $"رصيد أول المدة للمورد : {supplier.Name} , {supplierInital.Notes}",
                            TransactionDate = supplierInital.OperationDate,
                            TransactionId = supplierInital.Id,
                            TransactionShared = supplierInital.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceSupplier
                        });
                    }


                    //تحديث حالة الاعتماد 
                    supplierInital.IsApproval = true;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = $"تم اعتماد رصيد اول للمورد {supplier.Name} بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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
                var model = db.PersonIntialBalances.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceSupplier).ToList();
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

        #endregion
        #region  اضافة رصيد أول المدة للموردين مجمع
        public ActionResult CreateEditIntial()
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.Branchcount = branches.Count();
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1" }, new SelectListItem { Text = "دائن", Value = "2", Selected = true } };
            var personIntialBalance = db.PersonIntialBalances.Where(x => !x.IsDeleted && !x.IsCustomer);
            IntialBalanceListVM intialBalanceVM = new IntialBalanceListVM();
            intialBalanceVM.Persons = new List<IntialBalanceVM>();
            foreach (var item in db.Persons.Where(x => !personIntialBalance.Any(x2 => x.Id == x2.PersonId) && !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)).OrderBy(x => x.CreatedOn).ToList())
            {
                intialBalanceVM.Persons.Add(new IntialBalanceVM { SupplierId = item.Id, PersonName = item.Name });
            }
            intialBalanceVM.DateIntial = Utility.GetDateTime();
            return View(intialBalanceVM);
            //}
        }
        [HttpPost]
        public JsonResult CreateEditIntial(IntialBalanceListVM intialBalanceList)
        {
            if (intialBalanceList.DateIntial == null || intialBalanceList.BranchId == null)
                return Json(new { isValid = false, message = "تأكد من اختيار الفرع وادخال تاريخ العملية" });

            foreach (var vm in intialBalanceList.Persons)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.SupplierId == null || vm.DebitCredit == null)
                {
                    continue;
                }
                vm.BranchId = intialBalanceList.BranchId;
                var supplier = db.Persons.Where(x => !x.IsDeleted && x.Id == vm.SupplierId).FirstOrDefault();
                if (db.PersonIntialBalances.Where(x => !x.IsDeleted && !x.IsCustomer && x.PersonId == vm.SupplierId).Count() > 0)
                    return Json(new { isValid = false, message = "تم تسجيل رصيد اول المدة للمورد من قبل" });

                var dateIntial = intialBalanceList.DateIntial.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                db.PersonIntialBalances.Add(new PersonIntialBalance
                {
                    PersonId = vm.SupplierId,
                    BranchId = vm.BranchId,
                    Amount = vm.Amount ?? 0,
                    OperationDate = dateIntial,
                    Notes = vm.Notes,
                    IsCustomer = false,
                    IsDebit = vm.DebitCredit == 1 ? true : false,
                });
            
            }
            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                return Json(new { isValid = true, message = "تم الاضافة بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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