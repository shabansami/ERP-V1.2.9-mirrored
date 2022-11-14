namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_TB_EmployeeBranches : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employees", "BranchId", "dbo.Branches");
            DropIndex("dbo.Employees", new[] { "BranchId" });
            CreateTable(
                "dbo.EmployeeBranches",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        BranchId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.BranchId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            DropColumn("dbo.Employees", "EmpGuid");
            DropColumn("dbo.Employees", "BranchId");
            DropColumn("dbo.Contracts", "ConGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contracts", "ConGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.Employees", "BranchId", c => c.Guid());
            AddColumn("dbo.Employees", "EmpGuid", c => c.Guid(nullable: false));
            DropForeignKey("dbo.EmployeeBranches", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeBranches", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeBranches", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeBranches", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeBranches", "BranchId", "dbo.Branches");
            DropIndex("dbo.EmployeeBranches", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeBranches", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeBranches", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeBranches", new[] { "BranchId" });
            DropIndex("dbo.EmployeeBranches", new[] { "EmployeeId" });
            DropTable("dbo.EmployeeBranches");
            CreateIndex("dbo.Employees", "BranchId");
            AddForeignKey("dbo.Employees", "BranchId", "dbo.Branches", "Id");
        }
    }
}
