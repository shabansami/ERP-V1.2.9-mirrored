namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_OrderNumber_in_ProductionOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionOrders", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductionOrders", "OrderNumber");
        }
    }
}
