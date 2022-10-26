namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields_Job_Email_Transfer_in_person : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "Job", c => c.String());
            AddColumn("dbo.People", "Email", c => c.String());
            AddColumn("dbo.People", "Transfer", c => c.String());
            DropColumn("dbo.AccountsTrees", "AccountNameEn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AccountsTrees", "AccountNameEn", c => c.String());
            DropColumn("dbo.People", "Transfer");
            DropColumn("dbo.People", "Email");
            DropColumn("dbo.People", "Job");
        }
    }
}
