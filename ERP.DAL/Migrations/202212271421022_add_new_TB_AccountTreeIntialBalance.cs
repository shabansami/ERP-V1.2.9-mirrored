namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_TB_AccountTreeIntialBalance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountTreeIntialBalances",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BranchId = c.Guid(),
                        AccountTreeId = c.Guid(),
                        IsDebit = c.Boolean(nullable: false),
                        IsApproval = c.Boolean(nullable: false),
                        Amount = c.Double(nullable: false),
                        OperationDate = c.DateTime(),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.Branches", t => t.BranchId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.BranchId)
                .Index(t => t.AccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountTreeIntialBalances", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.AccountTreeIntialBalances", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.AccountTreeIntialBalances", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.AccountTreeIntialBalances", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.AccountTreeIntialBalances", "AccountTreeId", "dbo.AccountsTrees");
            DropIndex("dbo.AccountTreeIntialBalances", new[] { "DeletedBy" });
            DropIndex("dbo.AccountTreeIntialBalances", new[] { "ModifiedBy" });
            DropIndex("dbo.AccountTreeIntialBalances", new[] { "CreatedBy" });
            DropIndex("dbo.AccountTreeIntialBalances", new[] { "AccountTreeId" });
            DropIndex("dbo.AccountTreeIntialBalances", new[] { "BranchId" });
            DropTable("dbo.AccountTreeIntialBalances");
        }
    }
}
