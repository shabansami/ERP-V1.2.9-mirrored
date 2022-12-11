using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    public class ProductionLinesController : Controller
    {
        // GET: ProductionLines
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
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
        [Authorization]
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.ProductionLines.FirstOrDefault(x => x.Id == id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.LastRow = db.ProductionLines.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new ProductionLine());
            }
        }

        [Authorization]
        [HttpPost]
        public JsonResult CreateEdit(ProductionLine vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) )
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

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
                        emp.IsDeleted = false;
                    }
                    model.ProductionLineEmployees = vm.ProductionLineEmployees;
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
                        ProductionLineEmployees = vm.ProductionLineEmployees
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