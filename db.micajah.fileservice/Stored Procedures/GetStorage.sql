CREATE PROCEDURE [dbo].[GetStorage]
(
	@StorageGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE (StorageGuid = @StorageGuid);
END