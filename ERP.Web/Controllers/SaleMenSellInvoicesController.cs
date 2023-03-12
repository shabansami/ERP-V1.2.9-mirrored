using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class SaleMenSellInvoicesController : Controller
    {
        // GET: SaleMenSellInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService itemService;
        public static string DS { get; set; }
        public SaleMenSellInvoicesController()
        {
            db = new VTSaleEntities();
            itemService = new ItemService();
        }
        #region ادارة فواتير التوريد
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellInvoices.Where(x => !x.IsDeleted && x.BySaleMen && x.EmployeeId == auth.CookieValues.EmployeeId).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region wizard step 2 اضافة اصناف الفاتورة
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new ItemDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult AddItemDetails(ItemDetailsDT vm)
        //{
        //    List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
        //    string itemName = "";
        //    string storeName = "";
        //    if (vm.DT_Datasource != null)
        //        deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
        //    if (!bool.TryParse(vm.IsDiscountItemVal.ToString(), out var tt))
        //        return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);
        //    var item = db.Items.Where(x => x.Id == vm.ItemId).FirstOrDefault();
        //    if (item != null)
        //    {
        //        if (deDS.Where(x => x.ItemId == vm.ItemId && vm.ProductionOrderId == x.ProductionOrderId && vm.IsIntial == x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
        //            return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
        //        itemName = item.Name;
        //    }
        //    else
        //        return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
        //    if (vm.Price <= 0)
        //    {
        //        //سعر البيع يقبل صفر (الهدايا
        //        var sellPriceZeroSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.SellPriceZero).FirstOrDefault().SValue;
        //        if (sellPriceZeroSett == "0")//رفض البيع بصفر 
        //            return Json(new { isValid = false, msg = "تأكد من ادخال سعر البيع بشكل صحيح" }, JsonRequestBehavior.AllowGet);
        //    }

        //    if (vm.SerialItemId != null && vm.SerialItemId != Guid.Empty)
        //    {
        //        if (vm.Quantity > 1)
        //            return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من 1 " }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (vm.StoreId != null)
        //        storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
        //    else
        //        return Json(new { isValid = false, msg = "تأكد من اختيار مخزن للمندوب " }, JsonRequestBehavior.AllowGet);
        //    //احتساب طريقة الخصم على الصنف (قيمة/نسبة)
        //    double itemDiscount = 0;
        //    if (vm.IsDiscountItemVal == true)
        //        itemDiscount = vm.ItemDiscount;
        //    else if (vm.IsDiscountItemVal == false)
        //        itemDiscount = ((vm.Price * vm.Quantity) * vm.ItemDiscount) / 100;
        //    else
        //        return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);
        //    //التأكد من السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته
        //    var acceptItemCostSellDownSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AcceptItemCostSellDown).FirstOrDefault().SValue;
        //    if (acceptItemCostSellDownSett == "0")//منع البيع بسعر بيع اقل من التكلفة 
        //    {
        //        int.TryParse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue, out int itemCostCalculateId);
        //        var itemCost = itemService.GetItemCostCalculation(itemCostCalculateId, vm.ItemId ?? Guid.Empty);
        //        if (itemCost > vm.Price)
        //            return Json(new { isValid = false, msg = "سعر بيع الصنف اقل من تكلفته" }, JsonRequestBehavior.AllowGet);
        //    }
        //    //التأكد من وقوع سعر البيع ضمن ماتم تحديده عند اضافة الصنف (اعلى / اقل سعر بيع 
        //    if (item.MinPrice != null && item.MinPrice > 0)
        //    {
        //        if (vm.Price < item.MinPrice)
        //            return Json(new { isValid = false, msg = "سعر البيع اقل من السعر الادنى المحدد للصنف" }, JsonRequestBehavior.AllowGet);
        //    }
        //    if (item.MaxPrice != null && item.MaxPrice > 0)
        //    {
        //        if (vm.Price > item.MaxPrice)
        //            return Json(new { isValid = false, msg = "سعر البيع اكبر من السعر الاعلى المحدد للصنف" }, JsonRequestBehavior.AllowGet);
        //    }
        //    //كمية الصنف لاتكفى بسبب الرصيد الغير معتمد 
        //            //الكمية اكبر من الرصيد المتوفر 
        //            if (vm.Quantity > vm.CurrentBalanceVal)
        //                return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من الرصيد المتاح" }, JsonRequestBehavior.AllowGet);
        //            //الرصيد لايكفى بعد احتساب الرصيد المحجوز (الغير معتمد)
        //            var finalBalance = vm.CurrentBalanceVal - BalanceService.GetBalanceNotApproval(vm.ItemId, vm.StoreId);
        //            if (vm.Quantity > finalBalance)
        //                return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من الرصيد المتاح والرصيد المحجوز" }, JsonRequestBehavior.AllowGet);



        //    var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, CurrentBalanceVal = vm.CurrentBalanceVal, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = itemDiscount, IsDiscountItemVal = vm.IsDiscountItemVal, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
        //    deDS.Add(newItemDetails);
        //    DS = JsonConvert.SerializeObject(deDS);
        //    return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), itemDiscount = itemDiscount }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (!bool.TryParse(vm.IsDiscountItemVal.ToString(), out var tt))
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);

            var item = db.Items.Where(x => x.Id == vm.ItemId).FirstOrDefault();
            if (item != null)
            {
                //if (deDS.Where(x => x.ItemId == vm.ItemId&&x.Price==vm.Price&&vm.ProductionOrderId==x.ProductionOrderId&&vm.IsIntial==x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
                //    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = item.Name;
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
            if (vm.SerialItemId != null && vm.SerialItemId != Guid.Empty)
            {
                if (vm.Quantity > 1)
                    return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من 1 " }, JsonRequestBehavior.AllowGet);
            }
            if (vm.Quantity <= 0)
                return Json(new { isValid = false, msg = "تأكد من ادخال رقم صحيح للكمية " }, JsonRequestBehavior.AllowGet);

            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            vm.QuantityUnitName = "0";
            //فى حالة البيع بوحدة 
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
            //احتساب طريقة الخصم على الصنف (قيمة/نسبة)
            double itemDiscount = 0;
            if (vm.IsDiscountItemVal == true)
                itemDiscount = vm.ItemDiscount;
            else if (vm.IsDiscountItemVal == false)
                itemDiscount = ((vm.Price * vm.Quantity) * vm.ItemDiscount) / 100;
            else
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);

            //التأكد من السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته
            var acceptItemCostSellDownSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AcceptItemCostSellDown).FirstOrDefault().SValue;
            if (acceptItemCostSellDownSett == "0")//منع البيع بسعر بيع اقل من التكلفة 
            {
                int.TryParse(db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue, out int itemCostCalculateId);
                var itemCost = itemService.GetItemCostCalculation(itemCostCalculateId, vm.ItemId ?? Guid.Empty);
                if (itemCost > vm.Price)
                    return Json(new { isValid = false, msg = "سعر بيع الصنف اقل من تكلفته" }, JsonRequestBehavior.AllowGet);
            }
            //التأكد من وقوع سعر البيع ضمن ماتم تحديده عند اضافة الصنف (اعلى / اقل سعر بيع 
            if (item.MinPrice != null && item.MinPrice > 0)
            {
                if (vm.Price < item.MinPrice)
                    return Json(new { isValid = false, msg = "سعر البيع اقل من السعر الادنى المحدد للصنف" }, JsonRequestBehavior.AllowGet);
            }
            if (item.MaxPrice != null && item.MaxPrice > 0)
            {
                if (vm.Price > item.MaxPrice)
                    return Json(new { isValid = false, msg = "سعر البيع اكبر من السعر الاعلى المحدد للصنف" }, JsonRequestBehavior.AllowGet);
            }
            //كمية الصنف لاتكفى بسبب الرصيد الغير معتمد 
            //قبول اضافة صنف بدون رصيد
            int itemAcceptNoBalance = 0;
            var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
            if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
            {
                if (itemAcceptNoBalance == 0)
                {
                    //الكمية اكبر من الرصيد المتوفر 
                    if (vm.Quantity > vm.CurrentBalanceVal)
                        return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من الرصيد المتاح" }, JsonRequestBehavior.AllowGet);
                    //الرصيد لايكفى بعد احتساب الرصيد المحجوز (الغير معتمد)
                    var finalBalance = vm.CurrentBalanceVal - BalanceService.GetBalanceNotApproval(vm.ItemId, vm.StoreId);
                    if (vm.Quantity > finalBalance)
                        return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من الرصيد المتاح والرصيد المحجوز" }, JsonRequestBehavior.AllowGet);
                }

            }
            else
                return Json(new { isValid = false, msg = "تأكد من تحديد قبول اصناف بدون رصيد من الاعدادات" }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemUnitsId = vm.ItemUnitsId, CurrentBalanceVal = vm.CurrentBalanceVal, ItemName = itemName, Quantity = vm.Quantity, QuantityUnitName = vm.QuantityUnitName, QuantityUnit = vm.QuantityUnit, UnitId = vm.UnitId, Price = vm.Price, Amount = Math.Round(vm.Quantity * vm.Price, 2, MidpointRounding.ToEven), ItemDiscount = itemDiscount, IsDiscountItemVal = vm.IsDiscountItemVal, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), itemDiscount = itemDiscount, totalQuantity = deDS.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region تسجيل فاتورة توريد وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.ItemUnitsId = new SelectList(new List<ItemUnit>(), "Id", "Name");

            //مخازن المستخدم
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            var branchId = branches.FirstOrDefault()?.Id;
            var stores = EmployeeService.GetStoresByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());
            //خزن المستخدم
            var safes = EmployeeService.GetSafesByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());

            if (stores == null||stores.Count()==0) //فى حالة ان الموظف غير محدد له مخزن اى انه ليس مندوب
            {
                ViewBag.ErrorMsg = "لابد من تحديد مخزن للمندوب اولا لعرض هذه الشاشة";
                return View(new SaleMenSellInvoiceVM());
            }
            ViewBag.StoreId = new SelectList(stores, "Id", "Name");
            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    var vm = db.SellInvoices.Where(x => x.Id == guId).Select
                        (x => new SaleMenSellInvoiceVM
                        {
                            Id = x.Id,
                            BranchId = x.BranchId,
                            BySaleMen = x.BySaleMen,
                            SafeId=x.SafeId,
                            CustomerId = x.CustomerId,
                            DiscountPercentage = x.DiscountPercentage,
                            DueDate = x.DueDate,
                            SaleMenEmployeeId = x.EmployeeId,
                            InvoiceDate = x.InvoiceDate,
                            InvoiceDiscount = x.InvoiceDiscount,
                            Notes = x.Notes,
                            PayedValue = x.PayedValue,
                            PaymentTypeId = x.PaymentTypeId,
                            ProfitTax = x.ProfitTax,
                            RemindValue = x.RemindValue,
                            Safy = x.Safy,
                            SalesTax = x.SalesTax,
                            TotalDiscount = x.TotalDiscount,
                            TotalQuantity = x.TotalQuantity,
                            TotalValue = x.TotalValue,
                            PersonCategoryId = x.PersonCustomer.PersonCategoryId,
                            TotalItemDiscount = x.SellInvoicesDetails.Where(y => !y.IsDeleted).Select(y => y.ItemDiscount).Sum()
                        })
                        .FirstOrDefault();

                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).Select(item => new
                                  ItemDetailsDT
                    {
                        Amount = item.Amount,
                        ItemId = item.ItemId,
                        ItemDiscount = item.ItemDiscount,
                        ItemName = item.Item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId,
                        StoreName = item.Store.Name,
                        IsIntial = item.IsItemIntial ? 1 : 0,
                        ProductionOrderId = item.ProductionOrderId,
                        SerialItemId = item.ItemSerialId
                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);


                    //ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", vm.BranchId);
                    ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
                    ViewBag.SafeId = new SelectList(safes, "Id", "Name",vm.SafeId);

                    //int? departmentId = null;
                    //int? empId = null;
                    //if (vm.BySaleMen && vm.Employee != null)
                    //{
                    //    departmentId = vm.Employee.DepartmentId;
                    //    empId = vm.EmployeeId;
                    //    var list = EmployeeService.GetSaleMens(departmentId);
                    //    ViewBag.EmployeeId = new SelectList(list, "Id", "Name", empId);

                    //}
                    //else
                    //    ViewBag.EmployeeId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.DepartmentId == departmentId).Select(x => new { Id = x.Id, Name = x.Person.Name }), "Id", "Name", empId);


                    //ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name", departmentId);
                    ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name", vm.PersonCategoryId);
                    ViewBag.CustomerId = new SelectList(EmployeeService.GetCustomerByCategory(vm.PersonCategoryId, auth.CookieValues.EmployeeId,vm.CustomerId), "Id", "Name", vm.CustomerId);

                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
            {                   // add
                DS = JsonConvert.SerializeObject(new List<ItemDetailsDT>());

                //ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == 1), "Id", "Name", 1);
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.SafeId = new SelectList(safes, "Id", "Name");
                Random random = new Random();

                //ViewBag.DepartmentId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                //ViewBag.EmployeeId = new SelectList(new List<Employee>(), "Id", "Name");

                var vm = new SaleMenSellInvoiceVM();
                vm.InvoiceDate = Utility.GetDateTime();
                //ضريبة القيمة المضافة
                var taxSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.TaxPercentage).FirstOrDefault();
                if (double.TryParse(taxSetting?.SValue, out double TaxSetting))
                    vm.SalesTaxPercentage = TaxSetting;
                //ضريبة ارباح تجارية
                var taxProfitSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.TaxProfitPercentage).FirstOrDefault();
                if (double.TryParse(taxProfitSetting?.SValue, out double TaxProfitSetting))
                    vm.ProfitTaxPercentage = TaxProfitSetting;

                
                vm.BranchId = branchId;
                vm.SaleMenEmployeeId = auth.CookieValues.EmployeeId;
                //vm.SaleMenStoreId = auth.CookieValues.StoreId;
                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SaleMenSellInvoiceVM vm, string DT_DatasourceItems, bool? isInvoiceDisVal)
        {
            vm.BySaleMen = true;
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    //فى حالة الخصم على الفاتورة نسية /قيمة 
                    if (!bool.TryParse(isInvoiceDisVal.ToString(), out var t))
                        return Json(new { isValid = false, message = "تأكد من اختيار احتساب الخصم على الفاتورة" });


                    //الاصناف
                    List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                    List<SellInvoicesDetail> items = new List<SellInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new SellInvoicesDetail
                              {
                                  SellInvoiceId = vm.Id,
                                  ItemId = x.ItemId,
                                  StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  Price = x.Price,
                                  Amount = x.Amount,
                                  ItemDiscount = x.ItemDiscount,
                                  ItemSerialId = x.SerialItemId,
                                  ProductionOrderId = x.ProductionOrderId,
                                  IsItemIntial = x.IsIntial == 1 ? true : false
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                    //التأكد من اختيار نوع الدفع نقدى او جزئى فى حالة تحديد خزنة
                    if (!(vm.PaymentTypeId==(int)PaymentTypeCl.Cash||vm.PaymentTypeId==(int)PaymentTypeCl.Partial)&&vm.SafeId!=null)
                        return Json(new { isValid = false, message = "تأكد من اختيار نوع الدفع نقدى او جزئى" });


                    var isInsert = false;
                    SellInvoice model = null;
                    if (vm.Id != Guid.Empty)
                    {
                        model = db.SellInvoices.FirstOrDefault(x => x.Id == vm.Id);
                        if (model.InvoiceDate.ToShortDateString() != vm.InvoiceDate.ToShortDateString())
                            model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                    }
                    else
                    {
                        model = new SellInvoice()
                        {
                            Id = vm.Id,
                            SafeId=vm.SafeId,
                            BranchId = vm.BranchId,
                            CustomerId = vm.CustomerId,
                            BySaleMen = true,
                            DiscountPercentage = vm.DiscountPercentage,
                            DueDate = vm.DueDate,
                            EmployeeId = vm.SaleMenEmployeeId,
                            InvoiceDate = vm.InvoiceDate,
                            InvoiceDiscount = vm.InvoiceDiscount,
                            Notes = vm.Notes,
                            PayedValue = vm.PayedValue,
                            PaymentTypeId = vm.PaymentTypeId,
                            ProfitTax = vm.ProfitTax,
                            RemindValue = vm.RemindValue,
                            Safy = vm.Safy,
                            SalesTax = vm.SalesTax,
                            ProfitTaxPercentage = vm.ProfitTaxPercentage,
                            SalesTaxPercentage = vm.SalesTaxPercentage,
                            TotalDiscount = vm.TotalDiscount,
                            TotalQuantity = vm.TotalQuantity,
                            TotalValue = vm.TotalValue
                        };
                        model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                    }

                    //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                    //فى حالة الخصم على الفاتورة نسية /قيمة 
                    double invoiceDiscount = 0;
                    if (isInvoiceDisVal == true)
                        invoiceDiscount = vm.InvoiceDiscount;
                    else if (isInvoiceDisVal == false)
                    {
                        //edit
                        invoiceDiscount = (itemDetailsDT.Sum(x => x.Amount) * vm.DiscountPercentage) / 100;
                        model.DiscountPercentage = vm.DiscountPercentage;
                    }

                    model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                    model.TotalDiscount = itemDetailsDT.Sum(x => x.ItemDiscount) + invoiceDiscount;//إجمالي خصومات الفاتورة 
                    model.TotalValue = itemDetailsDT.Sum(x => x.Amount);//إجمالي قيمة المبيعات 
                    model.Safy = (model.TotalValue + vm.SalesTax) - (model.TotalDiscount + vm.ProfitTax);

                    //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                    if (vm.PaymentTypeId == (int)PaymentTypeCl.Cash && vm.PayedValue != model.Safy)
                        return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                    //التأكد من السماح البيع للعميل فى حالة تخطى رصيده حد الائتمانى الخطر المسموح به اولا حسب الاعدادات
                    int limitDangerSell = 0;
                    var limitDangerSellSett = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.LimitDangerSell).FirstOrDefault();
                    if (int.TryParse(limitDangerSellSett.SValue, out limitDangerSell))
                    {
                        if (limitDangerSell == 0)//لايمكن البيع للعميل فى حالة تخطى رصيده حد الائتمانى الخطر المسموح به 
                        {
                            var customer = db.Persons.Where(x => x.Id == vm.CustomerId).FirstOrDefault();
                            if (customer != null)
                            {
                                CustomerService customerService = new CustomerService();
                                var balance = customerService.GetPersonBalance(customer.AccountsTreeCustomerId);
                                if (customer.LimitDangerSell != 0)
                                    if (balance + vm.Safy - vm.PayedValue > customer.LimitDangerSell)
                                        return Json(new { isValid = false, message = "لا يمكن البيع لتخطى الحد الائتمانى للعميل " });
                            }
                            else
                                return Json(new { isValid = false, message = "تأكد من اختيار العميل" });
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من تحديد حالة السماح بالبيع عند تخطى الحد الائتمانى للخطر فى شاشة الاعدادات" });

                    if (vm.Id != Guid.Empty)
                    {
                        //delete all privous items 
                        var privousItems = db.SellInvoicesDetails.Where(x => x.SellInvoiceId == vm.Id).ToList();
                        foreach (var item in privousItems)
                        {
                            //تحديث حالة السيريال ان وجد بارتباطه بمخزن البيع
                            if (item.ItemSerial != null)
                            {
                                var serial = item.ItemSerial;
                                serial.SellInvoiceId = null;
                                serial.CurrentStoreId = item.StoreId;
                                db.Entry(serial).State = EntityState.Modified;
                            }
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }

                        // Update purchase invoice
                        model.CustomerId = vm.CustomerId;
                        model.BranchId = vm.BranchId;
                        model.PaymentTypeId = vm.PaymentTypeId;
                        model.SafeId=vm.SafeId;
                        model.PayedValue = vm.PayedValue;
                        model.RemindValue = vm.RemindValue;
                        model.SalesTax = vm.SalesTax;
                        model.ProfitTax = vm.ProfitTax;
                        model.InvoiceDiscount = invoiceDiscount;
                        //model.ShippingAddress = vm.ShippingAddress;
                        model.DueDate = vm.DueDate;
                        model.Notes = vm.Notes;
                        model.CaseId = (int)CasesCl.InvoiceSaleModified;
                        //فى حالة ان البيع من خلال مندوب
                        model.EmployeeId = vm.SaleMenEmployeeId;


                        db.Entry(model).State = EntityState.Modified;
                        db.SellInvoicesDetails.AddRange(items);
                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellInvoice = model,
                            IsSellInvoice = true,
                            CaseId = (int)CasesCl.InvoiceSaleModified
                        });

                    }
                    else
                    {

                        isInsert = true;
                        //اضافة رقم الفاتورة
                        string codePrefix = Properties.Settings.Default.CodePrefix;
                        model.InvoiceNumber = codePrefix + (db.SellInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                        model.CaseId = (int)CasesCl.InvoiceSaleMenCreated;
                        model.SellInvoicesDetails = items;


                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellInvoice = model,
                            IsSellInvoice = true,
                            CaseId = (int)CasesCl.InvoiceSaleMenCreated
                        });
                        db.SellInvoices.Add(model);
                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        if (isInsert)
                            return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
                        else
                            return Json(new { isValid = true, message = "تم التعديل بنجاح" });

                    }
                    else
                        return Json(new { isValid = false, isInsert, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });


            }
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        public ActionResult Edit(string invoGuid)
        {
            Guid GuId;

            if (!Guid.TryParse(invoGuid, out GuId) || string.IsNullOrEmpty(invoGuid) || invoGuid == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = GuId;
            return RedirectToAction("CreateEdit");

        }
        [HttpPost]
        public ActionResult Delete(string invoGuid)
        {
            Guid Id;
            if (Guid.TryParse(invoGuid, out Id))
            {
                var model = db.SellInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.InvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.SellInvoicesDetails.Where(x => x.SellInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        // حذف كل السيريالات الاصناف ان وجدت 
                        //var itemSerial = db.ItemSerials.Where(x => details.Any(y => y.ItemSerialId == x.Id)).ToList();
                        if (detail.ItemSerial != null)
                        {
                            var serial = detail.ItemSerial;

                            serial.IsDeleted = true;
                            db.Entry(serial).State = EntityState.Modified;
                            var itemSerialsHistories = db.CasesItemSerialHistories.Where(x => x.ItemSerialId == serial.Id).ToList();
                            foreach (var itemSerialHis in itemSerialsHistories)
                            {
                                itemSerialHis.IsDeleted = true;
                                db.Entry(itemSerialHis).State = EntityState.Modified;
                            }
                        }
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;

                    }

                    var expenses = db.SellInvoiceIncomes.Where(x => x.SellInvoiceId == model.Id).ToList();
                    //expenses.ForEach(x => x.IsDeleted = true);
                    foreach (var expense in expenses)
                    {
                        expense.IsDeleted = true;
                        db.Entry(expense).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    var casesSellInvoiceHistories = db.CasesSellInvoiceHistories.Where(x => x.SellInvoiceId == model.Id).ToList();
                    foreach (var cases in casesSellInvoiceHistories)
                    {
                        cases.IsDeleted = true;
                        db.Entry(cases).State = EntityState.Modified;
                    }
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
                    }

                    // حذف  اشعار الاستحقاق ان وجد 
                    var notifications = db.Notifications.Where(x => !x.IsDeleted && x.RefNumber == model.Id && x.NotificationTypeId == (int)NotificationTypeCl.SellInvoiceDueDateClient).ToList();
                    foreach (var notify in notifications)
                    {
                        notify.IsDeleted = true;
                        db.Entry(notify).State = EntityState.Modified;
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


        #region عرض بيانات فاتورة توريد بالتفصيل وحالاتها وطباعتها
        public ActionResult ShowSaleMenSellInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellInvoices.Where(x => x.Id == invoGuid && x.SellInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.SellInvoicesDetails = vm.SellInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            return View(vm);
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