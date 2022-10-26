using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Desktop.Services.Definations
{
    internal class POSServices
    {
        VTSaleEntities db;

        public POSServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public List<POSVM> GetAllPointOfSales()
        {
            return db.PointOfSales.Where(x => !x.IsDeleted).Select(x => new POSVM()
            {
                Id = x.Id,
                Name = x.Name,
                AccountID = x.BankAccountID,
                AccountName = x.BankAccount.AccountName,
                BranchID = x.BrunchId ?? Guid.Empty,
                BranchName = x.Branch.Name,
                StoreID = x.StoreID,
                StoreName = x.Store.Name,
                SafeID = x.SafeID,
                SafeName = x.Safe.Name
            }).ToList();
        }

        public DtoResult AddNewPOS(PointOfSale pos)
        {
            try
            {
                db.PointOfSales.Add(pos);
                db.SaveChanges(UserServices.UserInfo.UserId);
                return new DtoResult() { IsSuccessed = true };
            }
            catch (Exception ex)
            {
                return new DtoResult() { IsSuccessed = false, Message = ex.Message };
            }

        }
        public DtoResult Delete(Guid id)
        {
            var pos = db.PointOfSales.Find(id);
            if (pos != null)
            {
                pos.IsDeleted = true;
                db.SaveChanges(UserServices.UserInfo.UserId);
                return new DtoResult() { IsSuccessed = true };
            }
            return new DtoResult() { IsSuccessed = false, Message = "خطأ في رقم نقطة البيع" };
        }

    }
}
