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

    public class ContractSalaryPenaltiesController : Controller
    {
        // GET: ContractSalaryPenalties
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            //var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
            //var employees = db.Employees.Where(x => !x.IsDeleted);
            //employees = employees.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
            ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name");
            ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
            ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.SalaryPenaltyId = new SelectList(new List<SalaryPenalty>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ContractSalaryPenalties.Where(x => !x.IsDeleted&&!x.IsEveryMonth).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsPayed = x.IsPayed, SalaryPenalty = x.SalaryPenalty.Name, Employee = x.ContractScheduling.Contract.Employee.Person.Name, Amount=x.Amount, MonthYear =x.ContractScheduling.Name /*x.ContractScheduling.MonthYear.ToString().Substring(0, 7)*/, PenaltyDate = x.PenaltyDate.ToString(), Actions = n, Num = n }).ToList()
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
                    var model = db.ContractSalaryPenalties.FirstOrDefault(x=>x.Id==id);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", model.ContractScheduling.Contract.Employee.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetEmployees(), "Id", "Name", model.ContractScheduling.Contract.EmployeeId);
                    ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name", model.SalaryPenalty.SalaryPenaltyTypeId);
                    ViewBag.SalaryPenaltyId = new SelectList(db.SalaryPenalties.Where(x => !x.IsDeleted&&x.SalaryPenaltyTypeId== model.SalaryPenalty.SalaryPenaltyTypeId), "Id", "Name", model.SalaryPenaltyId);
                    ViewBag.ContractSchedulingId = new SelectList(db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == model.ContractScheduling.ContractId && !x.IsApproval).OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name =x.Name/* x.MonthYear.ToString().Substring(0, 7)*/ }), "Id", "Name", model.ContractSchedulingId);

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
                ViewBag.SalaryPenaltyTypeId = new SelectList(db.SalaryPenaltyTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SalaryPenaltyId = new SelectList(new List<SalaryPenalty>(), "Id", "Name");
                ViewBag.ContractSchedulingId = new SelectList(new List<ContractScheduling>(), "Id", "Name");
                ViewBag.LastRow = db.ContractSalaryPenalties.Where(x => !x.IsDeleted && !x.IsEveryMonth).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                ViewBag.salaryTypeName = "";
                ViewBag.salaryTypeId = null;

                return View(new ContractSalaryPenalty() { PenaltyDate = Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(ContractSalaryPenalty vm, Guid? EmployeeId, int? salaryTypeId)
        {
            if (ModelState.IsValid)
            {
                if (vm.SalaryPenaltyId == null  || vm.PenaltyDate == null || EmployeeId == null || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (salaryTypeId != (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار الشهر/الاسبوع" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    //if (db.ContractContractSalaryPenalties.Where(x => !x.IsDeleted && x.EmployeeId == vm.EmployeeId&&x.SalaryPenaltyId==vm.SalaryPenaltyId&&x.AdditionDate==vm.AdditionDate && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "تم تسجيل إضافة للموظف مسبقا فى نفس الفترة" });

                    var model = db.ContractSalaryPenalties.FirstOrDefault(x=>x.Id==vm.Id);
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);

                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.PenaltyDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist > 0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear = vm.PenaltyDate,
                            ToDate = vm.PenaltyDate,
                            Name = $"يوم : {vm.PenaltyDate.Value.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }
                    model.SalaryPenaltyId = vm.SalaryPenaltyId;
                    model.PenaltyDate = vm.PenaltyDate;
                    model.Notes = vm.Notes;
                    model.Amount = vm.Amount;
                    model.AmountPayed = vm.Amount;
                    model.EmployeeNotes = vm.EmployeeNotes;
                    model.ContractScheduling = contractScheduling;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {

                    isInsert = true;
                    var contractScheduling = db.ContractSchedulings.FirstOrDefault(x=>x.Id==vm.ContractSchedulingId);
                    //فى حالة العقد اليومى وعدم اختيار يوم مجدول
                    if (salaryTypeId == (int)ContractSalaryTypeCl.Daily && vm.ContractSchedulingId == null)
                    {
                        var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == EmployeeId).FirstOrDefault();
                        //التأكد من عدم وجود التاريخ مجدول مسبقا
                        var dateIsExist = db.ContractSchedulings.Where(x => !x.IsDeleted && vm.PenaltyDate == x.MonthYear && x.ContractId == contract.Id).Count();
                        if (dateIsExist > 0)
                            return Json(new { isValid = false, message = "تم تسجيل اليوم بالفعل .. اختر اليوم من قائمة الايام" });

                        //اضافة يوم فى جدول الجدولة 
                        contractScheduling = new ContractScheduling
                        {
                            Contract = contract,
                            MonthYear = vm.PenaltyDate,
                            ToDate = vm.PenaltyDate,
                            Name = $"يوم : {vm.PenaltyDate.Value.ToString("yyyy-MM-dd")}"
                        };
                        db.ContractSchedulings.Add(contractScheduling);
                    }
                    var ContractSalaryPenalties = new ContractSalaryPenalty
                    {
                        SalaryPenaltyId = vm.SalaryPenaltyId,
                        PenaltyDate = vm.PenaltyDate,
                        Notes = vm.Notes,
                        Amount = vm.Amount,
                        AmountPayed = vm.Amount,
                        EmployeeNotes = vm.EmployeeNotes,
                        ContractScheduling = contractScheduling
                    };

                    db.ContractSalaryPenalties.Add(ContractSalaryPenalties);

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
                var model = db.ContractSalaryPenalties.FirstOrDefault(x=>x.Id==Id);
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