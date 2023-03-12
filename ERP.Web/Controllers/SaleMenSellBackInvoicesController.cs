using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Services;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
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

    public class SaleMenSellBackInvoicesController : Controller
    {
        // GET: SaleMenSellBackInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public SaleMenSellBackInvoicesController()
        {
            db = new VTSaleEntities();
        }
        public static string DS { get; set; }

        #region ادارة فواتير مرتجع التوريد
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {
            int? n = null;
            return Json(new
            {
                data = db.SellBackInvoices.Where(x => !x.IsDeleted&&x.BySaleMen && x.EmployeeId == auth.CookieValues.EmployeeId).OrderBy(x => x.CreatedOn).Select(x => new { Id = x.Id, InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, IsApprovalAccountant = x.IsApprovalAccountant, ApprovalAccountant = x.IsApprovalAccountant ? "معتمده" : "غير معتمدة", Actions = n, Num = n }).ToList()
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
                itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = vm.ItemDiscount, StoreId = vm.StoreId, StoreName = storeName, ProductionOrderId = vm.ProductionOrderId, IsIntial = vm.IsIntial, SerialItemId = vm.SerialItemId };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region تسجيل فاتورة توريد وتعديلها وحذفها
        [HttpGet]
        public ActionResult CreateEdit()
        {
            ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            //مخازن المندوب
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            var branchId = branches.FirstOrDefault()?.Id;
            var stores = EmployeeService.GetStoresByUser(branchId.ToString(), auth.CookieValues.UserId.ToString());

            if (stores == null || stores.Count() == 0) //فى حالة ان الموظف غير محدد له مخزن اى انه ليس مندوب
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
                    var vm = db.SellBackInvoices.Where(x => x.Id == guId).Select
                       (x => new SaleMenSellInvoiceVM
                       {
                           Id = x.Id,
                           BranchId = x.BranchId,
                           BySaleMen = x.BySaleMen,
                           //SaleMenStoreId = auth.CookieValues.StoreId,
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
                           TotalItemDiscount = x.SellBackInvoicesDetails.Where(y => !y.IsDeleted).Select(y => y.ItemDiscount).Sum()
                       })
                       .FirstOrDefault();


                    List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
                    var items = db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoiceId == vm.Id).Select(item => new
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
                        SerialItemId=item.ItemSerialId
                        //IsIntial = item.IsItemIntial ? 1 : 0,
                        //ProductionOrderId = item.ProductionOrderId

                    }).ToList();
                    DS = JsonConvert.SerializeObject(items);

                    //ViewBag.StoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == vm.BranchId), "Id", "Name");
                    //ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", vm.BranchId);
                    ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
                    //ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
                    ////ViewBag.SaleMenId = new SelectList(db.Employees.Where(x => !x.IsDeleted && x.IsSaleMen && x.PersonId == vm.SaleMenId).Select(x => new { Id = x.PersonId, Name = x.Person.Name }), "Id", "Name", vm.SaleMenId);
                    //ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
                    //ViewBag.InvoiceNum = vm.InvoiceNum;
                
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
                //ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                //ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
                //ViewBag.BranchId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", 1);
                //ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");
                //ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted && x.BranchId == 1), "Id", "Name", 1);
                ////ViewBag.SaleMenId = new SelectList(new List<Person>(), "Id", "Name");
                //ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName");
                ViewBag.PersonCategoryId = new SelectList(db.PersonCategories.Where(x => !x.IsDeleted && x.IsCustomer), "Id", "Name");
                ViewBag.CustomerId = new SelectList(new List<Person>(), "Id", "Name");
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name");

                Random random = new Random();
                //ViewBag.InvoiceNum = Utility.GetDateTime().Year.ToString() + Utility.GetDateTime().Month.ToString() + Utility.GetDateTime().Day.ToString() + Utility.GetDateTime().Hour.ToString() + Utility.GetDateTime().Minute.ToString()  + random.Next(10).ToString();

                var vm = new SaleMenSellInvoiceVM();
                vm.InvoiceDate = Utility.GetDateTime();
                //var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
                vm.BranchId = branchId;

                vm.SaleMenEmployeeId = auth.CookieValues.EmployeeId;
                //vm.SaleMenStoreId = auth.CookieValues.StoreId;

                return View(vm);
            }
        }
        [HttpPost]
        public JsonResult CreateEdit(SaleMenSellInvoiceVM vm, string DT_DatasourceItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    //الاصناف
                    List<ItemDetailsDT> itemDetailsDT = new List<ItemDetailsDT>();
                    List<SellBackInvoicesDetail> items = new List<SellBackInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new SellBackInvoicesDetail
                              {
                                  SellBackInvoiceId = vm.Id,
                                  ItemId = x.ItemId,
                                  StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  Price = x.Price,
                                  Amount = x.Amount,
                                  ItemDiscount = x.ItemDiscount,
                                  ItemSerialId=x.SerialItemId,
                                  //ProductionOrderId = x.ProductionOrderId,
                                  //IsItemIntial = x.IsIntial == 1 ? true : false
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                    var isInsert = false;
                    SellBackInvoice model = null;
                    if (vm.Id != Guid.Empty)
                    {
                        model = db.SellBackInvoices.FirstOrDefault(x=>x.Id==vm.Id);
                        if (model.InvoiceDate.ToShortDateString() != vm.InvoiceDate.ToShortDateString())
                            model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                    }
                    else
                    {
                        model = new SellBackInvoice
                        {
                            Id = vm.Id,
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
                            TotalDiscount = vm.TotalDiscount,
                            TotalQuantity = vm.TotalQuantity,
                            TotalValue = vm.TotalValue
                        }; model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));
                    }

                    //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                    model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                    model.TotalDiscount = itemDetailsDT.Sum(x => x.ItemDiscount) + vm.InvoiceDiscount;//إجمالي خصومات الفاتورة 
                    model.TotalValue = itemDetailsDT.Sum(x => x.Amount);//إجمالي قيمة المشتريات 
                    model.Safy = (model.TotalValue  + vm.SalesTax) - (model.TotalDiscount + vm.ProfitTax);

                    //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                    if (vm.PaymentTypeId == (int)PaymentTypeCl.Cash && vm.PayedValue != model.Safy)
                        return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                    if (vm.Id != Guid.Empty)
                    {
                        //delete all privous items 
                        var privousItems = db.SellBackInvoicesDetails.Where(x => x.SellBackInvoiceId == vm.Id).ToList();
                        foreach (var item in privousItems)
                        {
                            item.IsDeleted = true;
                            db.Entry(item).State = EntityState.Modified;
                        }
                        
                        
                        // Update purchase invoice
                        model.SellInvoiceId = vm.SellInvoiceId == Guid.Empty ? null : vm.SellInvoiceId;
                        model.CustomerId = vm.CustomerId;
                        model.BranchId = vm.BranchId;
                        model.PaymentTypeId = vm.PaymentTypeId;
                        model.PayedValue = vm.PayedValue;
                        model.RemindValue = vm.RemindValue;
                        model.SalesTax = vm.SalesTax;
                        model.ProfitTax = vm.ProfitTax;
                        model.InvoiceDiscount = vm.InvoiceDiscount;
                        //model.ShippingAddress = vm.ShippingAddress;
                        model.DueDate = vm.DueDate;
                        model.Notes = vm.Notes;
                        model.CaseId = (int)CasesCl.BackInvoiceSaleModified;
                        //فى حالة ان البيع من خلال مندوب

                                model.EmployeeId = vm.SaleMenEmployeeId;

                        db.Entry(model).State = EntityState.Modified;
                        db.SellBackInvoicesDetails.AddRange(items);
                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellBackInvoice = model,
                            IsSellInvoice = false,
                            CaseId = (int)CasesCl.BackInvoiceSaleModified
                        });

                    }
                    else
                    {

                        isInsert = true;
                        //اضافة رقم الفاتورة
                        string codePrefix = Properties.Settings.Default.CodePrefix;
                        model.InvoiceNumber = codePrefix + (db.SellBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                        model.CaseId = (int)CasesCl.BackInvoiceSaleMenCreated;
                        model.SellBackInvoicesDetails = items;


                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellBackInvoice = model,
                            IsSellInvoice = false,
                            CaseId = (int)CasesCl.BackInvoiceSaleMenCreated
                        });
                        db.SellBackInvoices.Add(model);

                        //add new row in history serial item 
                        foreach (var item in itemDetailsDT)
                        {
                            if (item.SerialItemId != null)
                            {
                                var history = db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                {
                                    ItemSerialId = item.SerialItemId,
                                    ReferrenceId = model.Id,
                                    SerialCaseId = (int)SerialCaseCl.SellBack,
                                });
                            }
                        }

                    }
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                    {
                        if (isInsert)
                            return Json(new { isValid = true, isInsert, message = "تم الاضافة بنجاح" });
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
                var model = db.SellBackInvoices.Where(x => x.Id == Id).FirstOrDefault();
                if (model != null)
                {
                    model.IsDeleted = true;
                    model.CaseId = (int)CasesCl.BackInvoiceDeleted;
                    db.Entry(model).State = EntityState.Modified;

                    var details = db.SellBackInvoicesDetails.Where(x => x.SellBackInvoiceId == model.Id).ToList();
                    //details.ForEach(x => x.IsDeleted = true);
                    foreach (var detail in details)
                    {
                        detail.IsDeleted = true;
                        db.Entry(detail).State = EntityState.Modified;
                    }

                    //حذف الحالات 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellBackInvoice = model,
                        IsSellInvoice = false,
                        CaseId = (int)CasesCl.BackInvoiceDeleted
                    });
                    var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == model.Id && x.TransactionTypeId == (int)TransactionsTypesCl.SellReturn).ToList();
                    // حذف كل القيود 
                    foreach (var generalDay in generalDalies)
                    {
                        generalDay.IsDeleted = true;
                        db.Entry(generalDay).State = EntityState.Modified;
                    }

                    // حذف كل حالات الاصناف فى جدول itemSerial history ان وجدت 
                    var itemSerialsHistories = db.CasesItemSerialHistories.Where(x => x.ReferrenceId == model.Id).ToList();
                    foreach (var itemSerialHis in itemSerialsHistories)
                    {
                        itemSerialHis.IsDeleted = true;
                        db.Entry(itemSerialHis).State = EntityState.Modified;
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
        public ActionResult ShowSaleMenSellBackInvoice(Guid? invoGuid)
        {
            if (invoGuid == null || invoGuid == Guid.Empty)
                return RedirectToAction("Index");
            var vm = db.SellBackInvoices.Where(x => x.Id == invoGuid && x.SellBackInvoicesDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            vm.SellBackInvoicesDetails = vm.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList();
            vm.SellBackInvoiceIncomes = vm.SellBackInvoiceIncomes.Where(x => !x.IsDeleted).ToList();
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