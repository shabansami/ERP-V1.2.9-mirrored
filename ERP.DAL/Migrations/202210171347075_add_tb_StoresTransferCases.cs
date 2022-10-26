namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tb_StoresTransferCases : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoresTransferCases",
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
            
            AddColumn("dbo.StoresTransfers", "IsApprovalStore", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoresTransfers", "IsRefusStore", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoresTransfers", "StoresTransferCaseId", c => c.Int());
            CreateIndex("dbo.StoresTransfers", "StoresTransferCaseId");
            AddForeignKey("dbo.StoresTransfers", "StoresTransferCaseId", "dbo.StoresTransferCases", "Id");
            DropColumn("dbo.StoresTransfers", "ForSaleMen");
            DropColumn("dbo.StoresTransfers", "SaleMenStatus");
            DropColumn("dbo.StoresTransfers", "SaleMenIsApproval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoresTransfers", "SaleMenIsApproval", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoresTransfers", "SaleMenStatus", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoresTransfers", "ForSaleMen", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.StoresTransfers", "StoresTransferCaseId", "dbo.StoresTransferCases");
            DropForeignKey("dbo.StoresTransferCases", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferCases", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StoresTransferCases", "CreatedBy", "dbo.Users");
            DropIndex("dbo.StoresTransferCases", new[] { "DeletedBy" });
            DropIndex("dbo.StoresTransferCases", new[] { "ModifiedBy" });
            DropIndex("dbo.StoresTransferCases", new[] { "CreatedBy" });
            DropIndex("dbo.StoresTransfers", new[] { "StoresTransferCaseId" });
            DropColumn("dbo.StoresTransfers", "StoresTransferCaseId");
            DropColumn("dbo.StoresTransfers", "IsRefusStore");
            DropColumn("dbo.StoresTransfers", "IsApprovalStore");
            DropTable("dbo.StoresTransferCases");
        }
    }
}
