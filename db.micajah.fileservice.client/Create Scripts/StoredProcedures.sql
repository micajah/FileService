IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_DeleteFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Mfs_DeleteFile]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_GetDeletedFiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Mfs_GetDeletedFiles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_GetFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Mfs_GetFile]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_GetFiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Mfs_GetFiles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_UpdateFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Mfs_UpdateFile]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Mfs_DeleteFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT OFF;

	DELETE FROM dbo.Mfs_File
	WHERE (FileUniqueId = @FileUniqueId);
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Mfs_GetDeletedFiles]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (Deleted  = 1);	
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Mfs_GetFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (FileUniqueId = @FileUniqueId);
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Mfs_GetFiles]
(
	@OrganizationId uniqueidentifier,
	@DepartmentId uniqueidentifier,
	@LocalObjectId nvarchar(255),
	@LocalObjectTypeName nvarchar(50),
	@Deleted bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (OrganizationId = @OrganizationId) AND (DepartmentId = @DepartmentId) AND (LocalObjectTypeName = @LocalObjectTypeName) AND (LocalObjectId = @LocalObjectId)
		AND ((@Deleted IS NULL) OR (Deleted = @Deleted))
	ORDER BY UpdatedTime, [Name];
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Mfs_UpdateFile]
(
	@FileUniqueId nvarchar(32),
	@OrganizationId uniqueidentifier,
	@DepartmentId uniqueidentifier,
	@LocalObjectTypeName nvarchar(50),
	@LocalObjectId nvarchar(255),
	@Name nvarchar(255),
	@SizeInBytes int,
	@UpdatedBy nvarchar(255),
	@Deleted bit
)
AS
BEGIN
	SET NOCOUNT OFF;

	DECLARE @LocalObjectTypeId uniqueidentifier;

	BEGIN TRANSACTION;

	SELECT @LocalObjectTypeId = LocalObjectTypeId
	FROM dbo.Mfs_LocalObjectType
	WHERE [Name] = @LocalObjectTypeName;

	IF (@LocalObjectTypeId IS NULL)
	BEGIN
		SET @LocalObjectTypeId = NEWID();

		INSERT INTO dbo.Mfs_LocalObjectType
			(LocalObjectTypeId, [Name])
		VALUES
			(@LocalObjectTypeId, @LocalObjectTypeName);

		IF (@@ERROR <> 0)
			ROLLBACK TRANSACTION;
	END

	IF EXISTS(SELECT 0 FROM dbo.Mfs_File WHERE (FileUniqueId = @FileUniqueId))
		UPDATE dbo.Mfs_File
		SET OrganizationId = @OrganizationId, DepartmentId = @DepartmentId, LocalObjectTypeId = @LocalObjectTypeId, LocalObjectId = @LocalObjectId
			, [Name] = @Name, SizeInBytes = @SizeInBytes, UpdatedTime = GETDATE(), UpdatedBy = @UpdatedBy, Deleted = @Deleted
		WHERE (FileUniqueId = @FileUniqueId);
	ELSE
		INSERT INTO dbo.Mfs_File
			(FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted)
		VALUES
			(@FileUniqueId, @OrganizationId, @DepartmentId, @LocalObjectTypeId, @LocalObjectId, @Name, @SizeInBytes, GETDATE(), @UpdatedBy, @Deleted);

	IF (@@ERROR <> 0)
		ROLLBACK TRANSACTION;
	ELSE
		COMMIT;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (FileUniqueId = @FileUniqueId);
END

GO
