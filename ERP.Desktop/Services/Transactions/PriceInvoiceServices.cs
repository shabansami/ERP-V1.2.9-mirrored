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
using System.Windows.Forms;

namespace ERP.Desktop.Services.Transactions
{
    public class PriceInvoiceServices
    {
        VTSaleEntities db;
        public PriceInvoiceServices(VTSaleEntities dbContext)
        {
            db = dbContext;
        }

        #region AllInvoices 

        public List<PriceInvoiceVM> GetAllInvoices()
        {
            return db.QuoteOrderSells.Where(x => !x.IsDeleted).Select(x => new PriceInvoiceVM()
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                Date = x.CreatedOn,
                Total = x.TotalValue,
                Safy = (float)x.Safy,
            }).ToList();
        }

        /// <summary>
        /// get all invoices in shift and filter it
        /// </summary>
        /// <param name="shiftID">shift Id</param>
        /// <param name="searchText">text for searching</param>
        /// <param name="isOpenStatus">if get open invoices = <see langword="true"/> ,if gel closed invoices false,if all invoices set null</param>
        /// <returns>list of filtered invoice in the shift</returns>
        public List<PriceInvoiceVM> GetAllItemsFilter(string searchText)
        {
            var search = db.QuoteOrderSells.Where(x => !x.IsDeleted);
            if (searchText.Length > 2)
            {
                if (Guid.TryParse(searchText, out Guid id))
                {
                    search = search.Where(x => x.Id == id);
                }
                
                else if (DateTime.TryParse(searchText, out DateTime date))
                {
                    search = search.Where(x => x.InvoiceDate == date);
                }
                else
                {
                    search = search.Where(x => x.Notes.Contains(searchText));
                }
            }
            return search.Select(x => new PriceInvoiceVM()
            {
                Id = x.Id,
                Date = x.CreatedOn,
                Total = x.TotalValue,
                Safy = (float)x.Safy,
                InvoiceNumber = x.InvoiceNumber,
            }).ToList();
        }

        public List<InvoiceDetailVM> GetInvoiceDetails(Guid invoiceID)
        {
            return db.QuoteOrderSellDetails.Where(x => !x.IsDeleted && x.QuoteOrderSellId == invoiceID).Select(x => new InvoiceDetailVM()
            {
                Id = x.Id,
                ItemName = x.Item.Name,
                Amount = (float)x.Amount,
                Quantity = (float)x.Quantity
            }).ToList();
        }
        #endregion

        #region Invoice operations
        #region Create New
        public DtoResultObj<QuoteOrderSell> CreateInvoice(QuoteOrderSell model, bool isInvoiceDisVal = false)
        {
            try
            {
                if (model.InvoiceDate == null)
                    return new DtoResultObj<QuoteOrderSell> { IsSuccessed = false, Message = "تأكد من ادخال بيانات صحيحة" };

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
                model.InvoiceDiscount = invoiceDiscount;//إجمالي خصومات الفاتورة 
                model.TotalValue = 0;//إجمالي قيمة المبيعات 
                model.Safy = (model.TotalValue + model.SalesTax) - (model.InvoiceDiscount + model.ProfitTax);


                var db = DBContext.UnitDbContext;

                //اضافة رقم الفاتورة
                string codePrefix = Properties.Settings.Default.CodePrefix;
                model.InvoiceNumber = codePrefix + (db.QuoteOrderSells.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

                db.QuoteOrderSells.Add(model);
                if (db.SaveChanges(UserServices.UserInfo?.UserId) > 0)
                {
                    return new DtoResultObj<QuoteOrderSell> { IsSuccessed = true, Object = model, Message = "تم الاضافة بنجاح" };
                }
                else
                    return new DtoResultObj<QuoteOrderSell> { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };

            }
            catch (Exception ex)
            {
                return new DtoResultObj<QuoteOrderSell>() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
        }
        #endregion

        #region Load Invoice
        public DtoResultObj<QuoteOrderSell> GetInvoice(Guid invoiceID)
        {
            var invoice = db.QuoteOrderSells.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return new DtoResultObj<QuoteOrderSell> { IsSuccessed = invoice != null, Object = invoice, Message = invoice != null ? "" : "خطأ في رقم الفاتروة" };
        }
        #endregion


        #region Validations
        public DtoResult CheckValidInvoice(Guid invoiceID)
        {
            var isExist = db.QuoteOrderSells.Count(x => x.Id == invoiceID && !x.IsDeleted) > 0;
            return new DtoResult { IsSuccessed = isExist, Message = isExist ? "" : "لم يتم ايجاد الفاتورة" };
        }

        #endregion

        #region Calculations
        internal DtoResultObj<QuoteOrderSell> CalculateInvoiceTotals(Guid invoiceID)
        {
            var invoice = db.QuoteOrderSells.FirstOrDefault(x => x.Id == invoiceID && !x.IsDeleted);
            return CalculateInvoiceTotals(invoice);
        }

        public DtoResultObj<QuoteOrderSell> CalculateInvoiceTotals(QuoteOrderSell invoice)
        {
            var result = new DtoResultObj<QuoteOrderSell>();
            if (invoice == null)
            {
                result.Message = "الفاتورة غير موجودة";
                return result;
            }

            //صافى قيمة الفاتورة= ( إجمالي قيمة المبيعات + إجمالي قيمة الايرادات + قيمة الضريبة المضافة  - إجمالي خصومات الفاتورة - قيمة ض.أ.ت.ص )
            //فى حالة الخصم على الفاتورة نسية /قيمة 

            invoice.TotalValue = invoice.QuoteOrderSellDetails.Where(x => !x.IsDeleted).Sum(x => x.Amount);//إجمالي قيمة المبيعات 
            invoice.TotalQuantity = invoice.QuoteOrderSellDetails.Where(x => !x.IsDeleted).Sum(x => x.Quantity);


            double invoiceDiscount = invoice.InvoiceDiscount;
            if (invoice.DiscountPercentage != 0)
            {
                invoiceDiscount = (invoice.TotalValue * invoice.DiscountPercentage) / 100;
            }
            //invoice.InvoiceDiscount = invoice.QuoteOrderSellDetails.Where(x => !x.IsDeleted).Sum(x => x.ItemDiscount) + invoiceDiscount;//إجمالي خصومات الفاتورة 

            invoice.Safy = (invoice.TotalValue + invoice.SalesTax) - (invoice.InvoiceDiscount + invoice.ProfitTax);

            invoice.InvoiceDiscount = invoiceDiscount;

            db.SaveChanges(UserServices.UserInfo.UserId);
            result.Object = invoice;
            result.IsSuccessed = true;

            return result;
        }
        #endregion
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
        /// <param name="invoiceID">QuoteOrderSell id</param>
        /// <param name="itemDetail">Item detials</param>
        /// <param name="mergeMatched">is merge matched items if exist</param>
        /// <returns>result contains operation success status and message and list of items in the invoice</returns>
        public DtoResultObj<List<ItemDetailsDT>> AddItemInInvoice(Guid invoiceID, QuoteOrderSellDetail itemDetail, bool mergeMatched = false, bool saleWithoutBalance = true)
        {
            var result = new DtoResultObj<List<ItemDetailsDT>>();

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

            //if (itemDetail.Amount < itemDetail.ItemDiscount)
            //{
            //    result.Message = "خطأ في الخصم";
            //    return result;
            //}

            if (mergeMatched)
            {
                var existItemDetail = db.QuoteOrderSellDetails.FirstOrDefault(x => !x.IsDeleted && x.QuoteOrderSellId == invoiceID && x.ItemId == itemDetail.ItemId && x.Amount / x.Quantity == itemDetail.Price /*&& x.ItemDiscount / x.Quantity == itemDetail.ItemDiscount / itemDetail.Quantity*/);
                if (existItemDetail != null)
                {
                    existItemDetail.Amount += itemDetail.Amount;
                    existItemDetail.Quantity += itemDetail.Quantity;
                    //existItemDetail.ItemDiscount += itemDetail.ItemDiscount;
                    result.IsSuccessed = db.SaveChanges(UserServices.UserInfo?.UserId) > 0;
                    result.Object = GetInvoiceItems(invoiceID);

                    return result;
                }
            }
            itemDetail.QuoteOrderSellId = invoiceID;
            db.QuoteOrderSellDetails.Add(itemDetail);
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
                return db.QuoteOrderSellDetails.Where(x => !x.IsDeleted && x.QuoteOrderSellId == invoiceID).Select(item => new ItemDetailsDT
                {
                    Id = item.Id,
                    Amount = item.Amount,
                    ItemId = item.ItemId,
                    //ItemDiscount = item.ItemDiscount,
                    ItemName = item.Item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                }).ToList();
            }
        }
        #endregion

        #region Balance
        public DtoResult CheckItemBalance(QuoteOrderSellDetail itemDetail)
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
        public DtoResult DeleteItem(Guid QuoteOrderSellDetailID)
        {
            var result = new DtoResult();
            var item = db.QuoteOrderSellDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == QuoteOrderSellDetailID);
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
            var item = db.QuoteOrderSellDetails.FirstOrDefault(x => !x.IsDeleted && x.Id == id);
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
