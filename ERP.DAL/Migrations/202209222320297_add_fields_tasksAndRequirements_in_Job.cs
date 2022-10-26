namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields_tasksAndRequirements_in_Job : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "Tasks", c => c.String());
            AddColumn("dbo.Jobs", "Requirements", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "Requirements");
            DropColumn("dbo.Jobs", "Tasks");
        }
    }
}
