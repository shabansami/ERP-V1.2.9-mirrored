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

    public class FixedAssetIntialBalancesController : Controller
    {
        // GET: FixedAssetIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public FixedAssetIntialBalancesController()
        {
            db = new VTSaleEntities();
            storeService= new StoreService();
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
                data = db.Assets.Where(x => !x.IsDeleted&&x.IsIntialBalance).OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id, CategoryName = x.AccountsTree.AccountsTreeParent.AccountName, AssetName = x.Name, Amount = x.Amount, Actions = n, Num = n }).ToList()
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
                    ViewBag.AccountTreeParentId = new SelectList(AccountTreeService.GetFixedAssets(), "Id", "AccountName",model.AccountTreeParentId);
                    ViewBag.BranchId = new SelectList(branches, "Id", "Name", model.BranchId);
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
                var defaultStore = storeService.GetDefaultStore(db);
                var branchId = defaultStore != null ? defaultStore.BranchId : null;
                ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
                //ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", defaultStore?.Id);
                ViewBag.LastRow = db.Assets.Where(x => !x.IsDeleted).OrderByDescending(x => x.Id).FirstOrDefault();
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
                if (vm.BranchId == null || vm.AccountTreeParentId == null || vm.Amount == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                if (vm.IsDestruction)
                    if (vm.DestructionAmount == 0 || vm.UsefulLife == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات الاهلاك بشكل صحيح" });

                bool? isSaved = false;

                if (db.Assets.Where(x => !x.IsDeleted && x.Name == vm.Name).Count() > 0)
                    return Json(new { isValid = false, message = "الاسم موجود مسبقا" });
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });

                    if (vm.IsDestruction)
                    {
                        //التأكد من عدم وجود حساب فرعى من الحساب
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue)))
                            return Json(new { isValid = false, message = "حساب مجمع الاهلاك للاصل ليس بحساب فرعى" });
                    }

                    isSaved = AddAccountTree(vm, generalSetting);

                }
                else
                    return Json(new { isValid = false, message = "يجب تعريف حساب راس المال فى شاشة الاعدادات" });


                if (isSaved == true)
                    return Json(new { isValid = true, message = "تم الاضافة بنجاح" });

                else if (isSaved == null)
                    return Json(new { isValid = false, message = "تأكد من اختيار الحساب الرئيسى" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });
        }

        public bool? AddAccountTree(Asset model, List<GeneralSetting> generalSetting)
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
                            return null;

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
                            TypeId = (int)AccountTreeSelectorTypesCl.FixedAssets,
                            SelectedTree = false
                        };
                        context.AccountsTrees.Add(newAccountTree);
                        context.Assets.Add(model);

                        context.SaveChanges(auth.CookieValues.UserId);

                        AccountsTree newExpenseAccountTree = null;
                        //اضافة مخزن وحساب مصروفات السيارة فى حالة اضافه اصل سيارة للمناديب
                        if (model.IsCarStore||model.IsNotCarStore)
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
                                context.Stores.Add(store);
                                context.SaveChanges(auth.CookieValues.UserId);
                            }
                            //اضافة حساب مصروف جديد باسم السيارة 
                            long newAccountExpenceCarNum = 0;// new account number 
                            var val = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeExpensesAccount).FirstOrDefault().SValue;
                            if (val== null)
                                return null;
                            var generalId = Guid.Parse(val);
                            var accountTreeExpense = context.AccountsTrees.FirstOrDefault(x=>x.Id== generalId);
                            if (accountTreeExpense.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                                newAccountExpenceCarNum = long.Parse(accountTreeExpense.AccountNumber + "000001");
                            else
                                newAccountExpenceCarNum = accountTreeExpense.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                             newExpenseAccountTree = new AccountsTree
                            {
                                AccountLevel = accountTreeExpense.AccountLevel + 1,
                                AccountName = $"مصروف : {model.Name}",
                                AccountNumber = newAccountExpenceCarNum,
                                ParentId = accountTreeExpense.Id,
                                TypeId = (int)AccountTreeSelectorTypesCl.Expense,
                                SelectedTree = false
                            };
                            context.AccountsTrees.Add(newExpenseAccountTree);
                            context.SaveChanges(auth.CookieValues.UserId);

                            //اضافة مصروف السيارة ضمن انواع المصروفات
                            context.ExpenseTypes.Add(new ExpenseType
                            {
                                AccountsTree = newExpenseAccountTree,
                                Name = $"مصروف : {model.Name}"
                            });
                            context.SaveChanges(auth.CookieValues.UserId);
                        }

                        AccountsTree newDestructionAccountTree = null;
                        if (model.IsDestruction)//الاصل قابل للاهلاك
                        {
                            //احتساب قيمة الاهلاك للسنه الحالية 
                            var startYear = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue;
                            var endYear = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue;
                            if (!DateTime.TryParse(endYear, out DateTime financialYearEnd))
                                return null;
                            var diffMonths = ((financialYearEnd.Month + 1) + financialYearEnd.Year * 12) - (model.OperationDate.Value.Month + model.OperationDate.Value.Year * 12);
                            var destruForMonth = Math.Round(model.DestructionAmount / 12, 2, MidpointRounding.ToEven);
                            model.DestructionAmountCurrent = diffMonths == 12 ? model.DestructionAmount : destruForMonth * diffMonths;

                            //اضافة حساب مجمع اهلاك الاصل  
                            long newAccountDestructionNum = 0;// new account number 

                            var val = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDestructionAllowance).FirstOrDefault().SValue;
                            if (val == null)
                                return null;

                            var generalId = Guid.Parse(val);
                            var accountTreeDestruction = context.AccountsTrees.FirstOrDefault(x=>x.Id== generalId);
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
                        model.IsIntialBalance = true;
                        context.Entry(model).State = EntityState.Modified;

                        ////add model 
                        //model.AccountsTree = newAccountTree;
                        //model.AccountsTree1= newExpenseAccountTree;
                        //model.IsIntialBalance = true;
                        //context.Assets.Add(model);
                        //context.SaveChanges(auth.CookieValues.UserId);

                        //تسجيل قيود رصيد اول المدة
                        // حساب الاصل
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = newAccountTree.Id,
                            Debit = model.Amount ,
                            BranchId = model.BranchId,
                            Notes = $"رصيد أول المدة للأصل الثابت : {model.Name} _ {model.Notes}",
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceFixedAsset
                        });
                        context.SaveChanges(auth.CookieValues.UserId);

                        //رأس المال 
                        context.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                            Credit = model.Amount,
                            BranchId = model.BranchId,
                            Notes = $"رصيد أول المدة للأصل الثابت : {model.Name} _ {model.Notes}",
                            TransactionDate = Utility.GetDateTime(),
                            TransactionId = model.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceFixedAsset
                        });
                        context.SaveChanges(auth.CookieValues.UserId);

                        dbTran.Commit();
                        return true;

                    }
                    catch (DbEntityValidationException ex)
                    {
                        dbTran.Rollback();
                        return false;
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
                    if(assetExpenes!=null)
                    {
                        assetExpenes.IsDeleted = true;
                        db.Entry(assetExpenes).State = EntityState.Modified;
                    }             
                    //حذف مخزن السيارة فى حالة ان الاصل سيارة
                    var assetStore = db.Stores.Where(x => !x.IsDeleted && x.AccountTreeId == model.AccountTreeId).FirstOrDefault();
                    if(assetStore != null)
                    {
                        assetStore.IsDeleted = true;
                        db.Entry(assetStore).State = EntityState.Modified;
                    }
                    //حذف قيود اليومية لرصيد اول المدة للاصل
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == Id /*&& x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceFixedAsset*/).ToList();
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
        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

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