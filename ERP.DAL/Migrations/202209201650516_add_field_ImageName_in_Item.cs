namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_ImageName_in_Item : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ImageName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "ImageName");
        }
    }
}
