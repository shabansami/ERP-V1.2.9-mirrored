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
    public class CustomersController : Controller
    {
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        // GET: Clients
        public ActionResult Index()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
            ViewBag.RegionId = new SelectList(new List<Region>(), "Id", "Name");
            ViewBag.DistrictId = new SelectList(new List<District>(), "Id", "Name");
            //ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");

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
                    data = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer) && (x.Name.Contains(txtSearch) || x.Mob1.Contains(txtSearch) || x.Mob2.Contains(txtSearch) || x.Tel.Contains(txtSearch))).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn = x.CreatedOn, CountryName = x.Area.City.Country.Name, CityName = x.Area.City.Name, AreaName = x.Area.Name, RegionName = x.District.Region.Name, Name = x.Name, DistrictName = x.District.Name, PersonCategoryName = x.PersonCategory.Name, typ = (int)UploalCenterTypeCl.Customer, IsActive = x.IsActive, Actions = n, Num = n }).ToList(),
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
                return Json(new
                {
                    data = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn = x.CreatedOn, CountryName = x.Area.City.Country.Name, CityName = x.Area.City.Name, AreaName = x.Area.Name, RegionName = x.District.Region.Name, DistrictName = x.District.Name, Name = x.Name, PersonCategoryName = x.PersonCategory.Name, IsActive = x.IsActive, typ = (int)UploalCenterTypeCl.Customer, Actions = n, Num = n }).ToList(),
                    //Mob1 = x.Mob1,
                    //Mob2 = x.Mob2,
                    //Tel = x.Tel
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
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted && x.CountryId == model.Area.City.CountryId), "Id", "Name", model.Area.City.Id);
                    ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted && x.CityId == model.Area.CityId), "Id", "Name", model.AreaId);
                    if (model.District != null)
                    {
                        ViewBag.RegionId = new SelectList(db.Regions.Where(x => !x.IsDeleted && x.AreaId == model.AreaId), "Id", "Name", model.District.RegionId);
                        ViewBag.DistrictId = new SelectList(db.Districts.Where(x => !x.IsDeleted && x.RegionId == model.District.RegionId), "Id", "Name", model.DistrictId);

                    }
                    else
                    {
                        ViewBag.RegionId = new SelectList(db.Regions.Where(x => !x.IsDeleted && x.AreaId == model.AreaId), "Id", "Name");
                        ViewBag.DistrictId = new SelectList(new List<Area>(), "Id", "Name");
                    }
                    ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name", model.GenderId);
                    ViewBag.ParentId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name", model.ParentId);
                    ViewBag.PersonTypeId = new SelectList(db.PersonTypes.Where(x => !x.IsDeleted && (x.Id == (int)PersonTypeCl.Customer) || x.Id == (int)PersonTypeCl.SupplierAndCustomer), "Id", "Name", model.PersonTypeId);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", model.PersonCategoryId);
                    var customerResponsibles = db.Persons.Where(x => !x.IsDeleted && x.ParentId == id)
                       .Select(x => new ResponsibleDT
                       {
                           Id = x.Id,
                           ResponsibleName = x.Name,
                           ResponsibleMob = x.Mob1,
                           ResponsibleTransfer = x.Transfer

                       }).ToList();
                    DSCustomerResponsible = JsonConvert.SerializeObject(customerResponsibles);
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
                ViewBag.RegionId = new SelectList(new List<Region>(), "Id", "Name");
                ViewBag.DistrictId = new SelectList(new List<District>(), "Id", "Name");
                ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name", 1);
                ViewBag.ParentId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                ViewBag.PersonTypeId = new SelectList(db.PersonTypes.Where(x => !x.IsDeleted && (x.Id == (int)PersonTypeCl.Customer) || x.Id == (int)PersonTypeCl.SupplierAndCustomer), "Id", "Name", 2);
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                ViewBag.LastRow = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Person());
            }
        }
        public static string DSCustomerResponsible { get; set; }
        public ActionResult GetDSCustomerResponsible()
        {
            int? n = null;
            if (DSCustomerResponsible == null)
                return Json(new
                {
                    data = new ResponsibleDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DSCustomerResponsible)
                }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AddCustomerResponsible(string ResponsibleName, string ResponsibleMob, string ResponsibleTel, string ResponsibleJob, string ResponsibleEmail, string ResponsibleTransfer, string DT_Datasource)
        {

            List<ResponsibleDT> deDS = new List<ResponsibleDT>();
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DT_Datasource);
            if (string.IsNullOrEmpty(ResponsibleName) || string.IsNullOrEmpty(ResponsibleMob))
            {
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات السمئول صحيحة" }, JsonRequestBehavior.AllowGet);
            }
            var CustomerResponsible = new ResponsibleDT { ResponsibleName = ResponsibleName, ResponsibleMob = ResponsibleMob, ResponsibleTel = ResponsibleTel, ResponsibleJob = ResponsibleJob, ResponsibleEmail = ResponsibleEmail, ResponsibleTransfer = ResponsibleTransfer };
            deDS.Add(CustomerResponsible);
            DSCustomerResponsible = JsonConvert.SerializeObject(deDS);
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
                List<Person> CustomerResponsible = new List<Person>();

                if (DT_Datasource != null)
                {
                    deDS = JsonConvert.DeserializeObject<List<ResponsibleDT>>(DT_Datasource);
                    CustomerResponsible = deDS.Select(x => new Person
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
                    if (db.Persons.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Persons.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    model.PersonCategoryId = vm.PersonCategoryId;
                    model.EntityName = vm.EntityName;
                    //model.PersonTypeId = vm.PersonTypeId;
                    model.AreaId = vm.AreaId;
                    model.Address = vm.Address;
                    model.Mob1 = vm.Mob1;
                    model.Mob2 = vm.Mob2;
                    model.Tel = vm.Tel;
                    model.ParentId = vm.ParentId;
                    model.GenderId = vm.GenderId;
                    model.Notes = vm.Notes;
                    model.LocationPath = vm.LocationPath;
                    model.Address2 = vm.Address2;
                    model.WebSite = vm.WebSite;
                    model.TaxNumber = vm.TaxNumber;
                    model.CommercialRegistrationNo = vm.CommercialRegistrationNo;
                    model.LimitDangerSell = vm.LimitDangerSell;
                    model.DistrictId = vm.DistrictId;
                    db.Entry(model).State = EntityState.Modified;
                    //remove all old Customer Responsible
                    var oldCustomerResponsible = db.Persons.Where(x => !x.IsDeleted && x.ParentId == vm.Id).ToList();
                    foreach (var item in oldCustomerResponsible)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
                    }
                    db.Entry(model).State = EntityState.Modified;
                    if (CustomerResponsible.Count > 0)
                    {
                        //add new Customer Responsible
                        db.Persons.AddRange(CustomerResponsible);
                    }
                    AccountsTree accountTreeCust;
                    AccountsTree accountTreeSupp;
                    //فى حالة ان الشخص مورد وعميل يتم تحديث فى حساب المورد
                    if (model.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //update supplier name in account tree 
                        accountTreeSupp = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeSupplierId);
                        accountTreeSupp.AccountName = vm.Name;
                        db.Entry(accountTreeSupp).State = EntityState.Modified;
                    }

                    //update customer name in account tree 
                    accountTreeCust = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountsTreeCustomerId);
                    accountTreeCust.AccountName = vm.Name;
                    db.Entry(accountTreeCust).State = EntityState.Modified;
                    var aff = db.SaveChanges(auth.CookieValues.UserId);
                    if (aff > 0)
                        isSaved = true;

                }
                else
                {
                    if (vm.PersonTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    if (db.Persons.Where(x => !x.IsDeleted && x.Name == vm.Name && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0) ///??
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    //فى حالة ان الشخص مورد وعميل يتم اضافة فى حساب المورد
                    if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //add as customer in account tree
                        var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                        db.AccountsTrees.Add(accountTreeCust);
                        //add as supplier in account tree
                        var accountTreeSupp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                        db.AccountsTrees.Add(accountTreeSupp);

                        //add person in personTable
                        vm.AccountsTreeCustomer = accountTreeCust;
                        vm.AccountsTreeSupplier = accountTreeSupp;
                        db.Persons.Add(vm);
                        if (CustomerResponsible.Count > 0)
                        {
                            foreach (var item in CustomerResponsible)
                            {
                                item.PersonParent = vm;
                            }
                            //add new Customer Responsible
                            db.Persons.AddRange(CustomerResponsible);
                        }
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            isSaved = true;
                    }
                    else
                    {
                        //add as customer in account tree
                        var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer ? AccountTreeSelectorTypesCl.SupplierAndCustomer : AccountTreeSelectorTypesCl.Customer);
                        db.AccountsTrees.Add(accountTreeCust);
                        //add person in personTable
                        vm.AccountsTreeCustomer = accountTreeCust;
                        db.Persons.Add(vm);
                        if (CustomerResponsible.Count > 0)
                        {
                            //remove all old Customer Responsible
                            foreach (var item in CustomerResponsible)
                            {
                                item.PersonParent = vm;
                            }
                            //add new Customer Responsible
                            db.Persons.AddRange(CustomerResponsible);
                        }
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            isSaved = true;
                    }


                }
                if (isSaved == true)
                {
                    DSCustomerResponsible = null;
                    if (isInsert)
                        return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
                    else
                        return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });
                }
                else if (isSaved == null)
                    return Json(new { isValid = false, isInsert, message = "تأكد من تعريف الأكواد الحسابية فى شاشة الاعدادات" });

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
                        //delete supplier account tree
                        var accountTreeSupp = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountTreeSupplierId);
                        if (accountTreeSupp != null)
                        {
                            accountTreeSupp.IsDeleted = true;
                            db.Entry(accountTreeSupp).State = EntityState.Modified;
                        }
                    }
                    //delete supplier account tree
                    var accountTree = db.AccountsTrees.FirstOrDefault(x => x.Id == model.AccountsTreeCustomerId);
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

        #region تنشيط حالة العميل 
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
                        return Json(new { isValid = true, message = "تم تغيير حالة العميل الى نشط بنجاح" });
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

        #region ايقاف تنشيط حالة العميل 
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
                        return Json(new { isValid = true, message = "تم تغيير حالة العميل الى غير نشط بنجاح" });
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