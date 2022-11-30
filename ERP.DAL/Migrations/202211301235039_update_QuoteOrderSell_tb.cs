namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_QuoteOrderSell_tb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuoteOrderSells", "StoreId", c => c.Guid());
            AddColumn("dbo.QuoteOrderSellDetails", "ItemProductionId", c => c.Guid());
            CreateIndex("dbo.QuoteOrderSells", "StoreId");
            CreateIndex("dbo.QuoteOrderSellDetails", "ItemProductionId");
            AddForeignKey("dbo.QuoteOrderSellDetails", "ItemProductionId", "dbo.ItemProductions", "Id");
            AddForeignKey("dbo.QuoteOrderSells", "StoreId", "dbo.Stores", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuoteOrderSells", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.QuoteOrderSellDetails", "ItemProductionId", "dbo.ItemProductions");
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "ItemProductionId" });
            DropIndex("dbo.QuoteOrderSells", new[] { "StoreId" });
            DropColumn("dbo.QuoteOrderSellDetails", "ItemProductionId");
            DropColumn("dbo.QuoteOrderSells", "StoreId");
        }
    }
}
