namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_OperationNumber_in_ExpenseIncome : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseIncomes", "OperationNumber", c => c.String());
            AddColumn("dbo.Installments", "OperationNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Installments", "OperationNumber");
            DropColumn("dbo.ExpenseIncomes", "OperationNumber");
        }
    }
}
