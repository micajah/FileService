CREATE PROCEDURE [dbo].[GetOrganizations]
(
	@HasPrivateStorage bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT o.OrganizationGuid, o.ApplicationGuid, o.[Name], o.BWAAccountId, CASE WHEN s.StorageGuid IS NULL THEN 0 ELSE 1 END AS HasPrivateStorage
	FROM dbo.Organization AS o
	LEFT JOIN dbo.Storage AS s
		ON	o.OrganizationGuid = s.OrganizationId
	WHERE 
		(@HasPrivateStorage IS NULL)
		OR ((@HasPrivateStorage = 0) AND (s.StorageGuid IS NULL))
		OR ((@HasPrivateStorage = 1) AND (s.StorageGuid IS NOT NULL))
	ORDER BY [Name];
END