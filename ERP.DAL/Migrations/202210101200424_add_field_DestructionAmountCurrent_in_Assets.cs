namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_DestructionAmountCurrent_in_Assets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "DestructionAmountCurrent", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "DestructionAmountCurrent");
        }
    }
}
