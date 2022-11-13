using Newtonsoft.Json;
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

    public class ItemIntialBalancesController : Controller
    {
        // تم ربط جدول رصيد اول المدة للاصناف ItemIntialBalances
        // بجدول القيود اليومية من خلال رقم حساب المخزون 
        //وليس رقم id ItemIntialBalances
        // بسبب تشابه نفس الاي دى مع ارقام الاى دى اخرى للخزنة وحسابات البنوك

        // GET: ItemIntialBalances
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        ItemService itemService;
        public ItemIntialBalancesController()
        {
            db= new VTSaleEntities();
            storeService= new StoreService();   
            itemService= new ItemService();   
        }
        public static string DS { get; set; }

        public ActionResult Index()
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId&&!x.IsDamages), "Id", "Name",defaultStore?.Id);
            ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }), "Id", "Name");
            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            // اسماء الاصناف من جدول الاصناف
            var items = db.ItemIntialBalances.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", ItemName = x.Item.ItemCode + " | " + x.Item.Name, Quantity = x.Quantity, Price = x.Price, Amount = x.Amount, StoreName = x.Store.Name, CreatedOn = x.DateIntial.ToString(), Actions = n, Num = n }).ToList();
            //الاصناف من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل صنف عمليتين يتم تسجيلهم فى جدول القيود اليومية
            //var generalGrouping = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == (int)TransactionsTypesCl.BalanceFirstDuration && items.Any(p => p.AccountTreeId == x.TransactionId && !p.IsDeleted)).Select(x => new { TransactionId = x.TransactionId, ItemName = items.Where(p => p.AccountTreeId == x.TransactionId).FirstOrDefault().Item.Name, Quantity= items.Where(p => p.AccountTreeId == x.TransactionId).FirstOrDefault().Quantity,Price= items.Where(p => p.AccountTreeId == x.TransactionId).FirstOrDefault().Price, Amount = x.Credit + x.Debit,StoreName= items.Where(p => p.AccountTreeId == x.TransactionId).FirstOrDefault().Store.Name, CreatedOn = x.CreatedOn.ToString(), Actions = n, Num = n }).GroupBy(x => new { x.TransactionId, x.ItemName,x.Quantity,x.Price,x.StoreName, x.Num, x.Actions, x.Amount, x.CreatedOn }).ToList();
            //var data = generalGrouping.Select(x => new
            //{
            //    TransactionId = x.FirstOrDefault().TransactionId,
            //    ItemName = x.FirstOrDefault().ItemName,
            //    Quantity = x.FirstOrDefault().Quantity,
            //    Price = x.FirstOrDefault().Price,
            //    Amount = x.FirstOrDefault().Amount,
            //    StoreName = x.FirstOrDefault().StoreName,
            //    CreatedOn = x.FirstOrDefault().CreatedOn,
            //    Actions = n,
            //    Num = n
            //}).ToList();
            return Json(new
            {
                data = items
            }, JsonRequestBehavior.AllowGet);

        }

        #region  اضافة الاصناف 
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new List<ItemDetailsDT>()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && x.StoreId == vm.StoreId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ContainerId = vm.ContainerId, StoreId = vm.StoreId, StoreName = storeName };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit()
        {
            DS = null;
            // add
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId&&!x.IsDamages), "Id", "Name", defaultStore?.Id);
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");


            return View();
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(string DT_Datasource, string DateIntial)
        {
            DateTime dateInit;
            if (!DateTime.TryParse(DateIntial, out dateInit))
                return Json(new { isValid = false, message = "تأكد من اختيار تاريخ رصيد اول المدة" });

            dateInit = dateInit.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
            //الاصناف
            List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();

            if (DT_Datasource != null)
            {
                itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_Datasource);
                if (itemDetailsDT.Count() == 0)
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
            //التأكد من عدم تسجيل مسبق لارصدة اول المدة للاصناف
            int count = 1;
            var listMsg = "";

            foreach (var item in itemDetailsDT)
            {
                var itemInitBalance = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == item.ItemId && x.StoreId == item.StoreId).FirstOrDefault();
                if (itemInitBalance != null)
                {
                    if (count == 1)
                    {
                        listMsg += "تم تسجيل رصيد اول المدة للاصناف :- \r\n";
                        listMsg += item.ItemName + "\r\n";
                        count++;
                    }
                    else
                        listMsg += " -- " + item.ItemName + "\r\n";
                }
            }
            if (count > 1)
                return Json(new { isValid = false, message = listMsg });

            //    //تسجيل القيود
            // General Dailies
            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                // الحصول على حسابات من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                // التأكد من عدم وجود حساب فرعى من الحساب رأس المال
                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });
                //التأكد من عدم وجود حساب فرعى من حساب المخزون
                if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue)))
                    return Json(new { isValid = false, message = "حساب المخزون ليس بحساب فرعى" });

                foreach (var item in itemDetailsDT)
                {
                    //var itemInitBalance = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == item.ItemId).FirstOrDefault();
                    //if (itemInitBalance != null)
                    //    return Json(new { isValid = false, message = $"تم تسجيل رصيد اول المدة للصنف {item.ItemName} من قبل" });

                    // اضافة الصنف فى رصيد اول المدة للاصناف
                    var newItemInitBalance = new ItemIntialBalance
                    {
                        AccountTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
                        Amount = item.Amount,
                        ItemId = item.ItemId,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId,
                        DateIntial = dateInit
                    };

                    db.ItemIntialBalances.Add(newItemInitBalance);

                }

                DS = null;
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم اضافة رصيد المدة للصنف بنجاح" });
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


            }
            else
                return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });

            //}


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
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    // التأكد من عدم وجود حساب فرعى من الحساب رأس المال
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب رأس المال ليس بحساب فرعى" });
                    //التأكد من عدم وجود حساب فرعى من حساب المخزون
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue)))
                        return Json(new { isValid = false, message = "حساب المخزون ليس بحساب فرعى" });
                    // use Transactions
                    var itemIniatialBalance = db.ItemIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                    var branchId = itemIniatialBalance.Store != null ? itemIniatialBalance.Store.BranchId : null;

                    // حساب المخزون
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
                        Debit = itemIniatialBalance.Amount,
                        Notes = $"رصيد أول المدة للصنف : {itemIniatialBalance.Item.Name}",
                        TransactionDate = itemIniatialBalance.DateIntial,
                        TransactionId = itemIniatialBalance.Id,
                        BranchId = branchId,
                        TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                    });
                    //رأس المال 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                        Credit = itemIniatialBalance.Amount,
                        Notes = $"رصيد أول المدة للصنف : {itemIniatialBalance.Item.Name}",
                        TransactionDate = itemIniatialBalance.DateIntial,
                        TransactionId = itemIniatialBalance.Id,
                        BranchId = branchId,
                        TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                    });

                    //تحديث حالة الاعتماد 
                    itemIniatialBalance.IsApproval = true;
                    db.Entry(itemIniatialBalance).State = EntityState.Modified;
                    DS = null;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد الصنف بنجاح" });
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
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    ////هل الصنف يسمح بالسحب منه بالسالب
                    //var result = itemService.IsAllowNoBalance(model.ItemId, model.StoreId, model.Quantity);
                    //if (!result.IsValid)
                    //    return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف القيود
                    var items = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
                    foreach (var item in items)
                    {
                        item.IsDeleted = true;
                        db.Entry(item).State = EntityState.Modified;
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
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //هل الصنف يسمح بالسحب منه بالسالب
                    var result = itemService.IsAllowNoBalance(model.ItemId, model.StoreId, model.Quantity);
                    if (!result.IsValid)
                        return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });

                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
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