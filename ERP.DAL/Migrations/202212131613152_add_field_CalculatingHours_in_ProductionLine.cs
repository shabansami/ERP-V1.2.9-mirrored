namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_CalculatingHours_in_ProductionLine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionLineEmployees", "CalculatingHours", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductionLineEmployees", "CalculatingHours");
        }
    }
}
