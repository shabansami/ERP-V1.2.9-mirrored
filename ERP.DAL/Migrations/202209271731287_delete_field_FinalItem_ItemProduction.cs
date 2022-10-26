namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_field_FinalItem_ItemProduction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemProductions", "ItemFinalId", "dbo.Items");
            DropIndex("dbo.ItemProductions", new[] { "ItemFinalId" });
            DropColumn("dbo.ItemProductions", "ItemFinalId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemProductions", "ItemFinalId", c => c.Guid());
            CreateIndex("dbo.ItemProductions", "ItemFinalId");
            AddForeignKey("dbo.ItemProductions", "ItemFinalId", "dbo.Items", "Id");
        }
    }
}
