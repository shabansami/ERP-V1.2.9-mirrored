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
ALTER TABLE dbo.ShiftsOffline SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.SellBackInvoices ADD
	ShiftOfflineID int NOT NULL CONSTRAINT DF_SellBackInvoices_ShiftOfflineID DEFAULT 1
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