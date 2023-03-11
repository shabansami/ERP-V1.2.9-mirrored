using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public class PrintInvoiceService
    {
        public PrintInvoiceDto  GetInvoiceData(string id, string typ,string lang) {
            PrintInvoiceDto invoice = new PrintInvoiceDto();
            // الحصول على بيانات المؤسسة من الاعدادات
            CustomerService personService = new CustomerService();
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                using (var db = new VTSaleEntities())
                {
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                    if (typ == "purchase")
                    {
                        var purchase = db.PurchaseInvoices.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (purchase.Count() > 0)
                        {
                            invoice = purchase.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                TotalIncomExpenses = x.TotalExpenses,
                                PayedValue = x.PayedValue,
                                RemindValue = x.RemindValue,
                                SafeBankName = x.Safe != null ? x.Safe.Name : x.BankAccount != null ? x.BankAccount.AccountName : string.Empty,
                                PaymentTypeName = x.PaymentType != null ? x.PaymentType.Name : string.Empty,
                                SuppCustomerName = x.PersonSupplier.Name,
                                TotalDiscount = x.TotalDiscount,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.Safy,
                                PersonBalance = personService.GetPersonBalance(x.PersonSupplier.AccountTreeSupplierId),
                                SalesTax = x.SalesTax,
                                ProfitTax = x.ProfitTax,
                                InvoiceExpenses = x.PurchaseInvoicesExpenses.Where(e => !e.IsDeleted).Select(e => new InvoiceExpensesDT
                                {
                                    ExpenseTypeName = e.ExpenseTypeAccountTree.AccountName,
                                    ExpenseAmount = e.Amount
                                }).ToList(),
                                ItemDetails = x.PurchaseInvoicesDetails.Where(i => !i.IsDeleted).OrderBy(i=>i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount
                                    
                                }).ToList(),
                                RptTitle = "فاتورة توريد",
                                PersonTypeName="المورد",
                                Notes=x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"المورد : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }
                    else if (typ == "purchaseBack")
                    {
                        var purchaseBack = db.PurchaseBackInvoices.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (purchaseBack.Count() > 0)
                        {
                            invoice = purchaseBack.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                TotalIncomExpenses = x.TotalExpenses,
                                PayedValue = x.PayedValue,
                                RemindValue = x.RemindValue,
                                SafeBankName = x.Safe != null ? x.Safe.Name : x.BankAccount != null ? x.BankAccount.AccountName : string.Empty,
                                PaymentTypeName = x.PaymentType != null ? x.PaymentType.Name : string.Empty,
                                SuppCustomerName = x.PersonSupplier.Name,
                                TotalDiscount = x.TotalDiscount,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.Safy,
                                PersonBalance = personService.GetPersonBalance(x.PersonSupplier.AccountTreeSupplierId),
                                SalesTax = x.SalesTax,
                                ProfitTax = x.ProfitTax,
                                InvoiceExpenses = x.PurchaseBackInvoicesExpenses.Where(e => !e.IsDeleted).Select(e => new InvoiceExpensesDT
                                {
                                    ExpenseTypeName = e.ExpenseTypeAccountsTree.AccountName,
                                    ExpenseAmount = e.Amount
                                }).ToList(),
                                ItemDetails = x.PurchaseBackInvoicesDetails.Where(i => !i.IsDeleted).OrderBy(i => i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                RptTitle = "فاتورة مرتجع توريد",
                                PersonTypeName = "المورد",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"المورد : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }
                    else if (typ == "sell")
                    {
                        var sell = db.SellInvoices.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (sell.Count() > 0)
                        {
                            invoice = sell.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                TotalIncomExpenses = x.TotalExpenses,
                                PayedValue = x.PayedValue,
                                RemindValue = x.RemindValue,
                                SafeBankName = x.Safe != null ? x.Safe.Name : x.BankAccount != null ? x.BankAccount.AccountName : string.Empty,
                                PaymentTypeName = x.PaymentType != null ? x.PaymentType.Name : string.Empty,
                                SuppCustomerName = x.PersonCustomer.Name,
                                SuppCustomerAddress = x.PersonCustomer.Address,
                                SuppCustomerAddress2 = x.PersonCustomer.Address2,
                                SuppCustomerCommercialRegisterNo = x.PersonCustomer.CommercialRegistrationNo,
                                SuppCustomerTaxNo = x.PersonCustomer.TaxNumber,
                                TotalDiscount = x.TotalDiscount,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.Safy,
                                PersonBalance = personService.GetPersonBalance(x.PersonCustomer.AccountsTreeCustomerId),
                                SalesTax = x.SalesTax,
                                ProfitTax = x.ProfitTax,
                                InvoiceExpenses = x.SellInvoiceIncomes.Where(e => !e.IsDeleted).Select(e => new InvoiceExpensesDT
                                {
                                    ExpenseTypeName = e.IncomeTypeAccountTree.AccountName,
                                    ExpenseAmount = e.Amount
                                }).ToList(),
                                ItemDetails = x.SellInvoicesDetails.Where(i => !i.IsDeleted).OrderBy(i=>i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount,
                                    UnitId= i.UnitId,
                                    QuantityUnit= i.QuantityUnit,
                                    QuantityUnitName=i.UnitId!=null? $"{i.QuantityUnit}{i.Unit?.Name}":null

                                }).ToList(),
                                RptTitle =lang=="en"?"Sell Invoice":"فاتورة مبيعات",
                                PersonTypeName =lang=="en"?"customer Name":"العميل",
                                Notes = x.Notes,
                                CustmerTaxNumber=x.PersonCustomer.TaxNumber,
                                CustomerCommercialRegistrationNo=x.PersonCustomer.CommercialRegistrationNo,
                                ApprovalAccounting=x.IsApprovalAccountant?"معتمدة محاسبيا":"غير معتمدة بعد",
                                SaleMenName=x.Employee!=null?x.Employee?.Person.Name:null,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"العميل : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }
                    else if (typ == "sellBack")
                    {
                        var sellBack = db.SellBackInvoices.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (sellBack.Count() > 0)
                        {
                            invoice = sellBack.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                TotalIncomExpenses = x.TotalExpenses,
                                PayedValue = x.PayedValue,
                                RemindValue = x.RemindValue,
                                SafeBankName = x.Safe != null ? x.Safe.Name : x.BankAccount != null ? x.BankAccount.AccountName : string.Empty,
                                PaymentTypeName = x.PaymentType != null ? x.PaymentType.Name : string.Empty,
                                SuppCustomerName = x.PersonCustomer.Name,
                                TotalDiscount = x.TotalDiscount,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.Safy,
                                PersonBalance = personService.GetPersonBalance(x.PersonCustomer.AccountsTreeCustomerId),
                                SalesTax = x.SalesTax,
                                ProfitTax = x.ProfitTax,
                                InvoiceExpenses = x.SellBackInvoiceIncomes.Where(e => !e.IsDeleted).Select(e => new InvoiceExpensesDT
                                {
                                    ExpenseTypeName = e.IncomeTypeAccountTree.AccountName,
                                    ExpenseAmount = e.Amount
                                }).ToList(),
                                ItemDetails = x.SellBackInvoicesDetails.Where(i => !i.IsDeleted).OrderBy(i => i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                RptTitle =lang=="en"?"Sell Back Invoice":"فاتورة مرتجع مبيعات",
                                PersonTypeName = lang == "en" ?"customer Name"  : "العميل",
                                Notes = x.Notes,
                                CustmerTaxNumber = x.PersonCustomer.TaxNumber,
                                CustomerCommercialRegistrationNo = x.PersonCustomer.CommercialRegistrationNo,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"العميل : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }       
                    else if (typ == "Quote")
                    {
                        var generalNoteAr=db.GeneralSettings.Where(x=>x.Id==(int)GeneralSettingCl.QuotationNoteAr).FirstOrDefault()?.SValue;
                        var generalNoteEn=db.GeneralSettings.Where(x=>x.Id==(int)GeneralSettingCl.QuotationNoteEn).FirstOrDefault()?.SValue;
                        var quote = db.QuoteOrderSells.Where(x => !x.IsDeleted && x.Id == Id&&x.QuoteOrderSellType==(int)QuoteOrderSellTypeCl.Qoute).ToList();
                        if (quote.Count() > 0)
                        {
                            invoice = quote.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                SuppCustomerName = x.Customer.Name,
                                TotalDiscount = x.InvoiceDiscount,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.Safy,
                                SalesTax = x.SalesTax,
                                ProfitTax = x.ProfitTax,
                                ItemDetails = x.QuoteOrderSellDetails.Where(i => !i.IsDeleted).OrderBy(i => i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    UnitId = i.UnitId,
                                    QuantityUnit = i.QuantityUnit,
                                    QuantityUnitName = i.UnitId != null ? $"{i.QuantityUnit}{i.Unit?.Name}" : null
                                }).ToList(),
                                RptTitle = lang == "en" ? "Quotation" : "عرض سعر",
                                PersonTypeName = lang == "en" ? "customer Name" : "العميل",
                                Notes = lang == "en"?!string.IsNullOrEmpty(x.Notes)?x.Notes: generalNoteEn: !string.IsNullOrEmpty(x.Notes) ? x.Notes : generalNoteAr,
                                CustmerTaxNumber = x.Customer.TaxNumber,
                                CustomerCommercialRegistrationNo = x.Customer.CommercialRegistrationNo,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"العميل : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }
                    else if (typ == "OrderSell")
                    {
                        var quote = db.QuoteOrderSells.Where(x => !x.IsDeleted && x.Id == Id&&x.QuoteOrderSellType==(int)QuoteOrderSellTypeCl.OrderSell).ToList();
                        if (quote.Count() > 0)
                        {
                            invoice = quote.Select(x => new PrintInvoiceDto
                            {
                                Id = x.Id,
                                InvoiceNumber = x.InvoiceNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                InvoiceDate = x.InvoiceDate.ToString(),
                                SuppCustomerName = x.Customer.Name,
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.TotalValue,
                                ItemDetails = x.QuoteOrderSellDetails.Where(i => !i.IsDeleted).OrderBy(i => i.CreatedOn).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                }).ToList(),
                                RptTitle = lang == "en" ? "Sell Order" : "امر بيع",
                                PersonTypeName = lang == "en" ? "customer Name" : "العميل",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            if (invoice != null)
                            {
                                var qrSring = $"العميل : {invoice.SuppCustomerName},الرقم الضريبى {invoice.EntityDataTaxCardNo},تاريخ الفاتورة :{invoice.InvoiceDate},قيمة الضريبة :{invoice.SalesTax},صافى الفاتورة :{invoice.Safy}";
                                invoice.QRCode = Utility.ConvertToQRCode(qrSring);
                                //invoice.Logo = ' + localStorage.getItem("logo")+';
                            }


                        }
                    }
                }
                

            }
            return invoice;


        }
        public PrintGeneralRecordDto PrintGeneralRecordData(string id, string typ) {
            PrintGeneralRecordDto voucher = new PrintGeneralRecordDto();
            // الحصول على بيانات المؤسسة من الاعدادات
            CustomerService personService = new CustomerService();
            Guid Id;
            if (Guid.TryParse(id, out Id))
            {
                using (var db = new VTSaleEntities())
                {
                    var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                    if (typ == "voucherPayment")
                    {
                        var vouchers = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (vouchers.Count() > 0)
                        {
                            voucher = vouchers.Select(x => new PrintGeneralRecordDto
                            {
                                InvoiceNumber = x.VoucherNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                OperationDate = x.VoucherDate.ToString(),
                                Amount = x.VoucherDetails.Where(v=>!v.IsDeleted).Sum(v=>(double?)v.Amount??0),
                                IsDebitFirstRpt = false,
                                AccountCreditList = new List<AccountDetails>()
                                {new AccountDetails(){
                                    AccountName = x.AccountsTree.AccountName,
                                    Amount = x.VoucherDetails.Where(v => !v.IsDeleted).Sum(v => (double?)v.Amount ?? 0),
                                }},
                                AccountDebitList = x.VoucherDetails.Where(i => !i.IsDeleted).Select(i => new AccountDetails()
                                {
                                    AccountName = i.AccountsTree.AccountName,
                                    Amount = i.Amount,
                                }).ToList(),

                                RptTitle = "سند صرف",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            
                        }
                    }
                    else if (typ == "voucherReceipt")
                    {
                        var vouchers = db.Vouchers.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (vouchers.Count() > 0)
                        {
                            voucher = vouchers.Select(x => new PrintGeneralRecordDto
                            {
                                InvoiceNumber = x.VoucherNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                OperationDate = x.VoucherDate.ToString(),
                                Amount = x.VoucherDetails.Where(v=>!v.IsDeleted).Sum(v=>(double?)v.Amount??0),
                                IsDebitFirstRpt = true,
                                AccountDebitList = new List<AccountDetails>()
                                {new AccountDetails(){
                                    AccountName = x.AccountsTree.AccountName,
                                    Amount = x.VoucherDetails.Where(v => !v.IsDeleted).Sum(v => (double?)v.Amount ?? 0),
                                }},
                                AccountCreditList = x.VoucherDetails.Where(i => !i.IsDeleted).Select(i => new AccountDetails()
                                {
                                    AccountName = i.AccountsTree.AccountName,
                                    Amount = i.Amount,
                                }).ToList(),

                                RptTitle = "سند قبض",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            
                        }
                    }
                    else if (typ == "generalRecord")
                    {
                        var generalRecords = db.GeneralRecords.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (generalRecords.Count() > 0)
                        {
                            voucher = generalRecords.Select(x => new PrintGeneralRecordDto
                            {
                                InvoiceNumber = x.GeneralRecordNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                OperationDate = x.TransactionDate.ToString(),
                                Amount = x.GeneralRecordDetails.Where(v=>!v.IsDeleted&&v.IsDebit).Sum(v=>(double?)v.Amount??0),
                                IsDebitFirstRpt = true,
                                AccountDebitList = x.GeneralRecordDetails.Where(i => !i.IsDeleted&&i.IsDebit).Select(i => new AccountDetails()
                                {
                                    AccountName = i.AccountTree.AccountName,
                                    Amount = i.Amount,
                                }).ToList(), 
                                AccountCreditList = x.GeneralRecordDetails.Where(i => !i.IsDeleted&&!i.IsDebit).Select(i => new AccountDetails()
                                {
                                    AccountName = i.AccountTree.AccountName,
                                    Amount = i.Amount,
                                }).ToList(),
                                RptTitle = "قيود يومية",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            
                        }
                    }
                    else if (typ == "expense")
                    {
                        var expenses = db.ExpenseIncomes.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (expenses.Count() > 0)
                        {
                            voucher = expenses.Select(x => new PrintGeneralRecordDto
                            {
                                InvoiceNumber = x.OperationNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                OperationDate = x.PaymentDate.ToString(),
                                Amount = x.Amount,
                                IsDebitFirstRpt = true,
                                AccountDebitList =new List<AccountDetails>(){new AccountDetails()
                                {
                                    AccountName = x.ExpenseIncomeTypeAccountsTree.AccountName,
                                    Amount = x.Amount
                                }},                             
                                AccountCreditList = new List<AccountDetails>(){new AccountDetails()
                                {
                                    AccountName = x.Safe.Name,
                                    Amount = x.Amount
                                }},
                                RptTitle = "مصروف",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            
                        }
                    }
                    else if (typ == "income")
                    {
                        var expenses = db.ExpenseIncomes.Where(x => !x.IsDeleted && x.Id == Id).ToList();
                        if (expenses.Count() > 0)
                        {
                            voucher = expenses.Select(x => new PrintGeneralRecordDto
                            {
                                InvoiceNumber = x.OperationNumber,
                                BranchName = x.Branch != null ? x.Branch.Name : string.Empty,
                                OperationDate = x.PaymentDate.ToString(),
                                Amount = x.Amount,
                                IsDebitFirstRpt = false,
                                AccountCreditList = new List<AccountDetails>(){new AccountDetails()
                                {
                                    AccountName = x.ExpenseIncomeTypeAccountsTree.AccountName,
                                    Amount = x.Amount
                                }},                             
                                AccountDebitList = new List<AccountDetails>(){new AccountDetails()
                                {
                                    AccountName = x.Safe.Name,
                                    Amount = x.Amount
                                }},
                                RptTitle = "ايراد",
                                Notes = x.Notes,
                                //بيانات الجهة
                                EntityDataName = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue : string.Empty,
                                EntityCommercialRegisterNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue : string.Empty,
                                EntityDataTaxCardNo = generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue != null ? generalSetting.Where(g => g.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue : string.Empty,
                            }).FirstOrDefault();
                            
                        }
                    }
                }
                

            }
            return voucher;


        }

        
    }
}