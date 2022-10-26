namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_AccountNameEn_in_AccountsTree : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountsTrees", "AccountNameEn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountsTrees", "AccountNameEn");
        }
    }
}
