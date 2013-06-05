CREATE PROCEDURE [dbo].[InsertStorage]
(
	@StorageGuid uniqueidentifier,
	@Path nvarchar(3000),
	@MaxSizeInMB decimal(18,4),
	@MaxFileCount int,
	@OrganizationId uniqueidentifier,
	@Active bit
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@StorageGuid IS NULL)
		SET @StorageGuid = NEWID();

	INSERT INTO dbo.Storage (StorageGuid, [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active)
	VALUES (@StorageGuid, @Path, @MaxSizeInMB, 0, @MaxFileCount, 0, @OrganizationId, @Active);

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END