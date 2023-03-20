using Newtonsoft.Json;
using ERP.DAL;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;
using System.Web.Mvc;
using ERP.Web.DataTablesDS;

namespace ERP.Web.Services
{
    public static class AccountTreeService
    {
        public static bool selected { get; set; } = true;
        public static bool showAll { get; set; } = false;

        #region Account Trees
        // way used GetAccountTree
        public static List<DrawTree> GetAccountTree(bool selectedTree = false,bool showAllLevel=false,int? spcLevel=null,string accountsexption = null)
        {

            //spcLevel اظهار حسابات مستوى معين مثلا المصروفات فقط
            //accountsexption  استثناء الحسابات الموجودة من الظهور بالشجرة
            selected = selectedTree;
            showAll = showAllLevel;
            List<DrawTree> accountTrees = new List<DrawTree>();
            using (var db = new VTSaleEntities())
            {
                //-================== 4 ============================
                List<AccountsTree> parentss = null;
                if (showAllLevel)
                parentss = db.AccountsTrees.Where(x => !x.IsDeleted).ToList();
                else
                parentss = db.AccountsTrees.Where(x => !x.IsDeleted && x.SelectedTree).ToList();
                if (spcLevel!=null)
                {
                    _accountsTrees = new List<AccountsTree>();
                    var account = db.AccountsTrees.Where(x => !x.IsDeleted && x.AccountNumber == spcLevel).FirstOrDefault();
                    CallRecursive(account, accountsexption);
                    
                    accountTrees = ChildrenOf(_accountsTrees, account.Id);
                }
                else
                    accountTrees = ChildrenOf(parentss, null);

                //var parents2s = db.AccountsTrees.Where(x => !x.IsDeleted).Select(x=>new {id=x.Id,name=x.AccountName,select=x.SelectedTree }).ToList();
            }
            return accountTrees;
        }
        public static List<AccountsTree> _accountsTrees = new List<AccountsTree>();

        public static List<DrawTree> ChildrenOf(List<AccountsTree> accountsTrees, Guid? parentId)
        {
            var accountChildrens = accountsTrees.Where(i => i.ParentId == parentId && !i.IsDeleted);
            if (selected)
            {
                return accountChildrens.Select(i => new DrawTree
                {
                    id = i.Id,
                    title =i.AccountNumber+" || "+ i.AccountName,
                    ParentId = i.ParentId,
                    SelectedTree = i.SelectedTree
                    ,
                    subs = ChildrenOf(accountsTrees, i.Id),
                   
                })
                .ToList();
            }
            else if(showAll)
            {
                return accountChildrens.Select(i => new DrawTree
                {
                    id = i.Id,
                    title = i.AccountNumber + " || " + i.AccountName,
                    ParentId = i.ParentId,
                    SelectedTree = true,
                    subs = ChildrenOf(accountsTrees, i.Id),
                    ShowAllLevel = !i.SelectedTree
                }).ToList();
            }
           else {
                return accountChildrens.Select(i => new DrawTree
                {
                    id = i.Id,
                    title = i.AccountNumber + " || " + i.AccountName,
                    ParentId = i.ParentId,
                    SelectedTree = true,
                    subs = ChildrenOf(accountsTrees, i.Id)
                   
                })
                .ToList();
            }

        }

        // way 2 not used GetAccountTree
        public static List<DrawTree> GetChild(Guid? parentId)
        {
            using (var db = new VTSaleEntities())
            {
                return db.AccountsTrees.Where(x => !x.IsDeleted && x.ParentId == parentId).Select(x => new DrawTree
                {
                    id = x.Id,
                    title = x.AccountName,
                    ParentId = x.ParentId
                                ,
                    subs = GetChild(x.ParentId)
                }).ToList();
            }

        }
        static List<DrawTree> GetNods(AccountsTree accountsTree)
        {
            if (accountsTree != null && accountsTree.AccountsTreesChildren.Count(x => !x.IsDeleted) > 0)
                return accountsTree.AccountsTreesChildren.Where(x => !x.IsDeleted).AsEnumerable().Select(x => new DrawTree() { id = x.Id, title = x.AccountName, subs = GetNods(x) }).ToList();
            else
                return new List<DrawTree>();
        }

        //
        public static bool CheckFinancailYear(DateTime dt)
        {
            using (var db = new VTSaleEntities())
            {
                var financial = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                var start = DateTime.Parse(financial.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue);
                var end = DateTime.Parse(financial.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue);
                if (dt >= start && dt <= end)
                    return true;
                else
                    return false;
            }
        }

        public static bool CheckAccountTreeIdHasChilds(Guid? accountTreeId)
        {
            //فى ان الحساب رئيسيى وتم ادخاله من الاعدادات isAccountSetting=true
            //فى حالة التأكد من ان الحساب ليس له ابناء isAccountSetting=false
            using (var db = new VTSaleEntities())
            {
                var accountTree = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == accountTreeId).FirstOrDefault();
                return accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Any();
            }
        }     
        //حسابات الاصول الثابتة فقط من شجرة الحسابات 
        public static List<AccountsTree> GetFixedAssets()
        {
            using (var db = new VTSaleEntities())
            {
                var generalSettingfixedAsset = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeFixedAssets).FirstOrDefault();
                Guid fixedAssetParentValue;
                if (generalSettingfixedAsset != null && Guid.TryParse(generalSettingfixedAsset.SValue, out fixedAssetParentValue))
                {
                    var accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == fixedAssetParentValue).FirstOrDefault();
                    return accountTrees.AccountsTreesChildren.Where(x => !x.IsDeleted).ToList();
                }
                else
                    return new List<AccountsTree>();

            }
        }
        private static List<AccountsTree> accountsTrees1(ICollection<AccountsTree> accountsTrees, Guid? parentId)
        {
            AccountsTree accountsTree = null;
            using (var db = new VTSaleEntities())
            {
                var ss = accountsTrees.Select(x => new AccountsTree
                {
                    Id = x.Id,
                    AccountName = x.AccountName,
                    ParentId=x.ParentId,
                    AccountsTreesChildren = accountsTrees1(x.AccountsTreesChildren, x.ParentId).ToList()
                }).ToList();

                if(ss.Count()>0)
                _accountsTrees.AddRange(ss);
            }

            return _accountsTrees;
        }
        private static void PrintRecursive(AccountsTree treeNode, List<int> list=null)
        {
            //// Print the node.  
            //System.Diagnostics.Debug.WriteLine(treeNode.Text);
            //MessageBox.Show(treeNode.Text);

            // Visit each node recursively.  
            foreach (AccountsTree tn in treeNode.AccountsTreesChildren.Where(x => !x.IsDeleted))
            {
                if (list!=null)
                {
                    if (!list.Any(x => x == tn.AccountNumber))
                    {
                        _accountsTrees.Add(tn);
                        PrintRecursive(tn, list);
                    }
                }else
                {
                    _accountsTrees.Add(tn);
                    PrintRecursive(tn, list);
                }
                

            }
        }

        // Call the procedure using the TreeView.  
        private static void CallRecursive(AccountsTree treeView,string accountsexption)
        {
            // Print each node recursively.  
            foreach (AccountsTree n in treeView.AccountsTreesChildren.Where(x => !x.IsDeleted))
            {
                //recursiveTotalNodes++;
                List<int> list = new List<int>();
                if(accountsexption != null)
                   list= JsonConvert.DeserializeObject<List<int>>(accountsexption);
                if(!list.Any(x=>x==n.AccountNumber))
                {
                    _accountsTrees.Add(n);
                    PrintRecursive(n,list);
                }

            }
        }
        //حسابات الفرعية بدون اى ابناء من شجرة الحسابات  مثلا الحصول على كل حسابات المصروفات -الايرادات
        public static List<AccountsTree> GetAccountTreeLastChild(Guid? spcLevel, string accountsexption=null)
        {
            List<AccountsTree> accountTrees = new List<AccountsTree>();
            using (var db = new VTSaleEntities())
            {
                //-================== 4 ============================
                if (spcLevel != null)
                {
                    _accountsTrees = new List<AccountsTree>();
                    CallRecursive(db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == spcLevel).FirstOrDefault(), accountsexption);
                    foreach (var item in _accountsTrees)
                    {
                        if (item.AccountsTreesChildren.Where(x=>!x.IsDeleted).Count()==0)
                        {
                            accountTrees.Add(item);
                        }
                    }
                }
            }
            return accountTrees;
        }


        #endregion

        #region Tree View Account Tree 
        public class AccountTreeDto
        {
            public Guid Id { get; set; }
            public long AccountNumber { get; set; }
            public string AccountName { get; set; }
            public Guid? ParentId { get; set; }
            public List<GeneralDaily> GeneralDailies { get; set; }

        }
        public static List<TreeViewDraw> GetAccountTreeView(DateTime dtFrom,DateTime dtTo)
        {
            List<TreeViewDraw> accountTrees = new List<TreeViewDraw>();
            using (var db = new VTSaleEntities())
            {
                //8 milliseconds after maping AccountTree to DTO class
                var parentss = db.AccountsTrees.Where(x => !x.IsDeleted)
                    .Select(x => new AccountTreeDto
                    {
                        Id = x.Id,
                        AccountName = x.AccountName,
                        AccountNumber = x.AccountNumber,
                        ParentId = x.ParentId,
                        GeneralDailies = x.GeneralDailies.Where(y => !y.IsDeleted&&y.TransactionDate>=dtFrom&&y.TransactionDate<=dtTo).ToList()
                    })
                    .ToList();
                accountTrees = ChildrenOf2(parentss, null);
                //var yyw = Total(accountTrees[0]);
                var yy = ReturnWithTotal(accountTrees);
                return yy;
            }
        }
        public static double Total(TreeViewDraw node)
        {
            double price = node.total;
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    price += Total(child);
                }
            }

            return price;
        }
        public static List<TreeViewDraw> ReturnWithTotal(List<TreeViewDraw> treeViewDraws)
        {

            return treeViewDraws.Select(i => new TreeViewDraw
            {
                //total =Total(i),
                //icon = "fa fa-folder text-default",
                text = i.text + "||" + Total(i),
                children = ReturnWithTotal(i.children)
            })
                .ToList();

        }
        public static List<TreeViewDraw> ChildrenOf2(List<AccountTreeDto> accountsTrees, Guid? parentId)
        {
            var accountChildrens = accountsTrees.Where(i => i.ParentId == parentId);

            return accountChildrens.Select(i => new TreeViewDraw
            {

                // 30 seconds
                total = i.GeneralDailies.Where(g => !g.IsDeleted).Select(x => x.Debit - x.Credit).DefaultIfEmpty(0).Sum(),
                // 14 seconds
                //total = i.GeneralDailies.Where(g => !g.IsDeleted).Sum(x => x.Debit - x.Credit),

                // 19 seconds
                //total = GetGeneralDay(i.Id),
                text =i.AccountNumber+"-"+ i.AccountName,
                icon = "",
                children = ChildrenOf2(accountsTrees, i.Id)
            })
            .ToList();
        }
        //============================old
        public static List<TreeViewDraw> GetAccountTreeView2()
        {
            List<TreeViewDraw> accountTrees = new List<TreeViewDraw>();
            using (var db = new VTSaleEntities())
            {
                var parentss = db.AccountsTrees.Where(x => !x.IsDeleted).ToList();
                accountTrees = ChildrenOf22(parentss, null);
                return ReturnTotal2(null, accountTrees);
            }
        }


        public static List<TreeViewDraw> ReturnTotal2(TreeViewDraw treeViewDraw, List<TreeViewDraw> treeViewDraws)
        {

            return treeViewDraws.Select(i => new TreeViewDraw
            {
                total = i.children.SelectMany(x => x.children).Sum(x => x.total),
                //icon = i.children.Select(x => x.total).DefaultIfEmpty(0).Sum().ToString()+" | many="+ i.children.SelectMany(x => x.children).Sum(x => x.total).ToString(),
                text = i.text,
                children = ReturnTotal2(i, i.children)
            })
                .ToList();

        }

        public static List<TreeViewDraw> TreeSum(List<TreeViewDraw> treeViewDraws)
        {
            foreach (var item in treeViewDraws)
            {
                if (item.children.Count > 0)
                {
                    item.total += item.children.SelectMany(x => x.children).Sum(x => x.total);
                    item.children = TreeSum(item.children);
                }
            }

            return treeViewDraws;
        }
        public static List<TreeViewDraw> ChildrenOf22(List<AccountsTree> accountsTrees, Guid? parentId)
        {
            var accountChildrens = accountsTrees.Where(i => i.ParentId == parentId && !i.IsDeleted);

            return accountChildrens.Select(i => new TreeViewDraw
            {
                total = i.GeneralDailies.Where(g => !g.IsDeleted).Select(x => x.Debit + x.Credit).DefaultIfEmpty(0).Sum(),
                text = i.AccountName + "  ||  " + i.GeneralDailies.Where(g => !g.IsDeleted).Select(x => x.Debit + x.Credit).DefaultIfEmpty(0).Sum(),
                //icon = i.GeneralDailies.Where(g => !g.IsDeleted).Select(x => x.Debit + x.Credit).DefaultIfEmpty(0).Sum().ToString(),
                children = ChildrenOf22(accountsTrees, i.Id)
            })
            .ToList();
        }
        #endregion

        #region  VoucherPayments سندات الصرف والقبض
        // Voucher Payment سندات صرف  مورد
        // Voucher Receipt سندات دفع عميل
        public static List<DropDownList> GetVouchers(VTSaleEntities db,bool IsVoucherPayment)
        {
            List<DropDownList> list = new List<DropDownList>();

            if (IsVoucherPayment)//سندات صرف
            {
                ///شيكات تحت التحصيل اوراق دفع
                Guid checkUnderCollectionPayments;
                AccountsTree accountsTreePayment = new AccountsTree();
                var cheqUnderPayments = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue;
                if (Guid.TryParse(cheqUnderPayments, out checkUnderCollectionPayments))
                    accountsTreePayment = db.AccountsTrees.Where(x => x.Id == checkUnderCollectionPayments).FirstOrDefault();

                if (accountsTreePayment != null)
                    list.Add(new DropDownList
                    {
                        Name = accountsTreePayment.AccountNumber + "     " + accountsTreePayment.AccountName,
                        Id = accountsTreePayment.Id
                    });

                Guid earnedDiscount;//حساب مكتسب
                AccountsTree accountsTreeEarnedDiscount = new AccountsTree();
                var earnedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue;
                if (Guid.TryParse(earnedDiscountSetting, out earnedDiscount))
                    accountsTreeEarnedDiscount = db.AccountsTrees.Where(x => x.Id == earnedDiscount).FirstOrDefault();
                if (accountsTreeEarnedDiscount != null)
                    list.Add(new DropDownList
                    {
                        Name = accountsTreeEarnedDiscount.AccountNumber + "     " + accountsTreeEarnedDiscount.AccountName,
                        Id = accountsTreeEarnedDiscount.Id
                    });
            }
            else
            {
                Guid checkUnderCollection;//شيكات تحت التحصيل اوراق قبض/دفع
                AccountsTree accountsTreeReceipt = new AccountsTree();
                var cheqUnder = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue;
                if (Guid.TryParse(cheqUnder, out checkUnderCollection))
                    accountsTreeReceipt = db.AccountsTrees.Where(x => x.Id == checkUnderCollection).FirstOrDefault();
                if (accountsTreeReceipt != null)
                    list.Add(new DropDownList
                    {
                        Name = accountsTreeReceipt.AccountNumber + "     " + accountsTreeReceipt.AccountName,
                        Id = accountsTreeReceipt.Id
                    });

                Guid premittedDiscount;//حساب خصم مسموح به
                AccountsTree accountsTreePremittedDiscount = new AccountsTree();
                var premittedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue;
                if (Guid.TryParse(premittedDiscountSetting, out premittedDiscount))
                    accountsTreePremittedDiscount = db.AccountsTrees.Where(x => x.Id == premittedDiscount).FirstOrDefault();
                if (accountsTreePremittedDiscount != null)
                    list.Add(new DropDownList
                    {
                        Name = accountsTreePremittedDiscount.AccountNumber + "     " + accountsTreePremittedDiscount.AccountName,
                        Id = accountsTreePremittedDiscount.Id
                    });
            }

            Guid safeAccount;//حساب الخزينة
            AccountsTree accountTreeSafeAccount = new AccountsTree();
            var safeAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue;
            if (Guid.TryParse(safeAccountSetting, out safeAccount))
            {
                var safes = GetAccountChilds(safeAccount);
                foreach (var item in safes)
                {
                    list.Add(new DropDownList
                    {
                        Name = item.AccountNumber + "     " + item.AccountName,
                        Id = item.Id
                    });
                }
            }
            Guid bankAccount;//حساب البنوك
            AccountsTree accountTreeBankAccount = new AccountsTree();
            var bankAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue;
            if (Guid.TryParse(bankAccountSetting, out bankAccount))
            {
                var banks = GetAccountChilds(bankAccount);
                foreach (var item in banks)
                {
                    list.Add(new DropDownList
                    {
                        Name = item.AccountNumber + "     " + item.AccountName,
                        Id = item.Id
                    });
                }
            }
               
            return list;
        }

        public static List<DropDownList> GetBankVouchers(VTSaleEntities db, bool IsVoucherPayment)
        {
            List<DropDownList> list = new List<DropDownList>();

            //if (IsVoucherPayment)//سندات صرف
            //{
            //    ///شيكات تحت التحصيل اوراق دفع
            //    Guid checkUnderCollectionPayments;
            //    AccountsTree accountsTreePayment = new AccountsTree();
            //    var cheqUnderPayments = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue;
            //    if (Guid.TryParse(cheqUnderPayments, out checkUnderCollectionPayments))
            //        accountsTreePayment = db.AccountsTrees.Where(x => x.Id == checkUnderCollectionPayments).FirstOrDefault();

            //    if (accountsTreePayment != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreePayment.AccountNumber + "     " + accountsTreePayment.AccountName,
            //            Id = accountsTreePayment.Id
            //        });

            //    Guid earnedDiscount;//حساب مكتسب
            //    AccountsTree accountsTreeEarnedDiscount = new AccountsTree();
            //    var earnedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue;
            //    if (Guid.TryParse(earnedDiscountSetting, out earnedDiscount))
            //        accountsTreeEarnedDiscount = db.AccountsTrees.Where(x => x.Id == earnedDiscount).FirstOrDefault();
            //    if (accountsTreeEarnedDiscount != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreeEarnedDiscount.AccountNumber + "     " + accountsTreeEarnedDiscount.AccountName,
            //            Id = accountsTreeEarnedDiscount.Id
            //        });
            //}
            //else
            //{
            //    Guid checkUnderCollection;//شيكات تحت التحصيل اوراق قبض/دفع
            //    AccountsTree accountsTreeReceipt = new AccountsTree();
            //    var cheqUnder = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue;
            //    if (Guid.TryParse(cheqUnder, out checkUnderCollection))
            //        accountsTreeReceipt = db.AccountsTrees.Where(x => x.Id == checkUnderCollection).FirstOrDefault();
            //    if (accountsTreeReceipt != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreeReceipt.AccountNumber + "     " + accountsTreeReceipt.AccountName,
            //            Id = accountsTreeReceipt.Id
            //        });

            //    Guid premittedDiscount;//حساب خصم مسموح به
            //    AccountsTree accountsTreePremittedDiscount = new AccountsTree();
            //    var premittedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue;
            //    if (Guid.TryParse(premittedDiscountSetting, out premittedDiscount))
            //        accountsTreePremittedDiscount = db.AccountsTrees.Where(x => x.Id == premittedDiscount).FirstOrDefault();
            //    if (accountsTreePremittedDiscount != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreePremittedDiscount.AccountNumber + "     " + accountsTreePremittedDiscount.AccountName,
            //            Id = accountsTreePremittedDiscount.Id
            //        });
            //}

            //Guid safeAccount;//حساب الخزينة
            //AccountsTree accountTreeSafeAccount = new AccountsTree();
            //var safeAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue;
            //if (Guid.TryParse(safeAccountSetting, out safeAccount))
            //{
            //    var safes = GetAccountChilds(safeAccount);
            //    foreach (var item in safes)
            //    {
            //        list.Add(new DropDownList
            //        {
            //            Name = item.AccountNumber + "     " + item.AccountName,
            //            Id = item.Id
            //        });
            //    }
            //}
            Guid bankAccount;//حساب البنوك
            AccountsTree accountTreeBankAccount = new AccountsTree();
            var bankAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue;
            if (Guid.TryParse(bankAccountSetting, out bankAccount))
            {
                var banks = GetAccountChilds(bankAccount);
                foreach (var item in banks)
                {
                    list.Add(new DropDownList
                    {
                        Name = item.AccountNumber + "     " + item.AccountName,
                        Id = item.Id
                    });
                }
            }

            return list;
        }
        public static List<DropDownList> GetSafeVouchers(VTSaleEntities db, bool IsVoucherPayment)
        {
            List<DropDownList> list = new List<DropDownList>();

            //if (IsVoucherPayment)//سندات صرف
            //{
            //    ///شيكات تحت التحصيل اوراق دفع
            //    Guid checkUnderCollectionPayments;
            //    AccountsTree accountsTreePayment = new AccountsTree();
            //    var cheqUnderPayments = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue;
            //    if (Guid.TryParse(cheqUnderPayments, out checkUnderCollectionPayments))
            //        accountsTreePayment = db.AccountsTrees.Where(x => x.Id == checkUnderCollectionPayments).FirstOrDefault();

            //    if (accountsTreePayment != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreePayment.AccountNumber + "     " + accountsTreePayment.AccountName,
            //            Id = accountsTreePayment.Id
            //        });

            //    Guid earnedDiscount;//حساب مكتسب
            //    AccountsTree accountsTreeEarnedDiscount = new AccountsTree();
            //    var earnedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue;
            //    if (Guid.TryParse(earnedDiscountSetting, out earnedDiscount))
            //        accountsTreeEarnedDiscount = db.AccountsTrees.Where(x => x.Id == earnedDiscount).FirstOrDefault();
            //    if (accountsTreeEarnedDiscount != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreeEarnedDiscount.AccountNumber + "     " + accountsTreeEarnedDiscount.AccountName,
            //            Id = accountsTreeEarnedDiscount.Id
            //        });
            //}
            //else
            //{
            //    Guid checkUnderCollection;//شيكات تحت التحصيل اوراق قبض/دفع
            //    AccountsTree accountsTreeReceipt = new AccountsTree();
            //    var cheqUnder = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue;
            //    if (Guid.TryParse(cheqUnder, out checkUnderCollection))
            //        accountsTreeReceipt = db.AccountsTrees.Where(x => x.Id == checkUnderCollection).FirstOrDefault();
            //    if (accountsTreeReceipt != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreeReceipt.AccountNumber + "     " + accountsTreeReceipt.AccountName,
            //            Id = accountsTreeReceipt.Id
            //        });

            //    Guid premittedDiscount;//حساب خصم مسموح به
            //    AccountsTree accountsTreePremittedDiscount = new AccountsTree();
            //    var premittedDiscountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue;
            //    if (Guid.TryParse(premittedDiscountSetting, out premittedDiscount))
            //        accountsTreePremittedDiscount = db.AccountsTrees.Where(x => x.Id == premittedDiscount).FirstOrDefault();
            //    if (accountsTreePremittedDiscount != null)
            //        list.Add(new DropDownList
            //        {
            //            Name = accountsTreePremittedDiscount.AccountNumber + "     " + accountsTreePremittedDiscount.AccountName,
            //            Id = accountsTreePremittedDiscount.Id
            //        });
            //}

            Guid safeAccount;//حساب الخزينة
            AccountsTree accountTreeSafeAccount = new AccountsTree();
            var safeAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue;
            if (Guid.TryParse(safeAccountSetting, out safeAccount))
            {
                var safes = GetAccountChilds(safeAccount);
                foreach (var item in safes)
                {
                    list.Add(new DropDownList
                    {
                        Name = item.AccountNumber + "     " + item.AccountName,
                        Id = item.Id
                    });
                }
            }
            //Guid bankAccount;//حساب البنوك
            //AccountsTree accountTreeBankAccount = new AccountsTree();
            //var bankAccountSetting = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue;
            //if (Guid.TryParse(bankAccountSetting, out bankAccount))
            //{
            //    var banks = GetAccountChilds(bankAccount);
            //    foreach (var item in banks)
            //    {
            //        list.Add(new DropDownList
            //        {
            //            Name = item.AccountNumber + "     " + item.AccountName,
            //            Id = item.Id
            //        });
            //    }
            //}

            return list;
        }

        public static List<AccountsTree> GetAccountChilds(Guid accountParent)
        {
            using (var db = new VTSaleEntities())
            {
                //var generalSettingfixedAsset = db.GeneralSettings.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralFixedAssets).FirstOrDefault();
                //int fixedAssetParentValue;
                //if (generalSettingfixedAsset != null && int.TryParse(generalSettingfixedAsset.SValue, out fixedAssetParentValue))
                //{
                var accountTrees = db.AccountsTrees.Where(x => !x.IsDeleted && x.Id == accountParent).FirstOrDefault();
                return accountTrees.AccountsTreesChildren.Where(x => !x.IsDeleted).ToList();
                //}
                //else
                //    return new List<AccountsTree>();

            }
        }
        #endregion

        #region التأكد من عدم ادخال او تعديل الحسابات الرئيسية للدليل المحاسبى 
        //الاصول/الخصوم/المصروفات/الايرادات
        public static bool IsAccountMain(long AccountNumber)
        {
            if(AccountNumber==Lookups.GeneralAssets|| AccountNumber == Lookups.GeneralPropertyRights || AccountNumber == Lookups.GeneralExpenses || AccountNumber == Lookups.GeneralIncomes)
                return true;
            else 
                return false;
        }
        #endregion
    }
}