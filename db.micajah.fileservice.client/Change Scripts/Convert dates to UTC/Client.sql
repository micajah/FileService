UPDATE [mfs].[File]
SET UpdatedTime = dbo.GetUTCDateTime(UpdatedTime)
GO
