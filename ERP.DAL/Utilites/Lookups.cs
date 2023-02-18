//using ERP.DAL;
//using ERP.DAL;
using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.DAL.Utilites;

namespace ERP.Web.Utilites
{
    public class Lookups
    {
        //admin:1   MyomSG2prP7o/pLAjDi8Hg==
        #region Lookups on Project
        public enum RoleCl
        {
            Admin = 1, //مدير النظام
        }
        public enum PersonTypeCl
        {
            Supplier = 1,//مورد
            Customer = 2,//عميل
            SupplierAndCustomer = 3,// مورد وعميل
            Employee = 4,// موظف
            SupplierAndCustomerResponsible = 5,// مسئول عميل / مورد
        }
        public enum GroupTypeCl // مجموعات الاصناف
        {
            Basic = 1,//اساسية
            Sell = 2,//بيع
        }
        public enum SocialStatusC1
        {
            Single = 1, //اعزب
            Married = 2,//متزوج
            Divorced = 3,//مطلق
            Widower = 4//ارمل
        }
        public enum ProductionTypeCl //نوع المنتج (مدخلات/مخرجات) مثلا مواد خام/منتج نهائى
        {
            In = 1, //مدخلات
            Out = 2,//مخرجات
        }     
        public enum OfferTypeCl //انواع العروض 
        {
            ForItem = 1, //لصنف/اكتر
            ForInvoice = 2,//لفاتورة
        }

        // الاعدادات العامة
        public enum GeneralSettingCl
        {
            AccountTreePurchaseAccount = 1, // حساب المشتريات
            AccountTreeSalesAccount,//حساب المبيعات
            AccountTreeSupplierAccount, //حساب الموردين
            AccountTreeCustomerAccount, //حساب العملاء
            AccountTreeSalesReturnAccount, // حساب مرددوات المبيعات
            AccountTreePurchaseReturnAccount, // حساب مرددوات المشتريات
            AccountTreeDeliveryAccount, // حساب خدمة الدليفرى
            AccountTreeSalesTaxAccount, // حساب ضريبة المبيعات
            AccountTreeAddedTaxAccount, //حساب الضريبة المضافة
            AccountTreePremittedDiscountAccount, //حساب الخصم المسموح به
            AccountTreeEarnedDiscount, //حساب الخصم المكتسب
            AccountTreeSafeAccount, //حساب الخزينة
            AccountTreeGeneralRevenus, //حساب انواع لايرادات 
            AccountTreeBankAccount, //حساب البنك
            AccountTreeExpensesAccount, //حساب انواع المصروفات
            AccountTreeCommercialTax, //حساب ضريبة ارباح تجارية
            AccountTreeShareCapitalAccount, //حساب رأس المال
            AccountTreeStockAccount, //حساب المخزون
            AccountTreeCheckUnderCollectionReceipts, //حساب شيكات تحت التحصيل اوراق قبض

            EntityDataEntityDataName, //اسم الجهة
            EntityDataAbbreviationName,//الاسم المختصر للجهة
            EntityDataTel1,//رقم تلفون الجهة 1
            EntityDataTel2,//رقم تلفون الجهة 2
            EntityDataArea,//منطقة الجهة
            EntityDataAddress,//عنوان الجهة
            EntityDataCommercialRegisterNo,//رقم السجل التجارى للجهة
            EntityDataTaxCardNo,//رقم السجل الضريبى للجهة

            BarCodeDataLetter, //الباركود يحتوى على حروف
            BarCodeDataNumber, //الباركود يحتوى على ارقام

            StoreProductionInternalId, //مخزن التصنيع الداخلى الافتراضى للجهة
            StoreDefaultlId, //المخزن الافتراضى فى فواتير البيع والشراء 
            StoreUnderProductionId, //مخزن تحت التصنيع الافتراضى للجهة


            FinancialYearStartDate, // تاريخ بداية السنة المالية
            FinancialYearEndDate, // تاريخ نهاية السنة المالية

            UploadCenterPurchaseInvoice, //  رفع ملفات فاتورة التوريد
            UploadCenterPurchaseBackInvoice,//رفع ملفات  فاتورة مرتجع توريد 
            UploadCenterSellInvoice, //رفع ملفات  فاتورة بيع
            UploadCenterSellBackInvoice, //رفع ملفات  فاتورة مرتجع بيع
            UploadCenterEmployee, //رفع ملفات  موظف  
            UploadCenterCheque, //رفع ملفات شيك بنكى 

            ItemCostCalculateId,   // طريقة احتساب تكلفة المنتج (متوسك سعر الشراء-اخر سعر شراء - اعلى سعر شراء ...

            AccountTreeLoans, //حساب السلف والقروض
            AccountTreeSalaries, //حساب الرواتب والاجور
            AccountTreeEmployeeAccount, //حساب ذمم الموظفين /اجور مستحقة للعاملين
            AccountTreeMiscellaneousRevenus, //حساب الايرادات مستحقة التحصيل (استقطاعات وخصومات الموظفين
            AccountTreeCustodyAccount, //حساب عهد الموظفين/سلفة مؤقتة
            AccountTreeMaintenance, //حساب ايرادات الصيانة

            StoreMaintenance, //مخزن الصيانة
            StoreMaintenanceDamage, //مخزن توالف الصيانة

            EntityDataSchema, //اسم الاسكيما الخاص بالمؤسسة
            EntityDataSecurity, //رقم الحماية فى حالة التشغيل الاوفلاين
            AccountTreeDestructionAllowance, //حساب مخصص اهلاك الاصول الثابتة 
            AccountTreeAssetsDepreciationComplex, //حساب مجمع اهلاك الاصول الثابتة 

            ItemAcceptNoBalance, //قبول اضافة اصناف بدون رصيد فى فواتير البيع
            InvoicesApprovalAfterSave, //الاعتماد المباشر بعد حفظ فواتير البيع والتوريد
            ItemCostCalculateShowInSellReg, //اظهار تكلفة الصنف فى شاشة البيع 
            PeriodAllowedPayInstallment, //المدة المسموح بها فى سداد قسط 
            UploadCenterSupplier, //مجلد الموردين 
            UploadCenterCustomer, //مجلد العملاء 
            UploadCenterInstallment, //مجلد الاقساط 
            AccountTreeInstallmentsBenefits, //حساب فوائد الاقساط 
            AccountTreeCheckUnderCollectionPayments, //حساب شيكات برسم السداد اوراق دفع
            PaidWithBalance, //قبول/رفض صرف نقدية بدون رصيد
            LimitDangerSell, //السماح بالبيع عند تخطى الحد الائتمانى للخطر
            InventoryType, //نوع الجرد (دورى/مستمر)

            //اعدادات الطباعه
            LogoEntity, //لوجو المؤسسة
            PrintLine1Up,//السطر الاول اعلى الصفحة
            PrintLine2Up,//السطر الثانى اعلى الصفحة
            PrintLine3Up,//السطر الثالث اعلى الصفحة
            PrintLine1Down,//السطر الاول اسفل الصفحة
            PrintLine2Down,//السطر الثانى اسفل الصفحة

          
            AccountTreeSalesCost,  //حساب تكلفة بضاعه مباعه مع الجرد المستمر 
            TaxPercentage,//نسبة ضريبة القيمة المضافة
            TaxProfitPercentage,//نسبة ضريبة ارباح تجارية 

            UploadCenterProductionOrder, //مجلد اوامر الانتاج 
            UploadCenterGeneralRecord, //مجلد قيود اليومية 
            StartDateSearch, //بداية تاريخ البحث فى الموقع 
            EndDateSearch, //نهاية تاريخ البحث فى الموقع 
            QuotationNoteAr, //ملاحظة عروض الاسعار فى الطباعة بالعربية 
            QuotationNoteEn, //ملاحظة عروض الاسعار فى الطباعة بالانجليزية 
            AccountTreeInventoryOperatingDeviation, //حساب انحراف التشغيل فى حالة جرد المخزن  
            SellPriceZero, //سعر البيع يقبل صفر (الهدايا)  
            AcceptItemCostSellDown, // السماح ببيع الصنف فى حالة سعر البيع اقل من تكلفته  

        }

        public enum GeneralSettingTypeCl // انواع الاعدادات
        {
            AccountTree = 1, //اعدادات شجرة الحسابات
            EntityData,   // بيانات الجهة
            BarCodeData, // الباركود وطوله
            StoreDefault, // مخازن المواد الخام والتصنيع الافتراضيين الافتراضى
            FinancialYearDate,// تاريخ بداية ونهاية السنة المالية
            UploadCenterFiles, // رفع الملفات الى مركز التحميل
            OtherSetting //اعدادات اخرى 
        }
        public enum AccountTreeSelectorTypesCl // انواع الحسابات لسهولة فلتره التقارير
        {
            Supplier = 1, //مورد
            Customer,   // عميل
            SupplierAndCustomer, // مورد وعميل
            Expense, // مصروف
            Revenuse, // ايراد
            Safe, // الخزينة
            FixedAssets, // اصول ثابتة
            Bank, // بنك
            Loan, // سلف/قروض 
            Employee, // موظف 
            Custody, // عهد 
            Store, // مخزن فى حالة الجرد المستمر 
        }

        public enum PaymentTypeCl
        {
            Cash = 1, //نقدي
            Deferred = 2,//اجل
            Partial = 3,//جزئي
            Installment = 4,//تقسيط
            BankWallet = 5,//محفظة بنكية
            BankCard = 6//بطاقة بنكية(فيزا)
        }

        //انواع حركات المعاملات
        public enum TransactionsTypesCl
        {
            Purchases = 1, //مشتريات
            Sell,//مبيعات
            PurchasesReturn,//مرتجع مشتريات
            SellReturn, //مرتجع مبيعات
            CashOut, //صرف نقدية لمورد
            CashIn, //استلام (ادخال) نقدية من  عميل
            Expense, //تسجيل مصروف
            InitialBalanceItem, //رصيد اول اصناف
            FreeRestrictions, //قيود يومية
            ProductionOrderExpenses, //تكاليف وامر الانتاج
            ChequeOut, // صرف شيك لمورد
            ChequeIn, // استلام شيك من عميل 
            Loans, // سلف وقروض الموظفين 
            EmployeeSalaries, // صرف رواتب الموظفين 
            Custody, // عهد الموظفين 
            FixedAssets, // اصول ثابتة 
            ReturnCustody, // ارجاع عهدة مصروفة 
            ReturnCashCustody, // ارجاع عهدة نقدية 
            Maintenance, // صيانة 
            Destruction, // اهلاك الاصول 
            Income, // تسجيل ايراد 
            VoucherPayment, // سند صرف 
            VoucherReceipt, // سند قبض 
            Installment, // تقسيط 
            InitialBalanceCustomer, //رصيد اول المدة عملاء
            InitialBalanceSupplier, //رصيد اول المدة مودين
            InitialBalanceFixedAsset, //رصيد اول المدة اصول ثابتة
            InitialBalanceSafe, //رصيد اول المدة خزن
            InitialBalanceBank, //رصيد اول المدة بنوك
            InitialBalanceAsset, //رصيد اول المدة اصول
            InitialBalancePropertyRights, //رصيد اول المدة حقوق ملكية
            StorePermissionReceive, //اذن استلام
            StorePermissionLeave, //اذن صرف
            InitialBalanceAccountTree,//رصيد اول المدة لاي حساب فى الشجرة 

        }
        //حالات الفواتير
        public enum CasesCl
        {
            InvoiceCreated = 1, //تم تسجيل فاتورة التوريد
            InvoiceModified,//تم تعديل الفاتورة 
            InvoiceDeleted,//تم حذف الفاتورة  
            InvoiceApprovalAccountant, //تم اعتماد فاتورة التوريد محاسبيا
            InvoiceApprovalStore, //تم اعتماد فاتورة التوريد مخزنيا
            InvoiceQuantityDiffReal, //تم ادخال كميات فعلية مختلفة عن الاصلية 
            InvoiceFinalApproval, //تم اعتمادها بشكل نهائى 

            BackInvoiceCreated, //تم تسجيل فاتورة مرتجع التوريد
            BackInvoiceModified,//تم تعديل الفاتورة 
            BackInvoiceDeleted,//تم حذف الفاتورة  
            BackInvoiceApprovalAccountant, //تم اعتماد فاتورة مرتجع التوريد محاسبيا
            BackInvoiceApprovalStore, //تم اعتماد فاتورة  مرتجع التوريد مخزنيا
            BackInvoiceQuantityDiffReal, //تم ادخال كميات فعلية مختلفة عن الاصلية 
            BackInvoiceFinalApproval, //تم اعتمادها بشكل نهائى 
            FullBackInvoice, //تم ارجاع كامل فاتورة التوريد   
            PartialBackInvoice, //تم ارجاع جزء من فاتورة التوريد   

            InvoiceSaleMenCreated, //تم تسجيل فاتورة من خلال مندوب
            InvoiceSaleModified,//تم تعديل الفاتورة من خلال المندوب 
            BackInvoiceSaleMenCreated, //تم تسجيل فاتورة مرتجع من خلال مندوب
            BackInvoiceSaleModified,//تم تعديل الفاتورة مرتجع من خلال المندوب 

            InvoiceUnApproval, //تم فك الاعتماد للفاتورة   

            //MaintenanceInvoiceCreated, //تم تسجيل فاتورة صيانة
            //MaintenanceInvoiceModified,//تم تعديل فاتورة الصيانة 
            //MaintenanceInvoiceDeleted,//تم حذف الفاتورة  

        }
        public enum MaintenanceCaseCl
        {
            Pending = 1, // قيد الانتظار
            MaintenanceInProgress, // جارى الصيانة
            Refused,// تم الرفض
            Done, // تم تنفيذ الطلب 
            Postponed, //تم تأجيل الطلب
            ApprovalAccountant, //تم اعتماد الطلب محاسبيا
            ApprovalStore, //تم اعتماد الطلب مخزنيا
            ApprovalFinal, //تم اعتماد الطلب بشكل نهائى
        }
        //انواع رفع الملفات الى مركز التحميل
        public enum UploalCenterTypeCl
        {
            PurchaseInvoice = 1, // فاتورة التوريد
            PurchaseBackInvoice,//فاتورة مرتجع توريد 
            SellInvoice, //فاتورة بيع
            SellBackInvoice, //فاتورة مرتجع بيع
            Employee, //موظف 
            Cheque, //شيك بنكى 
            Supplier, //مورد 
            Customer, //عميل 
            Installment, //قسط 
            ProductionOrder, //اوامر الانتاج 
            GeneralRecord, //قيود اليومية 
        }

        //طريقة احتساب تكلفة المنتج شراء
        public enum ItemCostCalculationCl
        {
            AveragePurchasePrice = 1, // متوسط سعر الشراء
            LastPurchasePrice, //اخر سعر شراء
            HighestPurchasePrice, //اعلى سعر شراء
            LowestPurchasePrice, //اقل سعر شراء
        }
        // طريقة احتساب تكلفة المنتج بيع
        public enum ItemCostCalculationSellCl
        {
            AverageSellPrice = 1, // متوسط سعر البيع
            LastSellPrice, //اخر سعر البيع
            HighestSellPrice, //اعلى سعر البيع
            LowestSellPrice, //اقل سعر البيع
        }
        //حالات سيريال الاصناف
        public enum SerialCaseCl
        {
            Sell = 1, // بيع
            SellBack, //مرتجع بيع
            Maintenance, //فى الصيانة
            RepairedReceiptCustomer, //تم اصلاحه بعد الصيانة وتسليمه للعميل
            RepairedReceiptStore, //تم اصلاحه بعد الصيانة وتحويله الى مخزن 
            StoreTransfer, //تحويل مخزنى
            SellSparePart //بيع قطع غيار
            // تالفDamage, 

        }



        //حالات الرواتب للموظفين للعقد
        public enum ContractSalaryTypeCl
        {
            Monthly = 1, //شهرى
            Weekly, //اسبوعى
            Daily, // يومى
            Production, // بالانتاج
        }
        //انواع الاشعارات
        public enum NotificationTypeCl
        {
            ExpenesePeriodic = 1, //مصروفات دورية
            IncomePeriodic, // ايرادات دورية
            SellInvoiceDueDateClient,//موعد استحقاق فاتورة بيع 
            ChequesClients, // موعد استحقاق شيك من عميل 
            SellInvoicePayments, //موعد استحقاق دفعة من فاتورة آجل
            SellInvoiceInstallments, //موعد استحقاق قسط من فاتورة تقسيط
            ChequesSuppliers, // موعد استحقاق شيك الى مورد 
            Task, // مهمة ادارية 
            PurchaseInvoiceDueDateSupplier, // موعد استحقاق فاتورة توريد 
        }
        //حالات التحويلات المخزنية
        public enum StoresTransferCaseCl
        {
            StoreTransferCreated = 1, //تم تسجيل التحويل المخزنى
            StoreTransferUpdated , //تم تعديل التحويل المخزنى
            StoreTransferDeleted , //تم حذف التحويل المخزنى
            StoreTransferStoreApproval, //تم اعتماد التحويل مخزنيا
            StoreTransferStoreRefus, //تم رفض التحويل مخزنيا
            StoreTransferQuantityDiffReal, //تم ادخال كميات فعلية مختلفة عن الاصلية 
            StoreTransferFinalApproval, //تم اعتمادها بشكل نهائى 
            StoreTransferUnApproval, //فك اعتماد تحويل مخزنى 
        }

        //الفاتورة عرض سعر/امر بيع
        public enum QuoteOrderSellTypeCl
        {
            Qoute=1,//عرض سعر
            OrderSell//امر بيع
        }       
        //نوع اصناف امر البيع (اصناف للبيع/اصناف للانتاج)
        public enum OrderSellItemTypeCl
        {
            Sell=1,//اصناف للبيع
            ProductionOrder//اصناف للانتاج
        }       
        //حالات امر البيع (تم البيع /تم الانتاج/ جارى التنفيذ/تم الانتهاء)
        public enum OrderSellCaseCl
        {
            Pocessing=1,//جارى التنفيذ
            SellDone,//تم البيع
            ProductionOrderDone,//تم الانتاج
            Done,//تم الانتهاء
        }
        //ارقام حسابات المركز المالى 
        //public static int InvestmentFormation => 121; //تكوين استثمارى
        public static int InvestmentSpending => 122; //انفاق استثمارى
        public static int StorematerialsFuelSparePart => 161; //مخزن خامات ومواد ووقود وقطع غيار
        public static int NoCollection => 266; //مخصص ديون مشكوك فى تحصيلها
        public static int AccountsReceivableDepartment => 174; //حسابات مدينة لدى المصالح والهيئات
        public static int RevenueReceivable => 175; //ايرادات مستحقة التحصيل
        public static int OtherDebitAccounts => 177; //حسابات مدينة أخرى
        public static int AccountsReceivableHolding => 1731; //حسابات مدينة لدى الشركات القابضة
        public static int AccountsReceivableSubsidiaries => 1732; //* حسابات مدينة لدى الشركات التابعة / الشقيقة
        public static int ExpensesPaidAdvance => 176; //مصروفات مدفوعة مقدما
        public static int TradedInvestmentSecurities => 18; //استثمارات وأوراق مالية متداولة (اذون اخزانة)
        public static int BankDepositForTerm => 191; //ودائع بالبنوك لأجل أو اخطار سابق
        public static int WarrantyCover => 192; //غطاء ضمان
                                                //حقوق الملكية
        public static int PaidCapital => 21; //رأس المال المدفوع
        public static int CapitalReserve => 223; //احتياطى رأسمالى
        public static int OtherReserves => 224; //احتياطيات أخرى
        public static int ProfitLossStage => 23; //أرباح ( خسائر ) مرحلة
        public static int LongTermCommitments => 255; //التزامات طويلة الأجل (مقابل مشروعات تحت التنفيذ)
        public static int LongTermLiabilities => 256; //التزامات طويلة الأجل (مقابل أصول)
        public static int OtherCreditAccounts => 289; //حسابات دائنة اخرى
        public static int DisputedTaxCustom => 267; //مخصص ضرائب متنازع عليها
        public static int ProvisionForClaims => 268; //مخصص المطالبات والمنازعات
        public static int OtherAllowances => 269; //مخصصات أخرى 
        public static int AccountsPayable => 284; //حسابات دائنة لدى المصالح والهيئات
        public static int ExpenseReceivable => 286; //مصروفات مستحقة السداد
        public static int IncomesPaidAdvance => 287; //ايرادات محصلة مقدما
        public static int DeferredInstallmentSalesProfit => 288; //أرباح مبيعات تقسيط مؤجلة ( تخص أعوام لاحقة )
        public static int OtherAccountsPayable => 289; //حسابات دائنة أخرى
        public static int AccountsPayableHolding => 2831; //حسابات دائنة لدى الشركات القابضة
        public static int AccountsPayableSubsidiaries => 2832; //* حسابات دائنة لدى الشركات التابعة / الشقيقة

        //ارقام حسابات الحسابات الرئيسية
        public static int GeneralAssets => 1;//حساب الاصول 
        public static int GeneralPropertyRights => 2;//حساب حقوق الملكية والالتزامات 
        public static int GeneralExpenses => 3;//حساب المصروفات 
        public static int GeneralIncomes => 4;//حساب الايرادات 
        public static int GeneralFixedAssets => 11;//حساب الاصول الثابتة فقط 

        //الحسابات التى لا يتم عرضها بسبب استخدامها فى الاعدادات العامة
        public static int PurchasesForSale => 34;//حساب مشتريات بغرض البيع 
        public static int DepreciationAndConsumption => 332;//حساب الاهلاك والاستهلاك 
        public static int PurchasedMerchandiseSales => 412;//حساب مبيعات بضائع مشتراه 
        public static int FullProductionSales => 411;//حساب مبيعات انتاج تام 

        #endregion
    }

}