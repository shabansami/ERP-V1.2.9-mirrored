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
    public class ContainersController : Controller
    {
        VTSaleEntities db;
        VTSAuth auth;
        public ContainersController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
        }
        // GET: Containers
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Containers.Where(x => !x.IsDeleted).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, CreatedOn=x.CreatedOn, Name = x.Name, ShippingCompany=x.ShippingCompany, ExitDate=x.ExitDate.ToString(), ArrivalDate=x.ArrivalDate.ToString(), Actions = n, Num = n }).ToList()
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
                    var model = db.Containers.FirstOrDefault(x=>x.Id==id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBag.LastRow = db.Containers.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Container());
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Container vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name) ||string.IsNullOrEmpty(vm.CompanyName))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.Containers.Where(x => !x.IsDeleted && x.Name == vm.Name && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.Containers.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;
                    model.CompanyName = vm.CompanyName;
                    model.ShippingCompany = vm.ShippingCompany;
                    model.ArrivalDate = vm.ArrivalDate;
                    model.ExitDate = vm.ExitDate;
                    model.RespoName = vm.RespoName;
                    model.RespoTel = vm.RespoTel;
                    model.ShippingPort = vm.ShippingPort;
                    model.ContainerNumber = vm.ContainerNumber;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.Containers.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    isInsert = true;
                    db.Containers.Add(new Container
                    {
                        Name = vm.Name,
                       CompanyName=vm.CompanyName,
                       ShippingCompany=vm.ShippingCompany,
                       ArrivalDate=vm.ArrivalDate,
                       ExitDate=vm.ExitDate,
                       RespoName=vm.RespoName,
                       RespoTel=vm.RespoTel,
                       ShippingPort=vm.ShippingPort,
                        ContainerNumber=vm.ContainerNumber
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
                var model = db.Containers.FirstOrDefault(x=>x.Id==Id);
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