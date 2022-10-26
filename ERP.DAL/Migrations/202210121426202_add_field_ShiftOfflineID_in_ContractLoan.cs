namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_ShiftOfflineID_in_ContractLoan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractLoans", "ShiftOfflineID", c => c.Guid());
            CreateIndex("dbo.ContractLoans", "ShiftOfflineID");
            AddForeignKey("dbo.ContractLoans", "ShiftOfflineID", "dbo.ShiftsOfflines", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractLoans", "ShiftOfflineID", "dbo.ShiftsOfflines");
            DropIndex("dbo.ContractLoans", new[] { "ShiftOfflineID" });
            DropColumn("dbo.ContractLoans", "ShiftOfflineID");
        }
    }
}
