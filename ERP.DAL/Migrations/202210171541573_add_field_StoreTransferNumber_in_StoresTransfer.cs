﻿namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_StoreTransferNumber_in_StoresTransfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoresTransfers", "StoreTransferNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoresTransfers", "StoreTransferNumber");
        }
    }
}
