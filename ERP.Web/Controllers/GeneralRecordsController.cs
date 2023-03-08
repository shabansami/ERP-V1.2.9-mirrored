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
using ERP.Web.ViewModels;
using Newtonsoft.Json;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class GeneralRecordsController : Controller
    {
        // GET: GeneralRecords
        VTSaleEntities db;
        public static string DS { get; set; }
        public GeneralRecordsController()
        {
            db= new VTSaleEntities();
        }
        
        VTSAuth auth => TempData["userInfo"] as VTSAuth;

        #region عرض القيود الحرة المسجلة سابقا
        public ActionResult Index()
        {
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo,Guid? accountId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.GeneralRecords.Where(x => !x.IsDeleted);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.TransactionDate) <= dtTo.Date);
            if (accountId != null && accountId != Guid.Empty)
                list = list.Where(x => x.GeneralRecordDetails.Where(d => !d.IsDeleted && d.AccountTreeId == accountId).Any());
            return Json(new
            {
                data = list.Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده",  TransactionDate = x.TransactionDate.ToString(), typ = (int)UploalCenterTypeCl.GeneralRecord, Amount = x.GeneralRecordDetails.Where(d => !d.IsDeleted&&d.IsDebit).DefaultIfEmpty().Sum(d => d.Amount), Notes = x.Notes, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region اضافة قيود متعددة 
        public ActionResult GetDSGenaralComplex()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new GeneralRecordDetailDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<GeneralRecordDetailDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddGenaralComplex(string accountTreeTxtId, string amountTxt, string debitCredit, string notes, string DT_Datasource)
        {
            List<GeneralRecordDetailDT> deDS = new List<GeneralRecordDetailDT>();
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<GeneralRecordDetailDT>>(DT_Datasource);
            double amount;
            Guid accountTreeId;
            int debitCreditId;
            if (!double.TryParse(amountTxt, out amount) || !Guid.TryParse(accountTreeTxtId, out accountTreeId) || !int.TryParse(debitCredit, out debitCreditId))
                return Json(new { isValid = false, message = " تأكد من ادخال البيانات المطلوبة بشكل صحيح " }, JsonRequestBehavior.AllowGet);

            //التأكد من عدم وجود اى ابناء للحساب المختار (اخر مستوى فى السشجرة)
            if (AccountTreeService.CheckAccountTreeIdHasChilds(accountTreeId))
                return Json(new { isValid = false, message = "تأكد اختيار حساب فرعى صحيح.. الحساب المختار ليس بحساب فرعى)" });
            //التأكد من عدم تكرار الحساب
            //if(deDS.Any(x=>x.AccountTreeId==accountTreeId))
            //    return Json(new { isValid = false, message = "تم ادخال الحساب مسبقا" });

            var accountTree = db.AccountsTrees.Where(x => x.Id == accountTreeId).FirstOrDefault();
            var newGeneral = new GeneralRecordDetailDT
            {
                AccountTreeId= accountTreeId,
                AccountTreeName=accountTree.AccountName,
                AccountTreeNum=accountTree.AccountNumber,
                DebitCredit=debitCreditId,
                DebitAmount=debitCreditId==1?amount:0,
                CreditAmount=debitCreditId==2?amount:0,
                Notes=notes
            };

            deDS.Add(newGeneral);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ادارة القيود 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            GeneralRecordVM vm = new GeneralRecordVM();
             Guid? branchId=null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.GeneralRecords.FirstOrDefault(x => x.Id == id);
                    branchId = model.BranchId;
                    vm.Id = model.Id;
                    vm.BranchId = model.BranchId;
                    vm.TransactionDate = model.TransactionDate;
                    vm.Notes = model.Notes;
                    List<GeneralRecordDetailDT> dt = model.GeneralRecordDetails.Where(x => !x.IsDeleted).Select(x => new GeneralRecordDetailDT
                    {
                        AccountTreeId = x.AccountTreeId,
                        AccountTreeName = x.AccountTree?.AccountName,
                        AccountTreeNum = x.AccountTree?.AccountNumber,
                        DebitAmount = x.IsDebit ? x.Amount : 0,
                        CreditAmount = !x.IsDebit ? x.Amount : 0,
                        DebitCredit = x.IsDebit ? 1 : 2,
                        Notes = x.Notes
                    }).ToList();
                    DS = JsonConvert.SerializeObject(dt);
                }

            }
            else
                vm = new GeneralRecordVM { TransactionDate = Utility.GetDateTime() };


            ViewBag.BranchId = new SelectList(branches, "Id", "Name",branchId);
            ViewBag.Branchcount = branches.Count();
            ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };

            return View(vm);
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

                List<GeneralRecordDetailDT> deDS = new List<GeneralRecordDetailDT>();
                List<GeneralRecordDetail> generalRecordDetails = new List<GeneralRecordDetail>();

                if (DT_Datasource != null)
                {
                    deDS = JsonConvert.DeserializeObject<List<GeneralRecordDetailDT>>(DT_Datasource);
                    if (deDS.Any(x => x.DebitCredit == null))
                        return Json(new { isValid = false, message = "تأكد من اختيار حالة الحساب(مدين/دائن)" });
                    if (deDS.Sum(x => x.DebitAmount)==0&& deDS.Sum(x => x.CreditAmount)==0)
                        return Json(new { isValid = false, message = "تأكد من ادخال المبلغ للحسابات)" });

                    //التاكد من ان القيود موزونة
                    if (deDS.Sum(x => x.DebitAmount) != deDS.Sum(x => x.CreditAmount))
                        return Json(new { isValid = false, message = "القيود غير موزونة" });
                    //التأكد من كل الحسابات المحدد ليس لها حسابات فرعية
                    foreach (var generalRecordDetail in deDS)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(generalRecordDetail.AccountTreeId))
                            return Json(new { isValid = false, message = $"الحساب {generalRecordDetail.AccountTreeName} ليس بحساب فرعى" });
                    }
                    generalRecordDetails = deDS.Select(x => new GeneralRecordDetail
                    {
                        GeneralRecordId=vm.Id,
                        AccountTreeId= x.AccountTreeId,
                        IsDebit=x.DebitCredit==1?true:false,
                        Notes=x.Notes,
                        Amount= x.DebitCredit == 1 ? x.DebitAmount : x.CreditAmount,    
                    }).ToList();
                }
 
                using (var context = new VTSaleEntities())
                {

                    using (var tran = context.Database.BeginTransaction())
                    {

                        try
                        {
                            GeneralRecord generalRecord = null;
                            if (vm.Id != Guid.Empty)
                            {
                              generalRecord=context.GeneralRecords.Where(x=>x.Id==vm.Id).FirstOrDefault();
                                generalRecord.BranchId = vm.BranchId;
                                generalRecord.TransactionDate = vm.TransactionDate;
                                generalRecord.Notes=vm.Notes;
                                //حذف اى حسابات مسجلة مسبقا
                                var prevoiusGeneral = generalRecord.GeneralRecordDetails.Where(x => !x.IsDeleted).ToList();
                                foreach (var item in prevoiusGeneral)
                                {
                                    item.IsDeleted = true;
                                }
                                context.GeneralRecordDetails.AddRange(generalRecordDetails);
                            }else
                            {
                                generalRecord = new GeneralRecord()
                                {
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    TransactionDate = vm.TransactionDate,
                                    GeneralRecordDetails = generalRecordDetails,
                                };
                                //            //اضافة رقم السند
                                string codePrefix = Properties.Settings.Default.CodePrefix;
                                generalRecord.GeneralRecordNumber = codePrefix + (context.GeneralRecords.Count(x => x.GeneralRecordNumber.StartsWith(codePrefix)) + 1);
                                context.GeneralRecords.Add(generalRecord);
                            }
                            if (context.SaveChanges(auth.CookieValues.UserId)>0)
                            {
                                DS = null;
                                //اعتماد القيود فى حالة الضغط على حفظ واعتماد
                                if (isApproval == true)
                                {
                                    //    //تسجيل القيود
                                    // General Dailies
                                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                    {
                                        //التأكد من عدم تكرار اعتماد القيد
                                        if (GeneralDailyService.GeneralDailaiyExists(generalRecord.Id, (int)TransactionsTypesCl.FreeRestrictions))
                                            return Json(new { isValid = false, message = "تم تسجيل القيد مسبقا" });

                                        // use Transactions
                                        foreach (var item in generalRecordDetails)
                                        {
                                            if (item.AccountTreeId != null)
                                                if (AccountTreeService.CheckAccountTreeIdHasChilds(item.AccountTreeId))
                                                    return Json(new { isValid = false, message = "الحساب  ليس بحساب فرعى" });
                                                //  حساب
                                                context.GeneralDailies.Add(new GeneralDaily
                                                {
                                                    AccountsTreeId = item.AccountTreeId,
                                                    Debit =item.IsDebit?item.Amount:0,
                                                    Credit =item.IsDebit?0: item.Amount,
                                                    Notes =string.IsNullOrEmpty(item.Notes)?vm.Notes: item.Notes,
                                                    BranchId = generalRecord.BranchId,
                                                    TransactionDate = generalRecord.TransactionDate,
                                                    TransactionId = generalRecord.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.FreeRestrictions
                                                });
                 
                                        }

                                        //تحديث حالة الاعتماد 
                                        generalRecord.IsApproval = true;
                                        context.SaveChanges(auth.CookieValues.UserId);
                                    }
                                    else
                                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات.. لم يتم الاعتماد" });

                                }

                            }
                            tran.Commit();
                            if (isApproval == true)
                                return Json(new { isValid = true, message = "تم حفظ واعتماد القيود بنجاح" });
                            else
                            return Json(new { isValid = true, message = "تم الحفظ بنجاح" });

                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                        }

                    }
                }
               
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
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
                    //حذف الحسابات 
                    var generalRecordDetails = model.GeneralRecordDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in generalRecordDetails)
                    {
                        item.IsDeleted = true;
                    }
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
                    if (generalRecord!=null)
                    {
                        //التأكد من عدم تكرار اعتماد القيد
                        if (GeneralDailyService.GeneralDailaiyExists(generalRecord.Id, (int)TransactionsTypesCl.FreeRestrictions))
                            return Json(new { isValid = false, message = "تم تسجيل القيد مسبقا" });
                        // use Transactions
                        foreach (var item in generalRecord.GeneralRecordDetails.Where(x=>!x.IsDeleted))
                        {
                            if (item.AccountTreeId != null)
                            {
                                if (AccountTreeService.CheckAccountTreeIdHasChilds(item.AccountTreeId))
                                    return Json(new { isValid = false, message = "الحساب  ليس بحساب فرعى" });
                            }
                            else
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                            //  حساب
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = item.AccountTreeId,
                                Debit = item.IsDebit ? item.Amount : 0,
                                Credit = item.IsDebit ? 0 : item.Amount,
                                Notes = string.IsNullOrEmpty(item.Notes) ? generalRecord.Notes : item.Notes,
                                BranchId = generalRecord.BranchId,
                                TransactionDate = generalRecord.TransactionDate,
                                TransactionId = generalRecord.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.FreeRestrictions
                            });
                        }

                        //تحديث حالة الاعتماد 
                        generalRecord.IsApproval = true;
                        if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, message = "تم اعتماد القيد بنجاح" });
                        else
                            return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


                    }
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
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.FreeRestrictions).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                        }
                        model.IsApproval = false;
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
            #region تاريخ البداية والنهاية فى البحث
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.FinancialYearDate))
            {
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                ViewBag.StartDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.StartDateSearch).FirstOrDefault().SValue;
                ViewBag.EndDateSearch = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EndDateSearch).FirstOrDefault().SValue;
            }
            #endregion

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