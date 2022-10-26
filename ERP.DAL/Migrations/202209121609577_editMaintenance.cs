namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editMaintenance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Maintenances", "InvoiceNumber", c => c.String());
            DropColumn("dbo.Maintenances", "InvoiceGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Maintenances", "InvoiceGuid", c => c.Guid(nullable: false));
            DropColumn("dbo.Maintenances", "InvoiceNumber");
        }
    }
}
