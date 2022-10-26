using ERP.Web.Identity;
using ERP.DAL;
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
    [Authorization] // شاشة غير مستخدمة 

    public class SaleMenAreasController : Controller
    {
        // GET: SaleMenAreas
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted&&x.IsSaleMen).Select(x=>new { Id=x.PersonId,Name=x.Person.Name}), "Id", "Name");

            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SaleMenAreas.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, CountryName = x.Area.City.Country.Name, CityName = x.Area.City.Name, AreaName = x.Area.Name, SaleMenName=x.Person.Name, Actions = n, Num = n }).ToList()
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
                    var model = db.SaleMenAreas.FirstOrDefault(x=>x.Id==id);

                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", model.Area.City.CountryId);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name", model.Area.CityId);
                    ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name", model.AreaId);
                    ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen).Select(x => new { Id = x.PersonId, Name = x.Person.Name }), "Id", "Name",model.PersonId);

                    return View(new SaleMenAreaVM {Id=model.Id });
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
                ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
                ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen).Select(x => new { Id = x.PersonId, Name = x.Person.Name }), "Id", "Name");
                ViewBag.LastRow = db.SaleMenAreas.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new SaleMenAreaVM());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SaleMenAreaVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.AreaId == null|| vm.SaleMenId == null|| vm.CityId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.SaleMenAreas.Where(x => !x.IsDeleted && x.PersonId == vm.SaleMenId && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "تم تحديد المنطقة للمندوب مسبقا" });

                    var model = db.SaleMenAreas.FirstOrDefault(x=>x.Id==vm.Id);
                    model.PersonId = vm.SaleMenId;
                    model.AreaId     = vm.AreaId;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.SaleMenAreas.Where(x => !x.IsDeleted && x.PersonId == vm.SaleMenId ).Count() > 0)
                        return Json(new { isValid = false, message = "تم تحديد المنطقة للمندوب مسبقا" });

                    isInsert = true;
                    db.SaleMenAreas.Add(new SaleMenArea
                    {
                        AreaId = vm.AreaId,
                        PersonId = vm.SaleMenId
                    });
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
                var model = db.SaleMenAreas.FirstOrDefault(x=>x.Id==Id);
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