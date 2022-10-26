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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;
using System.Diagnostics;
using ERP.DAL.Models;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace ERP.Desktop.Services.Transactions
{
    public class SaleServices
    {
        VTSaleEntities db;
        public SaleServices(VTSaleEntities dbContext)
        {
            db = dbContext;
        }

        #region AllInvoices 

        public List<ShiftInvoiceVM> GetAllInvoices(Guid ShiftID)
        {
            return db.SellInvoices.Where(x => !x.IsDeleted && x.ShiftOffLineID == ShiftID).Select(x => new ShiftInvoiceVM()
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
            var search = db.SellInvoices.Where(x => !x.IsDeleted && x.ShiftOffLineID == shiftID);
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
                InvoKind = "مبيعات",
                InvoiceNumber = x.InvoiceNumber,
                Safy = (float)x.Safy,
                ClintName = x.PersonCustomer.Name,
                IsFinalApproved = x.IsFinalApproval,
                Status = x.IsFinalApproval ? "مغلقة" : "مفتوحة"
            }).ToList();
        }

        public List<InvoiceDetailVM> GetInvoiceDetails(Guid invoiceID)
        {
            return db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == invoiceID).Select(x => new InvoiceDetailVM()
            {
                Id = x.Id,
                ItemName = x.Item.Name,
                Amount = (float)x.Amount,
                Quantity = (float)x.Quantity
            }).ToList();
        }
        public DtoResultObj<SellInvoice> OpenAnInvoice(Guid invoiceID)
        {
            var result = new DtoResultObj<SellInvoice>();
            var invoice = db.SellInvoices.FirstOrDefault(x => !x.IsDeleted && x.Id == invoiceID);
            if (invoice == null)
            {
                result.Message = "خطأ في رقم الفاتورة";
                return result;
            }
            else if (!invoice.IsFinalApproval)
            {
                result.Message = "الفاتورة غير مغلقة بالفعل";
                return result;
            }
            //فى حالة تم الاعتماد النهائى للفاتورة ثم مطلوب تعديلها 
            var generalDalies = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionId == invoiceID && x.TransactionTypeId == (int)TransactionsTypesCl.Sell).ToList();
            // حذف كل القيود 
            foreach (var generalDay in generalDalies)
            {
                generalDay.IsDeleted = true;
                db.Entry(generalDay).State = EntityState.Modified;
            }


            invoice.IsFinalApproval = false;
            invoice.IsApprovalAccountant = false;
            invoice.IsApprovalStore = false;
            invoice.CaseId = (int)CasesCl.InvoiceModified;
            //اضافة الحالة 
            db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
            {
                SellInvoice = invoice,
                IsSellInvoice = true,
                CaseId = (int)CasesCl.InvoiceModified
            });
            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
            {
                result.IsSuccessed = true;
                result.Object = invoice;
            }
            else
            {
                result.Message = "حدث خطأ اثناء فتح الفاتورة";
                return result;
            }
            return result;
        }
        #endregion

        #region Invoice operations
        #region Create New
        public DtoResultObj<SellInvoice> CreateInvoice(SellInvoice model, bool isInvoiceDisVal = false)
        {
            try
            {
                if (model.CustomerId == null || model.BranchId == null || model.InvoiceDate == null || model.PaymentTypeId == null)
                    return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "تأكد من ادخال بيانات صحيحة" };

                if (model.PaymentTypeId == (int)PaymentTypeCl.Deferred) //ف حالة السداد آجل
                {
                    if (model.PayedValue > 0)
                        return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "تم استلام مبلغ من العميل وحالة السداد آجل " };

                    model.BankAccountId = null;
                    model.SafeId = null;
                }
                else  // ف حالة السداد نقدى او جزئى
                {
                    if (model.BankAccountId == null && model.SafeId == null)
                        return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" };
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
                    return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" };

                model.CaseId = (int)CasesCl.InvoiceCreated;

                //فى حالة ان البيع من خلال مندوب
                if (model.BySaleMen)
                {
                    if (model.EmployeeId == null)
                        return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "تأكد من اختيار المندوب اولا" };
                }
                else
                    model.EmployeeId = null;

                var db = DBContext.UnitDbContext;

                //اضافة رقم الفاتورة
                string codePrefix = Properties.Settings.Default.CodePrefix;
                model.InvoiceNumber = codePrefix + (db.SellInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                //اضافة الحالة 
                db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellInvoice = model,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceCreated
                });
                db.SellInvoices.Add(model);
                if (db.SaveChanges(UserServices.UserInfo?.UserId) > 0)
                {
                    return new DtoResultObj<SellInvoice> { IsSuccessed = true, Object = model, Message = "تم الاضافة بنجاح" };
                }
                else
                    return new DtoResultObj<SellInvoice> { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };

            }
            catch (Exception ex)
            {
                return new DtoResultObj<SellInvoice>() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
        }
        #endregion

        #region Load Invoice
        public DtoResultObj<SellInvoice> GetInvoice(Guid invoiceID)
        {
            var invoice = db.SellInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return new DtoResultObj<SellInvoice> { IsSuccessed = invoice != null, Object = invoice, Message = invoice != null ? "" : "خطأ في رقم الفاتروة" };
        }
        #endregion

        #region Validations
        public DtoResultObj<SellInvoice> CheckValidInvoice(Guid invoiceID)
        {
            var db = DBContext.UnitDbContext;
            {
                var invoice = db.SellInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
                return CheckValidInvoice(invoice);
            }
        }

        public DtoResultObj<SellInvoice> CheckValidInvoice(SellInvoice invoice)
        {
            var result = new DtoResultObj<SellInvoice>();
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
        internal DtoResultObj<SellInvoice> CalculateInvoiceTotals(Guid invoiceID)
        {
            var invoice = db.SellInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CalculateInvoiceTotals(invoice);
        }

        public DtoResultObj<SellInvoice> CalculateInvoiceTotals(SellInvoice invoice)
        {
            var result = new DtoResultObj<SellInvoice>();
            if (invoice == null)
            {
                result.Message = "الفاتورة غير موجودة";
                return result;
            }

            //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
            //فى حالة الخصم على الفاتورة نسية /قيمة 

            invoice.TotalValue = invoice.SellInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Amount);//إجمالي قيمة المبيعات 
            invoice.TotalQuantity = invoice.SellInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Quantity);


            double invoiceDiscount = invoice.InvoiceDiscount;
            if (invoice.DiscountPercentage != 0)
            {
                invoiceDiscount = (invoice.TotalValue * invoice.DiscountPercentage) / 100;
            }

            invoice.OffersDiscount = GetOffersDiscount(invoice.Id);

            invoice.TotalDiscount = invoice.SellInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.ItemDiscount) + invoiceDiscount + invoice.OffersDiscount;//إجمالي خصومات الفاتورة 

            invoice.TotalExpenses = db.SellInvoiceIncomes.Where(x => x.SellInvoiceId == invoice.Id && !x.IsDeleted).Select(x => x.Amount).DefaultIfEmpty(0).Sum();//إجمالي قيمة الايرادادت

            invoice.Safy = (invoice.TotalValue + invoice.TotalExpenses + invoice.SalesTax) - (invoice.TotalDiscount + invoice.ProfitTax);

            invoice.InvoiceDiscount = invoiceDiscount;


            //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
            if (invoice.PaymentTypeId == (int)PaymentTypeCl.Cash)
            {
                invoice.PayedValue = invoice.Safy;
                invoice.RemindValue = 0;
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
            var invoice = db.SellInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CloseInvoice(invoice);
        }

        public DtoResult CloseInvoice(SellInvoice invoice)
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

            //تحديد الخزنة او الحساب البنكى او المحفظة او البطاقة البنكية

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
            Stopwatch stopwatch = new Stopwatch();

            //الاعتماد النهائى 
            if (approvalAfterSave == "1")
            {
                stopwatch.Start();
                //الاعتماد المحاسبى 
                invoice.IsApprovalAccountant = true;
                invoice.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                //اضافة الحالة 
                invoice.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellInvoice = invoice,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalAccountant
                });
                //الاعتماد المخزنى
                invoice.CaseId = (int)CasesCl.InvoiceApprovalStore;
                invoice.IsApprovalStore = true;
                //اضافة الحالة 
                invoice.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                {
                    SellInvoice = invoice,
                    IsSellInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalStore
                });

                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات
                    //+ إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                    double debit = 0;
                    double credit = 0;
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue)))

                    {
                        result.Message = "حساب المبيعات ليس بحساب فرعى";
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

                    var expenses = db.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == invoice.Id);
                    var customer = db.Persons.Where(x => x.Id == invoice.CustomerId).FirstOrDefault();

                    if (expenses.Count() > 0)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().IncomeTypeAccountTreeId))
                        {
                            result.Message = "حساب الايراد ليس بحساب فرعى";
                            return result;
                        }
                    }

                    // من ح/  العميل
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = customer.AccountsTreeCustomerId,
                        BranchId = invoice.BranchId,
                        Debit = invoice.Safy,
                        Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                        TransactionDate = invoice.InvoiceDate,
                        TransactionId = invoice.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    //الى ح/ المبيعات
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue),
                        BranchId = invoice.BranchId,
                        Credit = invoice.TotalValue,
                        Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                        TransactionDate = invoice.InvoiceDate,
                        TransactionId = invoice.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.Sell
                    });
                    debit = debit + invoice.Safy;

                    credit = credit + invoice.TotalValue;

                    // القيمة المضافة
                    if (invoice.SalesTax > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Credit = invoice.SalesTax,
                            Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        credit = credit + invoice.SalesTax;


                    }
                    // الخصومات
                    if (invoice.TotalDiscount > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Debit = invoice.TotalDiscount,
                            Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        debit = debit + invoice.TotalDiscount;
                    }

                    // ضريبة ارباح تجارية
                    if (invoice.ProfitTax > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Debit = invoice.ProfitTax,
                            Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        debit = debit + invoice.ProfitTax;
                    }

                    //  المبلغ المدفوع الى حساب العميل (دائن
                    if (invoice.PayedValue > 0 && invoice.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                    {
                        Guid? safeAccouyBank = null;
                        if (invoice.SafeId != null)
                            safeAccouyBank = db.Safes.Find(invoice.SafeId).AccountsTreeId;
                        else if (invoice.BankAccountId != null)
                            safeAccouyBank = db.BankAccounts.Find(invoice.BankAccountId).AccountsTreeId;

                        if (safeAccouyBank != null)
                        {

                            //  المبلغ المدفوع من حساب الخزينة (مدين
                            db.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = safeAccouyBank,
                                BranchId = invoice.BranchId,
                                Debit = invoice.PayedValue,
                                Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                                TransactionDate = invoice.InvoiceDate,
                                TransactionId = invoice.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.Sell
                            });
                            debit = debit + invoice.PayedValue;

                        }


                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = customer.AccountsTreeCustomerId,
                            BranchId = invoice.BranchId,
                            Credit = invoice.PayedValue,
                            Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        credit = credit + invoice.PayedValue;



                    }

                    // الايرادات (دائن
                    foreach (var expense in expenses)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = expense.IncomeTypeAccountTreeId,
                            BranchId = invoice.BranchId,
                            Credit = expense.Amount,
                            Notes = $"فاتورة بيع رقم : {invoice.InvoiceNumber} للعميل {customer.Name}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.Sell
                        });
                        credit = credit + expense.Amount;
                    }
                    //التاكد من توازن المعاملة 
                    if (debit == credit)
                    {

                        //حفظ القيود 
                        // اعتماد النهائى للفاتورة
                        invoice.CaseId = (int)CasesCl.InvoiceFinalApproval;
                        invoice.IsFinalApproval = true;
                        db.Entry(invoice).State = EntityState.Modified;
                        //اضافة الحالة 
                        db.CasesSellInvoiceHistories.Add(new CasesSellInvoiceHistory
                        {
                            SellInvoice = invoice,
                            IsSellInvoice = true,
                            CaseId = (int)CasesCl.InvoiceFinalApproval
                        });

                        //اضافة اشعار 
                        if (invoice.RemindValue > 0 && invoice.DueDate != null)
                        {
                            db.Notifications.Add(new Notification
                            {
                                Name = $"استحقاق فاتورة بيع رقم: {invoice.InvoiceNumber} على العميل: {customer.Name}",
                                DueDate = invoice.DueDate,
                                RefNumber = invoice.Id,
                                NotificationTypeId = (int)NotificationTypeCl.SellInvoiceDueDateClient,
                                Amount = invoice.RemindValue
                            });
                        }


                        //انشاء السيرالات نمبر الخاص بكل صنف 
                        var itemDetails = invoice.SellInvoicesDetails.Where(x => !x.IsDeleted).ToList();
                        foreach (var item in itemDetails)
                        {
                            var itm = db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault();
                            if (itm != null)
                            {
                                bool createSerial = itm.CreateSerial;
                                if (createSerial)
                                {
                                    //تسجيل سيريال جديد لكل قطعة من الكمية المسجلة
                                    for (int i = 1; i < item.Quantity + 1; i++)
                                    {
                                        //التأكد من ان الصنف لم يكن له اى سيريال مسبقا 
                                        var itemSerial = db.ItemSerials.Where(x => !x.IsDeleted && x.Id == item.ItemSerialId).FirstOrDefault();
                                        if (itemSerial != null)
                                        {
                                            //السيريال موجود مسبقا
                                            itemSerial.SellInvoiceId = invoice.Id;
                                            itemSerial.CurrentStoreId = null;
                                            itemSerial.ExpirationDate = invoice.InvoiceDate.AddMonths(1);
                                            itemSerial.SerialCaseId = (int)SerialCaseCl.Sell;
                                            db.Entry(itemSerial).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                        generate:
                                            var serial = GeneratBarcodes.GenerateRandomBarcode();
                                            var isExists = db.ItemSerials.Where(x => x.SerialNumber == serial).Any();
                                            if (isExists)
                                                goto generate;
                                            itemSerial = new ItemSerial
                                            {
                                                ItemId = item.ItemId,
                                                ProductionOrderId = item.ProductionOrderId,
                                                IsItemIntial = item.IsItemIntial ? true : false,
                                                ExpirationDate = invoice.InvoiceDate.AddMonths(1),
                                                SerialCaseId = (int)SerialCaseCl.Sell,
                                                SerialNumber = serial,
                                                SellInvoiceId = invoice.Id
                                            };


                                        }

                                        //add item serial case history
                                        db.CasesItemSerialHistories.Add(new CasesItemSerialHistory
                                        {
                                            ItemSerial = itemSerial,
                                            ReferrenceId = invoice.Id,
                                            SerialCaseId = (int)SerialCaseCl.Sell
                                        });
                                        //تحديث سيريال الصنف الجديد 
                                        //item.ItemSerial = itemSerial;
                                        //context.Entry(item).State = EntityState.Modified;
                                    }

                                }
                            }



                        }

                        //===========================

                        //invoice.IsFinalApproval = true;
                    }
                    else
                    {
                        result.Message = "قيد المعاملة غير موزون";
                        return result;
                    }

                }
                else
                {
                    result.Message = "يجب تعريف الأكواد الحسابية فى شاشة الاعدادات";
                    return result;

                }
            }
            db.OfferSellInvoices.AddRange(GetOffers(invoice.Id));
            db.SaveChanges(UserServices.UserInfo.UserId);
            result.IsSuccessed = true;
            stopwatch.Stop();
            //362 milesec (3 items) local
            //660 milesec (12 items) local

            //50sec 520 milesec (12 items) remote
            //3 min 26sec 700 milesec (12 items) remote

            //2 min 32sec 320milesec (12 items) web remote
            //10 min 46 sec run script data in server (remote)

            return result;


        }
        #endregion
        #endregion

        #region Invoice Incomes and outcomes
        public List<InvoiceExpensesDT> GetInvoiceIncomes(Guid invoiceID)
        {
            var db = DBContext.UnitDbContext;
            {
                return db.SellInvoiceIncomes.Where(x => !x.IsDeleted && x.SellInvoiceId == invoiceID).Select(expense => new InvoiceExpensesDT
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
        public DtoResultObj<Item> ItemNameSearch(string nam)
        {
            var result = new DtoResultObj<Item>();

            //Not deleted items
            var items = db.Items.Where(x => !x.IsDeleted);

            //Search by name
            var item = items.FirstOrDefault(x => x.Name.Trim() == nam);
            if (item != null)
            {
                result.IsSuccessed = true;
                result.Object = item;
                return result;
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
        public DtoResultObj<List<ItemDetailsDT>> AddItemInInvoice(Guid invoiceID, SellInvoicesDetail itemDetail, bool mergeMatched = false, bool saleWithoutBalance = true)
        {
            var result = new DtoResultObj<List<ItemDetailsDT>>();

            //التأكد من ان الفاتورة صالحة
            var invoiceResult = CheckValidInvoice(invoiceID);
            if (!invoiceResult.IsSuccessed)
            {
                result.Message = invoiceResult.Message;
                return result;
            }
            if (saleWithoutBalance)
            {
                //التأكد من وجود كمية كافية من المنتج المطلوب
                DtoResult itemBalanceResult = CheckItemBalance(itemDetail);
                if (!itemBalanceResult.IsSuccessed)
                {
                    result.Message = itemBalanceResult.Message;
                    return result;
                }
            }

            if (itemDetail.Amount < itemDetail.ItemDiscount)
            {
                result.Message = "خطأ في الخصم";
                return result;
            }

            if (mergeMatched)
            {
                var existItemDetail = db.SellInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.SellInvoiceId == invoiceID && x.ItemId == itemDetail.ItemId && x.Amount / x.Quantity == itemDetail.Price && x.ItemDiscount / x.Quantity == itemDetail.ItemDiscount / itemDetail.Quantity);
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
            itemDetail.SellInvoiceId = invoiceID;
            db.SellInvoicesDetails.Add(itemDetail);
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
                return db.SellInvoicesDetails.Where(x => !x.IsDeleted && x.SellInvoiceId == invoiceID).Select(item => new ItemDetailsDT
                {
                    Id = item.Id,
                    ItemCode = item.Item.ItemCode,
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
            }
        }
        #endregion

        #region Balance
        public DtoResult CheckItemBalance(SellInvoicesDetail itemDetail)
        {
            return CheckItemBalance(itemDetail.ItemId, itemDetail.Quantity);
        }
        public DtoResult CheckItemBalance(Guid? itemID, double requestedQuantity)
        {
            double currentBalance = 0;
            var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
            var schema = generalSetting.Where(x => x.Id == (int)GeneralSettingCl.EntityDataSchema).FirstOrDefault().SValue;
            if (!string.IsNullOrEmpty(schema))
                currentBalance = db.Database.SqlQuery<double>($"select [{schema}].[GetBalance](@ItemId,@StoreId)", new SqlParameter("@ItemId", itemID), new SqlParameter("@StoreId", 1)).FirstOrDefault();
            else
                currentBalance = 0;
            return new DtoResult() { IsSuccessed = currentBalance >= requestedQuantity, Message = currentBalance >= requestedQuantity ? "" : "الكمية غير متوفرة" };
        }
        #endregion

        #region Delete Item from Invoice
        public DtoResult DeleteItem(Guid sellInvoiceDetailID)
        {
            var result = new DtoResult();
            var item = db.SellInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == sellInvoiceDetailID);
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
            var item = db.SellInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
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

        #region Invoice Offers
        public List<OfferSellInvoice> GetOffers(Guid invoiceId)
        {
            var result = new List<OfferSellInvoice>();
            var now = DALUtility.GetDateTime();
            var offerTypeIds = db.OfferTypes.Where(x => !x.IsDeleted).Select(x => x.Id).ToList();
            var itemsOfferTypeId = offerTypeIds[0];
            var invoiceOfferTypeId = offerTypeIds[1];

            var invoiceDetails = db.SellInvoicesDetails.Where(x => x.SellInvoiceId == invoiceId && !x.IsDeleted && !x.SellInvoice.IsDeleted).OrderBy(x => x.CreatedBy).Select(x => new InvoiceDetail
            {
                Id = x.Id,
                Quantity = x.Quantity,
                ItemId = x.ItemId,
                Amount = x.Amount
            }).ToList();
            var invoiceTotalAmount = invoiceDetails.Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            if (invoiceDetails.Count == 0 || invoiceTotalAmount == 0)  //اذا مكنش في اصناف في الفاتورة او اجمالي اصنافها بصفر
            {
                return result;
            }

            //العروض الخاصة بالفواتير
            var invoiceOffers = db.Offers.Include(x => x.OfferDetails).Where(x => !x.IsDeleted && x.StartDate <= now && now <= x.EndDate && x.OfferTypeId == invoiceOfferTypeId).ToList();
            foreach (var invoiceOffer in invoiceOffers)
            { //تطبيق كل العروض اللي علي الفواتير اللي قيمة الفاتورة فيها بتسواي اجمالي الاصناف او اكثر
                if (invoiceTotalAmount >= invoiceOffer.InvoiceAmountFrom)
                {
                    var discount = invoiceOffer.DiscountPercentage > 0 ? invoiceOffer.DiscountPercentage * invoiceTotalAmount / 100 : invoiceOffer.DiscountAmount;
                    result.Add(new OfferSellInvoice() { SellInvoiceId = invoiceId, OfferId = invoiceId, Offer = invoiceOffer, OfferDiscount = discount });
                }
            }

            //العروض الخاصة بالاصناف
            var offers = db.Offers.Include(x => x.OfferDetails).Where(x => !x.IsDeleted && x.StartDate <= now && now <= x.EndDate && x.OfferTypeId == itemsOfferTypeId).ToList();
            if (offers.Count == 0)
            {//اذا مكنش فيه عروض
                return result;
            }
            offers.ForEach(o => o.OfferDetails = o.OfferDetails.Where(x => !x.IsDeleted).ToList()); //لتأكيد انه لا يوجد اصناف عروض محذوفة

            //خذف كل الاصناف اللي مش موجودة في عروض
            foreach (var item in invoiceDetails.Where(x => !offers.SelectMany(y => y.OfferDetails).Where(y => !y.IsDeleted).Any(y => y.ItemId == x.ItemId && y.Quantity <= x.Quantity)).ToList())
            {
                invoiceDetails.Remove(item);
            }
            if (invoiceDetails.Count == 0)
            {
                //اذا مكنش في اصناف في الفاتورة تابع لعرض معين
                return result;
            }

            //حذف كل العروض اللي فيها اصناف مش موجودة في الفاتورة
            var offerIds = offers.SelectMany(x => x.OfferDetails).Where(x => !invoiceDetails.Any(y => y.ItemId == x.ItemId && x.Quantity <= y.Quantity)).Select(x => x.OfferId).ToList();
            offers.RemoveAll(x => offerIds.Contains(x.Id));
            if (offers.Count == 0)
            {//اذا مكنش فيه عروض
                return result;
            }

            foreach (var invoiceItem in invoiceDetails)
            {
                //حذف كل العروض اللي تخطي حد عدد مراته في الفاتورة الواحدة
                offers.RemoveAll(x => x.Limit <= result.Count(y => y.OfferId == x.Id));
                if (invoiceItem.Quantity == 0)
                    continue;

                //اياجاد كل العروض اللي الصنف ده موجود فيها
                //foreach (var offer in offers.Where(x => x.OfferDetails.Any(y => y.ItemId == invoiceItem.ItemId && y.Quantity <= invoiceItem.Quantity)))
                foreach (var offer in CheckOffers(offers, x => x.OfferDetails.Any(y => y.ItemId == invoiceItem.ItemId && y.Quantity <= invoiceItem.Quantity))) //don't add ToList() 
                {
                    while (true)
                    {
                        if(result.Count(y => y.OfferId == offer.Id) >= offer.Limit) 
                            break;

                        var canObtainOffer = true;
                        foreach (var offerItem in offer.OfferDetails)
                        {
                            //التأكد من ان كل اصناف العرض موجودة في الفاتورة بالكميات المحددة
                            if (!invoiceDetails.Any(x => x.ItemId == offerItem.ItemId && x.Quantity >= offerItem.Quantity))
                            {
                                canObtainOffer = false;
                                break;
                            }
                        }
                        if (!canObtainOffer)
                            break;

                        //اذا امكن الحصول علي العرض
                        foreach (var offerItem in offer.OfferDetails)
                        {
                            var item = invoiceDetails.FirstOrDefault(x => x.ItemId == offerItem.ItemId && x.Quantity >= offerItem.Quantity);
                            item.Quantity -= offerItem.Quantity;
                        }

                        var discount = offer.DiscountPercentage > 0 ? offer.DiscountPercentage * invoiceTotalAmount / 100 : offer.DiscountAmount;
                        result.Add(new OfferSellInvoice() { SellInvoiceId = invoiceId, Offer = offer, OfferId = offer.Id, OfferDiscount = discount });
                    }
                }
            }

            return result;
        }

        public IEnumerable<Offer> CheckOffers(List<Offer> offers, Predicate<Offer> match)
        {
            if (match == null || offers == null || offers.Count == 0)
                yield break;
            foreach (var item in offers)
            {
                if (match(item))
                {
                    yield return item;
                }
            }
        }

        public double GetOffersDiscount(Guid invoiceId)
        {
            return GetOffers(invoiceId).Select(x => x.OfferDiscount).DefaultIfEmpty(0).Sum();
        }

        private class InvoiceDetail
        {
            public Guid Id { get; set; }
            public double Quantity { get; set; }
            public Guid? ItemId { get; set; }
            public double Amount { get; internal set; }
        }
        #endregion
    }
}
