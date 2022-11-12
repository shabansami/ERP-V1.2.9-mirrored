using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using ERP.DAL;
using static ERP.Web.Utilites.Lookups;
using ERP.DAL.Models;
using System.Text.RegularExpressions;
using Group = ERP.DAL.Group;
//using ERP.Web.NewFolder2;
using ItemType = ERP.DAL.ItemType;
using Unit = ERP.DAL.Unit;
using Item = ERP.DAL.Item;
using ItemUnit = ERP.DAL.ItemUnit;
using AccountsTree = ERP.DAL.AccountsTree;
using PersonCategory = ERP.DAL.PersonCategory;
using Person = ERP.DAL.Person;
using PersonIntialBalance = ERP.DAL.PersonIntialBalance;
using ERP.Web.ViewModels;

namespace ERP.Web.Services
{
    public class ItemImportDataZumurada
    {

        //public void Excute()
        //{

        //    using (var db = new VTSaleEntities())
        //    {
        //        db.Database.Log = Console.Write;

        //        using (DbContextTransaction transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                List<Item> items = new List<Item>();
        //        var oldData=db.OldDataaaas.ToList();
        //        foreach (var item in oldData)
        //        {
        //            if (!items.Where(x=>x.Name==item.Name||(x.ItemCode==item.OldCode&&item.OldCode!=null)).Any())
        //            {
        //                    generate:
        //                        var barcode = GeneratBarcodes.GenerateRandomBarcode();
        //                        var isExistInItems = items.Where(x => x.BarCode == barcode).Any();
        //                        if (isExistInItems)
        //                            goto generate;


        //                        items.Add(new Item
        //                {
        //                    Name = item.Name,
        //                    ItemCode = item.OldCode,
        //                    AvaliableToSell = true,
        //                    BarCode = barcode,
        //                    GroupBasicId = item.GroupBasicId,
        //                    ItemTypeId = item.ItemTypeId,
        //                    SellPrice = item.Price??0,
        //                    UnitId=item.UnitId,
        //                });
        //            }
        //        }
        //        db.Items.AddRange(items);
        //        db.SaveChanges(1);
        //                var branches = db.Branches.ToList();
        //                // الحصول على حسابات من الاعدادات
        //                var generalSetting = db.GeneralSettings.Where(x => x.SType == (int)GeneralSettingTypeCl.AccountTree).ToList();

        //                foreach (var branch in branches)
        //                {
        //                    var itemsBranch = oldData.Where(x => x.BranchId == branch.Id).ToList();
        //                    foreach (var item in itemsBranch)
        //                    {
        //                        double price = 0;
        //                        double quantity = 0;
        //                        if (double.TryParse(item.Price.ToString(), out price) && double.TryParse(item.Quantity.ToString(), out quantity))
        //                        {
        //                            var itemDb = db.Items.Where(x => x.ItemCode == item.OldCode || x.Name == item.Name).FirstOrDefault();
        //                            if (itemDb != null)
        //                            {
        //                                // اضافة الصنف فى رصيد اول المدة للاصناف
        //                                var newItemInitBalance = new ItemIntialBalance
        //                                {
        //                                    AccountTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
        //                                    Amount = price * quantity,
        //                                    ItemId = itemDb.Id,
        //                                    Price = price,
        //                                    Quantity = quantity,
        //                                    StoreId = item.StoreId
        //                                };

        //                                db.ItemIntialBalances.Add(newItemInitBalance);
        //                                db.SaveChanges(1);
        //                                var itemIniatialBalance = db.ItemIntialBalances.Where(x => x.Id == newItemInitBalance.Id).FirstOrDefault();
        //                                // حساب المخزون
        //                                db.GeneralDailies.Add(new GeneralDaily
        //                                {
        //                                    AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue),
        //                                    Debit = itemIniatialBalance.Amount,
        //                                    Notes = $"رصيد أول المدة للصنف : {itemIniatialBalance.Item.Name}",
        //                                    TransactionDate = Utility.GetDateTime(),
        //                                    TransactionId = itemIniatialBalance.Id,
        //                                    BranchId = branch.Id,
        //                                    TransactionTypeId = (int)TransactionsTypesCl.BalanceFirstDuration
        //                                }); ;
        //                                //رأس المال 
        //                                db.GeneralDailies.Add(new GeneralDaily
        //                                {
        //                                    AccountsTreeId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue),
        //                                    Credit = itemIniatialBalance.Amount,
        //                                    Notes = $"رصيد أول المدة للصنف : {itemIniatialBalance.Item.Name}",
        //                                    TransactionDate = Utility.GetDateTime(),
        //                                    TransactionId = itemIniatialBalance.Id,
        //                                    BranchId = branch.Id,
        //                                    TransactionTypeId = (int)TransactionsTypesCl.BalanceFirstDuration
        //                                });

        //                                //تحديث حالة الاعتماد 
        //                                itemIniatialBalance.IsApproval = true;
        //                                db.Entry(itemIniatialBalance).State = EntityState.Modified;
        //                                db.SaveChanges(1);
        //                            }


        //public void Excute2()
        //{

        //    using (var db = new VTSaleEntities())
        //    {
        //        db.Database.Log = Console.Write;

        //        using (DbContextTransaction transaction = db.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                List<int> items = new List<int>();
        //                int count = 1;
        //                var oldData = db.OldDataaaas.Where(x=>x.BranchId==1).OrderBy(x=>x.Name.Trim()).ToList();

        //                    var itemsdb = db.Items.Where(x=>!x.IsDeleted).ToList();
        //                foreach (var item in itemsdb)
        //                {
        //                    if (oldData.Where(x => x.Name.Trim() == item.Name.Trim()).Any())
        //                    {
        //                        item.ItemCode = count;
        //                        db.Entry(item).State = EntityState.Modified;
        //                        count++;
        //                    }
        //                    else
        //                        items.Add(item.Id);
        //                }
        //               var aff= db.SaveChanges(1);

        //                int countOther = count+1;
        //                foreach (var item in items)
        //                {
        //                    var itm = db.Items.Where(x => x.Id == item).FirstOrDefault();
        //                    if (itm != null)
        //                    {
        //                        itm.ItemCode = countOther;
        //                        countOther++;
        //                        db.Entry(itm).State = EntityState.Modified;
        //                    }
        //                }
        //                var aff2=db.SaveChanges(1);

        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                Console.WriteLine("Error occurred.");
        //            }
        //        }
        //    }



        //}

        #region Update Old version to latest version 1.3 
        //public bool UpdateDataToVersion1_3()
        //{
        //    //get old data 
        //    var dbOld = new sc_clientsDb2Entities();
        //    var items = dbOld.Items.Where(x => !x.IsDeleted).ToList();
        //    var itemUnitsOld = dbOld.ItemUnits.Where(x => !x.IsDeleted).ToList();
        //    var dtCustomers = dbOld.Persons.Where(x => !x.IsDeleted && x.PersonTypeId == 2).ToList();

        //    //Transaction
        //    using (var dbNew = new VTSaleEntities())
        //    {
        //        using (DbContextTransaction transaction = dbNew.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                Guid userId = dbNew.Users.FirstOrDefault().Id;
        //                Guid AreaId = dbNew.Areas.FirstOrDefault().Id;
        //                var groups = new List<ObjectDto>();
        //                var types = new List<ObjectDto>();
        //                var units = new List<ObjectDto>();
        //                var itemsSaved = new List<ObjectDto>();
        //                //add items
        //                foreach (var item in items)
        //                {
        //                    //add group if not exists
        //                    Guid? groupId;
        //                    string groupCoode;
        //                    var groupExist = groups.Where(x => x.OldId == item.GroupBasicId).FirstOrDefault();
        //                    if (groupExist != null)
        //                    {
        //                        groupId = groupExist.NewId;
        //                        groupCoode = groupExist.GroupCode;
        //                    }
        //                    else
        //                    {
        //                        //add new group
        //                        string codePrefix = Properties.Settings.Default.CodePrefix;
        //                        var GroupCode = codePrefix + (dbNew.Groups.Count(x => x.GroupCode.StartsWith(codePrefix)) + 1);

        //                        var groupNew = new Group { Name = item.Group.Name, GroupTypeId = 1, GroupCode = GroupCode };
        //                        dbNew.Groups.Add(groupNew);
        //                        if (dbNew.SaveChanges(userId) == 0)
        //                            throw new Exception("group not saved " + item.Group.Name);
        //                        groups.Add(new ObjectDto { OldId = item.GroupBasicId, NewId = groupNew.Id, Name = item.Group.Name, GroupCode = GroupCode });
        //                        groupId = groupNew.Id;
        //                        groupCoode = groupNew.GroupCode;
        //                    }
        //                    //add itemType if not exists
        //                    Guid? typeId;
        //                    var typeExist = types.Where(x => x.OldId == item.ItemTypeId).FirstOrDefault();
        //                    if (typeExist != null)
        //                    {
        //                        typeId = typeExist.NewId;
        //                    }
        //                    else
        //                    {
        //                        //add new type
        //                        var typeNew = new ItemType { Name = item.ItemType.Name };
        //                        dbNew.ItemTypes.Add(typeNew);
        //                        if (dbNew.SaveChanges(userId) == 0)
        //                            throw new Exception("type not saved " + item.ItemType.Name);
        //                        types.Add(new ObjectDto { OldId = item.ItemTypeId, NewId = typeNew.Id, Name = item.ItemType.Name });
        //                        typeId = typeNew.Id;
        //                    }
        //                    //add Unit if not exists
        //                    Guid? unitId;
        //                    var unitExist = units.Where(x => x.OldId == item.UnitId).FirstOrDefault();
        //                    if (unitExist != null)
        //                    {
        //                        unitId = unitExist.NewId;
        //                    }
        //                    else
        //                    {
        //                        //add new unit
        //                        var unitNew = new Unit { Name = item.Unit.Name };
        //                        dbNew.Units.Add(unitNew);
        //                        if (dbNew.SaveChanges(userId) == 0)
        //                            throw new Exception("unit not saved " + item.Unit.Name);
        //                        units.Add(new ObjectDto { OldId = item.UnitId, NewId = unitNew.Id, Name = item.Unit.Name });
        //                        unitId = unitNew.Id;
        //                    }
        //                    //add new item
        //                    string itemCode;
        //                    if (string.IsNullOrEmpty(item.ItemCode.ToString()))
        //                        itemCode = groupCoode + "-" + (dbNew.Items.Where(x => x.GroupBasicId == groupId && !x.IsDeleted).Count() + 1).ToString();
        //                    else
        //                        itemCode = item.ItemCode.ToString();

        //                    var itemNew = new Item
        //                    {
        //                        Name = item.Name,
        //                        ItemCode = itemCode,
        //                        GroupBasicId = groupId,
        //                        ItemTypeId = typeId,
        //                        SellPrice = item.SellPrice,
        //                        MaxPrice = item.MaxPrice,
        //                        MinPrice = item.MinPrice,
        //                        BarCode = item.BarCode,
        //                        UnitId = unitId,
        //                        AvaliableToSell = item.AvaliableToSell,
        //                        CreateSerial = false,
        //                    };
        //                    dbNew.Items.Add(itemNew);
        //                    if (dbNew.SaveChanges(userId) == 0)
        //                        throw new Exception("item not saved " + item.Name);
        //                    itemsSaved.Add(new ObjectDto { OldId = item.Id, NewId = itemNew.Id, Name = item.Name });
        //                    var t = 1;
        //                }

        //                //add item units
        //                foreach (var itemUnit in itemUnitsOld)
        //                {
        //                    //add Unit if not exists
        //                    Guid? unitId;
        //                    var unitExist = units.Where(x => x.OldId == itemUnit.UnitId).FirstOrDefault();
        //                    if (unitExist != null)
        //                    {
        //                        unitId = unitExist.NewId;
        //                    }
        //                    else
        //                    {
        //                        //add new unit
        //                        var unitNew = new Unit { Name = itemUnit.Unit.Name };
        //                        dbNew.Units.Add(unitNew);
        //                        if (dbNew.SaveChanges(userId) == 0)
        //                            throw new Exception("unit not saved itemUnit " + itemUnit.Unit.Name);
        //                        types.Add(new ObjectDto { OldId = itemUnit.UnitId, NewId = unitNew.Id, Name = itemUnit.Unit.Name });
        //                        unitId = unitNew.Id;
        //                    }
        //                    //add ItemUnit if not exists
        //                    var itemObj = itemsSaved.Where(x => x.OldId == itemUnit.ItemId).FirstOrDefault();
        //                    if (itemObj != null)
        //                    {
        //                        var itemUnitNew = new ItemUnit { ItemId = itemObj.NewId, UnitId = unitId, Quantity = itemUnit.Quantity, SellPrice = itemUnit.SellPrice };
        //                        dbNew.ItemUnits.Add(itemUnitNew);
        //                        if (dbNew.SaveChanges(userId) == 0)
        //                            throw new Exception("unit not saved itemUnit " + itemUnit.Id);
        //                    }

        //                }
        //                //add customer
        //                var cat = new PersonCategory { Name = "فئة عملاء عامة", IsCustomer = true };
        //                //var cat = new Guid("C36D7EF6-40BC-43EE-942B-27594E42B6D1");
        //                dbNew.PersonCategories.Add(cat);
        //                dbNew.SaveChanges(userId);
        //                foreach (var cust in dtCustomers)
        //                {
        //                    Person person = new Person()
        //                    {
        //                        Name = cust.Name.Trim(),
        //                        AreaId = AreaId,
        //                        Mob1 = cust.Mob1,
        //                        ParentId = null,
        //                        PersonCategoryId = cat.Id,
        //                        PersonTypeId = 2,
        //                        IsActive = true
        //                    };

        //                    //add as customer in account tree
        //                    long newAccountNum = 0;// new account number 
        //                    var val = dbNew.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeCustomerAccount).SValue;

        //                    var accountTree = dbNew.AccountsTrees.Find(Guid.Parse(val));
        //                    var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
        //                    if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
        //                        newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
        //                    else
        //                        newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

        //                    // add new account tree 
        //                    var accountTreeCust = new AccountsTree
        //                    {
        //                        AccountLevel = accountTree.AccountLevel + 1,
        //                        AccountName = person.Name,
        //                        AccountNumber = newAccountNum,
        //                        ParentId = accountTree.Id,
        //                        TypeId = (int)AccountTreeSelectorTypesCl.Customer,
        //                        SelectedTree = false
        //                    };
        //                    dbNew.AccountsTrees.Add(accountTreeCust);
        //                    //add person in personTable
        //                    person.AccountsTreeCustomer = accountTreeCust;
        //                    dbNew.Persons.Add(person);
        //                    if (dbNew.SaveChanges(userId) == 0)
        //                        throw new Exception("customer not saved " + cust.Id);

        //                }
        //                transaction.Commit();

        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //            }
        //        }
        //    }


        //    return true;
        //}

        //public class ObjectDto
        //{
        //    public int? OldId { get; set; }
        //    public Guid? NewId { get; set; }
        //    public string Name { get; set; }
        //    public string GroupCode { get; set; }
        //}
        #endregion
    }


    public class ItemImportMohRamadan
    {
        public void Excute()
        {
            string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:/Users/sh_sa/Documents/Database1.accdb;Persist Security Info=False";
            OleDbConnection cn = new OleDbConnection(constr);
            OleDbCommand cmd = cn.CreateCommand();
            DataTable dtGroup = new DataTable();
            DataTable dtTyp = new DataTable();
            DataTable dtItems = new DataTable();
            DataTable dtCustomers = new DataTable();
            OleDbDataReader dr;
            cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "select * from Groups ";
            dr = cmd.ExecuteReader();
            dtGroup.Load(dr);
            dr.Close();

            cmd.CommandText = "select * from Typs ";
            dr = cmd.ExecuteReader();
            dtTyp.Load(dr);
            dr.Close();

            cmd.CommandText = "select * from Items ";
            dr = cmd.ExecuteReader();
            dtItems.Load(dr);
            dr.Close();

            cmd.CommandText = "select * from Customers ";
            dr = cmd.ExecuteReader();
            dtCustomers.Load(dr);
            dr.Close();

            cn.Close();

            using (var db = new VTSaleEntities())
            {
                //add groups
                foreach (DataRow group in dtGroup.Rows)
                {
                    db.Groups.Add(new Group
                    {
                        GroupTypeId = 1,
                        Name = group["Nam"].ToString().Trim()
                    });

                }
                //add itemTypes
                foreach (DataRow group in dtTyp.Rows)
                {
                    db.ItemTypes.Add(new ItemType
                    {
                        Name = group["Nam"].ToString().Trim()
                    });
                }
                var aff = db.SaveChanges();

            }

            using (var db = new VTSaleEntities())
            {
                List<Item> items = new List<Item>();
                foreach (DataRow item in dtItems.Rows)
                {
                generate:
                    var barcode = GeneratBarcodes.GenerateRandomBarcode();
                    var isExistInItems = items.Where(x => x.BarCode == barcode).Any();
                    if (isExistInItems)
                        goto generate;

                    Guid? groupId = null;
                    var gg = item["Group"].ToString().Trim();
                    var group = db.Groups.Where(x => x.Name == gg).FirstOrDefault();
                    if (group != null)
                        groupId = group.Id;

                    Guid? itemTypId = null;
                    var tt = item["Typ"].ToString().Trim();
                    var itemTyp = db.ItemTypes.Where(x => x.Name == tt).FirstOrDefault();
                    if (itemTyp != null)
                        itemTypId = itemTyp.Id;

                    items.Add(new Item
                    {
                        Name = item["Nam"].ToString().Trim(),
                        ItemCode = item["Id"].ToString(),
                        AvaliableToSell = true,
                        BarCode = barcode,
                        GroupBasicId = groupId,
                        ItemTypeId = itemTypId,
                        SellPrice = double.Parse(item["sellPrice"].ToString()),
                        UnitId = new Guid(),
                        TechnicalSpecifications = item["desc"].ToString()
                    });
                }
                db.Items.AddRange(items);
                var aff2 = db.SaveChanges();
                foreach (DataRow cust in dtCustomers.Rows)
                {
                    Guid? personCatId = null;
                    var cc = cust["CatNam"].ToString().Trim();
                    var personCat = db.PersonCategories.Where(x => x.Name == cc).FirstOrDefault();
                    if (personCat != null)
                        personCatId = personCat.Id;


                    Person person = new Person()
                    {
                        Name = cust["Nam"].ToString().Trim(),
                        AreaId = new Guid(),
                        Mob1 = "123456789",
                        ParentId = null
                    };

                    //add as customer in account tree
                    //var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, person.Name, AccountTreeSelectorTypesCl.Customer);
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeCustomerAccount).SValue;

                    var accountTree = db.AccountsTrees.Find(int.Parse(val));
                    var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                    if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                        newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                    else
                        newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                    // add new account tree 
                    var accountTreeCust = new AccountsTree
                    {
                        AccountLevel = accountTree.AccountLevel + 1,
                        AccountName = person.Name,
                        AccountNumber = newAccountNum,
                        ParentId = accountTree.Id,
                        TypeId = (int)AccountTreeSelectorTypesCl.Customer,
                        SelectedTree = false
                    };
                    db.AccountsTrees.Add(accountTreeCust);
                    //add person in personTable
                    person.AccountsTreeCustomer = accountTreeCust;
                    db.Persons.Add(person);
                    var aff3 = db.SaveChanges();

                }

            }

            //using (var db = new VTSaleEntities())
            //{
            //    using (DbContextTransaction transaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {

            //            //add Items


            //            // اضافة العملاء 

            //            transaction.Commit();
            //        }
            //        catch (Exception ex)
            //        {
            //            transaction.Rollback();
            //            Console.WriteLine("Error occurred.");
            //        }
            //    }
            //}


        }
        public void ExcuteV1_2()
        {
            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\mohamed ramadan\Database1.mdb;Persist Security Info=False";
            string constr = @"Provider=sqloledb;Data Source=DESKTOP-GOIJQN1\SQLEXPRESS;Initial Catalog=test;Integrated Security=SSPI;";
            OleDbConnection cn = new OleDbConnection(constr);
            OleDbCommand cmd = cn.CreateCommand();
            DataTable dtGroup = new DataTable();
            DataTable dtSupp = new DataTable();
            DataTable dtItems = new DataTable();
            DataTable dtCustomers = new DataTable();
            OleDbDataReader dr;
            cn.Open();
            cmd.Connection = cn;
            cmd.CommandText = "select * from Items ";
            dr = cmd.ExecuteReader();
            dtItems.Load(dr);
            dr.Close();

            cmd.CommandText = "select * from customers ";
            dr = cmd.ExecuteReader();
            dtCustomers.Load(dr);
            dr.Close();
            cmd.CommandText = "select * from suppliers ";
            dr = cmd.ExecuteReader();
            dtSupp.Load(dr);
            dr.Close();

            cn.Close();

            using (var db = new VTSaleEntities())
            {
                List<Item> items = new List<Item>();
                foreach (DataRow item in dtItems.Rows)
                {
                generate:
                    var barcode = GeneratBarcodes.GenerateRandomBarcode();
                    var isExistInItems = items.Where(x => x.BarCode == barcode).Any();
                    if (isExistInItems)
                        goto generate;

                    Guid? groupId = null;
                    var gg = item["group"].ToString().Trim();
                    var groupMo = db.Groups.Where(x => x.Name.Trim() == gg).FirstOrDefault();
                    if (groupMo != null)
                        groupId = groupMo.Id;
                    else
                    {
                        var id = Guid.NewGuid();
                        db.Groups.Add(new Group { Id = id, GroupTypeId = 1, Name = gg });
                        db.SaveChanges();
                        groupId = id;
                    }
                    Guid? itemTypId = null;
                    var tt = item["typ"].ToString().Trim();
                    var itemTyp = db.ItemTypes.Where(x => x.Name.Trim() == tt).FirstOrDefault();
                    if (itemTyp != null)
                        itemTypId = itemTyp.Id;
                    else
                    {
                        var id = Guid.NewGuid();
                        db.ItemTypes.Add(new ItemType { Id = id, Name = tt });
                        db.SaveChanges();
                        itemTypId = id;
                    }
                    var itemm = new Item
                    {
                        Id = Guid.NewGuid(),
                        Name = item["nam"].ToString().Trim(),
                        ItemCode = db.Items.Count() + 1.ToString(),
                        AvaliableToSell = true,
                        BarCode = barcode,
                        GroupBasicId = groupId,
                        ItemTypeId = itemTypId,
                        SellPrice = 0,
                        UnitId = new Guid("A28CF43E-EE99-4925-9603-034BC29E77AD"),
                    };
                    db.Items.Add(itemm);
                    var aff2 = db.SaveChanges();
                }

                foreach (DataRow cust in dtCustomers.Rows)
                {
                    Guid? personCatId = null;
                    var cc = cust["cat"].ToString().Trim();
                    var personCat = db.PersonCategories.Where(x => x.Name.Trim() == cc).FirstOrDefault();
                    if (personCat != null)
                        personCatId = personCat.Id;
                    else
                    {
                        var id = Guid.NewGuid();
                        db.PersonCategories.Add(new PersonCategory {Id=id, Name = cc });
                        db.SaveChanges();
                        personCatId = id;
                    }


                    //add as customer in account tree
                    //var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, person.Name, AccountTreeSelectorTypesCl.Customer);
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeCustomerAccount).SValue;
                    Guid vall = Guid.Parse(val);
                    var accountTree = db.AccountsTrees.Where(x=>x.Id==vall).FirstOrDefault();
                    var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                    if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                        newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                    else
                        newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                    // add new account tree 
                    var accid = Guid.NewGuid();
                    var accountTreeCust = new AccountsTree
                    {
                        Id=accid,
                        AccountLevel = accountTree.AccountLevel + 1,
                        AccountName = cust["nam"].ToString().Trim(),
                        AccountNumber = newAccountNum,
                        ParentId = accountTree.Id,
                        TypeId = (int)AccountTreeSelectorTypesCl.Customer,
                        SelectedTree = false
                    };
                    db.AccountsTrees.Add(accountTreeCust);
                    db.SaveChanges();

                    var idd = Guid.NewGuid();
                    Person person = new Person()
                    {
                        Id = idd,
                        Name = cust["nam"].ToString().Trim(),
                        AreaId = new Guid("E176D214-1E63-4B77-B735-F8C974BD4312"),
                        Mob1 = "123456789",
                        ParentId = null,
                        PersonTypeId=(int)PersonTypeCl.Customer,
                        PersonCategoryId = personCatId,
                        AccountsTreeCustomerId = accid,
                    };
                    db.Persons.Add(person);
                    db.SaveChanges();

                }
                foreach (DataRow cust in dtSupp.Rows)
                {
                   
                    //add as customer in account tree
                    //var accountTreeCust = InsertGeneralSettings<Person>.ReturnAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, person.Name, AccountTreeSelectorTypesCl.Customer);
                    long newAccountNum = 0;// new account number 
                    var val = db.GeneralSettings.Find((int)GeneralSettingCl.AccountTreeSupplierAccount).SValue;
                    Guid vall = Guid.Parse(val);
                    var accountTree = db.AccountsTrees.Where(x=>x.Id==vall).FirstOrDefault();
                    var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                    if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                        newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                    else
                        newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                    // add new account tree 
                    var accid = Guid.NewGuid();
                    var accountTreeSupp = new AccountsTree
                    {
                        Id=accid,
                        AccountLevel = accountTree.AccountLevel + 1,
                        AccountName = cust["nam"].ToString().Trim(),
                        AccountNumber = newAccountNum,
                        ParentId = accountTree.Id,
                        TypeId = (int)AccountTreeSelectorTypesCl.Supplier,
                        SelectedTree = false
                    };
                    db.AccountsTrees.Add(accountTreeSupp);
                    db.SaveChanges();

                    var idd = Guid.NewGuid();
                    Person person = new Person()
                    {
                        Id = idd,
                        Name = cust["nam"].ToString().Trim(),
                        AreaId = new Guid("E176D214-1E63-4B77-B735-F8C974BD4312"),
                        Mob1 = "123456789",
                        ParentId = null,
                        PersonTypeId = (int)PersonTypeCl.Supplier,
                        AccountTreeSupplierId = accid,
                    };
                    db.Persons.Add(person);
                    db.SaveChanges();


                }

            }

        

        }
    }

    class GeneralDay
    {
        public Guid GeneraDayId { get; set; }
        //public int TransactionId { get; set; }
        //public double Amount { get; set; }
    }
    public class FixInitialBalance
    {
        public void Excute()
        {
            using (var db = new VTSaleEntities())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        //رصيد اول المدة العملاء
                        var custInitOld = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == 8 && x.Notes.Contains("للعميل")).ToList();
                        var supInitOld = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == 8 && x.Notes.Contains("للمورد")).ToList();
                        var itemInitOld = db.GeneralDailies.Where(x => !x.IsDeleted && x.TransactionTypeId == 8 && x.Notes.Contains("للصنف")).ToList();

                        var custGroup = custInitOld.GroupBy(x => x.TransactionId).Select(x => new { TransactionId = x.FirstOrDefault().TransactionId, Amount = x.FirstOrDefault().Debit + x.FirstOrDefault().Credit, dt = x.FirstOrDefault().TransactionDate, TransactionIdsList = x.Select(d => new GeneralDay { GeneraDayId = d.Id }).ToList() }).ToList();
                        //اضافة رصيد العميل فى الجدول الجديد
                        //var ttt = custGroup.Where(x => x.AccountTreeCust == 25).Any();
                        List<PersonIntialBalance> listt = new List<PersonIntialBalance>();
                        var branchId = new Guid("00008531-1234-1234-1234-012345678910");
                        foreach (var item in custGroup)
                        {
                            var custInit = new PersonIntialBalance
                            {
                                Amount = item.Amount,
                                BranchId = branchId,
                                IsCustomer = true,
                                IsDebit = true,
                                OperationDate = item.dt,
                                PersonId = db.Persons.Where(x => x.AccountsTreeCustomerId == item.TransactionId).FirstOrDefault().Id,
                            };
                            listt.Add(custInit);
                            db.PersonIntialBalances.Add(custInit);
                            db.SaveChanges();

                            var tranList = item.TransactionIdsList.Select(x => x.GeneraDayId);
                            var genDays = db.GeneralDailies.Where(x => tranList.Contains(x.Id)).ToList();
                            foreach (var item2 in genDays)
                            {
                                item2.TransactionId = custInit.Id;
                                item2.TransactionTypeId = 25;
                                db.Entry(item2).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }

                        var supGroup = supInitOld.GroupBy(x => x.TransactionId).Select(x => new { TransactionId = x.FirstOrDefault().TransactionId, Amount = x.FirstOrDefault().Debit + x.FirstOrDefault().Credit, dt = x.FirstOrDefault().TransactionDate, TransactionIdsList = x.Select(d => new GeneralDay { GeneraDayId = d.Id }).ToList() }).ToList();
                        //اضافة رصيد العميل فى الجدول الجديد
                        //var twtt = supGroup.Where(x => x.AccountTreeCust == 25).Any();
                        List<PersonIntialBalance> lists = new List<PersonIntialBalance>();
                        foreach (var item in supGroup)
                        {
                            var suppInit = new PersonIntialBalance
                            {
                                Amount = item.Amount,
                                BranchId = branchId,
                                IsCustomer = false,
                                IsDebit = false,
                                OperationDate = item.dt,
                                PersonId = db.Persons.Where(x => x.AccountTreeSupplierId == item.TransactionId).FirstOrDefault().Id,
                            };
                            listt.Add(suppInit);
                            db.PersonIntialBalances.Add(suppInit);
                            db.SaveChanges();

                            var tranList = item.TransactionIdsList.Select(x => x.GeneraDayId);
                            var genDays = db.GeneralDailies.Where(x => tranList.Contains(x.Id)).ToList();
                            foreach (var item2 in genDays)
                            {
                                item2.TransactionId = suppInit.Id;
                                item2.TransactionTypeId = 26;
                                db.Entry(item2).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }

                        var t = 0;

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error occurred.");
                    }
                }
            }

        }
    }


}