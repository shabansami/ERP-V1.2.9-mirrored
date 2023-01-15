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
using ERP.Web.ViewModels;
using Newtonsoft.Json;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class VoucherReceiptsController : Controller
    {
        // GET: VoucherReceipts
        // Voucher Payment سندات صرف  مورد
        // Voucher Receipt سندات دفع عميل
        VTSaleEntities db;
        public static string DS { get; set; }
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public VoucherReceiptsController()
        {
            db = new VTSaleEntities();
        }

        #region عرض وادارة سندات القبض
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
        public ActionResult GetAll(string accountTreeToId, string isApprovalStatus, string dFrom, string dTo)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.Vouchers.Where(x => !x.IsDeleted && !x.IsVoucherPayment);
            int isAppStatus;
            if (Guid.TryParse(accountTreeToId, out Guid accountTo))
                    list = list.Where(x => x.VoucherDetails.Where(v => !v.IsDeleted && v.AccountTreeId == accountTo).Any());
            
            if (int.TryParse(isApprovalStatus, out isAppStatus))
                    if (isAppStatus == 1)
                        list = list.Where(x => x.IsApproval);
                    else if (isAppStatus == 2)
                        list = list.Where(x => !x.IsApproval);
            
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.VoucherDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.VoucherDate) <= dtTo.Date);

            return Json(new
            {
                data = list.Select(x => new { Id = x.Id, VoucherNumber = x.VoucherNumber, IsApproval = x.IsApproval, Amount = x.VoucherDetails.Where(v => !v.IsDeleted).DefaultIfEmpty().Sum(v => v.Amount), Notes = x.Notes, VoucherDate = x.VoucherDate.ToString(), Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region اضافة قيود متعددة 
        public ActionResult GetDSVoucherTransaction()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new VoucherDetailDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<VoucherDetailDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddVoucherTransaction(string accountTreeTxtId, string amountTxt, string notes, string DT_Datasource)
        {
            List<VoucherDetailDT> deDS = new List<VoucherDetailDT>();
            if (DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<VoucherDetailDT>>(DT_Datasource);
            double amount;
            Guid accountTreeId;
            if (!double.TryParse(amountTxt, out amount) || !Guid.TryParse(accountTreeTxtId, out accountTreeId))
                return Json(new { isValid = false, message = " تأكد من ادخال البيانات المطلوبة بشكل صحيح " }, JsonRequestBehavior.AllowGet);

            //التأكد من عدم وجود اى ابناء للحساب المختار (اخر مستوى فى السشجرة)
            if (AccountTreeService.CheckAccountTreeIdHasChilds(accountTreeId))
                return Json(new { isValid = false, message = "تأكد اختيار حساب فرعى صحيح.. الحساب المختار ليس بحساب فرعى)" });
            //التأكد من عدم تكرار الحساب
            if (deDS.Any(x => x.AccountTreeIdDT == accountTreeId))
                return Json(new { isValid = false, message = "تم ادخال الحساب مسبقا" });

            var accountTree = db.AccountsTrees.Where(x => x.Id == accountTreeId).FirstOrDefault();
            var newVoucher = new VoucherDetailDT
            {
                AccountTreeIdDT = accountTreeId,
                AccountTreeNameDT = accountTree.AccountName,
                AccountTreeNumDT = accountTree.AccountNumber,
                CreditAmount = amount,
                Notes = notes
            };

            deDS.Add(newVoucher);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ادارة السندات 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            VoucherVM vm = new VoucherVM();
            Guid? branchId = null;
            Guid? accountTreeToId = null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);

            if (TempData["model"] != null)
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Vouchers.Where(x => x.Id == id).FirstOrDefault();
                    branchId = model.BranchId;
                    accountTreeToId = model.AccountTreeId;
                    vm.Id = model.Id;
                    vm.BranchId = model.BranchId;
                    vm.VoucherDate = model.VoucherDate;
                    vm.Notes = model.Notes;
                    vm.AccountTreeId = model.AccountTreeId;
                    List<VoucherDetailDT> dt = model.VoucherDetails.Where(x => !x.IsDeleted).Select(x => new VoucherDetailDT
                    {
                        AccountTreeIdDT = x.AccountTreeId,
                        AccountTreeNameDT = x.AccountsTree?.AccountName,
                        AccountTreeNumDT = x.AccountsTree?.AccountNumber,
                        CreditAmount = x.Amount,
                        Notes = x.Notes
                    }).ToList();
                    DS = JsonConvert.SerializeObject(dt);
                }

            }
            else
                vm = new VoucherVM { VoucherDate = Utility.GetDateTime() };

            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.AccountTreeId = new SelectList(AccountTreeService.GetVouchers(db, false), "Id", "Name", accountTreeToId);
            return View(vm);
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(VoucherVM vm, string DT_Datasource, bool? isApproval)
        {
            if (ModelState.IsValid)
            {
                if (vm.BranchId == null || vm.VoucherDate == null || vm.AccountTreeId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                vm.VoucherDate = vm.VoucherDate.Value.AddHours(Utility.GetDateTime().Hour).AddMinutes(Utility.GetDateTime().Minute);

                List<VoucherDetailDT> deDS = new List<VoucherDetailDT>();
                List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
                bool isInsert = false;
                if (DT_Datasource != null)
                {
                    deDS = JsonConvert.DeserializeObject<List<VoucherDetailDT>>(DT_Datasource);
                    if (deDS.Sum(x => x.CreditAmount) == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال المبلغ للحسابات)" });

                    //التأكد من كل الحسابات المحدد ليس لها حسابات فرعية
                    foreach (var voucherDetail in deDS)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(voucherDetail.AccountTreeIdDT))
                            return Json(new { isValid = false, message = $"الحساب {voucherDetail.AccountTreeNameDT} ليس بحساب فرعى" });
                    }
                    voucherDetails = deDS.Select(x => new VoucherDetail
                    {
                        VoucherId = vm.Id,
                        AccountTreeId = x.AccountTreeIdDT,
                        Notes = x.Notes,
                        Amount = x.CreditAmount,
                    }).ToList();

                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال الحسابات المدينة" });


                using (var context = new VTSaleEntities())
                {

                    using (var tran = context.Database.BeginTransaction())
                    {

                        try
                        {
                            Voucher voucher = null;
                            if (vm.Id != Guid.Empty)
                            {
                                voucher = context.Vouchers.Where(x => x.Id == vm.Id).FirstOrDefault();
                                voucher.BranchId = vm.BranchId;
                                voucher.VoucherDate = vm.VoucherDate;
                                voucher.Notes = vm.Notes;
                                voucher.AccountTreeId = vm.AccountTreeId;
                                //حذف اى حسابات مسجلة مسبقا
                                var prevoiusVouchers = voucher.VoucherDetails.Where(x => !x.IsDeleted).ToList();
                                foreach (var item in prevoiusVouchers)
                                {
                                    item.IsDeleted = true;
                                }
                                context.VoucherDetails.AddRange(voucherDetails);
                            }
                            else
                            {
                                isInsert = true;
                                voucher = new Voucher()
                                {
                                    BranchId = vm.BranchId,
                                    Notes = vm.Notes,
                                    AccountTreeId = vm.AccountTreeId,
                                    VoucherDate = vm.VoucherDate,
                                    IsVoucherPayment = false,
                                    VoucherDetails = voucherDetails,
                                };
                                //            //اضافة رقم السند
                                string codePrefix = Properties.Settings.Default.CodePrefix;
                                voucher.VoucherNumber = codePrefix + (context.Vouchers.Count(x => x.VoucherNumber.StartsWith(codePrefix)) + 1);

                                context.Vouchers.Add(voucher);
                            }
                            if (context.SaveChanges(auth.CookieValues.UserId) > 0)
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
                                        if (GeneralDailyService.GeneralDailaiyExists(voucher.Id, (int)TransactionsTypesCl.VoucherReceipt))
                                            return Json(new { isValid = false, message = "تم تسجيل القيد مسبقا" });

                                        // من ح/ حساب الحساب المدين
                                        context.GeneralDailies.Add(new GeneralDaily
                                        {
                                            AccountsTreeId = voucher.AccountTreeId,
                                            Debit = voucherDetails.DefaultIfEmpty().Sum(x => x.Amount),
                                            BranchId = voucher.BranchId,
                                            Notes =voucher.Notes,
                                            TransactionDate = voucher.VoucherDate,
                                            TransactionId = voucher.Id,
                                            TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                                        });

                                        // use Transactions الى حساب
                                        // الحسابات الدائنة
                                        foreach (var item in voucherDetails)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(item.AccountTreeId))
                                                return Json(new { isValid = false, message = "الحساب  ليس بحساب فرعى" });
                                            //الى ح/
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = item.AccountTreeId,
                                                Credit = item.Amount,
                                                BranchId = voucher.BranchId,
                                                Notes = string.IsNullOrEmpty(item.Notes) ? vm.Notes : item.Notes,
                                                TransactionDate = voucher.VoucherDate,
                                                TransactionId = voucher.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                                            });
                                        }
                                        //تحديث حالة الاعتماد 
                                        voucher.IsApproval = true;
                                        context.SaveChanges(auth.CookieValues.UserId);
                                    }
                                    else
                                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات.. لم يتم الاعتماد" });

                                }

                            }
                            tran.Commit();
                            if (isApproval == true)
                                return Json(new { isValid = true, isInsert=isInsert, message = "تم حفظ واعتماد سند القبض بنجاح" });
                            else
                                return Json(new { isValid = true, isInsert= isInsert, message = "تم الحفظ بنجاح" });

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
                var model = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    //حذف الحسابات 
                    var voucherDetails = model.VoucherDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in voucherDetails)
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
                var voucher = db.Vouchers.Where(x => x.Id == Id).FirstOrDefault();
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    //التأكد من عدم تكرار اعتماد القيد
                    if (GeneralDailyService.GeneralDailaiyExists(voucher.Id, (int)TransactionsTypesCl.VoucherReceipt))
                        return Json(new { isValid = false, message = "تم تسجيل القيد مسبقا" });


                    // من ح/ حساب الحساب المدين
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = voucher.AccountTreeId,
                        Debit = voucher.VoucherDetails.Where(x => !x.IsDeleted).DefaultIfEmpty().Sum(x => x.Amount),
                        BranchId = voucher.BranchId,
                        Notes = voucher.Notes,
                        TransactionDate = voucher.VoucherDate,
                        TransactionId = voucher.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                    });

                    // use Transactions الى حساب
                    // الحسابات الدائنة
                    foreach (var item in voucher.VoucherDetails.Where(x => !x.IsDeleted))
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(item.AccountTreeId))
                            return Json(new { isValid = false, message = "الحساب  ليس بحساب فرعى" });
                        //الى ح/
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = item.AccountTreeId,
                            Credit = item.Amount,
                            BranchId = voucher.BranchId,
                            Notes = string.IsNullOrEmpty(item.Notes) ? voucher.Notes : item.Notes,
                            TransactionDate = voucher.VoucherDate,
                            TransactionId = voucher.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.VoucherReceipt
                        });
                    }

                    //تحديث حالة الاعتماد 
                    voucher.IsApproval = true;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد سند القبض بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات.. لم يتم الاعتماد" });
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
                var model = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.VoucherReceipt).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
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












