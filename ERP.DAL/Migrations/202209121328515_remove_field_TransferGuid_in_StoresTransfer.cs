namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_field_TransferGuid_in_StoresTransfer : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StoresTransfers", "TransferGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoresTransfers", "TransferGuid", c => c.Guid(nullable: false));
        }
    }
}
