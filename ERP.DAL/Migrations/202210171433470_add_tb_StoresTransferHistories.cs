namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tb_StoresTransferHistories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoresTransferHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoresTransferCaseId = c.Int(),
                        StoresTransferId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StoresTransfers", t => t.StoresTransferId)
                .ForeignKey("dbo.StoresTransferCases", t => t.StoresTransferCaseId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoresTransferCaseId)
                .Index(t => t.StoresTransferId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoresTransferHistories", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferHistories", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferHistories", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferHistories", "StoresTransferCaseId", "dbo.StoresTransferCases");
            DropForeignKey("dbo.StoresTransferHistories", "StoresTransferId", "dbo.StoresTransfers");
            DropIndex("dbo.StoresTransferHistories", new[] { "DeletedBy" });
            DropIndex("dbo.StoresTransferHistories", new[] { "ModifiedBy" });
            DropIndex("dbo.StoresTransferHistories", new[] { "CreatedBy" });
            DropIndex("dbo.StoresTransferHistories", new[] { "StoresTransferId" });
            DropIndex("dbo.StoresTransferHistories", new[] { "StoresTransferCaseId" });
            DropTable("dbo.StoresTransferHistories");
        }
    }
}
