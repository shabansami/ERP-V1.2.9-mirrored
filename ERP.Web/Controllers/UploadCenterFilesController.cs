using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class UploadCenterFilesController : Controller
    {
        // GET: UploadCenterFiles
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
                data = db.UploadCenters.Where(x => !x.IsDeleted && !x.IsFolder).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, FolderName = x.UploadCenterParent.Name, FileTitle = x.Name, FileName = x.FileName, IsFolder = x.IsFolder, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit(Guid? fd)
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
                ViewBag.LastRow = db.UploadCenters.Where(x => !x.IsDeleted && !x.IsFolder && (x.ParentId == fd && fd.HasValue)).OrderByDescending(x => x.CreatedOn).ToList();
                return View(new UploadCenter() { ParentId = fd });
            }


        }
        public ActionResult GetByInvoGuid(string parntId)
        {
            int? n = null;
            Guid parentId;
            if (Guid.TryParse(parntId, out parentId))
            {
                return Json(new
                {
                    data = db.UploadCenters.Where(x => !x.IsDeleted && !x.IsFolder && x.ParentId == parentId).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, TitleFile = x.Name, FileName = x.FileName, FolderName = x.UploadCenterParent != null ? x.UploadCenterParent.Name : null, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;

            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet); ;


        }
        [HttpPost]
        public JsonResult CreateEdit(UploadCenter vm, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال اسم الملف" });


                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                var aff = 0;
                if (vm.Id != Guid.Empty)
                {
                    //if (db.UploadCenters.Where(x => !x.IsDeleted && x.Id != vm.Id && x.Name == vm.Name).Count() > 0)
                    //    return Json(new { isValid = false, message = "مسمى الملف موجود مسبقا" });

                    var model = db.UploadCenters.FirstOrDefault(x=>x.Id==vm.Id);

                    if (file != null)
                    {
                        var path = Server.MapPath($"~/Files/UploadCenter/{vm.Id}/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        else
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(path);
                            FileInfo[] files = dirInfo.GetFiles();
                            foreach (var fle in files)
                            {
                                fle.Delete();
                            }

                            var fullPath = Path.Combine(Server.MapPath($"~/Files/UploadCenter/{vm.Id}/"), Path.GetFileName(file.FileName));
                            file.SaveAs(fullPath);
                            model.FileName = file.FileName;
                        }
                    }
                    model.Name = vm.Name;
                    model.ParentId = vm.ParentId;

                    db.Entry(model).State = EntityState.Modified;
                    aff = db.SaveChanges(auth.CookieValues.UserId);
                }
                else
                {
                    //if (db.UploadCenters.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                    //    return Json(new { isValid = false, message = "مسمى الملف موجود مسبقا" });
                    if (file == null)
                        return Json(new { isValid = false, message = "تأكد من اختيار ملف" });
                    isInsert = true;
                    var GuidByFolderId = db.UploadCenters.Where(x => x.Id == vm.ParentId).FirstOrDefault().ReferenceGuid;
                    var newFolder = new UploadCenter
                    {
                        Name = vm.Name,
                        IsFolder = false,
                        ParentId = vm.ParentId,
                        ReferenceGuid = GuidByFolderId
                    };
                    if (file != null)
                        newFolder.FileName = file.FileName;

                    db.UploadCenters.Add(newFolder);
                    aff = db.SaveChanges(auth.CookieValues.UserId);
                    if (file != null && aff > 0)
                    {
                        var path = Server.MapPath($"~/Files/UploadCenter/{newFolder.Id}/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        var fullPath = Path.Combine(Server.MapPath($"~/Files/UploadCenter/{newFolder.Id}/"), Path.GetFileName(file.FileName));
                        file.SaveAs(fullPath);
                    }

                }
                if (aff > 0)
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
                    {
                        var path = Server.MapPath($"~/Files/UploadCenter/{model.Id}/");
                        if (Directory.Exists(path))
                            Directory.Delete(path, true);
                        return Json(new { isValid = true, message = "تم الحذف بنجاح" });

                    }
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