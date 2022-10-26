namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeRowGuidFromPerson : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.People", "PersonGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "PersonGuid", c => c.Guid(nullable: false));
        }
    }
}
