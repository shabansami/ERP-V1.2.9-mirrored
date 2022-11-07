namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_tb_ItemPrice_Supplier : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemPrices", "CustomerId", "dbo.People");
            AddColumn("dbo.ItemPrices", "SupplierId", c => c.Guid());
            AddColumn("dbo.ItemPrices", "Person_Id", c => c.Guid());
            CreateIndex("dbo.ItemPrices", "SupplierId");
            CreateIndex("dbo.ItemPrices", "Person_Id");
            AddForeignKey("dbo.ItemPrices", "CustomerId", "dbo.People", "Id");
            AddForeignKey("dbo.ItemPrices", "SupplierId", "dbo.People", "Id");
            AddForeignKey("dbo.ItemPrices", "Person_Id", "dbo.People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPrices", "Person_Id", "dbo.People");
            DropForeignKey("dbo.ItemPrices", "SupplierId", "dbo.People");
            DropForeignKey("dbo.ItemPrices", "CustomerId", "dbo.People");
            DropIndex("dbo.ItemPrices", new[] { "Person_Id" });
            DropIndex("dbo.ItemPrices", new[] { "SupplierId" });
            DropColumn("dbo.ItemPrices", "Person_Id");
            DropColumn("dbo.ItemPrices", "SupplierId");
            AddForeignKey("dbo.ItemPrices", "CustomerId", "dbo.People", "Id");
        }
    }
}
