using ERP.Web.DataTablesDS;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using System.Data.Entity;

namespace ERP.Web.Services
{
    public static class ReportsAccountTreeService
    {
        static ItemService _itemService = new ItemService();

        #region ميزان المراجعة
        public static List<AuditBalanceDto> AuditBalances(DateTime? dtFrom, DateTime? dtTo, int? accountLevel, Guid? accountTreeId, Guid? branchId = null,bool? StatusVal=null,int? AccountMain=null) 
        {
            //StatusVal  
            // الكل بمبالغ وصفرية true
            // حالة عرض حسابات ميزان المراجعة بمبالغ فقط false

            //AccountMain عرض حسابات لحساب رئيسيى مثل المصروفات او الايرادات لاستخدامها فى قائمة الدخل
            using (var db = new VTSaleEntities())
            {
                IQueryable<AccountsTree> accountTrees = null;
                if (accountTreeId != null && accountTreeId.HasValue)
                    accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == accountTreeId);
                else if(AccountMain!=null)
                    accountTrees= db.AccountsTrees.Where(x => !x.IsDeleted&&x.AccountNumber.ToString().StartsWith(AccountMain.ToString()));
                else
                    accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted);

                List<int> initialBalanes = new List<int>
                {
                    (int)TransactionsTypesCl.InitialBalanceItem,
                    (int)TransactionsTypesCl.InitialBalanceAsset,
                    (int)TransactionsTypesCl.InitialBalanceBank,
                    (int)TransactionsTypesCl.InitialBalanceCustomer,
                    (int)TransactionsTypesCl.InitialBalanceFixedAsset,
                    (int)TransactionsTypesCl.InitialBalancePropertyRights,
                    (int)TransactionsTypesCl.InitialBalanceSafe,
                    (int)TransactionsTypesCl.InitialBalanceSupplier,
                };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                List<AuditBalanceDto> accounts = new List<AuditBalanceDto>();
                if (branchId != null)
                    accounts = accountTrees
                    .Select(x => new AuditBalanceDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        AccountNumber = x.AccountNumber,
                        AccountName = x.AccountName,
                        AccountLevel = x.AccountLevel,
                        IntialBalanceFrom = x.GeneralDailies.Where(g => !g.IsDeleted && g.BranchId == branchId && initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        IntialBalanceTo = x.GeneralDailies.Where(g => !g.IsDeleted && g.BranchId == branchId && initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),
                        ActionsFrom = x.GeneralDailies.Where(g => !g.IsDeleted && g.BranchId == branchId && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        ActionsTo = x.GeneralDailies.Where(g => !g.IsDeleted && g.BranchId == branchId && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),

                    }).OrderBy(x => new { x.ParentId, x.AccountName })
                    .ToList();
                else
                    accounts = accountTrees
                    .Select(x => new AuditBalanceDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        AccountNumber = x.AccountNumber,
                        AccountName = x.AccountName,
                        AccountLevel = x.AccountLevel,
                        IntialBalanceFrom = x.GeneralDailies.Where(g => !g.IsDeleted && initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        IntialBalanceTo = x.GeneralDailies.Where(g => !g.IsDeleted && initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),
                        ActionsFrom = x.GeneralDailies.Where(g => !g.IsDeleted && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        ActionsTo = x.GeneralDailies.Where(g => !g.IsDeleted && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && DbFunctions.TruncateTime(g.TransactionDate) >= dtFrom && DbFunctions.TruncateTime(g.TransactionDate) <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),

                    }).OrderBy(x => new { x.ParentId, x.AccountName })
                    .ToList();
                accounts = accounts.Select(x => new AuditBalanceDto
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    AccountNumber = x.AccountNumber,
                    AccountName = x.AccountName,
                    AccountLevel = x.AccountLevel,
                    IntialBalanceFrom = x.IntialBalanceFrom, //A1
                    IntialBalanceTo = x.IntialBalanceTo,//B1
                    ActionsFrom = x.ActionsFrom, //A2
                    ActionsTo = x.ActionsTo, //B2
                    SumFrom = x.ActionsFrom + x.IntialBalanceFrom,//C=A1+A2
                    SumTo = x.ActionsTo + x.IntialBalanceTo,//D=B1+B2
                    //if(C>D)? C-D  :0
                    ResultFrom = (x.ActionsFrom + x.IntialBalanceFrom) > (x.ActionsTo + x.IntialBalanceTo) ? ((x.ActionsFrom + x.IntialBalanceFrom) - (x.ActionsTo + x.IntialBalanceTo)) : 0,
                    //if(D>C)? D-C  :0
                    ResultTo = (x.ActionsTo + x.IntialBalanceTo) > (x.ActionsFrom + x.IntialBalanceFrom) ? ((x.ActionsTo + x.IntialBalanceTo) - (x.ActionsFrom + x.IntialBalanceFrom)) : 0

                }).ToList();
                stopwatch.Stop();//231 m

                ///////=========== الطريقة الاولى لعرض كل الحسابات بارصدتها 
                /////================================================================
                var accountList = ChildrenOf2(accounts, null);
                var accountRecursive = ReturnWithTotal(accountList);
                var accountFlattened = accountRecursive.RecursiveSelector(x => x.children).ToList();
                List<AuditBalanceDto> AuditBalanceDto = new List<AuditBalanceDto>();
                if (accountLevel != null && accountLevel.HasValue)
                {
                    accountFlattened = accountFlattened.Where(x => x.AccountLevel == accountLevel).ToList();
                }
                if (StatusVal == false)
                    accountFlattened = accountFlattened.Where(x => x.IntialBalanceFrom != 0 || x.IntialBalanceTo != 0 || x.ActionsFrom != 0 || x.ActionsTo != 0 || x.SumFrom != 0 || x.SumTo != 0 || x.ResultFrom != 0 || x.ResultTo != 0).ToList();

                //======================================================================
                //======================================================================

                ////List<AuditBalanceDto> accountFlattened = new List<AuditBalanceDto>();
                ////var accountList = ChildrenOf2(accounts, null);
                ////var accountRecursive = ReturnWithTotal(accountList);
                ////if (accountLevel != null && accountLevel.HasValue)
                ////{
                ////    //
                ////    if (accountLevel==1) //مجمع (الحسابات فى اول مستوى فقط 
                ////    {
                ////        accountFlattened = accountRecursive.Where(x => x.ParentId == null).ToList();
                ////        accountFlattened = accountFlattened.Where(x => x.IntialBalanceFrom != 0 || x.IntialBalanceTo != 0 || x.ActionsFrom != 0 || x.ActionsTo != 0 || x.SumFrom != 0 || x.SumTo != 0 || x.ResultFrom != 0 || x.ResultTo != 0).ToList();
                ////    }
                ////    else //حسابات اخر مستوى فقط
                ////    {
                        //var accountsList = accountRecursive.RecursiveSelector(x => x.children).ToList();
                        //var accountsListParent = accountsList.Where(x => x.ParentId == null).ToList();
                        //foreach (var account in accountsListParent)
                        //{
                        //    var parentAccounts = AccountTreeService.GetAccountTreeLastChild(account.Id);
                        //    foreach (var accountChild in parentAccounts)
                        //    {
                        //        var acc = accountsList.Where(x => x.Id == accountChild.Id).FirstOrDefault();
                        //        accountFlattened.Add(acc);
                        //    }

                        //}
                ////        accountFlattened = accounts.Where(x => x.IntialBalanceFrom != 0 || x.IntialBalanceTo != 0 || x.ActionsFrom != 0 || x.ActionsTo != 0 || x.SumFrom != 0 || x.SumTo != 0 || x.ResultFrom != 0 || x.ResultTo != 0).ToList();
                ////    }

                ////}else
                ////    accountFlattened = accountRecursive.RecursiveSelector(x => x.children).ToList();

                //if (StatusVal == false)
                //    accountFlattened = accountFlattened.Where(x => x.IntialBalanceFrom != 0 || x.IntialBalanceTo != 0 || x.ActionsFrom != 0 || x.ActionsTo != 0 || x.SumFrom != 0 || x.SumTo != 0 || x.ResultFrom != 0 || x.ResultTo != 0).ToList();


                return accountFlattened;
                


            }
        }

        public static List<AuditBalanceDto> ChildrenOf2(List<AuditBalanceDto> accountsTrees, Guid? parentId)
        {
            var accountChildrens = accountsTrees.Where(i => i.ParentId == parentId);

            return accountChildrens.Select(i => new AuditBalanceDto
            {
                Id = i.Id,
                ParentId = i.ParentId,
                AccountNumber = i.AccountNumber,
                AccountName = i.AccountName,
                AccountLevel = i.AccountLevel,
                IntialBalanceFrom = i.IntialBalanceFrom,
                IntialBalanceTo = i.IntialBalanceTo,
                ActionsFrom = i.ActionsFrom,
                ActionsTo = i.ActionsTo,
                SumFrom = i.SumFrom,
                SumTo = i.SumTo,
                ResultFrom = i.ResultFrom,
                ResultTo = i.ResultTo,
                children = ChildrenOf2(accountsTrees, i.Id)
            })
            .ToList();
        }
        public static List<AuditBalanceDto> ReturnWithTotal(List<AuditBalanceDto> treeViewDraws)
        {

            return treeViewDraws.Select(i => new AuditBalanceDto
            {
                Id = i.Id,
                ParentId = i.ParentId,
                AccountNumber = i.AccountNumber,
                AccountName = i.AccountName,
                AccountLevel = i.AccountLevel,
                IntialBalanceFrom = TotalIntialBalanceFrom(i),
                IntialBalanceTo = TotalIntialBalanceTo(i),
                ActionsFrom = TotalActionsFrom(i),
                ActionsTo = TotalActionsTo(i),
                SumFrom = TotalSumFrom(i),
                SumTo = TotalSumTo(i),
                ResultFrom = TotalResultFrom(i),
                ResultTo = TotalResultTo(i),

                children = ReturnWithTotal(i.children)
            })
                .ToList();

        }

        public static IEnumerable<T> RecursiveSelector<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> selector)
        {
            if (nodes.Any() == false)
            {
                return nodes;
            }

            var descendants = nodes
                                .SelectMany(selector)
                                .RecursiveSelector(selector);

            return nodes.Concat(descendants);
        }

        #region Total
        public static double TotalIntialBalanceFrom(AuditBalanceDto node)
        {
            double total = node.IntialBalanceFrom;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalIntialBalanceFrom(child);
                }
            }

            return total;
        }
        public static double TotalIntialBalanceTo(AuditBalanceDto node)
        {
            double total = node.IntialBalanceTo;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalIntialBalanceTo(child);
                }
            }

            return total;
        }
        public static double TotalActionsFrom(AuditBalanceDto node)
        {
            double total = node.ActionsFrom;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalActionsFrom(child);
                }
            }

            return total;
        }
        public static double TotalActionsTo(AuditBalanceDto node)
        {
            double total = node.ActionsTo;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalActionsTo(child);
                }
            }

            return total;
        }
        public static double TotalSumFrom(AuditBalanceDto node)
        {
            double total = node.SumFrom;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalSumFrom(child);
                }
            }

            return total;
        }
        public static double TotalSumTo(AuditBalanceDto node)
        {
            double total = node.SumTo;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalSumTo(child);
                }
            }

            return total;
        }
        public static double TotalResultFrom(AuditBalanceDto node)
        {
            double total = node.ResultFrom;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalResultFrom(child);
                }
            }

            return total;
        }
        public static double TotalResultTo(AuditBalanceDto node)
        {
            double total = node.ResultTo;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    total += TotalResultTo(child);
                }
            }

            return total;
        }

        #endregion

        #endregion

        #region قائمة الدخل 
        public static List<IncomeListDto> IncomeLists(DateTime? dtFrom, DateTime? dtTo)
        {

            using (var db = new VTSaleEntities())
            {
                List<IncomeListDto> incomeLists = new List<IncomeListDto>();
                var generalSetting = db.GeneralSettings.ToList();
                //=================================
                // اى حساب يزيد يكون مع طبيعته
                //اى حساب يقل يعكس مع طبيعته
                //كل الأصول(مدين) حساب(1)
                //كل المصروفات(مدين) حساب(3)
                //كل الخصوم(دائنة) حساب(2)
                //كل الإيرادات(دائنة) حساب(4)
                //=================================

                //مبيعات بضاعه
                //=================================================
                //حساب المبيعات من الاعدادات
                var saleAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var saleDetails = db.AccountsTrees.Where(x => x.Id == saleAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountSale = GetAuditBalances(dtFrom, dtTo, saleAccountId);
                var AmountSales = AmountSale.ResultFrom+ AmountSale.ResultTo;
                //var AmountSales = GetAuditBalances(dtFrom, dtTo, saleAccountId).ResultTo;//قديما
                saleDetails.Partial = Math.Round(AmountSales, 2);

                //مرددوات المبيعات بضاعه
                //==================================================
                //حساب مرددوات المبيعات من الاعدادات
                var saleReturnAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var saleReturnDetails = db.AccountsTrees.Where(x => x.Id == saleReturnAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountReturnSale = GetAuditBalances(dtFrom, dtTo, saleReturnAccountId);
                var AmountReturnSales = AmountReturnSale.ResultFrom + AmountReturnSale.ResultTo;
                //var AmountReturnSales = GetAuditBalances(dtFrom, dtTo, saleReturnAccountId).ResultFrom;//قديما
                saleReturnDetails.Partial = Math.Round(AmountReturnSales, 2);


                ////صافى المبيعات
                ////=====================================================
                //var saleSafy = AmountSales - AmountReturnSales;
                //incomeLists.Add(new IncomeListDto
                //{
                //    AccountName = "صافــي المبيعات",
                //    Whole = Math.Round(saleSafy, 2),
                //    IsTotal = true
                //});


                //خصم مسموح به
                //======================================================
                //حساب خصم مسموح به من الاعدادات
                var premittedDiscountAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var premittedDiscountDetails = db.AccountsTrees.Where(x => x.Id == premittedDiscountAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountPremittedDiscount = GetAuditBalances(dtFrom, dtTo, premittedDiscountAccountId);
                var AmountPremittedDiscounts = AmountPremittedDiscount.ResultFrom + AmountPremittedDiscount.ResultTo;
                //var AmountPremittedDiscounts = GetAuditBalances(dtFrom, dtTo, premittedDiscountAccountId).ResultFrom;//قديما
                premittedDiscountDetails.Partial = Math.Round(AmountPremittedDiscounts, 2);


                //صافى قيمة المبيعات
                //=======================================================
                var saleValueSafy = AmountSales - AmountReturnSales - AmountPremittedDiscounts;
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "صافــي قيمة المبيعات",
                    Whole = Math.Round(saleValueSafy, 2),
                    IsTotal = true
                });
                //اضافة حساب المبيعات الى جدول قائمة الدخل 
                incomeLists.Add(saleDetails);
                //اضافة حساب المرتجع المبيعات الى جدول قائمة الدخل 
                incomeLists.Add(saleReturnDetails);
                //اضافة حساب خصم مسموح به الى جدول قائمة الدخل 
                incomeLists.Add(premittedDiscountDetails);

                //مخزون اول المدة
                //=================================================
                //حساب المخزون من الاعدادات
                var stockAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var stockADetails = db.AccountsTrees.Where(x => x.Id == stockAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountStock = GetAuditBalances(dtFrom, dtTo, stockAccountId);
                var AmountStocks = AmountStock.ResultFrom + AmountStock.ResultTo;
                //var AmountStocks = GetAuditBalances(dtFrom, dtTo, stockAccountId).ResultFrom;//قديما
                stockADetails.Partial = Math.Round(AmountStocks, 2);



                //المشتريات
                //=================================================
                //حساب المشتريات من الاعدادات
                var purchaseAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var purchaseDetails = db.AccountsTrees.Where(x => x.Id == purchaseAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountPurchase = GetAuditBalances(dtFrom, dtTo, purchaseAccountId);
                var AmountPurchases = AmountPurchase.ResultFrom + AmountPurchase.ResultTo;
                //var AmountPurchases = GetAuditBalances(dtFrom, dtTo, purchaseAccountId).ResultFrom;//قديما
                purchaseDetails.Partial = Math.Round(AmountPurchases, 2);


                //مرددوات المشتريات 
                //==================================================
                //حساب مرددوات المشتريات من الاعدادات
                var purchaseReturnAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var purchaseReturnDetails = db.AccountsTrees.Where(x => x.Id == purchaseReturnAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountReturnPurchase = GetAuditBalances(dtFrom, dtTo, purchaseReturnAccountId);
                var AmountReturnPurchases = AmountReturnPurchase.ResultFrom + AmountReturnPurchase.ResultTo;
                //var AmountReturnPurchases = GetAuditBalances(dtFrom, dtTo, purchaseReturnAccountId).ResultTo;//قديما
                purchaseReturnDetails.Partial = Math.Round(AmountReturnPurchases, 2);


                //صافى المشتريات
                //=====================================================
                var purchaseSafy = AmountPurchases - AmountReturnPurchases;
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "صافــي المشتريات",
                    Partial = Math.Round(purchaseSafy, 2),
                    IsTotal = false
                });

                //خصم مكتسب به
                //======================================================
                //حساب خصم مسموح به من الاعدادات
                var earnedDiscountAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var earnedDiscountDetails = db.AccountsTrees.Where(x => x.Id == earnedDiscountAccountId).Select(x => new IncomeListDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountEarnedDiscount = GetAuditBalances(dtFrom, dtTo, earnedDiscountAccountId);
                var AmountEarnedDiscounts = AmountEarnedDiscount.ResultFrom + AmountEarnedDiscount.ResultTo;
                //var AmountEarnedDiscounts = GetAuditBalances(dtFrom, dtTo, earnedDiscountAccountId).ResultTo;//قديما
                earnedDiscountDetails.Partial = Math.Round(AmountEarnedDiscounts, 2);


                //تكلفة بضاعه مشتراه
                //=======================================================
                var purchaseValue = AmountPurchases - AmountReturnPurchases - AmountEarnedDiscounts;
                ////incomeLists.Add(new IncomeListDto
                ////{
                ////    AccountName = "تكلفة بضاعه مشتراه",
                ////    Partial = Math.Round(purchaseValue, 2),
                ////});

                //تكلفة بضاعة متاحة للبيع
                //=======================================================
                var salesStockValue = purchaseValue + AmountStocks;


                //مخزون آخر المدة
                //======================================================
                //احتساب متوسط سعر الشراء X الكميات
                //var itemPurchases = db.PurchaseInvoicesDetails.Where(x => !x.IsDeleted).Select(x=>x.ItemId).Distinct().ToList();
                //double stockLastValue = 0;
                //foreach (var item in itemPurchases)
                //{
                //    stockLastValue += BalanceService.GetItemCostCalculation((int)ItemCostCalculationCl.AveragePurchasePrice, item.Value) * BalanceService.GetBalanceByItemAllStores(item.Value);
                //}

                //                إجمالي المبيعات
                //(-) يطرح[المبيعات _ مسموحات المبيعات ومردوداتها والخصم المسموح به]
                //= صافي المبيعات.
                //إجمال المشتريات
                //(-) يطرح[المشتريات _ مسموحات المشتريات ومردوداتها والخصم المكتسب]
                //= صافي المشتريات.
                //تكلفة البضاعة المُتاحة للبيع = صافي المشتريات + بضاعة 1 / 1
                //إذن مخزون آخر المدة = صافي المبيعات _ تكلفة البضاعة المتاحة للبيع.
                // خانة ف فاتورة المبيعات (تكلفة المبيعات) سعر الشراء -سعر البيع 
                //اضافة الحساب الى جدول قائمة الدخل 
                double stockLastValue = 0;
                int itemCostCalculationId;
                if (!int.TryParse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue, out itemCostCalculationId))
                    itemCostCalculationId = (int)ItemCostCalculationCl.AveragePurchasePrice;
                //ارصدة الحالية
                var items = db.Items.Where(x => !x.IsDeleted && x.GroupBasicId != null);
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        double buyPrice = 0;
                        var itemBalance = BalanceService.GetBalance(item.Id, null, null);
                        if (itemBalance > 0)
                        {
                            buyPrice += _itemService.GetItemCostCalculation(itemCostCalculationId, item.Id);
                            stockLastValue += buyPrice * itemBalance;
                        }
                    }
                }

                //stockLastValue = salesStockValue - saleValueSafy; //تم إلغائها بهذا الشكل بناءا على المحاسب ا/ياسر


                //تكلفة المبيعات
                //=======================================================
                var salesValue = salesStockValue - stockLastValue;
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "تكلفة المبيعات",
                    Whole = Math.Round(salesValue, 2),
                    IsTotal=true
                });

                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "تكلفة بضاعه متاحة للبيع",
                    Partial = Math.Round(salesStockValue, 2),
                });
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مخزون آخر المدة",
                    Partial = Math.Round(stockLastValue, 2),
                });  
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مخزون أول المدة",
                    Partial = Math.Round(AmountStocks, 2),
                });
                //اضافة مخزون اول المدة الى جدول قائمة الدخل 
                incomeLists.Add(stockADetails);
                //اضافة حساب المشتريات الى جدول قائمة الدخل 
                incomeLists.Add(purchaseDetails);
                //اضافة حساب مرددوات المشتريات الى جدول قائمة الدخل 
                incomeLists.Add(purchaseReturnDetails);
                //اضافة حساب خصم مكتسب الى جدول قائمة الدخل 
                incomeLists.Add(earnedDiscountDetails);



                //مجمل الربح
                //=======================================================
                var totalprofit = saleValueSafy - salesValue;
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مجمل الربح",
                    Whole = Math.Round(totalprofit, 2),
                    IsTotal = true
                });

                //مصروف الاهلاك
                //======================================================
                //حساب مجمع الاهلاك من الاعدادات
                var accountTreeAssetsDepreciationComplex = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue);
                //اجمالى الارصدة من ميزان المراجعة 
                var AssetsDepreciationComple = GetAuditBalances(dtFrom, dtTo, accountTreeAssetsDepreciationComplex);
                var AssetsDepreciationComplex = AssetsDepreciationComple.ResultFrom + AssetsDepreciationComple.ResultTo;
                //var AssetsDepreciationComplex = GetAuditBalances(dtFrom, dtTo, accountTreeAssetsDepreciationComplex).ResultTo;//قديما

                //اجمالى المصروفات
                //======================================================
                //حساب المصروفات من الاعدادات
                ////var expenseAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralExpenses).FirstOrDefault().Id;
                ////////بيانات وتفاصيل الحساب 
                ////var expenseAccounts = AccountTreeService.GetAccountTreeLastChild(expenseAccountId);
                ////double AmountTotalExpense = 0;
                ////foreach (var item in expenseAccounts)
                ////{
                ////    ////اجمالى الارصدة من ميزان المراجعة 
                ////    var AmountTotalExpensee = GetAuditBalances(dtFrom, dtTo, item.Id);
                ////     AmountTotalExpense += (AmountTotalExpensee.ResultFrom + AmountTotalExpensee.ResultTo);
                ////    //AmountTotalExpense += GetAuditBalances(dtFrom, dtTo, item.Id).ResultFrom;//قديما
                ////}
                ////اضافة حساب مصروفات اخرى بدون مصروف االاهلاك الى جدول قائمة الدخل 
                ////incomeLists.Add(new IncomeListDto
                ////{
                ////    AccountName = "مصروفات اخرى ",
                ////    Partial = Math.Round(AmountTotalExpense - AssetsDepreciationComplex, 2),
                ////});//اضافة الحساب الى جدول قائمة الدخل 
                ////incomeLists.Add(new IncomeListDto
                ////{
                ////    AccountName = "اجمالى المصروفات",
                ////    Whole = Math.Round(AmountTotalExpense, 2),
                ////    IsTotal = true
                ////});
                var expenses = ReportsAccountTreeService.AuditBalances(dtFrom, dtTo, null, null, null, null, Lookups.GeneralExpenses);
                var expense = expenses.Where(x => x.AccountNumber == Lookups.GeneralExpenses).ToList();
                var totalExpenses = expense.Select(x => x.ResultFrom + x.ResultTo).Sum();
            //اضافة الحساب الى جدول قائمة الدخل 
            incomeLists.Add(new IncomeListDto
            {
                AccountName = "اجمالى المصروفات",
                Whole = Math.Round(totalExpenses, 2),
                IsTotal = true
            });
                //عرض الحساباب الرئيسية حتى المستوى الثالث
                foreach (var e in expense.FirstOrDefault().children)
                {
                    if(e.AccountLevel==2)
                    {
                        incomeLists.Add(new IncomeListDto
                        {
                            AccountName = e.AccountName,
                            Partial = Math.Round(e.ResultFrom + e.ResultTo, 2),
                            IsTotal = false
                        });
                        foreach (var e3 in e.children)
                        {
                            if (e3.AccountLevel == 3)
                            {
                                incomeLists.Add(new IncomeListDto
                                {
                                    AccountName = e3.AccountName,
                                    Partial = Math.Round(e3.ResultFrom + e3.ResultTo, 2),
                                    IsTotal = false,
                                    IsThirdLevel=true
                                });
                            }
                        }
                    }
                }
                //اضافة الحساب الى جدول قائمة الدخل 
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مصروف الاهلاك",
                    Partial = Math.Round(AssetsDepreciationComplex, 2),
                });

                //اجمالى الايرادات المتنوعة 
                //======================================================
                //حساب الايرادات من الاعدادات
                //var incomeAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralRevenus).FirstOrDefault().SValue);
                //var incomeAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralIncomes).FirstOrDefault().SValue);
                var incomeAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralIncomes).FirstOrDefault().Id;
                //بيانات وتفاصيل الحساب 
                //var incomeAccounts = db.AccountsTrees.Where(x => !x.IsDeleted && x.ParentId == incomeAccountId);
                var incomeAccounts = AccountTreeService.GetAccountTreeLastChild(incomeAccountId);
                double AmountTotalIncome = 0;
                foreach (var income in incomeAccounts)
                {
                    ////اجمالى الارصدة من ميزان المراجعة 
                    var incomee = GetAuditBalances(dtFrom, dtTo, income.Id);
                    AmountTotalIncome += (incomee.ResultFrom + incomee.ResultTo);
                    //AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, income.Id).ResultTo;//قديما
                }
                //الايرادات المتنوعه (الاستقطاعات وخصومات الموظفين
                var incomeiscellaneousRevenusAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue);
                ////اجمالى الارصدة من ميزان المراجعة 
                var AmountTotalIncomeee = GetAuditBalances(dtFrom, dtTo, incomeiscellaneousRevenusAccountId);
                 AmountTotalIncome += (AmountTotalIncomeee.ResultFrom + AmountTotalIncomeee.ResultTo);
                //AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, incomeiscellaneousRevenusAccountId).ResultTo;//قديما
                //ايرادات الصيانة 
                var maintenanceAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue);
                ////اجمالى الارصدة من ميزان المراجعة 
                var AmountTotalIncomee = GetAuditBalances(dtFrom, dtTo, maintenanceAccountId);
                AmountTotalIncome += (AmountTotalIncomee.ResultFrom + AmountTotalIncomee.ResultTo);
                //AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, maintenanceAccountId).ResultTo;قديما

                //اضافة الحساب الى جدول قائمة الدخل 
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "اجمالى الايرادات",
                    Whole = Math.Round(AmountTotalIncome, 2),
                    IsTotal = true
                });

                //صافى الربح
                //=======================================================
                var totalprofitSafy = totalprofit - totalExpenses + AmountTotalIncome;
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "صافى الربح",
                    Whole = Math.Round(totalprofit, 2),
                    IsTotal = true
                });
                return incomeLists;
            }
        }

        //ميزان المراجعة للحساب 
        public static AuditBalanceDto GetAuditBalances(DateTime? dtFrom, DateTime? dtTo, Guid? accountTreeId)
        {

            using (var db = new VTSaleEntities())
            {
                IQueryable<AccountsTree> accountTrees = null;
                if (accountTreeId != null && accountTreeId.HasValue)
                    accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == accountTreeId);
                else
                    accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted);

                List<int> initialBalanes = new List<int>
                {
                    (int)TransactionsTypesCl.InitialBalanceItem,
                    (int)TransactionsTypesCl.InitialBalanceAsset,
                    (int)TransactionsTypesCl.InitialBalanceBank,
                    (int)TransactionsTypesCl.InitialBalanceCustomer,
                    (int)TransactionsTypesCl.InitialBalanceFixedAsset,
                    (int)TransactionsTypesCl.InitialBalancePropertyRights,
                    (int)TransactionsTypesCl.InitialBalanceSafe,
                    (int)TransactionsTypesCl.InitialBalanceSupplier,
                };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var accounts = accountTrees
                    .Select(x => new AuditBalanceDto
                    {
                        Id = x.Id,
                        ParentId = x.ParentId,
                        AccountNumber = x.AccountNumber,
                        AccountName = x.AccountName,
                        IntialBalanceFrom = x.GeneralDailies.Where(g => !g.IsDeleted && initialBalanes.Contains(g.TransactionTypeId ?? 0) && g.TransactionDate >= dtFrom && g.TransactionDate <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        IntialBalanceTo = x.GeneralDailies.Where(g => !g.IsDeleted && initialBalanes.Contains(g.TransactionTypeId ?? 0) && g.TransactionDate >= dtFrom && g.TransactionDate <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),
                        ActionsFrom = x.GeneralDailies.Where(g => !g.IsDeleted && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && g.TransactionDate >= dtFrom && g.TransactionDate <= dtTo).Select(y => y.Debit).DefaultIfEmpty(0).Sum(),
                        ActionsTo = x.GeneralDailies.Where(g => !g.IsDeleted && !initialBalanes.Contains(g.TransactionTypeId ?? 0) && g.TransactionDate >= dtFrom && g.TransactionDate <= dtTo).Select(y => y.Credit).DefaultIfEmpty(0).Sum(),

                    }).OrderBy(x => new { x.ParentId, x.AccountName })
                    .ToList();
                accounts = accounts.Select(x => new AuditBalanceDto
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    AccountNumber = x.AccountNumber,
                    AccountName = x.AccountName,
                    IntialBalanceFrom = x.IntialBalanceFrom, //A1
                    IntialBalanceTo = x.IntialBalanceTo,//B1
                    ActionsFrom = x.ActionsFrom, //A2
                    ActionsTo = x.ActionsTo, //B2
                    SumFrom = x.ActionsFrom + x.IntialBalanceFrom,//C=A1+A2
                    SumTo = x.ActionsTo + x.IntialBalanceTo,//D=B1+B2
                   //if(C>D)? C-D  :0
                    ResultFrom = (x.ActionsFrom + x.IntialBalanceFrom) > (x.ActionsTo + x.IntialBalanceTo) ? ((x.ActionsFrom + x.IntialBalanceFrom) - (x.ActionsTo + x.IntialBalanceTo)) : 0,
                    //if(D>C)? D-C  :0
                    ResultTo = (x.ActionsTo + x.IntialBalanceTo) > (x.ActionsFrom + x.IntialBalanceFrom) ? ((x.ActionsTo + x.IntialBalanceTo) - (x.ActionsFrom + x.IntialBalanceFrom)) : 0

                }).ToList();
                stopwatch.Stop();//231 m
                return accounts.FirstOrDefault();
            }
        }

        #endregion

        #region حساب المتاجرة 
        public static List<TradingAccountDto> TradingAccount(DateTime? dtFrom, DateTime? dtTo)
        {
            using (var db = new VTSaleEntities())
            {
                List<TradingAccountDto> tradingAccounts = new List<TradingAccountDto>();
                var generalSetting = db.GeneralSettings.ToList();

                //مخزون اول المدة
                //=================================================
                //حساب المخزون من الاعدادات
                var stockAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var stockADetails = db.AccountsTrees.Where(x => x.Id == stockAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = "مخزون أول المدة", Order = 1 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountStocks = GetAuditBalances(dtFrom, dtTo, stockAccountId).ResultFrom;
                stockADetails.Debit = Math.Round(AmountStocks, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(stockADetails);

                //المشتريات
                //=================================================
                //حساب المشتريات من الاعدادات
                var purchaseAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var purchaseDetails = db.AccountsTrees.Where(x => x.Id == purchaseAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = "المشتريات", Order = 2 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountPurchases = GetAuditBalances(dtFrom, dtTo, purchaseAccountId).ResultFrom;
                purchaseDetails.Debit = Math.Round(AmountPurchases, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(purchaseDetails);
                //تستخدم مع حساب ارباح وخسائر
                tradingAccounts.Add(new TradingAccountDto { Debit = Math.Round(AmountPurchases, 2), Visiable = false, Order = 52 });


                //اجمالى المصروفات
                //======================================================
                //حساب المصروفات من الاعدادات
                //var expenseAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeExpensesAccount).FirstOrDefault().SValue);
                //var expenseAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralExpenses).FirstOrDefault().SValue);
                var expenseAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralExpenses).FirstOrDefault().Id;
                //بيانات وتفاصيل الحساب 
                var expenseAccount = db.AccountsTrees.Where(x => x.Id == expenseAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 3 }).FirstOrDefault();
                //var expenseAccount = db.AccountsTrees.Where(x => x.Id == expenseAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName,Order=3 }).FirstOrDefault();
                //var expenseAccounts = db.AccountsTrees.Where(x => !x.IsDeleted && x.ParentId == expenseAccountId);
                var expenseAccounts = AccountTreeService.GetAccountTreeLastChild(expenseAccountId);
                double AmountTotalExpense = 0;
                foreach (var item in expenseAccounts)
                {
                    ////اجمالى الارصدة من ميزان المراجعة 
                    AmountTotalExpense += GetAuditBalances(dtFrom, dtTo, item.Id).ResultFrom;
                }
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(new TradingAccountDto
                {
                    AccountNumber = expenseAccount.AccountNumber,
                    AccountName = "اجمالى المصروفات",
                    Debit = Math.Round(AmountTotalExpense, 2),
                    Order = 3
                });
                //تستخدم مع حساب ارباح وخسائر
                tradingAccounts.Add(new TradingAccountDto { Debit = Math.Round(AmountTotalExpense, 2), Visiable = false, Order = 51 });

                //مرددوات المبيعات بضاعه
                //==================================================
                //حساب مرددوات المبيعات من الاعدادات
                var saleReturnAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var saleReturnDetails = db.AccountsTrees.Where(x => x.Id == saleReturnAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 4 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountReturnSales = GetAuditBalances(dtFrom, dtTo, saleReturnAccountId).ResultFrom;
                saleReturnDetails.Debit = Math.Round(AmountReturnSales, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(saleReturnDetails);

                //خصم مسموح به
                //======================================================
                //حساب خصم مسموح به من الاعدادات
                var premittedDiscountAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var premittedDiscountDetails = db.AccountsTrees.Where(x => x.Id == premittedDiscountAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 5 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountPremittedDiscounts = GetAuditBalances(dtFrom, dtTo, premittedDiscountAccountId).ResultFrom;
                premittedDiscountDetails.Debit = Math.Round(AmountPremittedDiscounts, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(premittedDiscountDetails);



                //مبيعات بضاعه
                //=================================================
                //حساب المبيعات من الاعدادات
                var saleAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var saleDetails = db.AccountsTrees.Where(x => x.Id == saleAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 7 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountSales = GetAuditBalances(dtFrom, dtTo, saleAccountId).ResultTo;
                saleDetails.Credit = Math.Round(AmountSales, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(saleDetails);


                //صافى المبيعات
                //=====================================================
                var saleSafy = AmountSales - AmountReturnSales;

                //صافى قيمة المبيعات
                //=======================================================
                var saleValueSafy = AmountSales - AmountReturnSales - AmountPremittedDiscounts;
                //تستخدم مع حساب ارباح وخسائر
                tradingAccounts.Add(new TradingAccountDto { Credit = saleValueSafy, Visiable = false, Order = 50 });

                //مرددوات المشتريات 
                //==================================================
                //حساب مرددوات المشتريات من الاعدادات
                var purchaseReturnAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var purchaseReturnDetails = db.AccountsTrees.Where(x => x.Id == purchaseReturnAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 8 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountReturnPurchases = GetAuditBalances(dtFrom, dtTo, purchaseReturnAccountId).ResultTo;
                purchaseReturnDetails.Credit = Math.Round(AmountReturnPurchases, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(purchaseReturnDetails);

                //صافى المشتريات
                //=====================================================
                var purchaseSafy = AmountPurchases - AmountReturnPurchases;

                //خصم مكتسب 
                //======================================================
                //حساب خصم مكتسب به من الاعدادات
                var earnedDiscountAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue);
                //بيانات وتفاصيل الحساب 
                var earnedDiscountDetails = db.AccountsTrees.Where(x => x.Id == earnedDiscountAccountId).Select(x => new TradingAccountDto { ParentId = x.ParentId, AccountNumber = x.AccountNumber, AccountName = x.AccountName, Order = 10 }).FirstOrDefault();
                //اجمالى الارصدة من ميزان المراجعة 
                var AmountEarnedDiscounts = GetAuditBalances(dtFrom, dtTo, earnedDiscountAccountId).ResultTo;
                earnedDiscountDetails.Credit = Math.Round(AmountEarnedDiscounts, 2);
                //اضافة الحساب الى جدول حساب المتاجرة 
                tradingAccounts.Add(earnedDiscountDetails);

                //تكلفة بضاعه مشتراه
                //=======================================================
                var purchaseValue = AmountPurchases - AmountReturnPurchases - AmountEarnedDiscounts;

                //تكلفة بضاعة متاحة للبيع
                //=======================================================
                var salesStockValue = purchaseValue + AmountStocks;

                //اضافة الحساب الى جدول حساب المتاجرة 
                double stockLastValue = 0;
                stockLastValue = salesStockValue - saleValueSafy;
                tradingAccounts.Add(new TradingAccountDto
                {
                    AccountName = "مخزون آخر المدة",
                    Credit = Math.Round(stockLastValue, 2),
                    Order = 9
                });

                //تكلفة المبيعات
                //=======================================================
                var salesValue = (purchaseValue + AmountStocks) - stockLastValue;

                //مجمل الربح
                //=======================================================
                var totalprofit = saleValueSafy - salesValue;
                tradingAccounts.Add(new TradingAccountDto
                {
                    AccountName = "مجمل الربح",
                    Debit = Math.Round(totalprofit, 2),
                    Order = 6
                });

                //مجمل الخسارة
                //=======================================================
                var totalLoss = saleDetails.Credit + purchaseReturnDetails.Credit + Math.Round(stockLastValue, 2) + earnedDiscountDetails.Credit;
                tradingAccounts.Add(new TradingAccountDto
                {
                    AccountName = "مجمل الخسارة",
                    Credit = Math.Round(totalLoss, 2),
                    Order = 11
                });
                return tradingAccounts;

            }

        }
        #endregion
        #region حساب ارباح وخسائر 
        public static List<TradingAccountDto> ProfitLossAccount(DateTime? dtFrom, DateTime? dtTo)
        {
            using (var db = new VTSaleEntities())
            {
                List<TradingAccountDto> profitLossAccount = new List<TradingAccountDto>();
                var generalSetting = db.GeneralSettings.ToList();
                var tradingAccount = TradingAccount(dtFrom, dtTo);
                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "ح/ المتاجرة (مجمل خسارة)",
                    Debit = tradingAccount.Where(x => x.Order == 11).FirstOrDefault().Credit,
                    Order = 1,

                });

                var expenSafy = tradingAccount.Where(x => x.Order == 52).FirstOrDefault().Debit - tradingAccount.Where(x => x.Order == 51).FirstOrDefault().Debit;
                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "ح/ المصروفات بخلاف ح/ مشتريات بضائع بغرض البيع",
                    Debit = expenSafy,
                    Order = 2,
                });
                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "صافى الربح",
                    Debit = tradingAccount.Where(x => x.Order == 11).FirstOrDefault().Credit + expenSafy,
                    Order = 3,
                });
                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "ح/ المتاجرة (مجمل ربح)",
                    Debit = tradingAccount.Where(x => x.Order == 6).FirstOrDefault().Debit,
                    Order = 4,
                });
                //اجمالى الايرادات المتنوعة 
                //======================================================
                //حساب الايرادات من الاعدادات
                //var incomeAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralRevenus).FirstOrDefault().SValue);
                //var incomeAccountId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralIncomes).FirstOrDefault().SValue);
                var incomeAccountId = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == Lookups.GeneralIncomes).FirstOrDefault().Id;
                //بيانات وتفاصيل الحساب 
                //var incomeAccounts = db.AccountsTrees.Where(x => !x.IsDeleted && x.ParentId == incomeAccountId);
                var incomeAccounts = AccountTreeService.GetAccountTreeLastChild(incomeAccountId);
                double AmountTotalIncome = 0;
                foreach (var income in incomeAccounts)
                {
                    ////اجمالى الارصدة من ميزان المراجعة 
                    AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, income.Id).ResultTo;
                }
                //الايرادات المتنوعه (الاستقطاعات وخصومات الموظفين
                var incomeiscellaneousRevenusAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue);
                ////اجمالى الارصدة من ميزان المراجعة 
                AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, incomeiscellaneousRevenusAccountId).ResultTo;
                //ايرادات الصيانة 
                var maintenanceAccountId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue);
                ////اجمالى الارصدة من ميزان المراجعة 
                AmountTotalIncome += GetAuditBalances(dtFrom, dtTo, maintenanceAccountId).ResultTo;
                var amountTotalIncome = Math.Round(AmountTotalIncome, 2);

                var totalIncom = amountTotalIncome - tradingAccount.Where(x => x.Order == 50).FirstOrDefault().Credit;
                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "ح/ الايرادات بخلاف ح/ اجمالي مبيعات بضائع مشتراه/صافى قيمة المبيعات ",
                    Credit = totalIncom,
                    Order = 5,
                });

                profitLossAccount.Add(new TradingAccountDto
                {
                    AccountName = "صافى الخسارة ",
                    Credit = totalIncom + tradingAccount.Where(x => x.Order == 6).FirstOrDefault().Debit,
                    Order = 6,
                });

                return profitLossAccount;

            }

        }
        #endregion


        #region المركز المالى 
        public static List<IncomeListDto> FinancialCenter(DateTime? dtFrom, DateTime? dtTo)
        {

            using (var db = new VTSaleEntities())
            {
                List<IncomeListDto> financialCenter = new List<IncomeListDto>();
                //AuditBalanceDto auditBalance = new AuditBalanceDto();
                double amount = 0;
                var generalSetting = db.GeneralSettings.ToList();
                //الاصول غير المتداولة
                //الاصول الثابتة
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "الاصول غير المتداولة",
                    Num = 1
                });
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "الاصول الثابتة",
                    Num = 1
                });
                //ميزان المراجعة بالارصدة
                //var auditBalances = AuditBalances(dtFrom, dtTo, null, null);

                //حسابات الاصول الثابتة
                var FixedAssets = AccountTreeService.GetFixedAssets();
                double totalFixedAssets = 0;
                foreach (var item in FixedAssets)
                {
                    amount = 0;
                    var getLast = AccountTreeService.GetAccountTreeLastChild(item.Id);
                    foreach (var last in getLast)
                    {
                        var auditBalance01 = GetAuditBalances(dtFrom, dtTo, last.Id);
                        if (auditBalance01 != null)
                        {
                            amount += auditBalance01.ResultFrom + auditBalance01.ResultTo;

                        }
                    }
                    //auditBalance = auditBalances.Where(x => x.Id == item.Id).FirstOrDefault();

                        financialCenter.Add(new IncomeListDto
                        {
                            AccountName = item.AccountName,
                            Amount = Math.Round(amount, 2),
                            Num = 1
                        });
                        totalFixedAssets += amount;
                    
                }
                //مجمع اهلاك الاصول الثابتة
                //var assetsDepreciationComplexAccId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue);
                //var accNumAssetsDepreciationComplex = db.AccountsTrees.Where(x => x.Id == assetsDepreciationComplexAccId).FirstOrDefault().AccountNumber;
                //double totalassetsDepreciationComplexs = 0;

                //auditBalance = auditBalances.Where(x => x.Id == assetsDepreciationComplexAccId).FirstOrDefault();
                //totalassetsDepreciationComplexs = auditBalance.ResultFrom > auditBalance.ResultTo ? auditBalance.ResultFrom : auditBalance.ResultTo;
                //financialCenter.Add(new IncomeListDto
                //{
                //    AccountNumber = accNumAssetsDepreciationComplex,
                //    AccountName = "مجمع اهلاك الاصول الثابتة",
                //    Amount = Math.Round(totalassetsDepreciationComplexs, 2),
                //    Num = 1
                //});

                //حسابات مخصص الاهلاك
                var destructionAllowanceId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDestructionAllowance).FirstOrDefault().SValue);
                var accNum = db.AccountsTrees.Where(x => x.Id == destructionAllowanceId).FirstOrDefault().AccountNumber;
                var destructionAccounts = AccountTreeService.GetAccountTreeLastChild(destructionAllowanceId);
                double totalDestructionAllowances = 0;
                foreach (var item in destructionAccounts)
                {
                    var auditBalance02 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance02 != null)
                    {
                        amount = auditBalance02.ResultFrom + auditBalance02.ResultTo;
                        totalDestructionAllowances += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = accNum,
                    AccountName = "اجمالى مجمع الاهلاك",
                    Amount = Math.Round(totalDestructionAllowances, 2),
                    Num = 1
                });
                //صافى الاصول الثابية
                //=========================
                //var generalFixedAssetId = int.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralFixedAssets).FirstOrDefault().SValue);
                //var accNumGeneralFixedAsset = db.AccountsTrees.Where(x => x.Id == generalFixedAssetId).FirstOrDefault().AccountNumber;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.GeneralFixedAssets,
                    AccountName = "صافى الاصول الثابتة",
                    Amount = Math.Round(totalFixedAssets - totalDestructionAllowances, 2),
                    Num = 1,
                    IsTotal = true,
                });

                //تكوين استثمارى
                //=========================
                //var accIdinvestmentFormation = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.InvestmentFormation).FirstOrDefault().Id;
                //var investmentFormation = AccountTreeService.GetAccountTreeLastChild(accIdinvestmentFormation);
                //double totalinvestmentFormations = 0;
                //foreach (var item in investmentFormation)
                //{
                //    auditBalance = auditBalances.Where(x => x.Id == item.Id).FirstOrDefault();
                //    amount = auditBalance.ResultFrom > auditBalance.ResultTo ? auditBalance.ResultFrom : auditBalance.ResultTo;
                //    totalinvestmentFormations += amount;
                //}
                //financialCenter.Add(new IncomeListDto
                //{
                //    AccountNumber = Lookups.InvestmentFormation,
                //    AccountName = "تكوين استثمارى",
                //    Amount = Math.Round(totalinvestmentFormations, 2),
                //    Num = 1
                //});
                //انفاق استثمارى
                //=========================
                var accIdinvestmentSpending = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.InvestmentSpending).FirstOrDefault().Id;
                var investmentSpendings = AccountTreeService.GetAccountTreeLastChild(accIdinvestmentSpending);
                double totalinvestmentSpendings = 0;
                foreach (var item in investmentSpendings)
                {
                    var auditBalance0 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance0 != null)
                    {
                        amount = auditBalance0.ResultFrom + auditBalance0.ResultTo;
                        totalinvestmentSpendings += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.InvestmentSpending,
                    AccountName = "انفاق استثمارى",
                    Amount = Math.Round(totalinvestmentSpendings, 2),
                    Num = 1
                });
                //اجمالى مشروعات تحت التنفيذ
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 12,
                    AccountName = "اجمالى مشروعات تحت التنفيذ",
                    Amount = Math.Round(/*totalinvestmentFormations +*/ totalinvestmentSpendings, 2),
                    Num = 1,
                    IsTotal = true,
                });

                //مجموع الأصول غير المتداولة
                //=================================
                var totalNonAssets = (/*totalinvestmentFormations +*/ totalinvestmentSpendings + totalFixedAssets) - totalDestructionAllowances;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 12,
                    AccountName = "مجموع الأصول غير المتداولة",
                    Amount = Math.Round(totalNonAssets, 2),
                    Num = 1,
                    IsTotal = true,
                });
                //الاصول المتداولة
                //مخزون
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "الاصول المتداولة",
                });
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مخزون",
                    Num = 3
                });
                //مخزن خامات ومواد ووقود وقطع غيار 
                //=========================
                var accIdFuelSparePart = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.StorematerialsFuelSparePart).FirstOrDefault().Id;
                double totalStorematerialsFuelSparePart = 0;
                var auditBalance = GetAuditBalances(dtFrom, dtTo, accIdFuelSparePart);
                if(auditBalance!=null)
                    totalStorematerialsFuelSparePart = auditBalance.ResultFrom + auditBalance.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.StorematerialsFuelSparePart,
                    AccountName = "مخزن خامات ومواد ووقود وقطع غيار",
                    Amount = Math.Round(totalStorematerialsFuelSparePart, 2),
                    Num = 3
                });
                //مخزن بضائع مشتراه بغرض البيع 
                //=========================
                var AccIdWarehouseMerchandisePurchasedSale = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                double totalWarehouseMerchandisePurchasedSale = 0;
                var auditBalance2 = GetAuditBalances(dtFrom, dtTo, AccIdWarehouseMerchandisePurchasedSale);
                if (auditBalance2 != null)
                    totalWarehouseMerchandisePurchasedSale = auditBalance2.ResultFrom + auditBalance2.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 122,
                    AccountName = "مخزن بضائع مشتراه بغرض البيع",
                    Amount = Math.Round(totalWarehouseMerchandisePurchasedSale, 2),
                    Num = 3
                });
                //إجمالى المخزون
                //=================================
                var totalStors = totalStorematerialsFuelSparePart + totalWarehouseMerchandisePurchasedSale;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 12,
                    AccountName = "إجمالى المخزون",
                    Amount = Math.Round(totalStors, 2),
                    Num = 3,
                    IsTotal = true,
                });
                //العملاء
                var customerAccId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustomerAccount).FirstOrDefault().SValue);
                var accNumCust = db.AccountsTrees.Where(x => x.Id == customerAccId).FirstOrDefault().AccountNumber;
                var customerAccounts = AccountTreeService.GetAccountTreeLastChild(customerAccId);
                double totalCustomers = 0;
                foreach (var item in customerAccounts)
                {
                    var auditBalance1 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance1 != null)
                    {
                        amount = auditBalance1.ResultFrom + auditBalance1.ResultTo;
                        totalCustomers += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = accNumCust,
                    AccountName = "العمـــلاء",
                    Amount = Math.Round(totalCustomers, 2),
                    Num = 4
                });
                //مخصص ديون مشكوك فى تحصيلها
                //=========================
                var accIdNoCollection = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.NoCollection).FirstOrDefault().Id;
                double totalNoCollection = 0;
                var auditBalance3 = GetAuditBalances(dtFrom, dtTo, accIdNoCollection);
                if(auditBalance3!=null)
                  totalNoCollection = auditBalance3.ResultFrom + auditBalance3.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.NoCollection,
                    AccountName = "مخصص ديون مشكوك فى تحصيلها",
                    Amount = Math.Round(totalNoCollection, 2),
                    Num = 5
                });
                //صافى العملاء
                //=================================
                var customersSafy = totalCustomers + totalNoCollection;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 12,
                    AccountName = "صافى العملاء",
                    Amount = Math.Round(customersSafy, 2),
                    IsTotal = true,
                });
                //حسابات مدينة لدى المصالح والهيئات
                var accNumReceivableDepartment = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsReceivableDepartment).FirstOrDefault().Id;
                var AccountsReceivableDepartments = AccountTreeService.GetAccountTreeLastChild(accNumReceivableDepartment);
                double totalAccountsReceivables = 0;
                foreach (var item in AccountsReceivableDepartments)
                {
                    var auditBalance4 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance4!=null)
                    {
                        amount = auditBalance4.ResultFrom + auditBalance4.ResultTo;
                        totalAccountsReceivables += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsReceivableDepartment,
                    AccountName = "حسابات مدينة لدى المصالح والهيئات",
                    Amount = Math.Round(totalAccountsReceivables, 2),

                });
                //ايرادات مستحقة التحصيل
                var accNumRevenueReceivable = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.RevenueReceivable).FirstOrDefault().Id;
                var AccountsRevenueReceivables = AccountTreeService.GetAccountTreeLastChild(accNumRevenueReceivable);
                double totalRevenueReceivables = 0;
                foreach (var item in AccountsRevenueReceivables)
                {
                    var auditBalance4 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance4!=null)
                    {
                        amount = auditBalance4.ResultFrom + auditBalance4.ResultTo ;
                        totalRevenueReceivables += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.RevenueReceivable,
                    AccountName = "ايرادات مستحقة التحصيل",
                    Amount = Math.Round(totalRevenueReceivables, 2),
                    Num = 6
                });

                //حسابات مدينة أخرى
                var accNumOtherDebitAccounts = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.OtherDebitAccounts).FirstOrDefault().Id;
                var AccountsOtherDebitAccounts = AccountTreeService.GetAccountTreeLastChild(accNumOtherDebitAccounts);
                double totalOtherDebitAccounts = 0;
                foreach (var item in AccountsRevenueReceivables)
                {
                    var auditBalance4 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if(auditBalance4!=null)
                    {
                        amount = auditBalance4.ResultFrom + auditBalance4.ResultTo;
                        totalOtherDebitAccounts += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.OtherDebitAccounts,
                    AccountName = "حسابات مدينة أخرى/السلف",
                    Amount = Math.Round(totalOtherDebitAccounts, 2),
                    Num = 7
                });
                //حسابات مدينة لدى الشركات القابضة / التابعة / الشقيقة
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 173,
                    AccountName = "حسابات مدينة لدى الشركات القابضة / التابعة / الشقيقة",
                    Num = 9
                });
                //حسابات مدينة لدى الشركات القابضة
                //=========================
                var accIdAccountsReceivableHolding = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsReceivableHolding).FirstOrDefault().Id;
                double totalAccountsReceivableHolding = 0;
                var auditBalance5 = GetAuditBalances(dtFrom, dtTo, accIdAccountsReceivableHolding);
                if (auditBalance5 != null)
                    totalAccountsReceivableHolding = auditBalance5.ResultFrom + auditBalance5.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsReceivableHolding,
                    AccountName = "حسابات مدينة لدى الشركات القابضة",
                    Amount = Math.Round(totalAccountsReceivableHolding, 2),
                    Num = 9
                });
                //حسابات مدينة لدى الشركات التابعة/الشقيقة
                //=========================
                var accIdAccountsReceivableSubsidiaries = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsReceivableSubsidiaries).FirstOrDefault().Id;
                double totalAccountsReceivableSubsidiaries = 0;
                var auditBalance6 = GetAuditBalances(dtFrom, dtTo, accIdAccountsReceivableSubsidiaries);
                if (auditBalance6 != null)
                    totalAccountsReceivableSubsidiaries = auditBalance6.ResultFrom + auditBalance6.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsReceivableSubsidiaries,
                    AccountName = "حسابات مدينة لدى الشركات التابعة/الشقيقة",
                    Amount = Math.Round(totalAccountsReceivableSubsidiaries, 2),
                    Num = 9
                });

                //مصروفات مدفوعة مقدما
                //=========================
                var accIdExpensesPaidAdvance = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.ExpensesPaidAdvance).FirstOrDefault().Id;
                double totalExpensesPaidAdvance = 0;
                var auditBalance7 = GetAuditBalances(dtFrom, dtTo, accIdExpensesPaidAdvance);
                if (auditBalance7 != null)
                    totalExpensesPaidAdvance = auditBalance7.ResultFrom +auditBalance7.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.ExpensesPaidAdvance,
                    AccountName = "مصروفات مدفوعة مقدما",
                    Amount = Math.Round(totalExpensesPaidAdvance, 2),
                    Num = 9
                });

                //إجمالى عملاء و أوراق قبض ومدينون أخرون
                //=================================
                var totalReceiptsOtherDebtors = totalAccountsReceivables + totalRevenueReceivables + totalOtherDebitAccounts + totalAccountsReceivableHolding + totalAccountsReceivableSubsidiaries + totalExpensesPaidAdvance;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 17,
                    AccountName = "إجمالى عملاء و أوراق قبض ومدينون أخرون",
                    Amount = Math.Round(customersSafy + totalReceiptsOtherDebtors, 2),
                    IsTotal = true,
                });
                //استثمارات وأوراق مالية متداولة (اذون اخزانة)
                //=========================
                var accIdTradedInvestmentSecurities = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.TradedInvestmentSecurities).FirstOrDefault().Id;
                double totalTradedInvestmentSecurities = 0;
                var auditBalance8 = GetAuditBalances(dtFrom, dtTo, accIdTradedInvestmentSecurities);
                if (auditBalance8 != null)
                    totalTradedInvestmentSecurities = auditBalance8.ResultFrom + auditBalance8.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.TradedInvestmentSecurities,
                    AccountName = "استثمارات وأوراق مالية متداولة (اذون اخزانة)",
                    Amount = Math.Round(totalTradedInvestmentSecurities, 2),
                    Num = 10,
                    IsTotal = true
                });
                //ودائع بالبنوك لأجل أو اخطار سابق
                //=========================
                var accIdBankDepositForTerm = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.BankDepositForTerm).FirstOrDefault().Id;
                double totalBankDepositForTerm = 0;
                var auditBalance9 = GetAuditBalances(dtFrom, dtTo, accIdBankDepositForTerm);
                if (auditBalance9 != null)
                    totalBankDepositForTerm = auditBalance9.ResultFrom + auditBalance9.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.BankDepositForTerm,
                    AccountName = "ودائع بالبنوك لأجل أو اخطار سابق",
                    Amount = Math.Round(totalBankDepositForTerm, 2),
                    Num = 11
                });
                //غطاء ضمان
                //=========================
                var accIdWarrantyCover = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.WarrantyCover).FirstOrDefault().Id;
                double totalWarrantyCover = 0;
                var auditBalance10 = GetAuditBalances(dtFrom, dtTo, accIdAccountsReceivableHolding);
                if (auditBalance10 != null)
                    totalWarrantyCover = auditBalance10.ResultFrom + auditBalance10.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.WarrantyCover,
                    AccountName = "غطاء ضمان",
                    Amount = Math.Round(totalWarrantyCover, 2),
                    Num = 11
                });
                //حسابات جارية بالبنوك
                var bankAccountAccId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue);
                var accNumBankAccount = db.AccountsTrees.Where(x => x.Id == bankAccountAccId).FirstOrDefault().AccountNumber;
                var bankAccounts = AccountTreeService.GetAccountTreeLastChild(bankAccountAccId);
                double totalBankAccounts = 0;
                foreach (var item in bankAccounts)
                {
                    var auditBalance11 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance11 != null)
                    {
                      amount = auditBalance11.ResultFrom + auditBalance11.ResultTo ;
                      totalBankAccounts += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = accNumBankAccount,
                    AccountName = "حسابات جارية بالبنوك",
                    Amount = Math.Round(totalBankAccounts, 2),
                    Num = 11
                });
                //نقدية بالصندوق   
                var safeAccountAccId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue);
                var accNumsafe = db.AccountsTrees.Where(x => x.Id == safeAccountAccId).FirstOrDefault().AccountNumber;
                var safeAccounts = AccountTreeService.GetAccountTreeLastChild(safeAccountAccId);
                double totalSafeAccounts = 0;
                foreach (var item in safeAccounts)
                {
                    var auditBalance12 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance12 != null)
                    {
                        amount = auditBalance12.ResultFrom + auditBalance12.ResultTo ;
                    totalSafeAccounts += amount;

                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = accNumsafe,
                    AccountName = "نقدية بالصندوق",
                    Amount = Math.Round(totalSafeAccounts, 2),
                    Num = 11
                });

                //إجمالى نقدية وأرصدة لدى البنوك
                //=================================
                var safyAmount = totalBankDepositForTerm + totalWarrantyCover + totalBankAccounts + totalSafeAccounts;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 19,
                    AccountName = "إجمالى نقدية وأرصدة لدى البنوك",
                    Amount = Math.Round(safyAmount, 2),
                    IsTotal = true,
                });
                //مجموع الأصول المتداولة
                //=================================
                var totalAssets = totalStors + customersSafy + totalReceiptsOtherDebtors + totalTradedInvestmentSecurities + safyAmount;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع الأصول المتداولة",
                    Amount = Math.Round(totalAssets, 2),
                    IsTotal = true,
                });
                //مجموع الأصول 
                //=================================
                var safyAssets = totalNonAssets + totalAssets;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع الأصول ",
                    Amount = Math.Round(safyAssets, 2),
                    IsTotal = true,
                });
                // 
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = " ",
                    Amount = 0,
                });
                //حقوق الملكية 
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "حقوق الملكية ",
                    Amount = 0,
                    IsTotal = true,
                });
                //رأس المال المدفوع
                //=========================
                var accIdPaidCapital = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.PaidCapital).FirstOrDefault().Id;
                var paidCapitals = AccountTreeService.GetAccountTreeLastChild(accIdPaidCapital);
                double totalPaidCapitals = 0;
                foreach (var item in paidCapitals)
                {
                    var auditBalance13 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance13 != null)
                    {
                        amount = auditBalance13.ResultFrom + auditBalance13.ResultTo ;
                    totalPaidCapitals += amount;

                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.PaidCapital,
                    AccountName = "رأس المال المدفوع",
                    Amount = Math.Round(totalPaidCapitals, 2),
                    Num = 12
                });
                //احتياطى رأسمالى
                //=========================
                var accIdCapitalReserve = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.CapitalReserve).FirstOrDefault().Id;
                double totalCapitalReserve = 0;
                var auditBalance14 = GetAuditBalances(dtFrom, dtTo, accIdCapitalReserve);
                if (auditBalance14 != null)
                    totalCapitalReserve = auditBalance14.ResultFrom + auditBalance14.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.CapitalReserve,
                    AccountName = "احتياطى رأسمالى",
                    Amount = Math.Round(totalCapitalReserve, 2),
                    Num = 12
                });
                //احتياطيات أخرى
                //=========================
                var accIdOtherReserves = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.OtherReserves).FirstOrDefault().Id;
                double totalOtherReserves = 0;
                var auditBalance15 = GetAuditBalances(dtFrom, dtTo, accIdOtherReserves);
                if (auditBalance15 != null)
                    totalOtherReserves = auditBalance15.ResultFrom + auditBalance15.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.OtherReserves,
                    AccountName = "احتياطيات أخرى",
                    Amount = Math.Round(totalOtherReserves, 2),
                    Num = 12
                });
                //إجمالى الاحتياطيات  
                //=================================
                var totalReserves = totalCapitalReserve + totalOtherReserves;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 22,
                    AccountName = "إجمالى الاحتياطيات  ",
                    Amount = Math.Round(totalReserves, 2),
                    IsTotal = true,
                });
                //أرباح (خسائر) مرحلة
                //=========================
                var accIdProfitLossStage = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.ProfitLossStage).FirstOrDefault().Id;
                double totalProfitLossStages = 0;
                var auditBalance16 = GetAuditBalances(dtFrom, dtTo, accIdProfitLossStage);
                if (auditBalance16 != null)
                    totalProfitLossStages = auditBalance16.ResultFrom + auditBalance16.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.ProfitLossStage,
                    AccountName = "أرباح (خسائر) مرحلة",
                    Amount = Math.Round(totalProfitLossStages, 2),
                    Num = 13
                });
                //مجموع حقوق الملكية   
                //=================================
                var totalPropertyRights = totalReserves + totalProfitLossStages;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع حقوق الملكية",
                    Amount = Math.Round(totalPropertyRights, 2),
                    IsTotal = true,
                });
                //الالتزمات غير المتداولة   
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "الالتزمات غير المتداولة",
                });
                //التزامات طويلة الأجل (مقابل مشروعات تحت التنفيذ)
                //=========================
                var accIdLongTermCommitment = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.LongTermCommitments).FirstOrDefault().Id;
                double totalLongTermCommitments = 0;
                var auditBalance17 = GetAuditBalances(dtFrom, dtTo, accIdLongTermCommitment);
                if (auditBalance17 != null)
                    totalLongTermCommitments = auditBalance17.ResultFrom + auditBalance17.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.LongTermCommitments,
                    AccountName = "التزامات طويلة الأجل (مقابل مشروعات تحت التنفيذ)",
                    Amount = Math.Round(totalLongTermCommitments, 2),
                    Num = 14
                });
                //التزامات طويلة الأجل (مقابل أصول)
                //=========================
                var accIdLongTermLiabilities = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.LongTermLiabilities).FirstOrDefault().Id;
                double totalLongTermLiabilities = 0;
                var auditBalance18 = GetAuditBalances(dtFrom, dtTo, accIdLongTermLiabilities);
                if (auditBalance18 != null)
                    totalLongTermLiabilities = auditBalance18.ResultFrom + auditBalance18.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.LongTermLiabilities,
                    AccountName = "التزامات طويلة الأجل (مقابل أصول)",
                    Amount = Math.Round(totalLongTermLiabilities, 2),
                    Num = 14
                });
                //حسابات دائنة اخرى
                //=========================
                var accIdOtherCreditAccounts = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.OtherCreditAccounts).FirstOrDefault().Id;
                double totalOtherCreditAccounts = 0;
                var auditBalance19 = GetAuditBalances(dtFrom, dtTo, accIdOtherCreditAccounts);
                if (auditBalance19 != null)
                    totalOtherCreditAccounts = auditBalance19.ResultFrom + auditBalance19.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.OtherCreditAccounts,
                    AccountName = "حسابات دائنة اخرى",
                    Amount = Math.Round(totalOtherCreditAccounts, 2),
                });
                //مجموع الالتزمات غير المتداولة   
                //=================================
                var TotalNonCurrentLiabilities = totalLongTermCommitments + totalLongTermLiabilities + totalOtherCreditAccounts;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع الالتزمات غير المتداولة",
                    Amount = Math.Round(TotalNonCurrentLiabilities, 2),
                    IsTotal = true,
                });
                //الالتزمات المتداولة   
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "الالتزمات المتداولة",
                });
                //مخصص ضرائب متنازع عليها
                //=========================
                var accIdDisputedTaxCustom = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.DisputedTaxCustom).FirstOrDefault().Id;
                double totalDisputedTaxCustoms = 0;
                var auditBalance20 = GetAuditBalances(dtFrom, dtTo, accIdDisputedTaxCustom);
                if (auditBalance20 != null)
                    totalDisputedTaxCustoms = auditBalance20.ResultFrom + auditBalance20.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.DisputedTaxCustom,
                    AccountName = "مخصص ضرائب متنازع عليها)",
                    Amount = Math.Round(totalDisputedTaxCustoms, 2),
                    Num = 15
                });
                //مخصص المطالبات والمنازعات
                //=========================
                var accIdProvisionForClaims = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.ProvisionForClaims).FirstOrDefault().Id;
                double totalProvisionForClaims = 0;
                var auditBalance21 = GetAuditBalances(dtFrom, dtTo, accIdProvisionForClaims);
                if (auditBalance21 != null)
                    totalProvisionForClaims = auditBalance21.ResultFrom + auditBalance21.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.ProvisionForClaims,
                    AccountName = "مخصص المطالبات والمنازعات",
                    Amount = Math.Round(totalProvisionForClaims, 2),
                    Num = 15
                });
                //مخصصات أخرى
                //=========================
                var accIdOtherAllowances = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.OtherAllowances).FirstOrDefault().Id;
                double totalOtherAllowances = 0;
                var auditBalance22 = GetAuditBalances(dtFrom, dtTo, accIdOtherAllowances);
                if (auditBalance22 != null)
                    totalOtherAllowances = auditBalance22.ResultFrom + auditBalance22.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.OtherAllowances,
                    AccountName = "مخصصات أخرى",
                    Amount = Math.Round(totalOtherAllowances, 2),
                    Num = 15
                });
                //اجمالى المخصصات     
                //=================================
                var totalAllowances = totalDisputedTaxCustoms + totalProvisionForClaims + totalOtherAllowances;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "اجمالى المخصصات",
                    Amount = Math.Round(totalAllowances, 2),
                    IsTotal = true,
                });
                // موردون وأوراق دفع ودائنون أخرون
                //=================================
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = " موردون وأوراق دفع ودائنون أخرون",
                });
                //موردون
                var supplierAccId = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSupplierAccount).FirstOrDefault().SValue);
                var accNumSupp = db.AccountsTrees.Where(x => x.Id == supplierAccId).FirstOrDefault().AccountNumber;
                var supplierAccounts = AccountTreeService.GetAccountTreeLastChild(supplierAccId);
                double totalSuppliers = 0;
                foreach (var item in supplierAccounts)
                {
                    var auditBalance30 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance30 != null)
                    {
                       amount = auditBalance30.ResultFrom + auditBalance30.ResultTo;
                       totalSuppliers += amount;
                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = accNumSupp,
                    AccountName = "موردون",
                    Amount = Math.Round(totalSuppliers, 2),
                    Num = 8
                });
                //حسابات دائنة لدى المصالح والهيئات
                var accNumAccountsPayable = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsPayable).FirstOrDefault().Id;
                var AccountsPayables = AccountTreeService.GetAccountTreeLastChild(accNumAccountsPayable);
                double totalAccountsPayables = 0;
                foreach (var item in AccountsPayables)
                {
                    var auditBalance23 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance23 != null)
                    {
                        amount = auditBalance23.ResultFrom + auditBalance23.ResultTo;
                    totalAccountsPayables += amount;

                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsPayable,
                    AccountName = "حسابات دائنة لدى المصالح والهيئات",
                    Amount = Math.Round(totalAccountsPayables, 2),
                    Num = 16
                });
                //مصروفات مستحقة التحصيل
                var accNumExpenseReceivable = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.ExpenseReceivable).FirstOrDefault().Id;
                var AccountsExpenseReceivables = AccountTreeService.GetAccountTreeLastChild(accNumExpenseReceivable);
                double totalExpenseReceivables = 0;
                foreach (var item in AccountsExpenseReceivables)
                {
                    var auditBalance24 = GetAuditBalances(dtFrom, dtTo, item.Id);
                    if (auditBalance24 != null)
                    {
                        amount = auditBalance24.ResultFrom + auditBalance24.ResultTo;
                    totalExpenseReceivables += amount;

                    }
                }
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.ExpenseReceivable,
                    AccountName = "مصروفات مستحقة التحصيل",
                    Amount = Math.Round(totalExpenseReceivables, 2),
                    Num = 17
                });
                //ايرادات محصلة مقدما
                //=========================
                var accIdIncomesPaidAdvance = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.IncomesPaidAdvance).FirstOrDefault().Id;
                double totalIncomesPaidAdvance = 0;
                var auditBalance25 = GetAuditBalances(dtFrom, dtTo, accIdIncomesPaidAdvance);
                if (auditBalance25 != null)
                    totalIncomesPaidAdvance = auditBalance25.ResultFrom + auditBalance25.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.IncomesPaidAdvance,
                    AccountName = "ايرادات محصلة مقدما",
                    Amount = Math.Round(totalIncomesPaidAdvance, 2),
                });
                //أرباح مبيعات تقسيط مؤجلة ( تخص أعوام لاحقة )
                //=========================
                var accIdDeferredInstallmentSalesProfit = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.DeferredInstallmentSalesProfit).FirstOrDefault().Id;
                double totalDeferredInstallmentSalesProfit = 0;
                var auditBalance26 = GetAuditBalances(dtFrom, dtTo, accIdDeferredInstallmentSalesProfit);
                if (auditBalance26 != null)
                    totalDeferredInstallmentSalesProfit = auditBalance26.ResultFrom + auditBalance26.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.DeferredInstallmentSalesProfit,
                    AccountName = "أرباح مبيعات تقسيط مؤجلة ( تخص أعوام لاحقة )",
                    Amount = Math.Round(totalDeferredInstallmentSalesProfit, 2),
                });
                //حسابات دائنة أخرى
                //=========================
                var accIdOtherAccountsPayable = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.OtherAccountsPayable).FirstOrDefault().Id;
                double totalOtherAccountsPayable = 0;
                var auditBalance27 = GetAuditBalances(dtFrom, dtTo, accIdOtherAccountsPayable);
                if (auditBalance27 != null)
                    totalOtherAccountsPayable = auditBalance27.ResultFrom + auditBalance27.ResultTo ;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.OtherAccountsPayable,
                    AccountName = "حسابات دائنة أخرى",
                    Amount = Math.Round(totalOtherAccountsPayable, 2),
                    Num = 18
                });
                //حسابات دائنة لدى الشركات القابضة / التابعة / الشقيقة
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = 283,
                    AccountName = "حسابات دائنة لدى الشركات القابضة / التابعة / الشقيقة",
                    Num = 9
                });
                //حسابات دائنة لدى الشركات القابضة
                //=========================
                var accIdAccountsPayableHolding = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsPayableHolding).FirstOrDefault().Id;
                double totalAccountsPayableHolding = 0;
                var auditBalance28 = GetAuditBalances(dtFrom, dtTo, accIdAccountsPayableHolding);
                if (auditBalance28 != null)
                    totalAccountsPayableHolding = auditBalance28.ResultFrom + auditBalance28.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsPayableHolding,
                    AccountName = "حسابات دائنة لدى الشركات القابضة",
                    Amount = Math.Round(totalAccountsPayableHolding, 2),
                    Num = 9
                });
                //حسابات دائنة لدى الشركات التابعة/الشقيقة
                //=========================
                var accIdAccountsPayableSubsidiaries = db.AccountsTrees.Where(x => x.AccountNumber == Lookups.AccountsPayableSubsidiaries).FirstOrDefault().Id;
                double totalAccountsPayableSubsidiaries = 0;
                var auditBalance29 = GetAuditBalances(dtFrom, dtTo, accIdAccountsPayableSubsidiaries);
                if (auditBalance29 != null)
                    totalAccountsPayableSubsidiaries = auditBalance29.ResultFrom + auditBalance29.ResultTo;
                financialCenter.Add(new IncomeListDto
                {
                    AccountNumber = Lookups.AccountsPayableSubsidiaries,
                    AccountName = "حسابات دائنة لدى الشركات التابعة/الشقيقة",
                    Amount = Math.Round(totalAccountsPayableSubsidiaries, 2),
                    Num = 9
                });
                //إجمالى موردون وأوراق دفع ودائنون أخرون
                //=================================
                var totalSupplierandOthers = totalSuppliers + totalAccountsPayables + totalExpenseReceivables + totalIncomesPaidAdvance + totalDeferredInstallmentSalesProfit + totalOtherAccountsPayable + totalAccountsPayableHolding + totalAccountsPayableSubsidiaries;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "إجمالى موردون وأوراق دفع ودائنون أخرون",
                    Amount = Math.Round(totalSupplierandOthers, 2),
                    IsTotal = true,
                });
                //مجموع الالتزمات المتداولة 
                //=================================
                var TotalCurrentLiabilities = totalSupplierandOthers + totalAllowances;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع الالتزمات المتداولة",
                    Amount = Math.Round(TotalCurrentLiabilities, 2),
                    IsTotal = true,
                });
                //مجموع حقوق الملكية 
                //=================================
                var TotalAllPropertyRights = totalPropertyRights + TotalNonCurrentLiabilities + TotalCurrentLiabilities;
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "مجموع حقوق الملكية",
                    Amount = Math.Round(TotalAllPropertyRights, 2),
                    IsTotal = true,
                });
                return financialCenter;
            }
        }
        #endregion
    }


}