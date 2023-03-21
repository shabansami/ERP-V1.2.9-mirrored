namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewFieldAccountTreeCreditInProductionOrderExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClosingPeriods",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
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
            
            AddColumn("dbo.ProductionOrderExpens", "AccountTreeCreditId", c => c.Guid());
            AddColumn("dbo.Vouchers", "IsSafe", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ProductionOrderExpens", "AccountTreeCreditId");
            AddForeignKey("dbo.ProductionOrderExpens", "AccountTreeCreditId", "dbo.AccountsTrees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClosingPeriods", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ClosingPeriods", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ClosingPeriods", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionOrderExpens", "AccountTreeCreditId", "dbo.AccountsTrees");
            DropIndex("dbo.ClosingPeriods", new[] { "DeletedBy" });
            DropIndex("dbo.ClosingPeriods", new[] { "ModifiedBy" });
            DropIndex("dbo.ClosingPeriods", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrderExpens", new[] { "AccountTreeCreditId" });
            DropColumn("dbo.Vouchers", "IsSafe");
            DropColumn("dbo.ProductionOrderExpens", "AccountTreeCreditId");
            DropTable("dbo.ClosingPeriods");
        }
    }
}
