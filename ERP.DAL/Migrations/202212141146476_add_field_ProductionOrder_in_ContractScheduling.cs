namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_ProductionOrder_in_ContractScheduling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractSchedulings", "ProductionOrderId", c => c.Guid());
            CreateIndex("dbo.ContractSchedulings", "ProductionOrderId");
            AddForeignKey("dbo.ContractSchedulings", "ProductionOrderId", "dbo.ProductionOrders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractSchedulings", "ProductionOrderId", "dbo.ProductionOrders");
            DropIndex("dbo.ContractSchedulings", new[] { "ProductionOrderId" });
            DropColumn("dbo.ContractSchedulings", "ProductionOrderId");
        }
    }
}
