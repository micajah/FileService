CREATE PROCEDURE [dbo].[UpdateFileTemporaryGuid]
(
	@FileUniqueId nvarchar(32),
	@TemporaryGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[File] 
	SET TemporaryGuid = @TemporaryGuid 
	WHERE (FileUniqueId = @FileUniqueId);
END