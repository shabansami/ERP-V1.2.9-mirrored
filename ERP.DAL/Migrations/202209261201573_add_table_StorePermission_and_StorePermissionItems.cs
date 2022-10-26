namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_table_StorePermission_and_StorePermissionItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StorePermissionItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StorePermissionId = c.Guid(),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.StorePermissions", t => t.StorePermissionId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StorePermissionId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.StorePermissions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StoreId = c.Guid(),
                        SafeId = c.Guid(),
                        PersonId = c.Guid(),
                        AccountTreeId = c.Guid(),
                        PermissionDate = c.DateTime(),
                        IsApproval = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.Safes", t => t.SafeId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.StoreId)
                .Index(t => t.SafeId)
                .Index(t => t.PersonId)
                .Index(t => t.AccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StorePermissionItems", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissionItems", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissionItems", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissionItems", "StorePermissionId", "dbo.StorePermissions");
            DropForeignKey("dbo.StorePermissions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.StorePermissions", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.StorePermissions", "SafeId", "dbo.Safes");
            DropForeignKey("dbo.StorePermissions", "PersonId", "dbo.People");
            DropForeignKey("dbo.StorePermissions", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.StorePermissionItems", "ItemId", "dbo.Items");
            DropIndex("dbo.StorePermissions", new[] { "DeletedBy" });
            DropIndex("dbo.StorePermissions", new[] { "ModifiedBy" });
            DropIndex("dbo.StorePermissions", new[] { "CreatedBy" });
            DropIndex("dbo.StorePermissions", new[] { "AccountTreeId" });
            DropIndex("dbo.StorePermissions", new[] { "PersonId" });
            DropIndex("dbo.StorePermissions", new[] { "SafeId" });
            DropIndex("dbo.StorePermissions", new[] { "StoreId" });
            DropIndex("dbo.StorePermissionItems", new[] { "DeletedBy" });
            DropIndex("dbo.StorePermissionItems", new[] { "ModifiedBy" });
            DropIndex("dbo.StorePermissionItems", new[] { "CreatedBy" });
            DropIndex("dbo.StorePermissionItems", new[] { "ItemId" });
            DropIndex("dbo.StorePermissionItems", new[] { "StorePermissionId" });
            DropTable("dbo.StorePermissions");
            DropTable("dbo.StorePermissionItems");
        }
    }
}
