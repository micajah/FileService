CREATE PROCEDURE [dbo].[GetVendorApplications]
(
	@VendorGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM dbo.Application
	WHERE (VendorGuid = @VendorGuid);
END