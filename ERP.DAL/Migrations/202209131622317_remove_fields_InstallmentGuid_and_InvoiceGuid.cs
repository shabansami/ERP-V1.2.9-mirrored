namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_fields_InstallmentGuid_and_InvoiceGuid : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.InventoryInvoices", "InvoiceGuid");
            DropColumn("dbo.Installments", "InstallmentGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Installments", "InstallmentGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.InventoryInvoices", "InvoiceGuid", c => c.Guid(nullable: false));
        }
    }
}
