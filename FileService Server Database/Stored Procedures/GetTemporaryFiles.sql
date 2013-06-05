CREATE PROCEDURE [dbo].[GetTemporaryFiles]
(	
	@TemporaryGuid uniqueidentifier
)
AS
BEGIN
	DECLARE @Date datetime;
	SET @Date = DATEADD(day, -1, GETUTCDATE());

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE 
		((@TemporaryGuid IS NOT NULL) AND ([TemporaryGuid] = @TemporaryGuid))
		OR ((@TemporaryGuid IS NULL) AND (TemporaryGuid IS NOT NULL) AND (CreatedTime <= @Date) AND (Deleted = 0));
END