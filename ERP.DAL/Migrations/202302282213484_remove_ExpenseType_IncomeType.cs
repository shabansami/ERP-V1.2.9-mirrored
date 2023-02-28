namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_ExpenseType_IncomeType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExpenseTypes", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.ExpenseTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.ExpenseTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "AccountsTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.IncomeTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.IncomeTypes", "ModifiedBy", "dbo.Users");
            DropIndex("dbo.ExpenseTypes", new[] { "AccountsTreeId" });
            DropIndex("dbo.ExpenseTypes", new[] { "CreatedBy" });
            DropIndex("dbo.ExpenseTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.ExpenseTypes", new[] { "DeletedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "AccountsTreeId" });
            DropIndex("dbo.IncomeTypes", new[] { "CreatedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.IncomeTypes", new[] { "DeletedBy" });
            AddColumn("dbo.PersonCategories", "AccountTreeId", c => c.Guid());
            CreateIndex("dbo.PersonCategories", "AccountTreeId");
            AddForeignKey("dbo.PersonCategories", "AccountTreeId", "dbo.AccountsTrees", "Id");
            DropTable("dbo.ExpenseTypes");
            DropTable("dbo.IncomeTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.IncomeTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExpenseTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountsTreeId = c.Guid(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.PersonCategories", "AccountTreeId", "dbo.AccountsTrees");
            DropIndex("dbo.PersonCategories", new[] { "AccountTreeId" });
            DropColumn("dbo.PersonCategories", "AccountTreeId");
            CreateIndex("dbo.IncomeTypes", "DeletedBy");
            CreateIndex("dbo.IncomeTypes", "ModifiedBy");
            CreateIndex("dbo.IncomeTypes", "CreatedBy");
            CreateIndex("dbo.IncomeTypes", "AccountsTreeId");
            CreateIndex("dbo.ExpenseTypes", "DeletedBy");
            CreateIndex("dbo.ExpenseTypes", "ModifiedBy");
            CreateIndex("dbo.ExpenseTypes", "CreatedBy");
            CreateIndex("dbo.ExpenseTypes", "AccountsTreeId");
            AddForeignKey("dbo.IncomeTypes", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.IncomeTypes", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.IncomeTypes", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.IncomeTypes", "AccountsTreeId", "dbo.AccountsTrees", "Id");
            AddForeignKey("dbo.ExpenseTypes", "ModifiedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.ExpenseTypes", "DeletedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.ExpenseTypes", "CreatedBy", "dbo.Users", "Id");
            AddForeignKey("dbo.ExpenseTypes", "AccountsTreeId", "dbo.AccountsTrees", "Id");
        }
    }
}
