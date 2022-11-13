using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
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

    public class GeneralRecordsController : Controller
    {
        // GET: GeneralRecords
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region عرض القيود الحرة المسجلة سابقا
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.GeneralRecords.Where(x => !x.IsDeleted);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list = list.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.TransactionDate) <= dtTo.Date);

                return Json(new
                {
                    data = list.Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", AccountFrom = x.AccountTreeFrom.AccountName, AccountTreeFromId = x.AccountTreeFromId, AccountTreeToId = x.AccountTreeToId, AccountTo = x.AccountTreeTo.AccountName, TransactionDate = x.TransactionDate.ToString(), Amount = x.Amount, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new
                {
                    data = list.Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", AccountFrom = x.AccountTreeFrom.AccountName, AccountTreeFromId = x.AccountTreeFromId, AccountTreeToId = x.AccountTreeToId, AccountTo = x.AccountTreeTo.AccountName, TransactionDate = x.TransactionDate.ToString(), Amount = x.Amount, Actions = n, Num = n }).ToList()
                }, JsonRequestBehavior.AllowGet);

            
            
            //var items = db.GeneralRecords.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", AccountFrom = x.AccountsTree.AccountName, AccountTo = x.AccountsTree1.AccountName, TransactionDate = x.TransactionDate.ToString(), Amount = x.Amount, Actions = n, Num = n }).ToList();
            //return Json(new
            //{
            //    data = items
            //}, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region ادارة القيود 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.GeneralRecords.Where(x => x.Id == id).FirstOrDefault();
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    return View(model);
                }
                else
                    return RedirectToAction("Index");

            }
            else
            {                   // add
                                // add
            ViewBag.BranchId = new SelectList(branches, "Id", "Name",1);
            ViewBag.LastRow = db.GeneralRecords.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            return View(new GeneralRecord { TransactionDate = Utility.GetDateTime() });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(GeneralRecord vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.BranchId == null || vm.TransactionDate == null || (vm.AccountTreeFromId == null && vm.AccountTreeToId == null) || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                vm.TransactionDate = vm.TransactionDate.Value.AddHours(Utility.GetDateTime().Hour).AddMinutes(Utility.GetDateTime().Minute);
                //التأكد من عدم وجود اى ابناء للحساب المختار (اخر مستوى فى السشجرة)
                if(vm.AccountTreeFromId!=null)
                 if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeFromId))
                    return Json(new { isValid = false, message = "تأكد اختيار حساب فرعى صحيح ... (من حساب)" });
                if (vm.AccountTreeToId != null)
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeToId))
                    return Json(new { isValid = false, message = "تأكد اختيار حساب فرعى صحيح ... (إلي حساب)" });

                if (vm.Id != Guid.Empty)
                {
                    var model = db.GeneralRecords.Where(x => x.Id == vm.Id).FirstOrDefault();
                    model.AccountTreeFromId = vm.AccountTreeFromId;
                    model.AccountTreeToId = vm.AccountTreeToId;
                    model.Amount = vm.Amount;
                    model.BranchId = vm.BranchId;
                    model.TransactionDate = vm.TransactionDate;
                    model.Notes = vm.Notes;
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    var newGeneralRecord = new GeneralRecord
                    {
                        AccountTreeFromId = vm.AccountTreeFromId,
                        AccountTreeToId = vm.AccountTreeToId,
                        Amount = vm.Amount,
                        BranchId = vm.BranchId,
                        TransactionDate = vm.TransactionDate,
                        Notes = vm.Notes
                    };

                    db.GeneralRecords.Add(newGeneralRecord);

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
                var model = db.GeneralRecords.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
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

        #endregion

        #region اعتماد القيد 
        [HttpPost]
        public ActionResult Approval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // use Transactions
                    var generalRecord = db.GeneralRecords.Where(x => x.Id == Id).FirstOrDefault();
                    //التأكد من عدم تكرار اعتماد القيد
                    if (GeneralDailyService.GeneralDailaiyExists(generalRecord.Id, (int)TransactionsTypesCl.FreeRestrictions))
                        return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(generalRecord.AccountTreeFromId))
                        return Json(new { isValid = false, message = "الحساب من ليس بحساب فرعى" });
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(generalRecord.AccountTreeToId))
                        return Json(new { isValid = false, message = "الحساب الى ليس بحساب فرعى" });

                    // من حساب
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = generalRecord.AccountTreeFromId,
                        Debit = generalRecord.Amount,
                        Notes = generalRecord.Notes,
                        BranchId = generalRecord.BranchId,
                        TransactionDate = generalRecord.TransactionDate,
                        TransactionId = generalRecord.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.FreeRestrictions
                    });
                    //إلي حساب 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = generalRecord.AccountTreeToId,
                        Credit = generalRecord.Amount,
                        Notes = generalRecord.Notes,
                        BranchId = generalRecord.BranchId,
                        TransactionDate = generalRecord.TransactionDate,
                        TransactionId = generalRecord.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.FreeRestrictions
                    });

                    //تحديث حالة الاعتماد 
                    generalRecord.IsApproval = true;
                    db.Entry(generalRecord).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد القيد بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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
                var model = db.GeneralRecords.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.FreeRestrictions).ToList();
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

        #endregion

        #region بحث القيود 
        public ActionResult Search()
        {
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.FinancialYearStartDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                ViewBag.FinancialYearEndDate = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
            }
            else
                ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";
            
            ViewBag.TransactionTypeId = new SelectList(db.TransactionsTypes.Where(x => !x.IsDeleted), "Id", "Name");
            return View();
        }
        public ActionResult SearchData(string dFrom, string dTo, Guid? transactionId, int? transactionTypeId, Guid? accountTreeId,int isFirstInitPage)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            List<GeneralDayDto> list;
            bool isFirstInit=false;
            if (isFirstInitPage == 1)
                isFirstInit = true;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                list =GeneralDailyService.SearchGeneralDailies(dtFrom, dtTo, transactionId,transactionTypeId,accountTreeId, isFirstInit);
            }
            else
                list = GeneralDailyService.SearchGeneralDailies(null, null, transactionId, transactionTypeId, accountTreeId, isFirstInit);

            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

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