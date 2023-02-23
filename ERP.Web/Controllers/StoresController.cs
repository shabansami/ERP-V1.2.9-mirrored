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

    public class StoresController : Controller
    {
        // GET: Stores
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public StoresController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Stores.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, EmployeeName = x.Employee!=null?x.Employee.Person.Name:null,IsDamages=x.IsDamages?"مخزن توالف":null, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Stores.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name",model.EmployeeId);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name",model.BranchId);

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;

                ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name");
                ViewBag.BranchId = new SelectList(branches, "Id", "Name",branchId);
                ViewBag.LastRow = db.Stores.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Store());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Store vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.BranchId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.Stores.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Stores.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.IsDamages = vm.IsDamages;
                    model.BranchId = vm.BranchId;
                    model.EmployeeId = vm.EmployeeId;
                    model.Address = vm.Address;
                    //حذف حساب المخزن من شجرة الحسابات فى حالة الجرد المستمر 
                    if (model.AccountTreeId != null)
                    {
                        var acount = db.AccountsTrees.Where(x => x.Id == model.AccountTreeId).FirstOrDefault();
                        if (acount != null)
                            acount.AccountName = vm.Name;
                    }


                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Stores.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //تحديد نوع الجرد
                    var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                    Guid accountTreeStockAccount;
                    if (int.TryParse(inventoryType, out int inventoryTypeVal))
                        accountTreeStockAccount = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                    else
                        return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                 
                    if (inventoryTypeVal==2)   // حالة الجرد المستمر
                    {
                        long newAccountNum = 0;// new account number 

                        var accountTree = db.AccountsTrees.Find(accountTreeStockAccount);
                        var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                        if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                            newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                        else
                            newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                        // add new account tree 
                        var newAccountTree = new AccountsTree
                        {
                            AccountLevel = accountTree.AccountLevel + 1,
                            AccountName = vm.Name,
                            AccountNumber = newAccountNum,
                            ParentId = accountTree.Id,
                            TypeId = (int)AccountTreeSelectorTypesCl.Operational,
                            SelectedTree = false
                        };
                        vm.AccountsTree = newAccountTree;
                    }
                    //============================================
                    db.Stores.Add(vm);

                    //db.Stores.Add(new Store
                    //{
                    //    Name = vm.Name,
                    //    IsDamages=vm.IsDamages,
                    //    BranchId = vm.BranchId,
                    //    EmployeeId = vm.EmployeeId,
                    //    Address = vm.Address
                    //});
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
                var model = db.Stores.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    // التاكد من عدم ارتبط المخزن  باى عمليات اخري 
                    var storeExistSetting = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.StoreDefault&&x.SValue==Id.ToString()).Any();
                    if (storeExistSetting)
                        return Json(new { isValid = false, message = "لا يمكن حذف المخزن لارتباطه بعمليات اخرى" });
                   if(StoreService.InvoicesHasStore(Id,db))
                        return Json(new { isValid = false, message = "لا يمكن حذف المخزن لارتباطه بعمليات اخرى" });

                    //حذف حساب المخزن من شجرة الحسابات فى حالة الجرد المستمر 
                    if (model.AccountTreeId != null)
                    {
                        var acount = db.AccountsTrees.Where(x => x.Id == model.AccountTreeId).FirstOrDefault();
                        if (acount != null)
                            acount.IsDeleted = true;
                        var geneDays = db.GeneralDailies.Where(x => !x.IsDeleted && x.AccountsTreeId == model.AccountTreeId).ToList();
                        foreach (var item in geneDays)
                        {
                            item.IsDeleted = true;
                        }
                    }


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