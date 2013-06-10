CREATE PROCEDURE [dbo].[GetFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, [Checksum], FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE FileUniqueId = @FileUniqueId;
END