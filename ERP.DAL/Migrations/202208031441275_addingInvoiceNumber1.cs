namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingInvoiceNumber1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryInvoices", "InvoiceNumber", c => c.String());
            AddColumn("dbo.PriceInvoices", "InvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PriceInvoices", "InvoiceNumber");
            DropColumn("dbo.InventoryInvoices", "InvoiceNumber");
        }
    }
}
