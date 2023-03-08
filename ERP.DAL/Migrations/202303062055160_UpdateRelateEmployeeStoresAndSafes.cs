namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRelateEmployeeStoresAndSafes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stores", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Employees", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Stores", "Employee_Id", "dbo.Employees");
            DropIndex("dbo.Employees", new[] { "StoreId" });
            DropIndex("dbo.Employees", new[] { "Store_Id" });
            DropIndex("dbo.Stores", new[] { "EmployeeId" });
            DropIndex("dbo.Stores", new[] { "Employee_Id" });
            CreateTable(
                "dbo.EmployeeStores",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        StoreId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.StoreId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.EmployeeSafes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        SafeId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId, cascadeDelete: true)
                .ForeignKey("dbo.Safes", t => t.SafeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.EmployeeId)
                .Index(t => t.SafeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            DropColumn("dbo.Employees", "StoreId");
            DropColumn("dbo.Employees", "Store_Id");
            DropColumn("dbo.Stores", "EmployeeId");
            DropColumn("dbo.Stores", "Employee_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "Employee_Id", c => c.Guid());
            AddColumn("dbo.Stores", "EmployeeId", c => c.Guid());
            AddColumn("dbo.Employees", "Store_Id", c => c.Guid());
            AddColumn("dbo.Employees", "StoreId", c => c.Guid());
            DropForeignKey("dbo.EmployeeSafes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeSafes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeSafes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeSafes", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.EmployeeSafes", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.EmployeeStores", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeStores", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeStores", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.EmployeeStores", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.EmployeeStores", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.EmployeeSafes", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeSafes", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeSafes", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeSafes", new[] { "SafeId" });
            DropIndex("dbo.EmployeeSafes", new[] { "EmployeeId" });
            DropIndex("dbo.EmployeeStores", new[] { "DeletedBy" });
            DropIndex("dbo.EmployeeStores", new[] { "ModifiedBy" });
            DropIndex("dbo.EmployeeStores", new[] { "CreatedBy" });
            DropIndex("dbo.EmployeeStores", new[] { "StoreId" });
            DropIndex("dbo.EmployeeStores", new[] { "EmployeeId" });
            DropTable("dbo.EmployeeSafes");
            DropTable("dbo.EmployeeStores");
            CreateIndex("dbo.Stores", "Employee_Id");
            CreateIndex("dbo.Stores", "EmployeeId");
            CreateIndex("dbo.Employees", "Store_Id");
            CreateIndex("dbo.Employees", "StoreId");
            AddForeignKey("dbo.Stores", "Employee_Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.Employees", "StoreId", "dbo.Stores", "Id");
            AddForeignKey("dbo.Employees", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Stores", "EmployeeId", "dbo.Employees", "Id");
        }
    }
}
