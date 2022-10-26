begin transaction
/****** Object:  Table [dbo].[PriceInvoices]    Script Date: 4/4/2022 11:28:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceInvoices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceGuid] [uniqueidentifier] NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[CustomerId] [int] NULL,
	[BranchId] [int] NULL,
	[TotalQuantity] [float] NOT NULL,
	[TotalValue] [float] NOT NULL,
	[SalesTax] [float] NOT NULL,
	[ProfitTax] [float] NOT NULL,
	[InvoiceDiscount] [float] NOT NULL,
	[DiscountPercentage] [float] NOT NULL,
	[TotalDiscount] [float] NOT NULL,
	[Safy] [float] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[IsFullReturned] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedOn] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_PriceInvoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriceInvoicesDetails]    Script Date: 4/4/2022 11:28:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceInvoicesDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PriceInvoiceId] [int] NULL,
	[ItemId] [int] NULL,
	[Quantity] [float] NOT NULL,
	[Price] [float] NOT NULL,
	[Amount] [float] NOT NULL,
	[ItemDiscount] [float] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedOn] [datetime] NULL,
	[DeletedBy] [int] NULL,
	[DeletedOn] [datetime] NULL,
 CONSTRAINT [PK_PriceInvoicesDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_GuidInvoice]  DEFAULT (newid()) FOR [InvoiceGuid]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_InvoiceDate]  DEFAULT (getdate()) FOR [InvoiceDate]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_TotalQuantity]  DEFAULT ((0)) FOR [TotalQuantity]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_TotalValue]  DEFAULT ((0)) FOR [TotalValue]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_SalesTax]  DEFAULT ((0)) FOR [SalesTax]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_ProfitTax]  DEFAULT ((0)) FOR [ProfitTax]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_DiscountCashPercentage]  DEFAULT ((0)) FOR [InvoiceDiscount]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_DiscountPercentage]  DEFAULT ((0)) FOR [DiscountPercentage]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_TotalDiscount]  DEFAULT ((0)) FOR [TotalDiscount]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_Safy]  DEFAULT ((0)) FOR [Safy]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_ModifiedOn]  DEFAULT (getdate()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_DeletedOn]  DEFAULT (getdate()) FOR [DeletedOn]
GO
ALTER TABLE [dbo].[PriceInvoices] ADD  CONSTRAINT [DF_PriceInvoices_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_Price]  DEFAULT ((0)) FOR [Price]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_Value]  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_ModifiedOn]  DEFAULT (getdate()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] ADD  CONSTRAINT [DF_PriceInvoicesDetails_DeletedOn]  DEFAULT (getdate()) FOR [DeletedOn]
GO
ALTER TABLE [dbo].[PriceInvoices]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoices_Persons] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Persons] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoices] CHECK CONSTRAINT [FK_PriceInvoices_Persons]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoicesDetails_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] CHECK CONSTRAINT [FK_PriceInvoicesDetails_Items]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoicesDetails_PriceInvoices] FOREIGN KEY([PriceInvoiceId])
REFERENCES [dbo].[PriceInvoices] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] CHECK CONSTRAINT [FK_PriceInvoicesDetails_PriceInvoices]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoicesDetails_Users] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] CHECK CONSTRAINT [FK_PriceInvoicesDetails_Users]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoicesDetails_Users1] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] CHECK CONSTRAINT [FK_PriceInvoicesDetails_Users1]
GO
ALTER TABLE [dbo].[PriceInvoicesDetails]  WITH CHECK ADD  CONSTRAINT [FK_PriceInvoicesDetails_Users2] FOREIGN KEY([ModifiedBy])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[PriceInvoicesDetails] CHECK CONSTRAINT [FK_PriceInvoicesDetails_Users2]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'العميل' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'CustomerId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'اجمالى العدد' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'TotalQuantity'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'اجمالى القيمة' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'TotalValue'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ضريبة قيمة مضافة' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'SalesTax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ضريبة ارباح تجارية' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'ProfitTax'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'خصم نسبة او كاش ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'InvoiceDiscount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'اجمالى الخصومات من الاصناف والفاتورة' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'TotalDiscount'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'صافى الفاتورة' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoices', @level2type=N'COLUMN',@level2name=N'Safy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'خصم نسبة او كاش ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PriceInvoicesDetails', @level2type=N'COLUMN',@level2name=N'ItemDiscount'
GO
commit