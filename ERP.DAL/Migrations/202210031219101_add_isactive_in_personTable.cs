namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_isactive_in_personTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "IsActive", c => c.Boolean(nullable: false,defaultValue:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "IsActive");
        }
    }
}
