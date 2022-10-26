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

    public class UploadCenterFoldersController : Controller
    {
        // GET: UploadCenterFolders
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.UploadCenters.Where(x => !x.IsDeleted&&x.IsFolder).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, ParentName = x.UploadCenterParent.Name, FolderName = x.Name, Actions = n, Num = n }).ToList()
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
                    var model = db.UploadCenters.FirstOrDefault(x=>x.Id==id);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.LastRow = db.UploadCenters.Where(x => !x.IsDeleted&&x.IsFolder).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new UploadCenter());
            }


        }
        [HttpPost]
        public JsonResult CreateEdit(UploadCenter vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.Id != Guid.Empty)
                {
                    if (db.UploadCenters.Where(x => !x.IsDeleted && x.Id != vm.Id && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "مسمى المجلد موجود مسبقا" });

                    var model = db.UploadCenters.FirstOrDefault(x=>x.Id==vm.Id);
                    model.Name = vm.Name;    
                    model.ParentId = vm.ParentId;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    if (db.UploadCenters.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                        return Json(new { isValid = false, message = "مسمى المجلد موجود مسبقا" });

                    isInsert = true;
                    var newFolder = new UploadCenter
                    {
                        Name = vm.Name,
                        IsFolder = true,
                        ParentId = vm.ParentId
                    };
                   
                    db.UploadCenters.Add(newFolder);
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
                var model = db.UploadCenters.FirstOrDefault(x=>x.Id==Id);
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