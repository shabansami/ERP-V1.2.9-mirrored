using ERP.Desktop.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.DAL;
using System.Threading.Tasks;
using ERP.Desktop.ViewModels;
using ERP.Web.Utilites;
using System.Data.OleDb;
using System.Data;
using static ERP.Web.Utilites.Lookups;
using System.Windows.Controls;

namespace ERP.Desktop.Services.Items
{
    public class ItemsServices
    {
        VTSaleEntities db;

        public ItemsServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public List<ItemsVM> GetAllItems()
        {
            var item = db.Items.Where(x => !x.IsDeleted).Select(x => new ItemsVM()
            {
                Id = x.Id,
                Name = x.Name,
                GroupBasicId = x.GroupBasicId,
                GroupBasicName = x.GroupBasic.Name,
                GroupSellId = x.GroupSellId.ToString(),
                GroupSellName = x.GroupSell.Name.ToString(),
                UnitId = x.UnitId,
                UnitName = x.Unit.Name,
                ItemTypeId = x.ItemTypeId,
                ItemTypeName = x.ItemType.Name,
                BarCode = x.BarCode.ToString(),
                ItemCode = x.ItemCode.ToString(),
                SellPrice = (float)x.SellPrice,
                MinPrice = x.MinPrice.ToString(),
                MaxPrice = x.MaxPrice.ToString(),
                AvailableToSell = x.AvaliableToSell ? "متوفر" : "غير متوفر",
                TechnicalSpecifications = x.TechnicalSpecifications.ToString()

            }).ToList();
            return item;
        }

        public List<ItemsVM> GetFilteredItems(string filter)
        {
            var list = db.Items.Where(x => !x.IsDeleted || x.Name.Contains(filter) || x.BarCode.Contains(filter) || (x.ItemCode + "").Contains(filter)).Select(x => new ItemsVM()
            {
                Id = x.Id,
                Name = x.Name,
                GroupBasicId = x.GroupBasicId,
                GroupBasicName = x.GroupBasic.Name,
                GroupSellId = x.GroupSellId.ToString(),
                GroupSellName = x.GroupSell.Name.ToString(),
                UnitId = x.UnitId,
                UnitName = x.Unit.Name,
                ItemTypeId = x.ItemTypeId,
                ItemTypeName = x.ItemType.Name,
                BarCode = x.BarCode.ToString(),
                ItemCode = x.ItemCode.ToString(),
                SellPrice = (float)x.SellPrice,
                MinPrice = x.MinPrice.ToString(),
                MaxPrice = x.MaxPrice.ToString(),
                AvailableToSell = x.AvaliableToSell ? "متوفر" : "غير متوفر",
                TechnicalSpecifications = x.TechnicalSpecifications.ToString()
            });
            list = list.Where(x => x.Name.Contains(filter) || x.BarCode.Contains(filter) || x.GroupBasicName.Contains(filter) || x.GroupSellName.Contains(filter) || (x.ItemCode + "").Contains(filter));
            return list.ToList();
        }

        public bool AddItemProduction(ItemProductionVM itemProductionVM)
        {
            var itemDetails = new List<ItemProductionDetail>();
            foreach (var item in itemProductionVM.itemProductionDetails)
            {
                itemDetails.Add(new ItemProductionDetail
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                });
            }

            var itemProduction = new ItemProduction
            {
                CustomerId = itemProductionVM.CustomerId,
                Name = itemProductionVM.ItemProductionName,
                ItemProductionDetails = itemDetails
            };

            db.ItemProductions.Add(itemProduction);
            if (db.SaveChanges(UserServices.UserInfo?.UserId) > 0)
            {
                return true;
            }
            else
                return false;

        }

        //تحديث الباركود لعدد 6 ارقام فقط
        public void UpdateBarcodLength()
        {
            var items = db.Items.Where(x => !x.IsDeleted).ToList();
            foreach (var item in items)
            {
                string barcode;
                barcode = generateBarCode();
                item.BarCode = barcode;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;

            }
            db.SaveChanges(UserServices.UserInfo.UserId);
        }
        string generateBarCode()
        {
        generate:
            var barcode = GeneratBarcodes.GenerateRandomBarcode();
            var isExistInItems = db.Items.Where(x => x.BarCode == barcode).Any();
            var isExistInItemSerials = db.ItemSerials.Where(x => x.SerialNumber == barcode).Any();
            if (isExistInItems)
                goto generate;
            else
                return barcode;
        }

        //تحديث اسعار الاصناف من ملف اكسيس
        public void UpdateItem()
        {
            var connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/sh_sa/Downloads/updateData.accdb;Persist Security Info=False;";
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // The insertSQL string contains a SQL statement that
                // inserts a new row in the source table.
                OleDbCommand command = new OleDbCommand("select * from ItemData");

                // Set the Connection to the new OleDbConnection.
                command.Connection = connection;

                // Open the connection and execute the insert command.
                try
                {
                    connection.Open();
                    OleDbDataReader dr;
                    dr = command.ExecuteReader();

                    dt.Load(dr);
                    dr.Close();
                    connection.Close();
                    List<string> ii = new List<string>();

                    foreach (DataRow item in dt.Rows)
                    {
                        var itemName = item["ItemName"].ToString().Trim();
                        var itemDb = db.Items.Where(x => x.Name.Trim() == itemName).FirstOrDefault();
                        if (itemDb != null)
                        {
                            double price;
                            if (double.TryParse(item["Price"].ToString(), out price))
                            {
                                itemDb.SellPrice = price;
                                db.Entry(itemDb).State = System.Data.Entity.EntityState.Modified;

                            }
                        }
                        else
                            ii.Add(itemName);

                    }
                    db.SaveChanges(UserServices.UserInfo.UserId);
                }

                // The connection is automatically closed when the
                // code exits the using block.
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        // تكلفة المنتج بدلالة (متوسط سعر الشراء - اخر سعر شراء - اعلى او اقل سعر شراء )
        public double GetItemCostCalculation(int costCalculation, Guid itemId)
        {
            switch (costCalculation)
            {
                case (int)ItemCostCalculationCl.AveragePurchasePrice: // متوسط سعر الشراء 
                                                                      //متوسط سعر الشراء =(قيمة الرصيد اول المدة*قيمة المشتريات)/(كمية الرصيد اول المدة+كمية المشتريات+
                    var totalSumOfQuantity = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price * x.Quantity).DefaultIfEmpty(0).Sum();
                    var totalSumOfQuantityFirst = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Price * x.Quantity).DefaultIfEmpty(0).Sum();

                    var totlaQuantity = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                    var totlaQuantityFirst = db.ItemIntialBalances.Where(x => !x.IsDeleted && x.ItemId == itemId).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

                    var totalSum = totalSumOfQuantity + totalSumOfQuantityFirst;
                    var totalQuantity = totlaQuantity + totlaQuantityFirst;

                    return totalQuantity > 0 ? Math.Round(totalSum / totalQuantity, 2) : 0;

                default:
                    return 0;
            }

        }

    }
}
