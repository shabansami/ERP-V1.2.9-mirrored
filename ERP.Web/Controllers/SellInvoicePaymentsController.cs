using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
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
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SellInvoicePaymentsController : Controller
    {
        // GET: SellInvoicePayments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public static string DSPyments { get; set; }

        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted&&x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellInvoicePayments.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, InvoiceNumber = x.SellInvoice.InvoiceNumber,CustomerName=x.SellInvoice.PersonCustomer.Name, InvoiceDate = x.SellInvoice.InvoiceDate.ToString(), OperationDate = x.OperationDate.ToString(),Notes=x.Notes,Amount=x.Amount, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }


        [Authorization]
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //if (TempData["model"] != null) //edit
            //{
            //    Guid id;
            //    if (Guid.TryParse(TempData["model"].ToString(), out id))
            //    {
            //        var model = db.SellInvoicePayments.Where(x => x.Id == id).FirstOrDefault();

            //        ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name",model.SellInvoice.CustomerId);
            //        ViewBag.SellInvoiceId = new SelectList(new List<DropDownList>() { new DropDownList {Id=model.SellInvoiceId, Name = $"فاتورة رقم : {model.SellInvoice.InvoiceNumber} | بتاريخ {model.SellInvoice.InvoiceDate.ToString("yyyy-MM-dd")}" } }, "Id", "Name", model.SellInvoiceId);
            //        var amount = model.SellInvoice.Safy;
            //        var totalInvoiceAmount = db.SellInvoicePayments.Where(x => !x.IsDeleted && x.SellInvoiceId == model.SellInvoiceId).DefaultIfEmpty().Sum(y => (double?)y.Amount ?? 0);
            //        var currentAmount = model.Amount;
            //        var remindAmount = amount - (totalInvoiceAmount - currentAmount);

            //        ViewBag.InvoiceAmount = amount;
            //        ViewBag.TotalInvoiceAmount = totalInvoiceAmount;
            //        ViewBag.RemindAmount = remindAmount;

            //        return View(model);
            //    }
            //    else
            //        return RedirectToAction("Index");
            //}
            //else
            //{                   // add
                ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
                    ViewBag.SellInvoiceId = new SelectList(new List<DropDownList>(), "Id", "Name");
            ViewBag.InvoiceAmount = 0;
            ViewBag.TotalInvoiceAmount = 0;
            ViewBag.RemindAmount = 0;
            ViewBag.LastRow = db.SellInvoicePayments.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new SellInvoicePayment() { OperationDate=Utility.GetDateTime()});
            //}
        }

        [HttpPost]
        public JsonResult CreateEdit(SellInvoicePayment vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.SellInvoiceId == null||vm.Amount==0||vm.OperationDate==null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
                //التأكد من ان رقم الفاتورة المدخل موجود ويتبع فواتير الاجل والجزئى فقط 
                var sellInvoice = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == vm.SellInvoiceId).FirstOrDefault();
                if (sellInvoice!=null)
                {
                    if (sellInvoice.PaymentTypeId==(int)PaymentTypeCl.Partial||sellInvoice.PaymentTypeId==(int)PaymentTypeCl.Deferred)
                    {
                        var safy = sellInvoice.Safy;
                        if (vm.Id != Guid.Empty)
                        {
                            var model = db.SellInvoicePayments.FirstOrDefault(x => x.Id == vm.Id);
                            var prevousPayment = db.SellInvoicePayments.Where(x => !x.IsDeleted && x.SellInvoiceId == sellInvoice.Id && x.Id != model.Id).DefaultIfEmpty().Sum(y => (double?)y.Amount??0);
                            //التاكد من ان اجمالى المبالغ السابقة بالاضافة الى المبلغ المسدد الحالى يساوى او اقل من قيمة الفاتورة
                            if (prevousPayment+vm.Amount>safy)
                                return Json(new { isValid = false, message = "المبالع المسددة اكبر من قيمة الفاتورة" });

                            model.SellInvoiceId = vm.SellInvoiceId;
                            model.OperationDate = vm.OperationDate;
                            model.Amount=vm.Amount;
                            model.Notes = vm.Notes;
                            db.Entry(model).State = EntityState.Modified;
                        }
                        else
                        {
                            isInsert = true;
                            var prevousPayment = db.SellInvoicePayments.Where(x => !x.IsDeleted && x.SellInvoiceId == sellInvoice.Id).DefaultIfEmpty().Sum(y => (double?)y.Amount ?? 0);
                            //التاكد من ان اجمالى المبالغ السابقة بالاضافة الى المبلغ المسدد الحالى يساوى او اقل من قيمة الفاتورة
                            if (prevousPayment + vm.Amount > safy)
                                return Json(new { isValid = false, message = "المبالع المسددة اكبر من قيمة الفاتورة" });
                            db.SellInvoicePayments.Add(new SellInvoicePayment
                            {
                                SellInvoiceId = vm.SellInvoiceId,
                                OperationDate = vm.OperationDate,
                                Amount = vm.Amount,
                                Notes = vm.Notes
                            });
                        }
                    }
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
                var model = db.SellInvoicePayments.FirstOrDefault(x => x.Id == Id);
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