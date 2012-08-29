BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_LocalObjectType Table'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Mfs_File_Mfs_LocalObjectType')
      ALTER TABLE [dbo].[Mfs_File] DROP CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'DF_Mfs_LocalObjectType_LocalObjectTypeGuid')
      ALTER TABLE [dbo].[Mfs_LocalObjectType] DROP CONSTRAINT [DF_Mfs_LocalObjectType_LocalObjectTypeGuid]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE TABLE [dbo].[tmp_Mfs_LocalObjectType] (
   [LocalObjectTypeId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Mfs_LocalObjectType_LocalObjectTypeGuid] DEFAULT (newid()),
   [Name] [nvarchar] (50) NOT NULL
)
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   INSERT INTO [dbo].[tmp_Mfs_LocalObjectType] ([LocalObjectTypeId], [Name])
   SELECT [LocalObjectTypeGuid], [Name]
   FROM [dbo].[Mfs_LocalObjectType]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   DROP TABLE [dbo].[Mfs_LocalObjectType]
GO

EXEC sp_rename N'[dbo].[tmp_Mfs_LocalObjectType]', N'Mfs_LocalObjectType'

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   ALTER TABLE [dbo].[Mfs_LocalObjectType] ADD CONSTRAINT [PK_Mfs_LocalObjectType] PRIMARY KEY CLUSTERED ([LocalObjectTypeId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_LocalObjectType Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_LocalObjectType Table'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_File Table'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Mfs_File_Mfs_LocalObjectType')
      ALTER TABLE [dbo].[Mfs_File] DROP CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE TABLE [dbo].[tmp_Mfs_File] (
   [FileUniqueId] [nvarchar] (32) NOT NULL,
   [OrganizationId] [uniqueidentifier] NOT NULL,
   [DepartmentId] [uniqueidentifier] NOT NULL,
   [LocalObjectTypeId] [uniqueidentifier] NOT NULL,
   [LocalObjectId] [nvarchar] (255) NOT NULL,
   [Name] [nvarchar] (255) NOT NULL,
   [SizeInBytes] [int] NOT NULL,
   [UpdatedTime] [datetime] NOT NULL,
   [UpdatedBy] [nvarchar] (255) NULL,
   [Deleted] [bit] NOT NULL
)
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   INSERT INTO [dbo].[tmp_Mfs_File] ([FileUniqueId], [OrganizationId], [DepartmentId], [LocalObjectTypeId], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted])
   SELECT [FileUniqueId], [OrganizationGuid], [DepartmentGuid], [LocalObjectTypeGuid], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted]
   FROM [dbo].[Mfs_File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   DROP TABLE [dbo].[Mfs_File]
GO

EXEC sp_rename N'[dbo].[tmp_Mfs_File]', N'Mfs_File'

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   ALTER TABLE [dbo].[Mfs_File] ADD CONSTRAINT [PK_Mfs_File] PRIMARY KEY CLUSTERED ([OrganizationId], [DepartmentId], [FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE INDEX [IX_Mfs_File_1] ON [dbo].[Mfs_File] ([OrganizationId], [DepartmentId], [LocalObjectId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE INDEX [IX_Mfs_File_2] ON [dbo].[Mfs_File] ([DepartmentId], [LocalObjectId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE UNIQUE INDEX [IX_Mfs_File_3] ON [dbo].[Mfs_File] ([FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_File Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_File Table'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_FilesView View'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

exec('ALTER VIEW dbo.Mfs_FilesView
AS
	SELECT f.FileUniqueId, f.OrganizationId, f.DepartmentId, f.LocalObjectTypeId, f.LocalObjectId, f.[Name], f.SizeInBytes, f.UpdatedTime, f.UpdatedBy, f.Deleted, t.[Name] AS LocalObjectTypeName
	FROM dbo.Mfs_File AS f
	INNER JOIN Mfs_LocalObjectType AS t
		ON	f.LocalObjectTypeId = t.LocalObjectTypeId')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_FilesView View Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_FilesView View'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_GetDeletedFiles Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

exec('ALTER PROCEDURE [dbo].[Mfs_GetDeletedFiles]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (Deleted  = 1);	
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_GetDeletedFiles Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_GetDeletedFiles Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_GetFile Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

exec('ALTER PROCEDURE [dbo].[Mfs_GetFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, OrganizationId, DepartmentId, LocalObjectTypeId, LocalObjectId, [Name], SizeInBytes, UpdatedTime, UpdatedBy, Deleted, LocalObjectTypeName
	FROM dbo.Mfs_FilesView
	WHERE (FileUniqueId = @FileUniqueId);
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_GetFile Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_GetFile Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_GetFiles Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

exec('ALTER PROCEDURE [dbo].[Mfs_GetFiles]
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
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_GetFiles Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_GetFiles Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_UpdateFile Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO

exec('ALTER PROCEDURE [dbo].[Mfs_UpdateFile]
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
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_UpdateFile Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_UpdateFile Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_LocalObjectType Table'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Mfs_File_Mfs_LocalObjectType')
      ALTER TABLE [dbo].[Mfs_File] ADD CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType] FOREIGN KEY ([LocalObjectTypeId]) REFERENCES [dbo].[Mfs_LocalObjectType] ([LocalObjectTypeId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_LocalObjectType Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_LocalObjectType Table'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Mfs_File Table'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Mfs_File_Mfs_LocalObjectType')
      ALTER TABLE [dbo].[Mfs_File] DROP CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Mfs_File_Mfs_LocalObjectType')
      ALTER TABLE [dbo].[Mfs_File] ADD CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType] FOREIGN KEY ([LocalObjectTypeId]) REFERENCES [dbo].[Mfs_LocalObjectType] ([LocalObjectTypeId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Mfs_File Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Mfs_File Table'
END
GO

UPDATE [dbo].[Mfs_Version]
SET [Version] = 3
GO
