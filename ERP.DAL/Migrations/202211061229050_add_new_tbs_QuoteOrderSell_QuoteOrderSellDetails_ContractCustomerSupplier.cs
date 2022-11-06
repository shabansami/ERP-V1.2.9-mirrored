namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_tbs_QuoteOrderSell_QuoteOrderSellDetails_ContractCustomerSupplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractCustomerSuppliers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PersonTypeId = c.Int(nullable: false),
                        CustomerId = c.Guid(),
                        SupplierId = c.Guid(),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        DayCount = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                        Person_Id = c.Guid(),
                        Person_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.PersonTypes", t => t.PersonTypeId, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.SupplierId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .ForeignKey("dbo.People", t => t.Person_Id1)
                .Index(t => t.PersonTypeId)
                .Index(t => t.CustomerId)
                .Index(t => t.SupplierId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy)
                .Index(t => t.Person_Id)
                .Index(t => t.Person_Id1);
            
            CreateTable(
                "dbo.QuoteOrderSellDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QuoteOrderSellId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        QuantityReal = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.QuoteOrderSells", t => t.QuoteOrderSellId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.QuoteOrderSellId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.QuoteOrderSells",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QuoteOrderSellId = c.Int(nullable: false),
                        CustomerId = c.Guid(),
                        BranchId = c.Guid(),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalQuantity = c.Double(nullable: false),
                        TotalValue = c.Double(nullable: false),
                        Notes = c.String(),
                        InvoiceNumber = c.String(),
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
                .ForeignKey("dbo.People", t => t.CustomerId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CustomerId)
                .Index(t => t.BranchId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.Items", "Thickness", c => c.String());
            AddColumn("dbo.Items", "ItemSize", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuoteOrderSellDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSellDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSellDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSellDetails", "QuoteOrderSellId", "dbo.QuoteOrderSells");
            DropForeignKey("dbo.QuoteOrderSells", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSells", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSells", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.QuoteOrderSells", "CustomerId", "dbo.People");
            DropForeignKey("dbo.QuoteOrderSells", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.QuoteOrderSellDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ContractCustomerSuppliers", "Person_Id1", "dbo.People");
            DropForeignKey("dbo.ContractCustomerSuppliers", "Person_Id", "dbo.People");
            DropForeignKey("dbo.ContractCustomerSuppliers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractCustomerSuppliers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractCustomerSuppliers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractCustomerSuppliers", "SupplierId", "dbo.People");
            DropForeignKey("dbo.ContractCustomerSuppliers", "PersonTypeId", "dbo.PersonTypes");
            DropForeignKey("dbo.ContractCustomerSuppliers", "CustomerId", "dbo.People");
            DropIndex("dbo.QuoteOrderSells", new[] { "DeletedBy" });
            DropIndex("dbo.QuoteOrderSells", new[] { "ModifiedBy" });
            DropIndex("dbo.QuoteOrderSells", new[] { "CreatedBy" });
            DropIndex("dbo.QuoteOrderSells", new[] { "BranchId" });
            DropIndex("dbo.QuoteOrderSells", new[] { "CustomerId" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "DeletedBy" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "CreatedBy" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "ItemId" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "QuoteOrderSellId" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "Person_Id1" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "Person_Id" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "DeletedBy" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "CreatedBy" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "SupplierId" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "CustomerId" });
            DropIndex("dbo.ContractCustomerSuppliers", new[] { "PersonTypeId" });
            DropColumn("dbo.Items", "ItemSize");
            DropColumn("dbo.Items", "Thickness");
            DropTable("dbo.QuoteOrderSells");
            DropTable("dbo.QuoteOrderSellDetails");
            DropTable("dbo.ContractCustomerSuppliers");
        }
    }
}
