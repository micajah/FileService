CREATE PROCEDURE [dbo].[UpdateVendor]
(
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE [dbo].[Vendor] 
	SET [Name] = @Name 
	WHERE ([VendorGuid] = @VendorGuid);
	
	SELECT VendorGuid, [Name] 
	FROM Vendor 
	WHERE (VendorGuid = @VendorGuid);
END