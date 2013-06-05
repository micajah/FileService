CREATE PROCEDURE [dbo].[DeleteFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT f.FileUniqueId, f.ParentFileUniqueId, f.FileExtensionGuid, f.ApplicationGuid, f.StorageGuid, f.DepartmentGuid, f.[Name], f.SizeInBytes, f.Height, f.Width, f.Align, f.ExpirationRequired
		, f.CreatedTime, f.UpdatedTime, f.TemporaryGuid, f.Deleted, f.FileExtension, f.MimeType, f.StoragePath, f.OrganizationGuid
	FROM dbo.GetChildFiles(@FileUniqueId) AS c
	INNER JOIN FilesView AS f
		ON	c.FileUniqueId = f.FileUniqueId;
	
	BEGIN TRANSACTION;

	DECLARE @StorageGuid uniqueidentifier, @SizeInBytes int;

	DECLARE FileCursor CURSOR FOR 
	SELECT f.StorageGuid, f.SizeInBytes 
	FROM dbo.GetChildFiles(@FileUniqueId) AS c
	INNER JOIN [File] f
		ON	(c.FileUniqueId = f.FileUniqueId) AND (f.Deleted = 0);
	
	OPEN FileCursor;
	
	FETCH NEXT FROM FileCursor INTO @StorageGuid, @SizeInBytes;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE dbo.Storage
		SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) - @SizeInBytes,
			CurrentFileCount = ISNULL(CurrentFileCount, 0) - 1
		WHERE StorageGuid = @StorageGuid;

		FETCH NEXT FROM FileCursor INTO @StorageGuid, @SizeInBytes;
	END

	CLOSE FileCursor;
	DEALLOCATE FileCursor;
	
	UPDATE dbo.[File]
	SET Deleted = 1 
	WHERE FileUniqueId IN (SELECT FileUniqueId FROM dbo.GetChildFiles(@FileUniqueId));

	COMMIT;
END