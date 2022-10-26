namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields_emploees_details : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "PassportNumber", c => c.String());
            AddColumn("dbo.Employees", "PassportExpirationDate", c => c.DateTime());
            AddColumn("dbo.Employees", "PassportIssuer", c => c.String());
            AddColumn("dbo.Employees", "PassportJob", c => c.String());
            AddColumn("dbo.Employees", "ResidenceNumber", c => c.String());
            AddColumn("dbo.Employees", "ResidenceExpirationDate", c => c.DateTime());
            AddColumn("dbo.Employees", "ResidenceIssuer", c => c.String());
            AddColumn("dbo.Employees", "ResidenceJob", c => c.String());
            AddColumn("dbo.Employees", "MedicalInsurancePolicyNumber", c => c.Int());
            AddColumn("dbo.Employees", "MedicalInsuranceCategory", c => c.String());
            AddColumn("dbo.Employees", "MedicalInsuranceExpirationDate", c => c.DateTime());
            AddColumn("dbo.Employees", "MedicalInsuranceReleaseDate", c => c.DateTime());
            AddColumn("dbo.Employees", "MedicalInsuranceCompany", c => c.String());
            AddColumn("dbo.Employees", "SocialSecurityNumber", c => c.String());
            AddColumn("dbo.Employees", "SocialSecurityCurrentBalance", c => c.Double(nullable: false));
            DropColumn("dbo.Employees", "NumberOfInsurance");
            DropColumn("dbo.People", "PassportNumber");
            DropColumn("dbo.People", "PassportExpirationDate");
            DropColumn("dbo.People", "PassportIssuer");
            DropColumn("dbo.People", "PassportJob");
            DropColumn("dbo.People", "ResidenceNumber");
            DropColumn("dbo.People", "ResidenceExpirationDate");
            DropColumn("dbo.People", "ResidenceIssuer");
            DropColumn("dbo.People", "ResidenceJob");
            DropColumn("dbo.People", "MedicalInsurancePolicyNumber");
            DropColumn("dbo.People", "MedicalInsuranceCategory");
            DropColumn("dbo.People", "MedicalInsuranceExpirationDate");
            DropColumn("dbo.People", "MedicalInsuranceReleaseDate");
            DropColumn("dbo.People", "MedicalInsuranceCompany");
            DropColumn("dbo.People", "SocialSecurityNumber");
            DropColumn("dbo.People", "SocialSecurityCurrentBalance");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "SocialSecurityCurrentBalance", c => c.Double(nullable: false));
            AddColumn("dbo.People", "SocialSecurityNumber", c => c.String());
            AddColumn("dbo.People", "MedicalInsuranceCompany", c => c.String());
            AddColumn("dbo.People", "MedicalInsuranceReleaseDate", c => c.DateTime());
            AddColumn("dbo.People", "MedicalInsuranceExpirationDate", c => c.DateTime());
            AddColumn("dbo.People", "MedicalInsuranceCategory", c => c.String());
            AddColumn("dbo.People", "MedicalInsurancePolicyNumber", c => c.Int());
            AddColumn("dbo.People", "ResidenceJob", c => c.String());
            AddColumn("dbo.People", "ResidenceIssuer", c => c.String());
            AddColumn("dbo.People", "ResidenceExpirationDate", c => c.DateTime());
            AddColumn("dbo.People", "ResidenceNumber", c => c.String());
            AddColumn("dbo.People", "PassportJob", c => c.String());
            AddColumn("dbo.People", "PassportIssuer", c => c.String());
            AddColumn("dbo.People", "PassportExpirationDate", c => c.DateTime());
            AddColumn("dbo.People", "PassportNumber", c => c.String());
            AddColumn("dbo.Employees", "NumberOfInsurance", c => c.String());
            DropColumn("dbo.Employees", "SocialSecurityCurrentBalance");
            DropColumn("dbo.Employees", "SocialSecurityNumber");
            DropColumn("dbo.Employees", "MedicalInsuranceCompany");
            DropColumn("dbo.Employees", "MedicalInsuranceReleaseDate");
            DropColumn("dbo.Employees", "MedicalInsuranceExpirationDate");
            DropColumn("dbo.Employees", "MedicalInsuranceCategory");
            DropColumn("dbo.Employees", "MedicalInsurancePolicyNumber");
            DropColumn("dbo.Employees", "ResidenceJob");
            DropColumn("dbo.Employees", "ResidenceIssuer");
            DropColumn("dbo.Employees", "ResidenceExpirationDate");
            DropColumn("dbo.Employees", "ResidenceNumber");
            DropColumn("dbo.Employees", "PassportJob");
            DropColumn("dbo.Employees", "PassportIssuer");
            DropColumn("dbo.Employees", "PassportExpirationDate");
            DropColumn("dbo.Employees", "PassportNumber");
        }
    }
}
