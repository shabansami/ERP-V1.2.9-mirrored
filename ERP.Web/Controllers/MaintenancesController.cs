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
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.ViewModels;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class MaintenancesController : Controller
    {
        // GET: Maintenances
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DS { get; set; }

        #region ادارة فواتير الصيانه
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            bool isAdmin = false;
            if (auth.CookieValues.EmployeeId == null)
            {
                isAdmin = auth.CookieValues.IsAdmin;
            }
            int? n = null;
            return Json(new
            {
                data = db.Maintenances.Where(x => !x.IsDeleted && (x.EmployeeResponseId == auth.CookieValues.EmployeeId || isAdmin)).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, IsApprovalAccountant = x.IsApprovalAccountant, IsFinalApproval = x.IsFinalApproval, InvoiceGuid = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.Person.Name, Safy = x.Safy, InvoType = x.BySaleMen ? "مندوب" : "بدون مناديب", CaseName = x.MaintenanceCas != null ? x.MaintenanceCas.Name : "", Actions = n, Num = n }).ToList()
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

        public ActionResult AddItemDetails(ItemDetailsDT vm)
        {
            List<ItemDetailsDT> deDS = new List<ItemDetailsDT>();
            string itemName = "";
            string storeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(vm.DT_Datasource);
            if (vm.ItemId != null)
            {
                if (deDS.Where(x => x.ItemId == vm.ItemId && vm.ProductionOrderId == x.ProductionOrderId && vm.IsIntial == x.IsIntial && vm.SerialItemId == x.SerialItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف او السيريال موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x => x.Id == vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);

            if (vm.SerialItemId != null && vm.SerialItemId != Guid.Empty)
            {
                if (vm.Quantity > 1)
                    return Json(new { isValid = false, msg = "الكمية المدخلة اكبر من 1 " }, JsonRequestBehavior.AllowGet);
            }
            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = vm.ItemDiscount, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح " }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region تسجيل فاتورة صيانة 
        [HttpGet]
        public ActionResult CreateEdit()
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.IncomeTypes.Where(x => !x.IsDeleted), "Id", "Name");
            // add
            DS = JsonConvert.SerializeObject(new List<ItemDetailsDT>());
            //التاكد من تحديد مخزن الصيانة من الاعدادات اولا 
            var storeMaintenanceSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreMaintenance).FirstOrDefault();
            Guid? maintenanceStoreId = null;
            Guid? branchId = null;
            Store maintenanceStore = null;
            if (!string.IsNullOrEmpty(storeMaintenanceSetting.SValue))
            {
                maintenanceStoreId = Guid.Parse(storeMaintenanceSetting.SValue);
                maintenanceStore = db.Stores.Where(x => x.Id == maintenanceStoreId).FirstOrDefault();
                branchId = maintenanceStore.BranchId;
            }
            else
                ViewBag.Msg = "تأكد من اختيار مخزن الصيانة من شاشة الاعدادات العامة";
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", branchId);
            ViewBag.Branchcount = branches.Count();
            ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchId && !x.IsDamages), "Id", "Name", maintenanceStoreId);
            ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)), "Id", "Name");
            Random random = new Random();
            //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();
            ViewBag.DepartmentSaleMenId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeSaleMenId = new SelectList(new List<Employee>(), "Id", "Name");
            ViewBag.DepartmentResponseId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.EmployeeResponseId = new SelectList(new List<Employee>(), "Id", "Name");

            var vm = new MaintenanceVM();
            vm.InvoiceDate = Utility.GetDateTime();
            vm.DeliveryDate = Utility.GetDateTime().AddDays(3);
            return View(vm);
            //}
        }
        [HttpPost]
        public JsonResult CreateEdit(MaintenanceVM vm, string DT_DatasourceItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.StoreId == null || vm.EmployeeResponseId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    //اذا ادخل ررقم فاتورة بيه التأكد من ان الفاتورة موجودة بالفعل
                    if (!string.IsNullOrWhiteSpace(vm.SellInvoiceNumber))
                    {
                        var invoice = db.SellInvoices.FirstOrDefault(x => x.InvoiceNumber == vm.SellInvoiceNumber && !x.IsDeleted);
                        if (invoice == null)
                        {
                            return Json(new { isValid = false, message = "لا يوجد فاتورة البيع هذا الرقم!" });
                        }
                        else
                            vm.SellInvoiceId = invoice.Id;
                    }

                    //الاصناف
                    List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                    List<MaintenanceDetail> items = new List<MaintenanceDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });


                    var isInsert = false;
                    Maintenance model = JsonConvert.DeserializeObject<Maintenance>(JsonConvert.SerializeObject(vm));
                    model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                    model.DeliveryDate = vm.DeliveryDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                    isInsert = true;
                    model.InvoiceNumber = Properties.Settings.Default.CodePrefix + (db.Maintenances.Count(x => !x.IsDeleted && x.InvoiceNumber.Contains(Properties.Settings.Default.CodePrefix)) + 1);
                    model.MaintenanceCaseId = (int)MaintenanceCaseCl.Pending;
                    foreach (var item in itemDetailsDT)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            items.Add(new MaintenanceDetail
                            {
                                MaintenanceId = vm.Id,
                                ItemId = item.ItemId,
                                ItemSerialId = item.SerialItemId,
                                MaintenanceCaseId = (int)MaintenanceCaseCl.Pending,
                                RecordGuid = Guid.NewGuid()                                                                                                       
                            });


                        }

                    }
                    model.MaintenanceDetails = items;
                    //فى حالة ان الصيانة من خلال مندوب
                    if (vm.BySaleMen)
                    {
                        if (vm.EmployeeSaleMenId == null)
                            return Json(new { isValid = false, message = "تأكد من اختيار المندوب اولا" });
                        else
                            model.EmployeeSaleMenId = vm.EmployeeSaleMenId;
                    }
                    else
                        model.EmployeeSaleMenId = null;


                    //اضافة حالة الطلب 
                    db.MaintenanceCaseHistories.Add(new MaintenanceCaseHistory
                    {
                        Maintenance = model,
                        MaintenanceCaseId = (int)MaintenanceCaseCl.Pending
                    });
                    db.Maintenances.Add(model);



                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        if (isInsert)
                        {
                            foreach (var item in itemDetailsDT)
                            {
                                //add new row in history serial item
                                if (item.SerialItemId != null && item.SerialItemId != Guid.Empty)
                                {
                                    //update item serial case history
                                    var serial = db.ItemSerials.FirstOrDefault(x => x.Id == item.SerialItemId);
                                    if (serial != null)
                                    {
                                        serial.SerialCaseId = (int)SerialCaseCl.Maintenance;
                                        db.Entry(serial).State = EntityState.Modified;
                                        //add item serial case history
                                        db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                        {
                                            ItemSerialId = serial.Id,
                                            ReferrenceId = model.Id,
                                            SerialCaseId = (int)SerialCaseCl.Maintenance
                                        });
                                        db.SaveChanges(auth.CookieValues.UserId);
                                    }
                                }
                            }
                            return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });

                        }
                        else
                            return Json(new { isValid = true, isInsert, message = "تم التعديل بنجاح" });

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
                var model = db.Maintenances.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.MaintenanceDetails.Where(x => x.MaintenanceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                        //حذف اى قطع غيار
                        foreach (var item in detail.MaintenanceSpareParts.Where(x => !x.IsDeleted))
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //حذف اى ايرادات
                        foreach (var item in detail.MaintenanceIncomes.Where(x => !x.IsDeleted))
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        //حذف اى توالف
                        foreach (var item in detail.MaintenanceDamages.Where(x => !x.IsDeleted))
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                            //حذف اى اصناف توالف
                            foreach (var item2 in item.MaintenanceDamageDetails.Where(x => !x.IsDeleted))
                            {
                                item2.IsDeleted = true;
                                db.Entry(item2).State = EntityState.Modified;
                            }
                        }
                    }


                    // حذف كل حالات الاصناف فى جدول itemSerial history ان وجدت 
                    var maintenanceHistories = db.MaintenanceCaseHistories.Where(x => x.MaintenanceId == model.Id).ToList();
                    foreach (var itemHis in maintenanceHistories)
                    {
                        itemHis.IsDeleted = true;
                        db.Entry(itemHis).State = EntityState.Modified;
                    }

                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.Maintenance).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
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
        public ActionResult ShowMaintenance(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.Maintenances.Where(x => x.Id == invoGuid && x.MaintenanceDetails.Any(y => !y.IsDeleted)).OrderBy(x=>x.CreatedOn).FirstOrDefault();
            vm.MaintenanceDetails = vm.MaintenanceDetails.Where(x => !x.IsDeleted).OrderBy(x => x.CreatedOn).ToList();
            return View(vm);
        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.Maintenances.FirstOrDefault(x => x.Id == invoGuid);
            vm.MaintenanceCaseHistories = vm.MaintenanceCaseHistories.Where(x => !x.IsDeleted).ToList();
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