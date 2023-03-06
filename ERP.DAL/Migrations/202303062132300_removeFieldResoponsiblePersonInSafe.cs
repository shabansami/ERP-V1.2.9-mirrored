namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeFieldResoponsiblePersonInSafe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Safes", "ResoponsiblePersonId", "dbo.People");
            DropIndex("dbo.Safes", new[] { "ResoponsiblePersonId" });
            DropColumn("dbo.Safes", "ResoponsiblePersonId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Safes", "ResoponsiblePersonId", c => c.Guid());
            CreateIndex("dbo.Safes", "ResoponsiblePersonId");
            AddForeignKey("dbo.Safes", "ResoponsiblePersonId", "dbo.People", "Id");
        }
    }
}
