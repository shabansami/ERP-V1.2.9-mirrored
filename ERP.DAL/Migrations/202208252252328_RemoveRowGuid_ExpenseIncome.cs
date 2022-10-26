namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRowGuid_ExpenseIncome : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExpenseIncomes", "RowGuid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpenseIncomes", "RowGuid", c => c.Guid(nullable: false));
        }
    }
}
