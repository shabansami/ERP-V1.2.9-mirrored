namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_QuantityReal_in_QuoteOrderSell : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuoteOrderSells", "QuoteOrderSellType", c => c.Int(nullable: false));
            DropColumn("dbo.QuoteOrderSellDetails", "QuantityReal");
            DropColumn("dbo.QuoteOrderSells", "QuoteOrderSellId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuoteOrderSells", "QuoteOrderSellId", c => c.Int(nullable: false));
            AddColumn("dbo.QuoteOrderSellDetails", "QuantityReal", c => c.Double(nullable: false));
            DropColumn("dbo.QuoteOrderSells", "QuoteOrderSellType");
        }
    }
}
