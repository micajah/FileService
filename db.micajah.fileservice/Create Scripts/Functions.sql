/****** Object:  UserDefinedFunction [dbo].[GetChildFiles]    Script Date: 03/10/2009 13:52:10 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetChildFiles]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetChildFiles]
GO

/****** Object:  UserDefinedFunction [dbo].[GetChildFiles]    Script Date: 03/10/2009 13:52:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

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
GO