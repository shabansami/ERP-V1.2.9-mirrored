using Newtonsoft.Json;
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

    public class GeneralRecordComplexController : Controller
    {
        // GET: GeneralRecordComplex
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DS { get; set; }

        #region ادارة القيود 
        public ActionResult GetDSGenaralComplex()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new GeneralRecordVM()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<GeneralRecordVM>>(DS)
                }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddGenaralComplex(string ComplexAccountTreeId, string ComplexAmount, string ComplexNotes, string ComplexDebitCredit, string DT_Datasource)
        {
            List<GeneralRecordVM> deDS = new List<GeneralRecordVM>();
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<GeneralRecordVM>>(DT_Datasource);
            double amount;
            Guid accountTreeId;
            int debitCreditId;
            if (!double.TryParse(ComplexAmount, out amount) || !Guid.TryParse(ComplexAccountTreeId, out accountTreeId) || !int.TryParse(ComplexDebitCredit, out debitCreditId))
                return Json(new { isValid = false, message = " تأكد من ادخال البيانات المطلوبة بشكل صحيح " }, JsonRequestBehavior.AllowGet);

            //التأكد من عدم وجود اى ابناء للحساب المختار (اخر مستوى فى السشجرة)
            if (AccountTreeService.CheckAccountTreeIdHasChilds(accountTreeId))
                return Json(new { isValid = false, message = "تأكد اختيار حساب فرعى صحيح.. الحساب المختار ليس بحساب فرعى)" });

            var accountTree = db.AccountsTrees.Where(x => x.Id == accountTreeId).FirstOrDefault();
            var newGeneral = new GeneralRecordVM
            {
                ComplexAccountTreeId = accountTreeId,
                ComplexAccountTreeName = accountTree.AccountName,
                ComplexAccountTreeNum = accountTree.AccountNumber,
                ComplexDebitCredit = debitCreditId,
                ComplexDebitName = debitCreditId == 1 ? amount : 0,
                ComplexCreditName = debitCreditId == 2 ? amount : 0,
                ComplexNotes = ComplexNotes,
                ComplexAmount = amount
            };

            deDS.Add(newGeneral);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateEdit()
        {
            // add
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name");
            ViewBag.ComplexDebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };

            return View(new GeneralRecordVM { TransactionDate = Utility.GetDateTime() });
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(GeneralRecordVM vm, string DT_Datasource, bool? isApproval)
        {
            if (ModelState.IsValid)
            {
                if (vm.BranchId == null || vm.TransactionDate == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                vm.TransactionDate = vm.TransactionDate.Value.AddHours(Utility.GetDateTime().Hour).AddMinutes(Utility.GetDateTime().Minute);

                List<GeneralRecordVM> deDS = new List<GeneralRecordVM>();
                List<GeneralRecord> newGeneralRecords = new List<GeneralRecord>();

                if (DT_Datasource != null)
                {
                    deDS = JsonConvert.DeserializeObject<List<GeneralRecordVM>>(DT_Datasource);
                    if (deDS.Any(x => x.ComplexDebitCredit == null))
                        return Json(new { isValid = false, message = "تأكد من اختيار حالة الحساب(مدين/دائن)" });
                    if (deDS.Any(x => x.ComplexAmount == 0))
                        return Json(new { isValid = false, message = "تأكد من ادخال المبلغ للحسابات)" });

                    //التاكد من ان القيود موزونة
                    if (deDS.Sum(x => x.ComplexDebitName) != deDS.Sum(x => x.ComplexCreditName))
                        return Json(new { isValid = false, message = "القيود غير موزونة" });

                    newGeneralRecords = deDS.Select(x => new GeneralRecord
                    {
                        AccountTreeFromId = x.ComplexDebitCredit == 1 ? x.ComplexAccountTreeId : null,
                        AccountTreeToId = x.ComplexDebitCredit == 2 ? x.ComplexAccountTreeId : null,
                        Amount = x.ComplexAmount,
                        BranchId = vm.BranchId,
                        TransactionDate = vm.TransactionDate,
                        Notes = x.ComplexNotes

                    }).ToList();
                }

                db.GeneralRecords.AddRange(newGeneralRecords);

                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    DS = null;
                    //اعتماد القيود فى حالة الضغط على حفظ واعتماد
                    if (isApproval == true)
                    {
                        //    //تسجيل القيود
                        // General Dailies
                        if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                        {
                            // use Transactions
                            foreach (var item in newGeneralRecords)
                            {
                                var generalRecord = db.GeneralRecords.Where(x => x.Id == item.Id).FirstOrDefault();
                                if (item.AccountTreeFromId != null)
                                {
                                    //التأكد من عدم تكرار اعتماد القيد
                                    if (GeneralDailyService.GeneralDailaiyExists(generalRecord.Id, (int)TransactionsTypesCl.FreeRestrictions))
                                        continue;
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
                                }
                                else if (item.AccountTreeToId != null)
                                {
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
                                }
                                else
                                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية.. لم يتم الاعتماد" });

                                //تحديث حالة الاعتماد 
                                generalRecord.IsApproval = true;
                                db.Entry(generalRecord).State = EntityState.Modified;
                            }
                            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                                return Json(new { isValid = true, message = "تم حفظ واعتماد القيود بنجاح" });
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية.. لم يتم الاعتماد" });

                        }
                        else
                            return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات.. لم يتم الاعتماد" });

                    }

                }
                return Json(new { isValid = true, message = "تم الحفظ بنجاح" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
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