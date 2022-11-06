using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ERP.DAL.Models;

namespace ERP.DAL
{
    public partial class VTSaleEntities : DbContext
    {
        public VTSaleEntities() : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<VTSaleEntities>(null);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<AccountsTree> AccountsTrees { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<CasesItemSerialHistory> CasesItemSerialHistories { get; set; }
        public virtual DbSet<CasesPurchaseInvoiceHistory> CasesPurchaseInvoiceHistories { get; set; }
        public virtual DbSet<CasesSellInvoiceHistory> CasesSellInvoiceHistories { get; set; }
        public virtual DbSet<Cheque> Cheques { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Container> Containers { get; set; }
        public virtual DbSet<ContractAttendanceLeaving> ContractAttendanceLeavings { get; set; }
        public virtual DbSet<ContractDefinitionVacation> ContractDefinitionVacations { get; set; }
        public virtual DbSet<ContractLoan> ContractLoans { get; set; }
        public virtual DbSet<ContractLoanScheduling> ContractLoanSchedulings { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<ContractSalaryAddition> ContractSalaryAdditions { get; set; }
        public virtual DbSet<ContractSalaryPenalty> ContractSalaryPenalties { get; set; }
        public virtual DbSet<ContractSalaryType> ContractSalaryTypes { get; set; }
        public virtual DbSet<ContractSchedulingAbsence> ContractSchedulingAbsences { get; set; }
        public virtual DbSet<ContractScheduling> ContractSchedulings { get; set; }
        public virtual DbSet<ContractSchedulingsMain> ContractSchedulingsMains { get; set; }
        public virtual DbSet<ContractType> ContractTypes { get; set; }
        public virtual DbSet<ContractVacation> ContractVacations { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }
        public virtual DbSet<DelayAbsenceSystem> DelayAbsenceSystems { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EmployeeGivingCustody> EmployeeGivingCustodies { get; set; }
        public virtual DbSet<EmployeeReturnCashCustody> EmployeeReturnCashCustodies { get; set; }
        public virtual DbSet<EmployeeReturnCustody> EmployeeReturnCustodies { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<ExpenseIncome> ExpenseIncomes { get; set; }
        public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<GeneralDaily> GeneralDailies { get; set; }
        public virtual DbSet<GeneralRecord> GeneralRecords { get; set; }
        public virtual DbSet<GeneralSetting> GeneralSettings { get; set; }
        public virtual DbSet<GeneralSettingType> GeneralSettingTypes { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupType> GroupTypes { get; set; }
        public virtual DbSet<IncomeType> IncomeTypes { get; set; }
        public virtual DbSet<Installment> Installments { get; set; }
        public virtual DbSet<InstallmentSchedule> InstallmentSchedules { get; set; }
        public virtual DbSet<InventoryInvoiceDetail> InventoryInvoiceDetails { get; set; }
        public virtual DbSet<InventoryInvoice> InventoryInvoices { get; set; }
        public virtual DbSet<ItemCostCalculation> ItemCostCalculations { get; set; }
        public virtual DbSet<ItemIntialBalance> ItemIntialBalances { get; set; }
        public virtual DbSet<ItemPrice> ItemPrices { get; set; }
        public virtual DbSet<ItemProductionDetail> ItemProductionDetails { get; set; }
        public virtual DbSet<ItemProduction> ItemProductions { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemSerial> ItemSerials { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<ItemUnit> ItemUnits { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<MaintenanceCaseHistory> MaintenanceCaseHistories { get; set; }
        public virtual DbSet<MaintenanceCas> MaintenanceCases { get; set; }
        public virtual DbSet<MaintenanceDamageDetail> MaintenanceDamageDetails { get; set; }
        public virtual DbSet<MaintenanceDamage> MaintenanceDamages { get; set; }
        public virtual DbSet<MaintenanceDetail> MaintenanceDetails { get; set; }
        public virtual DbSet<MaintenanceIncome> MaintenanceIncomes { get; set; }
        public virtual DbSet<Maintenance> Maintenances { get; set; }
        public virtual DbSet<MaintenanceSparePart> MaintenanceSpareParts { get; set; }
        public virtual DbSet<MaintenProblemType> MaintenProblemTypes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PagesRole> PagesRoles { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<PersonCategory> PersonCategories { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonType> PersonTypes { get; set; }
        public virtual DbSet<PointOfSale> PointOfSales { get; set; }
        public virtual DbSet<PriceInvoice> PriceInvoices { get; set; }
        public virtual DbSet<PriceInvoicesDetail> PriceInvoicesDetails { get; set; }
        public virtual DbSet<PricesChanx> PricesChanges { get; set; }
        public virtual DbSet<PricingPolicy> PricingPolicies { get; set; }
        public virtual DbSet<ProductionOrderColor> ProductionOrderColors { get; set; }
        public virtual DbSet<ProductionOrderDetail> ProductionOrderDetails { get; set; }
        public virtual DbSet<ProductionOrderExpens> ProductionOrderExpenses { get; set; }
        public virtual DbSet<ProductionOrderReceipt> ProductionOrderReceipts { get; set; }
        public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }
        public virtual DbSet<PurchaseBackInvoice> PurchaseBackInvoices { get; set; }
        public virtual DbSet<PurchaseBackInvoicesDetail> PurchaseBackInvoicesDetails { get; set; }
        public virtual DbSet<PurchaseBackInvoicesExpens> PurchaseBackInvoicesExpenses { get; set; }
        public virtual DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public virtual DbSet<PurchaseInvoicesDetail> PurchaseInvoicesDetails { get; set; }
        public virtual DbSet<PurchaseInvoicesExpens> PurchaseInvoicesExpenses { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Safe> Safes { get; set; }
        public virtual DbSet<SalaryAddition> SalaryAdditions { get; set; }
        public virtual DbSet<SalaryAdditionType> SalaryAdditionTypes { get; set; }
        public virtual DbSet<SalaryPenalty> SalaryPenalties { get; set; }
        public virtual DbSet<SalaryPenaltyType> SalaryPenaltyTypes { get; set; }
        public virtual DbSet<SaleMenArea> SaleMenAreas { get; set; }
        public virtual DbSet<SaleMenCustomer> SaleMenCustomers { get; set; }
        public virtual DbSet<SaleMenStoreHistory> SaleMenStoreHistories { get; set; }
        public virtual DbSet<SelectorType> SelectorTypes { get; set; }
        public virtual DbSet<SellBackInvoiceIncome> SellBackInvoiceIncomes { get; set; }
        public virtual DbSet<SellBackInvoice> SellBackInvoices { get; set; }
        public virtual DbSet<SellBackInvoicesDetail> SellBackInvoicesDetails { get; set; }
        public virtual DbSet<SellInvoiceIncome> SellInvoiceIncomes { get; set; }
        public virtual DbSet<SellInvoicePayment> SellInvoicePayments { get; set; }
        public virtual DbSet<SellInvoice> SellInvoices { get; set; }
        public virtual DbSet<SellInvoicesDetail> SellInvoicesDetails { get; set; }
        public virtual DbSet<SerialCas> SerialCases { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<ShiftsOffline> ShiftsOfflines { get; set; }
        public virtual DbSet<SocialStatus> SocialStatuses { get; set; }
        public virtual DbSet<StoreAdjustment> StoreAdjustments { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoresTransferDetail> StoresTransferDetails { get; set; }
        public virtual DbSet<StoresTransfer> StoresTransfers { get; set; }
        public virtual DbSet<SupplierPayment> SupplierPayments { get; set; }
        public virtual DbSet<TransactionsType> TransactionsTypes { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<UploadCenter> UploadCenters { get; set; }
        public virtual DbSet<UploadCenterType> UploadCenterTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<VacationDay> VacationDays { get; set; }
        public virtual DbSet<VacationType> VacationTypes { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<WeekDay> WeekDays { get; set; }
        public virtual DbSet<WorkingPeriod> WorkingPeriods { get; set; }
        public virtual DbSet<PersonIntialBalance> PersonIntialBalances { get; set; }
        public virtual DbSet<DamageInvoice> DamageInvoices { get; set; }
        public virtual DbSet<DamageInvoiceDetail> DamageInvoiceDetails { get; set; }
        public virtual DbSet<ItemCustomSellPrice> ItemCustomSellPrices { get; set; }
        public virtual DbSet<NotificationEmployee> NotificationEmployees { get; set; }
        public virtual DbSet<Nationality> Nationalities { get; set; }
        public virtual DbSet<StorePermission> StorePermissions { get; set; }
        public virtual DbSet<StorePermissionItem> StorePermissionItems { get; set; }
        public virtual DbSet<ProductionType> ProductionTypes { get; set; }
        public virtual DbSet<StoresTransferCase> StoresTransferCases { get; set; }
        public virtual DbSet<StoresTransferHistory> StoresTransferHistories { get; set; }
        public virtual DbSet<OfferType> OfferTypes { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OfferDetail> OfferDetails { get; set; }
        public virtual DbSet<OfferSellInvoice> OfferSellInvoices { get; set; }
        public virtual DbSet<ContractCustomerSupplier> ContractCustomerSuppliers { get; set; }
        public virtual DbSet<QuoteOrderSell> QuoteOrderSells { get; set; }
        public virtual DbSet<QuoteOrderSellDetail> QuoteOrderSellDetails { get; set; }
    }
}
