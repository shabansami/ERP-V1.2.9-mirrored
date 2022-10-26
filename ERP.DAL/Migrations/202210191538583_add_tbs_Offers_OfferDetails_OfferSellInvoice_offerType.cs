namespace ERP.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tbs_Offers_OfferDetails_OfferSellInvoice_offerType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OfferDetails",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OfferId = c.Guid(nullable: false),
                        ItemId = c.Guid(),
                        Quantity = c.Double(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Offers", t => t.OfferId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.OfferId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OfferTypeId = c.Int(nullable: false),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AmountBefore = c.Double(nullable: false),
                        AmountAfter = c.Double(nullable: false),
                        Limit = c.Int(nullable: false),
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
                .ForeignKey("dbo.OfferTypes", t => t.OfferTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.OfferTypeId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.OfferSellInvoices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OfferId = c.Guid(),
                        SellInvoiceId = c.Guid(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Guid(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.OfferId)
                .ForeignKey("dbo.SellInvoices", t => t.SellInvoiceId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.DeletedBy)
                .ForeignKey("dbo.Users", t => t.ModifiedBy)
                .Index(t => t.OfferId)
                .Index(t => t.SellInvoiceId)
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
            CreateTable(
                "dbo.OfferTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
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
                .Index(t => t.CreatedBy)
                .Index(t => t.ModifiedBy)
                .Index(t => t.DeletedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfferDetails", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.OfferDetails", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.OfferDetails", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.OfferDetails", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.Offers", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Offers", "OfferTypeId", "dbo.OfferTypes");
            DropForeignKey("dbo.OfferTypes", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.OfferTypes", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.OfferTypes", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.OfferSellInvoices", "ModifiedBy", "dbo.Users");
            DropForeignKey("dbo.OfferSellInvoices", "DeletedBy", "dbo.Users");
            DropForeignKey("dbo.OfferSellInvoices", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.OfferSellInvoices", "SellInvoiceId", "dbo.SellInvoices");
            DropForeignKey("dbo.OfferSellInvoices", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.OfferDetails", "ItemId", "dbo.Items");
            DropIndex("dbo.OfferTypes", new[] { "DeletedBy" });
            DropIndex("dbo.OfferTypes", new[] { "ModifiedBy" });
            DropIndex("dbo.OfferTypes", new[] { "CreatedBy" });
            DropIndex("dbo.OfferSellInvoices", new[] { "DeletedBy" });
            DropIndex("dbo.OfferSellInvoices", new[] { "ModifiedBy" });
            DropIndex("dbo.OfferSellInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.OfferSellInvoices", new[] { "SellInvoiceId" });
            DropIndex("dbo.OfferSellInvoices", new[] { "OfferId" });
            DropIndex("dbo.Offers", new[] { "DeletedBy" });
            DropIndex("dbo.Offers", new[] { "ModifiedBy" });
            DropIndex("dbo.Offers", new[] { "CreatedBy" });
            DropIndex("dbo.Offers", new[] { "OfferTypeId" });
            DropIndex("dbo.OfferDetails", new[] { "DeletedBy" });
            DropIndex("dbo.OfferDetails", new[] { "ModifiedBy" });
            DropIndex("dbo.OfferDetails", new[] { "CreatedBy" });
            DropIndex("dbo.OfferDetails", new[] { "ItemId" });
            DropIndex("dbo.OfferDetails", new[] { "OfferId" });
            DropTable("dbo.OfferTypes");
            DropTable("dbo.OfferSellInvoices");
            DropTable("dbo.Offers");
            DropTable("dbo.OfferDetails");
        }
    }
}
