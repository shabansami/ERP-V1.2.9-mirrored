using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Web.ViewModels;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    
    [Authorization]
    public class EmployeesController : Controller
    {
        // GET: Employees
        VTSaleEntities db = new VTSaleEntities();
        //VTSAuth auth = new VTSAuth();
         VTSAuth auth => TempData["userInfo"] as VTSAuth;
        //var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.JobId = new SelectList(db.Jobs.Where(x => !x.IsDeleted), "Id", "Name");
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
                    data = db.Employees.OrderBy(x => x.CreatedOn).Where(x => !x.IsDeleted && (x.Person.Name.Contains(txtSearch) || x.Person.Mob1.Contains(txtSearch) || x.Person.Mob2.Contains(txtSearch) || x.Person.Tel.Contains(txtSearch))).Select(x => new { Id = x.Id, JobName = x.Job.Name, DepartmentName = x.Department.Name, Name = x.Person.Name, Tel = x.Person.Mob1, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;
            }
            else
                return Json(new
                {
                    data = db.Employees.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, JobName = x.Job.Name, DepartmentName = x.Department.Name, Name = x.Person.Name, Tel = x.Person.Mob1, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Employees.Where(x => x.Id == id).FirstOrDefault();
                    var employee = new EmployeeViewModel
                    {
                        Id = model.Id,
                        PersonHidId = model.PersonId,
                        JobId = model.JobId,
                        DepartmentId = model.DepartmentId,
                        IsSaleMen = model.IsSaleMen,
                        NationalID = model.NationalID,
                        BirthDay = model.BirthDay,
                        SocialStatusId = model.SocialStatusId,
                        DateOfHiring = model.DateOfHiring,
                        HasRole = model.HasRole,
                        //Salary=model.Salary,
                        Person = model.Person,
                        CommissionPercentage = model.CommissionPercentage,
                        Is100Percentage = model.Is100Percentage,
                        PassportExpirationDate = model.PassportExpirationDate,
                        PassportIssuer = model.PassportIssuer,
                        PassportJob = model.PassportJob,
                        PassportNumber = model.PassportNumber,
                        ResidenceExpirationDate = model.ResidenceExpirationDate,
                        ResidenceIssuer = model.ResidenceIssuer,
                        ResidenceJob = model.ResidenceJob,
                        ResidenceNumber = model.ResidenceNumber,
                        MedicalInsuranceCategory = model.MedicalInsuranceCategory,
                        MedicalInsuranceCompany = model.MedicalInsuranceCompany,
                        MedicalInsuranceExpirationDate = model.MedicalInsuranceExpirationDate,
                        MedicalInsurancePolicyNumber = model.MedicalInsurancePolicyNumber,
                        MedicalInsuranceReleaseDate = model.MedicalInsuranceReleaseDate,
                        SocialSecurityCurrentBalance = model.SocialSecurityCurrentBalance,
                        SocialSecurityNumber = model.SocialSecurityNumber,
                    };

                    var defaultBranchIdss = model?.EmployeeBranches.Where(x => !x.IsDeleted).FirstOrDefault()?.BranchId;

                    var empbranchIds = model.EmployeeBranches.Where(b => !b.IsDeleted);
                    var defaultBranchId = empbranchIds.FirstOrDefault()?.BranchId;
                    employee.BranchIds = empbranchIds.Select(x => x.BranchId).ToList();
                    ViewBag.NationalityId = new SelectList(db.Nationalities.Where(x => !x.IsDeleted), "Id", "Name", model.NationalID);
                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", model.Person.Area?.City.Country.Id);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name", model.Person.Area?.City.Id);
                    ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name", model.Person.AreaId);
                    ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name", model.Person.GenderId);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", model.DepartmentId);
                    ViewBag.JobId = new SelectList(db.Jobs.Where(x => !x.IsDeleted), "Id", "Name", model.JobId);
                    ViewBag.BranchIds = new SelectList(branches, "Id", "Name", employee.BranchIds);
                    ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == defaultBranchId && !x.IsDamages), "Id", "Name", model.StoreId);
                    ViewBag.SocialStatusId = new SelectList(db.SocialStatuses.Where(x => !x.IsDeleted), "Id", "Name", model.SocialStatusId);
                    return View(employee);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.NationalityId = new SelectList(db.Nationalities.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted).GroupBy(x => x.CountryId).FirstOrDefault().ToList(), "Id", "Name");
                ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted).GroupBy(x => x.CityId).FirstOrDefault().ToList(), "Id", "Name");
                ViewBag.GenderId = new SelectList(db.Genders.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.JobId = new SelectList(db.Jobs.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.BranchIds = new SelectList(branches, "Id", "Name");
                ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && !x.IsDamages).GroupBy(x => x.BranchId).FirstOrDefault().ToList(), "Id", "Name");
                ViewBag.SocialStatusId = new SelectList(db.SocialStatuses.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.LastRow = db.Employees.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new EmployeeViewModel() { Person = new Person(), Is100Percentage = true, BranchIds = new List<Guid> { new Guid("A563B457-F143-4CEF-BB94-36197778422B") } });
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(EmployeeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Person.Name) || string.IsNullOrEmpty(vm.Person.Mob1) || string.IsNullOrEmpty(vm.NationalID) || vm.JobId == null|| vm.DepartmentId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (vm.BranchIds.Count() == 0)
                    return Json(new { isValid = false, message = "تأكد من اختيار فرع /اكتر للموظف" });
                if (vm.IsSaleMen)
                    if (vm.BranchIds.Count()>1)
                        return Json(new { isValid = false, message = "تأكد من اختيار فرع واحد فقط لان الموظف مندوب " });
                var isInsert = false;


                if (vm.Id != Guid.Empty)
                {
                    if (db.Employees.Where(x => !x.IsDeleted && x.NationalID == vm.NationalID && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Employees.FirstOrDefault(x=>x.Id==vm.Id);
                    //تسجيل حركة تغيير المخزن للمندوب 
                    if (vm.IsSaleMen && vm.StoreId != null && vm.StoreId != model.StoreId)
                    {
                        db.SaleMenStoreHistories.Add(new SaleMenStoreHistory
                        {
                            StoreId = vm.StoreId,
                            EmployeeId = model.Id,
                            TransferDate = Utility.GetDateTime()

                        });
                    }

                    //حذف اى فروع مسجلة للموظف سابقا
                    var prviousBranches = db.EmployeeBranches.Where(x => !x.IsDeleted && x.EmployeeId == model.Id).ToList();
                    foreach (var item in prviousBranches)
                    {
                        item.IsDeleted = true;
                        db.Entry(model).State = EntityState.Modified;

                    }
                    var empBranches = vm.BranchIds.Select(x => new EmployeeBranch { BranchId = x,EmployeeId=model.Id }).ToList();
                    //model.EmployeeBranches = empBranches;
                    db.EmployeeBranches.AddRange(empBranches);

                    model.JobId = vm.JobId;
                    model.DepartmentId = vm.DepartmentId;
                    model.IsSaleMen = vm.IsSaleMen;
                    model.NationalID = vm.NationalID;
                    model.BirthDay = vm.BirthDay;
                    model.SocialStatusId = vm.SocialStatusId;
                    model.DateOfHiring = vm.DateOfHiring;
                    model.HasRole = vm.HasRole;
                    //model.Salary = vm.Salary;
                    model.SocialStatusId = vm.SocialStatusId;
                    model.StoreId = vm.StoreId;
                    model.CommissionPercentage = vm.CommissionPercentage;
                    model.Is100Percentage = vm.Is100Percentage;
                    model.PassportExpirationDate= vm.PassportExpirationDate;    
                    model.PassportIssuer= vm.PassportIssuer;
                    model.PassportJob= vm.PassportJob;  
                    model.PassportNumber=vm.PassportNumber;
                    model.ResidenceExpirationDate= vm.ResidenceExpirationDate;
                    model.ResidenceIssuer= vm.ResidenceIssuer;
                    model.ResidenceJob= vm.ResidenceJob;
                    model.ResidenceNumber= vm.ResidenceNumber;  
                    model.MedicalInsuranceCategory= vm.MedicalInsuranceCategory;
                    model.MedicalInsuranceCompany = vm.MedicalInsuranceCompany;
                    model.MedicalInsuranceExpirationDate= vm.MedicalInsuranceExpirationDate;
                    model.MedicalInsurancePolicyNumber=vm.MedicalInsurancePolicyNumber;
                    model.MedicalInsuranceReleaseDate=vm.MedicalInsuranceReleaseDate;
                    model.SocialSecurityCurrentBalance=vm.SocialSecurityCurrentBalance;
                    model.SocialSecurityNumber=vm.SocialSecurityNumber;

                    db.Entry(model).State = EntityState.Modified;
                    var model2 = db.Persons.FirstOrDefault(x=>x.Id==vm.PersonHidId);
                    model2.Name = vm.Person.Name;
                    model2.NameEn = vm.Person.NameEn;
                    model2.Address = vm.Person.Address;
                    model2.Mob1 = vm.Person.Mob1;
                    model2.Mob2 = vm.Person.Mob2;
                    model2.Tel = vm.Person.Tel;
                    model2.AreaId = vm.AreaId;
                    model2.GenderId = vm.GenderId;
                    db.Entry(model2).State = EntityState.Modified;

                    AccountsTree accountTreeEmp;
                    //update employee name in account tree 
                    accountTreeEmp = db.AccountsTrees.FirstOrDefault(x=>x.Id==model.Person.AccountsTreeCustomerId);
                    accountTreeEmp.AccountName = vm.Person.Name;
                    db.Entry(accountTreeEmp).State = EntityState.Modified;

                }
                else
                {
                    if (db.Employees.Where(x => !x.IsDeleted && x.NationalID == vm.NationalID).Count() > 0)
                        return Json(new { isValid = false, message = "الرقم القومي موجود مسبقا" });

                    isInsert = true;
                    //add as customer in account tree
                    var accountTreeEmp = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeEmployeeAccount, vm.Person.Name, AccountTreeSelectorTypesCl.Employee);
                    db.AccountsTrees.Add(accountTreeEmp);
                    //var accountTreeCustody = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustodyAccount, $"عهدة الموظف : "+ vm.Person.Name,  AccountTreeSelectorTypesCl.Custody );
                    //db.AccountsTrees.Add(accountTreeCustody);
                    //add person in personTable
                    var person = new Person
                    {
                        PersonTypeId = (int)Lookups.PersonTypeCl.Employee,
                        AccountsTreeCustomer = accountTreeEmp,
                        //AccountsTree2= accountTreeCustody,
                        Name = vm.Person.Name,
                        NameEn = vm.Person.NameEn,
                        GenderId = vm.GenderId,
                        Address = vm.Person.Address,
                        Mob1 = vm.Person.Mob1,
                        Mob2 = vm.Person.Mob2,
                        Tel = vm.Person.Tel,
                        AreaId = vm.AreaId

                    };
                    db.Persons.Add(person);
                    var empBranches = vm.BranchIds.Select(x => new EmployeeBranch { BranchId = x }).ToList();
                    //model.EmployeeBranches = empBranches;

                    var employee = new Employee
                    {
                        JobId = vm.JobId,
                        DepartmentId = vm.DepartmentId,
                        Person = person,
                        IsSaleMen = vm.IsSaleMen,
                        NationalID = vm.NationalID,
                        BirthDay = vm.BirthDay,
                        SocialStatusId = vm.SocialStatusId,
                        DateOfHiring = vm.DateOfHiring,
                        HasRole = vm.HasRole,
                        EmployeeBranches = empBranches,
                        StoreId = vm.StoreId,
                        CommissionPercentage = vm.CommissionPercentage,
                        Is100Percentage = vm.Is100Percentage,
                        PassportExpirationDate = vm.PassportExpirationDate,
                    PassportIssuer = vm.PassportIssuer,
                    PassportJob = vm.PassportJob,
                    PassportNumber = vm.PassportNumber,
                    ResidenceExpirationDate = vm.ResidenceExpirationDate,
                    ResidenceIssuer = vm.ResidenceIssuer,
                    ResidenceJob = vm.ResidenceJob,
                    ResidenceNumber = vm.ResidenceNumber,
                    MedicalInsuranceCategory = vm.MedicalInsuranceCategory,
                    MedicalInsuranceCompany = vm.MedicalInsuranceCompany,
                    MedicalInsuranceExpirationDate = vm.MedicalInsuranceExpirationDate,
                    MedicalInsurancePolicyNumber = vm.MedicalInsurancePolicyNumber,
                    MedicalInsuranceReleaseDate = vm.MedicalInsuranceReleaseDate,
                    SocialSecurityCurrentBalance = vm.SocialSecurityCurrentBalance,
                    SocialSecurityNumber = vm.SocialSecurityNumber,

                };
                    db.Employees.Add(employee);

                    //تسجيل حركة تغيير المخزن للمندوب 
                    if (vm.IsSaleMen && vm.StoreId != null)
                    {
                        db.SaleMenStoreHistories.Add(new SaleMenStoreHistory
                        {
                            StoreId = vm.StoreId,
                            Employee = employee,
                            TransferDate = Utility.GetDateTime()

                        });
                    }
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
                var model = db.Employees.FirstOrDefault(x=>x.Id==Id);
                var model2 = db.Persons.FirstOrDefault(x=>x.Id==model.PersonId);
                if (model != null)
                {
                    model.IsDeleted = true;
                    model2.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    db.Entry(model2).State = EntityState.Modified;

                    //الغاء تنشيط اى عقد سابق 
                    var contracts = db.Contracts.Where(x => !x.IsDeleted && x.EmployeeId == model.Id).ToList();
                    foreach (var contract in contracts)
                    {
                        contract.IsDeleted = true;
                        db.Entry(contract).State = EntityState.Modified;
                        var contractSchuds = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id).ToList();
                        foreach (var contractSchud in contractSchuds)
                        {
                            contractSchud.IsDeleted = true;
                            db.Entry(contractSchud).State = EntityState.Modified;
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