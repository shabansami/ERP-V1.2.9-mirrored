namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edit_StorePermissionItemsList_in_StorePermission : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StorePermissionItems", "StorePermissionItem_Id", "dbo.StorePermissionItems");
            DropIndex("dbo.StorePermissionItems", new[] { "StorePermissionItem_Id" });
            DropColumn("dbo.StorePermissionItems", "StorePermissionItem_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StorePermissionItems", "StorePermissionItem_Id", c => c.Guid());
            CreateIndex("dbo.StorePermissionItems", "StorePermissionItem_Id");
            AddForeignKey("dbo.StorePermissionItems", "StorePermissionItem_Id", "dbo.StorePermissionItems", "Id");
        }
    }
}
