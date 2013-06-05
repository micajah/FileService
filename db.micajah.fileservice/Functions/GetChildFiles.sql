CREATE FUNCTION [dbo].[GetChildFiles]
(
	@FileUniqueId nvarchar(32)
)
RETURNS @ChildFiles TABLE (FileUniqueId nvarchar(32) PRIMARY KEY) 
AS  
BEGIN 
	INSERT @ChildFiles (FileUniqueId) VALUES (@FileUniqueId)
	
	DECLARE @CurrentFileUniqueId nvarchar(32)

	DECLARE FileCursor CURSOR FOR 
	SELECT FileUniqueId 
	FROM dbo.[File] 
	WHERE (ParentFileUniqueId = @FileUniqueId) AND (Deleted = 0)

	OPEN FileCursor

	FETCH NEXT FROM FileCursor INTO @CurrentFileUniqueId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF EXISTS(SELECT 0 FROM dbo.[File] WHERE (ParentFileUniqueId = @CurrentFileUniqueId) AND (Deleted = 0))
			INSERT @ChildFiles (FileUniqueId)
			SELECT FileUniqueId FROM dbo.GetChildFiles(@CurrentFileUniqueId)
		ELSE
			INSERT @ChildFiles (FileUniqueId) VALUES (@CurrentFileUniqueId)
		
		FETCH NEXT FROM FileCursor INTO @CurrentFileUniqueId
	END

	CLOSE FileCursor
	DEALLOCATE FileCursor

	RETURN
END