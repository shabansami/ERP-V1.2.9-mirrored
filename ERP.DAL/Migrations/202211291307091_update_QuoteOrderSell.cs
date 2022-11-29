namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_QuoteOrderSell : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderSellCases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.SellInvoices", "OrderSellId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "OrderSellId", c => c.Guid());
            AddColumn("dbo.QuoteOrderSellDetails", "OrderSellItemType", c => c.Int());
            AddColumn("dbo.QuoteOrderSells", "QuoteId", c => c.Guid());
            AddColumn("dbo.QuoteOrderSells", "OrderSellCaseId", c => c.Int());
            CreateIndex("dbo.SellInvoices", "OrderSellId");
            CreateIndex("dbo.ProductionOrders", "OrderSellId");
            CreateIndex("dbo.QuoteOrderSells", "QuoteId");
            CreateIndex("dbo.QuoteOrderSells", "OrderSellCaseId");
            AddForeignKey("dbo.QuoteOrderSells", "OrderSellCaseId", "dbo.OrderSellCases", "Id");
            AddForeignKey("dbo.QuoteOrderSells", "QuoteId", "dbo.QuoteOrderSells", "Id");
            AddForeignKey("dbo.ProductionOrders", "OrderSellId", "dbo.QuoteOrderSells", "Id");
            AddForeignKey("dbo.SellInvoices", "OrderSellId", "dbo.QuoteOrderSells", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellInvoices", "OrderSellId", "dbo.QuoteOrderSells");
            DropForeignKey("dbo.ProductionOrders", "OrderSellId", "dbo.QuoteOrderSells");
            DropForeignKey("dbo.QuoteOrderSells", "QuoteId", "dbo.QuoteOrderSells");
            DropForeignKey("dbo.QuoteOrderSells", "OrderSellCaseId", "dbo.OrderSellCases");
            DropForeignKey("dbo.OrderSellCases", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.OrderSellCases", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.OrderSellCases", "CreatedBy", "dbo.Users");
            DropIndex("dbo.OrderSellCases", new[] { "DeletedBy" });
            DropIndex("dbo.OrderSellCases", new[] { "ModifiedBy" });
            DropIndex("dbo.OrderSellCases", new[] { "CreatedBy" });
            DropIndex("dbo.QuoteOrderSells", new[] { "OrderSellCaseId" });
            DropIndex("dbo.QuoteOrderSells", new[] { "QuoteId" });
            DropIndex("dbo.ProductionOrders", new[] { "OrderSellId" });
            DropIndex("dbo.SellInvoices", new[] { "OrderSellId" });
            DropColumn("dbo.QuoteOrderSells", "OrderSellCaseId");
            DropColumn("dbo.QuoteOrderSells", "QuoteId");
            DropColumn("dbo.QuoteOrderSellDetails", "OrderSellItemType");
            DropColumn("dbo.ProductionOrders", "OrderSellId");
            DropColumn("dbo.SellInvoices", "OrderSellId");
            DropTable("dbo.OrderSellCases");
        }
    }
}
