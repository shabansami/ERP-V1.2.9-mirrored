namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOffersDiscountinSellInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellInvoices", "OffersDiscount", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellInvoices", "OffersDiscount");
        }
    }
}
