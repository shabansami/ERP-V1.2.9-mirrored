namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_IsApproval_in_PersonIntialBalance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonIntialBalances", "IsApproval", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonIntialBalances", "IsApproval");
        }
    }
}
