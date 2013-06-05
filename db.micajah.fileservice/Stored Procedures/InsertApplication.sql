CREATE PROCEDURE [dbo].[InsertApplication]
(
	@ApplicationGuid uniqueidentifier,
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@ApplicationGuid IS NULL)
		SET @ApplicationGuid = NEWID();

	INSERT INTO [dbo].[Application] ([ApplicationGuid], [VendorGuid], [Name]) 
	VALUES (@ApplicationGuid, @VendorGuid, @Name);
	
	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM Application 
	WHERE (ApplicationGuid = @ApplicationGuid);
END