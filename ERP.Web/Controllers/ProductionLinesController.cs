using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Web.ViewModels;
using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class ProductionLinesController : Controller
    {
        // GET: ProductionLines
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        private static string DS;
        public ProductionLinesController()
        {
            db = new VTSaleEntities();
        }
        [Authorization]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.ProductionLines.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, Name = x.Name, Notes = x.Notes,HasProductionEmp=  x.ProductionOrders.Where(y => !y.IsDeleted).Any(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        #region اضافة موظف لخط الانتاج
        public ActionResult GetDSProductionLineEmp()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new ProductionLineEmpDto()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ProductionLineEmpDto>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProductionLineEmp(ProductionLineEmpDto vm)
        {
            List<ProductionLineEmpDto> deDS = new List<ProductionLineEmpDto>();
            string employeeName = "";
            string jobName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ProductionLineEmpDto>>(vm.DT_Datasource);
            if (vm.EmployeeId != null)
            {
                if (deDS.Where(x => x.EmployeeId == vm.EmployeeId).Count() > 0)
                    return Json(new { isValid = false, msg = "الموظف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                var emp = db.Employees.FirstOrDefault(x => x.Id == vm.EmployeeId);
                employeeName = emp.Person.Name;
                jobName = emp.Job!=null?emp.Job.Name:null;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار موظف " }, JsonRequestBehavior.AllowGet);
            //التأكد من ادخال اجر الساعه فى حالة احتساب عدد ساعات الانتاج
            if (vm.CalculatingHours)
                if (vm.HourlyWage<=0)
                    return Json(new { isValid = false, msg = "لابد من ادخال أجر الساعه للموظف" }, JsonRequestBehavior.AllowGet);

            var newProEmp = new ProductionLineEmpDto { EmployeeId = vm.EmployeeId, ProductionEmpType=vm.IsProductionEmp?"بالانتاج":null, CalculatingHours = vm.CalculatingHours, IsProductionEmp=vm.IsProductionEmp, HourlyWage =vm.HourlyWage, EmployeeName = employeeName, DT_Datasource = vm.DT_Datasource, JobName =jobName };
            deDS.Add(newProEmp);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الموظف بنجاح " }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Authorization]
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ProductionLineVM model= new ProductionLineVM();
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                     model = db.ProductionLines.Where(x => x.Id == id).Select(
                        x=>new ProductionLineVM
                        {
                            Id=x.Id,
                            Name= x.Name,
                            Notes= x.Notes,
                            ProductionLineEmps=x.ProductionLineEmployees.Where(p=>!p.IsDeleted)
                            .Select(p=>new ProductionLineEmpDto {
                                EmployeeId=p.EmployeeId,
                                ProductionLineId=p.ProductionLineId,
                                EmployeeName=p.Employee.Person!=null?p.Employee.Person.Name:null,
                                JobName=p.Employee.Job!=null?p.Employee.Job.Name:null,
                                IsProductionEmp=p.IsProductionEmp,
                                HourlyWage=p.HourlyWage
                            }).ToList()
                        }).FirstOrDefault();
                    DS = JsonConvert.SerializeObject(model.ProductionLineEmps);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                DS = null;
                ViewBag.LastRow = db.ProductionLines.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");
            return View(model);

        }

        [Authorization]
        [HttpPost]
        public JsonResult CreateEdit(ProductionLineVM vm,string DT_Datasource)
        {
            var t = JsonConvert.DeserializeObject<List<ProductionLineEmpDto>>(DT_Datasource);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) )
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                //الموظفين
                List<ProductionLineEmpDto> productionLineEmpDtos = new List<ProductionLineEmpDto>();
                List<ProductionLineEmployee> productionLineEmployees = new List<ProductionLineEmployee>();

                if (DT_Datasource != null)
                {
                    productionLineEmpDtos = JsonConvert.DeserializeObject<List<ProductionLineEmpDto>>(DT_Datasource);
                    productionLineEmployees = productionLineEmpDtos.Select(
                        x => new ProductionLineEmployee
                        {
                            ProductionLineId=vm.Id,
                           EmployeeId= x.EmployeeId,
                           IsProductionEmp= x.IsProductionEmp,
                            HourlyWage=x.HourlyWage,
                            CalculatingHours=x.CalculatingHours,
                        }
                        ).ToList();
                }

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    if (db.ProductionLines.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.ProductionLines.FirstOrDefault(x => x.Id == vm.Id);
                    model.Name = vm.Name;
                    model.Notes = vm.Notes;
                    //حذف اى موظفين سابقين غير مرتبط خط الانتاج باى اوامر انتاج بعد
                    //var isEsits=db.ProductionLines.Where(x=>!x.IsDeleted&&x.Id==model.Id&&x.ProductionOrders.Where(y=>!y.IsDeleted).Any())
                    var prevouisEmps = db.ProductionLineEmployees.Where(x => !x.IsDeleted && x.ProductionLineId == model.Id).ToList();
                    foreach (var emp in prevouisEmps)
                    {
                        emp.IsDeleted = true;
                    }
                    db.ProductionLineEmployees.AddRange(productionLineEmployees);
                }
                else
                {
                    if (db.ProductionLines.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    ProductionLine productionLine = new ProductionLine
                    {
                        Name = vm.Name,
                        Notes = vm.Notes,
                        ProductionLineEmployees = productionLineEmployees
                    };
                    db.ProductionLines.Add(productionLine);
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
        [Authorization]
        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        [Authorization]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ProductionLines.FirstOrDefault(x => x.Id == Id);
                if (model != null)
                {
                    model.IsDeleted = true;
                    var proEmps=model.ProductionLineEmployees.Where(x=>!x.IsDeleted).ToList();
                    foreach (var item in proEmps)
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