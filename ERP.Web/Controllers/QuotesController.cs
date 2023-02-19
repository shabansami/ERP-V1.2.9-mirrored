using ERP.DAL.Utilites;
using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Services;
using ERP.Web.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Identity;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class QuotesController : Controller
    {
        // GET: Quotes
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService itemService;
        StoreService storeService;
        public QuotesController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            itemService = new ItemService();

        }
        public static string DS { get; set; }

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

            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted&&x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            return View();
        }
        public ActionResult GetAll(string dFrom, string dTo,Guid? customerId)
        {
            int? n = null;
            DateTime dtFrom, dtTo;
            var list = db.QuoteOrderSells.Where(x => !x.IsDeleted && x.QuoteOrderSellType == (int)QuoteOrderSellTypeCl.Qoute);
            if (DateTime.TryParse(dFrom, out dtFrom) && DateTime.TryParse(dTo, out dtTo))
                list = list.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dtFrom.Date && DbFunctions.TruncateTime(x.InvoiceDate) <= dtTo.Date);
            if(customerId != null)
                list=list.Where(x => x.CustomerId == customerId);

                var items = list.OrderByDescending(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNumber = x.InvoiceNumber, CustomerName = x.Customer.Name, InvoiceDate = x.InvoiceDate.ToString(), Safy = x.Safy,OrderSellExist=x.QuoteOrderSellsChildren.Where(t=>!t.IsDeleted).Any(), Actions = n, Num = n }).ToList();
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
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId ).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.Price <= 0)
            {
                //سعر البيع يقبل صفر (الهدايا
                var sellPriceZeroSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.SellPriceZero).FirstOrDefault().SValue;
                if (sellPriceZeroSett == "0")//رفض البيع بصفر 
                    return Json(new { isValid = false, msg = "تأكد من ادخال سعر البيع بشكل صحيح" }, JsonRequestBehavior.AllowGet);
            }

            if (vm.Quantity <= 0)
                return Json(new { isValid = false, msg = "تأكد من ادخال رقم صحيح للكمية " }, JsonRequestBehavior.AllowGet);

            //فى حالة البيع بوحدة 
            vm.QuantityUnitName = "0";
            if (vm.ItemUnitsId != null && vm.ItemUnitsId != Guid.Empty)
            {
                var itemUnit = db.ItemUnits.Where(x => x.Id == vm.ItemUnitsId).FirstOrDefault();
                if (itemUnit != null && itemUnit.Quantity > 0)
                {
                    vm.UnitId = itemUnit.UnitId;
                    vm.QuantityUnit = vm.Quantity;
                    vm.QuantityUnitName = $"{vm.Quantity} {itemUnit.Unit?.Name}";
                    vm.Quantity = vm.Quantity * itemUnit.Quantity;
                    vm.Price = Math.Round(itemUnit.SellPrice / itemUnit.Quantity, 2, MidpointRounding.ToEven);
                }
            }
            //التأكد من السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته
            var acceptItemCostSellDownSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AcceptItemCostSellDown).FirstOrDefault().SValue;
            if (acceptItemCostSellDownSett == "0")//منع البيع بسعر بيع اقل من التكلفة 
            {
                int.TryParse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue, out int itemCostCalculateId);
                var itemCost = itemService.GetItemCostCalculation(itemCostCalculateId, vm.ItemId ?? Guid.Empty);
                if (itemCost > vm.Price)
                    return Json(new { isValid = false, msg = "سعر بيع الصنف اقل من تكلفته" }, JsonRequestBehavior.AllowGet);
            }

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemUnitsId = vm.ItemUnitsId, ItemName = itemName, Quantity = vm.Quantity, QuantityUnitName = vm.QuantityUnitName, QuantityUnit=vm.QuantityUnit, Price = vm.Price, UnitId = vm.UnitId, Amount = vm.Quantity * vm.Price };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), totalQuantity = deDS.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpGet]
        public ActionResult CreateEdit()
        {
            DS = null;
            QuoteOrderSell vm = new QuoteOrderSell();
            Guid? customerId = null;
            Guid? branchId = null;
            //ViewBag.ExpenseTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemUnitsId = new SelectList(new List<ItemUnit>(), "Id", "Name");

            if (TempData["model"] != null) //edit
            {
                Guid id;
                if (Guid.TryParse(TempData["model"].ToString(), out id))
                {
                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == id);
                    customerId=model?.CustomerId;
                    branchId=model?.BranchId;
                    var itemDetails = model.QuoteOrderSellDetails.Where(x=>!x.IsDeleted).Select(x=> new ItemDetailsDT 
                    { ItemId = x.ItemId,
                        ItemName =x.Item.Name,
                        Quantity = x.Quantity, 
                        Price = x.Price,
                        UnitId = x.UnitId,
                        QuantityUnit = x.QuantityUnit,
                        Amount = x.Quantity * x.Price
                    }).ToList();
                    DS = JsonConvert.SerializeObject(itemDetails);
                    vm = model;
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                var defaultStore = storeService.GetDefaultStore(db);
                 branchId = defaultStore != null ? defaultStore.BranchId : null;
                vm.InvoiceDate = Utility.GetDateTime();
                //ضريبة القيمة المضافة
                var taxSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.TaxPercentage).FirstOrDefault();
                if (double.TryParse(taxSetting?.SValue, out double TaxSetting))
                    vm.SalesTaxPercentage = TaxSetting;
                //ضريبة ارباح تجارية
                var taxProfitSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.TaxProfitPercentage).FirstOrDefault();
                if (double.TryParse(taxProfitSetting?.SValue, out double TaxProfitSetting))
                    vm.ProfitTaxPercentage = TaxProfitSetting;


            }
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && x.IsActive && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name",customerId);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");

            return View(vm);
        }
        [ValidateInput((false))]
        [HttpPost]
        public JsonResult CreateEdit(QuoteOrderSell vm, string DT_Datasource, bool? isInvoiceDisVal,string editorNotes)
        {
            if (ModelState.IsValid)
            {
                DateTime invoiceDate;
                if (vm.InvoiceDate == null || vm.CustomerId == null || vm.BranchId == null)
                    return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                //فى حالة الخصم على الفاتورة نسية /قيمة 
                if (!bool.TryParse(isInvoiceDisVal.ToString(), out var t))
                    return Json(new { isValid = false, message = "تأكد من اختيار احتساب الخصم على الفاتورة" });

                invoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                //الاصناف
                List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                List<QuoteOrderSellDetail> items = new List<QuoteOrderSellDetail>();

                if (DT_Datasource != null)
                {
                    itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_Datasource);
                    if (itemDetailsDT.Count() == 0)
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    else
                        items = itemDetailsDT.Select(x => new QuoteOrderSellDetail { 
                            ItemId = x.ItemId,
                            Amount = x.Amount, 
                            Quantity = x.Quantity,
                            Price = x.Price ,
                            QuantityUnit=x.QuantityUnit,
                            UnitId=x.UnitId}).ToList();
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                var isInsert = false;
                if (vm.Id != Guid.Empty)
                {
                    //حذف اى اصناف تم تسجيلها مسباق 

                    var model = db.QuoteOrderSells.FirstOrDefault(x => x.Id == vm.Id);
                    var previousItems = model.QuoteOrderSellDetails.Where(x => !x.IsDeleted).ToList();
                    foreach (var item in previousItems)
                    {
                        item.IsDeleted = true;
                    }

                    model.CustomerId = vm.CustomerId;
                    model.InvoiceDate = invoiceDate;
                    model.BranchId = vm.BranchId;
                    model.Notes = editorNotes;
                    double invoiceDiscount = 0;
                    if (isInvoiceDisVal == true)
                    {
                        invoiceDiscount = vm.InvoiceDiscount;
                        model.DiscountPercentage = 0;
                    }
                    else if (isInvoiceDisVal == false)
                    {
                        invoiceDiscount = (itemDetailsDT.Sum(x => x.Amount) * vm.DiscountPercentage) / 100;
                        model.DiscountPercentage = vm.DiscountPercentage;
                    }
                    model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                    model.InvoiceDiscount = invoiceDiscount;//إجمالي خصومات الفاتورة 
                    model.TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0);
                    model.TotalValue = items.Sum(x => (double?)x.Amount ?? 0);
                    model.Safy = (model.TotalValue + vm.SalesTax) - (model.InvoiceDiscount + vm.ProfitTax);
                    model.ProfitTaxPercentage = vm.ProfitTaxPercentage;
                    model.ProfitTax = vm.ProfitTax;
                    model.SalesTaxPercentage=vm.SalesTaxPercentage; 
                    model.SalesTax = vm.SalesTax;
                    model.QuoteOrderSellDetails = items;
                    //db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    isInsert = true;
                    //اضافة رقم الفاتورة
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    vm.InvoiceNumber = codePrefix + (db.QuoteOrderSells.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);
                    vm.InvoiceDate = invoiceDate;
                    double invoiceDiscount = 0;
                    if (isInvoiceDisVal == true)
                    {
                        invoiceDiscount = vm.InvoiceDiscount;
                        vm.DiscountPercentage = 0;
                    }
                    else if (isInvoiceDisVal == false)
                        invoiceDiscount = (itemDetailsDT.Sum(x => x.Amount) * vm.DiscountPercentage) / 100;
                    
                    vm.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                    vm.InvoiceDiscount = invoiceDiscount;//إجمالي خصومات الفاتورة 
                    vm.TotalQuantity = items.Sum(x => (double?)x.Quantity ?? 0);
                    vm.TotalValue = items.Sum(x => (double?)x.Amount ?? 0);
                    vm.Safy = (vm.TotalValue + vm.SalesTax) - (vm.InvoiceDiscount + vm.ProfitTax);
                    vm.Notes = editorNotes;
                    vm.QuoteOrderSellType = (int)QuoteOrderSellTypeCl.Qoute;
                    vm.QuoteOrderSellDetails = items;
                    db.QuoteOrderSells.Add(vm);

                }
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    DS = null;
                    if (isInsert)
                        return Json(new { isValid = true, refGid = vm.Id, isInsert, message = "تم الاضافة بنجاح" });
                    else
                        return Json(new { isValid = true, refGid = vm.Id, isInsert, message = "تم التعديل بنجاح" });

                }
                else
                    return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        [Authorization]
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
                var model = db.QuoteOrderSells.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;
                    //حذف الاصناف
                    var items =model.QuoteOrderSellDetails.Where(x => !x.IsDeleted ).ToList();
                    foreach (var item in items)
                    {
                        item.IsDeleted = true;
                        //db.Entry(item).State = EntityState.Modified;
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