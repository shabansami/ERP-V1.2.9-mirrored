namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tbl_ProductionType_Update_ProductionDetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductionOrders", "FinalItemId", "dbo.Items");
            DropForeignKey("dbo.ProductionOrders", "ProductionStoreId", "dbo.Stores");
            DropIndex("dbo.ProductionOrders", new[] { "FinalItemId" });
            CreateTable(
                "dbo.ProductionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.ProductionOrders", "ProductionUnderStoreId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "Store_Id", c => c.Guid());
            AddColumn("dbo.ProductionOrderDetails", "ItemProductionId", c => c.Guid());
            AddColumn("dbo.ProductionOrderDetails", "ProductionTypeId", c => c.Int());
            AddColumn("dbo.ItemProductionDetails", "ProductionTypeId", c => c.Int());
            CreateIndex("dbo.ProductionOrders", "ProductionUnderStoreId");
            CreateIndex("dbo.ProductionOrders", "Store_Id");
            CreateIndex("dbo.ProductionOrderDetails", "ItemProductionId");
            CreateIndex("dbo.ProductionOrderDetails", "ProductionTypeId");
            CreateIndex("dbo.ItemProductionDetails", "ProductionTypeId");
            AddForeignKey("dbo.ItemProductionDetails", "ProductionTypeId", "dbo.ProductionTypes", "Id");
            AddForeignKey("dbo.ProductionOrderDetails", "ItemProductionId", "dbo.ItemProductions", "Id");
            AddForeignKey("dbo.ProductionOrderDetails", "ProductionTypeId", "dbo.ProductionTypes", "Id");
            AddForeignKey("dbo.ProductionOrders", "ProductionUnderStoreId", "dbo.Stores", "Id");
            AddForeignKey("dbo.ProductionOrders", "Store_Id", "dbo.Stores", "Id");
            DropColumn("dbo.ProductionOrders", "FinalItemId");
            DropColumn("dbo.ProductionOrders", "OrderQuantity");
            DropColumn("dbo.ProductionOrders", "FinalItemCost");
            DropColumn("dbo.ProductionOrders", "MaterialItemCost");
            DropColumn("dbo.ProductionOrders", "DamagesCost");
            DropColumn("dbo.ProductionOrders", "TotalExpenseCost");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionOrders", "TotalExpenseCost", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionOrders", "DamagesCost", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionOrders", "MaterialItemCost", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionOrders", "FinalItemCost", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionOrders", "OrderQuantity", c => c.Double(nullable: false));
            AddColumn("dbo.ProductionOrders", "FinalItemId", c => c.Guid());
            DropForeignKey("dbo.ProductionOrders", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.ProductionOrders", "ProductionUnderStoreId", "dbo.Stores");
            DropForeignKey("dbo.ProductionOrderDetails", "ProductionTypeId", "dbo.ProductionTypes");
            DropForeignKey("dbo.ProductionOrderDetails", "ItemProductionId", "dbo.ItemProductions");
            DropForeignKey("dbo.ItemProductionDetails", "ProductionTypeId", "dbo.ProductionTypes");
            DropForeignKey("dbo.ProductionTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionTypes", "CreatedBy", "dbo.Users");
            DropIndex("dbo.ProductionTypes", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ItemProductionDetails", new[] { "ProductionTypeId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ProductionTypeId" });
            DropIndex("dbo.ProductionOrderDetails", new[] { "ItemProductionId" });
            DropIndex("dbo.ProductionOrders", new[] { "Store_Id" });
            DropIndex("dbo.ProductionOrders", new[] { "ProductionUnderStoreId" });
            DropColumn("dbo.ItemProductionDetails", "ProductionTypeId");
            DropColumn("dbo.ProductionOrderDetails", "ProductionTypeId");
            DropColumn("dbo.ProductionOrderDetails", "ItemProductionId");
            DropColumn("dbo.ProductionOrders", "Store_Id");
            DropColumn("dbo.ProductionOrders", "ProductionUnderStoreId");
            DropTable("dbo.ProductionTypes");
            CreateIndex("dbo.ProductionOrders", "FinalItemId");
            AddForeignKey("dbo.ProductionOrders", "ProductionStoreId", "dbo.Stores", "Id");
            AddForeignKey("dbo.ProductionOrders", "FinalItemId", "dbo.Items", "Id");
        }
    }
}
