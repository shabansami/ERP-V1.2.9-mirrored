namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_ComplexProductionIndex_in_ProductionOrderDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionOrderDetails", "ComplexProductionIndex", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductionOrderDetails", "ComplexProductionIndex");
        }
    }
}
