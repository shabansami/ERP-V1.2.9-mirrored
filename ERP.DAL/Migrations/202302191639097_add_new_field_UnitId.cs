namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_field_UnitId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuoteOrderSellDetails", "UnitId", c => c.Guid());
            AddColumn("dbo.QuoteOrderSellDetails", "QuantityUnit", c => c.Double(nullable: false));
            AddColumn("dbo.SellInvoicesDetails", "UnitId", c => c.Guid());
            AddColumn("dbo.SellInvoicesDetails", "QuantityUnit", c => c.Double(nullable: false));
            CreateIndex("dbo.QuoteOrderSellDetails", "UnitId");
            CreateIndex("dbo.SellInvoicesDetails", "UnitId");
            AddForeignKey("dbo.QuoteOrderSellDetails", "UnitId", "dbo.Units", "Id");
            AddForeignKey("dbo.SellInvoicesDetails", "UnitId", "dbo.Units", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellInvoicesDetails", "UnitId", "dbo.Units");
            DropForeignKey("dbo.QuoteOrderSellDetails", "UnitId", "dbo.Units");
            DropIndex("dbo.SellInvoicesDetails", new[] { "UnitId" });
            DropIndex("dbo.QuoteOrderSellDetails", new[] { "UnitId" });
            DropColumn("dbo.SellInvoicesDetails", "QuantityUnit");
            DropColumn("dbo.SellInvoicesDetails", "UnitId");
            DropColumn("dbo.QuoteOrderSellDetails", "QuantityUnit");
            DropColumn("dbo.QuoteOrderSellDetails", "UnitId");
        }
    }
}
