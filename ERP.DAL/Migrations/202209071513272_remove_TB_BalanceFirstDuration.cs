namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_TB_BalanceFirstDuration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BalanceFirstDurations", "ItemId", "dbo.Items");
            DropForeignKey("dbo.BalanceFirstDurations", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.BalanceFirstDurations", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.BalanceFirstDurations", "ModifiedBy", "dbo.Users");
            DropIndex("dbo.BalanceFirstDurations", new[] { "StoreId" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "ItemId" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "CreatedBy" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "ModifiedBy" });
            DropIndex("dbo.BalanceFirstDurations", new[] { "DeletedBy" });
            DropTable("dbo.BalanceFirstDurations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.BalanceFirstDurations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(),
                        Price = c.Double(),
                        Amount = c.Double(),
                        Notes = c.String(),
                        IsApproval = c.Boolean(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.BalanceFirstDurations", "DeletedBy");
            CreateIndex("dbo.BalanceFirstDurations", "ModifiedBy");
            CreateIndex("dbo.BalanceFirstDurations", "CreatedBy");
            CreateIndex("dbo.BalanceFirstDurations", "ItemId");
            CreateIndex("dbo.BalanceFirstDurations", "StoreId");
            AddForeignKey("dbo.BalanceFirstDurations", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.BalanceFirstDurations", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.BalanceFirstDurations", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.BalanceFirstDurations", "StoreId", "dbo.Stores", "Id");
            AddForeignKey("dbo.BalanceFirstDurations", "ItemId", "dbo.Items", "Id");
        }
    }
}
