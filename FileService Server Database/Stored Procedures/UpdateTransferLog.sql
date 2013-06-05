CREATE PROCEDURE [dbo].[UpdateTransferLog]
(
	@FileUniqueId nvarchar(32),
	@IsDownload bit
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @FileSize int;

	SELECT @FileSize = SizeInBytes 
	FROM dbo.[File] 
	WHERE (@FileUniqueId = @FileUniqueId) AND (Deleted = 0);

	If (ISNULL(@FileSize, 0) <= 0) RETURN;

	DECLARE @CurrentDate datetime, @LogDate datetime, @TransferLogGuid uniqueidentifier;

	SET @CurrentDate = GETUTCDATE();
	SET @LogDate = @CurrentDate;

	SET @LogDate = DATEADD(dd, -DATEPART(dd, @CurrentDate) + 1, @LogDate);
	SET @LogDate = DATEADD(hh, -DATEPART(hh, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(mi, -DATEPART(mi, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(ss, -DATEPART(ss, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(ms, -DATEPART(ms, @CurrentDate), @LogDate);

	SELECT @TransferLogGuid = TransferLogGuid
	FROM dbo.TransferLog 
	WHERE (FileUniqueId = @FileUniqueId) AND (MonthAudit = @LogDate) AND (SizeInBytes = @FileSize) AND (IsDownload = @IsDownload);
	
	IF (@TransferLogGuid IS NULL)
		INSERT INTO dbo.TransferLog(FileUniqueId, DownloadCount, SizeInBytes, IsDownload, MonthAudit) 
		VALUES (@FileUniqueId, 1, @FileSize, @IsDownload, @LogDate);
	ELSE
		UPDATE dbo.TransferLog 
		SET DownloadCount = DownloadCount + 1
		WHERE TransferLogGuid = @TransferLogGuid;
END