CREATE PROCEDURE [dbo].[GetStorages]
(
	@Private bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE 
		(@Private IS NULL)
		OR ((@Private = 0) AND (OrganizationId IS NULL))
		OR ((@Private = 1) AND (OrganizationId IS NOT NULL))
	ORDER BY 2;
END