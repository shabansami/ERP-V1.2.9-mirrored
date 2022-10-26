using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class BranchesController : Controller
    {
        // GET: Branches
        VTSaleEntities db;
        VTSAuth auth;
        public BranchesController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
        }
        public ActionResult Index()
        {
            ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
            ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Branches.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn=x.CreatedOn, CountryName=x.Area.City.Country.Name,CityName=x.Area.City.Name,AreaName=x.Area.Name ,Name = x.Name, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Branches.FirstOrDefault(x=>x.Id==id);
                    ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", model.Area.City.CountryId);
                    ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted&&x.CountryId==model.Area.City.CountryId), "Id", "Name", model.Area.CityId);
                    ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted && x.CityId == model.Area.CityId), "Id", "Name", model.AreaId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(new List<City>(), "Id", "Name");
                ViewBag.AreaId = new SelectList(new List<Area>(), "Id", "Name");
                ViewBag.LastRow = db.Branches.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Branch());
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(Branch vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name)||vm.AreaId==null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.Branches.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Branches.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.AreaId = vm.AreaId;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Branches.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    db.Branches.Add(new Branch
                    {
                        Name = vm.Name,
                        AreaId=vm.AreaId
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
                var model = db.Branches.FirstOrDefault(x=>x.Id==Id);
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                    // التاكد من عدم ارتبط الفرع باى مخازن او قيود 
                    var branchExistStore = db.Stores.Where(x => !x.IsDeleted && x.BranchId == Id).Any();
                    var branchExistDailies = db.GeneralDailies.Where(x => !x.IsDeleted && x.BranchId == Id).Any();
                    if (branchExistStore || branchExistDailies)
                        return Json(new { isValid = false, message = "لا يمكن حذف الفرع لارتباطه بعمليات اخرى" });

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