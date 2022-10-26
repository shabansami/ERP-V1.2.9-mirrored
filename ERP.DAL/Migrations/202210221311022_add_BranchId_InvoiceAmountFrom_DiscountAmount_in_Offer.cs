namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_BranchId_InvoiceAmountFrom_DiscountAmount_in_Offer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Offers", "BranchId", c => c.Guid());
            AddColumn("dbo.Offers", "InvoiceAmountFrom", c => c.Double(nullable: false));
            AddColumn("dbo.Offers", "InvoiceAmountTo", c => c.Double(nullable: false));
            AddColumn("dbo.Offers", "DiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.Offers", "DiscountPercentage", c => c.Double(nullable: false));
            CreateIndex("dbo.Offers", "BranchId");
            AddForeignKey("dbo.Offers", "BranchId", "dbo.Branches", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Offers", "BranchId", "dbo.Branches");
            DropIndex("dbo.Offers", new[] { "BranchId" });
            DropColumn("dbo.Offers", "DiscountPercentage");
            DropColumn("dbo.Offers", "DiscountAmount");
            DropColumn("dbo.Offers", "InvoiceAmountTo");
            DropColumn("dbo.Offers", "InvoiceAmountFrom");
            DropColumn("dbo.Offers", "BranchId");
        }
    }
}
