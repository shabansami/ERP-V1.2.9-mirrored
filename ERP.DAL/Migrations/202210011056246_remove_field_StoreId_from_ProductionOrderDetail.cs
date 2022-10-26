namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_StoreId_from_ProductionOrderDetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductionOrderDetails", "StoreId", "dbo.Stores");
            DropIndex("dbo.ProductionOrderDetails", new[] { "StoreId" });
            DropColumn("dbo.ProductionOrderDetails", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionOrderDetails", "StoreId", c => c.Guid());
            CreateIndex("dbo.ProductionOrderDetails", "StoreId");
            AddForeignKey("dbo.ProductionOrderDetails", "StoreId", "dbo.Stores", "Id");
        }
    }
}
