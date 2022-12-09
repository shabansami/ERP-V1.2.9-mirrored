namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_EmployeeProduction_inProductionOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductionOrders", "EmployeeProductionId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "EmployeeOperationId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "Employee_Id", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "Employee_Id1", c => c.Guid());
            CreateIndex("dbo.ProductionOrders", "EmployeeProductionId");
            CreateIndex("dbo.ProductionOrders", "EmployeeOperationId");
            CreateIndex("dbo.ProductionOrders", "Employee_Id");
            CreateIndex("dbo.ProductionOrders", "Employee_Id1");
            AddForeignKey("dbo.ProductionOrders", "EmployeeOperationId", "dbo.Employees", "Id");
            AddForeignKey("dbo.ProductionOrders", "EmployeeProductionId", "dbo.Employees", "Id");
            AddForeignKey("dbo.ProductionOrders", "Employee_Id", "dbo.Employees", "Id");
            AddForeignKey("dbo.ProductionOrders", "Employee_Id1", "dbo.Employees", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductionOrders", "Employee_Id1", "dbo.Employees");
            DropForeignKey("dbo.ProductionOrders", "Employee_Id", "dbo.Employees");
            DropForeignKey("dbo.ProductionOrders", "EmployeeProductionId", "dbo.Employees");
            DropForeignKey("dbo.ProductionOrders", "EmployeeOperationId", "dbo.Employees");
            DropIndex("dbo.ProductionOrders", new[] { "Employee_Id1" });
            DropIndex("dbo.ProductionOrders", new[] { "Employee_Id" });
            DropIndex("dbo.ProductionOrders", new[] { "EmployeeOperationId" });
            DropIndex("dbo.ProductionOrders", new[] { "EmployeeProductionId" });
            DropColumn("dbo.ProductionOrders", "Employee_Id1");
            DropColumn("dbo.ProductionOrders", "Employee_Id");
            DropColumn("dbo.ProductionOrders", "EmployeeOperationId");
            DropColumn("dbo.ProductionOrders", "EmployeeProductionId");
        }
    }
}
