namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_IsReceive_in_StorePermission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StorePermissions", "IsReceive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StorePermissions", "IsReceive");
        }
    }
}
