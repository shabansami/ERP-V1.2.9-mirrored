using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;
using ERP.Web.Services;

namespace ERP.Web.Controllers
{
    [Authorization] // لم يتم استخدامها 

    public class SellFullBackInvoicesController : Controller
    {
        // GET: SellFullBackInvoices
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة فواتير مرتجع البيع
        public ActionResult Index(string msg)
        {
            if (msg != null)
                ViewBag.Msg = "فاتورة البيع تم ارجاعها مسبقا";

            return View();
        }

        public ActionResult GetAll(string invoId, string dtFrom, string dtTo)
        {
            int? n = null;
            Guid invoiceId = Guid.Empty;
            Guid.TryParse(invoId, out invoiceId);

            DateTime dateFrom, dateTo;
            DateTime.TryParse(dtFrom, out dateFrom);
            DateTime.TryParse(dtTo, out dateTo);
            var data = db.SellInvoices.Where(x => !x.IsDeleted && x.IsApprovalAccountant && x.IsApprovalStore && ((x.Id == invoiceId && invoiceId != Guid.Empty) || (DbFunctions.TruncateTime(x.InvoiceDate) >= dateFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dateTo))).Select(x => new { Id = x.Id,  InvoiceNum = x.Id, InvoiceDate = x.InvoiceDate.ToString(), CustomerName = x.PersonCustomer.Name, Safy = x.Safy, Actions = n, Num = n });
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

        #region ارجاع كامل فاتورة توريد 
        [HttpPost]
        public ActionResult ReturnAllInvoice(string invoGuid)
        {
            Guid guid;
            if (Guid.TryParse(invoGuid, out guid))
            {
                var sellInvoice = db.SellInvoices.Where(x => x.Id == guid).FirstOrDefault();
                if (sellInvoice != null)
                {
                    if (sellInvoice.IsFullReturned == true)
                        return Json(new { isValid = false, message = "فاتورة التوريد تم ارجاعها مسبقا" });
                    var itemBackIetails = sellInvoice.SellInvoicesDetails.Where(x => !x.IsDeleted).Select(
                        purchDetails => new SellBackInvoicesDetail
                        {
                            Amount = purchDetails.Amount,
                            ItemDiscount = purchDetails.ItemDiscount,
                            ItemId = purchDetails.ItemId,
                            Price = purchDetails.Price,
                            Quantity = purchDetails.Quantity,
                            QuantityReal = purchDetails.QuantityReal,
                            StoreId = purchDetails.StoreId
                        }).ToList();
                    var backExpenses = sellInvoice.SellInvoiceIncomes.Where(x => !x.IsDeleted).Select(purchExpenses => new SellBackInvoiceIncome
                    {
                        Amount = purchExpenses.Amount,
                        IncomeTypeAccountTreeId = purchExpenses.IncomeTypeAccountTreeId
                    }).ToList();

                    //اضافة رقم الفاتورة
                    string codePrefix = Properties.Settings.Default.CodePrefix;
                    var invoiceNumber = codePrefix + (db.SellBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                    var sellBack = new SellBackInvoice
                    {
                        BankAccountId = sellInvoice.BankAccountId,
                        BranchId = sellInvoice.BranchId,
                        CaseId = (int)CasesCl.BackInvoiceCreated,
                        InvoiceDate = Utility.GetDateTime(),
                        InvoiceDiscount = sellInvoice.InvoiceDiscount,
                        InvoiceNumber = invoiceNumber,
                        IsApprovalAccountant = false,
                        IsApprovalStore = false,
                        PayedValue = sellInvoice.PayedValue,
                        PaymentTypeId = sellInvoice.PaymentTypeId,
                        CustomerId = sellInvoice.CustomerId,
                        ProfitTax = sellInvoice.ProfitTax,
                        SellInvoiceId = sellInvoice.Id,
                        RemindValue = sellInvoice.RemindValue,
                        SafeId = sellInvoice.SafeId,
                        Safy = sellInvoice.Safy,
                        SalesTax = sellInvoice.SalesTax,
                        TotalDiscount = sellInvoice.TotalDiscount,
                        TotalExpenses = sellInvoice.TotalExpenses,
                        TotalQuantity = sellInvoice.TotalQuantity,
                        TotalValue = sellInvoice.TotalValue,
                        SellBackInvoicesDetails = itemBackIetails,
                        SellBackInvoiceIncomes = backExpenses
                    };

                    db.SellBackInvoices.Add(sellBack);
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellBackInvoice = sellBack,
                        IsSellInvoice = false,
                        CaseId = (int)CasesCl.BackInvoiceCreated
                    });

                    //تحديث حقل IsFullReturned  
                    sellInvoice.IsFullReturned = true;
                    db.Entry(sellInvoice).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellInvoice = sellInvoice,
                        IsSellInvoice = true,
                        CaseId = (int)CasesCl.FullBackInvoice
                    });
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, message = "تم استرجاع كامل الفاتورة بنجاح" });
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

        #region wizard step 2  اصناف الفاتورة
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
                if (deDS.Where(x => x.ItemId == vm.ItemId).Count() > 0)
                    return Json(new { isValid = false, msg = "اسم الصنف موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                itemName = db.Items.FirstOrDefault(x=>x.Id==vm.ItemId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار الصنف " }, JsonRequestBehavior.AllowGet);
            if (vm.StoreId != null)
                storeName = db.Stores.FirstOrDefault(x=>x.Id==vm.StoreId).Name;
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار المخزن " }, JsonRequestBehavior.AllowGet);

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ItemDiscount = vm.ItemDiscount,  StoreId = vm.StoreId, StoreName = storeName };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3  مصروفات الفاتورة
        public ActionResult GetDStSellBackInvoiceExpenses()
        {
            int? n = null;
            if (DSExpenses == null)
                return Json(new
                {
                    data = new InvoiceExpensesDT()
                }, JsonRequestBehavior.AllowGet);
            else
                return Json(new
                {
                    data = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DSExpenses)
                }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddSellInvoiceExpenses(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            string expenseTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "المصروف المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                //expenseTypeName = db.ExpenseTypes.FirstOrDefault(x=>x.Id==vm.ExpenseTypeId).Name;
            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار مصروف " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل المصروف بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ارجاع جزء من فاتورة البيع 
        public ActionResult ReturnPatialInvoice(string invoGuid)
        {
            Guid guId;

            if (!Guid.TryParse(invoGuid, out guId) || string.IsNullOrEmpty(invoGuid) || invoGuid == "undefined")
                return RedirectToAction("Index");

            //ViewBag.ContainerId = new SelectList(db.Containers.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            //ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");

            var vm = db.SellInvoices.Where(x => x.Id == guId).FirstOrDefault();
            if (vm.IsFullReturned == true)
                return RedirectToAction("Index", "SellFullBackInvoices", new { msg = "1" });


            List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
            var items = db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).Select(item => new
                          ItemDetailsDT
            {
                Id = item.Id,
                Amount = item.Amount,
                ItemId = item.ItemId,
                ItemDiscount = item.ItemDiscount,
                ItemName = item.Item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                StoreId = item.StoreId,
                StoreName = item.Store.Name
            }).ToList();
            DS = JsonConvert.SerializeObject(items);
            var expenses = db.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == vm.Id).Select(expense => new
                         InvoiceExpensesDT
            {
                ExpenseAmount = expense.Amount,
                ExpenseTypeId = expense.IncomeTypeAccountTreeId,
                ExpenseTypeName = expense.IncomeTypeAccountTree.AccountName
            }).ToList();
            DSExpenses = JsonConvert.SerializeObject(expenses);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.CustomerId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Customer || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", vm.CustomerId);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
            ViewBag.SafeId = new SelectList(db.Safes.Where(x => !x.IsDeleted), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
            //ViewBag.InvoiceNum = vm.InvoiceNum;
            return View(vm);

        }
        [HttpPost]
        public JsonResult ReturnPatialInvoice(SellInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.CustomerId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
                        return Json(new { isValid = false, message = "تأكد من ادخال بيانات صحيحة" });

                    if (vm.PaymentTypeId == (int)PaymentTypeCl.Deferred) //ف حالة السداد آجل
                    {
                        if (vm.PayedValue > 0)
                            return Json(new { isValid = false, message = "تم استلام مبلغ من العميل وحالة السداد آجل " });

                        vm.BankAccountId = null;
                        vm.SafeId = null;
                    }
                    else  // ف حالة السداد نقدى او جزئى
                    {
                        if (vm.BankAccountId == null && vm.SafeId == null)
                            return Json(new { isValid = false, message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" });
                    }


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
                                  ItemId = x.ItemId,
                                  StoreId = x.StoreId,
                                  Quantity = x.Quantity,
                                  Price = x.Price,
                                  Amount = x.Amount,
                                  ItemDiscount = x.ItemDiscount,
                              }).ToList();
                        }
                    }
                    else
                        return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });

                    //المصروفات
                    List<InvoiceExpensesDT> invoiceExpensesDT = new List<InvoiceExpensesDT>();
                    List<SellBackInvoiceIncome> invoicesExpens = new List<SellBackInvoiceIncome>();

                    if (DT_DatasourceExpenses != null)
                    {
                        invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        invoicesExpens = invoiceExpensesDT.Select(
                            x => new SellBackInvoiceIncome
                            {
                                IncomeTypeAccountTreeId = x.ExpenseTypeId,
                                Amount = x.ExpenseAmount
                            }
                            ).ToList();
                    }


                    SellBackInvoice model = new SellBackInvoice();
                    model.InvoiceDate = vm.InvoiceDate.Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second));

                    //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                    model.TotalQuantity = itemDetailsDT.Sum(x => x.Quantity);
                    model.TotalDiscount = itemDetailsDT.Sum(x => x.ItemDiscount) + vm.InvoiceDiscount;//إجمالي خصومات الفاتورة 
                    model.TotalValue = itemDetailsDT.Sum(x => x.Amount);//إجمالي قيمة المشتريات 
                    model.TotalExpenses = invoiceExpensesDT.Sum(x => x.ExpenseAmount);//إجمالي قيمة المصروفات
                    model.Safy = (model.TotalValue + model.TotalExpenses + vm.SalesTax) - (model.TotalDiscount + vm.ProfitTax);

                    //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                    if (vm.PaymentTypeId == (int)PaymentTypeCl.Cash && vm.PayedValue != model.Safy)
                        return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                    model.IsApprovalAccountant = false;
                    model.IsApprovalStore = false;

                    // Update purchase invoice
                    model.SellInvoiceId = vm.Id;
                    model.CustomerId = vm.CustomerId;
                    model.BranchId = vm.BranchId;
                    model.PaymentTypeId = vm.PaymentTypeId;
                    model.SafeId = vm.SafeId;
                    model.BankAccountId = vm.BankAccountId;
                    model.PayedValue = vm.PayedValue;
                    model.RemindValue = vm.RemindValue;
                    model.SalesTax = vm.SalesTax;
                    model.ProfitTax = vm.ProfitTax;
                    model.InvoiceDiscount = vm.InvoiceDiscount;
                    model.DueDate = vm.DueDate;
                    model.Notes = vm.Notes;
                    model.CaseId = (int)CasesCl.BackInvoiceCreated;
                    model.SellBackInvoicesDetails = items;
                    model.SellBackInvoiceIncomes = invoicesExpens;

                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellBackInvoice = model,
                        IsSellInvoice = false,
                        CaseId = (int)CasesCl.BackInvoiceCreated
                    });


                    //تحديث حقل IsFullReturned  
                    var SellInvoice = db.SellInvoices.Where(x => x.Id == vm.Id).FirstOrDefault();
                    SellInvoice.IsFullReturned = false;
                    db.Entry(SellInvoice).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                    {
                        SellInvoice = SellInvoice,
                        IsSellInvoice = true,
                        CaseId = (int)CasesCl.PartialBackInvoice
                    });
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                        return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.SellBackInvoice, refGid = model.Id, message = "تم الاضافة بنجاح" });
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
    }
}