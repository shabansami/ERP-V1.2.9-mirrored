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
using static ERP.Web.Services.ItemService;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class PurchaseFullBackInvoicesController : Controller
    {
        // GET: PurchaseFullBackInvoices
        VTSaleEntities db;
        VTSAuth auth => TempData["userInfo"] as VTSAuth;
        ItemService itemService;
        public PurchaseFullBackInvoicesController()
        {
            db = new VTSaleEntities();
            itemService = new ItemService();
        }
        public static string DS { get; set; }
        public static string DSExpenses { get; set; }

        #region ادارة فواتير مرتجع التوريد
        public ActionResult Index(string msg)
        {
            if (msg != null)
                ViewBag.Msg = "فاتورة التوريد تم ارجاعها مسبقا";

            return View();
        }

        public ActionResult GetAll(string invoId,string dtFrom,string dtTo)
        {
            int? n = null;
            //Guid.TryParse(invoId, out Guid invoiceId);

            DateTime dateFrom, dateTo;
            var data = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.IsFinalApproval&&!x.IsFullReturned==true);
            if (!string.IsNullOrEmpty(invoId))
                data = data.Where(x => x.InvoiceNumber.Contains(invoId));
            if (DateTime.TryParse(dtFrom, out dateFrom) && DateTime.TryParse(dtTo, out dateTo))
                data = data.Where(x => DbFunctions.TruncateTime(x.InvoiceDate) >= dateFrom && DbFunctions.TruncateTime(x.InvoiceDate) <= dateTo);

              var list =data.OrderBy(x=>x.CreatedOn).Select(x => new { Id = x.Id,  InvoiceNum = x.InvoiceNumber, InvoiceDate = x.InvoiceDate.ToString(), SupplierName = x.PersonSupplier.Name, Safy = x.Safy, Actions = n, Num = n }).ToList();
            //var t = data.ToList();
            if (list.Count()>0)
            {
             return Json(new
                        {
                            data = list
             }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    data =new { }
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
                var purchaseInvoice = db.PurchaseInvoices.Where(x => x.Id == guid).FirstOrDefault();
                if (purchaseInvoice != null)
                {
                    if (purchaseInvoice.IsFullReturned==true)
                        return Json(new { isValid = false, message = "فاتورة التوريد تم ارجاعها مسبقا" });
                    var itemBackIetails = purchaseInvoice.PurchaseInvoicesDetails.Where(x => !x.IsDeleted).Select(
                        purchDetails=>new PurchaseBackInvoicesDetail
                        {
                            Amount= purchDetails.Amount,
                            ContainerId=purchDetails.ContainerId,
                            ItemDiscount= purchDetails.ItemDiscount,
                            ItemEntryDate= purchDetails.ItemEntryDate,
                            ItemId= purchDetails.ItemId,
                            Price= purchDetails.Price,
                            Quantity= purchDetails.Quantity,
                            QuantityReal= purchDetails.QuantityReal,
                            StoreId= purchDetails.StoreId
                        }).ToList();
                    //هل الصنف يسمح بالسحب منه بالسالب
                    foreach (var item in itemBackIetails)
                    {
                        var result = itemService.IsAllowNoBalance(item.ItemId, item.StoreId,item.Quantity);
                        if (!result.IsValid)
                            return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });
                    }
                    var backExpenses = purchaseInvoice.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted).Select(purchExpenses => new PurchaseBackInvoicesExpens
                    {
                        Amount=purchExpenses.Amount,
                        ExpenseTypeAccountsTreeID= purchExpenses.ExpenseTypeAccountTreeId
                    }).ToList();



                    //Transaction
                    using (var context = new VTSaleEntities())
                    {
                        using (DbContextTransaction transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                var model = new PurchaseBackInvoice
                                {
                                    BankAccountId = purchaseInvoice.BankAccountId,
                                    BranchId = purchaseInvoice.BranchId,
                                    CaseId = (int)CasesCl.BackInvoiceCreated,
                                    InvoiceDate = Utility.GetDateTime().Add(new TimeSpan(Utility.GetDateTime().Hour, Utility.GetDateTime().Minute, Utility.GetDateTime().Second)),
                                    InvoiceDiscount = purchaseInvoice.InvoiceDiscount,
                                    IsApprovalAccountant = false,
                                    IsApprovalStore = false,
                                    PayedValue = purchaseInvoice.PayedValue,
                                    PaymentTypeId = purchaseInvoice.PaymentTypeId,
                                    SupplierId = purchaseInvoice.SupplierId,
                                    ProfitTax = purchaseInvoice.ProfitTax,
                                    PurchaseInvoiceId = purchaseInvoice.Id,
                                    RemindValue = purchaseInvoice.RemindValue,
                                    SafeId = purchaseInvoice.SafeId,
                                    Safy = purchaseInvoice.Safy,
                                    SalesTax = purchaseInvoice.SalesTax,
                                    TotalDiscount = purchaseInvoice.TotalDiscount,
                                    TotalExpenses = purchaseInvoice.TotalExpenses,
                                    TotalQuantity = purchaseInvoice.TotalQuantity,
                                    TotalValue = purchaseInvoice.TotalValue,
                                    PurchaseBackInvoicesDetails = itemBackIetails,
                                    PurchaseBackInvoicesExpenses = backExpenses
                                };                                

                                //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                                model.TotalQuantity = itemBackIetails.Sum(x => x.Quantity);
                                model.TotalDiscount = itemBackIetails.Sum(x => x.ItemDiscount) + model.InvoiceDiscount;//إجمالي خصومات الفاتورة 
                                model.TotalValue = itemBackIetails.Sum(x => x.Amount);//إجمالي قيمة المشتريات 
                                model.TotalExpenses = backExpenses.Sum(x => x.Amount);//إجمالي قيمة المصروفات
                                model.Safy = (model.TotalValue + model.TotalExpenses + model.SalesTax) - (model.TotalDiscount + model.ProfitTax);

                                //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                                if (model.PaymentTypeId == (int)PaymentTypeCl.Cash && model.PayedValue != model.Safy)
                                    return Json(new { isValid = false, message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" });

                                    //اضافة رقم الفاتورة
                                    string codePrefix = Properties.Settings.Default.CodePrefix;
                                    model.InvoiceNumber = codePrefix + (db.PurchaseBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                                //تحديث حقل IsFullReturned  
                                purchaseInvoice.IsFullReturned = true;
                                db.Entry(purchaseInvoice).State = EntityState.Modified;
                                //اضافة الحالة 
                                db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                {
                                    PurchaseInvoice = purchaseInvoice,
                                    IsPurchaseInvoice = true,
                                    CaseId = (int)CasesCl.FullBackInvoice
                                });

                                //اضافة الحالة 
                                context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = false,
                                        CaseId = (int)CasesCl.BackInvoiceCreated
                                    });
                                    context.PurchaseBackInvoices.Add(model);


                                //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
                                var approvalAfterSave = context.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
                                if (approvalAfterSave == "1")
                                {
                                    //الاعتماد المحاسبى 
                                    model.IsApprovalAccountant = true;
                                    model.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                                    //اضافة الحالة 
                                    context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalAccountant
                                    });
                                    //الاعتماد المخزنى
                                    model.CaseId = (int)CasesCl.InvoiceApprovalStore;
                                    model.IsApprovalStore = true;
                                    //اضافة الحالة 
                                    context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                    {
                                        PurchaseBackInvoice = model,
                                        IsPurchaseInvoice = true,
                                        CaseId = (int)CasesCl.InvoiceApprovalStore
                                    });
                                }
                                context.SaveChanges(auth.CookieValues.UserId);
                                //الاعتماد النهائى 
                                if (approvalAfterSave == "1")
                                {
                                    //    //تسجيل القيود
                                    // General Dailies
                                    if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                                    {
                                        //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                                        double debit = 0;
                                        double credit = 0;
                                        // الحصول على حسابات من الاعدادات
                                        var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                                        //التأكد من عدم وجود حساب فرعى من الحساب
                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب المشتريات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                                            return Json(new { isValid = false, message = "حساب المورد ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب القيمة المضافة ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الخصومات ليس بحساب فرعى" });

                                        if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                                            return Json(new { isValid = false, message = "حساب الارباح التجارية ليس بحساب فرعى" });

                                        var expenses = context.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == model.Id);
                                        var supplier = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault();

                                        if (expenses.Count() > 0)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountsTreeID))
                                                return Json(new { isValid = false, message = "حساب المصروفات ليس بحساب فرعى" });
                                        }

                                        // حساب مرتجع مشتريات
                                        if (model.TotalValue != 0)
                                        {
                                            //=============================================================
                                            //تحديد نوع الجرد
                                            var inventoryType = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InventoryType).FirstOrDefault().SValue;
                                            if (int.TryParse(inventoryType, out int inventoryTypeVal))
                                            {
                                                //فى حالة الجرد المستمر
                                                if (inventoryTypeVal == 2) //نوع الجرد مستمر 
                                                {
                                                    if (Guid.TryParse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesCost).FirstOrDefault().SValue, out Guid accountTreeSalesCost))
                                                    {

                                                        //الى ح/ المخزون
                                                        foreach (var item in model.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                                        {
                                                            var store = db.Stores.Where(x => x.Id == item.StoreId).FirstOrDefault();
                                                            if (store != null)
                                                            {
                                                                if (store.AccountTreeId != null)
                                                                {
                                                                    if (AccountTreeService.CheckAccountTreeIdHasChilds(store.AccountTreeId))
                                                                        return Json(new { isValid = false, message = $"حساب المخزن {store.Name} ليس بحساب فرعى" });

                                                                    db.GeneralDailies.Add(new GeneralDaily
                                                                    {
                                                                        AccountsTreeId = store.AccountTreeId,
                                                                        BranchId = model.BranchId,
                                                                        Credit = item.Amount,
                                                                        Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                                        TransactionDate = model.InvoiceDate,
                                                                        TransactionId = model.Id,
                                                                        TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                                                    });
                                                                }
                                                            }
                                                        }
                                                        credit = credit + model.TotalValue;
                                                    }
                                                    else
                                                        return Json(new { isValid = false, message = "تأكد من تحديد حساب تكلفة بضاعه مباعه من الاعدادات اولا" });
                                                }
                                                else
                                                {

                                                    // الى حساب مرتجع المشتريات
                                                    context.GeneralDailies.Add(new GeneralDaily
                                                    {
                                                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue),
                                                        BranchId = model.BranchId,
                                                        Credit = model.TotalValue,
                                                        Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                        TransactionDate = model.InvoiceDate,
                                                        TransactionId = model.Id,
                                                        TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                                    });
                                                    credit = credit + model.TotalValue;
                                                }

                                            }
                                            else
                                                return Json(new { isValid = false, message = "تأكد من تحديد نوع الجرد اولا" });
                                            //============================================

                                            // من حساب المورد
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                                                BranchId = model.BranchId,
                                                Debit = model.Safy,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            debit = debit + model.Safy;
                                        }



                                        // القيمة المضافة
                                        if (model.SalesTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Credit = model.SalesTax,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + model.SalesTax;


                                        }
                                        // الخصومات
                                        if (model.TotalDiscount > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.TotalDiscount,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            debit = debit + model.TotalDiscount;
                                        }

                                        // ضريبة ارباح تجارية
                                        if (model.ProfitTax > 0)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                                                BranchId = model.BranchId,
                                                Debit = model.ProfitTax,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            debit = debit + model.ProfitTax;
                                        }

                                        //  المبلغ المدفوع من حساب المورد (مدين
                                        if (model.PayedValue > 0 && model.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = context.Persons.Where(x => x.Id == model.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                                                BranchId = model.BranchId,
                                                Credit = model.PayedValue,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + model.PayedValue;

                                            //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
                                            Guid? safeAccouyBank = null;
                                            if (model.SafeId != null)
                                                safeAccouyBank = context.Safes.FirstOrDefault(x => x.Id == model.SafeId).AccountsTreeId;
                                            else if (model.BankAccountId != null)
                                                safeAccouyBank = context.BankAccounts.FirstOrDefault(x => x.Id == model.BankAccountId).AccountsTreeId;

                                            if (safeAccouyBank != null)
                                            {
                                                context.GeneralDailies.Add(new GeneralDaily
                                                {
                                                    AccountsTreeId = safeAccouyBank,
                                                    BranchId = model.BranchId,
                                                    Debit = model.PayedValue,
                                                    Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                    TransactionDate = model.InvoiceDate,
                                                    TransactionId = model.Id,
                                                    TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                                });
                                                debit = debit + model.PayedValue;
                                            }


                                        }

                                        // المصروفات (مدين
                                        foreach (var expense in expenses)
                                        {
                                            context.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = expense.ExpenseTypeAccountsTreeID,
                                                //AccountsTreeId = expense.ExpenseType.AccountsTreeId,
                                                BranchId = model.BranchId,
                                                Credit = expense.Amount,
                                                Notes = $"فاتورة مرتجع توريد رقم : {model.InvoiceNumber} الى المورد {supplier.Name}",
                                                TransactionDate = model.InvoiceDate,
                                                TransactionId = model.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                                            });
                                            credit = credit + expense.Amount;

                                        }
                                        //التاكد من توازن المعاملة 
                                        if (debit == credit)
                                        {

                                            //حفظ القيود 
                                            // اعتماد النهائى للفاتورة
                                            model.CaseId = (int)CasesCl.BackInvoiceFinalApproval;
                                            model.IsFinalApproval = true;
                                            context.Entry(model).State = EntityState.Modified;
                                            //اضافة الحالة 
                                            context.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                                            {
                                                PurchaseBackInvoice = model,
                                                IsPurchaseInvoice = false,
                                                CaseId = (int)CasesCl.BackInvoiceFinalApproval
                                            });
                                            context.SaveChanges(auth.CookieValues.UserId);
                                        }
                                        else
                                        {
                                            return Json(new { isValid = false, message = "قيد المعاملة غير موزوون" });
                                        }

                                    }
                                    else
                                        return Json(new { isValid = false, message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات" });
                                }
                                transaction.Commit();
                                return Json(new { isValid = true, message = "تم استرجاع كامل الفاتورة بنجاح" });
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
            string containerName = "";
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

            if (vm.ContainerId != null)
                containerName = db.Containers.FirstOrDefault(x=>x.Id==vm.ContainerId).Name;

            var newItemDetails = new ItemDetailsDT { ItemId = vm.ItemId, ItemName = itemName, Quantity = vm.Quantity, Price = vm.Price, Amount = vm.Quantity * vm.Price, ContainerId = vm.ContainerId, ContainerName = containerName, ItemDiscount = vm.ItemDiscount, ItemEntryDate = vm.ItemEntryDate, StoreId = vm.StoreId, StoreName = storeName };
            deDS.Add(newItemDetails);
            DS = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم اضافة الصنف بنجاح ", totalAmount = deDS.Sum(x => x.Amount), totalDiscountItems = deDS.Sum(x => x.ItemDiscount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region wizard step 3  مصروفات الفاتورة
        public ActionResult GetDStPurchaseBackInvoiceExpenses()
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
        public ActionResult AddPurchaseInvoiceExpenses(InvoiceExpensesDT vm)
        {
            List<InvoiceExpensesDT> deDS = new List<InvoiceExpensesDT>();
            string expenseTypeName = "";
            if (vm.DT_Datasource != null)
                deDS = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(vm.DT_Datasource);
            if (vm.ExpenseTypeId != null)
            {
                if (deDS.Where(x => x.ExpenseTypeId == vm.ExpenseTypeId).Count() > 0)
                    return Json(new { isValid = false, msg = "المصروف المحدد موجود مسبقا " }, JsonRequestBehavior.AllowGet);
                expenseTypeName = db.AccountsTrees.FirstOrDefault(x=>x.Id==vm.ExpenseTypeId).AccountName;
                if (AccountTreeService.CheckAccountTreeIdHasChilds(vm.ExpenseTypeId))
                    return Json(new { isValid = false, msg = "حساب المصروف ليس بحساب فرعى" });

            }
            else
                return Json(new { isValid = false, msg = "تأكد من اختيار مصروف " }, JsonRequestBehavior.AllowGet);

            var newPurchaseInvoExpens = new InvoiceExpensesDT { ExpenseTypeId = vm.ExpenseTypeId, ExpenseTypeName = expenseTypeName, DT_Datasource = vm.DT_Datasource, ExpenseAmount = vm.ExpenseAmount };
            deDS.Add(newPurchaseInvoExpens);
            DSExpenses = JsonConvert.SerializeObject(deDS);
            return Json(new { isValid = true, msg = "تم تسجيل المصروف بنجاح ", totalExpenses = deDS.Sum(x => x.ExpenseAmount) }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ارجاع جزء من فاتورة التوريد 
        public ActionResult ReturnPatialInvoice(string invoGuid)
        {
            Guid guId;

            if (!Guid.TryParse(invoGuid, out guId) || string.IsNullOrEmpty(invoGuid) || invoGuid == "undefined")
                return RedirectToAction("Index");

            //ViewBag.ContainerId = new SelectList(db.Containers.Where(x => !x.IsDeleted), "Id", "Name");
            ViewBag.StoreId = new SelectList(new List<Store>(), "Id", "Name");
            //ViewBag.ItemId = new SelectList(new List<Item>(), "Id", "Name");
            //ViewBag.ExpenseTypeId = new SelectList(db.ExpenseTypes.Where(x => !x.IsDeleted), "Id", "Name");

            var vm = db.PurchaseInvoices.Where(x => x.Id == guId).FirstOrDefault();
            if (vm.IsFullReturned == true)
                return RedirectToAction("Index", "PurchaseFullBackInvoices", new { msg = "1" });
                

            List<ItemDetailsDT> itemDetailsDTs = new List<ItemDetailsDT>();
            var items = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == vm.Id).Select(item => new
                          ItemDetailsDT
            {
                Id=item.Id,
                Amount = item.Amount,
                ContainerId = item.ContainerId,
                ContainerName = item.Container != null ? item.Container.Name : null,
                ItemId = item.ItemId,
                ItemDiscount = item.ItemDiscount,
                ItemEntryDate = item.ItemEntryDate.ToString()/*!=null? item.ItemEntryDate.Value.ToShortDateString():null*/,
                ItemName = item.Item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                StoreId = item.StoreId,
                StoreName = item.Store.Name
            }).ToList();
            DS = JsonConvert.SerializeObject(items);
            var expenses = db.PurchaseInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseInvoiceId == vm.Id).Select(expense => new
                         InvoiceExpensesDT
            {
                ExpenseAmount = expense.Amount,
                ExpenseTypeId = expense.ExpenseTypeAccountTreeId,
                ExpenseTypeName = expense.ExpenseTypeAccountTree.AccountName
            }).ToList();
            DSExpenses = JsonConvert.SerializeObject(expenses);
            var branches = EmployeeService.GetBranchesByUser(auth.CookieValues);
            ViewBag.SupplierId = new SelectList(db.Persons.Where(x => !x.IsDeleted && (x.PersonTypeId == (int)PersonTypeCl.Supplier || x.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)), "Id", "Name", vm.SupplierId);
            ViewBag.BranchId = new SelectList(branches, "Id", "Name", vm.BranchId);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(x => !x.IsDeleted), "Id", "Name", vm.PaymentTypeId);
            ViewBag.SafeId = new SelectList(EmployeeService.GetSafesByUser(vm.BranchId.ToString(), auth.CookieValues.UserId.ToString()), "Id", "Name", vm.Safe != null ? vm.SafeId : null);
            ViewBag.BankAccountId = new SelectList(db.BankAccounts.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, AccountName = x.AccountName + " / " + x.Bank.Name }), "Id", "AccountName", vm.BankAccount != null ? vm.BankAccountId : null);
            //ViewBag.InvoiceNum = vm.InvoiceNum;
            return View(vm);

        }
        [HttpPost]
        public JsonResult ReturnPatialInvoice(PurchaseInvoice vm, string DT_DatasourceItems, string DT_DatasourceExpenses)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (vm.SupplierId == null || vm.BranchId == null || vm.InvoiceDate == null || vm.PaymentTypeId == null)
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
                    List<PurchaseBackInvoicesDetail> items = new List<PurchaseBackInvoicesDetail>();

                    if (DT_DatasourceItems != null)
                    {
                        itemDetailsDT = JsonConvert.DeserializeObject<List<ItemDetailsDT>>(DT_DatasourceItems);
                        if (itemDetailsDT.Count() == 0)
                            return Json(new { isValid = false, message = "تأكد من ادخال صنف واحد على الاقل" });
                        else
                        {
                            items = itemDetailsDT.Select(x =>
                              new PurchaseBackInvoicesDetail
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

                    //هل الصنف يسمح بالسحب منه بالسالب
                    foreach (var item in items)
                    {
                        var result = itemService.IsAllowNoBalance(item.ItemId, item.StoreId, item.Quantity);
                        if (!result.IsValid)
                            return Json(new { isValid = false, message = $"غير مسموح بالسحب بالسالب من الرصيد للصنف {result.ItemNotAllowed}" });
                    }
                    //التأكد من ارجاع اصناف فاتورة التوريد بنفس العدد او اقل 
                    var purchaseInvo = db.PurchaseInvoices.Where(x => x.Id == vm.Id).FirstOrDefault();
                    if (purchaseInvo!=null)
                    {
                        foreach (var item in purchaseInvo.PurchaseInvoicesDetails.Where(x=>!x.IsDeleted))
                        {
                            if(!items.Any(x=>x.ItemId==item.ItemId&&x.Quantity<=item.Quantity))
                                return Json(new { isValid = false, message = $"تم ادخال عدد للصنف {item.Item?.Name} اكبر من عدد فاتورة التوريد" });
                        }
                    }

                    //المصروفات
                    List<InvoiceExpensesDT> invoiceExpensesDT = new List<InvoiceExpensesDT>();
                    List<PurchaseBackInvoicesExpens> invoicesExpens = new List<PurchaseBackInvoicesExpens>();

                    if (DT_DatasourceExpenses != null)
                    {
                        invoiceExpensesDT = JsonConvert.DeserializeObject<List<InvoiceExpensesDT>>(DT_DatasourceExpenses);
                        invoicesExpens = invoiceExpensesDT.Select(
                            x => new PurchaseBackInvoicesExpens
                            {
                                ExpenseTypeAccountsTreeID = x.ExpenseTypeId,
                                Amount = x.ExpenseAmount
                            }
                            ).ToList();
                    }


                    PurchaseBackInvoice model = new PurchaseBackInvoice();
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
                    model.PurchaseInvoiceId = vm.Id;
                        model.SupplierId = vm.SupplierId;
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
                        model.PurchaseBackInvoicesDetails = items;
                        model.PurchaseBackInvoicesExpenses = invoicesExpens;

                        //اضافة الحالة 
                        db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                        {
                            PurchaseBackInvoice = model,
                            IsPurchaseInvoice = false,
                            CaseId = (int)CasesCl.BackInvoiceCreated
                        });


                    //تحديث حقل IsFullReturned  
                    var purchaseInvoice = db.PurchaseInvoices.Where(x => x.Id == vm.Id).FirstOrDefault();
                    purchaseInvoice.IsFullReturned = false;
                    db.Entry(purchaseInvoice).State = EntityState.Modified;
                    //اضافة الحالة 
                    db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                    {
                        PurchaseInvoice = purchaseInvoice,
                        IsPurchaseInvoice = true,
                        CaseId = (int)CasesCl.PartialBackInvoice
                    });
                    if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                            return Json(new { isValid = true, typ = (int)UploalCenterTypeCl.PurchaseBackInvoice, refGid = model.Id, message = "تم الاضافة بنجاح" });
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