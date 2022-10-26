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
    public class ContractSalaryAdditionsController : Controller
    {
        // GET: ContractSalaryAdditions
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        
        public ActionResult Index()
        {
            //var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
            //var employees = db.Employees.Where(x => !x.IsDeleted);
            //employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
            ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name");
            ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
            ViewBag.SalaryAdditionTypeId = new SelectList(db.SalaryAdditionTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SalaryAdditionId = new SelectList(new List<SalaryAddition>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;

            return Json(new
            {
                data = db.ContractSalaryAdditions.Where(x => !x.IsDeleted && !x.IsEveryMonth).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsPayed = x.IsPayed, SalaryAddition = x.SalaryAddition.Name, Employee = x.ContractScheduling.Contract.Employee.Person.Name, MonthYear=x.ContractScheduling.Name/*x.ContractScheduling.MonthYear.ToString().Substring(0,7)*/, AdditionDate = x.AdditionDate.ToString(), Amount=x.Amount, Actions = n, Num = n }).ToList()
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
                    var model = db.ContractSalaryAdditions.FirstOrDefault(x=>x.Id==id);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name",model.ContractScheduling.Contract.Employee.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name", model.ContractScheduling.Contract.EmployeeId);
                    ViewBag.SalaryAdditionTypeId = new SelectList(db.SalaryAdditionTypes.Where(x => !x.IsDeleted), "Id", "Name", model.SalaryAddition.SalaryAdditionTypeId);
                    ViewBag.SalaryAdditionId = new SelectList(db.SalaryAdditions.Where(x => !x.IsDeleted&&x.SalaryAdditionTypeId== model.SalaryAddition.SalaryAdditionTypeId), "Id", "Name", model.SalaryAdditionId);
                    ViewBag.ContractSchedulingId = new SelectList(db.ContractSchedulings.Where(x => !x.IsDeleted&&x.ContractId==model.ContractScheduling.ContractId && !x.IsApproval).OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name =x.Name /*x.MonthYear.ToString().Substring(0, 7)*/ }), "Id", "Name",model.ContractSchedulingId);

                    ViewBag.salaryTypeName = $"نوع العقد : {model.ContractScheduling.Contract.ContractSalaryType.Name}";
                    ViewBag.salaryTypeId = model.ContractScheduling.Contract.ContractSalaryTypeId;
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
                ViewBag.SalaryAdditionTypeId = new SelectList(db.SalaryAdditionTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SalaryAdditionId = new SelectList(new List<SalaryAddition>(), "Id", "Name");
                ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
                ViewBag.LastRow = db.ContractSalaryAdditions.Where(x => !x.IsDeleted&&!x.IsEveryMonth).OrderByDescending(x => x.CreatedOn).FirstOrDefault();

                ViewBag.salaryTypeName = "";
                ViewBag.salaryTypeId =null;

                return View(new ContractSalaryAddition() {AdditionDate=Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ContractSalaryAddition vm, Guid? EmployeeId,int? salaryTypeId)
        {
            if (ModelState.IsValid)
            {
                if (vm.SalaryAdditionId == null  || vm.AdditionDate == null|| EmployeeId == null|| vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (salaryTypeId!=(int)ContractSalaryTypeCl.Daily&&vm.ContractSchedulingId==null)
                    return Json(new { isValid = false, message = "تأكد من اختيار الشهر/الاسبوع" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    //if (db.ContractSalaryAdditions.Where(x => !x.IsDeleted && x.EmployeeId == vm.EmployeeId&&x.SalaryAdditionId==vm.SalaryAdditionId&&x.AdditionDate==vm.AdditionDate && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "تم تسجيل إضافة للموظف مسبقا فى نفس الفترة" });

                    var model = db.ContractSalaryAdditions.FirstOrDefault(x=>x.Id==vm.Id);
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);

                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.AdditionDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist>0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear = vm.AdditionDate,
                            ToDate = vm.AdditionDate,
                            Name = $"يوم : {vm.AdditionDate.Value.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }

                    model.SalaryAdditionId = vm.SalaryAdditionId;
                    model.AdditionDate = vm.AdditionDate;
                    model.Notes = vm.Notes;
                    model.Amount = vm.Amount;
                    model.AmountPayed = vm.Amount;
                    model.IsAffactAbsence = vm.IsAffactAbsence;
                    model.ContractScheduling = contractScheduling;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {

                    isInsert = true;
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);
                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId==(int)ContractSalaryTypeCl.Daily&& vm.ContractSchedulingId==null) 
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.AdditionDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist > 0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear=vm.AdditionDate,
                            ToDate = vm.AdditionDate,
                            Name = $"يوم : {vm.AdditionDate.Value.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }
                    var ContractSalaryAdditions = new ContractSalaryAddition
                    {
                        SalaryAdditionId = vm.SalaryAdditionId,
                        AdditionDate = vm.AdditionDate,
                        Notes = vm.Notes,
                        Amount = vm.Amount,
                        AmountPayed = vm.Amount,
                        IsAffactAbsence = vm.IsAffactAbsence,
                        IsFirstTime = true,
                        ContractScheduling = contractScheduling
                    };

                    db.ContractSalaryAdditions.Add(ContractSalaryAdditions);

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
                var model = db.ContractSalaryAdditions.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

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