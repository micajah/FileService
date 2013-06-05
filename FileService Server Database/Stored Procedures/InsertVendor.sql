CREATE PROCEDURE [dbo].[InsertVendor]
(
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@VendorGuid IS NULL)
		SET @VendorGuid = NEWID();

	INSERT INTO [dbo].[Vendor] ([VendorGuid], [Name]) 
	VALUES (@VendorGuid, @Name);
	
	SELECT VendorGuid, [Name] 
	FROM Vendor 
	WHERE (VendorGuid = @VendorGuid);
END