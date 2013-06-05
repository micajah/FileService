CREATE PROCEDURE [dbo].[UpdateStorage]
(
	@StorageGuid uniqueidentifier,
	@MaxSizeInMB decimal(18,4),
	@MaxFileCount int,
	@OrganizationId uniqueidentifier,
	@Active bit
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE dbo.Storage
	SET MaxSizeInMB = @MaxSizeInMB, MaxFileCount = @MaxFileCount, OrganizationId = @OrganizationId, Active = @Active
	WHERE StorageGuid = @StorageGuid;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END