using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Models;

namespace ERP.Web.Controllers
{
    [Authorization]
    public class StorePermissionsLeaveController : Controller
    {
        // GET: StorePermissionsLeave
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        public StorePermissionsLeaveController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
        }
        public static string DS { get; set; }

        public ActionResult Index()
        {
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", defaultStore?.Id);
            ViewBag.ItemId = new SelectList(db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }), "Id", "Name");
            ViewBag.PersonId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.PersonTypeId != 5).Select(x => new { Id = x.Id, Name = x.Name }), "Id", "Name");
            ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name", db.Safes.Select(x => x.Id).FirstOrDefault());

            return View();
        }
        public ActionResult GetAll()
        {
            int? n = null;
            // اسماء الاصناف من جدول الاصناف
            var items = db.StorePermissions.Where(x => !x.IsDeleted && !x.IsReceive).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApproval = x.IsApproval, IsApprovalStatus = x.IsApproval ? "معتمده" : "غير معتمده", StoreName = x.Store.Name, SafeName = x.Safe.Name, PersonName = x.Person.Name, Total = x.StorePermissionItems.Sum(y => y.Amount), CreatedOn = x.PermissionDate.ToString(), Actions = n, Num = n }).ToList();
            //الاصناف من جدول القيود اليومية بعد عمل جروبينج بسبب ان لكل صنف عمليتين يتم تسجيلهم فى جدول القيود اليومية
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
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
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
            var defaultStore = storeService.GetDefaultStore(db);
            var branchId = defaultStore != null ? defaultStore.BranchId : null;
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", defaultStore?.Id);
            ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == branchId), "Id", "Name", db.Safes.Select(x => x.Id).FirstOrDefault());
            ViewBag.PersonId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.PersonTypeId != 5).Select(x => new { Id = x.Id, Name = x.Name }), "Id", "Name");
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");


            return View();
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(StorePermission vm, string DT_Datasource)
        {


            if (vm.StoreId == null || vm.SafeId == null || vm.PersonId == null)
            {
                return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
            }
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
            var Person = db.Persons.Where(x => x.Id == vm.PersonId).FirstOrDefault();
            if (Person.PersonTypeId == (int)PersonTypeCl.Supplier)
            {
                vm.AccountTreeId = Person.AccountTreeSupplierId;
            }
            else
            {
                vm.AccountTreeId = Person.AccountsTreeCustomerId;
            }
            vm.IsReceive = false;
            var StorePermissionItemList = new List<StorePermissionItem>();
            foreach (var item in itemDetailsDT)
            {
                // اضافة الصنف فى رصيد اول المدة للاصناف
                StorePermissionItem items = new StorePermissionItem
                {
                    Amount = item.Amount,
                    ItemId = item.ItemId,
                    Price = item.Price,
                    Quantity = item.Quantity,
                };
                StorePermissionItemList.Add(items);
            }
            vm.StorePermissionItems = StorePermissionItemList;
            db.StorePermissions.Add(vm);
            DS = null;
            if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                return Json(new { isValid = true, message = "تم اضافة اذن الصرف بنجاح" });
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });


        }


        [HttpPost]
        public ActionResult Approval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.StorePermissions.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();


                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.AccountTreeId))
                        return Json(new { isValid = false, message = "حساب الشخص ليس بحساب فرعى" });

                    //التأكد من عدم وجود حساب فرعى من حساب الخزينة
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(model.Safe.AccountsTreeId))
                        return Json(new { isValid = false, message = "حساب الخزينة ليس بحساب فرعى" });
                    //التأكد من عدم تكرار اعتماد القيد
                    if (GeneralDailyService.GeneralDailaiyExists(model.Id, (int)TransactionsTypesCl.Expense))
                        return Json(new { isValid = false, message = "تم الاعتماد مسبقا " });
                    //ال حساب الشخص
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = model.AccountTreeId,
                        Credit = model.StorePermissionItems.Sum(x => x.Amount),
                        BranchId = model.Safe.BranchId,
                        Notes = $" اذن صرف الى حساب {model.AccountTree.AccountName}",
                        TransactionDate = model.PermissionDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.StorePermissionLeave
                    });

                    // من ح/ حساب الخزينة
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = model.Safe.AccountsTreeId,
                        Debit = model.StorePermissionItems.Sum(x => x.Amount),
                        BranchId = model.Safe.BranchId,
                        Notes = $" اذن صرف من حساب {model.Safe.Name}",
                        TransactionDate = model.PermissionDate,
                        TransactionId = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.StorePermissionLeave
                    });


                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = model.AccountTreeId,
                        BranchId = model.Safe.BranchId,
                        Debit = model.StorePermissionItems.Sum(x => x.Amount),
                        Notes = $"اذن صرف من حساب {model.Safe.Name}",
                        TransactionDate = model.PermissionDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    //الى ح/ المبيعات
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue),
                        BranchId = model.Safe.BranchId,
                        Credit = model.StorePermissionItems.Sum(x => x.Amount),
                        Notes = $"اذن صرف الى حساب {model.Person.Name}",
                        TransactionDate = model.PermissionDate,
                        TransactionId = model.Id,
                        TransactionShared = model.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    //تحديث حالة الاعتماد 
                    model.IsApproval = true;
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
                var model = db.StorePermissions.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
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
                var model = db.StorePermissions.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (model != null)
                {
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