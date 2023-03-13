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
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class StoresTransfersController : Controller
    {
        // GET: StoresTransfers
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        StoreService storeService;
        ItemService itemService;
        public StoresTransfersController()
        {
            db = new VTSaleEntities();
            storeService = new StoreService();
            itemService = new ItemService();
        }
        public static string DS { get; set; }

        #region ادارة التحويلات
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

            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.StoreFromId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.StoreToId = new SelectList(new List<Store>(), "Id", "Name");
            ViewBag.BranchFromId = new SelectList(branches, "Id", "Name");
            ViewBag.BranchToId = new SelectList(branches, "Id", "Name");

            return View();
        }

        public ActionResult GetAll(string storeFromId, string storeToId, string dtFrom, string dtTo, string cmbo_approvalStore, string cmbo_forSaleMen)
        {
            int? n = null;
            Guid storeFrmId;
            Guid storeTId;
            Guid.TryParse(storeFromId, out storeFrmId);
            Guid.TryParse(storeToId, out storeTId);

            DateTime dateFrom, dateTo;
            var storesTrans = db.StoresTransfers.Where(x => !x.IsDeleted);
            if (storeFrmId != Guid.Empty)
                storesTrans = storesTrans.Where(x => x.StoreFromId == storeFrmId);
            if (storeTId != Guid.Empty)
                storesTrans = storesTrans.Where(x => x.StoreToId == storeTId);
            if (DateTime.TryParse(dtFrom, out dateFrom) && DateTime.TryParse(dtTo, out dateTo))
                storesTrans = storesTrans.Where(x => DbFunctions.TruncateTime(x.TransferDate) >= dateFrom && DbFunctions.TruncateTime(x.TransferDate) <= dateTo);
            else if (DateTime.TryParse(dtFrom, out dateFrom))
                storesTrans = storesTrans.Where(x => DbFunctions.TruncateTime(x.TransferDate) >= dateFrom);
            else if (DateTime.TryParse(dtTo, out dateTo))
                storesTrans = storesTrans.Where(x => DbFunctions.TruncateTime(x.TransferDate) <= dateTo);
            if (!string.IsNullOrEmpty(cmbo_approvalStore))
            {
                if (cmbo_approvalStore == "1") //حالة التحويل فى  الانتظار 
                    storesTrans = storesTrans.Where(x => !x.IsApprovalStore && !x.IsRefusStore);
                else if (cmbo_approvalStore == "2")//حالة التحويل تم الاعتماد المخزنى 
                    storesTrans = storesTrans.Where(x => x.IsApprovalStore);
                else if (cmbo_approvalStore == "3")//حالة التحويل تم الرفض المخزنى 
                    storesTrans = storesTrans.Where(x => x.IsRefusStore);
            }
            if (!string.IsNullOrEmpty(cmbo_forSaleMen))//التحويلات من والى مناديب
            {
                if (cmbo_forSaleMen == "1") //مندوب 
                    storesTrans = storesTrans.Where(x => x.EmployeeFromId != null || x.EmployeeToId != null);
                else if (cmbo_forSaleMen == "2")//لغير المناديب 
                    storesTrans = storesTrans.Where(x => x.EmployeeFromId == null && x.EmployeeToId == null);
            }
            var data = storesTrans.OrderBy(x => x.CreatedOn)
                .Select(x => new
                {
                    Id = x.Id,
                    StoreTransferNumber = x.StoreTransferNumber,
                    ApprovalStore = (!x.IsApprovalStore && !x.IsRefusStore) ? "فى الانتظار" : x.IsApprovalStore ? "تم الاعتماد" : "تم الرفض",
                    CaseName = x.StoresTransferCase.Name,
                    IsFinalApproval = x.IsFinalApproval,
                    IsRefusStore = x.IsRefusStore,
                    BranchFromName = x.StoreFrom.Branch.Name,
                    StoreFromName = x.StoreFrom.Name,
                    BranchToName = x.StoreTo.Branch.Name,
                    StoreToName = x.StoreTo.Name,
                    TransferDate = x.TransferDate.ToString(),
                    EmployeeFromName = x.EmployeeFromId != null ? x.EmployeeFrom.Person.Name : null,
                    EmployeeToName = x.EmployeeToId != null ? x.EmployeeTo.Person.Name : null,
                    Actions = n,
                    Num = n
                }).ToList();
            //var data = db.StoresTransfers.Where(x => !x.IsDeleted || (DbFunctions.TruncateTime(x.TransferDate) >= dateFrom && DbFunctions.TruncateTime(x.TransferDate) <= dateTo) || x.StoreFromId == storeFrmId || x.StoreToId == storeTId).Select(x => new { Id = x.Id, BranchFromName = x.Store.Branch.Name, StoreFromName = x.Store.Name, BranchToName = x.Store1.Branch.Name, StoreToName = x.Store1.Name, TransferDate = x.TransferDate.ToString(), Actions = n }).ToList();
            //var t = data.ToList();
            if (data.Count() > 0)
            {
                return Json(new
                {
                    data = data.ToList()
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    data = new { }
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region wizard step 2 اضافة اصناف الفاتورة
        public ActionResult GetDSItemDetails()
        {
            int? n = null;
            if (DS == null)
                return Json(new
                {
                    data = new StoresTransferDetailsDto()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<StoresTransferDetailsDto>>(DS)
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddItemDetails(StoresTransferDetailsDto vm)
        {
            List<StoresTransferDetailsDto> deDS = new List<StoresTransferDetailsDto>();
            string itemName = "";

            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<StoresTransferDetailsDto>>(vm.DT_Datasource);
            //if (vm.ItemId != null)
            //{
            //    if (deDS.Where(x => x.ItemId == vm.ItemId).Count() > 0)
            //        return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
            //    itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            //}
            //else
            //    return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);




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

            var newItemDetails = new StoresTransferDetailsDto { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, ProductionOrderId = vm.ProductionOrderId, IsItemIntial = vm.IsItemIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalQuantity = deDS.Sum(x => x.Quantity) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل تحويل مخزنى
        [HttpGet]
        public ActionResult CreateEdit(string shwTab)
        {
            //تحميل كل الاصناف فى اول تحميل للصفحة 
            var itemList = db.Items.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, Name = x.ItemCode + " | " + x.Name }).ToList();
            ViewBag.ItemId = new SelectList(itemList, "Id", "Name");
            var vm = new StoresTransfer();
            //مخازن المستخدم
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            var branchId = branches.FirstOrDefault()?.Id;
            var stores = EmployeeService.GetStoresByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());

            if (shwTab != null && shwTab == "1")
                ViewBag.ShowTab = true;
            else
                ViewBag.ShowTab = false;

            if (TempData["model"] != null) //edit
            {
                Guid guId;
                if (Guid.TryParse(TempData["model"].ToString(), out guId))
                {
                    vm = db.StoresTransfers.Where(x => x.Id == guId).FirstOrDefault();

                    List<StoresTransferDetailsDto> itemDetailsDTs = new List<StoresTransferDetailsDto>();
                    var items = db.StoresTransferDetails.Where(x => !x.IsDeleted && x.StoresTransferId == vm.Id).Select(item => new
                                  StoresTransferDetailsDto
                    {

                        ItemId = item.ItemId,
                        ItemName = item.Item.Name,
                        Quantity = item.Quantity,
                        IsItemIntial = item.IsItemIntial,
                        ProductionOrderId = item.ProductionOrderId,
                        SerialItemId = item.ItemSerialId
                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);

                    ViewBag.StoreFromId = new SelectList(stores, "Id", "Name", vm.StoreFromId);
                    ViewBag.StoreToId = new SelectList(stores, "Id", "Name", vm.StoreToId);
                    ViewBag.BranchFromId = new SelectList(branches, "Id", "Name", vm.StoreFrom.BranchId);
                    ViewBag.BranchToId = new SelectList(branches, "Id", "Name", vm.StoreTo.BranchId);
                    //ViewBag.DepartmentFromId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                    //ViewBag.EmployeeFromId = new SelectList(new List<Employee>(), "Id", "Name");
                    //ViewBag.DepartmentToId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                    //ViewBag.EmployeeToId = new SelectList(new List<Employee>(), "Id", "Name");

                    //return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else// add
            {
                DS = JsonConvert.SerializeObject(new List<StoresTransferDetailsDto>());

                ViewBag.StoreFromId = new SelectList(stores, "Id", "Name", stores.FirstOrDefault()?.Id);
                ViewBag.StoreToId = new SelectList(stores, "Id", "Name", stores.FirstOrDefault()?.Id);
                ViewBag.BranchFromId = new SelectList(branches, "Id", "Name", branchId);
                ViewBag.BranchToId = new SelectList(branches, "Id", "Name", branchId);
                
                //ViewBag.DepartmentFromId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                //ViewBag.EmployeeFromId = new SelectList(new List<Employee>(), "Id", "Name");
                //ViewBag.DepartmentToId = new SelectList(db.Departments.Where(x => !x.IsDeleted), "Id", "Name");
                //ViewBag.EmployeeToId = new SelectList(new List<Employee>(), "Id", "Name");

                vm.TransferDate = Utility.GetDateTime();

            }
            //قبول اضافة صنف بدون رصيد
            int itemAcceptNoBalance = 0;
            var acceptNoBalance = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault();
            if (int.TryParse(acceptNoBalance.SValue, out itemAcceptNoBalance))
                ViewBag.ItemAcceptNoBalance = itemAcceptNoBalance;
            ViewBag.Branchcount = branches.Count();
            return View(vm);
        }
        [HttpPost]
        public JsonResult CreateEdit(StoresTransfer vm, string DT_DatasourceItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.StoreFromId == null || vm.StoreToId == null || vm.TransferDate == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });
                    if (vm.StoreFromId == vm.StoreToId)
                        return Json(new { isValid = false, message = "لا يمكن اختيار نفس المخزن فى التحويل" });

                    //الاصناف
                    List<StoresTransferDetailsDto> storesTransferDetailsDto = new List<StoresTransferDetailsDto>();
                    List<StoresTransferDetail> items = new List<StoresTransferDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        storesTransferDetailsDto = JsonConvert.DeserializeObject<List<StoresTransferDetailsDto>>(DT_DatasourceItems);
                        if (storesTransferDetailsDto.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = storesTransferDetailsDto.Select(x =>
                              new StoresTransferDetail
                              {
                                  StoresTransferId = vm.Id,
                                  ItemId = x.ItemId,
                                  Quantity = x.Quantity,
                                  ProductionOrderId = x.ProductionOrderId,
                                  IsItemIntial = x.IsItemIntial,
                                  ItemSerialId = x.SerialItemId,
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                    var isInsert = false;
                    using (var context = new VTSaleEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                //=======================
                                // لا يوجد حذف لعملية تحويل مخزنى بسبب احتمال اجراء عمليه بيع او مرتجع بناءا على التحويل
                                //==================
                                StoresTransfer model = new StoresTransfer();
                                var StoretransferapprovalAfterSave = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.StoreTransferApprovalAfterSave).FirstOrDefault().SValue;

                                if (vm.Id != Guid.Empty)
                                {
                                    model = context.StoresTransfers.FirstOrDefault(x => x.Id == vm.Id);
                                    if (model.TransferDate.ToShortDateString() != vm.TransferDate.ToShortDateString())
                                        model.TransferDate = vm.TransferDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                                    //delete all privous items 
                                    var privousItems = context.StoresTransferDetails.Where(x => !x.IsDeleted && x.StoresTransferId == vm.Id).ToList();
                                    foreach (var item in privousItems)
                                    {
                                        //تحديث حالة السيريال ان وجد بارتباطه بمخزن البيع
                                        if (item.ItemSerial != null)
                                        {
                                            var serial = item.ItemSerial;
                                            serial.CurrentStoreId = null;
                                            //serial.CurrentStoreId = item.StoreId;
                                            context.Entry(serial).State = EntityState.Modified;
                                        }

                                        item.IsDeleted = true;
                                        context.Entry(item).State = EntityState.Modified;

                                    }
                                    context.SaveChanges(auth.CookieValues.UserId);
                                }
                                else
                                {
                                    isInsert = true;
                                    model = vm;
                                    model.TransferDate = vm.TransferDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                                }
                                if (StoretransferapprovalAfterSave == "1")
                                {
                                    foreach (var item in items)
                                    {
                                        item.QuantityReal = item.Quantity;
                                    }
                                }
                                model.StoresTransferDetails = items;
                                model.Notes = vm.Notes;
                                model.StoreFromId = vm.StoreFromId;
                                model.StoreToId = vm.StoreToId;
                                Guid? empFromId = null;
                                Guid? empToId = null;

                                //التأكد من ان المخازن (من/الى) مرتبطة بمندوب او لا 
                                empFromId = EmployeeService.GetSaleMenByStore(model.StoreFromId);
                                empToId = EmployeeService.GetSaleMenByStore(model.StoreToId);
                                //if (empFromId != null || empToId != null)
                                //{
                                //    model.ForSaleMen = true;
                                //}
                                //model.ForSaleMen = vm.ForSaleMen;
                                model.EmployeeFromId = empFromId;
                                model.EmployeeToId = empToId;
                                if (model.Id != Guid.Empty)
                                {
                                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferUpdated;
                                    //اضافة الحالة 
                                    context.StoresTransferHistories.Add(new StoresTransferHistory
                                    {
                                        StoresTransfer = model,
                                        StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferUpdated
                                    });
                                    context.Entry(model).State = EntityState.Modified;

                                }
                                else
                                {
                                    //اضافة رقم العملية
                                    string codePrefix = Properties.Settings.Default.CodePrefix;
                                    model.StoreTransferNumber = codePrefix + (context.StoresTransfers.Count(x => x.StoreTransferNumber.StartsWith(codePrefix)) + 1);

                                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferCreated;
                                    //اضافة الحالة 
                                    context.StoresTransferHistories.Add(new StoresTransferHistory
                                    {
                                        StoresTransfer = model,
                                        StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferCreated
                                    });
                                    context.StoresTransfers.Add(model);
                                }
                                context.SaveChanges(auth.CookieValues.UserId);

                                //تحديث حالة السيريال بتحويل مخزنى
                                var serials = items.Where(x => x.ItemSerialId != null).Select(x => x.ItemSerialId).ToList();
                                if (serials.Count() > 0)
                                {
                                    foreach (var itemSerial in serials)
                                    {
                                        var serial = context.ItemSerials.Where(x => x.Id == itemSerial).FirstOrDefault();
                                        if (serial != null)
                                        {
                                            serial.SerialCaseId = (int)SerialCaseCl.StoreTransfer;
                                            serial.CurrentStoreId = model.StoreToId;
                                            context.Entry(serial).State = EntityState.Modified;
                                            context.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                            {
                                                ItemSerialId = itemSerial,
                                                ReferrenceId = model.Id,
                                                SerialCaseId = (int)SerialCaseCl.StoreTransfer
                                            });
                                            context.SaveChanges(auth.CookieValues.UserId);
                                        }
                                    }
                                }


                                //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
                                Stopwatch stopwatch = new Stopwatch();
                                Stopwatch stopwatch1 = new Stopwatch();
                                if (StoretransferapprovalAfterSave == "1")
                                {

                                    var casesStoretranHistory = new StoresTransferHistory
                                    {
                                        StoresTransfer = model,
                                    };

                                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferStoreApproval;
                                    model.IsApprovalStore = true;
                                    casesStoretranHistory.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferStoreApproval;
                                    context.StoresTransferHistories.Add(casesStoretranHistory);
                                    context.SaveChanges(auth.CookieValues.UserId);


                                    model.IsFinalApproval = true;
                                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferFinalApproval;

                                    //اضافة الحالة 
                                    context.StoresTransferHistories.Add(new StoresTransferHistory
                                    {
                                        StoresTransfer = model,
                                        StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferFinalApproval
                                    });
                                    context.SaveChanges(auth.CookieValues.UserId);

                                }
                                context.SaveChanges(auth.CookieValues.UserId);



                                transaction.Commit();
                                if (isInsert)
                                    return Json(new { isValid = true, isInsert, message = "تم التحويل المخزنى بنجاح" });
                                else
                                    return Json(new { isValid = true, isInsert, message = "تم تعديل التحويل المخزنى بنجاح" });


                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                            }
                        }
                    }

                }
                else
                    return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

            }
            catch (Exception ex)
            {
                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    ////هل الصنف يسمح بالسحب منه بالسالب
                    //foreach (var item in model.StoresTransferDetails.Where(x=>!x.IsDeleted))
                    //{
                    //    var result = itemService.IsAllowNoBalance(item.ItemId, item.StoresTransfer.StoreFromId, item.Quantity);
                    //    if (!result.IsValid)
                    //        return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });
                    //}

                    model.IsDeleted = true;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.StoresTransferDetails.Where(x => x.StoresTransferId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
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
        public ActionResult Edit(string id)
        {
            Guid Id;

            if (!Guid.TryParse(id, out Id) || string.IsNullOrEmpty(id) || id == "undefined")
                return RedirectToAction("Index");

            TempData["model"] = Id;
            return RedirectToAction("CreateEdit");

        }
        #endregion
        #region فك الاعتماد 
        [HttpPost]
        public ActionResult UnApproval(string id)
        {
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //هل الصنف يسمح بالسحب منه بالسالب
                    foreach (var item in model.StoresTransferDetails.Where(x => !x.IsDeleted))
                    {
                        var result = itemService.IsAllowNoBalance(item.ItemId, item.StoresTransfer.StoreFromId, item.Quantity);
                        if (!result.IsValid)
                            return Json(new { isValid = false, message = $"غير مسموع بفك الاعتماد لوجود صنف/اكثر رصيده بالسالب {result.ItemNotAllowed}" });
                    }
                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferUnApproval;
                    //اضافة الحالة 
                    db.StoresTransferHistories.Add(new StoresTransferHistory
                    {
                        StoresTransfer = model,
                        StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferUnApproval
                    });
                    model.IsFinalApproval = false;
                    model.IsApprovalStore = false;
                    model.IsRefusStore = false;
                    db.Entry(model).State = EntityState.Modified;


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

        #endregion

        #region عرض بيانات عملية تحويل مخزنى بالتفصيل وطباعتها
        public ActionResult ShowDetails(string id)
        {

            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                var vm = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (vm != null)
                {
                    vm.StoresTransferDetails = vm.StoresTransferDetails.Where(x => !x.IsDeleted).ToList();
                    return View(vm);
                }
                else
                    return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Index");

        }

        public ActionResult ShowHistory(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.StoresTransfers.Where(x => x.Id == invoGuid && x.StoresTransferHistories.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.StoresTransferHistories = vm.StoresTransferHistories.Where(x => !x.IsDeleted).ToList();
            return View(vm);
        }
        #endregion

        #region الاعتماد النهائى 
        public ActionResult GetFinalApproval()
        {
            int? n = null;
            return Json(new
            {
                data = db.StoresTransfers.Where(x => !x.IsDeleted && x.IsApprovalStore).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoType = x.EmployeeTo != null ? "مندوب" : "بدون مناديب", EmployeeName = x.EmployeeTo != null ? x.EmployeeTo.Person.Name : null, InvoiceNum = x.StoreTransferNumber, InvoiceDate = x.TransferDate.ToString(), CaseId = x.StoresTransferCaseId, CaseName = x.StoresTransferCase != null ? x.StoresTransferCase.Name : "", IsFinalApproval = x.IsFinalApproval, FinalApproval = x.IsFinalApproval ? "معتمده نهائيا" : "غير معتمده", Actions = n, Num = n }).ToList()
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ApprovalFinalInvoice()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ApprovalFinal(string invoGuid)
        {
            try
            {
                Guid Id;
                if (Guid.TryParse(invoGuid, out Id))
                {
                    var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                    if (model == null)
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

                    model.IsFinalApproval = true;
                    model.StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferFinalApproval;
                    db.Entry(model).State = EntityState.Modified;

                    //اضافة الحالة 
                    db.StoresTransferHistories.Add(new StoresTransferHistory
                    {
                        StoresTransfer = model,
                        StoresTransferCaseId = (int)StoresTransferCaseCl.StoreTransferFinalApproval
                    });
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                    else
                        return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            catch (Exception ex)
            {

                return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
        }
        #endregion
        #region اعتماد عملية تحويل مخزنى الطريقة القديمة
        [HttpPost]
        public ActionResult Approval(string id, string app) // store transfer id - type 1=>
        {
            Guid Id;
            int approve;
            if (Guid.TryParse(id, out Id) && int.TryParse(app, out approve))
            {
                var model = db.StoresTransfers.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    //model.SaleMenStatus = true;
                    //if (approve == 1) //type(1) 1=>SaleMenIsApproval=true تم الموافقة والاعتماد التحويل 
                    //    model.SaleMenIsApproval = true;
                    //else //type(1) 1=>SaleMenIsApproval=false تم رفض  التحويل 
                    //    model.SaleMenIsApproval = false;

                    db.Entry(model).State = EntityState.Modified;
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        if (approve == 1)
                            return Json(new { isValid = true, message = "تم الاعتماد بنجاح" });
                        else
                            return Json(new { isValid = true, message = "تم الرفض بنجاح" });
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