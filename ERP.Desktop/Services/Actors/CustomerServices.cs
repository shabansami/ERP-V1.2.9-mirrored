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
    class CustomerServices
    {
        VTSaleEntities db;

        public CustomerServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public DtoResultObj<Person> AddNew(Person customer)
        {
            var result = new DtoResultObj<Person>();
            if (customer.PersonTypeId is null)
            {
                result.Message = "تأكد من ادخال بيانات صحيحة";
                return result;
            }
            var checkResult = CheckNotDuplicatedMobil(customer);
            if (!checkResult.IsSuccessed)
            {
                result.Message = checkResult.Message;
                return result;
            }

            //فى حالة ان الشخص مورد وعميل يتم اضافة فى حساب المورد
            if (customer.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
            {
                //add as customer in account tree
                var accountTreeCust = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, customer.Name, AccountTreeSelectorTypesCl.Operational);
                db.AccountsTrees.Add(accountTreeCust);
                //add as supplier in account tree
                var accountTreeSupp = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, customer.Name, AccountTreeSelectorTypesCl.Operational);
                db.AccountsTrees.Add(accountTreeSupp);

                //add person in personTable
                customer.AccountsTreeCustomer = accountTreeCust;
                customer.AccountsTreeSupplier = accountTreeSupp;
                db.Persons.Add(customer);
                db.SaveChanges(UserServices.UserInfo.UserId);
            }
            else
            {
                //add as customer in account tree
                var accountTreeCust = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeCustomerAccount, customer.Name,  AccountTreeSelectorTypesCl.Operational);
                db.AccountsTrees.Add(accountTreeCust);
                //add person in personTable
                customer.AccountsTreeCustomer = accountTreeCust;
                db.Persons.Add(customer);
                db.SaveChanges(UserServices.UserInfo.UserId);
            }
            result.IsSuccessed = true;
            result.Object = customer;
            return result;
        }

        public DtoResultObj<Person> Update(Person vm)
        {
            Person model;
            try
            {
                model = db.Persons.Find(vm.Id);
                model.Name = vm.Name;
                model.IsActive = true;
                model.AreaId = vm.AreaId;
                model.Address = vm.Address;
                model.Mob1 = vm.Mob1;
                model.Mob2 = vm.Mob2;
                model.IsActive = true;
                model.GenderId = vm.GenderId;
                AccountsTree accountTreeCust;
                AccountsTree accountTreeSupp;

                //فى حالة ان الشخص مورد وعميل يتم تحديث فى حساب المورد
                if (model.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                {
                    //update supplier name in account tree 
                    accountTreeSupp = db.AccountsTrees.Find(model.AccountTreeSupplierId);
                    if (accountTreeSupp != null)
                    {
                        if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                        {
                            accountTreeSupp.AccountName = vm.Name;
                            db.Entry(accountTreeSupp).State = EntityState.Modified;
                        }
                        else
                        {
                            accountTreeSupp.AccountName = vm.Name;
                            accountTreeSupp.IsDeleted = true;
                            accountTreeSupp.DeletedBy = UserServices.UserInfo.UserId;
                            accountTreeSupp.DeletedOn = TimeNow;
                            db.Entry(accountTreeSupp).State = EntityState.Modified; 
                            model.AccountTreeSupplierId = null;
                        }
                    }
                    else if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                    {
                        accountTreeSupp = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.Operational);
                        db.AccountsTrees.Add(accountTreeSupp);
                        model.AccountsTreeSupplier = accountTreeSupp;
                    }
                }
                else if (vm.PersonTypeId == (int)PersonTypeCl.SupplierAndCustomer)
                {
                    accountTreeSupp = db.AccountsTrees.Find(model.AccountTreeSupplierId);
                    if (accountTreeSupp != null)
                    {
                        accountTreeSupp.AccountName = vm.Name;
                        accountTreeSupp.IsDeleted = false;
                        accountTreeSupp.ModifiedBy = UserServices.UserInfo.UserId;
                        accountTreeSupp.ModifiedOn = TimeNow;
                        db.Entry(accountTreeSupp).State = EntityState.Modified;
                    }
                    else
                    {
                        accountTreeSupp = AccountServices<Person>.GetAccountTree(GeneralSettingCl.AccountTreeSupplierAccount, vm.Name, AccountTreeSelectorTypesCl.Operational);
                        db.AccountsTrees.Add(accountTreeSupp);
                        model.AccountsTreeSupplier = accountTreeSupp;
                    }
                }

                //update customer name in account tree 

                model.PersonTypeId = vm.PersonTypeId;
                db.Entry(model).State = EntityState.Modified;

                accountTreeCust = db.AccountsTrees.Find(model.AccountsTreeCustomerId);
                accountTreeCust.AccountName = vm.Name;
                db.Entry(accountTreeCust).State = EntityState.Modified;
                db.SaveChanges(UserServices.UserInfo.UserId);
            }
            catch (Exception ex)
            {
                return new DtoResultObj<Person> { IsSuccessed = false, Message = ex.Message };
            }
            return new DtoResultObj<Person>() { IsSuccessed = true, Object = model };
        }

        public DtoResult CheckNotDuplicatedMobil(Person customer)
        {
            var result = new DtoResult() { IsSuccessed = true };
            if (db.Persons.Where(x => !x.IsDeleted && x.Mob1 == customer.Mob1 && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer)).Count() > 0)
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
                var customer = db.Persons.FirstOrDefault(x => !x.IsDeleted && x.Id == id && (x.PersonTypeId == (int)Lookups.PersonTypeCl.Customer || x.PersonTypeId == (int)Lookups.PersonTypeCl.SupplierAndCustomer));
                if (customer is null)
                {
                    return new DtoResult() { IsSuccessed = false, Message = "لم يتم ايجاد العميل" };
                }
                var isExistGenerarDays = db.GeneralDailies.Where(x => !x.IsDeleted && (x.AccountsTreeId == customer.AccountsTreeCustomerId || x.AccountsTreeId == customer.AccountTreeSupplierId || x.AccountsTreeId == customer.AccountTreeEmpCustodyId)).Any();
                if (isExistGenerarDays)
                    return new DtoResult() { IsSuccessed = false, Message = "لا يمكن الحذف لارتباط الحساب بمعاملات مسجله مسبقا" };

                customer.IsDeleted = true;
                if (customer.AccountsTreeCustomerId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == customer.AccountsTreeCustomerId);
                    if (acc != null)
                    {
                        acc.IsDeleted = true;
                        acc.DeletedOn = TimeNow;
                        acc.DeletedBy = UserServices.UserInfo.UserId;
                    }
                }
                if (customer.AccountTreeEmpCustodyId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == customer.AccountTreeEmpCustodyId);
                    if (acc != null)
                    {
                        acc.IsDeleted = true;
                        acc.DeletedOn = TimeNow;
                        acc.DeletedBy = UserServices.UserInfo.UserId;
                    }
                }
                if (customer.AccountTreeSupplierId != null)
                {
                    var acc = db.AccountsTrees.FirstOrDefault(x => x.Id == customer.AccountTreeSupplierId);
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
