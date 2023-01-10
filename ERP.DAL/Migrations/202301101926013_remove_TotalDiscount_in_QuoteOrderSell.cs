namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_TotalDiscount_in_QuoteOrderSell : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.QuoteOrderSells", "TotalDiscount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuoteOrderSells", "TotalDiscount", c => c.Double(nullable: false));
        }
    }
}
