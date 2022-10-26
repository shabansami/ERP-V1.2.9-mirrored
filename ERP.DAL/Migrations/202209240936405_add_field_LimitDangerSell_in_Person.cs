namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_LimitDangerSell_in_Person : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "LimitDangerSell", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "LimitDangerSell");
        }
    }
}
