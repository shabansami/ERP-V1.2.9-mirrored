namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_TB_ContractSchedulingProduction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductionOrders", "EmployeeOperationId", "dbo.Employees");
            DropForeignKey("dbo.ProductionOrders", "EmployeeProductionId", "dbo.Employees");
            DropForeignKey("dbo.ContractSchedulings", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ProductionOrders", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.ProductionOrders", "Employee_Id1", "dbo.Employees");
            DropIndex("dbo.ContractSchedulings", new[] { "ProductionOrderId" });
            DropIndex("dbo.ProductionOrders", new[] { "EmployeeProductionId" });
            DropIndex("dbo.ProductionOrders", new[] { "EmployeeOperationId" });
            DropIndex("dbo.ProductionOrders", new[] { "Employee_Id" });
            DropIndex("dbo.ProductionOrders", new[] { "Employee_Id1" });
            CreateTable(
                "dbo.ContractSchedulingProductions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ContractSchedulingId = c.Guid(nullable: false),
                        ProductionOrderId = c.Guid(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContractSchedulings", t => t.ContractSchedulingId, cascadeDelete: true)
                .ForeignKey("dbo.ProductionOrders", t => t.ProductionOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ContractSchedulingId)
                .Index(t => t.ProductionOrderId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.ProductionLineEmployees", "HourlyWage", c => c.Double(nullable: false));
            DropColumn("dbo.ContractSchedulings", "ProductionOrderId");
            DropColumn("dbo.ProductionOrders", "EmployeeProductionId");
            DropColumn("dbo.ProductionOrders", "EmployeeOperationId");
            DropColumn("dbo.ProductionOrders", "Employee_Id");
            DropColumn("dbo.ProductionOrders", "Employee_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductionOrders", "Employee_Id1", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "Employee_Id", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "EmployeeOperationId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "EmployeeProductionId", c => c.Guid());
            AddColumn("dbo.ContractSchedulings", "ProductionOrderId", c => c.Guid());
            DropForeignKey("dbo.ContractSchedulingProductions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingProductions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingProductions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ContractSchedulingProductions", "ProductionOrderId", "dbo.ProductionOrders");
            DropForeignKey("dbo.ContractSchedulingProductions", "ContractSchedulingId", "dbo.ContractSchedulings");
            DropIndex("dbo.ContractSchedulingProductions", new[] { "DeletedBy" });
            DropIndex("dbo.ContractSchedulingProductions", new[] { "ModifiedBy" });
            DropIndex("dbo.ContractSchedulingProductions", new[] { "CreatedBy" });
            DropIndex("dbo.ContractSchedulingProductions", new[] { "ProductionOrderId" });
            DropIndex("dbo.ContractSchedulingProductions", new[] { "ContractSchedulingId" });
            DropColumn("dbo.ProductionLineEmployees", "HourlyWage");
            DropTable("dbo.ContractSchedulingProductions");
            CreateIndex("dbo.ProductionOrders", "Employee_Id1");
            CreateIndex("dbo.ProductionOrders", "Employee_Id");
            CreateIndex("dbo.ProductionOrders", "EmployeeOperationId");
            CreateIndex("dbo.ProductionOrders", "EmployeeProductionId");
            CreateIndex("dbo.ContractSchedulings", "ProductionOrderId");
            AddForeignKey("dbo.ProductionOrders", "Employee_Id1", "dbo.Employees", "Id");
            AddForeignKey("dbo.ProductionOrders", "Employee_Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.ContractSchedulings", "ProductionOrderId", "dbo.ProductionOrders", "Id");
            AddForeignKey("dbo.ProductionOrders", "EmployeeProductionId", "dbo.Employees", "Id");
            AddForeignKey("dbo.ProductionOrders", "EmployeeOperationId", "dbo.Employees", "Id");
        }
    }
}
