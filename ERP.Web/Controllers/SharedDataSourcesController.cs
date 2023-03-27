using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;

namespace ERP.Web.Controllers
{
    public class SharedDataSourcesController : Controller
    {
        VTSaleEntities db;
        ItemService _itemService;
        // GET: SharedDataSources
        public SharedDataSourcesController()
        {
            db = new VTSaleEntities();
            _itemService = new ItemService();
        }

        // GET: SharedDataSources
        public ActionResult Index()
        {
            return View();
        }

        // Get cities by countryId
        public JsonResult onCountryChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Cities.Where(x => !x.IsDeleted && x.CountryId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }


        public JsonResult onCityChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Areas.Where(x => !x.IsDeleted && x.CityId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult onAreaChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                //var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                var list = db.Branches.Where(x => !x.IsDeleted && x.AreaId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult onAreaChangeGetRegions(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Regions.Where(x => !x.IsDeleted && x.AreaId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult onRegionChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Districts.Where(x => !x.IsDeleted && x.RegionId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindProductionOrderColors()
        {
            var list = db.ProductionOrderColors.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.Name, ColorHEX = x.ColorHEX }).ToList();
            var selectList = new SelectList(list, "Id", "Name");
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult onBranchChange(string id)
        //{
        //    Guid Id;
        //    if (Guid.TryParse(id, out Id))
        //    {
        //        var list = db.Persons.Where(x => !x.IsDeleted && x.PersonTypeId == (int)PersonTypeCl.Employee && x.Employees.Any(e => !e.IsDeleted && (e.EmployeeBranches.Where(n=>!n.IsDeleted).FirstOrDefault().BranchId == Id || e.EmployeeBranches.Where(n => !n.IsDeleted).FirstOrDefault().BranchId == null))).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
        //        var selectList = new SelectList(list, "Id", "Name");
        //        return Json(selectList.Items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult getSafesOnBranchChanged(string id,string userId) // تحميل الخزن بدلالة رقم الفرع 
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list =EmployeeService.GetSafesByUser(id, userId);
                if(list.Count()>0)
                {
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
                
                //if (Guid.TryParse(userId,out Guid usrId))
                //{
                //    var user = db.Users.Where(x => x.Id ==usrId).FirstOrDefault();
                //    if (user!=null)
                //    {
                //        //فى حالة اليوزر ادمن 
                //        IQueryable<Safe> safes = null;
                //        if (user.IsAdmin)
                //            safes = db.Safes.Where(x => !x.IsDeleted && x.BranchId == Id);
                //        else
                //        {
                //            var employee= user?.Person?.Employees.Where(x=>!x.IsDeleted).FirstOrDefault();
                //            var empSafes = db.EmployeeSafes.Where(x => !x.IsDeleted);
                //            safes = empSafes.Where(x => x.EmployeeId == employee.Id).Select(x => x.Safe);
                //            safes = safes.Where(x => !x.IsDeleted && x.BranchId == Id);
                //        }
                //        if (safes!=null)
                //        {
                //            var list =safes.Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                //            var selectList = new SelectList(list, "Id", "Name");
                //            return Json(selectList.Items, JsonRequestBehavior.AllowGet);

                //        }
                //        else
                //            return Json(null, JsonRequestBehavior.AllowGet);

                //    }
                //    else
                //        return Json(null, JsonRequestBehavior.AllowGet);


                //}
                //else
                //    return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getStoresOnBranchChanged(string id, bool isDamage = false, string userId=null,bool allStore=false) //  تحميل المخازن بدون مخازن التوالف بدلالة رقم الفرع 
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                if(allStore==true)
                {
                    var allStores = db.Stores.Where(x => !x.IsDeleted && x.BranchId == Id && !x.IsDamages);
                    return Json(allStores, JsonRequestBehavior.AllowGet);
                }
                var list = EmployeeService.GetStoresByUser(id, userId,isDamage);
                if (list.Count() > 0)
                {
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
                //if (Guid.TryParse(userId, out Guid usrId))
                //{
                //    var user = db.Users.Where(x => x.Id == usrId).FirstOrDefault();
                //    if (user != null)
                //    {
                //        //فى حالة اليوزر ادمن 
                //        IQueryable<Store> stores = null;
                //        if (user.IsAdmin)
                //            stores = db.Stores.Where(x => !x.IsDeleted && x.BranchId == Id && x.IsDamages == isDamage);
                //        else
                //        {
                //            var employee = user?.Person?.Employees.Where(x => !x.IsDeleted).FirstOrDefault();
                //            var empStores = db.EmployeeStores.Where(x => !x.IsDeleted);
                //            stores = empStores.Where(x => x.EmployeeId == employee.Id).Select(x => x.Store);
                //            stores = stores.Where(x => !x.IsDeleted && x.BranchId == Id && x.IsDamages == isDamage);
                //        }
                //        if (stores != null)
                //        {
                //            var list = stores.Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                //            var selectList = new SelectList(list, "Id", "Name");
                //            return Json(selectList.Items, JsonRequestBehavior.AllowGet);

                //        }
                //        else
                //            return Json(null, JsonRequestBehavior.AllowGet);

                //    }
                //    else
                //        return Json(null, JsonRequestBehavior.AllowGet);


                //}
                //else
                //    return Json(null, JsonRequestBehavior.AllowGet);            
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);

                //var list = db.Stores.Where(x => !x.IsDeleted && x.BranchId == Id && x.IsDamages == isDamage).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                //var selectList = new SelectList(list, "Id", "Name");
                //return Json(selectList.Items, JsonRequestBehavior.AllowGet);
        }
        //تحميل المخازن والخزن عند اضافة موظف 
        public JsonResult getStoresOnMultiplBranchesChanged(string id) //  تحميل المخازن بدون مخازن التوالف بدلالة رقم الفرع 
        {
            var idString= id.Split(',').ToList();
            List<Guid> Ids=idString.Select(Guid.Parse).ToList();
            if (Ids.Count() > 0 && Ids != null)
            {
                var listStores = db.Stores.Where(x => !x.IsDeleted && Ids.Any(b => b == x.BranchId) && !x.IsDamages).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectListStore = new SelectList(listStores, "Id", "Name");
                var listSafes = db.Safes.Where(x => !x.IsDeleted && Ids.Any(b => b == x.BranchId)).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectListSafe = new SelectList(listSafes, "Id", "Name");
                return Json(new { selectListStore=selectListStore.Items, selectListSafe = selectListSafe.Items }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult getStoreDamagesOnBranchChanged(string id) //  تحميل  مخازن التوالف فقط بدلالة رقم الفرع 
        //{
        //    int Id;
        //    if (int.TryParse(id, out Id))
        //    {
        //        var list = db.Stores.Where(x => !x.IsDeleted && x.BranchId == Id&&x.IsDamages).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
        //        var selectList = new SelectList(list, "Id", "Name");
        //        return Json(selectList.Items, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //}   
        //الاصناف بدلالة نوعها (خام - وسيط - نهائى)
        public JsonResult ItemsOnGroupChanged(string id) //group type
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Items.Where(x => !x.IsDeleted && (x.GroupBasicId == Id)).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //وحدات الاصناف 
        public JsonResult GeItemUnits(string id) //Item id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var item = db.Items.Where(x => x.Id == Id).FirstOrDefault();
                var units = item.ItemUnits.Where(x => !x.IsDeleted).Select(x => new  { Id = x.Id, Name = x.Unit.Name,SellPrice=x.SellPrice }).ToList();
                //var selectList = new SelectList(units, "Id", "Name");
                return Json(units, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //وحدات الاصناف عرض سعر بيع الوحدة
        public JsonResult GetItemUnitPrice(string id) //ItemUnit id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var itemunits = db.ItemUnits.Where(x => x.Id == Id).FirstOrDefault();
               if(itemunits!=null)
                    return Json(itemunits.SellPrice, JsonRequestBehavior.AllowGet);
               else
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //اظهار اصناف البيع فقط عند اختيار مجموعة
        public JsonResult ItemsSellOnGroupChanged(string id) //group type
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Items.Where(x => !x.IsDeleted && x.GroupBasicId == Id && x.AvaliableToSell).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //اظهار اصناف البيع حسب الباركود
        public JsonResult ItemsBarcodeEnter(string barcode) //barcode
        {
            if (!string.IsNullOrEmpty(barcode))
            {
                var list = db.Items.Where(x => !x.IsDeleted && x.BarCode == barcode).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //اظهار اصناف البيع حسب السيريال
        public JsonResult ItemsSerialEnter(string serial, bool isReturnBack = true) //serial ,isReturnBack =true فى حالة اضافة اصناف فى الصيانة والمرتجع يتم التأكد من ان السيريال تم بيعه وليس موجود فى امخازنا والعكس صحيح 
        {
            if (!string.IsNullOrEmpty(serial))
            {

                var itemSerials = db.ItemSerials.Where(x => !x.IsDeleted && x.SerialNumber == serial).Take(1);
                if (isReturnBack)
                    //فى حالة الصيانة والمرتجع 
                    itemSerials = itemSerials.Where(x => x.SellInvoiceId != null);
                else
                    //فى حالة البيع وقطع الغيار  
                    itemSerials = itemSerials.Where(x => x.SellInvoiceId == null);
                if (itemSerials.Count() == 0)
                    return Json(new { }, JsonRequestBehavior.AllowGet);
                var lastCase = "";
                if (itemSerials.FirstOrDefault().SerialCaseId == (int)SerialCaseCl.SellBack)
                    lastCase = "مرتجع بيع";
                if (itemSerials.FirstOrDefault().SerialCaseId == (int)SerialCaseCl.RepairedReceiptStore)
                    lastCase = " صيانه بالمخزن";
                var txt = $"سيريال : {itemSerials.FirstOrDefault().SerialNumber} | {lastCase}";
                var list = itemSerials.Select(x => new { SerialItemId = x.Id, ProductionOrderId = x.ProductionOrderId, IsIntial = x.IsItemIntial, Id = x.Item.Id, Name = x.Item.ItemCode + " | " + x.Item.Name, CurrentStore = x.CurrentStoreId, CmboBalanceTxt = txt }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //اظهار سعر البيع الافتراضى عند اختيار صنف ما
        public JsonResult GetDefaultSellPrice(string itemId) //item id
        {
            Guid Id;
            if (Guid.TryParse(itemId, out Id))
            {
                var itemSellPrice = db.Items.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault().SellPrice;
                return Json(new { data = itemSellPrice }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(0, JsonRequestBehavior.AllowGet);
        }
        //تغيير سعر البيع حسب سياسة البيع المحدد
        public JsonResult GetPricePolicySellPrice(string itemId, Guid? pricePolicyId, Guid? personId,bool isCustomer) //price policy id personId(customer/supplier)
        {
            Guid Id;
            if (Guid.TryParse(itemId, out Id) && pricePolicyId != null)
            {
                IQueryable<ItemPrice> itemPrice = null;
                double? itemSellPrice = 0;
                itemPrice = db.ItemPrices.Where(x => !x.IsDeleted && x.ItemId == Id && x.PricingPolicyId == pricePolicyId);
                if (isCustomer)
                    itemPrice = itemPrice.Where(x => x.CustomerId == personId);
                else//supplier
                    itemPrice = itemPrice.Where(x => x.SupplierId == personId);

                if (itemPrice.Count()==0)
                    itemSellPrice = 0;
                else
                    itemSellPrice = itemPrice.FirstOrDefault().SellPrice;

                return Json(new { data = itemSellPrice }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //اخر اسعار سعر بيع للصنف
        public JsonResult GetPreviousPrices(string itemId, bool isSell) //item id
        {
            Guid Id;
            if (Guid.TryParse(itemId, out Id))
            {
                SelectList selectList = null;
                if (isSell)
                {
                    var items = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == Id).GroupBy(x => x.Price);
                    selectList = new SelectList(items.Select(x => new { Id = x.FirstOrDefault().Price, Name = x.FirstOrDefault().Price }).ToList(), "Id", "Name");
                }
                else
                {
                    var items = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == Id).GroupBy(x => x.Price);
                    selectList = new SelectList(items.Select(x => new { Id = x.FirstOrDefault().Price, Name = x.FirstOrDefault().Price }).ToList(), "Id", "Name");
                }
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //السعر من جدول تحديد اسعار البيع تلقائيا حسب الفرع/الفئة/الصنف
        public JsonResult GetItemCustomSellPrice(string itemId, string branchId) //item id
        {
            IQueryable<ItemCustomSellPrice> itemCustomSellPrices = null;
            IQueryable<ItemCustomSellPrice> itemCustomSellPrice = null;
            double profitPercentage = 0;
            if(Guid.TryParse(itemId,out Guid ItemId)&& Guid.TryParse(branchId, out Guid BranchId))
            {
                itemCustomSellPrices = db.ItemCustomSellPrices.Where(x => !x.IsDeleted);
                var item = db.Items.Where(x => x.Id == ItemId).FirstOrDefault();
                itemCustomSellPrice = itemCustomSellPrices.Where(x => x.ItemId == item.Id && x.BranchId == BranchId);
                if (itemCustomSellPrice.Count() == 0)
                {
                    itemCustomSellPrice = itemCustomSellPrices.Where(x => x.GroupBasicId == item.GroupBasicId && x.BranchId == BranchId);
                    if (itemCustomSellPrice.Count() == 0)
                    {
                        itemCustomSellPrice = itemCustomSellPrices.Where(x => x.GroupBasicId == item.GroupBasicId);
                        if (itemCustomSellPrice.Count() == 0)
                        {
                            itemCustomSellPrice = itemCustomSellPrices.Where(x => x.BranchId == BranchId);
                            if (itemCustomSellPrice.Count() == 0)
                            {
                                itemCustomSellPrice = itemCustomSellPrices.Where(x => x.ItemId == item.Id);
                            }
                        }

                    }
                }
                if (itemCustomSellPrice.Count() > 0)
                {
                    profitPercentage = itemCustomSellPrice.OrderByDescending(x => x.Id).FirstOrDefault().ProfitPercentage;
                    //متوسط سعر الشراء 
                    var costCalculationPurchase = Math.Round(_itemService.GetItemCostCalculation(1, item.Id), 2, MidpointRounding.ToEven);
                    var val = costCalculationPurchase * profitPercentage / 100;
                    return Json(new { data = Math.Round(val + costCalculationPurchase, 2, MidpointRounding.ToEven) }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { data=0}, JsonRequestBehavior.AllowGet);

        }

        //الاصناف بدلالة مجموعتها الاساسية (خام - وسيط - نهائى)
        public JsonResult OnItemTypeChange(string id, bool hasItemProduction = false) // item type,Has item producation
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                SelectList selectList = null;
                IQueryable<Item> items = db.Items.Where(x => !x.IsDeleted && x.ItemTypeId == Id && x.GroupBasic.GroupTypeId == (int)GroupTypeCl.Basic);
                //if (hasItemProduction) // الاصناف الموجود لها توليفة مسجله مسبقا بدلالة مجموعتها 
                //    items = items.Where(x => x.ItemProductions.Where(y => !y.IsDeleted).Any(p => p.ItemFinalId == x.Id));
                // الاصناف كلها بدلالة مجموعتها 
                var list = items.Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
                selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        //التحويل بين الوحدات )
        public JsonResult UnitConvert(string id) // item id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Items.Where(x => !x.IsDeleted && x.Id == Id).Select(x => new { UnitConvertFromName = x.UnitConvertFrom.Name, UnitConvertFromCount = x.UnitConvertFromCount }).FirstOrDefault();
                return Json(new { isValid = list.UnitConvertFromName != null ? true : false, unitConvertFromName = list.UnitConvertFromName, unitConvertFromCount = list.UnitConvertFromCount }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isValid = false }, JsonRequestBehavior.AllowGet);
        }
        //العملاء بدلالة فئة العملاء والمندوب ان وجد )
        public JsonResult GetCustomerOnCategoryChange(string id, string saleMenId) // category id
        {
            Guid Id;
            Guid saleMenID;
            if (Guid.TryParse(id, out Id))
            {
                SelectList selectList = null;
                if (Guid.TryParse(saleMenId, out saleMenID))
                {
                    var list1 = db.SaleMenCustomers.Where(x => !x.IsDeleted && x.EmployeeId == saleMenID && x.CustomerPerson.PersonCategoryId == Id).Select(x => new { Id = x.CustomerPerson.Id, Name = x.CustomerPerson.Name }).ToList();
                    selectList = new SelectList(list1, "Id", "Name");
                }
                else
                {
                    var list2 = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) && x.PersonCategoryId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                    selectList = new SelectList(list2, "Id", "Name");
                }
                //list = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)&&x.PersonCategoryId==Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                //var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //الموردين بدلالة فئة الموردين  )
        public JsonResult GetSupplierOnCategoryChange(string id) // category id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                SelectList selectList = null;
                var list2 = db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer) && x.PersonCategoryId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                selectList = new SelectList(list2, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult onGroupTypeChange(string id)
        {

            int Id;
            if (int.TryParse(id, out Id))
            {
                int? parentId = null;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var data = GroupService.GetItemGroupTree(Id);
                stopwatch.Stop();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountsTrees(bool selectedTree = false, bool showAllLevel = false, int? spcLevel = null, string accountsexption = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = AccountTreeService.GetAccountTree(selectedTree, showAllLevel, spcLevel, accountsexption);
            stopwatch.Stop();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public bool isHaveChilds(Guid? parentId)
        {
            var GroupChild = db.Groups.Where(x => !x.IsDeleted && x.ParentId == parentId).Count();
            if (GroupChild > 0)
                return true;
            else
                return false;
        }
        public List<DrawTree> GetChild(int? parentId, int typId)
        {
            return db.Groups.Where(x => !x.IsDeleted && x.GroupTypeId == typId).Select(x => new DrawTree
            {
                id = x.Id,
                title = x.Name,
                ParentId = x.ParentId
            }).ToList();
        }
        #region Group Tree View
        public JsonResult GetItemGroupView()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = new List<TreeViewDraw>();
            data = _itemService.GetGroups();

            stopwatch.Stop();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Upload Center 
        public JsonResult GetUploadCenters(bool selectedTree = false)
        {
            var data = UploadCenterService.GetUploadCenter(selectedTree);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Acccount Tree View
        public JsonResult GetAccountsTreeView()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = new List<TreeViewDraw>();
            //if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
            //{
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                DateTime dtFrome = DateTime.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue);
                DateTime dtTo = DateTime.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue);
                data = AccountTreeService.GetAccountTreeView(dtFrome, dtTo);
            //}
            //else
            //    ViewBag.Msg = "يجب تعريف بداية ونهاية السنة المالية فى شاشة الاعدادات";

            stopwatch.Stop();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Balances Store 
        public JsonResult GetBalanceByStore(string itemId, string storeId)
        {
            if (Guid.TryParse(itemId, out var itmId) && Guid.TryParse(storeId, out var stId))
            {
                var data = BalanceService.GetBalance(itmId, stId);
                return Json(new { balance = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { balance = 0 }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductionOrdersOnStoreChange(string itemId, string storeId)
        {
            if (Guid.TryParse(itemId, out var itmId) && Guid.TryParse(storeId, out var stId))
            {
                var data = BalanceService.GetBalanceProductionOrder(stId, itmId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        //رصيد صنف
        public JsonResult GetBalanceItem(string itemId)
        {
            if (Guid.TryParse(itemId, out var itmId))
            {
                var data = BalanceService.GetBalance(itmId, null);
                return Json(new { balance = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { balance = 0 }, JsonRequestBehavior.AllowGet);
        }     
        // محجوز غير معتمد بعد رصيد صنف
        public JsonResult GetBalanceNotApproval(string itemId, string storeId)
        {
            if (Guid.TryParse(itemId, out var itmId))
            {
                Guid.TryParse(storeId, out var stId);
                var data = BalanceService.GetBalanceNotApproval(itmId, stId);
                return Json(new { balance = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { balance = 0 }, JsonRequestBehavior.AllowGet);
        }
        // تكلفة المنتج بدلالة (متوسط سعر الشراء - اخر سعر شراء - اعلى او اقل سعر شراء )
        public JsonResult GetPriceOnItemCostCalculateChange(string itemCostCalcId, string itemId)
        {
            if (int.TryParse(itemCostCalcId, out var itmCostId) || Guid.TryParse(itemId, out var itmId))
            {
                var data = _itemService.GetItemCostCalculation(int.Parse(itemCostCalcId), Guid.Parse(itemId));
                return Json(new { price = data }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { price = 0 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region UserRoles
        public JsonResult GetAllPages(Guid? roleId)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var data = UsersRoleService.GetPages(roleId);
            stopwatch.Stop();//46:989,47:497
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //هل للمستخدم صلاحية بقتح الشاشة فى روابط الصفحة الرئيسية للادمن
        public JsonResult CheckUserAuth(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.ToLower();
                VTSAuth auth = new VTSAuth();
                if (auth.LoadDataFromCookies())
                {
                    var roleId = auth.CookieValues.RoleId;
                    //التأكد من عمد انتها صلاحية الجلسة
                    if (Utility.GetDateTime() > auth.CookieValues.SessionKey.ExpireDate)
                        return Json(new { isValid = false, Msg = "انتهاء صلاحية الجلسة ", Redirect = true }, JsonRequestBehavior.AllowGet);

                    IQueryable<PagesRole> pages = null;
                    if (!VTSAuth.IsDemo)
                    {
                        var sessionDb = db.Users.Where(x => x.Id == auth.CookieValues.UserId).FirstOrDefault().SessionKey;
                        var sessionCookie = VTSAuth.Encrypt(JsonConvert.SerializeObject(auth.CookieValues.SessionKey));
                        if (sessionDb != sessionCookie)
                            return Json(new { isValid = false, Msg = "تم استخدام جلسه مختلفه ", Redirect = true }, JsonRequestBehavior.AllowGet);

                    }

                    pages = db.PagesRoles.Where(x => !x.IsDeleted && x.RoleId == roleId);
                    if (pages.Count() > 0)
                    {
                        //var pagesSession = Session["pagesSession"] != null;
                        //var pagesSession = UsersRoleService.PagesLoad != null;
                        if (pages.Where(x => x.Page.Url.ToLower() == url || x.Page.OtherUrls.ToLower().Contains(url)).Any() /*&& pagesSession*/)
                            return Json(new { isValid = true, Msg = " ", Redirect = false }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { isValid = false, Msg = "ليس لك صلاحية الوصول للصفحة ", Redirect = false }, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json(new { isValid = false, Msg = "ليس لك صلاحية الوصول للصفحة ", Redirect = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { isValid = false, Msg = "ليس لك صلاحية الوصول للصفحة ", Redirect = true }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isValid = false, Msg = "ليس لك صلاحية الوصول للصفحة ", Redirect = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Employee
        //الموظفين بدلالة الادارة  )
        public JsonResult GetEmployeeByDepartment(string id, bool isUserRole = false, bool isContractReg = false, bool showAll = true,bool isProductionEmp=false) // department id
        {
            /* 
           isUserRole تستخدم عند عرض الموظفين المسموح لهم بتسجيل الدخول فى النظام
           isContractReg تستخدم فى شاشة اضافة عقد جديد بحيث يتم عرض كل الموظفين سواء له عقد او لا 
           showAll تستخدم لعرض كل موظفى التعاقدات (شهرى/اسبوعى/يومى ) او فى حالة الغياب والسلف يكون (شهرى/اسبوعى) فقط
            isProductionEmp تستخدم مع شاشة خط الانتاج وتعرض الموظفين من نوع بالانتاج فقط
             */
            Guid Id;
            //EmployeeService employeeService = new EmployeeService();
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == Id);
                if (!isContractReg)
                {
                    var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                    if (!showAll)
                        contracts = contracts.Where(x => x.ContractSalaryTypeId != (int)ContractSalaryTypeCl.Daily&& x.ContractSalaryTypeId != (int)ContractSalaryTypeCl.Production);

                    list = list.Where(e => contracts.Any(c => c.EmployeeId == e.Id));
                }
                //var list = employeeService.GetEmployees().Where(x => x.DepartmentId == Id);
                if (isUserRole)
                    list = list.Where(x => x.HasRole);
                if (isProductionEmp)
                    list = list.Where(x=>x.Contracts.Where(c=>!c.IsDeleted&&c.IsActive&&c.IsApproval&&c.ContractSalaryTypeId==(int)ContractSalaryTypeCl.Production).Any());

                var list2 = list.Select(x => new { Id = x.Id, Name = x.Person.Name }).ToList();
                var selectList = new SelectList(list2, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult onSalaryAdditionTypeChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.SalaryAdditions.Where(x => !x.IsDeleted && x.SalaryAdditionTypeId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult onSalaryPenaltyTypeChange(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.SalaryPenalties.Where(x => !x.IsDeleted && x.SalaryPenaltyTypeId == Id).Select(x => new { Id = x.Id, Name = x.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalaryByEmployeeId(string id) //مرتب الموظف 
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var contract = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval && x.EmployeeId == Id).FirstOrDefault();
                return Json(contract.Salary, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContractSchedulingByContractId(string id) // اشهر العقد المتبقية  
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var contract = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval && x.EmployeeId == Id).FirstOrDefault();

                var list = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id && !x.IsPayed).Select(x => new { Id = x.Id, Name = x.MonthYear }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        //اشهر/اسابيع/اليوم الموظف المتبقية المتعاقد عليها 
        public JsonResult GetContractSchedulingEmployee(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var contract = db.Contracts.Where(x => x.IsActive && x.EmployeeId == Id).FirstOrDefault();
                var list = db.ContractSchedulings.Where(x => !x.IsDeleted && x.ContractId == contract.Id && !x.IsApproval).OrderBy(x => x.Name).Select(x => new { Id = x.Id, Name = x.Name /*x.MonthYear.ToString().Substring(0,7)*/ }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(new { list = selectList.Items, salaryTypeName = contract.ContractSalaryType.Name, salaryTypeId = contract.ContractSalaryTypeId }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region المناديب
        //المندوبين بدلالة الادارة  )
        public JsonResult GetSaleMenByDepartment(string id) // department id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == Id && x.IsSaleMen);
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                list = list.Where(e => contracts.Any(c => c.EmployeeId == e.Id));

                var list2 = list.Select(x => new { Id = x.Id, Name = x.Person.Name }).ToList();
                var selectList = new SelectList(list2, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //المناديب بدلالة  العملاء 
        public JsonResult GetSaleMenByCustomerId(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var list = db.SaleMenCustomers.Where(x => !x.IsDeleted && x.CustomerId == Id);
                var contracts = db.Contracts.Where(x => !x.IsDeleted && x.IsActive && x.IsApproval);
                var list2 = list.Where(e => contracts.Any(c => c.EmployeeId == e.Employee.Id)).Select(x => new { Id = x.Employee.Id, Name = x.Employee.Person.Name }).ToList();
                var selectList = new SelectList(list2, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //المناديب بدلالة مناطق العملاء (تم إلغائها)
        public JsonResult GetSaleMenByCustomer(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var areas = db.Persons.Where(x => !x.IsDeleted && x.Id == Id).Select(x => x.AreaId);
                var list = db.SaleMenAreas.Where(x => !x.IsDeleted && areas.Any(a => a.Value == x.AreaId)).Select(x => new { Id = x.PersonId, Name = x.Person.Name }).ToList();
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        //المندوب بدلالة المخزن )
        public JsonResult GetSaleMenByStore(string id) //id = storeId
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var saleMenId = EmployeeService.GetSaleMenByStore(Id);
                if (saleMenId != null)
                {
                    var saleMenName = db.Employees.FirstOrDefault(x => x.Id == saleMenId).Person.Name;
                    if (saleMenName != null)
                        return Json(new { saleMenName = "المندوب : " + saleMenName }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { saleMenName = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { saleMenName = "" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { saleMenName = "" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Contract
        //شاشة تسجيل الغياب
        //الحصول على عدد ايام الغياب بدلالة تاريخين من والى فى 
        public JsonResult GetDiffDaysSchedulingAbsence(string dFrom, string dTo)
        {
            DateTime dtFrom, dtTo;
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
            {
                if (dtFrom > dtTo)
                    return Json(new { isValid = false, msg = "التاريخ (من) اكبر من التاريخ (الى) تأكد من ادخال التواريخ بشكل صحيح" }, JsonRequestBehavior.AllowGet);
                var days = (dtTo - dtFrom).Days + 1;
                if (dtFrom == dtTo)
                    days = 1;
                return Json(new { isValid = true, val = days }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isValid = false, msg = "التاريخ (من) اكبر من التاريخ (الى) تأكد من ادخال التواريخ بشكل صحيح" }, JsonRequestBehavior.AllowGet);
        }
        //شاشة تسجيل الغياب لموظف 
        //الحصول على عدد ايام الاجازة المتاحه للموظف  بدلالة نوع الاجازة 
        public JsonResult GetBalanceVacationDays(string schedulingId, string vcationTypeId, string dayAbs)
        {
            if (dayAbs == null || dayAbs == "0")
                return Json(new { isValid = false, msg = "تأكد من وجود عدد ايام الغياب" }, JsonRequestBehavior.AllowGet);

            Guid contractSchedulingId, vacationTypeId;
            if (Guid.TryParse(schedulingId, out contractSchedulingId) && Guid.TryParse(vcationTypeId, out vacationTypeId))
            {
                var contractScheduling = db.ContractSchedulings.FirstOrDefault(x => x.Id == contractSchedulingId);

                var sumDayBalanceAvaliable = 0;
                var dayBalanceAvaliable = db.ContractDefinitionVacations.Where(x => !x.IsDeleted && x.ContractId == contractScheduling.ContractId && x.VacationTypeId == vacationTypeId).ToList();
                if (dayBalanceAvaliable != null)
                    sumDayBalanceAvaliable = dayBalanceAvaliable.Sum(x => x.DayNumber);

                double sumAbsenceInPrevoiusScheduling = 0;
                var absenceInPrevoiusScheduling = db.ContractSchedulingAbsences.Where(x => !x.IsDeleted && x.ContractSchedulingId == contractSchedulingId && x.VacationTypeId == vacationTypeId).ToList();
                if (absenceInPrevoiusScheduling != null)
                    sumAbsenceInPrevoiusScheduling = absenceInPrevoiusScheduling.Sum(x => x.AbsenceDayNumber);


                if ((sumDayBalanceAvaliable - sumAbsenceInPrevoiusScheduling) < int.Parse(dayAbs))
                    return Json(new { isValid = false, val = sumDayBalanceAvaliable - sumAbsenceInPrevoiusScheduling, msg = "عدد ايام الغياب اكبر من الرصيد المتاح " }, JsonRequestBehavior.AllowGet);

                return Json(new { isValid = true, val = sumDayBalanceAvaliable - sumAbsenceInPrevoiusScheduling }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { isValid = false, msg = "تأكد من وجود عدد ايام الغياب او نوع الاجازة او الشهر" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region الطباعه 
        //طباعه الفواتير
        public JsonResult PrintInvoice(string id, string typ) // department id
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                PrintInvoiceService printInvoiceService = new PrintInvoiceService();
                var invoice= printInvoiceService.GetInvoiceData(id, typ,null);                
                                
                return Json(new { invoice }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //طباعه التحصيلات النقدى والشيك
        public JsonResult PrintPayment(string id, string typ)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                PrintPaymentDto payment = new PrintPaymentDto();
                // الحصول على بيانات المؤسسة من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                if (typ == "custPaymentCash")
                {
                    var paymentCash = db.CustomerPayments.Where(x => !x.IsDeleted && x.Id == Id);
                    if (paymentCash.Count() > 0)
                    {
                        payment = paymentCash.ToList().Select(x => new PrintPaymentDto
                        {
                            Id = x.Id,
                            BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                            PaymentDate = x.PaymentDate.Value.ToString("yyyy-MM-dd"),
                            SafeBankName = x.Safe != null ? x.Safe.Name : string.Empty,
                            SuppCustomerName = x.PersonCustomer.Name,
                            Amount = x.Amount,
                            Notes = x.Notes,
                            PersonDebit = GetPersonBalance(x.PersonCustomer != null ? x.CustomerId : null, true).Data,
                            //بيانات الجهة
                            EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                            EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                            EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            QRCode = Utility.ConvertToQRCode($"العميل : {x.PersonCustomer.Name},تاريخ العملية :{x.PaymentDate},المبلغ :{x.Amount}"),
                            //Logo = ' + localStorage.getItem("logo")+'
                        }).FirstOrDefault();
                        return Json(payment, JsonRequestBehavior.AllowGet);
                    }
                }

                if (typ == "suppPaymentCash")
                {
                    var paymentCash = db.SupplierPayments.Where(x => !x.IsDeleted && x.Id == Id);
                    if (paymentCash.Count() > 0)
                    {
                        payment = paymentCash.ToList().Select(x => new PrintPaymentDto
                        {
                            Id = x.Id,
                            BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                            PaymentDate = x.PaymentDate.Value.ToString("yyyy-MM-dd"),
                            SafeBankName = x.Safe != null ? x.Safe.Name : string.Empty,
                            SuppCustomerName = x.PersonSupplier.Name,
                            Amount = x.Amount,
                            Notes = x.Notes,
                            PersonDebit = GetPersonBalance(x.PersonSupplier != null ? x.SupplierId : null, false).Data,
                            //بيانات الجهة
                            EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                            EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                            EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            QRCode = Utility.ConvertToQRCode($"المورد : {x.PersonSupplier.Name},تاريخ العملية :{x.PaymentDate},المبلغ :{x.Amount}"),
                            //Logo = ' + localStorage.getItem("logo")+'
                        }).FirstOrDefault();
                        return Json(payment, JsonRequestBehavior.AllowGet);

                    }
                }
                if (typ == "voucherPayment")
                {
                    var voucherPayment = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id);
                    if (voucherPayment.Count() > 0)
                    {
                        payment = voucherPayment.ToList().Select(x => new PrintPaymentDto
                        {
                            Id = x.Id,
                            BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                            PaymentDate = x.VoucherDate.Value.ToString("yyyy-MM-dd"),
                            SafeBankName = x.AccountsTree.AccountName,
                            SuppCustomerName = x.VoucherDetails.FirstOrDefault().AccountsTree?.AccountName,
                            Amount = x.VoucherDetails.Where(d=>!d.IsDeleted).DefaultIfEmpty().Sum(s=>s.Amount),
                            Notes = x.Notes,
                            //بيانات الجهة
                            EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                            EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                            EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            QRCode = Utility.ConvertToQRCode($"من حساب : {x.VoucherDetails.FirstOrDefault().AccountsTree?.AccountName},تاريخ العملية :{x.VoucherDate},المبلغ :{x.VoucherDetails.Where(d => !d.IsDeleted).DefaultIfEmpty().Sum(s => s.Amount)}"),
                            //Logo = ' + localStorage.getItem("logo")+'
                        }).FirstOrDefault();
                        return Json(payment, JsonRequestBehavior.AllowGet);

                    }
                }
                if (typ == "voucherReceipt")
                {
                    var voucherPayment = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id);
                    if (voucherPayment.Count() > 0)
                    {
                        payment = voucherPayment.ToList().Select(x => new PrintPaymentDto
                        {
                            Id = x.Id,
                            BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                            PaymentDate = x.VoucherDate.Value.ToString("yyyy-MM-dd"),
                            SafeBankName = x.AccountsTree?.AccountName,
                            SuppCustomerName = x.VoucherDetails.FirstOrDefault().AccountsTree?.AccountName,
                            Amount = x.VoucherDetails.Where(d => !d.IsDeleted).DefaultIfEmpty().Sum(d=> d.Amount),
                            Notes = x.Notes,
                            //بيانات الجهة
                            EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                            EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                            EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            QRCode = Utility.ConvertToQRCode($"الى حساب : {x.VoucherDetails.FirstOrDefault().AccountsTree?.AccountName},تاريخ العملية :{x.VoucherDate},المبلغ :{x.VoucherDetails.Where(d => !d.IsDeleted).DefaultIfEmpty().Sum(d => d.Amount)}"),
                            //Logo = ' + localStorage.getItem("logo")+'
                        }).FirstOrDefault();
                        return Json(payment, JsonRequestBehavior.AllowGet);

                    }
                }

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }
        //طباعه التحصيلات الاقساط
        public JsonResult PrintInstallmentSchedule(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                // الحصول على بيانات المؤسسة من الاعدادات
                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();

                var schedule = db.InstallmentSchedules.Where(x => !x.IsDeleted && x.Id == Id).FirstOrDefault();
                if (schedule != null)
                {
                    return Json(new
                    {
                        Id = schedule.Id,
                        BranchName = schedule.Safe?.Branch.Name,
                        PaymentDate = schedule.PaymentDate?.ToString("yyyy-MM-dd"),
                        CustomerName = schedule.Installment?.SellInvoice != null ? schedule.Installment.SellInvoice.PersonCustomer.Name : schedule.Installment.CustomerIntialBalance.Person.Name,
                        InstallmentDate = schedule.InstallmentDate?.ToString("yyyy-MM-dd"),
                        Amount = schedule.Amount,
                        RemindSchedules = schedule.Installment.InstallmentSchedules.Where(x => !x.IsDeleted && !x.IsPayed && x.Id != schedule.Id).Count(),
                        CustomerDebit = GetPersonBalance(schedule.Installment?.SellInvoice != null ? schedule.Installment.SellInvoice.CustomerId : schedule.Installment.CustomerIntialBalance.PersonId, true).Data,
                        //بيانات الجهة
                        EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                        EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                        EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region مديونية عميل او مورد
        public JsonResult GetPersonBalance(Guid? id, bool isCustomer)
        {
            if (id == null)
                return Json("0", JsonRequestBehavior.AllowGet);
            Guid? accountId;
            if (isCustomer)
                accountId = db.Persons.Where(x => x.Id == id).FirstOrDefault().AccountsTreeCustomerId; //عميل
            else
                accountId = db.Persons.Where(x => x.Id == id).FirstOrDefault().AccountTreeSupplierId;//مورد

            CustomerService customerService = new CustomerService();
            var balance = customerService.GetPersonBalance(accountId);
            return Json(balance, JsonRequestBehavior.AllowGet);
        }     
        //سياسة اسعار عميل محدد فى فاتورة بيع 
        public JsonResult GetItemPriceByCustomer(Guid? id,Guid? itemId,bool isCustomer)//customer id
        {
            double?  customeSell=0;
            Guid? pricingPolicyId=null;
            if (id == null||id==Guid.Empty|| itemId == null|| itemId == Guid.Empty)
                return Json(new { pricingPolicyId= pricingPolicyId, customeSell= customeSell??0 }, JsonRequestBehavior.AllowGet);
            var itemPrices = db.ItemPrices.Where(x => x.ItemId == itemId);
            if (isCustomer)
                itemPrices = itemPrices.Where(x => x.CustomerId == id);
            else
                itemPrices = itemPrices.Where(x => x.SupplierId == id);
            if (itemPrices.Count()>0)
            {
                var data=itemPrices.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                pricingPolicyId = data.PricingPolicyId;
                customeSell = data.SellPrice;
            }
            return Json(new { pricingPolicyId = pricingPolicyId, customeSell = customeSell ?? 0 }, JsonRequestBehavior.AllowGet);
        }

        //تحديد مدة الاستحقاق تلقائيا فى حالة تحديدها مسبقا 
        public JsonResult GetContractCustomer(Guid? id, bool isCustomer,DateTime? invoDate)
        {
            if (id == null||invoDate==null)
                return Json(0, JsonRequestBehavior.AllowGet);
            IQueryable<ContractCustomerSupplier> contract;
            if (isCustomer)
                contract = db.ContractCustomerSuppliers.Where(x =>!x.IsDeleted&& x.CustomerId == id); //عميل
            else
                contract = db.ContractCustomerSuppliers.Where(x => !x.IsDeleted && x.SupplierId == id); //عميل
            contract = contract.Where(x => invoDate >= DbFunctions.TruncateTime(x.FromDate) && invoDate <= DbFunctions.TruncateTime(x.ToDate));
            var data = contract.OrderByDescending(x=>x.CreatedOn).FirstOrDefault();
            return Json(data!=null?invoDate.Value.AddDays(data.DayCount).ToString("yyyy-MM-dd") :invoDate.Value.AddMonths(1).ToString("yyyy-MM-dd"), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region رصيد حساب
        public JsonResult GetAccountBalance(string id, bool isSafe)
        {
            if (isSafe)
            {
                if (Guid.TryParse(id, out Guid safeId))
                {
                    var safe = db.Safes.Where(x => x.Id == safeId).FirstOrDefault();
                    if (safe != null)
                    {
                        var balance = GeneralDailyService.GetAccountBalance(safe.AccountsTreeId);
                        return Json(balance, JsonRequestBehavior.AllowGet);

                    }
                    else
                        return Json("0", JsonRequestBehavior.AllowGet);

                }
                else
                    return Json("0", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (Guid.TryParse(id, out Guid accountId))
                {
                    var balance = GeneralDailyService.GetAccountBalance(accountId);
                    return Json(balance, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("0", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region فواتير البيع 
        //ارقام فواتير البيع بدلالة العميل 
        public JsonResult GetSellInvoiceIds(Guid? customerId)
        {
            if (customerId!=null)
            {
                var list = db.SellInvoices.Where(x => !x.IsDeleted && x.CustomerId == customerId&&(x.PaymentTypeId==(int)PaymentTypeCl.Deferred||x.PaymentTypeId==(int)PaymentTypeCl.Partial)).ToList().Select(x => new { Id = x.Id, Name = $"فاتورة رقم : {x.InvoiceNumber} | بتاريخ {x.InvoiceDate.ToString("yyyy-MM-dd")}" });
                var selectList = new SelectList(list, "Id", "Name");
                return Json(selectList.Items, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        } 
        //قيمة فاتورة البيع بدلالة رقمها  
        public JsonResult GetInvoiceAmount(Guid? sellInvoiceId)
        {
            if (sellInvoiceId != null)
            {
                var amount = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == sellInvoiceId ).FirstOrDefault().Safy;
                var totalInvoiceAmount = db.SellInvoicePayments.Where(x => !x.IsDeleted && x.SellInvoiceId == sellInvoiceId).DefaultIfEmpty().Sum(y => (double?)y.Amount ?? 0);
                var remindAmount = amount - totalInvoiceAmount;
                return Json(new {invoiceAmount=amount, totalInvoiceAmount= totalInvoiceAmount, remindAmount= remindAmount }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(0, JsonRequestBehavior.AllowGet);
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