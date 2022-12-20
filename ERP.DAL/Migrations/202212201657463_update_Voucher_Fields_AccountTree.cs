namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Voucher_Fields_AccountTree : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VoucherDetails", "AccountTreeToId", "dbo.AccountsTrees");
            DropIndex("dbo.VoucherDetails", new[] { "AccountTreeToId" });
            RenameColumn(table: "dbo.VoucherDetails", name: "AccountTreeFromId", newName: "AccountTreeId");
            RenameIndex(table: "dbo.VoucherDetails", name: "IX_AccountTreeFromId", newName: "IX_AccountTreeId");
            AddColumn("dbo.Vouchers", "AccountTreeId", c => c.Guid());
            CreateIndex("dbo.Vouchers", "AccountTreeId");
            AddForeignKey("dbo.Vouchers", "AccountTreeId", "dbo.AccountsTrees", "Id");
            DropColumn("dbo.VoucherDetails", "AccountTreeToId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VoucherDetails", "AccountTreeToId", c => c.Guid());
            DropForeignKey("dbo.Vouchers", "AccountTreeId", "dbo.AccountsTrees");
            DropIndex("dbo.Vouchers", new[] { "AccountTreeId" });
            DropColumn("dbo.Vouchers", "AccountTreeId");
            RenameIndex(table: "dbo.VoucherDetails", name: "IX_AccountTreeId", newName: "IX_AccountTreeFromId");
            RenameColumn(table: "dbo.VoucherDetails", name: "AccountTreeId", newName: "AccountTreeFromId");
            CreateIndex("dbo.VoucherDetails", "AccountTreeToId");
            AddForeignKey("dbo.VoucherDetails", "AccountTreeToId", "dbo.AccountsTrees", "Id");
        }
    }
}
