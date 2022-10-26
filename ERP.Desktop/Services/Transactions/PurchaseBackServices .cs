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

namespace ERP.Desktop.Services.Transactions
{
    public class PurchaseBackServices
    {
        VTSaleEntities db;
        public PurchaseBackServices(VTSaleEntities dbContext)
        {
            db = dbContext;
        }
        #region AllInvoices 

        public List<ShiftInvoiceVM> GetAllInvoices(Guid ShiftID)
        {
            return db.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.ShiftOfflineID == ShiftID).Select(x => new ShiftInvoiceVM()
            {
                Id = x.Id,
                InvoiceNumPaper = "",
                InvoKind = "مبيعات",
                Safy = (float)x.Safy,
                InvoiceNumber = x.InvoiceNumber,
                ClintName = x.PersonSupplier.Name,
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
            var search = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.ShiftOfflineID == shiftID);
            if (isOpenStatus != null)
            {
                search = search.Where(x => x.IsFinalApproval == !isOpenStatus);
            }
            if (searchText.Length > 2)
            {
                if (Guid.TryParse(searchText, out Guid id))
                {
                    search = search.Where(x => x.Id == id || x.SupplierId == id);
                }
                else if (Guid.TryParse(searchText, out Guid guid))
                {
                    search = search.Where(x => x.Id == guid);
                }
                else
                {
                    search = search.Where(x => x.PersonSupplier.Name.Contains(searchText) || x.Notes.Contains(searchText));
                }
            }
            return search.Select(x => new ShiftInvoiceVM()
            {
                Id = x.Id,
                InvoiceNumPaper = "",
                InvoKind = "مبيعات",
                Safy = (float)x.Safy,
                InvoiceNumber = x.InvoiceNumber,
                ClintName = x.PersonSupplier.Name,
                IsFinalApproved = x.IsFinalApproval,
                Status = x.IsFinalApproval ? "مغلقة" : "مفتوحة"
            }).ToList();
        }

        public List<InvoiceDetailVM> GetInvoiceDetails(Guid invoiceID)
        {
            return db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == invoiceID).Select(x => new InvoiceDetailVM()
            {
                Id = x.Id,
                ItemName = x.Item.Name,
                Amount = (float)x.Amount,
                Quantity = (float)x.Quantity
            }).ToList();
        }
        public DtoResultObj<PurchaseBackInvoice> OpenAnInvoice(Guid invoiceID)
        {
            var result = new DtoResultObj<PurchaseBackInvoice>();
            var invoice = db.PurchaseBackInvoices.FirstOrDefault(x => !x.IsDeleted && x.Id == invoiceID);
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
        public DtoResultObj<PurchaseBackInvoice> CreateInvoice(PurchaseBackInvoice model, bool isInvoiceDisVal = false)
        {
            try
            {
                if (model.SupplierId == null || model.BranchId == null || model.InvoiceDate == null || model.PaymentTypeId == null)
                    return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = false, Message = "تأكد من ادخال بيانات صحيحة" };

                if (model.PaymentTypeId == (int)PaymentTypeCl.Deferred) //ف حالة السداد آجل
                {
                    if (model.PayedValue > 0)
                        return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = false, Message = "تم استلام مبلغ من العميل وحالة السداد آجل " };

                    model.BankAccountId = null;
                    model.SafeId = null;
                }
                else  // ف حالة السداد نقدى او جزئى
                {
                    if (model.BankAccountId == null && model.SafeId == null)
                        return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = false, Message = "تأكد من اختيار طريقة السداد (بنكى-خزنة) بشكل صحيح" };
                }


                model.InvoiceDate = CommonMethods.TimeNow;

                //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
                //فى حالة الخصم على الفاتورة نسية /قيمة 


                model.TotalQuantity = 0;
                model.TotalDiscount = 0;//إجمالي خصومات الفاتورة 
                model.TotalValue = 0;//إجمالي قيمة المبيعات 
                model.TotalExpenses = 0;//إجمالي قيمة الايرادادت
                model.Safy = (model.TotalValue + model.TotalExpenses + model.SalesTax) - (model.TotalDiscount + model.ProfitTax);

                //فى حالة السداد نقدى ولم يتم دفع كامل المبلغ
                if (model.PaymentTypeId == (int)PaymentTypeCl.Cash && model.PayedValue != model.Safy)
                    return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = false, Message = "يجب دفع كامل المبلغ (طريقة السداد نقدى)" };

                model.CaseId = (int)CasesCl.InvoiceCreated;

                //اضافة رقم الفاتورة
                string codePrefix = Properties.Settings.Default.CodePrefix;
                model.InvoiceNumber = codePrefix + (db.PurchaseBackInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                //اضافة الحالة 
                db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                {
                    PurchaseBackInvoice = model,
                    IsPurchaseInvoice = true,
                    CaseId = (int)CasesCl.InvoiceCreated
                });
                db.PurchaseBackInvoices.Add(model);
                if (db.SaveChanges(UserServices.UserInfo?.UserId) > 0)
                {
                    return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = true, Object = model, Message = "تم الاضافة بنجاح" };
                }
                else
                    return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };


            }
            catch (Exception ex)
            {
                return new DtoResultObj<PurchaseBackInvoice>() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
        }
        #endregion

        #region Load Invoice
        public DtoResultObj<PurchaseBackInvoice> GetInvoice(Guid invoiceID)
        {
            var invoice = db.PurchaseBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return new DtoResultObj<PurchaseBackInvoice> { IsSuccessed = invoice != null, Object = invoice, Message = invoice != null ? "" : "خطأ في رقم الفاتروة" };
        }
        #endregion

        #region Validations
        public DtoResultObj<PurchaseBackInvoice> CheckValidInvoice(Guid invoiceID)
        {
            var invoice = db.PurchaseBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CheckValidInvoice(invoice);
        }

        public DtoResultObj<PurchaseBackInvoice> CheckValidInvoice(PurchaseBackInvoice invoice)
        {
            var result = new DtoResultObj<PurchaseBackInvoice>();
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
        internal DtoResultObj<PurchaseBackInvoice> CalculateInvoiceTotals(Guid invoiceID)
        {
            var invoice = db.PurchaseBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CalculateInvoiceTotals(invoice);
        }

        public DtoResultObj<PurchaseBackInvoice> CalculateInvoiceTotals(PurchaseBackInvoice invoice)
        {
            var result = new DtoResultObj<PurchaseBackInvoice>();
            if (invoice == null)
            {
                result.Message = "الفاتورة غير موجودة";
                return result;
            }

            //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
            //فى حالة الخصم على الفاتورة نسية /قيمة 

            invoice.TotalValue = invoice.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Amount);//إجمالي قيمة المبيعات 
            invoice.TotalQuantity = invoice.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.Quantity);


            invoice.TotalDiscount = invoice.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted).Sum(x => x.ItemDiscount) + invoice.InvoiceDiscount;//إجمالي خصومات الفاتورة 

            invoice.TotalExpenses = db.PurchaseBackInvoicesExpenses.Where(x => x.PurchaseBackInvoiceId == invoice.Id && !x.IsDeleted).Select(x => x.Amount).DefaultIfEmpty(0).Sum();//إجمالي قيمة الايرادادت

            invoice.Safy = (invoice.TotalValue + invoice.TotalExpenses + invoice.SalesTax) - (invoice.TotalDiscount + invoice.ProfitTax);



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
            var invoice = db.PurchaseBackInvoices.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CloseInvoice(invoice);
        }

        public DtoResult CloseInvoice(PurchaseBackInvoice invoice)
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

            if (invoice.SupplierId is null)
            {
                result.Message = "الرجاء اختيار المورد";
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
            //الاعتماد النهائى 
            if (approvalAfterSave == "1")
            {
                //الاعتماد المحاسبى 
                invoice.IsApprovalAccountant = true;
                invoice.CaseId = (int)CasesCl.InvoiceApprovalAccountant;
                //اضافة الحالة 
                db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                {
                    PurchaseBackInvoice = invoice,
                    IsPurchaseInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalAccountant
                });
                //الاعتماد المخزنى
                invoice.CaseId = (int)CasesCl.InvoiceApprovalStore;
                invoice.IsApprovalStore = true;
                //اضافة الحالة 
                db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                {
                    PurchaseBackInvoice = invoice,
                    IsPurchaseInvoice = true,
                    CaseId = (int)CasesCl.InvoiceApprovalStore
                });
                //    //تسجيل القيود
                // General Dailies
                if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                {
                    //صافى قيمة الفاتورة= ( إجمالي قيمة المشتريات + إجمالي قيمة المصروفات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )

                    double debit = 0;
                    double credit = 0;
                    // الحصول على حسابات من الاعدادات
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                    //التأكد من عدم وجود حساب فرعى من الحساب
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue)))
                    {
                        result.Message = "حساب المشتريات ليس بحساب فرعى";
                        return result;
                    }
                    if (AccountTreeService.CheckAccountTreeIdHasChilds(db.Persons.Where(x => x.Id == invoice.SupplierId).FirstOrDefault().AccountTreeSupplierId))
                    {
                        result.Message = "حساب المورد ليس بحساب فرعى";
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
                    var expenses = db.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == invoice.Id);

                    if (expenses.Count() > 0)
                    {
                        if (AccountTreeService.CheckAccountTreeIdHasChilds(expenses.FirstOrDefault().ExpenseTypeAccountsTreeID))
                        {
                            result.Message = "حساب المصروفات ليس بحساب فرعى";
                            return result;
                        }
                    }

                    // حساب مرتجع المشتريات

                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue),
                        BranchId = invoice.BranchId,
                        Credit = invoice.TotalValue,
                        Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                        TransactionDate = invoice.InvoiceDate,
                        TransactionId = invoice.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                    });
                    credit = credit + invoice.TotalValue;
                    // حساب المورد
                    db.GeneralDailies.Add(new GeneralDaily
                    {
                        AccountsTreeId = db.Persons.Where(x => x.Id == invoice.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                        BranchId = invoice.BranchId,
                        Debit = invoice.Safy,
                        Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                        TransactionDate = invoice.InvoiceDate,
                        TransactionId = invoice.Id,
                        TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                    });
                    debit = debit + invoice.Safy;

                    // القيمة المضافة
                    if (invoice.SalesTax > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Credit = invoice.SalesTax,
                            Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                        });
                        credit = credit + invoice.SalesTax;


                    }
                    // الخصومات
                    if (invoice.TotalDiscount > 0)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue),
                            BranchId = invoice.BranchId,
                            Debit = invoice.TotalDiscount,
                            Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
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
                            Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                        });
                        debit = debit + invoice.ProfitTax;
                    }

                    //  المبلغ المدفوع من حساب المورد (مدين
                    if (invoice.PayedValue > 0 && invoice.PaymentTypeId != (int)PaymentTypeCl.Deferred)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = db.Persons.Where(x => x.Id == invoice.SupplierId).FirstOrDefault().AccountTreeSupplierId,
                            BranchId = invoice.BranchId,
                            Credit = invoice.PayedValue,
                            Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                        });
                        credit = credit + invoice.PayedValue;

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
                                Debit = invoice.PayedValue,
                                Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                                TransactionDate = invoice.InvoiceDate,
                                TransactionId = invoice.Id,
                                TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                            });
                            debit = debit + invoice.PayedValue;
                        }

                    }

                    // المصروفات (مدين
                    foreach (var expense in expenses)
                    {
                        db.GeneralDailies.Add(new GeneralDaily
                        {
                            AccountsTreeId = expense.ExpenseTypeAccountsTreeID,
                            //AccountsTreeId = expense.ExpenseType.AccountsTreeId,
                            BranchId = invoice.BranchId,
                            Credit = expense.Amount,
                            Notes = $"فاتورة مرتجع توريد رقم : {invoice.InvoiceNumber}",
                            TransactionDate = invoice.InvoiceDate,
                            TransactionId = invoice.Id,
                            TransactionTypeId = (int)TransactionsTypesCl.PurchasesReturn
                        });
                        credit = credit + expense.Amount;

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
                        db.CasesPurchaseInvoiceHistories.Add(new CasesPurchaseInvoiceHistory
                        {
                            PurchaseBackInvoice = invoice,
                            IsPurchaseInvoice = false,
                            CaseId = (int)CasesCl.BackInvoiceFinalApproval
                        });
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
        public List<InvoiceExpensesDT> GetInvoiceExpenses(Guid invoiceID)
        {
            return db.PurchaseBackInvoicesExpenses.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == invoiceID).Select(expense => new InvoiceExpensesDT
            {
                ExpenseAmount = expense.Amount,
                ExpenseTypeId = expense.ExpenseTypeAccountsTreeID,
                ExpenseTypeName = expense.ExpenseTypeAccountsTree.AccountName
            }).ToList();
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
        public DtoResultObj<List<ItemDetailsDT>> AddItemInInvoice(Guid invoiceID, PurchaseBackInvoicesDetail itemDetail, bool mergeMatched = false)
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
                var existItemDetail = db.PurchaseBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.PurchaseBackInvoiceId == invoiceID && x.ItemId == itemDetail.ItemId && x.Amount / x.Quantity == itemDetail.Price && x.ItemDiscount / x.Quantity == itemDetail.ItemDiscount / itemDetail.Quantity);
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
            itemDetail.PurchaseBackInvoiceId = invoiceID;
            db.PurchaseBackInvoicesDetails.Add(itemDetail);
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
                return db.PurchaseBackInvoicesDetails.Where(x => !x.IsDeleted && x.PurchaseBackInvoiceId == invoiceID).Select(item => new ItemDetailsDT
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
            }
        }
        #endregion

        #region Delete Item from Invoice
        public DtoResult DeleteItem(Guid PurchaseBackInvoiceDetailID)
        {
            var result = new DtoResult();
            var item = db.PurchaseBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == PurchaseBackInvoiceDetailID);
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
            var item = db.PurchaseBackInvoicesDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
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
