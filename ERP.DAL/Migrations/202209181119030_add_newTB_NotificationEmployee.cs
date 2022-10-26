namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_newTB_NotificationEmployee : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationEmployees",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        NotifictionId = c.Guid(),
                        EmployeeId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Notifications", t => t.NotifictionId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.NotifictionId)
                .Index(t => t.EmployeeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            AddColumn("dbo.Notifications", "StartAlertDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationEmployees", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.NotificationEmployees", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.NotificationEmployees", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.NotificationEmployees", "NotifictionId", "dbo.Notifications");
            DropForeignKey("dbo.NotificationEmployees", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.NotificationEmployees", new[] { "DeletedBy" });
            DropIndex("dbo.NotificationEmployees", new[] { "ModifiedBy" });
            DropIndex("dbo.NotificationEmployees", new[] { "CreatedBy" });
            DropIndex("dbo.NotificationEmployees", new[] { "EmployeeId" });
            DropIndex("dbo.NotificationEmployees", new[] { "NotifictionId" });
            DropColumn("dbo.Notifications", "StartAlertDate");
            DropTable("dbo.NotificationEmployees");
        }
    }
}
