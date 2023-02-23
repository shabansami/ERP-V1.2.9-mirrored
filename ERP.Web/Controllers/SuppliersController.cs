using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using Newtonsoft.Json;
using ERP.Web.DataTablesDS;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class SuppliersController : Controller
    {
        // GET: Suppliers
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
            ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            //البحث من الصفحة الرئيسية
            string txtSearch = null;
            if (TempData["txtSearch"] != null)
            {
                txtSearch = TempData["txtSearch"].ToString();
                return Json(new
                {
                    data = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer) && (x.Name.Contains(txtSearch) || x.Mob1.Contains(txtSearch) || x.Mob2.Contains(txtSearch) || x.Tel.Contains(txtSearch))).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn = x.CreatedOn, CountryName = x.Area.City.Country.Name, CityName = x.Area.City.Name, AreaName = x.Area.Name, Name = x.Name, StatusType = x.PersonType.Name, typ = (int)UploalCenterTypeCl.Supplier, IsActive = x.IsActive, Actions = n, Num = n }).ToList(),
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
                return Json(new
                {
                    data = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn = x.CreatedOn, CountryName = x.Area.City.Country.Name, CityName = x.Area.City.Name, AreaName = x.Area.Name, Name = x.Name, StatusType = x.PersonType.Name, typ = (int)UploalCenterTypeCl.Supplier, IsActive = x.IsActive, Actions = n, Num = n }).ToList(),
                }, JsonRequestBehavior.AllowGet); ;



        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Persons.FirstOrDefault(x => x.Id == id);
                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", model.Area.City.Country.Id);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name", model.Area.City.Id);
                    ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name", model.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name", model.GenderId);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name", model.PersonCategoryId);
                    ViewBag.PersonTypeId = new SelectList(db.PersonTypes.Where(x => !x.IsDeleted && (x.Id == (int)PersonTypeCl.Supplier) || x.Id == (int)PersonTypeCl.SupplierAndCustomer), "Id", "Name", model.PersonTypeId);
                    var supplierResponsibles = db.Persons.Where(x => !x.IsDeleted && x.ParentId == id)
                       .Select(x => new ResponsibleDT
                       {
                           Id = x.Id,
                           ResponsibleName = x.Name,
                           ResponsibleMob = x.Mob1,
                           ResponsibleTransfer = x.Transfer
                       }).ToList();
                    DSSupplierResponsible = JsonConvert.SerializeObject(supplierResponsibles);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
                ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
                ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name", 1);
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && !x.IsCustomer), "Id", "Name");
                ViewBag.PersonTypeId = new SelectList(db.PersonTypes.Where(x => !x.IsDeleted && (x.Id == (int)PersonTypeCl.Supplier) || x.Id == (int)PersonTypeCl.SupplierAndCustomer), "Id", "Name", 1);
                ViewBag.LastRow = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Person());
            }
        }
        public static string DSSupplierResponsible { get; set; }
        public ActionResult GetDSSupplierResponsible()
        {
            int? n = null;
            if (DSSupplierResponsible == null)
                return Json(new
                {
                    data = new ResponsibleDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DSSupplierResponsible)
                }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AddSupplierResponsible(string ResponsibleName, string ResponsibleMob, string ResponsibleTel, string ResponsibleJob, string ResponsibleEmail, string ResponsibleTransfer, string DT_Datasource)
        {

            List<ResponsibleDT> deDS = new List<ResponsibleDT>();
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DT_Datasource);
            if (string.IsNullOrEmpty(ResponsibleName) || string.IsNullOrEmpty(ResponsibleMob))
            {
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات السمئول صحيحة" }, JsonRequestBehavior.AllowGet);
            }
            var SupplierResponsible = new ResponsibleDT { ResponsibleName = ResponsibleName, ResponsibleMob = ResponsibleMob, ResponsibleTel = ResponsibleTel, ResponsibleJob = ResponsibleJob, ResponsibleEmail = ResponsibleEmail, ResponsibleTransfer = ResponsibleTransfer };
            deDS.Add(SupplierResponsible);
            DSSupplierResponsible = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CreateEdit(Person vm, string DT_Datasource)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) || vm.AreaId == null || vm.PersonCategoryId == null || string.IsNullOrEmpty(vm.Mob1))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                bool? isSaved = false;

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                List<ResponsibleDT> deDS = new List<ResponsibleDT>();
                List<Person> SupplierResponsible = new List<Person>();

                if (DT_Datasource != null)
                {
                    deDS = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DT_Datasource);
                    SupplierResponsible = deDS.Select(x => new Person
                    {
                        ParentId = vm.Id,
                        Name = x.ResponsibleName,
                        Mob1 = x.ResponsibleMob,
                        Tel = x.ResponsibleTel,
                        Email = x.ResponsibleEmail,
                        Transfer = x.ResponsibleTransfer,
                        PersonTypeId = (int)Lookups.PersonTypeCl.SupplierAndCustomerResponsible
                    }).ToList();
                }
                if (vm.Id != Guid.Empty)
                {
                    if (db.Persons.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                    var model = db.Persons.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    //model.PersonTypeId = vm.PersonTypeId;
                    model.AreaId = vm.AreaId;
                    model.Address = vm.Address;
                    model.Mob1 = vm.Mob1;
                    model.Mob2 = vm.Mob2;
                    model.Tel = vm.Tel;
                    model.GenderId = vm.GenderId;
                    model.Notes = vm.Notes;
                    model.PersonCategoryId = vm.PersonCategoryId;
                    model.LocationPath = vm.LocationPath;
                    model.TaxNumber = vm.TaxNumber;
                    db.Entry(model).State = EntityState.Modified;
                    //remove all old Customer Responsible
                    var oldSupplierResponsible = db.Persons.Where(x => !x.IsDeleted && x.ParentId == vm.Id).ToList();
                    foreach (var item in oldSupplierResponsible)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.Entry(model).State = EntityState.Modified;
                    if (SupplierResponsible.Count > 0)
                    {
                        //add new supplier Responsible
                        db.Persons.AddRange(SupplierResponsible);
                    }
                    AccountsTree accountTreeCust;
                    AccountsTree accountTreeSupp;
                    //فى حالة ان الشخص مورد وعميل يتم تحديث فى حساب العميل
                    if (model.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //update customer name in account tree 
                        accountTreeCust = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountsTreeCustomerId);
                        accountTreeCust.AccountName = vm.Name;
                        db.Entry(accountTreeCust).State = EntityState.Modified;
                    }

                    //update supplier name in account tree 
                    accountTreeSupp = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeSupplierId);
                    accountTreeSupp.AccountName = vm.Name;
                    db.Entry(accountTreeSupp).State = EntityState.Modified;
                    var aff = db.SaveChanges(auth.CookieValues.UserId);
                    if (aff > 0)
                        isSaved = true;
                }
                else
                {

                    if (vm.PersonTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    if (db.Persons.Where(x => !x.IsDeleted && x.Name == vm.Name && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0) ///??
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //فى حالة ان الشخص مورد وعميل يتم اضافة فى حساب العميل
                    if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //add as customer in account tree
                        var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, AccountTreeSelectorTypesCl.Operational);
                        db.AccountsTrees.Add(accountTreeCust);
                        //add as supplier in account tree
                        var accountTreeSupp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.Operational);
                        db.AccountsTrees.Add(accountTreeSupp);

                        //add person in personTable
                        vm.AccountsTreeCustomer = accountTreeCust;
                        vm.AccountsTreeSupplier = accountTreeSupp;
                        db.Persons.Add(vm);
                        if (SupplierResponsible.Count > 0)
                        {
                            foreach (var item in SupplierResponsible)
                            {
                                item.PersonParent = vm;
                            }
                            //add new Supplier Responsible
                            db.Persons.AddRange(SupplierResponsible);
                        }
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            isSaved = true;
                    }
                    else
                    {
                        //add as supplier in account tree
                        var accountTreeSupp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.Operational);
                        db.AccountsTrees.Add(accountTreeSupp);
                        //add person in personTable
                        vm.AccountsTreeSupplier = accountTreeSupp;
                        db.Persons.Add(vm);
                        if (SupplierResponsible.Count > 0)
                        {
                            //remove all old Supplier Responsible
                            foreach (var item in SupplierResponsible)
                            {
                                item.PersonParent = vm;
                            }
                            //add new Supplier Responsible
                            db.Persons.AddRange(SupplierResponsible);
                        }
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            isSaved = true;
                    }
                }
                if (isSaved == true)
                {
                    DSSupplierResponsible = null;
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
                var model = db.Persons.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    var isExistGenerarDays = db.GeneralDailies.Where(x => !x.IsDeleted && (x.AccountsTreeId == model.AccountsTreeCustomerId || x.AccountsTreeId == model.AccountTreeSupplierId || x.AccountsTreeId == model.AccountTreeEmpCustodyId)).Any();
                    if (isExistGenerarDays)
                        return Json(new { isValid = false, message = "لا يمكن الحذف لارتباط الحساب بمعاملات مسجله مسبقا" });

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (model.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //delete customer account tree
                        var accountTreeCust = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountsTreeCustomerId);
                        if (accountTreeCust != null)
                        {
                            accountTreeCust.IsDeleted = true;
                            db.Entry(accountTreeCust).State = EntityState.Modified;
                        }
                    }
                    //delete supplier account tree
                    var accountTree = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeSupplierId);
                    if (accountTree != null)
                    {
                        accountTree.IsDeleted = true;
                        db.Entry(accountTree).State = EntityState.Modified;
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

        #region تنشيط حالة المورد 
        [HttpPost]
        public ActionResult ActivePerson(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Persons.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    model.IsActive = true;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم تغيير حالة المورد الى نشط بنجاح" });
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

        #region ايقاف تنشيط حالة المورد 
        [HttpPost]
        public ActionResult UnActivePerson(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Persons.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    model.IsActive = false;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم تغيير حالة المورد الى غير نشط بنجاح" });
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