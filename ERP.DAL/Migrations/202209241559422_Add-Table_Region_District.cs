namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_Region_District : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AreaId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.AreaId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RegionId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.RegionId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.RegionId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Regions", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Regions", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Regions", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Districts", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Districts", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Districts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Districts", "RegionId", "dbo.Regions");
            DropForeignKey("dbo.Regions", "AreaId", "dbo.Areas");
            DropIndex("dbo.Districts", new[] { "DeletedBy" });
            DropIndex("dbo.Districts", new[] { "ModifiedBy" });
            DropIndex("dbo.Districts", new[] { "CreatedBy" });
            DropIndex("dbo.Districts", new[] { "RegionId" });
            DropIndex("dbo.Regions", new[] { "DeletedBy" });
            DropIndex("dbo.Regions", new[] { "ModifiedBy" });
            DropIndex("dbo.Regions", new[] { "CreatedBy" });
            DropIndex("dbo.Regions", new[] { "AreaId" });
            DropTable("dbo.Districts");
            DropTable("dbo.Regions");
        }
    }
}
