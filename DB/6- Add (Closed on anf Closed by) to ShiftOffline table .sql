BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ShiftsOffline
	DROP CONSTRAINT FK_ShiftsOffline_Users
GO
ALTER TABLE dbo.ShiftsOffline
	DROP CONSTRAINT FK_ShiftsOffline_Users1
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ShiftsOffline
	DROP CONSTRAINT FK_ShiftsOffline_PointOfSales
GO
ALTER TABLE dbo.PointOfSales SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ShiftsOffline
	DROP CONSTRAINT FK_ShiftsOffline_Employees
GO
ALTER TABLE dbo.Employees SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_ShiftsOffline
	(
	ID int NOT NULL IDENTITY (1, 1),
	PointOfSaleID int NOT NULL,
	Date datetime NOT NULL,
	EmployeeID int NOT NULL,
	IsClosed bit NOT NULL,
	ClosedOn datetime NULL,
	ClosedBy int NULL,
	CreatedOn datetime NOT NULL,
	CreatedBy int NOT NULL,
	IsDelete bit NOT NULL,
	DeleteBy int NULL,
	DeletedOn datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_ShiftsOffline SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_ShiftsOffline ON
GO
IF EXISTS(SELECT * FROM dbo.ShiftsOffline)
	 EXEC('INSERT INTO dbo.Tmp_ShiftsOffline (ID, PointOfSaleID, Date, EmployeeID, IsClosed, CreatedOn, CreatedBy, IsDelete, DeleteBy, DeletedOn)
		SELECT ID, PointOfSaleID, Date, EmployeeID, IsClosed, CreatedOn, CreatedBy, IsDelete, DeleteBy, DeletedOn FROM dbo.ShiftsOffline WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_ShiftsOffline OFF
GO
ALTER TABLE dbo.SellInvoices
	DROP CONSTRAINT FK_SellInvoices_ShiftsOffline
GO
ALTER TABLE dbo.PurchaseInvoices
	DROP CONSTRAINT FK_PurchaseInvoices_ShiftsOffline
GO
ALTER TABLE dbo.PurchaseBackInvoices
	DROP CONSTRAINT FK_PurchaseBackInvoices_ShiftsOffline
GO
ALTER TABLE dbo.SellBackInvoices
	DROP CONSTRAINT FK_SellBackInvoices_ShiftsOffline
GO
DROP TABLE dbo.ShiftsOffline
GO
EXECUTE sp_rename N'dbo.Tmp_ShiftsOffline', N'ShiftsOffline', 'OBJECT' 
GO
ALTER TABLE dbo.ShiftsOffline ADD CONSTRAINT
	PK_ShiftsOffline PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ShiftsOffline ADD CONSTRAINT
	FK_ShiftsOffline_Employees FOREIGN KEY
	(
	EmployeeID
	) REFERENCES dbo.Employees
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShiftsOffline ADD CONSTRAINT
	FK_ShiftsOffline_PointOfSales FOREIGN KEY
	(
	PointOfSaleID
	) REFERENCES dbo.PointOfSales
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShiftsOffline ADD CONSTRAINT
	FK_ShiftsOffline_Users FOREIGN KEY
	(
	CreatedBy
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ShiftsOffline ADD CONSTRAINT
	FK_ShiftsOffline_Users1 FOREIGN KEY
	(
	DeleteBy
	) REFERENCES dbo.Users
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SellBackInvoices ADD CONSTRAINT
	FK_SellBackInvoices_ShiftsOffline FOREIGN KEY
	(
	ShiftOfflineID
	) REFERENCES dbo.ShiftsOffline
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SellBackInvoices SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.PurchaseBackInvoices ADD CONSTRAINT
	FK_PurchaseBackInvoices_ShiftsOffline FOREIGN KEY
	(
	ShiftOfflineID
	) REFERENCES dbo.ShiftsOffline
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PurchaseBackInvoices SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.PurchaseInvoices ADD CONSTRAINT
	FK_PurchaseInvoices_ShiftsOffline FOREIGN KEY
	(
	ShiftOfflineID
	) REFERENCES dbo.ShiftsOffline
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.PurchaseInvoices SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SellInvoices ADD CONSTRAINT
	FK_SellInvoices_ShiftsOffline FOREIGN KEY
	(
	ShiftOffLineID
	) REFERENCES dbo.ShiftsOffline
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.SellInvoices SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
