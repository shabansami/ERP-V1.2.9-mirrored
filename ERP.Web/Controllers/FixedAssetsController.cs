using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class FixedAssetsController : Controller
    {
        // GET: FixedAssets
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public FixedAssetsController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public ActionResult Index()
        {            
            //ViewBag.AccountTreeParentId = new SelectList(db.AccountsTrees.Where(x => !x.IsDeleted && x.TypeId == (int)AccountTreeSelectorTypesCl.FixedAssets && x.SelectedTree), "Id", "AccountName");
            ViewBag.AccountTreeParentId = new SelectList(AccountTreeService.GetFixedAssets(), "Id", "AccountName");
            return View();
        }


        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.Assets.Where(x => !x.IsDeleted && !x.IsIntialBalance).OrderBy(x=>x.CreatedOn).Select(x => new { TransactionId = x.Id, CategoryName = x.AccountsTree.AccountsTreeParent.AccountName, AssetName = x.Name, Amount = x.Amount, Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet); ;

        }
        [HttpGet]
        public ActionResult CreateEdit()
        {
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            if (TempData["model"] != null) //show data
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.Assets.FirstOrDefault(x => x.Id == id);

                    ViewBag.ShowMode = true;
                    ViewBag.AccountTreeParentId = new SelectList(AccountTreeService.GetFixedAssets(), "Id", "AccountName", model.AccountTreeParentId);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
                    ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == model.BranchId), "Id", "Name", model.SafeId);
                    ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName",model.BankAccountId);

                    ViewBag.LastRow = db.Assets.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();

                    return View(model);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {
                // add
                //ViewBag.AccountTreeParentId = new SelectList(db.AccountsTrees.Where(x => !x.IsDeleted && x.TypeId == (int)AccountTreeSelectorTypesCl.FixedAssets && x.SelectedTree), "Id", "AccountName");
                ViewBag.AccountTreeParentId = new SelectList(AccountTreeService.GetFixedAssets(), "Id", "AccountName");
                //var defaultStore = storeService.GetDefaultStore(db);
                //var branchId = defaultStore != null ? defaultStore.BranchId : null;
                ViewBag.BranchId = new SelectList(branches, "Id", "Name");
                ViewBag.SafeId = new SelectList(new List<Safe>(), "Id", "Name");
                ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
                ViewBag.LastRow = db.Assets.Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                return View(new Asset()
                {
                    PurchaseDate = Utility.GetDateTime(),
                    OperationDate = Utility.GetDateTime()
                });
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(Asset vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.BranchId == null || vm.AccountTreeParentId == null || vm.Amount == 0 || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.BankAccountId == null && vm.SafeId == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار حساب الدفع (بنكى-خزنة) بشكل صحيح" });

                if (vm.IsDestruction )
                    if (vm.DestructionAmount==0||vm.UsefulLife==0)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات الاهلاك بشكل صحيح" });

                MessageReturn messageReturn = new MessageReturn();

                if (db.Assets.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.ToList();
                    // حساب الخزنة او البنك
                    Guid? safeAccountTreeId=null;
                    Guid? bankAccountTreeId=null;
                    if (vm.SafeId != null)
                    {
                        safeAccountTreeId = db.Safes.FirstOrDefault(x=>x.Id==vm.SafeId).AccountsTreeId;
                        //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(safeAccountTreeId))
                            return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                    }
                    else
                    {
                        bankAccountTreeId = db.BankAccounts.FirstOrDefault(x=>x.Id==vm.BankAccountId).AccountsTreeId;
                        // التأكد من عدم وجود حساب فرعى من الحساب البنك
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(bankAccountTreeId))
                            return Json(new { isValid = false, message = "حساب البنك ليس بحساب فرعى" });
                    }
                    if (vm.IsDestruction)
                    {
                        //التأكد من عدم وجود حساب فرعى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب مجمع الاهلاك للاصل ليس بحساب فرعى" });
                    }

                    messageReturn = AddAccountTree(vm, generalSetting, safeAccountTreeId, bankAccountTreeId);

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف حسابات الاصول فى شاشة الاعدادات" });


                if (messageReturn.IsValid)
                    return Json(new { isValid = true, message = messageReturn.Message });
                else
                    return Json(new { isValid = false, message = messageReturn.Message });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });
        }

        public MessageReturn AddAccountTree(Asset model, List<GeneralSetting> generalSetting, Guid? safeAccountTreeId, Guid? bankAccountTreeId)
        {
            //get account id from general setting 
            using (var context = new VTSaleEntities())
            {
                using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                {
                    try
                    {
                        //اضافة حساب الاصل 
                        long newAccountNum = 0;// new account number 
                        if (model.AccountTreeParentId == null)
                            return new MessageReturn { Message = "تأكد من اختيار الحساب الرئيسى", IsValid = false };

                        var accountTree = context.AccountsTrees.FirstOrDefault(x=>x.Id==model.AccountTreeParentId);
                        if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                            newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                        else
                            newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                        // add new account tree 
                        var newAccountTree = new AccountsTree
                        {
                            AccountLevel = accountTree.AccountLevel + 1,
                            AccountName = model.Name,
                            AccountNumber = newAccountNum,
                            ParentId = accountTree.Id,
                            TypeId = (int)AccountTreeSelectorTypesCl.Operational,
                            SelectedTree = false
                        };
                        context.AccountsTrees.Add(newAccountTree);
                        context.Assets.Add(model);

                        context.SaveChanges(auth.CookieValues.UserId);

                        AccountsTree newExpenseAccountTree = null;
                        //اضافة مخزن وحساب مصروفات السيارة فى حالة اضافه اصل سيارة للمناديب
                        if (model.IsCarStore || model.IsNotCarStore)
                        {
                            if (model.IsCarStore)
                            {
                                //اضافة مخزن فى حالة ان الاصل سيارة كمخزن
                                var store = new Store
                                {
                                    AccountsTree = newAccountTree,
                                    BranchId = model.BranchId,
                                    Name = $"مخزن : {model.Name}"
                                };

                                //تحديد نوع الجرد
                                var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                Guid accountTreeStockAccount;
                                if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                    accountTreeStockAccount = Guid.Parse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                                else
                                    return new MessageReturn { Message = "تأكد من تحديد نوع الجرد اولا", IsValid = false };

                                if (inventoryTypeVal == 2)   // حالة الجرد المستمر
                                {
                                    long newAccountNumStor = 0;// new account number 

                                    var accountTreeStor = db.AccountsTrees.Find(accountTreeStockAccount);
                                    var count = accountTreeStor.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                                    if (accountTreeStor.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                                        newAccountNumStor = long.Parse(accountTreeStor.AccountNumber + "000001");
                                    else
                                        newAccountNumStor = accountTreeStor.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                                    // add new account tree 
                                    var newAccountTreeStor = new AccountsTree
                                    {
                                        AccountLevel = accountTreeStor.AccountLevel + 1,
                                        AccountName = $"مخزن : {model.Name}",
                                        AccountNumber = newAccountNumStor,
                                        ParentId = accountTreeStor.Id,
                                        TypeId = (int)AccountTreeSelectorTypesCl.Operational,
                                        SelectedTree = false
                                    };
                                    store.AccountsTree = newAccountTreeStor;
                                }
                                context.Stores.Add(store);
                                context.SaveChanges(auth.CookieValues.UserId);
                            }

                        }


                        AccountsTree newDestructionAccountTree = null;
                        if (model.IsDestruction)//الاصل قابل للاهلاك
                        {
                            //احتساب قيمة الاهلاك للسنه الحالية 
                            var startYear = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                            var endYear = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
                            if (!DateTime.TryParse(endYear, out DateTime financialYearEnd))
                                return new MessageReturn { Message = "تأكد من تحديد تاريخ السنة المالية", IsValid = false };
                            var diffMonths = ((financialYearEnd.Month+1) + financialYearEnd.Year * 12) - (model.OperationDate.Value.Month + model.OperationDate.Value.Year * 12);
                            var destruForMonth = Math.Round(model.DestructionAmount / 12, 2, MidpointRounding.ToEven);
                            model.DestructionAmountCurrent = diffMonths==12? model.DestructionAmount:destruForMonth * diffMonths;
                            //اضافة حساب مجمع اهلاك الاصل  
                            long newAccountDestructionNum = 0;// new account number 

                            var val = generalSetting.Where(x=>x.Id==(int)GeneralSettingCl.AccountTreeDestructionAllowance).FirstOrDefault().SValue;
                            if (val == null)
                                return new MessageReturn { Message = "تأكد من تحديد حساب مجمع الاهلاك من الاعدادات العامة", IsValid = false };

                            var generalId = Guid.Parse(val);
                            var accountTreeDestruction = context.AccountsTrees.FirstOrDefault(x=>x.Id==generalId);
                            if (accountTreeDestruction.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                                newAccountDestructionNum = long.Parse(accountTreeDestruction.AccountNumber + "000001");
                            else
                                newAccountDestructionNum = accountTreeDestruction.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                            newDestructionAccountTree = new AccountsTree
                            {
                                AccountLevel = accountTreeDestruction.AccountLevel + 1,
                                AccountName = $"مخصص اهلاك  : {model.Name} ",
                                AccountNumber = newAccountDestructionNum,
                                ParentId = accountTreeDestruction.Id,
                                SelectedTree = false
                            };
                            context.AccountsTrees.Add(newDestructionAccountTree);
                            context.SaveChanges(auth.CookieValues.UserId);
                            //اضافة قيد اهلاك للعام الحالى 
                            // من ح/مخصص اهلاك الاصل 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = newDestructionAccountTree.Id,
                                BranchId = model.BranchId,
                                Debit = model.DestructionAmountCurrent,
                                Notes = $"اهلاك الاصل الثابت : {model.Name}  للعام المالى :{startYear}-{endYear} عن : {diffMonths.ToString()} اشهر",
                                TransactionDate = financialYearEnd,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Destruction
                            });

                            //الى ح/مجمع اهلاك الاصول الثابتة
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue),
                                BranchId = model.BranchId,
                                Credit = model.DestructionAmountCurrent,
                                Notes = $"اهلاك الاصل الثابت : {model.Name}  للعام المالى :{startYear}-{endYear} عن : {diffMonths.ToString()} اشهر",
                                TransactionDate = financialYearEnd,
                                TransactionId = model.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Destruction
                            });
                            context.SaveChanges(auth.CookieValues.UserId);
                        }
                        //add model 
                        model.AccountsTree = newAccountTree;// account حساب الاصل 
                        model.AccountsTreeExpense = newExpenseAccountTree;//حساب المصروف ان وجد
                        model.AccountsTreeDestruction = newDestructionAccountTree;// حساب مخصص الاهلاك
                        model.AccountsTreeParent = accountTree;//parent account حساب الرئيسى
                        model.IsIntialBalance = false;
                        context.Entry(model).State = EntityState.Modified;

                        //تسجيل قيود شراء الاصل
                        // حساب الاصل
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = newAccountTree.Id,
                            Debit = model.Amount ,
                            BranchId = model.BranchId,
                            Notes = $"شراء  : {model.Name}_ {model.Notes} ",
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.FixedAssets
                        });
                        context.SaveChanges(auth.CookieValues.UserId);

                        // حساب الخزينة او البنك
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = model.SafeId != null ? safeAccountTreeId : bankAccountTreeId,
                            Credit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"شراء  : {model.Name}_ {model.Notes} ",
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.FixedAssets
                        });
                        context.SaveChanges(auth.CookieValues.UserId);

                        dbTran.Commit();
                        return new MessageReturn { Message = "تم الاضافة بنجاح", IsValid = true };

                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        return new MessageReturn { Message = "حدث خطأ اثناء تنفيذ العملية", IsValid = false };
                        throw;
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.Assets.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var assetAccount = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == model.AccountTreeId).FirstOrDefault();
                    assetAccount.IsDeleted = true;
                    db.Entry(assetAccount).State = EntityState.Modified;

                    //حذف مصروفات السيارة فى حالة ان الاصل سيارة
                    var assetExpenes = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == model.AccountTreeExpenseId).FirstOrDefault();
                    if (assetExpenes != null)
                    {
                        assetExpenes.IsDeleted = true;
                        db.Entry(assetExpenes).State = EntityState.Modified;
                    }
                    //حذف مخزن السيارة فى حالة ان الاصل سيارة
                    var assetStore = db.Stores.Where(x => !x.IsDeleted && x.AccountTreeId == model.AccountTreeId).FirstOrDefault();
                    if (assetStore != null)
                    {
                        assetStore.IsDeleted = true;
                        db.Entry(assetStore).State = EntityState.Modified;
                    }
                    //حذف قيود اليومية لرصيد اول المدة للاصل
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == Id /*&& x.TransactionTypeId == (int)TransactionsTypesCl.FixedAssets*/).ToList();
                    if (generalDailies != null)
                    {
                        foreach (var item in generalDailies)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

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