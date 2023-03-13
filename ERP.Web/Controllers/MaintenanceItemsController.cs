using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class MaintenanceItemsController : Controller
    {
        // GET: MaintenanceItems
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DS_SpareParts { get; set; } //قطع الغيار
        public static string DS_Damages { get; set; } //التوالف
        public static string DS_Incomes { get; set; } //ايرادات الصيانة

        #region All Item
        public ActionResult Index(string invoGuid)
        {
            Guid invGuid;
            if (Guid.TryParse(invoGuid, out invGuid))
            {
                var vm = db.Maintenances.Where(x => x.Id == invGuid).FirstOrDefault();
                if (vm != null)
                {
                    ViewBag.MaintenanceCaseId = new SelectList(db.MaintenanceCases.Where(x => !x.IsDeleted && !x.ForAdmin), "Id", "Name", vm.MaintenanceCaseId);
                    ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);

                    return View(vm);
                }
                else
                    return RedirectToAction("Index", "Maintenances");
            }
            else
                return RedirectToAction("Index", "Maintenances");
        }

        [HttpPost] //تحديث الحالة للفاتورة 
        public ActionResult UpdateMaintenanceCase(string invoGuid, string maintCaseId)
        {
            Guid Id;
            int maintenanceCaseId;
            if (Guid.TryParse(invoGuid, out Id) && int.TryParse(maintCaseId, out maintenanceCaseId))
            {
                var model = db.Maintenances.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.MaintenanceCaseId = maintenanceCaseId;
                    db.Entry(model).State = EntityState.Modified;
                    //اضافة حالة الطلب 
                    db.MaintenanceCaseHistories.Add(new MaintenanceCaseHistory
                    {
                        Maintenance = model,
                        MaintenanceCaseId = maintenanceCaseId
                    });
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم تحديث حالة الفاتورة بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
        }
        [HttpPost] //تحديث طريقة سداد الفاتورة  
        public ActionResult UpdateMaintenancePayment(string invoGuid, string paymentTypeId, string safy, string payedValue, string remindValue, string invoiceDiscount, string discountPercentage, bool? isInvoiceDisVal)
        {
            Guid Id;
            int paymentTypId;
            double paydValue;
            double safyy;
            double remindValuee;
            if (Guid.TryParse(invoGuid, out Id) && int.TryParse(paymentTypeId, out paymentTypId) && double.TryParse(payedValue, out paydValue) && double.TryParse(remindValue, out remindValuee) && double.TryParse(safy, out safyy))
            {
                var model = db.Maintenances.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //فى حالة الخصم على الفاتورة نسية /قيمة 
                    if (!bool.TryParse(isInvoiceDisVal.ToString(), out var t))
                        return Json(new { isValid = false, message = "تأكد من اختيار احتساب الخصم على الفاتورة" });

                    //فى حالة الخصم على الفاتورة نسية /قيمة 
                    double invoDiscount = 0;
                    if (double.TryParse(invoiceDiscount, out var tt))
                        invoDiscount = double.Parse(invoiceDiscount);
                    if (isInvoiceDisVal == true)
                        model.DiscountPercentage = 0;
                    else if (isInvoiceDisVal == false)
                    {
                        if (double.TryParse(discountPercentage, out var ttt))
                            model.DiscountPercentage = double.Parse(discountPercentage);
                    }

                    //التأكد من ان المبلغ المدفوع يساوى او اقل من صافى الفاتورة 
                    if (paydValue > safyy)
                        return Json(new { isValid = false, message = "المبلغ المدفوع اكبر من صافى الفاتورة" });

                    //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                    if (paymentTypId == (int)PaymentTypeCl.Cash && paydValue != safyy)
                        return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                    //فى حالة السداد أجل وتم ادخال مبلغ مدفوع 
                    if (paymentTypId == (int)PaymentTypeCl.Deferred && paydValue > 0)
                        return Json(new { isValid = false, message = "تم استلام مبلغ من العميل وحالة السداد آجل " });

                    model.Safy = safyy;
                    model.PayedValue = paydValue;
                    model.RemindValue = remindValuee;
                    model.PaymentTypeId = paymentTypId;
                    model.InvoiceDiscount = invoDiscount;
                    var totalCurrentItemDiscount = model.MaintenanceDetails.Where(x => !x.IsDeleted).Sum(x => x.TotalItemDiscount);
                    model.TotalDiscount = totalCurrentItemDiscount + invoDiscount;
                    db.Entry(model).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم تحديث طريقة سداد الفاتورة بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد وادخال المبلغ المدفوع" });


        }

        #endregion

        #region Maintenance Item
        public ActionResult SaveItemData(string rcdGuid)
        {
            Guid recordGuid;
            if (Guid.TryParse(rcdGuid, out recordGuid))
            {
                var vm = db.MaintenanceDetails.Include(x => x.Maintenance).Where(x => x.RecordGuid == recordGuid).FirstOrDefault();
                if (vm != null)
                {
                    //التاكد من تحديد مخزن التوالف من الاعدادات اولا 
                    var storeDamageSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreMaintenanceDamage).FirstOrDefault();
                    Guid? damageStoreId = null;
                    Store damageStore = null;
                    if (!string.IsNullOrEmpty(storeDamageSetting.SValue))
                    {
                        damageStoreId = Guid.Parse(storeDamageSetting.SValue);
                        damageStore = db.Stores.Where(x => x.Id == damageStoreId).FirstOrDefault();
                    }
                    else
                    {
                        ViewBag.Msg = "تأكد من اختيار مخزن الصيانة من شاشة الاعدادات العامة";
                        return RedirectToAction("Index", "GeneralSettings");
                    }


                    ViewBag.MaintenProblemTypeId = new SelectList(db.MaintenProblemTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.MaintenProblemTypeId);
                    ViewBag.MaintenanceCaseId = new SelectList(db.MaintenanceCases.Where(x => !x.IsDeleted && !x.ForAdmin), "Id", "Name", vm.MaintenanceCaseId);

                    ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
                    ViewBag.ItemDamageId = new SelectList(new List<Item>(), "Id", "Name");
                    ViewBag.PricingPolicyId = new SelectList(db.PricingPolicies.Where(x => !x.IsDeleted), "Id", "Name");
                    //مخازن المستخدم
                    var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                    var branchId = branches.FirstOrDefault()?.Id;
                    var stores = EmployeeService.GetStoresByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());

                    ViewBag.StoreId = new SelectList(stores, "Id", "Name");


                    //قطع الغيار
                    var itemSpareParts = db.MaintenanceSpareParts.Where(x => !x.IsDeleted && x.MaintenanceDetailId == vm.Id).Select(item => new
                                  ItemDetailsDT
                    {
                        Amount = item.Amount,
                        ItemId = item.ItemId,
                        ItemDiscount = item.SparePartDiscount,
                        ItemName = item.Item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        StoreId = item.StoreId,
                        StoreName = item.Store.Name,
                        IsIntial = item.IsItemIntial ? 1 : 0,
                        ProductionOrderId = item.ProductionOrderId,
                        SerialItemId = item.ItemSerialId
                    }).ToList();
                    DS_SpareParts = JsonConvert.SerializeObject(itemSpareParts);
                    //التوالف
                    var damages = db.MaintenanceDamages.Where(x => !x.IsDeleted && x.MaintenanceDetailId == vm.Id);
                    List<ItemDetailsDT> itemDamages = new List<ItemDetailsDT>();
                    if (damages.Count() > 0)
                    {
                        itemDamages = damages.FirstOrDefault().MaintenanceDamageDetails.Where(x => !x.IsDeleted).Select(item => new
                                           ItemDetailsDT
                        {
                            Amount = item.Amount,
                            ItemId = item.ItemId,
                            ItemName = item.Item.Name,
                            Quantity = item.Quantity,
                            //StoreId = item.StoreId,
                            //StoreName = item.Store.Name,
                        }).ToList();
                    }

                    DS_Damages = JsonConvert.SerializeObject(itemDamages);

                    //ايراد الصيانة
                    var incomes = db.MaintenanceIncomes.Where(x => !x.IsDeleted && x.MaintenanceDetailId == vm.Id).OrderBy(x => x.CreatedOn).Select(income => new
                                 InvoiceExpensesDT
                    {
                        ExpenseAmount = income.Amount,
                        ExpenseTypeName = income.Name
                    }).ToList();
                    DS_Incomes = JsonConvert.SerializeObject(incomes);

                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult SaveItemData(MaintenanceDetail vm, string DT_DatasourceItemSpareParts, string DT_DatasourceItemDamages, string DT_DatasourceItemIncomes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.MaintenProblemTypeId == null || vm.MaintenanceCaseId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });


                    //قطع الغيار 
                    List<ItemDetailsDT> itemSparePartsDT = new List<ItemDetailsDT>();
                    List<MaintenanceSparePart> itemSpareParts = new List<MaintenanceSparePart>();
                    if (DT_DatasourceItemSpareParts != null)
                    {
                        itemSparePartsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItemSpareParts);
                        itemSpareParts = itemSparePartsDT.Select(x =>
                                  new MaintenanceSparePart
                                  {
                                      MaintenanceDetailId = vm.Id,
                                      ItemId = x.ItemId,
                                      StoreId = x.StoreId,
                                      Quantity = x.Quantity,
                                      Price = x.Price,
                                      Amount = x.Amount,
                                      ItemSerialId = x.SerialItemId,
                                      SparePartDiscount = x.ItemDiscount,
                                      ProductionOrderId = x.ProductionOrderId,
                                      IsItemIntial = x.IsIntial == 1 ? true : false
                                  }).ToList();

                    }
                    //التوالف 
                    List<ItemDetailsDT> itemDamagesDT = new List<ItemDetailsDT>();
                    List<MaintenanceDamageDetail> itemDamages = new List<MaintenanceDamageDetail>();
                    Guid? damageStoreId = null;
                    if (DT_DatasourceItemDamages != null)
                    {
                        //التاكد من تحديد مخزن التوالف من الاعدادات اولا 
                        var storeDamageSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreMaintenanceDamage).FirstOrDefault();
                        if (!string.IsNullOrEmpty(storeDamageSetting.SValue))
                            damageStoreId = Guid.Parse(storeDamageSetting.SValue);
                        else
                            return Json(new { isValid = false, message = "تأكد من اختيار مخزن الصيانة من شاشة الاعدادات العامة" });

                        itemDamagesDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItemDamages);
                        itemDamages = itemDamagesDT.Select(x =>
                                  new MaintenanceDamageDetail
                                  {
                                      ItemId = x.ItemId,
                                      Quantity = x.Quantity,
                                      Amount = x.Amount,
                                  }).ToList();

                    }



                    //ايرادات الصيانه لصنف محدد
                    List<InvoiceExpensesDT> invoiceIncomesDT = new List<InvoiceExpensesDT>();
                    List<MaintenanceIncome> invoicesIncomes = new List<MaintenanceIncome>();
                    if (DT_DatasourceItemIncomes != null)
                    {
                        invoiceIncomesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceItemIncomes);
                        invoicesIncomes = invoiceIncomesDT.Select(
                                                  x => new MaintenanceIncome
                                                  {
                                                      MaintenanceDetailId = vm.Id,
                                                      Name = x.ExpenseTypeName,
                                                      Amount = x.ExpenseAmount
                                                  }
                                                  ).ToList();
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال تكلفة الصيانة" });

                    MaintenanceDetail model = null;
                    model = db.MaintenanceDetails.FirstOrDefault(x => x.Id == vm.Id);


                    model.TotalItemDiscount = itemSparePartsDT.Sum(x => x.ItemDiscount);//إجمالي خصومات قطع الغيار 
                    model.TotalItemSpareParts = itemSparePartsDT.Sum(x => x.Amount);//إجمالي قيمة قطع الغيار  
                    model.TotalItemIncomes = invoiceIncomesDT.Sum(x => x.ExpenseAmount);//إجمالي قيمة الايرادادت
                    model.ItemSafy = Math.Round((model.TotalItemSpareParts + model.TotalItemIncomes) - (model.TotalItemDiscount), 2); // المبلغ النهائى

                    //delete all privous SpareParts 
                    var privousSpareParts = db.MaintenanceSpareParts.Where(x => x.MaintenanceDetailId == model.Id).ToList();
                    foreach (var item in privousSpareParts)
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
                    //delete all privous Damages 
                    var damage = model.MaintenanceDamages.Where(x => !x.IsDeleted).FirstOrDefault();
                    if (damage != null)
                    {
                        var privousItemDamages = db.MaintenanceDamageDetails.Where(x => x.MaintenanceDamageId == damage.Id).ToList();
                        foreach (var item in privousItemDamages)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        damage.IsDeleted = true;
                        db.Entry(damage).State = EntityState.Modified;
                    }

                    //delete all privous Incomes
                    var privousIncomes = db.MaintenanceIncomes.Where(x => x.MaintenanceDetailId == model.Id).ToList();
                    foreach (var income in privousIncomes)
                    {
                        income.IsDeleted = true;
                        db.Entry(income).State = EntityState.Modified;
                    }


                    // Update maintenance item invoice
                    model.MaintenProblemTypeId = vm.MaintenProblemTypeId;
                    model.MaintenanceCaseId = vm.MaintenanceCaseId;
                    model.Note = vm.Note;

                    db.Entry(model).State = EntityState.Modified;
                    db.MaintenanceSpareParts.AddRange(itemSpareParts);
                    db.MaintenanceIncomes.AddRange(invoicesIncomes);
                    if (itemDamages.Count() > 0)
                    {
                        var damageNew = new MaintenanceDamage
                        {
                            MaintenanceDetailId = model.Id,
                            StoreId = damageStoreId,
                            MaintenanceDamageDetails = itemDamages
                        };
                        db.MaintenanceDamages.Add(damageNew);
                    }

                    // تحديث صافى المبلغ المطلوب تحصيلة للفاتورة ككل وخصومات الاصناف  
                    var maintenance = model.Maintenance;
                    var maintenanceDetails = maintenance.MaintenanceDetails.Where(x => !x.IsDeleted);
                    var totalCurrentItemSafy = maintenanceDetails.Sum(x => x.ItemSafy);
                    var totalCurrentItemDiscount = maintenanceDetails.Sum(x => x.TotalItemDiscount);
                    maintenance.Safy = totalCurrentItemSafy;
                    maintenance.TotalDiscount = totalCurrentItemDiscount + maintenance.InvoiceDiscount;
                    maintenance.TotalSpareParts = maintenanceDetails.Sum(x => x.TotalItemSpareParts);
                    maintenance.TotalIncomes = maintenanceDetails.Sum(x => x.TotalItemIncomes);
                    //ارجاع المبلغ المدفوع والمتبقى للصفر بسبب تغيير قيمة المبلغ النهائى للصنف
                    maintenance.RemindValue = 0;
                    maintenance.PayedValue = 0;

                    db.Entry(maintenance).State = EntityState.Modified;

                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        return Json(new { isValid = true, invoGuid = model.Maintenance.Id, message = "تم التحديث بنجاح" });

                    }
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });


            }
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }

        #endregion

        #region wizard step 2 اضافة قطع غيار للصنف
        public ActionResult GetDSItemSpareParts()
        {
            int? n = null;
            if (DS_SpareParts == null)
                return Json(new
                {
                    data = new ItemDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS_SpareParts)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemSpareParts(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (!bool.TryParse(vm.IsDiscountItemVal.ToString(), out var tt))
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);

            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && vm.ProductionOrderId == x.ProductionOrderId && vm.IsIntial == x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.SerialItemId != null && vm.SerialItemId != Guid.Empty)
            {
                if (vm.Quantity > 1)
                    return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من 1 " }, JsonRequestBehavior.AllowGet);
            }

            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x => x.Id == vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            //احتساب طريقة الخصم على الصنف (قيمة/نسبة)
            double itemDiscount = 0;
            if (vm.IsDiscountItemVal == true)
                itemDiscount = vm.ItemDiscount;
            else if (vm.IsDiscountItemVal == false)
                itemDiscount = ((vm.Price * vm.Quantity) * vm.ItemDiscount) / 100;
            else
                return Json(new { isValid = false, msg = "تاكد من اختيار طريقة احتساب الخصم" }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = itemDiscount, IsDiscountItemVal = vm.IsDiscountItemVal, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS_SpareParts = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount), itemDiscount = itemDiscount }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3 اضافة توالف الصنف
        public ActionResult GetDSItemDamages()
        {
            int? n = null;
            if (DS_Damages == null)
                return Json(new
                {
                    data = new ItemDetailsDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DS_Damages)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDamages(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);

            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            //if (vm.StoreId != null)
            //    storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreId).Name;
            //else
            //    return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            var PriceVal = 1;
            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Amount = vm.Quantity * PriceVal };
            deDS.Add(newItemDetails);
            DS_Damages = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 4 تسجيل ايرادات الصيانة
        public ActionResult GetDStMaintenanceIncomes()
        {
            int? n = null;
            if (DS_Incomes == null)
                return Json(new
                {
                    data = new InvoiceExpensesDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DS_Incomes)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddMaintenanceIncomes(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);

            var newIncome = new InvoiceExpensesDT { ExpenseTypeName = vm.ExpenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newIncome);
            DS_Incomes = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل الإيراد بنجاح ", totalItemIncomes = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
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