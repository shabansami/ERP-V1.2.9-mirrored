using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SaleMenStoresController : Controller
    {
        // GET: SaleMenStores
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
            IQueryable<Employee> employees = null;
            employees = db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen&& contracts.Any(c => c.EmployeeId == x.Id));
            return Json(new
            {
                data = employees.Select(x => new { Id = x.Id, EmpGuid=x.EmpGuid, DepartmentName = x.Department.Name, EmployeeName = x.Person.Name, StoreName = x.Store!=null?x.Store.Name:null, Actions = n, Num = n }).ToList()
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
                    var model = db.Employees.FirstOrDefault(x=>x.Id==id);
                    ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", model.DepartmentId);
                    ViewBag.EmployeeId = new SelectList(EmployeeService.GetSaleMens(model.DepartmentId), "Id", "Name", model.Id);
                    ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", model.BranchId);
                    ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == model.BranchId && !x.IsDamages), "Id", "Name", model.StoreId);

                    SaleMenStoreVM vm = new SaleMenStoreVM
                    {
                        Id = model.Id,
                        EmployeeId = model.Id,
                        StoreId = model.StoreId
                    };
                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult CreateEdit(SaleMenStoreVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.EmployeeId == null || vm.StoreId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    var model = db.Employees.FirstOrDefault(x=>x.Id==vm.Id);
                    //تسجيل حركة تغيير المخزن للمندوب 
                    if (vm.StoreId != model.StoreId)
                    {
                        model.StoreId = vm.StoreId;
                        db.Entry(model).State = EntityState.Modified;

                        db.SaleMenStoreHistories.Add(new SaleMenStoreHistory
                        {
                            StoreId = vm.StoreId,
                            EmployeeId = model.Id,
                            TransferDate = Utility.GetDateTime()

                        });
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم التعديل بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    }else
                    return Json(new { isValid = false, message = "المخزن محدد للمندوب بالفعل مسبقا" });


                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    model.StoreId = null;
                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم حذف ارتباط المندوب بالمخزن" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }

        #region عرض حركات تحويلات المخازن للمندوب

        public ActionResult ShowHistory(Guid? empGuid)
        {
            if (empGuid == null || empGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.Employees.Where(x => !x.IsDeleted && x.EmpGuid == empGuid).FirstOrDefault();
            vm.SaleMenStoreHistories = vm.SaleMenStoreHistories.Where(x => !x.IsDeleted).ToList();
            return View(vm);
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