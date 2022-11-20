using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
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

    public class MaintenancesAccountantsController : Controller
    {
        // GET: MaintenancesAccountants
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region ادارة فواتير الصيانه
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Maintenances.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceGuid = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.Person.Name, Safy = x.Safy, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", IsApprovalAccountant = x.IsApprovalAccountant, InvoAccountant = x.IsApprovalAccountant ? "1" : "2", CaseName = x.MaintenanceCas != null ? x.MaintenanceCas.Name : "", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region اعتماد محاسبى فاتورة صيانة 
        public ActionResult ApprovalAccountant(string invoGuid)
        {

            Guid invGuid;
            if (Guid.TryParse(invoGuid, out invGuid))
            {
                var vm = db.Maintenances.Where(x => x.Id == invGuid).FirstOrDefault();
                if (vm != null)
                {
                    //ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
                    ViewBag.StoreReceiptId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId && !x.IsDamages), "Id", "Name");
                    vm.ReceiptDate = Utility.GetDateTime();
                    return View(vm);
                }
                else
                    return View("Index");
            }
            else
                return View("Index");

        }

        [HttpPost]
        public JsonResult ApprovalAccountant(Maintenance vm, string receiptStatus)
        {
            try
            {
                if (vm.PaymentTypeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد فى شاشة ادارة فاتورة صيانه" });
                if (vm.ReceiptDate == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار تاريخ التسليم" });

                if (vm.SafeId == null && vm.BankAccountId == null && vm.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                    return Json(new { isValid = false, message = "تأكد من اختيار حساب الدفع (خزينة/بنكى)" });
                if (string.IsNullOrEmpty(receiptStatus))
                    return Json(new { isValid = false, message = "تأكد من اختيار حالة التسليم" });
                if (receiptStatus == "2" && vm.StoreReceiptId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار المخزن المحول إليه" });

                var model = db.Maintenances.Where(x => x.Id == vm.Id).FirstOrDefault();
                if (model != null)
                {
                    //model.PaymentTypeId = vm.PaymentTypeId;
                    model.SafeId = vm.SafeId;
                    model.BankAccountId = vm.BankAccountId;
                    model.StoreReceiptId = vm.StoreReceiptId;
                    model.IsApprovalAccountant = true;
                    model.ReceiptDate = vm.ReceiptDate.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                    model.MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalAccountant;
                    //اضافة حالة الطلب 
                    db.MaintenanceCaseHistories.Add(new MaintenanceCaseHistory
                    {
                        Maintenance = model,
                        MaintenanceCaseId = (int)MaintenanceCaseCl.ApprovalAccountant
                    });

                    db.Entry(model).State = EntityState.Modified;
                    //===========================
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        return Json(new { isValid = true, message = "تم الاعتماد المحاسبى بنجاح" });
                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }

        }

        #endregion
        #region عرض بيانات فاتورة صيانة بالتفصيل
        public ActionResult ShowMaintenance(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.Maintenances.Where(x => x.Id == invoGuid && x.MaintenanceDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.MaintenanceDetails = vm.MaintenanceDetails.Where(x => !x.IsDeleted).ToList();
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