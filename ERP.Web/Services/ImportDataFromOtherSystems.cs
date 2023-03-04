using ERP.DAL;
using ERP.DAL.Models;
using ERP.Web.DataTablesDS;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Windows.Input;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public static class ImportDataFromOtherSystems
    {
        #region import data old system abdulahhusien
        public static bool ExcuteStores()
        {
            var branchId = new Guid("C4934360-3D2A-4C5E-B38D-E9C24E0E4645");
            using (var context = new VTSaleEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //مخزن ترومان
                        //مخزن دار رماد
                        //مخزن المسجد المعلق
                        //مخزن المحل
                        //مخزن خط المنيا
                        //مخزن خط ترومان
                        //مخزن مرتجع دار رماد
                        //مخزن مرتجع المسجد المعلق
                        //مخزن الرفايع 
                        //مخزن خط بريفكس
                        //مخزن خط الصعيد

                        //اضافة المخازن 
                        //context.Stores.AddRange(new List<Store>
                        //{
                        //    new Store { Name ="مخزن ترومان",BranchId=branchId},
                        //    new Store { Name ="مخزن دار رماد",BranchId=branchId},
                        //    new Store { Name ="مخزن المسجد المعلق",BranchId=branchId},
                        //    new Store { Name ="مخزن المحل",BranchId=branchId},
                        //    new Store { Name ="مخزن خط المنيا",BranchId=branchId},
                        //    new Store { Name ="مخزن خط ترومان",BranchId=branchId},
                        //    new Store { Name ="مخزن مرتجع دار رماد",BranchId=branchId},
                        //    new Store { Name ="مخزن مرتجع المسجد المعلق",BranchId=branchId},
                        //    new Store { Name ="مخزن الرفايع",BranchId=branchId},
                        //    new Store { Name ="مخزن خط بريفكس",BranchId=branchId},
                        //    new Store { Name ="مخزن خط الصعيد",BranchId=branchId},
                        //});

                        ////اضافة وحدة اساسية 
                        //context.Units.Add(new Unit
                        //{
                        //    Name = "بالقطعة"
                        //});

                        var query = $@"INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'575f3361-c07d-4f0f-811d-1349b72d4709', NULL,{branchId}, NULL, 0, N'مخزن خط المنيا', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'b7c97e9e-f786-4dcf-86e2-1690f68d1ce9', NULL,{branchId}, NULL, 0, N'مخزن المحل', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'ca6be5b6-3a1b-49a2-a396-1faffb72fef0', NULL,{branchId}, NULL, 0, N'مخزن خط ترومان', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'6c5bee64-17f1-47a0-a2fd-4d3450dbd3b2', NULL,{branchId}, NULL, 0, N'مخزن الرفايع', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'21a37820-319e-4a87-ba17-8e6afc583900', NULL,{branchId}, NULL, 0, N'مخزن دار رماد', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'a4ef50d4-30d2-44f6-84f3-9484d8511b04', NULL,{branchId}, NULL, 0, N'مخزن مرتجع دار رماد', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'dc5fddd2-0baf-44b1-9116-a5d3b6f3f4d0', NULL,{branchId}, NULL, 0, N'مخزن خط الصعيد', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'4cf0e6d1-ff63-40a6-a8ba-a60384f98b85', NULL,{branchId}, NULL, 0, N'مخزن مرتجع المسجد المعلق', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'0cc85fcd-65a9-43df-b0ee-bcf20722eab6', NULL,{branchId}, NULL, 0, N'مخزن المسجد المعلق', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'd235cd5b-050d-4c5b-8be1-e9a076e391c1', NULL,{branchId}, NULL, 0, N'مخزن ترومان', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Stores] ([Id], [AccountTreeId], [BranchId], [EmployeeId], [IsDamages], [Name], [Address], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy], [Employee_Id]) VALUES (N'6ed61abb-a982-4594-b10c-f51997e6f7aa', NULL,{branchId}, NULL, 0, N'مخزن خط بريفكس', NULL, CAST(N'2023-03-04T14:07:12.233' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL, NULL);
INSERT [dbo].[Units] ([Id], [Name], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [IsDeleted], [DeletedOn], [DeletedBy]) VALUES (N'52442eeb-32d1-49a7-bd54-48dcb844e9b9', N'بالقطعة', CAST(N'2023-03-04T15:20:57.770' AS DateTime), N'43e6bd4c-bdfb-4d4c-aa35-853c7446edff', NULL, NULL, 0, NULL, NULL);
";
                        context.Database.ExecuteSqlCommand(query);
                        //context.SaveChanges(new Guid("43E6BD4C-BDFB-4D4C-AA35-853C7446EDFF"));
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public class StoreDto
        {
            public Guid StoreId { get; set; }
            public string StoreName { get; set; }
        }
        public static bool ExcuteItems()
        {
            List<Item> items = new List<Item>();
            Guid userId = new Guid("43E6BD4C-BDFB-4D4C-AA35-853C7446EDFF");
            var branchId = new Guid("C4934360-3D2A-4C5E-B38D-E9C24E0E4645");

            using (var context = new VTSaleEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                       
                        DataTable dtItems = new DataTable();
                        using (OleDbConnection cn = new OleDbConnection())
                        {
                            using (OleDbCommand cmd =new OleDbCommand("",cn))
                            {
                                OleDbDataReader dr;
                                cn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=d:\\storee.xlsx;Extended Properties=\"Excel 12.0 Xml;HDR=YES\";";
                                cn.Open();

                                cmd.CommandText = "select * from [Sheet1$]";

                                dr = cmd.ExecuteReader();
                                dtItems.Load(dr);
                                dr.Close();
                                cn.Close();
                            }
                        }

                        //

                        Guid unitId =context.Units.FirstOrDefault().Id;
                        List<ItemIntialBalanceDetail> itemIntialBalanceDetails = new List<ItemIntialBalanceDetail>();

                        foreach (DataRow item in dtItems.Rows)
                        {
                            generate:
                                var barcode = GeneratBarcodes.GenerateRandomBarcode();
                                var isExistInItems = items.Where(x => x.BarCode == barcode).Any();
                                if (isExistInItems)
                                    goto generate;

                                //add group
                                Group group=new Group();
                            var g = item["group"].ToString().Trim();
                                var groupDb = context.Groups.Where(x => x.Name.Trim() ==g ).FirstOrDefault();
                            if (groupDb == null)
                            {
                                var groupNew= new Group
                                {
                                    Name = item["group"].ToString().Trim(),
                                    GroupTypeId = 1
                                };
                                string codePrefix = Properties.Settings.Default.CodePrefix;
                                groupNew.GroupCode = codePrefix + (context.Groups.Count(x => x.GroupCode.StartsWith(codePrefix)) + 1);
                                context.Groups.Add(groupNew);
                                context.SaveChanges(userId);
                                group = groupNew;
                            }
                            else
                                group=groupDb;
                            
                            //add item code
                            string itemCode = null;

                                if (group != null)
                                    itemCode = group.GroupCode + "-" + (context.Items.Where(x => x.GroupBasicId == group.Id && !x.IsDeleted).Count() + 1).ToString();
                                    else
                                    {
                                        throw new Exception("eeeee");
                                    }

                            //add item type
                            //add group
                            ItemType itemType = new ItemType();
                            var i = item["group"].ToString().Trim();
                            var itemTypeDb = context.ItemTypes.Where(x => x.Name.Trim() ==i).FirstOrDefault();
                            if (itemTypeDb == null)
                            {
                                var itemTypeNew = new ItemType
                                {
                                    Name = item["group"].ToString().Trim(),
                                };
                                context.ItemTypes.Add(itemTypeNew);
                                context.SaveChanges(userId);
                                itemType = itemTypeNew;
                            }
                            else
                                itemType = itemTypeDb;


                            //add item
                            var itemNew =new Item
                                {
                                    Name = item["item"].ToString(),
                                    ItemCode = itemCode,
                                    AvaliableToSell = true,
                                    BarCode = barcode,
                                    GroupBasicId =group.Id,
                                    ItemTypeId = itemType.Id,
                                    SellPrice =  0,
                                    UnitId = unitId,
                                };
                            context.Items.Add(itemNew);
                            context.SaveChanges(userId);

                            //اضافة رصيد اول مدة للاصناف 
                            double buyPrice = 0;
                            double.TryParse(item["BuyPrice"].ToString(), out buyPrice);
                            //مخزن ترومان 
                            if(double.TryParse(item["truman"].ToString(), out double trumanBalance) && trumanBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("D235CD5B-050D-4C5B-8BE1-E9A076E391C1"),
                                Quantity = trumanBalance,
                                Amount = buyPrice* trumanBalance,
                            });
                            //مخزن دار رماد
                            if(double.TryParse(item["darRamad"].ToString(), out double darRamadBalance)&&darRamadBalance>0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("21A37820-319E-4A87-BA17-8E6AFC583900"),
                                Quantity = darRamadBalance,
                                Amount = buyPrice * darRamadBalance,
                            });
                            //مخزن المسجد المعلق
                            if(double.TryParse(item["masged"].ToString(), out double masgedBalance) && masgedBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("0CC85FCD-65A9-43DF-B0EE-BCF20722EAB6"),
                                Quantity = masgedBalance,
                                Amount = buyPrice * masgedBalance,
                            });
                            //مخزن المحل
                            if(double.TryParse(item["shop"].ToString(), out double shopBalance) && shopBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("B7C97E9E-F786-4DCF-86E2-1690F68D1CE9"),
                                Quantity = shopBalance,
                                Amount = buyPrice * shopBalance,
                            });
                            //مخزن خط المنيا
                            if(double.TryParse(item["meniLine"].ToString(), out double meniLineBalance) && meniLineBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("575F3361-C07D-4F0F-811D-1349B72D4709"),
                                Quantity = meniLineBalance,
                                Amount = buyPrice * meniLineBalance,
                            });
                            //مخزن خط ترومان
                            //مخزن خط المنيا
                            if(double.TryParse(item["trumanLine"].ToString(), out double trumanLineBalance) && trumanLineBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("CA6BE5B6-3A1B-49A2-A396-1FAFFB72FEF0"),
                                Quantity = trumanLineBalance,
                                Amount = buyPrice * trumanLineBalance,
                            });
                            //مخزن مرتجع دار رماد
                            if(double.TryParse(item["darRamadBack"].ToString(), out double darRamadBackBalance) && darRamadBackBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("A4EF50D4-30D2-44F6-84F3-9484D8511B04"),
                                Quantity = darRamadBackBalance,
                                Amount = buyPrice * darRamadBackBalance,
                            });
                            //مخزن مرتجع المسجد المعلق
                            if(double.TryParse(item["masgedBack"].ToString(), out double masgedBackBalance) && masgedBackBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("4CF0E6D1-FF63-40A6-A8BA-A60384F98B85"),
                                Quantity = masgedBackBalance,
                                Amount = buyPrice * masgedBackBalance,
                            });
                            //مخزن الرفايع 
                            if(double.TryParse(item["rafie"].ToString(), out double rafieBalance) && rafieBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("6C5BEE64-17F1-47A0-A2FD-4D3450DBD3B2"),
                                Quantity = rafieBalance,
                                Amount = buyPrice * rafieBalance,
                            });
                            //مخزن خط بريفكس
                            if(double.TryParse(item["prefixLine"].ToString(), out double prefixLineBalance) && prefixLineBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("6ED61ABB-A982-4594-B10C-F51997E6F7AA"),
                                Quantity = prefixLineBalance,
                                Amount = buyPrice * prefixLineBalance,
                            });
                            //مخزن خط الصعيد
                            if(double.TryParse(item["saaedLine"].ToString(), out double saaedLineBalance) && saaedLineBalance > 0)
                            itemIntialBalanceDetails.Add(new ItemIntialBalanceDetail
                            {
                                ItemId = itemNew.Id,
                                Price = buyPrice,
                                StoreId = new Guid("DC5FDDD2-0BAF-44B1-9116-A5D3B6F3F4D0"),
                                Quantity = saaedLineBalance,
                                Amount = buyPrice * saaedLineBalance,
                            });



                        }

                        // اضافة الصنف فى رصيد اول المدة للاصناف
                        var newItemInitBalance = new ItemIntialBalance
                        {
                            BranchId = branchId,
                            DateIntial = Utility.GetDateTime(),
                            ItemIntialBalanceDetails = itemIntialBalanceDetails
                        };
                        //اضافة رقم الفاتورة
                        string codePrefix2 = Properties.Settings.Default.CodePrefix;
                        newItemInitBalance.InvoiceNumber = codePrefix2 + (context.ItemIntialBalances.Count(x => x.InvoiceNumber.StartsWith(codePrefix2)) + 1);

                        context.ItemIntialBalances.Add(newItemInitBalance);
                        context.SaveChanges(userId);
                        //    //تسجيل القيود
                        // General Dailies
                        if (GeneralDailyService.CheckGenralSettingHasValue((int)GeneralSettingTypeCl.AccountTree))
                        {
                            // الحصول على حسابات من الاعدادات
                            var generalSetting = context.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();
                            // التأكد من عدم وجود حساب فرعى من الحساب رأس المال
                            if (AccountTreeService.CheckAccountTreeIdHasChilds(Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue)))
                                return false;
                            // use Transactions

                            // حساب المخزون
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
                                Debit = itemIntialBalanceDetails.Sum(x => x.Amount),
                                Notes = $"رصيد أول المدة للاصناف ",
                                TransactionDate = Utility.GetDateTime(),
                                TransactionId = newItemInitBalance.Id,
                                BranchId = newItemInitBalance.BranchId,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                            });
                            //رأس المال 
                            context.GeneralDailies.Add(new GeneralDaily
                            {
                                AccountsTreeId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
                                Credit = itemIntialBalanceDetails.Sum(x => x.Amount),
                                Notes = $"رصيد أول المدة للاصناف",
                                TransactionDate = Utility.GetDateTime(),
                                TransactionId = newItemInitBalance.Id,
                                BranchId = newItemInitBalance.BranchId,
                                TransactionTypeId = (int)TransactionsTypesCl.InitialBalanceItem
                            });

                            //تحديث حالة الاعتماد 
                            newItemInitBalance.IsApproval = true;

                            context.SaveChanges(userId);
                        }
                        else
                            return false;



                        transaction.Commit();
                        return true;
                    }
         
                                
                                
                                catch (Exception ex)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        #endregion
    }
}
