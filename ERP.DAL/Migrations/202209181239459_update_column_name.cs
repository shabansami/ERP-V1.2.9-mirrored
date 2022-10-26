namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_column_name : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.NotificationEmployees", name: "NotifictionId", newName: "NotificationId");
            RenameIndex(table: "dbo.NotificationEmployees", name: "IX_NotifictionId", newName: "IX_NotificationId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.NotificationEmployees", name: "IX_NotificationId", newName: "IX_NotifictionId");
            RenameColumn(table: "dbo.NotificationEmployees", name: "NotificationId", newName: "NotifictionId");
        }
    }
}
