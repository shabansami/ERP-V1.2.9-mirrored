using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Desktop.Services.Actors
{
    class AccountServices<T> where T : class
    {

        //return account tree object 
        public static AccountsTree GetAccountTree(GeneralSettingCl generalSettingId, string accountName, AccountTreeSelectorTypesCl selectorTypeId)
        {
            //get account id from general setting 
            var db = DBContext.UnitDbContext;
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

    }
}
