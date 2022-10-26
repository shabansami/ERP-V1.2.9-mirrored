namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_OrderGuid_from_ProductionOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductionOrders", "OrderGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionOrders", "OrderGuid", c => c.Guid());
        }
    }
}
