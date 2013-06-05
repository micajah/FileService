CREATE PROCEDURE [dbo].[GetFiles]
(
	@OrganizationGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE 
		(@OrganizationGuid IS NULL)
		OR (OrganizationGuid = @OrganizationGuid);
END