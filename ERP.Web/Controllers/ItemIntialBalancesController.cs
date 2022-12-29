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
using ERP.Web.ViewModels;
using System.Security.Cryptography;
using ERP.DAL.Models;
using System.Runtime.Remoting.Contexts;

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
            var items = db.ItemIntialBalances.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).ToList().Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", InvoiceNumber=x.InvoiceNumber, TotalAmount = x.ItemIntialBalanceDetails.Where(d=>!d.IsDeleted).Sum(d=>(double?)d.Amount??0).ToString(), DateIntial = x.DateIntial.ToString(), Actions = n, Num = n }).ToList();
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
                //التأكد من عدم ادخال رصيد اول للصنف 
                    var itemInitBalance = db.ItemIntialBalanceDetails.Where(x => !x.IsDeleted && x.ItemId == vm.ItemId && x.StoreId == vm.StoreId).FirstOrDefault();
                    if (itemInitBalance != null)
                    return Json(new { isValid = false, msg = " تم ادخال رصيد اول للصنف مسبقا " }, JsonRequestBehavior.AllowGet);
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
        public JsonResult CreateEdit(string DT_Datasource, string DateIntial,string Notes,string BranchId)
        {
            DateTime dateInit;
            if (!DateTime.TryParse(DateIntial, out dateInit))
                return Json(new { isValid = false, message = "تأكد من اختيار تاريخ رصيد اول المدة" });

            if (!Guid.TryParse(BranchId,out Guid branchId))
                return Json(new { isValid = false, message = "تأكد من اختيار الفرع" });

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

            if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            {
                List<ItemIntialBalanceDetail> itemIntialBalanceDetails = new List<ItemIntialBalanceDetail>();
                itemIntialBalanceDetails = itemDetailsDT.Select(x => new ItemIntialBalanceDetail
                {
                    ItemId = x.ItemId,
                    Price = x.Price,
                    StoreId = x.StoreId,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                }).ToList();


                    // اضافة الصنف فى رصيد اول المدة للاصناف
                    var newItemInitBalance = new ItemIntialBalance
                    {
                        BranchId = branchId,
                        Notes = Notes,
                        DateIntial = dateInit,
                        ItemIntialBalanceDetails=itemIntialBalanceDetails
                    };
                    //اضافة رقم الفاتورة
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    newItemInitBalance.InvoiceNumber = codePrefix + (db.ItemIntialBalances.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                    db.ItemIntialBalances.Add(newItemInitBalance);

                

                DS = null;
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    return Json(new { isValid = true, message = "تم اضافة رصيد المدة للاصناف بنجاح" });
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
                    // use Transactions
                    var itemIniatialBalance = db.ItemIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                    var itemIntialBalanceDetails = itemIniatialBalance.ItemIntialBalanceDetails.Where(x => !x.IsDeleted).ToList();

                    //نوع الجرد
                    var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                    if (int.TryParse(inventoryType, out int inventoryTypeVal))
                    {
                        //فى حالة الجرد الدورى
                        if (inventoryTypeVal == 1)
                        {
                            //التأكد من عدم وجود حساب فرعى من حساب المخزون
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue)))
                                return Json(new { isValid = false, message = "حساب المخزون ليس بحساب فرعى" });

                            var accountTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                            foreach (var item in itemIntialBalanceDetails)
                            {
                                item.AccountTreeId = accountTreeId;
                            }
                        }
                        //فى حالة الجرد المستمر
                        if (inventoryTypeVal == 2)
                        {

                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد من الاعدادات" });


                    // حساب المخزون
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
                        Debit = itemIntialBalanceDetails.Sum(x=>x.Amount),
                        Notes = $"رصيد أول المدة للاصناف ",
                        TransactionDate = itemIniatialBalance.DateIntial,
                        TransactionId = itemIniatialBalance.Id,
                        BranchId = itemIniatialBalance.BranchId,
                        TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                    });
                    //رأس المال 
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                        Credit = itemIntialBalanceDetails.Sum(x => x.Amount),
                        Notes = $"رصيد أول المدة للاصناف",
                        TransactionDate = itemIniatialBalance.DateIntial,
                        TransactionId = itemIniatialBalance.Id,
                        BranchId = itemIniatialBalance.BranchId,
                        TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                    });

                    //تحديث حالة الاعتماد 
                    itemIniatialBalance.IsApproval = true;
                    DS = null;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم اعتماد رصيد اول للاصناف بنجاح" });
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
                    var itemIntialBalanceDetails = model.ItemIntialBalanceDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in itemIntialBalanceDetails)
                    {
                        item.IsDeleted = true;
                    }
                    //حذف القيود
                    var items = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
                    foreach (var item in items)
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
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {

                    var itemIntialBalanceDetails = model.ItemIntialBalanceDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in itemIntialBalanceDetails)
                    {
                        //هل الصنف يسمح بالسحب منه بالسالب
                        var result = itemService.IsAllowNoBalance(item.ItemId, item.StoreId, item.Quantity);
                        if (!result.IsValid)
                            return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });
                    }
                    //تحديث حالة الاعتماد 
                    model.IsApproval = false;
                    //حذف قيود اليومية
                    var generalDailies = db.GeneralDailies.Where(x => x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.InitialBalanceItem).ToList();
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
        #region عرض بيانات عملية تحويل مخزنى بالتفصيل وطباعتها
        public ActionResult ShowDetails(string id)
        {

            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var vm = db.ItemIntialBalances.Where(x => x.Id == Id).FirstOrDefault();
                if (vm != null)
                    return View(vm);
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

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