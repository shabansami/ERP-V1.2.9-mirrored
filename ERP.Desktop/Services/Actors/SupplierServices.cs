using Newtonsoft.Json;
using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Utilities;
using ERP.Web.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;
using static ERP.Desktop.Utilities.CommonMethods;
using System.Data.Entity;

namespace ERP.Desktop.Services.Actors
{
    class SupplierServices
    {
        VTSaleEntities db;

        public SupplierServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public DtoResultObj<Person> AddNew(Person vm)
        {
            var result = new DtoResultObj<Person>();
            if (vm.PersonTypeId is null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }
            var checkResult = CheckNotDuplicatedMobil(vm);
            if (!checkResult.IsSuccessed)
            {
                result.Message = checkResult.Message;
                return result;
            }
            //فى حالة ان الشخص مورد وعميل يتم اضافة فى حساب العميل
            if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
            {
                //add as customer in account tree
                var accountTreeCust = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                db.AccountsTrees.Add(accountTreeCust);
                //add as supplier in account tree
                var accountTreeSupp = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                db.AccountsTrees.Add(accountTreeSupp);

                //add person in personTable
                vm.AccountsTreeCustomer = accountTreeCust;
                vm.AccountsTreeSupplier = accountTreeSupp;
            }
            else
            {
                //add as supplier in account tree
                var accountTreeSupp = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.Supplier);
                db.AccountsTrees.Add(accountTreeSupp);
                //add person in personTable
                vm.AccountsTreeSupplier = accountTreeSupp;
            }
            db.Persons.Add(vm);
            db.SaveChanges(UserServices.UserInfo.UserId);
            result.IsSuccessed = true;
            result.Object = vm;
            return result;
        }

        public DtoResultObj<Person> Update(Person vm)
        {
            Person model;
            try
            {
                model = db.Persons.Find(vm.Id);
                model.Name = vm.Name;
                model.AreaId = vm.AreaId;
                model.Address = vm.Address;
                model.Mob1 = vm.Mob1;
                model.Mob2 = vm.Mob2;
                model.IsActive = true;
                model.GenderId = vm.GenderId;
                db.Entry(model).State = EntityState.Modified;

                AccountsTree accountTreeCust;
                AccountsTree accountTreeSupp;
                //فى حالة ان الشخص مورد وعميل يتم تحديث فى حساب العميل
                if (model.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                {
                    //update customer name in account tree 
                    accountTreeCust = db.AccountsTrees.Find(model.AccountsTreeCustomerId);
                    if (accountTreeCust != null)
                    {
                        if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                        {
                            accountTreeCust.AccountName = vm.Name;
                            db.Entry(accountTreeCust).State = EntityState.Modified;
                        }
                        else
                        {
                            accountTreeCust.AccountName = vm.Name;
                            accountTreeCust.IsDeleted = true;
                            accountTreeCust.DeletedBy = UserServices.UserInfo.UserId;
                            accountTreeCust.DeletedOn = TimeNow;
                            db.Entry(accountTreeCust).State = EntityState.Modified;
                        }
                    }
                    else if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        //add as customer in account tree
                        accountTreeCust = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                        db.AccountsTrees.Add(accountTreeCust);
                        model.AccountsTreeCustomer = accountTreeCust;
                    }
                }
                else if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                {
                    accountTreeCust = db.AccountsTrees.Find(model.AccountsTreeCustomerId);
                    if (accountTreeCust != null)
                    {
                        accountTreeCust.AccountName = vm.Name;
                        accountTreeCust.IsDeleted = false;
                        accountTreeCust.ModifiedBy = UserServices.UserInfo.UserId;
                        accountTreeCust.ModifiedOn = TimeNow;
                        db.Entry(accountTreeCust).State = EntityState.Modified;
                    }
                    else
                    {
                        //add as customer in account tree
                        accountTreeCust = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, vm.Name, AccountTreeSelectorTypesCl.SupplierAndCustomer);
                        db.AccountsTrees.Add(accountTreeCust);
                        model.AccountsTreeCustomer = accountTreeCust;
                    }
                }

                //update supplier name in account tree 
                accountTreeSupp = db.AccountsTrees.Find(model.AccountTreeSupplierId);
                accountTreeSupp.AccountName = vm.Name;
                model.PersonTypeId = vm.PersonTypeId;
                db.Entry(accountTreeSupp).State = EntityState.Modified;
                db.SaveChanges(UserServices.UserInfo.UserId);
            }
            catch (Exception ex)
            {
                return new DtoResultObj<Person> { IsSuccessed = false, Message = ex.Message };
            }
            return new DtoResultObj<Person>() { IsSuccessed = true, Object = model };
        }

        public DtoResult CheckNotDuplicatedMobil(Person supplier)
        {
            var result = new DtoResult() { IsSuccessed = true };
            if (db.Persons.Where(x => !x.IsDeleted && x.Mob1 == supplier.Mob1 && x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier).Count() > 0)
            {
                result.IsSuccessed = false;
                result.Message = "رقم الهاتف موجود مسبقا";
            }
            return result;
        }

        public DtoResult Delete(Guid id)
        {
            var db = DBContext.UnitDbContext;
            {
                var supplier = db.Persons.FirstOrDefault(x => !x.IsDeleted && x.Id == id && x.PersonTypeId == (int)Lookups.PersonTypeCl.Supplier);
                if (supplier is null)
                {
                    return new DtoResult() { IsSuccessed = false, Message = "لم يتم ايجاد المورد" };
                }
                var isExistGenerarDays = db.GeneralDailies.Where(x => !x.IsDeleted && (x.AccountsTreeId == supplier.AccountsTreeCustomerId || x.AccountsTreeId == supplier.AccountTreeSupplierId || x.AccountsTreeId == supplier.AccountTreeEmpCustodyId)).Any();
                if (isExistGenerarDays)
                    return new DtoResult() { IsSuccessed = false, Message = "لا يمكن الحذف لارتباط الحساب بمعاملات مسجله مسبقا" };

                supplier.IsDeleted = true;
                supplier.DeletedOn = TimeNow;
                supplier.DeletedBy = UserServices.UserInfo.UserId;
                if (supplier.AccountsTreeCustomerId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == supplier.AccountsTreeCustomerId);
                    if (acc != null)
                    {
                        acc.IsDeleted = true;
                        acc.DeletedOn = TimeNow;
                        acc.DeletedBy = UserServices.UserInfo.UserId;
                    }
                }
                if (supplier.AccountTreeEmpCustodyId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == supplier.AccountTreeEmpCustodyId);
                    if (acc != null)
                    {
                        acc.IsDeleted = true;
                        acc.DeletedOn = TimeNow;
                        acc.DeletedBy = UserServices.UserInfo.UserId;
                    }
                }
                if (supplier.AccountTreeSupplierId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == supplier.AccountTreeSupplierId);
                    if (acc != null)
                    {
                        acc.IsDeleted = true;
                        acc.DeletedOn = TimeNow;
                        acc.DeletedBy = UserServices.UserInfo.UserId;
                    }
                }
                db.SaveChanges(UserServices.UserInfo.UserId);
                return new DtoResult { IsSuccessed = true };
            }
        }
    }
}
