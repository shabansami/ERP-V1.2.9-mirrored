namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUptatesTB_fromOldSolutions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PointOfSales", "SafeID", "dbo.Safes");
            DropForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores");
            DropForeignKey("dbo.PointOfSales", "BankAccountID", "dbo.BankAccounts");
            DropIndex("dbo.PointOfSales", new[] { "SafeID" });
            DropIndex("dbo.PointOfSales", new[] { "BankAccountID" });
            DropIndex("dbo.PointOfSales", new[] { "StoreID" });
            RenameColumn(table: "dbo.People", name: "AccountTreeCustomerEmpId", newName: "AccountTreeEmpCustodyId");
            RenameIndex(table: "dbo.People", name: "IX_AccountTreeCustomerEmpId", newName: "IX_AccountTreeEmpCustodyId");
            CreateTable(
                "dbo.DamageInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        StoreId = c.Guid(),
                        TotalCostQuantityAmount = c.Double(nullable: false),
                        ItemCostCalculateId = c.Int(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemCostCalculations", t => t.ItemCostCalculateId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoreId)
                .Index(t => t.ItemCostCalculateId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.DamageInvoiceDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DamageInvoiceId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        CostQuantity = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DamageInvoices", t => t.DamageInvoiceId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.DamageInvoiceId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ItemCustomSellPrices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BranchId = c.Guid(),
                        GroupBasicId = c.Guid(),
                        ItemId = c.Guid(),
                        ProfitPercentage = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Groups", t => t.GroupBasicId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.BranchId)
                .Index(t => t.GroupBasicId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.PersonIntialBalances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BranchId = c.Guid(),
                        PersonId = c.Guid(),
                        IsCustomer = c.Boolean(nullable: false),
                        IsDebit = c.Boolean(nullable: false),
                        Amount = c.Double(nullable: false),
                        OperationDate = c.DateTime(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.BranchId)
                .Index(t => t.PersonId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.StoresTransferDetails", "QuantityReal", c => c.Double(nullable: false));
            AddColumn("dbo.StoresTransfers", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("dbo.PointOfSales", "BrunchId", c => c.Guid());
            AddColumn("dbo.PointOfSales", "DefaultBankWalletId", c => c.Guid());
            AddColumn("dbo.PointOfSales", "DefaultBankCardId", c => c.Guid());
            AddColumn("dbo.Installments", "IntialCustomerId", c => c.Guid());
            AddColumn("dbo.InstallmentSchedules", "SafeId", c => c.Guid());
            AddColumn("dbo.GeneralDailies", "TransactionShared", c => c.Guid());
            AlterColumn("dbo.Items", "ItemCode", c => c.String());
            AlterColumn("dbo.PointOfSales", "SafeID", c => c.Guid());
            AlterColumn("dbo.PointOfSales", "BankAccountID", c => c.Guid());
            AlterColumn("dbo.PointOfSales", "StoreID", c => c.Guid());
            CreateIndex("dbo.PointOfSales", "BrunchId");
            CreateIndex("dbo.PointOfSales", "SafeID");
            CreateIndex("dbo.PointOfSales", "BankAccountID");
            CreateIndex("dbo.PointOfSales", "StoreID");
            CreateIndex("dbo.PointOfSales", "DefaultBankWalletId");
            CreateIndex("dbo.PointOfSales", "DefaultBankCardId");
            CreateIndex("dbo.Installments", "IntialCustomerId");
            CreateIndex("dbo.InstallmentSchedules", "SafeId");
            AddForeignKey("dbo.PointOfSales", "BrunchId", "dbo.Branches", "Id");
            AddForeignKey("dbo.Installments", "IntialCustomerId", "dbo.PersonIntialBalances", "Id");
            AddForeignKey("dbo.InstallmentSchedules", "SafeId", "dbo.Safes", "Id");
            AddForeignKey("dbo.PointOfSales", "DefaultBankCardId", "dbo.BankAccounts", "Id");
            AddForeignKey("dbo.PointOfSales", "DefaultBankWalletId", "dbo.BankAccounts", "Id");
            AddForeignKey("dbo.PointOfSales", "SafeID", "dbo.Safes", "Id");
            AddForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores", "Id");
            AddForeignKey("dbo.PointOfSales", "BankAccountID", "dbo.BankAccounts", "Id");
            DropColumn("dbo.SellInvoices", "InvoiceGuid");
            DropColumn("dbo.PurchaseBackInvoices", "InvoiceGuid");
            DropColumn("dbo.PurchaseInvoices", "InvoiceGuid");
            DropColumn("dbo.SellBackInvoices", "InvoiceGuid");
            DropColumn("dbo.InstallmentSchedules", "ScheduleGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InstallmentSchedules", "ScheduleGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.SellBackInvoices", "InvoiceGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.PurchaseInvoices", "InvoiceGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.PurchaseBackInvoices", "InvoiceGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.SellInvoices", "InvoiceGuid", c => c.Guid(nullable: false));
            DropForeignKey("dbo.PointOfSales", "BankAccountID", "dbo.BankAccounts");
            DropForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores");
            DropForeignKey("dbo.PointOfSales", "SafeID", "dbo.Safes");
            DropForeignKey("dbo.PointOfSales", "DefaultBankWalletId", "dbo.BankAccounts");
            DropForeignKey("dbo.PointOfSales", "DefaultBankCardId", "dbo.BankAccounts");
            DropForeignKey("dbo.InstallmentSchedules", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.Installments", "IntialCustomerId", "dbo.PersonIntialBalances");
            DropForeignKey("dbo.PersonIntialBalances", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PersonIntialBalances", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PersonIntialBalances", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PersonIntialBalances", "PersonId", "dbo.People");
            DropForeignKey("dbo.PersonIntialBalances", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ItemCustomSellPrices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemCustomSellPrices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemCustomSellPrices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemCustomSellPrices", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemCustomSellPrices", "GroupBasicId", "dbo.Groups");
            DropForeignKey("dbo.ItemCustomSellPrices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PointOfSales", "BrunchId", "dbo.Branches");
            DropForeignKey("dbo.DamageInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoices", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.DamageInvoices", "ItemCostCalculateId", "dbo.ItemCostCalculations");
            DropForeignKey("dbo.DamageInvoiceDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoiceDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoiceDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.DamageInvoiceDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.DamageInvoiceDetails", "DamageInvoiceId", "dbo.DamageInvoices");
            DropIndex("dbo.InstallmentSchedules", new[] { "SafeId" });
            DropIndex("dbo.PersonIntialBalances", new[] { "DeletedBy" });
            DropIndex("dbo.PersonIntialBalances", new[] { "ModifiedBy" });
            DropIndex("dbo.PersonIntialBalances", new[] { "CreatedBy" });
            DropIndex("dbo.PersonIntialBalances", new[] { "PersonId" });
            DropIndex("dbo.PersonIntialBalances", new[] { "BranchId" });
            DropIndex("dbo.Installments", new[] { "IntialCustomerId" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "DeletedBy" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "CreatedBy" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "ItemId" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "GroupBasicId" });
            DropIndex("dbo.ItemCustomSellPrices", new[] { "BranchId" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultBankCardId" });
            DropIndex("dbo.PointOfSales", new[] { "DefaultBankWalletId" });
            DropIndex("dbo.PointOfSales", new[] { "StoreID" });
            DropIndex("dbo.PointOfSales", new[] { "BankAccountID" });
            DropIndex("dbo.PointOfSales", new[] { "SafeID" });
            DropIndex("dbo.PointOfSales", new[] { "BrunchId" });
            DropIndex("dbo.DamageInvoiceDetails", new[] { "DeletedBy" });
            DropIndex("dbo.DamageInvoiceDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.DamageInvoiceDetails", new[] { "CreatedBy" });
            DropIndex("dbo.DamageInvoiceDetails", new[] { "ItemId" });
            DropIndex("dbo.DamageInvoiceDetails", new[] { "DamageInvoiceId" });
            DropIndex("dbo.DamageInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.DamageInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.DamageInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.DamageInvoices", new[] { "ItemCostCalculateId" });
            DropIndex("dbo.DamageInvoices", new[] { "StoreId" });
            AlterColumn("dbo.PointOfSales", "StoreID", c => c.Guid(nullable: false));
            AlterColumn("dbo.PointOfSales", "BankAccountID", c => c.Guid(nullable: false));
            AlterColumn("dbo.PointOfSales", "SafeID", c => c.Guid(nullable: false));
            AlterColumn("dbo.Items", "ItemCode", c => c.Long());
            DropColumn("dbo.GeneralDailies", "TransactionShared");
            DropColumn("dbo.InstallmentSchedules", "SafeId");
            DropColumn("dbo.Installments", "IntialCustomerId");
            DropColumn("dbo.PointOfSales", "DefaultBankCardId");
            DropColumn("dbo.PointOfSales", "DefaultBankWalletId");
            DropColumn("dbo.PointOfSales", "BrunchId");
            DropColumn("dbo.StoresTransfers", "IsFinalApproval");
            DropColumn("dbo.StoresTransferDetails", "QuantityReal");
            DropTable("dbo.PersonIntialBalances");
            DropTable("dbo.ItemCustomSellPrices");
            DropTable("dbo.DamageInvoiceDetails");
            DropTable("dbo.DamageInvoices");
            RenameIndex(table: "dbo.People", name: "IX_AccountTreeEmpCustodyId", newName: "IX_AccountTreeCustomerEmpId");
            RenameColumn(table: "dbo.People", name: "AccountTreeEmpCustodyId", newName: "AccountTreeCustomerEmpId");
            CreateIndex("dbo.PointOfSales", "StoreID");
            CreateIndex("dbo.PointOfSales", "BankAccountID");
            CreateIndex("dbo.PointOfSales", "SafeID");
            AddForeignKey("dbo.PointOfSales", "BankAccountID", "dbo.BankAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PointOfSales", "SafeID", "dbo.Safes", "Id", cascadeDelete: true);
        }
    }
}
