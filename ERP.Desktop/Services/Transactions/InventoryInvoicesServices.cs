using ERP.Desktop.DTOs;
using ERP.DAL;
using ERP.Desktop.Services.Reports.ItemReports;
using ERP.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ERP.Web.Utilites.Lookups;
using static ERP.Desktop.Utilities.CommonMethods;

namespace ERP.Desktop.Services.Transactions
{
    public class InventoryInvoicesServices
    {
        public VTSaleEntities db;

        public InventoryInvoicesServices(VTSaleEntities db)
        {
            this.db = db;
        }

        public List<InventoryInvoiceVM> GetAll()
        {
            return db.InventoryInvoices.Where(x => !x.IsDeleted).Select(x => new InventoryInvoiceVM
            {
                Id = x.Id,
                InvoiceGuid = x.Id,
                InvoiceDate = x.InvoiceDate.ToString(),
                BranchName = x.Branch.Name,
                StoreName = x.Store.Name,
                TotalDifferenceAmount = x.TotalDifferenceAmount,
                InvoiceNumber = x.InvoiceNumber
            }).ToList();
        }

        public List<ItemBalanceDto> SearchItemBalances(string itemCode, string barCode, Guid? groupId, Guid? itemtypeId, Guid? itemId, Guid? branchId, Guid? storeId)
        {
            return new BalanceService(db).SearchItemBalanceInventory(itemCode, barCode, groupId, itemtypeId, itemId, branchId, storeId);
        }

        public DtoResultObj<InventoryInvoice> AddNew(DateTime InvoiceDate, Guid? BranchId, Guid? StoreId, int? ItemCostCalculateId, string Notes, List<ItemBalanceDto> itemBalanceDtos)
        {
            int itemCost;
            if (ItemCostCalculateId == null)
            {
                //احتساب تكلفة المنتج 
                var generals = db.GeneralSettings.ToList();
                var itemCot = generals.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
                if (!int.TryParse(itemCot, out itemCost))
                    return new DtoResultObj<InventoryInvoice>() { IsSuccessed = false, Message = "تأكد من اختيار طريقة الاحتساب" };
            }
            else
                itemCost = ItemCostCalculateId ?? 1;

            //الاصناف
            List<InventoryInvoiceDetail> items = new List<InventoryInvoiceDetail>();
            if (itemBalanceDtos.Count() == 0)
                return new DtoResultObj<InventoryInvoice>() { IsSuccessed = false, Message = "تأكد من وجود صنف واحد على الاقل" };
            else
            {
                var balanceServices = new BalanceService(db);
                items = itemBalanceDtos.Select(x =>
                  new InventoryInvoiceDetail
                  {
                      ItemId = x.Id,
                      Balance = x.Balance,
                      BalanceReal = x.BalanceReal,
                      DifferenceCount = x.BalanceReal - x.Balance,
                      DifferenceAmount = (x.BalanceReal - x.Balance) * balanceServices.GetItemCostCalculation(itemCost, x.Id)
                  }).ToList();
            }

            InventoryInvoice model = new InventoryInvoice();
            model.InvoiceDate = InvoiceDate;

            //اضافة رقم الفاتورة
            string codePrefix = Properties.Settings.Default.CodePrefix;
            model.InvoiceNumber = codePrefix + (db.InventoryInvoices.Count(x => x.InvoiceNumber.StartsWith(codePrefix)) + 1);

            model.BranchId = BranchId;
            model.StoreId = StoreId;
            model.TotalDifferenceAmount = items.Sum(x => x.DifferenceAmount);
            model.ItemCostCalculateId = itemCost;
            model.Notes = Notes;
            model.InventoryInvoiceDetails = items;

            db.InventoryInvoices.Add(model);

            if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                return new DtoResultObj<InventoryInvoice>() { IsSuccessed = true, Message = "تم الاضافة بنجاح", Object = model };
            else
                return new DtoResultObj<InventoryInvoice>() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };

        }

        public DtoResultObj<InventoryInvoice> GetInventoryInvoice(Guid invoGuid)
        {
            var vm = db.InventoryInvoices.Where(x => x.Id == invoGuid && x.InventoryInvoiceDetails.Any(y => !y.IsDeleted)).FirstOrDefault();
            if (vm != null)
            {
                return new DtoResultObj<InventoryInvoice>() { IsSuccessed = true, Object = vm };
            }
            else
                return new DtoResultObj<InventoryInvoice>() { IsSuccessed = false, Message = "لم يتم ايجاد الفاتورة" };
        }

        public DtoResult Delete(Guid Id)
        {

            var model = db.InventoryInvoices.Where(x => x.Id == Id).FirstOrDefault();
            if (model != null)
            {
                model.IsDeleted = true;
                db.Entry(model).State = EntityState.Modified;

                var details = db.InventoryInvoiceDetails.Where(x => !x.IsDeleted && x.InventoryInvoiceId == model.Id).ToList();
                //details.ForEach(x => x.IsDeleted = true);
                foreach (var detail in details)
                {
                    detail.IsDeleted = true;
                    db.Entry(detail).State = EntityState.Modified;
                }

                if (db.SaveChanges(UserServices.UserInfo.UserId) > 0)
                    return new DtoResult() { IsSuccessed = true, Message = "تم الحذف بنجاح" };
                else
                    return new DtoResult() { IsSuccessed = false, Message = "حدث خطأ اثناء تنفيذ العملية" };
            }
            else
                return new DtoResult() { IsSuccessed = false, Message = "لم يتم ايجاد الفاتورة" };


        }
    }


}
