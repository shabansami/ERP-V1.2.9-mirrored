namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_Is100Percentage_in_employeee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Is100Percentage", c => c.Boolean(nullable: false,defaultValue:true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Is100Percentage");
        }
    }
}
