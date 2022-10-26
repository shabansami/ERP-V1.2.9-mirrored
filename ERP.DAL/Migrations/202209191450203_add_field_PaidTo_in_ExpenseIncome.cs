namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_PaidTo_in_ExpenseIncome : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseIncomes", "PaidTo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseIncomes", "PaidTo");
        }
    }
}
