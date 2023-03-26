namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_TB_PriceInvoice_add_QuoteOrderSell_Details : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PriceInvoicesDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.PriceInvoices", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.PriceInvoices", "CustomerId", "dbo.People");
            DropForeignKey("dbo.PriceInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "PriceInvoiceId", "dbo.PriceInvoices");
            DropForeignKey("dbo.PriceInvoicesDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.PriceInvoicesDetails", "ModifiedBy", "dbo.Users");
            DropIndex("dbo.PriceInvoicesDetails", new[] { "PriceInvoiceId" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "ItemId" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "CreatedBy" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.PriceInvoicesDetails", new[] { "DeletedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "CustomerId" });
            DropIndex("dbo.PriceInvoices", new[] { "BranchId" });
            DropIndex("dbo.PriceInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.PriceInvoices", new[] { "DeletedBy" });
            DropTable("dbo.PriceInvoicesDetails");
            DropTable("dbo.PriceInvoices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PriceInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        InvoiceGuid = c.Guid(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        SalesTax = c.Double(nullable: false),
                        ProfitTax = c.Double(nullable: false),
                        InvoiceDiscount = c.Double(nullable: false),
                        DiscountPercentage = c.Double(nullable: false),
                        TotalDiscount = c.Double(nullable: false),
                        Safy = c.Double(nullable: false),
                        Notes = c.String(),
                        IsFullReturned = c.Boolean(),
                        InvoiceNumber = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PriceInvoicesDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PriceInvoiceId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        ItemDiscount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PriceInvoices", "DeletedBy");
            CreateIndex("dbo.PriceInvoices", "ModifiedBy");
            CreateIndex("dbo.PriceInvoices", "CreatedBy");
            CreateIndex("dbo.PriceInvoices", "BranchId");
            CreateIndex("dbo.PriceInvoices", "CustomerId");
            CreateIndex("dbo.PriceInvoicesDetails", "DeletedBy");
            CreateIndex("dbo.PriceInvoicesDetails", "ModifiedBy");
            CreateIndex("dbo.PriceInvoicesDetails", "CreatedBy");
            CreateIndex("dbo.PriceInvoicesDetails", "ItemId");
            CreateIndex("dbo.PriceInvoicesDetails", "PriceInvoiceId");
            AddForeignKey("dbo.PriceInvoicesDetails", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoicesDetails", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoicesDetails", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoicesDetails", "PriceInvoiceId", "dbo.PriceInvoices", "Id");
            AddForeignKey("dbo.PriceInvoices", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoices", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoices", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.PriceInvoices", "CustomerId", "dbo.People", "Id");
            AddForeignKey("dbo.PriceInvoices", "BranchId", "dbo.Branches", "Id");
            AddForeignKey("dbo.PriceInvoicesDetails", "ItemId", "dbo.Items", "Id");
        }
    }
}
