namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOfferDiscountinOfferSellInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferSellInvoices", "OfferDiscount", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferSellInvoices", "OfferDiscount");
        }
    }
}
