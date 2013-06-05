CREATE PROCEDURE [dbo].[GetVendor]
(
	@VendorGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VendorGuid, [Name] 
	FROM dbo.Vendor
	WHERE ([VendorGuid] = @VendorGuid);
END