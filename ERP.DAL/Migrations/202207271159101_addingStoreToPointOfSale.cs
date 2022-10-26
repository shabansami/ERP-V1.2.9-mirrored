namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingStoreToPointOfSale : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PointOfSales", "BrunchId", "dbo.Branches");
            DropIndex("dbo.PointOfSales", new[] { "BrunchId" });
            AddColumn("dbo.PointOfSales", "StoreID", c => c.Guid(nullable: false));
            CreateIndex("dbo.PointOfSales", "StoreID");
            AddForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores", "Id", cascadeDelete: true);
            DropColumn("dbo.PointOfSales", "BrunchId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PointOfSales", "BrunchId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.PointOfSales", "StoreID", "dbo.Stores");
            DropIndex("dbo.PointOfSales", new[] { "StoreID" });
            DropColumn("dbo.PointOfSales", "StoreID");
            CreateIndex("dbo.PointOfSales", "BrunchId");
            AddForeignKey("dbo.PointOfSales", "BrunchId", "dbo.Branches", "Id", cascadeDelete: true);
        }
    }
}
