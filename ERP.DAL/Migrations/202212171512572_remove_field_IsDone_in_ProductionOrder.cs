namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_IsDone_in_ProductionOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductionOrders", "IsDone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionOrders", "IsDone", c => c.Boolean(nullable: false));
        }
    }
}
