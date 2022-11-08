namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_SellInvoicePayments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellInvoicePayments", "OperationDate", c => c.DateTime());
            AddColumn("dbo.SellInvoicePayments", "Notes", c => c.String());
            DropColumn("dbo.SellInvoicePayments", "PayGuid");
            DropColumn("dbo.SellInvoicePayments", "DueDate");
            DropColumn("dbo.SellInvoicePayments", "IsPayed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SellInvoicePayments", "IsPayed", c => c.Boolean(nullable: false));
            AddColumn("dbo.SellInvoicePayments", "DueDate", c => c.DateTime());
            AddColumn("dbo.SellInvoicePayments", "PayGuid", c => c.Guid(nullable: false));
            DropColumn("dbo.SellInvoicePayments", "Notes");
            DropColumn("dbo.SellInvoicePayments", "OperationDate");
        }
    }
}
