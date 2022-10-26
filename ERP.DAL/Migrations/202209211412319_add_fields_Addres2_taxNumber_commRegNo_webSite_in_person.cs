namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields_Addres2_taxNumber_commRegNo_webSite_in_person : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "Address2", c => c.String());
            AddColumn("dbo.People", "TaxNumber", c => c.String());
            AddColumn("dbo.People", "CommercialRegistrationNo", c => c.String());
            AddColumn("dbo.People", "WebSite", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "WebSite");
            DropColumn("dbo.People", "CommercialRegistrationNo");
            DropColumn("dbo.People", "TaxNumber");
            DropColumn("dbo.People", "Address2");
        }
    }
}
