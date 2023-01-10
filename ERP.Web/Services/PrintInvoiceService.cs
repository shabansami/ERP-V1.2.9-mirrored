using ERP.DAL;
using ERP.Web.DataTablesDS;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public class PrintInvoiceService
    {
        public PrintInvoiceDto  GetInvoiceData(string id, string typ) {
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
                                ItemDetails = x.PurchaseInvoicesDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                InvoiceTitle = "فاتورة توريد",
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
                                ItemDetails = x.PurchaseBackInvoicesDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                InvoiceTitle = "فاتورة مرتجع توريد",
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
                                ItemDetails = x.SellInvoicesDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                InvoiceTitle = "فاتورة مبيعات",
                                PersonTypeName = "العميل",
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
                                ItemDetails = x.SellBackInvoicesDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                    ItemDiscount = i.ItemDiscount

                                }).ToList(),
                                InvoiceTitle = "فاتورة مرتجع مبيعات",
                                PersonTypeName = "العميل",
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
                    else if (typ == "Quote")
                    {
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
                                TotalQuantity = x.TotalQuantity,
                                TotalValue = x.TotalValue,
                                Safy = x.TotalValue,
                                ItemDetails = x.QuoteOrderSellDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                }).ToList(),
                                InvoiceTitle = "عرض سعر",
                                PersonTypeName = "العميل",
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
                                ItemDetails = x.QuoteOrderSellDetails.Where(i => !i.IsDeleted).Select(i => new ItemDetailsDT
                                {
                                    ItemName = i.Item.Name,
                                    Price = i.Price,
                                    Quantity = i.Quantity,
                                    Amount = i.Amount,
                                }).ToList(),
                                InvoiceTitle = "امر بيع",
                                PersonTypeName = "العميل",
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

        
    }
}