using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class UploadCenterTypeFilesController : Controller
    {
        // GET: UploadCenterTypeFiles
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();


        [HttpGet]
        public ActionResult Index(string typ, string refGid)
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            Guid referenceGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(typ) && Guid.TryParse(refGid, out referenceGuid))
            {
                // التأكد من تعريف المجلدات الرئيسية فى شاشة الاعدادات
                if (!GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.UploadCenterFiles))
                {
                    ViewBag.redirect = "setting";
                    return View(new UploadCenter());
                }
                //
                switch (int.Parse(typ))
                {
                    case (int)UploalCenterTypeCl.PurchaseInvoice:
                        ViewBag.TitlePage = "رفع ملفات فواتير التوريد";
                        var uploadCenterModel = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var purchaseInvoice = db.PurchaseInvoices.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterModel == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseInvoice).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "فاتورة توريد رقم " + purchaseInvoice.InvoiceNumber;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseInvoice
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseInvoice, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterModel.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseInvoice, UploadCenterParent = new UploadCenter { Name = uploadCenterModel.Name } });
                    case (int)UploalCenterTypeCl.PurchaseBackInvoice:
                        ViewBag.TitlePage = "رفع ملفات فواتير مرتجع التوريد";
                        var uploadCenterModelR = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var purchaseInvoiceR = db.PurchaseBackInvoices.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterModelR == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseBackInvoice).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "فاتورة مرتجع توريد رقم " + purchaseInvoiceR.InvoiceNumber;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseBackInvoice
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseBackInvoice, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterModelR.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.PurchaseBackInvoice, UploadCenterParent = new UploadCenter { Name = uploadCenterModelR.Name } });

                    case (int)UploalCenterTypeCl.SellInvoice:
                        ViewBag.TitlePage = "رفع ملفات فواتير البيع";
                        var uploadCenterModelSell = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var sellInvoice = db.SellInvoices.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterModelSell == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellInvoice).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "فاتورة بيع رقم " + sellInvoice.InvoiceNumber;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.SellInvoice
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.SellInvoice, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterModelSell.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.SellInvoice, UploadCenterParent = new UploadCenter { Name = uploadCenterModelSell.Name } });
                    case (int)UploalCenterTypeCl.SellBackInvoice:
                        ViewBag.TitlePage = "رفع ملفات فواتير مرتجع البيع";
                        var uploadCenterModelSellR = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var sellInvoiceR = db.SellBackInvoices.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterModelSellR == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellBackInvoice).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "فاتورة مرتجع بيع رقم " + sellInvoiceR.InvoiceNumber;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.SellBackInvoice
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            string path = "";
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.SellBackInvoice, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterModelSellR.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.SellBackInvoice, UploadCenterParent = new UploadCenter { Name = uploadCenterModelSellR.Name } });
                    case (int)UploalCenterTypeCl.Employee:
                        ViewBag.TitlePage = "رفع ملفات موظف";
                        var uploadCenterModelEmpR = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var employeeR = db.Employees.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterModelEmpR == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterEmployee).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = employeeR.Person.Name;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.Employee
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Employee, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterModelEmpR.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Employee, UploadCenterParent = new UploadCenter { Name = uploadCenterModelEmpR.Name } });

                    //الموردين
                    case (int)UploalCenterTypeCl.Supplier:
                        ViewBag.TitlePage = "رفع ملفات الموردين";
                        var uploadCenterSupp = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var supplier = db.Persons.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterSupp == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSupplier).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = supplier.Name;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.Supplier
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Supplier, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterSupp.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Supplier, UploadCenterParent = new UploadCenter { Name = uploadCenterSupp.Name } });

                    //العملاء
                    case (int)UploalCenterTypeCl.Customer:
                        ViewBag.TitlePage = "رفع ملفات العملاء";
                        var uploadCenterCust = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var customer = db.Persons.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterCust == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCustomer).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = customer.Name;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.Customer
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Customer, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterCust.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Customer, UploadCenterParent = new UploadCenter { Name = uploadCenterCust.Name } });

                    //الاقساط
                    case (int)UploalCenterTypeCl.Installment:
                        ViewBag.TitlePage = "رفع ملفات الاقساط";
                        var uploadCenterInstall = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var intallment = db.Installments.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterInstall == null)
                        {
                            // get defult folder from general setting 
                            var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterInstallment).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "فاتورة قسط رقم " + intallment.Id;
                            var uploadCenter = new UploadCenter
                            {
                                IsFolder = true,
                                Name = folderName,
                                ParentId = parent,
                                ReferenceGuid = referenceGuid,
                                UploadCenterTypeId = (int)UploalCenterTypeCl.Installment
                            };
                            db.UploadCenters.Add(uploadCenter);
                            int aff = db.SaveChanges(auth.CookieValues.UserId);
                            //string path = "";
                            if (aff == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }

                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Installment, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterInstall.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.Installment, UploadCenterParent = new UploadCenter { Name = uploadCenterInstall.Name } });

                    //اوامر الانتاج 
                    case (int)UploalCenterTypeCl.ProductionOrder:
                        ViewBag.TitlePage = "رفع ملفات لأمر الانتاج";
                        var uploadCenterProdOrder = db.UploadCenters.Where(x => x.ReferenceGuid == referenceGuid && x.IsFolder).FirstOrDefault();
                        var prodOrder = db.ProductionOrders.Where(x => x.Id == referenceGuid).FirstOrDefault();
                        // add first file (purchase guid not exsits yet)
                        if (uploadCenterProdOrder == null)
                        {
                            // get defult folder from general setting 
                            //var parent = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCustomer).FirstOrDefault().SValue);
                            //create directory 
                            var folderName = "امر انتاج رقم " + prodOrder.OrderNumber;
                            var uploadCenterParent = new UploadCenter
                            {
                                IsFolder = true,
                                Name = "أوامر الانتاج",
                                ParentId = null,
                                ReferenceGuid = null,
                                UploadCenterTypeId = null
                            };
                            db.UploadCenters.Add(uploadCenterParent);
                            UploadCenter uploadCenter = new UploadCenter();
                            if (db.SaveChanges(auth.CookieValues.UserId)>0)
                            {
                                uploadCenter = new UploadCenter
                                {
                                    IsFolder = true,
                                    Name = folderName,
                                    ParentId = uploadCenterParent.Id,
                                    ReferenceGuid = referenceGuid,
                                    UploadCenterTypeId = (int)UploalCenterTypeCl.ProductionOrder
                                };
                            }
                            
                            db.UploadCenters.Add(uploadCenter);
                            //string path = "";
                            if (db.SaveChanges(auth.CookieValues.UserId) == 0)
                            {
                                ViewBag.redirect = "errorCreateDir";
                                return View(new UploadCenter());
                                //path = Server.MapPath($"~/Files/UploadCenter/{ intallment.Id}/");
                                //Directory.CreateDirectory(path);
                            }
                            return View(new UploadCenter { ParentId = uploadCenter.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.ProductionOrder, UploadCenterParent = new UploadCenter { Name = folderName } });
                        }
                        else
                            return View(new UploadCenter { ParentId = uploadCenterProdOrder.Id, ReferenceGuid = referenceGuid, UploadCenterTypeId = (int)UploalCenterTypeCl.ProductionOrder, UploadCenterParent = new UploadCenter { Name = uploadCenterProdOrder.Name } });

                    default:
                        return View(new UploadCenter());
                }
            }
            else

            {
                ViewBag.redirect = "noQueryString";
                return View(new UploadCenter());
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
                    data = db.UploadCenters.Where(x => !x.IsDeleted && !x.IsFolder && x.ParentId == parentId).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, TitleFile = x.Name, FileName = x.FileName, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet); ;

            }
            else
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet); ;


        }
        [HttpPost]
        public JsonResult Index(UploadCenter vm, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.Name))
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                var aff = 0;
                if (db.UploadCenters.Where(x => !x.IsDeleted && x.Name == vm.Name && x.ReferenceGuid == vm.ReferenceGuid).Count() > 0)
                    return Json(new { isValid = false, message = "مسمى الملف موجود مسبقا" });

                if (file == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار ملف" });
                var newFolder = new UploadCenter
                {
                    Name = vm.Name,
                    IsFolder = false,
                    ParentId = vm.ParentId,
                    ReferenceGuid = vm.ReferenceGuid
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

                if (aff > 0)
                {
                    return Json(new { isValid = true, message = "تم رفع الملف بنجاح" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

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