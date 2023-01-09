namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addField_SalesTaxPercentage_in_All_Invoices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellInvoices", "SalesTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.SellInvoices", "ProfitTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.PurchaseBackInvoices", "SalesTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.PurchaseBackInvoices", "ProfitTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "SalesTax", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "SalesTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "ProfitTax", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "ProfitTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "InvoiceDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "DiscountPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "TotalDiscount", c => c.Double(nullable: false));
            AddColumn("dbo.QuoteOrderSells", "Safy", c => c.Double(nullable: false));
            AddColumn("dbo.PurchaseInvoices", "SalesTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.PurchaseInvoices", "ProfitTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.SellBackInvoices", "SalesTaxPercentage", c => c.Double(nullable: false));
            AddColumn("dbo.SellBackInvoices", "ProfitTaxPercentage", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellBackInvoices", "ProfitTaxPercentage");
            DropColumn("dbo.SellBackInvoices", "SalesTaxPercentage");
            DropColumn("dbo.PurchaseInvoices", "ProfitTaxPercentage");
            DropColumn("dbo.PurchaseInvoices", "SalesTaxPercentage");
            DropColumn("dbo.QuoteOrderSells", "Safy");
            DropColumn("dbo.QuoteOrderSells", "TotalDiscount");
            DropColumn("dbo.QuoteOrderSells", "DiscountPercentage");
            DropColumn("dbo.QuoteOrderSells", "InvoiceDiscount");
            DropColumn("dbo.QuoteOrderSells", "ProfitTaxPercentage");
            DropColumn("dbo.QuoteOrderSells", "ProfitTax");
            DropColumn("dbo.QuoteOrderSells", "SalesTaxPercentage");
            DropColumn("dbo.QuoteOrderSells", "SalesTax");
            DropColumn("dbo.PurchaseBackInvoices", "ProfitTaxPercentage");
            DropColumn("dbo.PurchaseBackInvoices", "SalesTaxPercentage");
            DropColumn("dbo.SellInvoices", "ProfitTaxPercentage");
            DropColumn("dbo.SellInvoices", "SalesTaxPercentage");
        }
    }
}
