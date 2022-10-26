namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingInvoiceNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellInvoices", "InvoiceNumber", c => c.String());
            AddColumn("dbo.PurchaseBackInvoices", "InvoiceNumber", c => c.String());
            AddColumn("dbo.PurchaseInvoices", "InvoiceNumber", c => c.String());
            AddColumn("dbo.SellBackInvoices", "InvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellBackInvoices", "InvoiceNumber");
            DropColumn("dbo.PurchaseInvoices", "InvoiceNumber");
            DropColumn("dbo.PurchaseBackInvoices", "InvoiceNumber");
            DropColumn("dbo.SellInvoices", "InvoiceNumber");
        }
    }
}
