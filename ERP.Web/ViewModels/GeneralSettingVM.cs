using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Web.ViewModels
{
    public class GeneralSettingVM
    {
        public GeneralSettingVM()
        {
            EntityData = new EntityData();
            AccountTree = new AccountTreeVM();
            UploadFileTree = new UploadFileTreeVM();
            PrintSetting = new PrintSetting();
        }
        public Guid Id { get; set; }
        public Nullable<Guid> AreaId { get; set; }
        public Nullable<Guid> ProductionInStoreId { get; set; }
        public Nullable<Guid> DefaultStoreId { get; set; }
        public Nullable<Guid> ProductionUnderStoreId { get; set; }
        public Nullable<Guid> MaintenanceStoreId { get; set; }
        public Nullable<Guid> MaintenanceDamageStoreId { get; set; }
        public Nullable<int> ItemCostCalculateId { get; set; } //احتساب تكلفة المنتج 
        public Nullable<int> ItemAcceptNoBalanceId { get; set; } //قبول اضافة اصناف بدون رصيد فى فواتير البيع 
        public Nullable<int> PaidWithBalance { get; set; } //قبول/رفض صرف نقدية بدون رصيد 
        public Nullable<int> InvoicesApprovalAfterSaveId { get; set; } //الاعتماد المباشر بعد حفظ فواتير البيع والتوريد 
        public Nullable<int> ItemCostCalculateShowInSellRegId { get; set; } //اظهار تكلفة الصنف فى شاشة البيع 
        public Nullable<int> PeriodAllowedPayInstallment { get; set; } //المدة المسموح بها فى سداد قسط 
        public Nullable<int> LimitDangerSell { get; set; } //السماح بالبيع عند تخطى الحد الائتمانى للخطر 
        public Nullable<int> TaxPercentage { get; set; } //نسبة الضريبة 
        public Nullable<int> TaxProfitPercentage { get; set; } //نسبة الضريبة ارباح تجارية 
        public Nullable<int> SellPriceZero { get; set; } //سعر البيع يقبل صفر (الهدايا)   
        public Nullable<int> AcceptItemCostSellDown { get; set; } //السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته   
        public Nullable<DateTime> StartDateSearch { get; set; }// بداية تاريخ البحث فى الموقع 
        public Nullable<DateTime> EndDateSearch { get; set; }// نهاية تاريخ البحث فى الموقع 
        public bool Letters { get; set; }
        public bool Numbers { get; set; }
        public int InventoryType { get; set; } //نوع الجرد 1-جرد دورى   2-جرد مستمر

        public EntityData EntityData { get; set; }
        public AccountTreeVM AccountTree { get; set; }
        public UploadFileTreeVM UploadFileTree { get; set; }
        public PrintSetting PrintSetting { get; set; }
    }
    public class EntityData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AbbreviationName { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Address { get; set; }
        public string CommercialRegisterNo { get; set; }
        public string TaxCardNo { get; set; }

    }
    public class AccountTreeVM
    {
        public int? PurchaseAccountSettingId { get; set; }
        public Guid? PurchaseAccountSettingValue { get; set; }
        public string PurchaseAccountName { get; set; }

        public int? SalesAccountSettingId { get; set; }
        public Guid? SalesAccountSettingValue { get; set; }
        public string SalesAccountName { get; set; }

        public int? SupplierAccountSettingId { get; set; }
        public Guid? SupplierAccountSettingValue { get; set; }
        public string SupplierAccountName { get; set; }

        public int? CustomerAccountSettingId { get; set; }
        public Guid? CustomerAccountSettingValue { get; set; }
        public string CustomerAccountName { get; set; }

        public int? SalesReturnAccountSettingId { get; set; }
        public Guid? SalesReturnAccountSettingValue { get; set; }
        public string SalesReturnAccountName { get; set; }

        public int? PurchaseReturnAccountSettingId { get; set; }
        public Guid? PurchaseReturnAccountSettingValue { get; set; }
        public string PurchaseReturnAccountName { get; set; }

        public int? DeliveryAccountSettingId { get; set; }
        public Guid? DeliveryAccountSettingValue { get; set; }
        public string DeliveryAccountName { get; set; }

        public int? SalesTaxAccountSettingId { get; set; }
        public Guid? SalesTaxAccountSettingValue { get; set; }
        public string SalesTaxAccountName { get; set; }

        public int? AddedTaxAccountSettingId { get; set; }
        public Guid? AddedTaxAccountSettingValue { get; set; }
        public string AddedTaxAccountName { get; set; }

        public int? PremittedDiscountAccountSettingId { get; set; }
        public Guid? PremittedDiscountAccountSettingValue { get; set; }
        public string PremittedDiscountAccountName { get; set; }

        public int? EarnedDiscountSettingId { get; set; }
        public Guid? EarnedDiscountSettingValue { get; set; }
        public string EarnedDiscountName { get; set; }

        public int? SafeAccountSettingId { get; set; }
        public Guid? SafeAccountSettingValue { get; set; }
        public string SafeAccountName { get; set; }

        public int? GeneralRevenusSettingId { get; set; }
        public Guid? GeneralRevenusSettingValue { get; set; }
        public string GeneralRevenusName { get; set; }

        public int? BankAccountSettingId { get; set; }
        public Guid? BankAccountSettingValue { get; set; }
        public string BankAccountName { get; set; }

        public int? ExpensesAccountSettingId { get; set; }
        public Guid? ExpensesAccountSettingValue { get; set; }
        public string ExpensesAccountName { get; set; }

        public int? CommercialTaxSettingId { get; set; }
        public Guid? CommercialTaxSettingValue { get; set; }
        public string CommercialTaxName { get; set; }

        public int? ShareCapitalAccountSettingId { get; set; }
        public Guid? ShareCapitalAccountSettingValue { get; set; }
        public string ShareCapitalAccountName { get; set; }

        public int? StockAccountSettingId { get; set; }
        public Guid? StockAccountSettingValue { get; set; }
        public string StockAccountName { get; set; }

        public int? CheckUnderCollAccountSettingId { get; set; }//اوراق قبض
        public Guid? CheckUnderCollAccountSettingValue { get; set; }
        public string CheckUnderCollAccountName { get; set; }
        public int? CheckUnderCollAccountPaymentSettingId { get; set; } //اوراق دفع
        public Guid? CheckUnderCollAccountPaymentSettingValue { get; set; }
        public string CheckUnderCollAccountPaymentName { get; set; }

        public int? LoanEmpAccountSettingId { get; set; }
        public Guid? LoanEmpAccountSettingValue { get; set; }
        public string LoanEmpAccountName { get; set; }

        public int? SalariesAccountSettingId { get; set; }
        public Guid? SalariesAccountSettingValue { get; set; }
        public string SalariesAccountName { get; set; }

        public int? EmployeeAccountSettingId { get; set; }
        public Guid? EmployeeAccountSettingValue { get; set; }
        public string EmployeeAccountName { get; set; }

        public int? MiscellaneousRevenusAccountSettingId { get; set; }
        public Guid? MiscellaneousRevenusAccountSettingValue { get; set; }
        public string MiscellaneousRevenusAccountName { get; set; }

        public int? CustodyAccountSettingId { get; set; }
        public Guid? CustodyAccountSettingValue { get; set; }
        public string CustodyAccountName { get; set; }

        public int? MaintenanceAccountSettingId { get; set; }
        public Guid? MaintenanceAccountSettingValue { get; set; }
        public string MaintenanceAccountName { get; set; }

        public int? DestructionAllowanceAccountSettingId { get; set; }
        public Guid? DestructionAllowanceAccountSettingValue { get; set; }
        public string DestructionAllowanceAccountName { get; set; }

        public int? AssetsDepreciationComplexAccountSettingId { get; set; }
        public Guid? AssetsDepreciationComplexAccountSettingValue { get; set; }
        public string AssetsDepreciationComplexAccountName { get; set; }

        public int? InstallmentsBenefitsAccountSettingId { get; set; }
        public Guid? InstallmentsBenefitsAccountSettingValue { get; set; }
        public string InstallmentsBenefitsAccountName { get; set; }
        //حساب تكلفة البضاعه المباعه فى حالة الجرد المستمر 
        public int? SaleCostAccountSettingId { get; set; }
        public Guid? SaleCostAccountSettingValue { get; set; }
        public string SaleCostAccountName { get; set; }
        //حساب انحراف التشغيل فى حالة جرد المخزن 
        public int? InventoryOperatingDeviationAccountSettingId { get; set; }
        public Guid? InventoryOperatingDeviationAccountSettingValue { get; set; }
        public string InventoryOperatingDeviationAccountName { get; set; }

    }
    public class UploadFileTreeVM
    {
        public int? UploadCenterPurchaseInvoiceSettingId { get; set; }
        public Guid? UploadCenterPurchaseInvoiceSettingValue { get; set; }
        public string UploadCenterPurchaseInvoiceName { get; set; }

        public int? UploadCenterPurchaseBackInvoiceSettingId { get; set; }
        public Guid? UploadCenterPurchaseBackInvoiceSettingValue { get; set; }
        public string UploadCenterPurchaseBackInvoiceName { get; set; }

        public int? UploadCenterSellInvoiceSettingId { get; set; }
        public Guid? UploadCenterSellInvoiceSettingValue { get; set; }
        public string UploadCenterSellInvoiceName { get; set; }

        public int? UploadCenterSellBackInvoiceSettingId { get; set; }
        public Guid? UploadCenterSellBackInvoiceSettingValue { get; set; }
        public string UploadCenterSellBackInvoiceName { get; set; }

        public int? UploadCenterEmployeeSettingId { get; set; }
        public Guid? UploadCenterEmployeeSettingValue { get; set; }
        public string UploadCenterEmployeeName { get; set; }

        public int? UploadCenterChequeSettingId { get; set; }
        public Guid? UploadCenterChequeSettingValue { get; set; }
        public string UploadCenterChequeName { get; set; }

        public int? UploadCenterSupplierSettingId { get; set; }
        public Guid? UploadCenterSupplierSettingValue { get; set; }
        public string UploadCenterSupplierName { get; set; }

        public int? UploadCenterCustomerSettingId { get; set; }
        public Guid? UploadCenterCustomerSettingValue { get; set; }
        public string UploadCenterCustomerName { get; set; }

        public int? UploadCenterInstallmentSettingId { get; set; }
        public Guid? UploadCenterInstallmentSettingValue { get; set; }
        public string UploadCenterInstallmentName { get; set; }

        public int? UploadCenterProductionOrderSettingId { get; set; }
        public Guid? UploadCenterProductionOrderSettingValue { get; set; }
        public string UploadCenterProductionOrderName { get; set; }
        public int? UploadCenterGeneralRecordSettingId { get; set; }
        public Guid? UploadCenterGeneralRecordSettingValue { get; set; }
        public string UploadCenterGeneralRecordName { get; set; }

    }
    public class PrintSetting
    {
        public int Id { get; set; }
        public HttpPostedFileBase LogoEntity { get; set; }
        public string Line1Up { get; set; }//سطر اول اعلى الصفحة
        public string Line2Up { get; set; }//سطر ثاني اعلى الصفحة
        public string Line3Up { get; set; }//سطر ثالث اعلى الصفحة
        public string Line1Down { get; set; }//سطر اول اسفل الصفحة
        public string Line2Down { get; set; }//سطر اول اسفل الصفحة
        public string QuotationNoteAr { get; set; }//ملاحظة عروض الاسعار فى الطباعة بالعربية
        public string QuotationNoteEn { get; set; }//ملاحظة عروض الاسعار فى الطباعة بالانجليزية

    }
}