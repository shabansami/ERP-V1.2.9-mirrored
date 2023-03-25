using ERP.DAL;
using ERP.DAL.Models;
using ERP.DAL.Utilites;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class AccountTreeIntialBalancesController : Controller
    {
        // GET: AccountTreeIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public AccountTreeIntialBalancesController()
        {
            db = new VTSaleEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            var list = db.AccountTreeIntialBalances.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, AccountTreeName = x.AccountsTree.AccountName , Amount = x.Amount, OperationDate = x.OperationDate.ToString(), IsApproval = x.IsApproval,DebitStatus=x.IsDebit?"مدين":"دائن", Actions = n, Num = n }).ToList();
            return Json(new
            {
                data = list
            }, JsonRequestBehavior.AllowGet); ;

        }
        public ActionResult CreateEdit()
        {
            IntialBalanceVM vm = new IntialBalanceVM();
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    vm = db.AccountTreeIntialBalances.Where(x => x.Id == id).Select(x => new IntialBalanceVM
                    {
                        Id= x.Id,
                        AccountId=x.AccountTreeId,
                        Amount= x.Amount,
                        DateIntial=x.OperationDate,
                        DebitCredit=x.IsDebit?1:2,
                        Notes= x.Notes,
                        BranchId=x.BranchId,
                    }).FirstOrDefault();
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                  // add
                vm.DateIntial = Utility.GetDateTime();
            }
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.Branchcount = branches.Count();
            var debitCreditList = new List<DropDownListInt> { new DropDownListInt { Id = 1, Name = "مدين" }, new DropDownListInt { Id = 2, Name = "دائن" } };
            ViewBag.DebitCredit = new SelectList(debitCreditList, "Id", "Name", vm.DebitCredit);
            //ViewBag.DebitCredit = new List<SelectListItem> { new SelectListItem { Text = "مدين", Value = "1", Selected = true }, new SelectListItem { Text = "دائن", Value = "2" } };
            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(IntialBalanceVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Amount == 0 || vm.Amount == null || vm.BranchId == null || vm.DateIntial == null || vm.DebitCredit == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                var isInsert = false;
                //التأكد من عدم وجود حساب تشغيلى من الحساب
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountId))
                    return Json(new { isValid = false, message = "الحساب المحدد ليس بحساب تشغيلى" });

                if (vm.Id !=Guid.Empty)
                {
                    if (db.AccountTreeIntialBalances.Where(x => !x.IsDeleted && x.AccountTreeId == vm.AccountId && x.Id != vm.Id).Count() > 0)
                        return Json(new { isValid = false, message = "تم تسجيل رصيد اول للحساب مسبقا" });

                    var model = db.AccountTreeIntialBalances.FirstOrDefault(x => x.Id == vm.Id);
                    model.BranchId = vm.BranchId;
                    model.AccountTreeId = vm.AccountId;
                    model.OperationDate = vm.DateIntial;
                    model.Amount = vm.Amount??0;
                    model.Notes = vm.Notes;
                    model.IsDebit=vm.DebitCredit==1?true:false;

                }
                else
                {
                    if (db.AccountTreeIntialBalances.Where(x => !x.IsDeleted && x.AccountTreeId == vm.AccountId).Count() > 0)
                        return Json(new { isValid = false, message = "تم تسجيل رصيد اول للحساب مسبقا" });
                    isInsert = true;
                    db.AccountTreeIntialBalances.Add(new AccountTreeIntialBalance
                    {
                        BranchId = vm.BranchId,
                        AccountTreeId = vm.AccountId,   
                        OperationDate=vm.DateIntial,
                        Amount=vm.Amount??0,
                        Notes=vm.Notes,
                        IsDebit=vm.DebitCredit==1?true:false    
                    }) ;
                }
                //}
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
                var accountTreeIntialBalance = db.AccountTreeIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                accountTreeIntialBalance.IsDeleted = true;

                var model = db.GeneralDailies.Where(x => x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceAccountTree).ToList();
                if (model != null)
                {
                    foreach (var item in model)
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
                        var vm = db.AccountTreeIntialBalances.FirstOrDefault(x => x.Id == Id);
                        if (vm != null)
                        {
                        //    //تسجيل القيود
                        // General Dailies
                        if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                        {
                            // الحصول على حسابات من الاعدادات
                            var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                            //التأكد من عدم وجود حساب تشغيلى من الحساب
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب تشغيلى" });
                            //التأكد من عدم وجود حساب تشغيلى من الحساب
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.AccountTreeId))
                                return Json(new { isValid = false, message = "الحساب المحدد ليس بحساب تشغيلى" });
                            //التأكد من عدم تكرار اعتماد القيد
                            if (GeneralDailyService.GeneralDailaiyExists(vm.Id, (int)TransactionsTypesCl.InitialBalanceAccountTree))
                                return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });

                            vm.IsApproval = true;
                            if (vm.IsDebit) // الرصيد مدين
                            {
                                // من حساب
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = vm.AccountTreeId,
                                    Debit = vm.Amount,
                                    Notes = $"رصيد أول المدة للحساب  : {vm.AccountsTree?.AccountName}",
                                    BranchId = vm.BranchId,
                                    TransactionDate = vm.OperationDate,
                                    TransactionId = vm.Id,
                                    TransactionShared = vm.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceAccountTree
                                });
                                //رأس المال 
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                                    Credit = vm.Amount,
                                    Notes = $"رصيد أول المدة للحساب : {vm.AccountsTree?.AccountName}",
                                    BranchId = vm.BranchId,
                                    TransactionDate = vm.OperationDate,
                                    TransactionId = vm.Id,
                                    TransactionShared = vm.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceAccountTree
                                });
                            }
                            else  //الرصيد دائن
                            {
                                //من حساب رأس المال 
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                                    Debit = vm.Amount,
                                    Notes = $"رصيد أول المدة للحساب : {vm.AccountsTree?.AccountName}",
                                    BranchId = vm.BranchId,
                                    TransactionDate = vm.OperationDate,
                                    TransactionId = vm.Id,
                                    TransactionShared = vm.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceAccountTree
                                }); 
                         
                            // الى حساب
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = vm.AccountTreeId,
                                Credit = vm.Amount,
                                Notes = $"رصيد أول المدة للحساب  : {vm.AccountsTree?.AccountName}",
                                BranchId = vm.BranchId,
                                TransactionDate = vm.OperationDate,
                                TransactionId = vm.Id,
                                TransactionShared = vm.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceAccountTree
                            });

                        }
                    }
                        else
                            return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });


                            //تحديث حالة الاعتماد 
                            vm.IsApproval = true;

                            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                                return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
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
                    var model = db.AccountTreeIntialBalances.FirstOrDefault(x => x.Id == Id);
                    if (model != null)
                    {
                        //تحديث حالة الاعتماد 
                        model.IsApproval = false;
                        //حذف قيود اليومية
                        var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceAccountTree).ToList();
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




















