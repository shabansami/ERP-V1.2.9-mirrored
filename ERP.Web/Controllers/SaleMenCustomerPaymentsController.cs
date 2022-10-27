﻿using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
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

    public class SaleMenCustomerPaymentsController : Controller
    {
        // GET: SaleMenCustomerPayments
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

            ViewBag.EmployeeId = auth.CookieValues.EmployeeId;
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
            int? n = null;
            return Json(new
            {
                data = db.CustomerPayments.Where(x => !x.IsDeleted&&x.BySaleMen&&x.EmployeeId==auth.CookieValues.EmployeeId).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id,  CustomerName = x.PersonCustomer.Name, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", Amount = x.Amount, Notes = x.Notes, PaymentDate = x.PaymentDate.ToString(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());
            if (auth.CookieValues.StoreId == null) //فى حالة ان الموظف غير محدد له مخزن اى انه ليس مندوب
            {
                ViewBagBase();
                ViewBag.ErrorMsg = "لابد من تحديد مخزن للمندوب اولا لعرض هذه الشاشة";
                return View(new CustomerPayment());
            }           
            if (auth.CookieValues.EmployeeId == null) //فى حالة ان الموظف غير محدد له مخزن اى انه ليس مندوب
            {
                ViewBagBase();
                ViewBag.ErrorMsg = "خطأ فى بيانات المندوب ..لا يمكن عرض هذه الشاشة";
                return View(new CustomerPayment());
            }
            ViewBag.SaleMenEmployeeId = auth.CookieValues.EmployeeId;

            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==id);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", model.PersonCustomer.PersonCategoryId);
                    ViewBag.CustomerId = new SelectList(EmployeeService.GetCustomerByCategory(model.PersonCustomer.PersonCategoryId, auth.CookieValues.EmployeeId,null), "Id", "Name", model.CustomerId);


                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                ViewBagBase();
                return View(new CustomerPayment() { PaymentDate = Utility.GetDateTime() });
            }
        }

        private void ViewBagBase()
        {
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
        }

        [HttpPost]
        public JsonResult CreateEdit(CustomerPayment vm, Guid? SaleMenEmployeeId)
        {
            if (ModelState.IsValid)
            {
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (vm.CustomerId == null || vm.Amount == 0 || vm.PaymentDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (SaleMenEmployeeId == null)
                    return Json(new { isValid = false, message = "خطأ فى بيانات المندوب ..لا يمكن عرض هذه الشاشة"});
                //فى حالة ادخال رقم فاتورة توريد يجب التأكد من صحة الرقم المدخلو وانه من ضمن فواتير التوريد 
                if (vm.SellInvoiceId != null)
                {
                    if (int.TryParse(vm.SellInvoiceId.ToString(), out var id))
                    {
                        var sellIsExists = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == vm.SellInvoiceId).Count();
                        if (sellIsExists == 0)
                            return Json(new { isValid = false, message = "خطأ .... رقم الفاتورة غير موجود فى فواتير البيع" });
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال رقم فاتورة البيع بشكل صحيح " });

                }
                var isInsert = false;

                if (vm.Id != Guid.Empty)
                {
                    //if (db.CustomerPayments.Where(x => !x.IsDeleted && x.Id != vm.Id).Count() > 0)
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });

                    var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==vm.Id);
                    model.CustomerId = vm.CustomerId;
                    model.Amount = vm.Amount;
                    model.Notes = vm.Notes;
                    model.BySaleMen =true;
                    //فى حالة ان البيع من خلال مندوب
                            model.EmployeeId = SaleMenEmployeeId;

                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    //if (db.CustomerPayments.Where(x => !x.IsDeleted && x.SupplierId == vm.SupplierId).Count() > 0) ///??
                    //    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                    isInsert = true;
                    var model = new CustomerPayment
                    {
                        BranchId = auth.CookieValues.BranchId,
                        SafeId = vm.SafeId,
                        CustomerId = vm.CustomerId,
                        Amount = vm.Amount,
                        Notes = vm.Notes,
                        PaymentDate = vm.PaymentDate,
                        SellInvoiceId = vm.SellInvoiceId,
                        BySaleMen = true
                    };
                    //فى حالة ان البيع من خلال مندوب
                     model.EmployeeId = SaleMenEmployeeId;
                    db.CustomerPayments.Add(model);

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
                var model = db.CustomerPayments.FirstOrDefault(x=>x.Id==Id);
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