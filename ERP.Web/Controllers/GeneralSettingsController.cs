using ERP.Web.Identity;
using ERP.DAL;
using ERP.Web.Utilites;
using ERP.Web.ViewModels;
using System;using ERP.DAL.Utilites;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP.Web.Utilites.Lookups;

namespace ERP.Web.Controllers
{
    [Authorization]

    public class GeneralSettingsController : Controller
    {
        // GET: GeneralSettings
        VTSaleEntities db = new VTSaleEntities();
        VTSAuth auth = new VTSAuth();
        public ActionResult Index()
        {
            GeneralSettingVM vm = new GeneralSettingVM();

            var model = db.GeneralSettings.ToList();
            var generalSettingArea = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataArea).FirstOrDefault();
            Guid id;
            if (Guid.TryParse(generalSettingArea.SValue, out id))
            {
                var areaId = Guid.Parse(generalSettingArea.SValue);
                var area = db.Areas.Where(x => x.Id == areaId).FirstOrDefault();
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name", area.City.Country.Id);
                ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name", area.City.Id);
                ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name", area.Id);
            }
            else
            {
                ViewBag.CountryId = new SelectList(db.Countries.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.CityId = new SelectList(db.Cities.Where(x => !x.IsDeleted), "Id", "Name");
                ViewBag.AreaId = new SelectList(db.Areas.Where(x => !x.IsDeleted), "Id", "Name");

            }

            vm.AreaId = id;
            vm.EntityData.Name = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataEntityDataName).FirstOrDefault().SValue;
            vm.EntityData.AbbreviationName = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataAbbreviationName).FirstOrDefault().SValue;
            vm.EntityData.Address = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataAddress).FirstOrDefault().SValue;
            vm.EntityData.CommercialRegisterNo = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo).FirstOrDefault().SValue;
            if (model.Where(x => x.Id == (int)GeneralSettingCl.BarCodeDataLetter).FirstOrDefault().SValue != null)
                vm.Letters = bool.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.BarCodeDataLetter).FirstOrDefault().SValue);
            if (model.Where(x => x.Id == (int)GeneralSettingCl.BarCodeDataNumber).FirstOrDefault().SValue != null)
                vm.Numbers = bool.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.BarCodeDataNumber).FirstOrDefault().SValue);
            vm.EntityData.TaxCardNo = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataTaxCardNo).FirstOrDefault().SValue;
            vm.EntityData.Tel1 = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataTel1).FirstOrDefault().SValue;
            vm.EntityData.Tel2 = model.Where(x => x.Id == (int)GeneralSettingCl.EntityDataTel2).FirstOrDefault().SValue;

            //ادارة المخازن
            //تحديد مخزن التصنيع الداخلى الافتراضى
            var productionInternalStore = model.Where(x => x.SType == (int)GeneralSettingTypeCl.StoreDefault && x.Id == (int)GeneralSettingCl.StoreProductionInternalId).FirstOrDefault().SValue;
            Guid? branchIn = null;
            if (!string.IsNullOrEmpty(productionInternalStore))
            {
                Guid stIn = Guid.Parse(productionInternalStore);
                branchIn = db.Stores.Where(x => x.Id == stIn).FirstOrDefault().BranchId;
            }
            ViewBag.BranchInId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", branchIn);
            ViewBag.ProductionInStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchIn && branchIn.HasValue && !x.IsDamages), "Id", "Name", productionInternalStore);

            //تحديد المخزن الافتراضى فى فواتير البيع والشراء الافتراضى للجهة
            var defaultStore = model.Where(x => x.SType == (int)GeneralSettingTypeCl.StoreDefault && x.Id == (int)GeneralSettingCl.StoreDefaultlId).FirstOrDefault().SValue;
            Guid? branchEx = null;
            if (!string.IsNullOrEmpty(defaultStore))
            {
                Guid stEx = Guid.Parse(defaultStore);
                branchEx = db.Stores.Where(x => x.Id == stEx).FirstOrDefault().BranchId;
            }
            ViewBag.BranchExId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", branchEx);
            ViewBag.DefaultStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchEx && branchEx.HasValue && !x.IsDamages), "Id", "Name", defaultStore);

            //تحديد مخزن تحت التصنيع الافتراضى
            var productionUnderStore = model.Where(x => x.SType == (int)GeneralSettingTypeCl.StoreDefault && x.Id == (int)GeneralSettingCl.StoreUnderProductionId).FirstOrDefault().SValue;
            Guid? branchUn = null;
            if (!string.IsNullOrEmpty(productionUnderStore))
            {
                Guid stUn = Guid.Parse(productionUnderStore);
                branchUn = db.Stores.Where(x => x.Id == stUn).FirstOrDefault().BranchId;
            }
            ViewBag.BranchUnderId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", branchUn);
            ViewBag.ProductionUnderStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchUn && branchUn.HasValue && !x.IsDamages), "Id", "Name", productionUnderStore);

            //تحديد مخزن الصيانة
            var maintenanceStore = model.Where(x => x.SType == (int)GeneralSettingTypeCl.StoreDefault && x.Id == (int)GeneralSettingCl.StoreMaintenance).FirstOrDefault().SValue;
            Guid? branchMaint = null;
            if (!string.IsNullOrEmpty(maintenanceStore))
            {
                Guid stUn = Guid.Parse(maintenanceStore);
                branchMaint = db.Stores.Where(x => x.Id == stUn).FirstOrDefault().BranchId;
            }
            ViewBag.BranchMaintenanceId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", branchMaint);
            ViewBag.MaintenanceStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchMaint && branchMaint.HasValue && !x.IsDamages), "Id", "Name", maintenanceStore);

            //تحديد مخزن توالف الصيانة
            var maintenanceDamageStore = model.Where(x => x.SType == (int)GeneralSettingTypeCl.StoreDefault && x.Id == (int)GeneralSettingCl.StoreMaintenanceDamage).FirstOrDefault().SValue;
            Guid? branchMaintDamage = null;
            if (!string.IsNullOrEmpty(maintenanceDamageStore))
            {
                Guid stUn = Guid.Parse(maintenanceDamageStore);
                branchMaintDamage = db.Stores.Where(x => x.Id == stUn).FirstOrDefault().BranchId;
            }
            ViewBag.BranchMaintenanceDamageId = new SelectList(db.Branches.Where(x => !x.IsDeleted), "Id", "Name", branchMaintDamage);
            ViewBag.MaintenanceDamageStoreId = new SelectList(db.Stores.Where(x => !x.IsDeleted && x.BranchId == branchMaintDamage && branchMaintDamage.HasValue && x.IsDamages), "Id", "Name", maintenanceDamageStore);


            #region Account Tree
            var accountTree = db.AccountsTrees.Where(x => !x.IsDeleted).ToList();

            //==================== Added Tax Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAddedTaxAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.AddedTaxAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAddedTaxAccount).FirstOrDefault().SValue);
                var accountTreeAddedTaxAccount = accountTree.Where(x => x.Id == vm.AccountTree.AddedTaxAccountSettingValue).FirstOrDefault();
                vm.AccountTree.AddedTaxAccountName = accountTreeAddedTaxAccount.AccountName + "..." + accountTreeAddedTaxAccount.AccountNumber;
            }
            vm.AccountTree.AddedTaxAccountSettingId = (int)GeneralSettingCl.AccountTreeAddedTaxAccount;

            //==================== Bank Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.BankAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeBankAccount).FirstOrDefault().SValue);
                var accountTreeBankAccount = accountTree.Where(x => x.Id == vm.AccountTree.BankAccountSettingValue).FirstOrDefault();
                vm.AccountTree.BankAccountName = accountTreeBankAccount.AccountName + "..." + accountTreeBankAccount.AccountNumber;
            }
            vm.AccountTree.BankAccountSettingId = (int)GeneralSettingCl.AccountTreeBankAccount;

            //==================== Commercial Tax =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.CommercialTaxSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCommercialTax).FirstOrDefault().SValue);
                var accountTreeCommercialTax = accountTree.Where(x => x.Id == vm.AccountTree.CommercialTaxSettingValue).FirstOrDefault();
                vm.AccountTree.CommercialTaxName = accountTreeCommercialTax.AccountName + "..." + accountTreeCommercialTax.AccountNumber;
            }
            vm.AccountTree.CommercialTaxSettingId = (int)GeneralSettingCl.AccountTreeCommercialTax;

            //==================== Customer Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustomerAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.CustomerAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustomerAccount).FirstOrDefault().SValue);
                var accountTreeCustomerAccount = accountTree.Where(x => x.Id == vm.AccountTree.CustomerAccountSettingValue).FirstOrDefault();
                vm.AccountTree.CustomerAccountName = accountTreeCustomerAccount.AccountName + "..." + accountTreeCustomerAccount.AccountNumber;
            }
            vm.AccountTree.CustomerAccountSettingId = (int)GeneralSettingCl.AccountTreeCustomerAccount;

            ////==================== Delivery Account =======================
            //if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDeliveryAccount).FirstOrDefault().SValue != null)
            //{
            //    vm.AccountTree.DeliveryAccountSettingValue = int.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDeliveryAccount).FirstOrDefault().SValue);
            //    var accountTreeDeliveryAccount = accountTree.Where(x => x.Id == (int)vm.AccountTree.DeliveryAccountSettingValue).FirstOrDefault();
            //    vm.AccountTree.DeliveryAccountName = accountTreeDeliveryAccount.AccountName+"..."+ accountTreeDeliveryAccount.AccountNumber;
            //}
            //vm.AccountTree.DeliveryAccountSettingId = (int)GeneralSettingCl.AccountTreeDeliveryAccount;

            //==================== MiscellaneousRevenus Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.MiscellaneousRevenusAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus).FirstOrDefault().SValue);
                var accountTreeMiscellaneousRevenusAccount = accountTree.Where(x => x.Id == vm.AccountTree.MiscellaneousRevenusAccountSettingValue).FirstOrDefault();
                vm.AccountTree.MiscellaneousRevenusAccountName = accountTreeMiscellaneousRevenusAccount.AccountName + "..." + accountTreeMiscellaneousRevenusAccount.AccountNumber;
            }
            vm.AccountTree.MiscellaneousRevenusAccountSettingId = (int)GeneralSettingCl.AccountTreeMiscellaneousRevenus;

            //==================== Earned Discount =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.EarnedDiscountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEarnedDiscount).FirstOrDefault().SValue);
                var accountTreeEarnedDiscount = accountTree.Where(x => x.Id == vm.AccountTree.EarnedDiscountSettingValue).FirstOrDefault();
                vm.AccountTree.EarnedDiscountName = accountTreeEarnedDiscount.AccountName + "..." + accountTreeEarnedDiscount.AccountNumber;
            }
            vm.AccountTree.EarnedDiscountSettingId = (int)GeneralSettingCl.AccountTreeEarnedDiscount;

            //==================== Expenses Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeExpensesAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.ExpensesAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeExpensesAccount).FirstOrDefault().SValue);
                var accountTreeExpensesAccount = accountTree.Where(x => x.Id == vm.AccountTree.ExpensesAccountSettingValue).FirstOrDefault();
                vm.AccountTree.ExpensesAccountName = accountTreeExpensesAccount.AccountName + "..." + accountTreeExpensesAccount.AccountNumber;
            }
            vm.AccountTree.ExpensesAccountSettingId = (int)GeneralSettingCl.AccountTreeExpensesAccount;

            //==================== General Revenus =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralRevenus).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.GeneralRevenusSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeGeneralRevenus).FirstOrDefault().SValue);
                var accountTreeGeneralRevenus = accountTree.Where(x => x.Id == vm.AccountTree.GeneralRevenusSettingValue).FirstOrDefault();
                vm.AccountTree.GeneralRevenusName = accountTreeGeneralRevenus.AccountName + "..." + accountTreeGeneralRevenus.AccountNumber;
            }
            vm.AccountTree.GeneralRevenusSettingId = (int)GeneralSettingCl.AccountTreeGeneralRevenus;

            //==================== Premitted Discount Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.PremittedDiscountAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePremittedDiscountAccount).FirstOrDefault().SValue);
                var accountTreePremittedDiscountAccount = accountTree.Where(x => x.Id == vm.AccountTree.PremittedDiscountAccountSettingValue).FirstOrDefault();
                vm.AccountTree.PremittedDiscountAccountName = accountTreePremittedDiscountAccount.AccountName + "..." + accountTreePremittedDiscountAccount.AccountNumber;
            }
            vm.AccountTree.PremittedDiscountAccountSettingId = (int)GeneralSettingCl.AccountTreePremittedDiscountAccount;

            //==================== Purchase Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.PurchaseAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseAccount).FirstOrDefault().SValue);
                var accountTreePurchaseAccount = accountTree.Where(x => x.Id == vm.AccountTree.PurchaseAccountSettingValue).FirstOrDefault();
                vm.AccountTree.PurchaseAccountName = accountTreePurchaseAccount.AccountName + "..." + accountTreePurchaseAccount.AccountNumber;
            }
            vm.AccountTree.PurchaseAccountSettingId = (int)GeneralSettingCl.AccountTreePurchaseAccount;

            //==================== Purchase Return Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.PurchaseReturnAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreePurchaseReturnAccount).FirstOrDefault().SValue);
                var accountTreePurchaseReturnAccount = accountTree.Where(x => x.Id == vm.AccountTree.PurchaseReturnAccountSettingValue).FirstOrDefault();
                vm.AccountTree.PurchaseReturnAccountName = accountTreePurchaseReturnAccount.AccountName + "..." + accountTreePurchaseReturnAccount.AccountNumber;
            }
            vm.AccountTree.PurchaseReturnAccountSettingId = (int)GeneralSettingCl.AccountTreePurchaseReturnAccount;

            //==================== Safe Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SafeAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSafeAccount).FirstOrDefault().SValue);
                var accountTreeSafeAccount = accountTree.Where(x => x.Id == vm.AccountTree.SafeAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SafeAccountName = accountTreeSafeAccount.AccountName + "..." + accountTreeSafeAccount.AccountNumber;
            }
            vm.AccountTree.SafeAccountSettingId = (int)GeneralSettingCl.AccountTreeSafeAccount;

            //==================== Sales Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SalesAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesAccount).FirstOrDefault().SValue);
                var accountTreeSalesAccount = accountTree.Where(x => x.Id == vm.AccountTree.SalesAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SalesAccountName = accountTreeSalesAccount.AccountName + "..." + accountTreeSalesAccount.AccountNumber;
            }
            vm.AccountTree.SalesAccountSettingId = (int)GeneralSettingCl.AccountTreeSalesAccount;


            //==================== Sales Return Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SalesReturnAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesReturnAccount).FirstOrDefault().SValue);
                var accountTreeSalesReturnAccount = accountTree.Where(x => x.Id == vm.AccountTree.SalesReturnAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SalesReturnAccountName = accountTreeSalesReturnAccount.AccountName + "..." + accountTreeSalesReturnAccount.AccountNumber;
            }
            vm.AccountTree.SalesReturnAccountSettingId = (int)GeneralSettingCl.AccountTreeSalesReturnAccount;

            //==================== Sales Tax Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SalesTaxAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalesTaxAccount).FirstOrDefault().SValue);
                var accountTreeSalesTaxAccount = accountTree.Where(x => x.Id == vm.AccountTree.SalesTaxAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SalesTaxAccountName = accountTreeSalesTaxAccount.AccountName + "..." + accountTreeSalesTaxAccount.AccountNumber;
            }
            vm.AccountTree.SalesTaxAccountSettingId = (int)GeneralSettingCl.AccountTreeSalesTaxAccount;

            //==================== Share Capital Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.ShareCapitalAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeShareCapitalAccount).FirstOrDefault().SValue);
                var accountTreeShareCapitalAccount = accountTree.Where(x => x.Id == vm.AccountTree.ShareCapitalAccountSettingValue).FirstOrDefault();
                vm.AccountTree.ShareCapitalAccountName = accountTreeShareCapitalAccount.AccountName + "..." + accountTreeShareCapitalAccount.AccountNumber;
            }
            vm.AccountTree.ShareCapitalAccountSettingId = (int)GeneralSettingCl.AccountTreeShareCapitalAccount;

            //==================== Stock Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.StockAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeStockAccount).FirstOrDefault().SValue);
                var accountTreeStockAccount = accountTree.Where(x => x.Id == vm.AccountTree.StockAccountSettingValue).FirstOrDefault();
                vm.AccountTree.StockAccountName = accountTreeStockAccount.AccountName + "..." + accountTreeStockAccount.AccountNumber;
            }
            vm.AccountTree.StockAccountSettingId = (int)GeneralSettingCl.AccountTreeStockAccount;

            //==================== Supplier Account =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSupplierAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SupplierAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSupplierAccount).FirstOrDefault().SValue);
                var accountTreeSupplierAccount = accountTree.Where(x => x.Id == vm.AccountTree.SupplierAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SupplierAccountName = accountTreeSupplierAccount.AccountName + "..." + accountTreeSupplierAccount.AccountNumber;
            }
            vm.AccountTree.SupplierAccountSettingId = (int)GeneralSettingCl.AccountTreeSupplierAccount;
            //==================== Check under collection Account اوراق القبض=======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.CheckUnderCollAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.CheckUnderCollAccountSettingValue).FirstOrDefault();
                vm.AccountTree.CheckUnderCollAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.CheckUnderCollAccountSettingId = (int)GeneralSettingCl.AccountTreeCheckUnderCollectionReceipts;
            //==================== Check under collection Account اوراق الدفع=======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.CheckUnderCollAccountPaymentSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments).FirstOrDefault().SValue);
                var accountTreeCheckPayment = accountTree.Where(x => x.Id == vm.AccountTree.CheckUnderCollAccountPaymentSettingValue).FirstOrDefault();
                vm.AccountTree.CheckUnderCollAccountPaymentName = accountTreeCheckPayment.AccountName + "..." + accountTreeCheckPayment.AccountNumber;
            }
            vm.AccountTree.CheckUnderCollAccountPaymentSettingId = (int)GeneralSettingCl.AccountTreeCheckUnderCollectionPayments;
            //==================== Loans Employees =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeLoans).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.LoanEmpAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeLoans).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.LoanEmpAccountSettingValue).FirstOrDefault();
                vm.AccountTree.LoanEmpAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.LoanEmpAccountSettingId = (int)GeneralSettingCl.AccountTreeLoans;
            //==================== Salaries =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.SalariesAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeSalaries).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.SalariesAccountSettingValue).FirstOrDefault();
                vm.AccountTree.SalariesAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.SalariesAccountSettingId = (int)GeneralSettingCl.AccountTreeSalaries;
            //==================== Employee =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEmployeeAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.EmployeeAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeEmployeeAccount).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.EmployeeAccountSettingValue).FirstOrDefault();
                vm.AccountTree.EmployeeAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.EmployeeAccountSettingId = (int)GeneralSettingCl.AccountTreeEmployeeAccount;

            //==================== custody =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustodyAccount).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.CustodyAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeCustodyAccount).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.CustodyAccountSettingValue).FirstOrDefault();
                vm.AccountTree.CustodyAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.CustodyAccountSettingId = (int)GeneralSettingCl.AccountTreeCustodyAccount;

            //==================== Maintenance =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.MaintenanceAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeMaintenance).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.MaintenanceAccountSettingValue).FirstOrDefault();
                vm.AccountTree.MaintenanceAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.MaintenanceAccountSettingId = (int)GeneralSettingCl.AccountTreeMaintenance;
            //==================== DestructionAllowance مخصص اهلاك =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDestructionAllowance).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.DestructionAllowanceAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeDestructionAllowance).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.DestructionAllowanceAccountSettingValue).FirstOrDefault();
                vm.AccountTree.DestructionAllowanceAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.DestructionAllowanceAccountSettingId = (int)GeneralSettingCl.AccountTreeDestructionAllowance;
            //==================== AssetsDepreciationComplex مجمع اهلاك الاصول الثابتة =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.AssetsDepreciationComplexAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.AssetsDepreciationComplexAccountSettingValue).FirstOrDefault();
                vm.AccountTree.AssetsDepreciationComplexAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.AssetsDepreciationComplexAccountSettingId = (int)GeneralSettingCl.AccountTreeAssetsDepreciationComplex;

            //==================== Installments Benefits فوائد الاقساط=======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeInstallmentsBenefits).FirstOrDefault().SValue != null)
            {
                vm.AccountTree.InstallmentsBenefitsAccountSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.AccountTreeInstallmentsBenefits).FirstOrDefault().SValue);
                var accountTreeCheck = accountTree.Where(x => x.Id == vm.AccountTree.InstallmentsBenefitsAccountSettingValue).FirstOrDefault();
                vm.AccountTree.InstallmentsBenefitsAccountName = accountTreeCheck.AccountName + "..." + accountTreeCheck.AccountNumber;
            }
            vm.AccountTree.InstallmentsBenefitsAccountSettingId = (int)GeneralSettingCl.AccountTreeInstallmentsBenefits;


            #endregion

            //تاريخ بداية ونهاية السنة المالية
            vm.FinancialYearStartDate = DateTime.TryParse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue, out var dt) ? DateTime.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearStartDate).FirstOrDefault().SValue) : Utility.GetDateTime();
            vm.FinancialYearEndDate = DateTime.TryParse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue, out var dt2) ? DateTime.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.FinancialYearEndDate).FirstOrDefault().SValue) : Utility.GetDateTime();

            //احتساب تكلفة المنتج 
            var itemCost = model.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateId).FirstOrDefault().SValue;
            ViewBag.ItemCostCalculateId = new SelectList(db.ItemCostCalculations.Where(x => !x.IsDeleted), "Id", "Name", itemCost != null ? itemCost : null);
            //اضافة اصناف بدون رصيد
            var itemAcceptNoBalance = model.Where(x => x.Id == (int)GeneralSettingCl.ItemAcceptNoBalance).FirstOrDefault().SValue;
            List<SelectListItem> selectListItem = new List<SelectListItem>
            {
                 new SelectListItem { Text = "قبول", Value = "1" },
                  new SelectListItem { Text = "رفض", Value = "0" }
            };
            ViewBag.ItemAcceptNoBalanceId = new SelectList(selectListItem, "Value", "Text", itemAcceptNoBalance != null ? itemAcceptNoBalance : null);
            //قبول/رفض صرف نقدية بدون رصيد
            var paidWithBalance = model.Where(x => x.Id == (int)GeneralSettingCl.PaidWithBalance).FirstOrDefault().SValue;
            ViewBag.PaidWithBalance = new SelectList(selectListItem, "Value", "Text", paidWithBalance != null ? paidWithBalance : null);
            //الاعتماد المباشر بعد حفظ فواتير البيع والتوريد
            var approvalAfterSave = model.Where(x => x.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave).FirstOrDefault().SValue;
            ViewBag.InvoicesApprovalAfterSaveId = new SelectList(selectListItem, "Value", "Text", approvalAfterSave != null ? approvalAfterSave : null);
            //اظهار تكلفة الصنف فى شاشة البيع
            var itemCostCalculateShowInSellReg = model.Where(x => x.Id == (int)GeneralSettingCl.ItemCostCalculateShowInSellReg).FirstOrDefault().SValue;
            List<SelectListItem> selectListItem3 = new List<SelectListItem>
            {
                 new SelectListItem { Text = "اظهار", Value = "1" },
                  new SelectListItem { Text = "اخفاء", Value = "0" }
            };
            ViewBag.ItemCostCalculateShowInSellRegId = new SelectList(selectListItem3, "Value", "Text", itemCostCalculateShowInSellReg != null ? itemCostCalculateShowInSellReg : null);
            //السماح بالبيع عند تخطى الحد الائتمانى للخطر
            var limitDangerSell = model.Where(x => x.Id == (int)GeneralSettingCl.LimitDangerSell).FirstOrDefault().SValue;
            ViewBag.LimitDangerSell = new SelectList(selectListItem, "Value", "Text", limitDangerSell != null ? limitDangerSell : null);
            //المدة المسموح بها فى سداد قسط
            vm.PeriodAllowedPayInstallment = int.TryParse(model.Where(x => x.Id == (int)GeneralSettingCl.PeriodAllowedPayInstallment).FirstOrDefault().SValue, out var period) ? int.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.PeriodAllowedPayInstallment).FirstOrDefault().SValue) : 5;
            #region Upload Center File
            var uploadCenterTree = db.UploadCenters.Where(x => !x.IsDeleted).ToList();

            //==================== Purchase Invoice =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseInvoice).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterPurchaseInvoiceSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseInvoice).FirstOrDefault().SValue);
                var uploadCenterPurchaseInvoice = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterPurchaseInvoiceSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterPurchaseInvoiceName = uploadCenterPurchaseInvoice.Name;
            }
            vm.UploadFileTree.UploadCenterPurchaseInvoiceSettingId = (int)GeneralSettingCl.UploadCenterPurchaseInvoice;
            //==================== Purchase back Invoice =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseBackInvoice).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterPurchaseBackInvoiceSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterPurchaseBackInvoice).FirstOrDefault().SValue);
                var uploadCenterPurchaseBackInvoice = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterPurchaseBackInvoiceSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterPurchaseBackInvoiceName = uploadCenterPurchaseBackInvoice.Name;
            }
            vm.UploadFileTree.UploadCenterPurchaseBackInvoiceSettingId = (int)GeneralSettingCl.UploadCenterPurchaseBackInvoice;
            //==================== Sell Invoice =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellInvoice).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterSellInvoiceSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellInvoice).FirstOrDefault().SValue);
                var uploadCenterSellInvoice = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterSellInvoiceSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterSellInvoiceName = uploadCenterSellInvoice.Name;
            }
            vm.UploadFileTree.UploadCenterSellInvoiceSettingId = (int)GeneralSettingCl.UploadCenterSellInvoice;
            //==================== Sell back Invoice =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellBackInvoice).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterSellBackInvoiceSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSellBackInvoice).FirstOrDefault().SValue);
                var uploadCenterSellBackInvoice = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterSellBackInvoiceSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterSellBackInvoiceName = uploadCenterSellBackInvoice.Name;
            }
            vm.UploadFileTree.UploadCenterSellBackInvoiceSettingId = (int)GeneralSettingCl.UploadCenterSellBackInvoice;
            //==================== Employee =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterEmployee).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterEmployeeSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterEmployee).FirstOrDefault().SValue);
                var uploadCenterEmployee = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterEmployeeSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterEmployeeName = uploadCenterEmployee.Name;
            }
            vm.UploadFileTree.UploadCenterEmployeeSettingId = (int)GeneralSettingCl.UploadCenterEmployee;
            //==================== Cheque =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCheque).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterChequeSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCheque).FirstOrDefault().SValue);
                var uploadCenterCheque = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterChequeSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterChequeName = uploadCenterCheque.Name;
            }
            vm.UploadFileTree.UploadCenterChequeSettingId = (int)GeneralSettingCl.UploadCenterCheque;
            //==================== Suppliers =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSupplier).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterSupplierSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterSupplier).FirstOrDefault().SValue);
                var uploadCenterSupplier = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterSupplierSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterSupplierName = uploadCenterSupplier.Name;
            }
            vm.UploadFileTree.UploadCenterSupplierSettingId = (int)GeneralSettingCl.UploadCenterSupplier;
            //==================== Customer =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCustomer).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterCustomerSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterCustomer).FirstOrDefault().SValue);
                var uploadCenterCustomer = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterCustomerSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterCustomerName = uploadCenterCustomer.Name;
            }
            vm.UploadFileTree.UploadCenterCustomerSettingId = (int)GeneralSettingCl.UploadCenterCustomer;
            //==================== Installment =======================
            if (model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterInstallment).FirstOrDefault().SValue != null)
            {
                vm.UploadFileTree.UploadCenterInstallmentSettingValue = Guid.Parse(model.Where(x => x.Id == (int)GeneralSettingCl.UploadCenterInstallment).FirstOrDefault().SValue);
                var uploadCenterInstallment = uploadCenterTree.Where(x => x.Id == vm.UploadFileTree.UploadCenterInstallmentSettingValue).FirstOrDefault();
                vm.UploadFileTree.UploadCenterInstallmentName = uploadCenterInstallment.Name;
            }
            vm.UploadFileTree.UploadCenterInstallmentSettingId = (int)GeneralSettingCl.UploadCenterInstallment;
            #endregion

            return View(vm);
        }


        [HttpPost]
        public JsonResult CreateEdit(GeneralSettingVM vm, int tabNum)
        {
            if (ModelState.IsValid)
            {

                if (TempData["userInfo"] != null)
                    auth = TempData["userInfo"] as VTSAuth;
                else
                    RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());

                switch (tabNum)
                {
                    case 1: // بيانات الجهة 
                        var model = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.EntityData).ToList();
                        foreach (var item in model)
                        {
                            if (item.Id == (int)GeneralSettingCl.EntityDataArea)
                                item.SValue = vm.AreaId.ToString();
                            if (item.Id == (int)GeneralSettingCl.EntityDataEntityDataName)
                                item.SValue = vm.EntityData.Name;
                            if (item.Id == (int)GeneralSettingCl.EntityDataAbbreviationName)
                                item.SValue = vm.EntityData.AbbreviationName;
                            if (item.Id == (int)GeneralSettingCl.EntityDataAddress)
                                item.SValue = vm.EntityData.Address;
                            if (item.Id == (int)GeneralSettingCl.EntityDataCommercialRegisterNo)
                                item.SValue = vm.EntityData.CommercialRegisterNo;
                            if (item.Id == (int)GeneralSettingCl.EntityDataTaxCardNo)
                                item.SValue = vm.EntityData.TaxCardNo;
                            if (item.Id == (int)GeneralSettingCl.EntityDataTel1)
                                item.SValue = vm.EntityData.Tel1;
                            if (item.Id == (int)GeneralSettingCl.EntityDataTel2)
                                item.SValue = vm.EntityData.Tel2;
                            if (item.Id == (int)GeneralSettingCl.BarCodeDataLetter)
                                item.SValue = vm.Letters.ToString();
                            if (item.Id == (int)GeneralSettingCl.BarCodeDataNumber)
                                item.SValue = vm.Numbers.ToString();

                            db.Entry(item).State = EntityState.Modified;
                        }
                        //تاريخ بداية ونهاية السنة المالية
                        var financialYear = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.FinancialYearDate).ToList();
                        foreach (var item in financialYear)
                        {
                            if (item.Id == (int)GeneralSettingCl.FinancialYearStartDate)
                                item.SValue = vm.FinancialYearStartDate.Value.ToString("yyyy-MM-dd");
                            if (item.Id == (int)GeneralSettingCl.FinancialYearEndDate)
                                item.SValue = vm.FinancialYearEndDate.Value.ToString("yyyy-MM-dd");

                            db.Entry(item).State = EntityState.Modified;
                        }

                        // طول الباركود
                        var modelBarCode = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.BarCodeData).ToList();
                        foreach (var item in modelBarCode)
                        {
                            if (item.Id == (int)GeneralSettingCl.BarCodeDataLetter)
                                item.SValue = vm.Letters.ToString();
                            if (item.Id == (int)GeneralSettingCl.BarCodeDataNumber)
                                item.SValue = vm.Numbers.ToString();

                            db.Entry(item).State = EntityState.Modified;
                        }

                        // احتساب تكلفة المنتج 
                        // قبول اضافة اصناف بدون رصيد فى فواتير البيع 
                        var itemCosts = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.Items).ToList();
                        foreach (var item in itemCosts)
                        {
                            // احتساب تكلفة المنتج 
                            if (item.Id == (int)GeneralSettingCl.ItemCostCalculateId)
                                item.SValue = vm.ItemCostCalculateId.ToString();
                            // قبول اضافة اصناف بدون رصيد فى فواتير البيع                             
                            if (item.Id == (int)GeneralSettingCl.ItemAcceptNoBalance)
                                item.SValue = vm.ItemAcceptNoBalanceId.ToString();

                            // قبول/رفض صرف نقدية بدون رصيد                            
                            if (item.Id == (int)GeneralSettingCl.PaidWithBalance)
                                item.SValue = vm.PaidWithBalance.ToString();

                            // الاعتماد المباشر بعد حفظ فواتير البيع والتوريد                             
                            if (item.Id == (int)GeneralSettingCl.InvoicesApprovalAfterSave)
                                item.SValue = vm.InvoicesApprovalAfterSaveId.ToString();

                            // اظهار تكلفة الصنف فى شاشة البيع                             
                            if (item.Id == (int)GeneralSettingCl.ItemCostCalculateShowInSellReg)
                                item.SValue = vm.ItemCostCalculateShowInSellRegId.ToString();

                            // المدة المسموح بها فى سداد قسط                             
                            if (item.Id == (int)GeneralSettingCl.PeriodAllowedPayInstallment)
                                item.SValue = vm.PeriodAllowedPayInstallment.ToString();

                            // السماح بالبيع عند تخطى الحد الائتمانى للخطر                             
                            if (item.Id == (int)GeneralSettingCl.LimitDangerSell)
                                item.SValue = vm.LimitDangerSell.ToString();

                            db.Entry(item).State = EntityState.Modified;
                        }
                        break;

                    case 3: //مخزن  الافتراضى
                        var modelStore = db.GeneralSettings.Where(x => !x.IsDeleted && x.SType == (int)GeneralSettingTypeCl.StoreDefault).ToList();
                        foreach (var item in modelStore)
                        {
                            //التصنيع الداخلى الافتراضى
                            if (item.Id == (int)GeneralSettingCl.StoreProductionInternalId)
                                item.SValue = vm.ProductionInStoreId.ToString();
                            //التصنيع الداخلى الافتراضى
                            if (item.Id == (int)GeneralSettingCl.StoreDefaultlId)
                                item.SValue = vm.DefaultStoreId.ToString();
                            //تحت التصنيع الافتراضى
                            if (item.Id == (int)GeneralSettingCl.StoreUnderProductionId)
                                item.SValue = vm.ProductionUnderStoreId.ToString();
                            db.Entry(item).State = EntityState.Modified;
                            //تحت الصيانة
                            if (item.Id == (int)GeneralSettingCl.StoreMaintenance)
                                item.SValue = vm.MaintenanceStoreId.ToString();
                            db.Entry(item).State = EntityState.Modified;
                            //تحت توالف الصيانة
                            if (item.Id == (int)GeneralSettingCl.StoreMaintenanceDamage)
                                item.SValue = vm.MaintenanceDamageStoreId.ToString();
                            db.Entry(item).State = EntityState.Modified;
                        };

                        break;

                    default:
                        break;
                }




                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    return Json(new { isValid = true, message = "تم تحديث البيانات بنجاح" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });
            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }
        [HttpPost]
        public JsonResult SaveAccountSetting(string accountId, string settingId)
        {

            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());


            if (!string.IsNullOrEmpty(accountId) || !string.IsNullOrEmpty(settingId))
            {
                var generalSettId = int.Parse(settingId);
                var general = db.GeneralSettings.FirstOrDefault(x=>x.Id== generalSettId);
                general.SValue = accountId;
                db.Entry(general).State = EntityState.Modified;

                var accountTreeId = Guid.Parse(accountId);
                var accountNum = db.AccountsTrees.FirstOrDefault(x=>x.Id== accountTreeId).AccountNumber;
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    return Json(new { isValid = true, accountNum = accountNum, message = "تم تحديث البيانات بنجاح" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }

        //حفظ اعدادات مركز التحميل 
        [HttpPost]
        public JsonResult SaveUploadCenterSetting(string folderId, string settingId)
        {

            if (TempData["userInfo"] != null)
                auth = TempData["userInfo"] as VTSAuth;
            else
                RedirectToAction("Login", "Default", Request.Url.AbsoluteUri.ToString());


            if (!string.IsNullOrEmpty(folderId) || !string.IsNullOrEmpty(settingId))
            {
                var generalSettId = int.Parse(settingId);
                var general = db.GeneralSettings.FirstOrDefault(x=>x.Id== generalSettId);
                general.SValue = folderId;
                db.Entry(general).State = EntityState.Modified;

                //var accountNum = db.AccountsTrees.FirstOrDefault(x=>x.Id==Guid.Parse(accountId)).AccountNumber;
                if (db.SaveChanges(auth.CookieValues.UserId) > 0)
                {
                    return Json(new { isValid = true, message = "تم تحديث البيانات بنجاح" });

                }
                else
                    return Json(new { isValid = false, message = "حدث خطأ اثناء تنفيذ العملية" });

            }
            else
                return Json(new { isValid = false, message = "تأكد من ادخال البيانات بشكل صحيح" });

        }
    }
}