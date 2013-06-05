CREATE PROCEDURE [dbo].[GetVendorsUsedSpace]
(
	@StartDate datetime = NULL, 
	@EndDate datetime = NULL, 
	@VendorName nvarchar(255) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		v.VendorGuid
		, v.Name
		, a.Name AS ApplicationName
		, o.Name AS OrganizationName
		, d.Name AS DepartmentName
		, COUNT(DISTINCT f.FileUniqueId) AS CurrentFilesCount
		, (CAST(SUM(f.SizeInBytes) AS decimal) / 1048576) AS CurrentFilesSizeInMB
		, SUM((CAST(l.SizeInBytes AS decimal) * l.DownloadCount) / 1048576)  AS Transfer
	FROM dbo.Vendor v
	INNER JOIN dbo.Application a
		ON	v.VendorGuid = a.VendorGuid
			AND (	(@VendorName IS NULL)
					OR ((@VendorName IS NOT NULL) AND (v.Name = @VendorName))
				)
	INNER JOIN dbo.Organization o
		ON	a.ApplicationGuid = o.ApplicationGuid
	INNER JOIN dbo.Department d
		ON	o.OrganizationGuid = d.OrganizationGuid
	INNER JOIN dbo.[File] f
		ON	f.ApplicationGuid = a.ApplicationGuid
			AND f.DepartmentGuid = d.DepartmentGuid
			AND f.Deleted = 0
			AND (	(@StartDate IS NULL)
					OR ((@StartDate IS NOT NULL) AND (f.CreatedTime >= @StartDate))
				)
			AND (	(@EndDate IS NULL)
					OR ((@EndDate IS NOT NULL) AND (f.CreatedTime <= @EndDate))
				)
	INNER JOIN dbo.TransferLog l
		ON	f.FileUniqueId = l.FileUniqueId
	GROUP BY v.VendorGuid, v.Name, a.Name, o.Name, d.Name;
END