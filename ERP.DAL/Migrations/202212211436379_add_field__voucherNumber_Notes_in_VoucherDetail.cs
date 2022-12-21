namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field__voucherNumber_Notes_in_VoucherDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoucherDetails", "Notes", c => c.String());
            AddColumn("dbo.GeneralRecords", "GeneralRecordNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GeneralRecords", "GeneralRecordNumber");
            DropColumn("dbo.VoucherDetails", "Notes");
        }
    }
}
