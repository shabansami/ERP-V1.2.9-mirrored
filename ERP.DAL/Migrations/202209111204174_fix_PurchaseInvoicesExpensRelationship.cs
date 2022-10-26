namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_PurchaseInvoicesExpensRelationship : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "PurchaseInvoiceId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "ExpenseTypeAccountTreeId", newName: "PurchaseInvoiceId");
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "__mig_tmp__0", newName: "ExpenseTypeAccountTreeId");
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "IX_ExpenseTypeAccountTreeId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "IX_PurchaseInvoiceId", newName: "IX_ExpenseTypeAccountTreeId");
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "__mig_tmp__0", newName: "IX_PurchaseInvoiceId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "IX_PurchaseInvoiceId", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "IX_ExpenseTypeAccountTreeId", newName: "IX_PurchaseInvoiceId");
            RenameIndex(table: "dbo.PurchaseInvoicesExpens", name: "__mig_tmp__0", newName: "IX_ExpenseTypeAccountTreeId");
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "ExpenseTypeAccountTreeId", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "PurchaseInvoiceId", newName: "ExpenseTypeAccountTreeId");
            RenameColumn(table: "dbo.PurchaseInvoicesExpens", name: "__mig_tmp__0", newName: "PurchaseInvoiceId");
        }
    }
}
