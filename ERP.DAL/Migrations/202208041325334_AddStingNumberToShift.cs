namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStingNumberToShift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShiftsOfflines", "ShiftNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShiftsOfflines", "ShiftNumber");
        }
    }
}
