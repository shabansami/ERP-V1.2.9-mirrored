using ERP.DAL;
using ERP.DAL.Dtos;
using ERP.DAL.Models;
using ERP.Web.DataTablesDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Services
{
    public class StoreService
    {
        public Store GetDefaultStore(VTSaleEntities db)
        {
            var generalSetting=db.GeneralSettings.Where(x=>x.Id==(int)GeneralSettingCl.StoreDefaultlId).FirstOrDefault();
            if (generalSetting!=null)
            {
                if (Guid.TryParse(generalSetting.SValue,out Guid defaultStoreId))
                {
                    var store = db.Stores.Where(x => x.Id == defaultStoreId && !x.IsDamages).FirstOrDefault();
                    if (store != null)
                        return store;
                    else
                        return null;
                }

                else
                    return null;
            }else 
                return null;
        }

        public static bool InvoicesHasStore(Guid storeId,VTSaleEntities db)
        {
            var sell=db.SellInvoicesDetails.Where(x=>!x.IsDeleted&&x.StoreId==storeId).Any();
            var sellBack=db.SellBackInvoicesDetails.Where(x=>!x.IsDeleted&&x.StoreId==storeId).Any();
            var purchase=db.PurchaseInvoicesDetails.Where(x=>!x.IsDeleted&&x.StoreId==storeId).Any();
            var purchaseBack=db.PurchaseBackInvoicesDetails.Where(x=>!x.IsDeleted&&x.StoreId==storeId).Any();
            var itemInit=db.ItemIntialBalanceDetails.Where(x => !x.IsDeleted && x.StoreId == storeId).Any();
            if (sell || sellBack || purchase || purchaseBack || itemInit)
                return true;
            return false;
        }
        public static List<DropDownList> GetStoreSaleMenByBranchId(Guid? employeeId)
        {
            if(employeeId == null||employeeId==Guid.Empty) return new List<DropDownList>(); 
            IQueryable<EmployeeStore> employeeStores = null;
            using (var db = new VTSaleEntities())
            {
                employeeStores = db.EmployeeStores.Where(x => !x.IsDeleted&&x.EmployeeId== employeeId);
                if (employeeStores.Any())
                    return employeeStores.Select(x => new DropDownList { Id = x.StoreId, Name = x.Store.Name }).ToList();
                else
                    return new List<DropDownList>();
            }

        }
    }
}