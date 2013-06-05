CREATE PROCEDURE [dbo].[GetThumbnailFileUniqueId]
(
	@FileUniqueId nvarchar(32),
	@Width int,
	@Height int,
	@Align int,
	@ThumbnailFileUniqueId nvarchar(32) = NULL OUTPUT	
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @ThumbnailFileUniqueId = f.FileUniqueId 
	FROM (SELECT FileUniqueId FROM dbo.GetChildFiles(@FileUniqueId)) AS cf 
	INNER JOIN dbo.[File] AS f 
		ON cf.FileUniqueId = f.FileUniqueId
	WHERE 
		(Deleted = 0)
		AND (	((@Width IS NULL) AND (Width IS NULL))
				OR (Width = @Width)
			)
		AND (	((@Height IS NULL) AND (Height IS NULL))
				OR (Height = @Height)
			)
		AND (Align = @Align);
END