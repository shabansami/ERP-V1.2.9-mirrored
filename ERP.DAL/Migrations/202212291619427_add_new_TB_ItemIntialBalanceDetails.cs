namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_TB_ItemIntialBalanceDetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemIntialBalances", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.ItemIntialBalances", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIntialBalances", "StoreId", "dbo.Stores");
            DropIndex("dbo.ItemIntialBalances", new[] { "AccountTreeId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "ItemId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "StoreId" });
            CreateTable(
                "dbo.ItemIntialBalanceDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ItemIntialBalanceId = c.Guid(),
                        AccountTreeId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        StoreId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.ItemIntialBalances", t => t.ItemIntialBalanceId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ItemIntialBalanceId)
                .Index(t => t.AccountTreeId)
                .Index(t => t.ItemId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.ItemIntialBalances", "BranchId", c => c.Guid());
            AddColumn("dbo.ItemIntialBalances", "Notes", c => c.String());
            AddColumn("dbo.ItemIntialBalances", "InvoiceNumber", c => c.String());
            CreateIndex("dbo.ItemIntialBalances", "BranchId");
            CreateIndex("dbo.GeneralRecords", "BranchId");
            AddForeignKey("dbo.ItemIntialBalances", "BranchId", "dbo.Branches", "Id");
            AddForeignKey("dbo.GeneralRecords", "BranchId", "dbo.Branches", "Id");
            DropColumn("dbo.ItemIntialBalances", "AccountTreeId");
            DropColumn("dbo.ItemIntialBalances", "ItemId");
            DropColumn("dbo.ItemIntialBalances", "Quantity");
            DropColumn("dbo.ItemIntialBalances", "Price");
            DropColumn("dbo.ItemIntialBalances", "Amount");
            DropColumn("dbo.ItemIntialBalances", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemIntialBalances", "StoreId", c => c.Guid());
            AddColumn("dbo.ItemIntialBalances", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.ItemIntialBalances", "Price", c => c.Double(nullable: false));
            AddColumn("dbo.ItemIntialBalances", "Quantity", c => c.Double(nullable: false));
            AddColumn("dbo.ItemIntialBalances", "ItemId", c => c.Guid());
            AddColumn("dbo.ItemIntialBalances", "AccountTreeId", c => c.Guid());
            DropForeignKey("dbo.GeneralRecords", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "ItemIntialBalanceId", "dbo.ItemIntialBalances");
            DropForeignKey("dbo.ItemIntialBalances", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemIntialBalanceDetails", "AccountTreeId", "dbo.AccountsTrees");
            DropIndex("dbo.GeneralRecords", new[] { "BranchId" });
            DropIndex("dbo.ItemIntialBalances", new[] { "BranchId" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "DeletedBy" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "CreatedBy" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "StoreId" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "ItemId" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "AccountTreeId" });
            DropIndex("dbo.ItemIntialBalanceDetails", new[] { "ItemIntialBalanceId" });
            DropColumn("dbo.ItemIntialBalances", "InvoiceNumber");
            DropColumn("dbo.ItemIntialBalances", "Notes");
            DropColumn("dbo.ItemIntialBalances", "BranchId");
            DropTable("dbo.ItemIntialBalanceDetails");
            CreateIndex("dbo.ItemIntialBalances", "StoreId");
            CreateIndex("dbo.ItemIntialBalances", "ItemId");
            CreateIndex("dbo.ItemIntialBalances", "AccountTreeId");
            AddForeignKey("dbo.ItemIntialBalances", "StoreId", "dbo.Stores", "Id");
            AddForeignKey("dbo.ItemIntialBalances", "ItemId", "dbo.Items", "Id");
            AddForeignKey("dbo.ItemIntialBalances", "AccountTreeId", "dbo.AccountsTrees", "Id");
        }
    }
}
