namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_stuctureDB_in_GeneralRecord_Voucher_masterDetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductionOrders", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.GeneralRecords", "AccountTreeFromId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecords", "AccountTreeToId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Vouchers", "AccountTreeFromId", "dbo.AccountsTrees");
            DropForeignKey("dbo.Vouchers", "AccountTreeToId", "dbo.AccountsTrees");
            DropIndex("dbo.ProductionOrders", new[] { "Store_Id" });
            DropIndex("dbo.Vouchers", new[] { "AccountTreeFromId" });
            DropIndex("dbo.Vouchers", new[] { "AccountTreeToId" });
            DropIndex("dbo.GeneralRecords", new[] { "AccountTreeFromId" });
            DropIndex("dbo.GeneralRecords", new[] { "AccountTreeToId" });
            CreateTable(
                "dbo.VoucherDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VoucherId = c.Guid(nullable: false),
                        AccountTreeFromId = c.Guid(),
                        AccountTreeToId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.Vouchers", t => t.VoucherId, cascadeDelete: true)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeFromId)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeToId)
                .Index(t => t.VoucherId)
                .Index(t => t.AccountTreeFromId)
                .Index(t => t.AccountTreeToId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.GeneralRecordDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GeneralRecordId = c.Guid(nullable: false),
                        AccountTreeId = c.Guid(),
                        Amount = c.Double(nullable: false),
                        IsDebit = c.Boolean(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeneralRecords", t => t.GeneralRecordId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .ForeignKey("dbo.AccountsTrees", t => t.AccountTreeId)
                .Index(t => t.GeneralRecordId)
                .Index(t => t.AccountTreeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            DropColumn("dbo.ProductionOrders", "Store_Id");
            DropColumn("dbo.Vouchers", "AccountTreeFromId");
            DropColumn("dbo.Vouchers", "AccountTreeToId");
            DropColumn("dbo.Vouchers", "Amount");
            DropColumn("dbo.GeneralRecords", "AccountTreeFromId");
            DropColumn("dbo.GeneralRecords", "AccountTreeToId");
            DropColumn("dbo.GeneralRecords", "Amount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GeneralRecords", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.GeneralRecords", "AccountTreeToId", c => c.Guid());
            AddColumn("dbo.GeneralRecords", "AccountTreeFromId", c => c.Guid());
            AddColumn("dbo.Vouchers", "Amount", c => c.Double(nullable: false));
            AddColumn("dbo.Vouchers", "AccountTreeToId", c => c.Guid());
            AddColumn("dbo.Vouchers", "AccountTreeFromId", c => c.Guid());
            AddColumn("dbo.ProductionOrders", "Store_Id", c => c.Guid());
            DropForeignKey("dbo.VoucherDetails", "AccountTreeToId", "dbo.AccountsTrees");
            DropForeignKey("dbo.VoucherDetails", "AccountTreeFromId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecordDetails", "AccountTreeId", "dbo.AccountsTrees");
            DropForeignKey("dbo.GeneralRecordDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralRecordDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralRecordDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.GeneralRecordDetails", "GeneralRecordId", "dbo.GeneralRecords");
            DropForeignKey("dbo.VoucherDetails", "VoucherId", "dbo.Vouchers");
            DropForeignKey("dbo.VoucherDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.VoucherDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.VoucherDetails", "CreatedBy", "dbo.Users");
            DropIndex("dbo.GeneralRecordDetails", new[] { "DeletedBy" });
            DropIndex("dbo.GeneralRecordDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.GeneralRecordDetails", new[] { "CreatedBy" });
            DropIndex("dbo.GeneralRecordDetails", new[] { "AccountTreeId" });
            DropIndex("dbo.GeneralRecordDetails", new[] { "GeneralRecordId" });
            DropIndex("dbo.VoucherDetails", new[] { "DeletedBy" });
            DropIndex("dbo.VoucherDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.VoucherDetails", new[] { "CreatedBy" });
            DropIndex("dbo.VoucherDetails", new[] { "AccountTreeToId" });
            DropIndex("dbo.VoucherDetails", new[] { "AccountTreeFromId" });
            DropIndex("dbo.VoucherDetails", new[] { "VoucherId" });
            DropTable("dbo.GeneralRecordDetails");
            DropTable("dbo.VoucherDetails");
            CreateIndex("dbo.GeneralRecords", "AccountTreeToId");
            CreateIndex("dbo.GeneralRecords", "AccountTreeFromId");
            CreateIndex("dbo.Vouchers", "AccountTreeToId");
            CreateIndex("dbo.Vouchers", "AccountTreeFromId");
            CreateIndex("dbo.ProductionOrders", "Store_Id");
            AddForeignKey("dbo.Vouchers", "AccountTreeToId", "dbo.AccountsTrees", "Id");
            AddForeignKey("dbo.Vouchers", "AccountTreeFromId", "dbo.AccountsTrees", "Id");
            AddForeignKey("dbo.GeneralRecords", "AccountTreeToId", "dbo.AccountsTrees", "Id");
            AddForeignKey("dbo.GeneralRecords", "AccountTreeFromId", "dbo.AccountsTrees", "Id");
            AddForeignKey("dbo.ProductionOrders", "Store_Id", "dbo.Stores", "Id");
        }
    }
}
