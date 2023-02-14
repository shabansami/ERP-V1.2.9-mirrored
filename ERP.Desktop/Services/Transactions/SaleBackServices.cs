using Newtonsoft.Json;
using ERP.Web.DataTablesDS;
using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Utilities;
using ERP.Desktop.ViewModels;
using ERP.Web.Services;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace ERP.Desktop.Services.Transactions
{
    public class SaleBackServices
    {
        VTSaleEntities db;
        public SaleBackServices(VTSaleEntities dbContext)
        {
            db = dbContext;
        }

        #region AllInvoices 

        public List<ShiftInvoiceVM> GetAllInvoices(Guid ShiftID)
        {
            return db.SellBackInvoices.Where(x => !x.IsDeleted && x.ShiftOfflineID == ShiftID).Select(x => new ShiftInvoiceVM()
            {
                Id = x.Id,
                InvoiceNumPaper = "",
                InvoiceNumber = x.InvoiceNumber,
                InvoKind = "مبيعات",
                Safy = (float)x.Safy,
                ClintName = x.PersonCustomer.Name,
                IsFinalApproved = x.IsFinalApproval,
                Status = x.IsFinalApproval ? "مغلقة" : "مفتوحة"
            }).ToList();
        }

        /// <summary>
        /// get all invoices in shift and filter it
        /// </summary>
        /// <param name="shiftID">shift Id</param>
        /// <param name="searchText">text for searching</param>
        /// <param name="isOpenStatus">if get open invoices = <see langword="true"/> ,if gel closed invoices false,if all invoices set null</param>
        /// <returns>list of filtered invoice in the shift</returns>
        public List<ShiftInvoiceVM> GetAllItemsFilter(Guid shiftID, string searchText, bool? isOpenStatus)
        {
            var search = db.SellBackInvoices.Where(x => !x.IsDeleted && x.ShiftOfflineID == shiftID);
            if (isOpenStatus != null)
            {
                search = search.Where(x => x.IsFinalApproval == !isOpenStatus);
            }
            if (searchText.Length > 2)
            {
                if (Guid.TryParse(searchText, out Guid id))
                {
                    search = search.Where(x => x.Id == id || x.EmployeeId == id || x.CustomerId == id);
                }
                else if (Guid.TryParse(searchText, out Guid guid))
                {
                    search = search.Where(x => x.Id == guid);
                }
                else
                {
                    search = search.Where(x => x.PersonCustomer.Name.Contains(searchText) || x.Notes.Contains(searchText));
                }
            }
            return search.Select(x => new ShiftInvoiceVM()
            {
                Id = x.Id,
                InvoiceNumPaper = "",
                InvoiceNumber = x.InvoiceNumber,
                InvoKind = "مبيعات",
                Safy = (float)x.Safy,
                ClintName = x.PersonCustomer.Name,
                IsFinalApproved = x.IsFinalApproval,
                Status = x.IsFinalApproval ? "مغلقة" : "مفتوحة"
            }).ToList();
        }

        public List<InvoiceDetailVM> GetInvoiceDetails(Guid invoiceID)
        {
            return db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoiceId == invoiceID).Select(x => new InvoiceDetailVM()
            {
                Id = x.Id,
                ItemName = x.Item.Name,
                Amount = (float)x.Amount,
                Quantity = (float)x.Quantity
            }).ToList();
        }
        public DtoResultObj<SellBackInvoice> OpenAnInvoice(Guid invoiceID)
        {
            var result = new DtoResultObj<SellBackInvoice>();
            var invoice = db.SellBackInvoices.FirstOrDefault(x => !x.IsDeleted && x.Id == invoiceID);
            if (invoice == null)
            {
                result.Message = "خطأ في رقم الفاتورة";
                return result;
            }
            else if (!invoice.IsFinalApproval)
            {
                result.Message = "الفاتورة غير مغقلة بالفعل";
                return result;
            }
            invoice.IsFinalApproval = false;
            invoice.IsApprovalAccountant = false;
            invoice.IsApprovalStore = false;
            db.SaveChanges(UserServices.UserInfo.UserId);
            result.IsSuccessed = true;
            result.Object = invoice;
            return result;
        }
        #endregion

        #region Invoice operations
        #region Create New
        public DtoResultObj<SellBackInvoice> CreateInvoice(SellBackInvoice model, bool isInvoiceDisVal = false)
        {
            try
            {
                if (model.CustomerId == null || model.BranchId == null || model.InvoiceDate == null || model.PaymentTypeId == null)
                    return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "تأكد من ادخال بيانات صحيحة" };

                if (model.PaymentTypeId == (int)PaymentTypeCl.Deferred) //ف حالة السداد آجل
                {
                    if (model.PayedValue > 0)
                        return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "تم استلام مبلغ من العميل وحالة السداد آجل " };

                    model.BankAccountId = null;
                    model.SafeId = null;
                }
                else  // ف حالة السداد نقدى او جزئى
                {
                    if (model.BankAccountId == null && model.SafeId == null)
                        return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" };
                }


                model.InvoiceDate = CommonMethods.TimeNow;

                //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                //فى حالة الخصم على الفاتورة نسية /قيمة 
                double invoiceDiscount = 0;
                if (isInvoiceDisVal == true)
                {
                    invoiceDiscount = model.InvoiceDiscount;
                    model.DiscountPercentage = 0;
                }
                else if (isInvoiceDisVal == false)
                {
                    model.DiscountPercentage = model.DiscountPercentage;
                }

                model.TotalQuantity = 0;
                model.TotalDiscount = invoiceDiscount;//إجمالي خصومات الفاتورة 
                model.TotalValue = 0;//إجمالي قيمة المبيعات 
                model.TotalExpenses = 0;//إجمالي قيمة الايرادادت
                model.Safy = (model.TotalValue + model.TotalExpenses + model.SalesTax) - (model.TotalDiscount + model.ProfitTax);

                //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                if (model.PaymentTypeId == (int)PaymentTypeCl.Cash && model.PayedValue != model.Safy)
                    return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" };

                model.CaseId = (int)CasesCl.InvoiceCreated;

                //فى حالة ان البيع من خلال مندوب
                if (model.BySaleMen)
                {
                    if (model.EmployeeId == null)
                        return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "تأكد من اختيار المندوب اولا" };
                }
                else
                    model.EmployeeId = null;

                var db = DBContext.UnitDbContext;

                //اضافة رقم الفاتورة
                string codePrefix = Properties.Settings.Default.CodePrefix;
                model.InvoiceNumber = codePrefix + (db.SellBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                //اضافة الحالة 
                db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellBackInvoice = model,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceCreated
                });
                db.SellBackInvoices.Add(model);
                if (db.SaveChanges(UserServices.UserInfo?.UserId) > 0)
                {
                    return new DtoResultObj<SellBackInvoice> { IsSuccessed = true, Object = model, Message = "تم الاضافة بنجاح" };
                }
                else
                    return new DtoResultObj<SellBackInvoice> { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };

            }
            catch (Exception ex)
            {
                return new DtoResultObj<SellBackInvoice>() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
        }
        #endregion

        #region Load Invoice
        public DtoResultObj<SellBackInvoice> GetInvoice(Guid invoiceID)
        {
            var invoice = db.SellBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return new DtoResultObj<SellBackInvoice> { IsSuccessed = invoice != null, Object = invoice, Message = invoice != null ? "" : "خطأ في رقم الفاتروة" };
        }
        #endregion

        #region Validations
        public DtoResultObj<SellBackInvoice> CheckValidInvoice(Guid invoiceID)
        {
            var db = DBContext.UnitDbContext;
            {
                var invoice = db.SellBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
                return CheckValidInvoice(invoice);
            }
        }

        public DtoResultObj<SellBackInvoice> CheckValidInvoice(SellBackInvoice invoice)
        {
            var result = new DtoResultObj<SellBackInvoice>();
            if (invoice == null)
            {
                result.Message = "لم يتم ايجاد الفاتورة";
                return result;
            }
            if (invoice.IsApprovalAccountant)
            {
                result.Message = "تم اعتماد الفاتورة من المحاسب";
                return result;
            }

            if (invoice.IsApprovalStore)
            {
                result.Message = "تم اعتماد الفاتورة من المخازن";
                return result;
            }

            if (invoice.IsFinalApproval)
            {
                result.Message = "تم الاعتماد النهائي للفاتورة";
                return result;
            }
            result.IsSuccessed = true;
            result.Object = invoice;
            return result;
        }
        #endregion

        #region Calculations
        internal DtoResultObj<SellBackInvoice> CalculateInvoiceTotals(Guid invoiceID)
        {
            var invoice = db.SellBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CalculateInvoiceTotals(invoice);
        }

        public DtoResultObj<SellBackInvoice> CalculateInvoiceTotals(SellBackInvoice invoice)
        {
            var result = new DtoResultObj<SellBackInvoice>();
            if (invoice == null)
            {
                result.Message = "الفاتورة غير موجودة";
                return result;
            }

            //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
            //فى حالة الخصم على الفاتورة نسية /قيمة 

            invoice.TotalValue = invoice.SellBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Amount);//إجمالي قيمة المبيعات 
            invoice.TotalQuantity = invoice.SellBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Quantity);


            double invoiceDiscount = invoice.InvoiceDiscount;
            if (invoice.DiscountPercentage != 0)
            {
                invoiceDiscount = (invoice.TotalValue * invoice.DiscountPercentage) / 100;
            }
            invoice.TotalDiscount = invoice.SellBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.ItemDiscount) + invoiceDiscount;//إجمالي خصومات الفاتورة 

            invoice.TotalExpenses = db.SellBackInvoiceIncomes.Where(x => x.SellBackInvoiceId == invoice.Id && !x.IsDeleted).Select(x => x.Amount).DefaultIfEmpty(0).Sum();//إجمالي قيمة الايرادادت

            invoice.Safy = (invoice.TotalValue + invoice.TotalExpenses + invoice.SalesTax) - (invoice.TotalDiscount + invoice.ProfitTax);

            invoice.InvoiceDiscount = invoiceDiscount;


            //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
            if (invoice.PaymentTypeId == (int)PaymentTypeCl.Cash || invoice.PaymentTypeId == (int)PaymentTypeCl.Partial || invoice.PaymentTypeId == (int)PaymentTypeCl.BankWallet || invoice.PaymentTypeId == (int)PaymentTypeCl.BankCard)
            {
                invoice.PayedValue = invoice.Safy;
                invoice.RemindValue = 0;
            }
            //فى حالة السداد اجل
            if (invoice.PaymentTypeId == (int)PaymentTypeCl.Deferred || invoice.PaymentTypeId == (int)PaymentTypeCl.Installment)
            {
                invoice.PayedValue = 0;
                invoice.RemindValue = invoice.Safy;
            }

            db.SaveChanges(UserServices.UserInfo.UserId);
            result.Object = invoice;
            result.IsSuccessed = true;

            return result;
        }
        #endregion

        #region Update Invoice

        #endregion

        #region Close Invoice
        public DtoResult CloseInvoice(Guid invoiceID)
        {
            var invoice = db.SellBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CloseInvoice(invoice);
        }

        public DtoResult CloseInvoice(SellBackInvoice invoice)
        {
            var result = new DtoResult();
            if (invoice == null)
            {
                result.Message = "خطأ في رقم الفاتروة";
                return result;
            }
            if (invoice.IsFinalApproval)
            {
                result.Message = "الفاتورة مغلقة بالفعل";
                return result;
            }

            if (invoice.CustomerId is null)
            {
                result.Message = "الرجاء اختيار العميل";
                return result;
            }

            if (invoice.Safy < 0)
            {
                result.Message = "خطأ في صافي الفاتورة";
                return result;
            }

            if (invoice.PayedValue < 0)
            {
                result.Message = "خطأ في المبلغ المدفوع";
                return result;
            }
            switch (invoice.PaymentTypeId)
            {
                case (int)PaymentTypeCl.Cash:
                    if (invoice.Safy != invoice.PayedValue)
                    {
                        result.Message = "خطأ في المبلغ المدفوع\nيجب دفع المبغ كامل";
                        return result;
                    }
                    break;
                case (int)PaymentTypeCl.Partial:
                case (int)PaymentTypeCl.BankWallet:
                case (int)PaymentTypeCl.BankCard:

                    if (invoice.PayedValue == 0)
                    {
                        result.Message = "خطأ في المبلغ المدفوع\nيجب دفع جزء من المبلغ او تغير نوع الدفع للأجل";
                        return result;
                    }
                    break;
                case (int)PaymentTypeCl.Deferred:
                    if (invoice.PayedValue != 0)
                    {
                        result.Message = "خطأ في المبلغ المدفوع\nيجب ان يكون المبلغ المدفوع 0 او تغير نوع الدفع لدفع جزئي";
                        return result;
                    }
                    break;
                default:
                    result.Message = "خطأ في نوع الدفع";
                    return result;
            }
            //فى حالة اختيار الاعتماد مباشرا بعد الحفظ من الاعدادات العامة 
            var approvalAfterSave = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
            if (approvalAfterSave == "1")
            {
                //الاعتماد المحاسبى 
                invoice.IsApprovalAccountant = true;
                invoice.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                //اضافة الحالة 
                invoice.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellBackInvoice = invoice,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalAccountant
                });
                //الاعتماد المخزنى
                invoice.CaseId = (int)CasesCl.InvoiceApprovalStore;
                invoice.IsApprovalStore = true;
                //اضافة الحالة 
                invoice.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellBackInvoice = invoice,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalStore
                });
            }
            //الاعتماد النهائى 
            if (approvalAfterSave == "1")
            {

                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    //صافى قيمة الفاتورة= ( إجمالي قيمة مرتجع المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                    double debit = 0;
                    double credit = 0;
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue)))
                    {
                        result.Message = "حساب مرتجع المبيعات ليس بحساب فرعى";
                        return result;
                    }

                    if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == invoice.CustomerId).FirstOrDefault().AccountsTreeCustomerId))
                    {
                        result.Message = "حساب العميل ليس بحساب فرعى";
                        return result;
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue)))
                    {
                        result.Message = "حساب القيمة المضافة ليس بحساب فرعى";
                        return result;
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue)))
                    {
                        result.Message = "حساب الخصومات ليس بحساب فرعى";
                        return result;
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue)))
                    {
                        result.Message = "حساب الارباح التجارية ليس بحساب فرعى";
                        return result;
                    }
                    var expenses = db.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == invoice.Id);
                    var customer = db.Persons.Where(x => x.Id == invoice.CustomerId).FirstOrDefault();

                    if (expenses.Count() > 0)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                        {
                            result.Message = "حساب الايراد ليس بحساب فرعى";
                            return result;
                        }
                    }

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
                                //من ح/ المخزون
                                foreach (var item in invoice.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                                {
                                    var store = db.Stores.Where(x => x.Id == item.StoreId).FirstOrDefault();
                                    if (store != null)
                                    {
                                        if (store.AccountTreeId != null)
                                        {
                                            if (AccountTreeService.CheckAccountTreeIdHasChilds(store.AccountTreeId))
                                            {
                                                result.Message = $"حساب المخزن {store.Name} ليس بحساب فرعى";
                                                return result;
                                            }
                                            db.GeneralDailies.Add(new GeneralDaily
                                            {
                                                AccountsTreeId = store.AccountTreeId,
                                                BranchId = invoice.BranchId,
                                                Debit = item.Amount,
                                                Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber} من العميل {customer.Name}",
                                                TransactionDate = invoice.InvoiceDate,
                                                TransactionId = invoice.Id,
                                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                            });
                                        }
                                        else
                                        {
                                            result.Message = $"حساب المخزون للمخزن {store.Name} غير موجود";
                                            return result;
                                        }
                                    }
                                }
                                // الي ح/  تكلفة بضاعه مباعه
                                db.GeneralDailies.Add(new GeneralDaily
                                {
                                    AccountsTreeId = accountTreeSalesCost,
                                    BranchId = invoice.BranchId,
                                    Credit = invoice.TotalValue,
                                    Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber} من العميل {customer.Name}",
                                    TransactionDate = invoice.InvoiceDate,
                                    TransactionId = invoice.Id,
                                    TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                                });


                                debit = debit + invoice.TotalValue;
                                credit = credit + invoice.TotalValue;
                            }
                            else
                            {
                                result.Message = "تأكد من تحديد حساب تكلفة بضاعه مباعه من الاعدادات اولا";
                                return result;
                            }
                        }
                        else
                        {

                            // حساب مرتجع المبيعات
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue),
                                BranchId = invoice.BranchId,
                                Debit = invoice.TotalValue,
                                Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                                TransactionDate = invoice.InvoiceDate,
                                TransactionId = invoice.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                            });
                            debit = debit + invoice.TotalValue;
                            // حساب العميل
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = db.Persons.Where(x => x.Id == invoice.CustomerId).FirstOrDefault().AccountsTreeCustomerId,
                                BranchId = invoice.BranchId,
                                Credit = invoice.Safy,
                                Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                                TransactionDate = invoice.InvoiceDate,
                                TransactionId = invoice.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                            });
                            credit = credit + invoice.Safy;
                        }

                    }
                    else
                    {
                        result.Message = "تأكد من تحديد نوع الجرد اولا";
                        return result;
                    }
                    //============================================





                    // القيمة المضافة
                    if (invoice.SalesTax > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Debit = invoice.SalesTax,
                            Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                        });
                        debit = debit + invoice.SalesTax;


                    }
                    // الخصومات
                    if (invoice.TotalDiscount > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Credit = invoice.TotalDiscount,
                            Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                        });
                        credit = credit + invoice.TotalDiscount;
                    }

                    // ضريبة ارباح تجارية
                    if (invoice.ProfitTax > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Credit = invoice.ProfitTax,
                            Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                        });
                        credit = credit + invoice.ProfitTax;
                    }

                    //  المبلغ المدفوع من حساب العميل (مدين
                    if (invoice.PayedValue > 0 && invoice.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = db.Persons.Where(x => x.Id == invoice.CustomerId).FirstOrDefault().AccountsTreeCustomerId,
                            BranchId = invoice.BranchId,
                            Debit = invoice.PayedValue,
                            Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                        });
                        debit = debit + invoice.PayedValue;

                        //  المبلغ المدفوع الى حساب الخزنة او البنك (دائن
                        Guid? safeAccouyBank = null;
                        if (invoice.SafeId != null)
                            safeAccouyBank = db.Safes.Find(invoice.SafeId).AccountsTreeId;
                        else if (invoice.BankAccountId != null)
                            safeAccouyBank = db.BankAccounts.Find(invoice.BankAccountId).AccountsTreeId;

                        if (safeAccouyBank != null)
                        {
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = safeAccouyBank,
                                BranchId = invoice.BranchId,
                                Credit = invoice.PayedValue,
                                Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                                TransactionDate = invoice.InvoiceDate,
                                TransactionId = invoice.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                            });
                            credit = credit + invoice.PayedValue;
                        }


                    }

                    // الايرادات (مدين
                    foreach (var expense in expenses)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = expense.IncomeTypeAccountTreeId,
                            BranchId = invoice.BranchId,
                            Debit = expense.Amount,
                            Notes = $"فاتورة مرتجع بيع رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.SellReturn
                        });
                        debit = debit + expense.Amount;

                    }
                    //التاكد من توازن المعاملة 
                    if (debit == credit)
                    {

                        //حفظ القيود 
                        // اعتماد النهائى للفاتورة
                        invoice.CaseId = (int)CasesCl.BackInvoiceFinalApproval;
                        invoice.IsFinalApproval = true;
                        db.Entry(invoice).State = EntityState.Modified;
                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellBackInvoice = invoice,
                            IsSellInvoice = false,
                            CaseId = (int)CasesCl.BackInvoiceFinalApproval
                        });

                        //add new row in history serial item 
                        foreach (var item in invoice.SellBackInvoicesDetails.Where(x => !x.IsDeleted).ToList())
                        {
                            if (item.ItemSerialId != null && item.ItemSerialId != Guid.Empty)
                            {
                                //update item serial case history
                                var serial = db.ItemSerials.Find(item.ItemSerialId);
                                if (serial != null)
                                {
                                    serial.SerialCaseId = (int)SerialCaseCl.SellBack;
                                    //تحديث حالة حقل فاتورة البيع للاصناف فى جدول السيريال بنل حتى يمكن بيعه مرة اخرى 
                                    serial.SellInvoiceId = null;
                                    serial.CurrentStoreId = item.StoreId;
                                    db.Entry(serial).State = EntityState.Modified;
                                    //add item serial case history
                                    db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                    {
                                        ItemSerialId = serial.Id,
                                        ReferrenceId = invoice.Id,
                                        SerialCaseId = (int)SerialCaseCl.SellBack
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        result.Message = "قيد المعاملة غير موزوون";
                        return result;
                    }

                }
                else
                {
                    result.Message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات";
                    return result;
                }
            }
            //invoice.IsFinalApproval = true;
            db.SaveChanges(UserServices.UserInfo.UserId);
            result.IsSuccessed = true;
            return result;
        }
        #endregion
        #endregion

        #region Invoice Incomes and outcomes
        public List<InvoiceExpensesDT> GetInvoiceIncomes(Guid invoiceID)
        {
            var db = DBContext.UnitDbContext;
            {
                return db.SellBackInvoiceIncomes.Where(x => !x.IsDeleted && x.SellBackInvoiceId == invoiceID).Select(expense => new InvoiceExpensesDT
                {
                    ExpenseAmount = expense.Amount,
                    ExpenseTypeId = expense.IncomeTypeAccountTreeId,
                    ExpenseTypeName = expense.IncomeTypeAccountTree.AccountName
                }).ToList();
            }
        }
        #endregion

        #region Invoice Item Operations

        #region Ieam Search

        public DtoResultObj<Item> ItemCodeSearch(string code)
        {
            var result = new DtoResultObj<Item>();

            //Not deleted items
            var items = db.Items.Where(x => !x.IsDeleted);

            //Search by barcode
            var item = items.FirstOrDefault(x => x.BarCode == code);
            if (item != null)
            {
                result.IsSuccessed = true;
                result.Object = item;
                return result;
            }

            //search by item code
            if (!string.IsNullOrEmpty(code))
            {
                item = items.FirstOrDefault(x => x.ItemCode == code);
                if (item != null)
                {
                    result.IsSuccessed = true;
                    result.Object = item;
                    return result;
                }
            }

            //search by ID
            if (Guid.TryParse(code, out Guid id))
            {
                item = items.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    result.IsSuccessed = true;
                    result.Object = item;
                    return result;
                }
            }

            return result;
        }
        #endregion

        #region Add new 

        /// <summary>
        /// Add new item to invoice
        /// </summary>
        /// <param name="invoiceID">sellInvoice id</param>
        /// <param name="itemDetail">Item detials</param>
        /// <param name="mergeMatched">is merge matched items if exist</param>
        /// <returns>result contains operation success status and message and list of items in the invoice</returns>
        public DtoResultObj<List<ItemDetailsDT>> AddItemInInvoice(Guid invoiceID, SellBackInvoicesDetail itemDetail, bool mergeMatched = false)
        {
            var result = new DtoResultObj<List<ItemDetailsDT>>();

            //التأكد من ان الفاتورة صالحة
            var invoiceResult = CheckValidInvoice(invoiceID);
            if (!invoiceResult.IsSuccessed)
            {
                result.Message = invoiceResult.Message;
                return result;
            }

            if (itemDetail.Amount < itemDetail.ItemDiscount)
            {
                result.Message = "خطأ في الخصم";
                return result;
            }

            if (mergeMatched)
            {
                var existItemDetail = db.SellBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.SellBackInvoiceId == invoiceID && x.ItemId == itemDetail.ItemId && x.Amount / x.Quantity == itemDetail.Price && x.ItemDiscount / x.Quantity == itemDetail.ItemDiscount / itemDetail.Quantity);
                if (existItemDetail != null)
                {
                    existItemDetail.Amount += itemDetail.Amount;
                    existItemDetail.Quantity += itemDetail.Quantity;
                    existItemDetail.QuantityReal += itemDetail.QuantityReal;
                    existItemDetail.ItemDiscount += itemDetail.ItemDiscount;
                    result.IsSuccessed = db.SaveChanges(UserServices.UserInfo?.UserId) > 0;
                    result.Object = GetInvoiceItems(invoiceID);

                    return result;
                }
            }
            itemDetail.SellBackInvoiceId = invoiceID;
            db.SellBackInvoicesDetails.Add(itemDetail);
            result.IsSuccessed = db.SaveChanges(UserServices.UserInfo?.UserId) > 0;
            result.Object = GetInvoiceItems(invoiceID);
            return result;
        }

        #endregion

        #region Get Items
        public List<ItemDetailsDT> GetInvoiceItems(Guid invoiceID)
        {
            var db = DBContext.UnitDbContext;
            {
                return db.SellBackInvoicesDetails.Where(x => !x.IsDeleted && x.SellBackInvoiceId == invoiceID).Select(item => new ItemDetailsDT
                {
                    Id = item.Id,
                    Amount = item.Amount,
                    ItemId = item.ItemId,
                    ItemDiscount = item.ItemDiscount,
                    ItemName = item.Item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    StoreId = item.StoreId,
                    StoreName = item.Store.Name,
                    SerialItemId = item.ItemSerialId
                }).ToList();
            }
        }
        #endregion

        #region Delete Item from Invoice
        public DtoResult DeleteItem(Guid sellBackInvoiceDetailID)
        {
            var result = new DtoResult();
            var item = db.SellBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == sellBackInvoiceDetailID);
            if (item == null)
            {
                result.Message = "خطأ في رقم الصنف";
                return result;
            }
            item.IsDeleted = true;
            db.SaveChanges(UserServices.UserInfo.UserId);
            result.IsSuccessed = true;
            return result;
        }

        #endregion

        #region Update Item
        internal DtoResult UpdateItemQuantity(Guid id, double quantity)
        {
            var result = new DtoResult();
            var item = db.SellBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
            if (item == null)
            {
                result.Message = "خطأ في رقم الصنف";
                return result;
            }


            item.Quantity = quantity;
            item.Amount = quantity * item.Price;
            db.SaveChanges(UserServices.UserInfo.UserId);

            result.IsSuccessed = true;
            return result;
        }


        #endregion
        #endregion
    }
}
