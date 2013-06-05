CREATE PROCEDURE [dbo].[GetApplication]
(
	@ApplicationGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM dbo.Application
	WHERE (ApplicationGuid = @ApplicationGuid);
END