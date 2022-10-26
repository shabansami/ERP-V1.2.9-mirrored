using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Utilites
{
    public static class InsertGeneralSettings<T> where T : class
    {

        //add account tree and any entity generic
        public static bool? AddAccountTree(GeneralSettingCl generalSettingId, string accountName, AccountTreeSelectorTypesCl selectorTypeId, T model, Guid userId)
        {
            //get account id from general setting 
            using (var db = new VTSaleEntities())
            {
                long newAccountNum = 0;// new account number 
                var val = db.GeneralSettings.Find((int)generalSettingId).SValue;
                if (val == null)
                    return null;

                var accountTree = db.AccountsTrees.Find(Guid.Parse(val));
                var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                    newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                else
                    newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                // add new account tree 
                var newAccountTree = new AccountsTree
                {
                    AccountLevel = accountTree.AccountLevel + 1,
                    AccountName = accountName,
                    AccountNumber = newAccountNum,
                    ParentId = accountTree.Id,
                    TypeId = (int)selectorTypeId,
                    SelectedTree = false
                };
                db.AccountsTrees.Add(newAccountTree);

                //add model 
                var prop = db.Entry(model).Entity.GetType().GetProperty("AccountsTree");
                prop.SetValue(db.Entry(model).Entity, newAccountTree);
                db.Set<T>().Add(model);

                if (db.SaveChanges(userId) > 0)
                    return true;
                else
                    return false;
            }
        }

        // way 2 manual
        //return account tree object 
        public static AccountsTree ReturnAccountTree(GeneralSettingCl generalSettingId, string accountName, AccountTreeSelectorTypesCl selectorTypeId)
        {

            //get account id from general setting 
            using (var db = new VTSaleEntities())
            {
                long newAccountNum = 0;// new account number 
                var val = db.GeneralSettings.Find((int)generalSettingId).SValue;
                if (val == null)
                    return null;
                var accountTree = db.AccountsTrees.Find(Guid.Parse(val));
                var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                    newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                else
                    newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                // add new account tree 
                return new AccountsTree
                {
                    AccountLevel = accountTree.AccountLevel + 1,
                    AccountName = accountName,
                    AccountNumber = newAccountNum,
                    ParentId = accountTree.Id,
                    TypeId = (int)selectorTypeId,
                    SelectedTree = false
                };

            }
        }

        //add account tree only
        public static bool? AddAccountTree(GeneralSettingCl generalSettingId, string accountName, AccountTreeSelectorTypesCl selectorTypeId, Guid userId)
        {

            //get account id from general setting 
            using (var db = new VTSaleEntities())
            {
                long newAccountNum = 0;// new account number 
                var val = db.GeneralSettings.Find((int)generalSettingId).SValue;
                if (val == null)
                    return null;
                var accountTree = db.AccountsTrees.Find(Guid.Parse(val));
                var count = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count();
                if (accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).Count() == 0)
                    newAccountNum = long.Parse(accountTree.AccountNumber + "000001");
                else
                    newAccountNum = accountTree.AccountsTreesChildren.Where(x => !x.IsDeleted).OrderByDescending(x => x.AccountNumber).FirstOrDefault().AccountNumber + 1;

                // add new account tree 
                db.AccountsTrees.Add(new AccountsTree
                {
                    AccountLevel = accountTree.AccountLevel + 1,
                    AccountName = accountName,
                    AccountNumber = newAccountNum,
                    ParentId = accountTree.Id,
                    TypeId = (int)selectorTypeId,
                    SelectedTree=false
                });
                if (db.SaveChanges(userId) > 0)
                    return true;
                else
                    return false;

            }
        }

    }
}