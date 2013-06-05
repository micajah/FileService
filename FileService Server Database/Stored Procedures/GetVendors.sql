CREATE PROCEDURE [dbo].[GetVendors]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VendorGuid, [Name] 
	FROM dbo.Vendor;
END