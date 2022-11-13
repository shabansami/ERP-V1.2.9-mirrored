using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.DAL;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class VoucherReceiptsController : Controller
    {
        // GET: VoucherReceipts
        // Voucher Payment سندات صرف  مورد
        // Voucher Receipt سندات دفع عميل

        VTSaleEntities db;
        VTSAuth auth;
        StoreService storeService;
        public VoucherReceiptsController()
        {
            db = new VTSaleEntities();
            auth = new VTSAuth();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {
            ViewBag.VoucherReceipts = new SelectList(AccountTreeService.GetVouchers(db, true), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string accountTreeToId, string isApprovalStatus, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.Vouchers.Where(x => !x.IsDeleted && !x.IsVoucherPayment);
            int isAppStatus;
            if (!string.IsNullOrEmpty(accountTreeToId))
            {
                if (Guid.TryParse(accountTreeToId, out Guid accountTo))
                    list = list.Where(x => x.AccountTreeToId == accountTo);
            }
            if (!string.IsNullOrEmpty(isApprovalStatus))
            {
                if (int.TryParse(isApprovalStatus, out isAppStatus))
                    if (isAppStatus == 1)
                        list = list.Where(x => x.IsApproval);
                    else if (isAppStatus == 2)
                        list = list.Where(x => !x.IsApproval);
            }
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.VoucherDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.VoucherDate) <= dtTo.Date);

            return Json(new
            {
                data = list.Select(x => new { Id = x.Id, AccountTreeFromName = x.AccountsTreeFrom.AccountName, AccountTreeToName = x.AccountsTreeTo.AccountName, IsApproval = x.IsApproval, Amount = x.Amount, Notes = x.Notes, VoucherDate = x.VoucherDate.ToString(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Vouchers.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    ViewBag.AccountTreeFromId = new SelectList(AccountTreeService.GetVouchers(db, false), "Id", "Name", model.AccountTreeFromId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");

            }
            else
            {                   // add

                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.AccountTreeFromId = new SelectList(AccountTreeService.GetVouchers(db, false), "Id", "Name");
                ViewBag.LastRow = db.Vouchers.Where(x => !x.IsDeleted && !x.IsVoucherPayment).OrderByDescending(x => x.Id).FirstOrDefault();
                return View(new Voucher() { VoucherDate = Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Voucher vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.AccountTreeFromId == null || vm.AccountTreeToId == null || vm.Amount == 0 || vm.BranchId == null || vm.VoucherDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeFromId))
                    return Json(new { isValid = false, message = "من حساب .. ليس بحساب فرعى" });
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeToId))
                    return Json(new { isValid = false, message = "الى حساب .. ليس بحساب فرعى" });

                vm.VoucherDate = vm.VoucherDate.Value.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                if (vm.Id != Guid.Empty)
                {
                    var model = db.Vouchers.Where(x => x.Id == vm.Id).FirstOrDefault();
                    model.Amount = vm.Amount;
                    model.Notes = vm.Notes;
                    model.BranchId = vm.BranchId;
                    model.AccountTreeFromId = vm.AccountTreeFromId;
                    model.AccountTreeToId = vm.AccountTreeToId;
                    model.VoucherDate = vm.VoucherDate;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    var model = new Voucher
                    {
                        BranchId = vm.BranchId,
                        AccountTreeFromId = vm.AccountTreeFromId,
                        AccountTreeToId = vm.AccountTreeToId,
                        Amount = vm.Amount,
                        Notes = vm.Notes,
                        VoucherDate = vm.VoucherDate,
                        IsVoucherPayment = false,
                    };
                    db.Vouchers.Add(model);

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
                var model = db.Vouchers.Where(x => x.Id == Id).FirstOrDefault();
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
        [HttpPost]
        public ActionResult Approval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    var model = db.Vouchers.Where(x => x.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(model.AccountTreeFromId))
                            return Json(new { isValid = false, message = "من حساب .. ليس بحساب فرعى" });
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(model.AccountTreeToId))
                            return Json(new { isValid = false, message = "الى حساب .. ليس بحساب فرعى" });
                        //التأكد من عدم تكرار اعتماد القيد
                        if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.VoucherReceipt))
                            return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                        //من ح/ 
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.AccountTreeFromId,
                            Debit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"سند قبض من حساب {model.AccountsTreeFrom.AccountName} الى حساب {model.AccountsTreeTo.AccountName}  {model.Notes}",
                            TransactionDate = model.VoucherDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                        });

                        // الى ح/ حساب 
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.AccountTreeToId,
                            Credit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"سند قبض من حساب {model.AccountsTreeFrom.AccountName} الى حساب {model.AccountsTreeTo.AccountName}  {model.Notes}",
                            TransactionDate = model.VoucherDate,
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                        });

                        //تحديث حالة الاعتماد 
                        model.IsApproval = true;
                        db.Entry(model).State = EntityState.Modified;

                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم اعتماد سند القبض بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من اختيار العملية " });

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

        }
        [HttpPost]
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Vouchers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    if (TempData["userInfo"] != null)
                        auth = TempData["userInfo"] as VTSAuth;
                    else
                        RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.VoucherReceipt).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم فك الاعتماد بنجاح" });
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