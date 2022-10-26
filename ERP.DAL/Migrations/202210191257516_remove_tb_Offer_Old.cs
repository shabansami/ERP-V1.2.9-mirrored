namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_tb_Offer_Old : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Offers", "UnitId", "dbo.Units");
            DropForeignKey("dbo.Offers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "ModifiedBy", "dbo.Users");
            DropIndex("dbo.Offers", new[] { "UnitId" });
            DropIndex("dbo.Offers", new[] { "CreatedBy" });
            DropIndex("dbo.Offers", new[] { "ModifiedBy" });
            DropIndex("dbo.Offers", new[] { "DeletedBy" });
            DropTable("dbo.Offers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        UnitId = c.Guid(),
                        QuantityOffer = c.Double(nullable: false),
                        IsDiscountVal = c.Boolean(nullable: false),
                        DiscountOffer = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Offers", "DeletedBy");
            CreateIndex("dbo.Offers", "ModifiedBy");
            CreateIndex("dbo.Offers", "CreatedBy");
            CreateIndex("dbo.Offers", "UnitId");
            AddForeignKey("dbo.Offers", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.Offers", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.Offers", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.Offers", "UnitId", "dbo.Units", "Id");
        }
    }
}
