UPDATE dbo.[File] 
SET CreatedTime = dbo.GetUTCDateTime(CreatedTime)
	, UpdatedTime = dbo.GetUTCDateTime(UpdatedTime)
GO

UPDATE dbo.TransferLog 
SET MonthAudit = dbo.GetUTCDateTime(MonthAudit)
GO
