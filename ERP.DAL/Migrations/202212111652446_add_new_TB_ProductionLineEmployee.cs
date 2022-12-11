namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_TB_ProductionLineEmployee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductionLines",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
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
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.ProductionLineEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductionLineId = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        IsProductionEmp = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.ProductionLines", t => t.ProductionLineId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.ProductionLineId)
                .Index(t => t.EmployeeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.ProductionOrders", "ProductionLineId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "ProductionOrderHours", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductionOrders", "ProductionLineId");
            AddForeignKey("dbo.ProductionOrders", "ProductionLineId", "dbo.ProductionLines", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductionOrders", "ProductionLineId", "dbo.ProductionLines");
            DropForeignKey("dbo.ProductionLines", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLines", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLines", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLineEmployees", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLineEmployees", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLineEmployees", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ProductionLineEmployees", "ProductionLineId", "dbo.ProductionLines");
            DropForeignKey("dbo.ProductionLineEmployees", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.ProductionLineEmployees", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionLineEmployees", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionLineEmployees", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionLineEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.ProductionLineEmployees", new[] { "ProductionLineId" });
            DropIndex("dbo.ProductionLines", new[] { "DeletedBy" });
            DropIndex("dbo.ProductionLines", new[] { "ModifiedBy" });
            DropIndex("dbo.ProductionLines", new[] { "CreatedBy" });
            DropIndex("dbo.ProductionOrders", new[] { "ProductionLineId" });
            DropColumn("dbo.ProductionOrders", "ProductionOrderHours");
            DropColumn("dbo.ProductionOrders", "ProductionLineId");
            DropTable("dbo.ProductionLineEmployees");
            DropTable("dbo.ProductionLines");
        }
    }
}
