CREATE PROCEDURE [dbo].[UpdateFileExpirationRequired]
(
	@FileUniqueId nvarchar(32),
	@ExpirationRequired bit
)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[File] 
	SET ExpirationRequired = @ExpirationRequired 
	WHERE FileUniqueId IN (
		SELECT c.FileUniqueId
		FROM dbo.GetChildFiles(@FileUniqueId) AS c
	);
END