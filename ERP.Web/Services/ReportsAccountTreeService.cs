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
        public static List<IncomeListDto> IncomeLists(DateTime? dtFrom, DateTime? dtTo,int? assetsDepreciationStatus)
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
                if(saleValueSafy!=0)
                incomeLists.Add(new IncomeListDto
                {
                    AccountName = "صافــي قيمة المبيعات",
                    Whole = Math.Round(saleValueSafy, 2),
                    IsTotal = true
                });
                //اضافة حساب المبيعات الى جدول قائمة الدخل 
                if (saleDetails.Partial != 0)
                    incomeLists.Add(saleDetails);
                //اضافة حساب المرتجع المبيعات الى جدول قائمة الدخل 
                if (saleReturnDetails.Partial != 0)
                    incomeLists.Add(saleReturnDetails);
                //اضافة حساب خصم مسموح به الى جدول قائمة الدخل 
                if (premittedDiscountDetails.Partial != 0)
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
                if (purchaseSafy != 0)
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
                if (salesValue != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "تكلفة المبيعات",
                    Whole = Math.Round(salesValue, 2),
                    IsTotal=true
                });
                if (salesStockValue != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "تكلفة بضاعه متاحة للبيع",
                    Partial = Math.Round(salesStockValue, 2),
                });
                if (stockLastValue != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مخزون آخر المدة",
                    Partial = Math.Round(stockLastValue, 2),
                });
                if (AmountStocks != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مخزون أول المدة",
                    Partial = Math.Round(AmountStocks, 2),
                });
                //اضافة مخزون اول المدة الى جدول قائمة الدخل 
                if (stockADetails.Partial != 0)
                    incomeLists.Add(stockADetails);
                //اضافة حساب المشتريات الى جدول قائمة الدخل 
                if (purchaseDetails.Partial != 0)
                    incomeLists.Add(purchaseDetails);
                //اضافة حساب مرددوات المشتريات الى جدول قائمة الدخل 
                if (purchaseReturnDetails .Partial!= 0)
                    incomeLists.Add(purchaseReturnDetails);
                //اضافة حساب خصم مكتسب الى جدول قائمة الدخل 
                if (earnedDiscountDetails.Partial != 0)
                    incomeLists.Add(earnedDiscountDetails);



                //مجمل الربح
                //=======================================================
                var totalprofit = saleValueSafy - salesValue;
                if (totalprofit != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "مجمل الربح",
                    Whole = Math.Round(totalprofit, 2),
                    IsTotal = true
                });


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
                ///
                                //مصروف الاهلاك
                //======================================================
                //حساب مجمع الاهلاك من الاعدادات
                var accountTreeAssetsDepreciationComplex = Guid.Parse(generalSetting.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue);
                //اجمالى الارصدة من ميزان المراجعة 
                var AssetsDepreciationComple = GetAuditBalances(dtFrom, dtTo, accountTreeAssetsDepreciationComplex);
                var AssetsDepreciationComplex = AssetsDepreciationComple.ResultFrom + AssetsDepreciationComple.ResultTo;
                //var AssetsDepreciationComplex = GetAuditBalances(dtFrom, dtTo, accountTreeAssetsDepreciationComplex).ResultTo;//قديما


                var expenses = ReportsAccountTreeService.AuditBalances(dtFrom, dtTo, null, null, null, null, Lookups.GeneralExpenses);
                var expense = expenses.Where(x => x.AccountNumber == Lookups.GeneralExpenses).ToList();
                var totalExpenses = expense.Select(x => x.ResultFrom + x.ResultTo).Sum();
                //اضافة الحساب الى جدول قائمة الدخل 
                if (totalExpenses != 0)
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
                        if (assetsDepreciationStatus == 2&&e.Id== accountTreeAssetsDepreciationComplex)
                        {
                            //مصروف الاهلاك منفصل عن المصروفات
                 
                        }else
                        {
                            if (Math.Round(e.ResultFrom - e.ResultTo, 2) != 0)
                                incomeLists.Add(new IncomeListDto
                            {
                                AccountName = e.AccountName,
                                AccountNumber=e.AccountNumber,
                                Partial = Math.Round(e.ResultFrom - e.ResultTo, 2),
                                IsTotal = false
                            });
                        }
                       
                        foreach (var e3 in e.children)
                        {
                            if (e3.AccountLevel == 3)
                            {
                                if (assetsDepreciationStatus == 2 && e.Id == accountTreeAssetsDepreciationComplex)
                                {
                                    //مصروف الاهلاك منفصل عن المصروفات

                                }
                                else
                                {
                                    if (Math.Round(e3.ResultFrom - e3.ResultTo, 2) != 0)
                                        incomeLists.Add(new IncomeListDto
                                    {
                                        AccountName = e3.AccountName,
                                        AccountNumber = e3.AccountNumber,
                                        Partial = Math.Round(e3.ResultFrom - e3.ResultTo, 2),
                                        IsTotal = false,
                                        IsThirdLevel = true
                                    });
                                }
                                
                            }
                        }
                    }
                }
                if(assetsDepreciationStatus==2)
                {
                    //اضافة الحساب الى جدول قائمة الدخل 
                    if (AssetsDepreciationComplex != 0)
                        incomeLists.Add(new IncomeListDto
                    {
                        AccountName = "مصروف الاهلاك",
                        Partial = Math.Round(AssetsDepreciationComplex, 2),
                    });
                }
               

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
                if (AmountTotalIncome != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "اجمالى الايرادات",
                    Whole = Math.Round(AmountTotalIncome, 2),
                    IsTotal = true
                });

                //صافى الربح
                //=======================================================
                var totalprofitSafy = totalprofit - totalExpenses + AmountTotalIncome;
                if (totalprofitSafy != 0)
                    incomeLists.Add(new IncomeListDto
                {
                    AccountName = "صافى الربح",
                    Whole = Math.Round(totalprofitSafy, 2),
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
                //الاصول
                var assets = ReportsAccountTreeService.AuditBalances(dtFrom, dtTo, null, null, null, null, Lookups.GeneralAssets);
                var assts = assets.Where(x => x.AccountNumber == Lookups.GeneralAssets).ToList();
                var totalAssets = assts.Select(x => x.ResultFrom + x.ResultTo).Sum();
                //اضافة الحساب الى جدول قائمة المركز المالى 
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "اجمالى الاصول",
                    Whole = Math.Round(totalAssets, 2),
                    IsTotal = true
                });
                //عرض الحساباب الرئيسية حتى المستوى الثالث
                foreach (var e in assts.FirstOrDefault().children)
                {
                    if (e.AccountLevel == 2)
                    {
                        if(Math.Round(e.ResultFrom + e.ResultTo, 2)!=0)
                            financialCenter.Add(new IncomeListDto
                            {
                                AccountName = e.AccountName,
                                AccountNumber = e.AccountNumber,
                                Partial = Math.Round(e.ResultFrom + e.ResultTo, 2),
                                IsTotal = false
                            });
                        

                        foreach (var e3 in e.children)
                        {
                            if (e3.AccountLevel == 3)
                            {

                                if (Math.Round(e3.ResultFrom + e3.ResultTo, 2) != 0)
                                    financialCenter.Add(new IncomeListDto
                                    {
                                        AccountName = e3.AccountName,
                                        AccountNumber = e3.AccountNumber,
                                        Partial = Math.Round(e3.ResultFrom + e3.ResultTo, 2),
                                        IsTotal = false,
                                        IsThirdLevel = true
                                    });
                                

                            }
                        }
                    }
                }

                //الخصوم
                var propertyRights = ReportsAccountTreeService.AuditBalances(dtFrom, dtTo, null, null, null, null, Lookups.GeneralPropertyRights);
                var propertyRight = propertyRights.Where(x => x.AccountNumber == Lookups.GeneralPropertyRights).ToList();
                var totalPropertyRights = propertyRight.Select(x => x.ResultFrom + x.ResultTo).Sum();
                //اضافة الحساب الى جدول قائمة المركز المالى 
                financialCenter.Add(new IncomeListDto
                {
                    AccountName = "اجمالى حقوق الملكية",
                    Whole = Math.Round(totalPropertyRights, 2),
                    IsTotal = true
                });
                //عرض الحساباب الرئيسية حتى المستوى الثالث
                foreach (var e in propertyRight.FirstOrDefault().children)
                {
                    if (e.AccountLevel == 2)
                    {
                        if(Math.Round(e.ResultFrom + e.ResultTo, 2) != 0)
                        financialCenter.Add(new IncomeListDto
                        {
                            AccountName = e.AccountName,
                            AccountNumber = e.AccountNumber,
                            Partial = Math.Round(e.ResultFrom + e.ResultTo, 2),
                            IsTotal = false
                        });


                        foreach (var e3 in e.children)
                        {
                            if (e3.AccountLevel == 3)
                            {
                                if (Math.Round(e3.ResultFrom + e3.ResultTo, 2) != 0)
                                financialCenter.Add(new IncomeListDto
                                {
                                    AccountName = e3.AccountName,
                                    AccountNumber = e3.AccountNumber,
                                    Partial = Math.Round(e3.ResultFrom + e3.ResultTo, 2),
                                    IsTotal = false,
                                    IsThirdLevel = true
                                });


                            }
                        }
                    }
                }
                return financialCenter;
            }
        }
        #endregion
    }


}