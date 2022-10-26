namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Field_DistrictId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "DistrictId", c => c.Guid());
            CreateIndex("dbo.People", "DistrictId");
            AddForeignKey("dbo.People", "DistrictId", "dbo.Districts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "DistrictId", "dbo.Districts");
            DropIndex("dbo.People", new[] { "DistrictId" });
            DropColumn("dbo.People", "DistrictId");
        }
    }
}
