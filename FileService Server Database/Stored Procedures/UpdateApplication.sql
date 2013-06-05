CREATE PROCEDURE [dbo].[UpdateApplication]
(
	@ApplicationGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE [dbo].[Application] 
	SET [Name] = @Name 
	WHERE ([ApplicationGuid] = @ApplicationGuid);
	
	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM Application 
	WHERE (ApplicationGuid = @ApplicationGuid);
END