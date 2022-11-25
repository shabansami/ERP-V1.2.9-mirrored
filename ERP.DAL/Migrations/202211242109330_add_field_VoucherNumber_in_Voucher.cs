namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_VoucherNumber_in_Voucher : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vouchers", "VoucherNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vouchers", "VoucherNumber");
        }
    }
}
