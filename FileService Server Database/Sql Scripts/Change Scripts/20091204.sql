-- ExpirationRequired

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.File Table'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_File')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_TransferLog_File')
      ALTER TABLE [dbo].[TransferLog] DROP CONSTRAINT [FK_TransferLog_File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Application')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Application]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Department')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Department]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_File')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_FileExtension')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_FileExtension]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Storage')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Storage]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'DF_File_IsDeleted')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [DF_File_IsDeleted]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   CREATE TABLE [dbo].[tmp_File] (
   [FileUniqueId] [nvarchar] (32) NOT NULL,
   [ParentFileUniqueId] [nvarchar] (32) NULL,
   [FileExtensionGuid] [uniqueidentifier] NOT NULL,
   [ApplicationGuid] [uniqueidentifier] NOT NULL,
   [StorageGuid] [uniqueidentifier] NOT NULL,
   [DepartmentGuid] [uniqueidentifier] NOT NULL,
   [Name] [nvarchar] (255) NOT NULL,
   [SizeInBytes] [int] NOT NULL,
   [Height] [int] NULL,
   [Width] [int] NULL,
   [Align] [int] NULL,
   [ExpirationRequired] [bit] NOT NULL CONSTRAINT [DF_File_ExpirationRequired] DEFAULT ((1)),
   [CreatedTime] [datetime] NOT NULL,
   [UpdatedTime] [datetime] NOT NULL,
   [TemporaryGuid] [uniqueidentifier] NULL,
   [Deleted] [bit] NOT NULL CONSTRAINT [DF_File_Deleted] DEFAULT ((0))
)
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   INSERT INTO [dbo].[tmp_File] ([FileUniqueId], [ParentFileUniqueId], [FileExtensionGuid], [ApplicationGuid], [StorageGuid], [DepartmentGuid], [Name], [SizeInBytes], [Height], [Width], [Align], [ExpirationRequired], [CreatedTime], [UpdatedTime], [TemporaryGuid], [Deleted])
   SELECT [FileUniqueId], [ParentFileUniqueId], [FileExtensionGuid], [ApplicationGuid], [StorageGuid], [DepartmentGuid], [Name], [SizeInBytes], [Height], [Width], [Align], 0, [CreatedTime], [UpdatedTime], [TemporaryGuid], [Deleted]
   FROM [dbo].[File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   DROP TABLE [dbo].[File]
GO

sp_rename N'[dbo].[tmp_File]', N'File'

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   ALTER TABLE [dbo].[File] ADD CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.File Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.File Table'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.FilesView View'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER VIEW [dbo].[FilesView]
AS
	SELECT f.FileUniqueId, f.ParentFileUniqueId, f.FileExtensionGuid, f.ApplicationGuid, f.StorageGuid, f.DepartmentGuid, f.[Name], f.SizeInBytes, f.Height, f.Width, f.Align, f.ExpirationRequired
		, f.CreatedTime, f.UpdatedTime, f.TemporaryGuid, f.Deleted, fe.FileExtension, fe.MimeType, (s.[Path] + CASE WHEN RIGHT(s.[Path], 1) = ''\'' THEN '''' ELSE ''\'' END) AS StoragePath, d.OrganizationGuid
	FROM dbo.[File] AS f
	INNER JOIN dbo.FileExtension AS fe
		ON	f.FileExtensionGuid = fe.FileExtensionGuid
	INNER JOIN dbo.Storage AS s
		ON	f.StorageGuid = s.StorageGuid
	INNER JOIN dbo.Department AS d
		ON	f.DepartmentGuid = d.DepartmentGuid')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.FilesView View Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.FilesView View'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.DeleteFile Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[DeleteFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT f.FileUniqueId, f.ParentFileUniqueId, f.FileExtensionGuid, f.ApplicationGuid, f.StorageGuid, f.DepartmentGuid, f.[Name], f.SizeInBytes, f.Height, f.Width, f.Align, f.ExpirationRequired
		, f.CreatedTime, f.UpdatedTime, f.TemporaryGuid, f.Deleted, f.FileExtension, f.MimeType, f.StoragePath, f.OrganizationGuid
	FROM dbo.GetChildFiles(@FileUniqueId) AS c
	INNER JOIN FilesView AS f
		ON	c.FileUniqueId = f.FileUniqueId;
	
	BEGIN TRANSACTION;

	DECLARE @StorageGuid uniqueidentifier, @SizeInBytes int;

	DECLARE FileCursor CURSOR FOR 
	SELECT f.StorageGuid, f.SizeInBytes 
	FROM dbo.GetChildFiles(@FileUniqueId) AS c
	INNER JOIN [File] f
		ON	(c.FileUniqueId = f.FileUniqueId) AND (f.Deleted = 0);
	
	OPEN FileCursor;
	
	FETCH NEXT FROM FileCursor INTO @StorageGuid, @SizeInBytes;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE dbo.Storage
		SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) - @SizeInBytes,
			CurrentFileCount = ISNULL(CurrentFileCount, 0) - 1
		WHERE StorageGuid = @StorageGuid;

		FETCH NEXT FROM FileCursor INTO @StorageGuid, @SizeInBytes;
	END

	CLOSE FileCursor;
	DEALLOCATE FileCursor;
	
	UPDATE dbo.[File]
	SET Deleted = 1 
	WHERE FileUniqueId IN (SELECT FileUniqueId FROM dbo.GetChildFiles(@FileUniqueId));

	COMMIT;
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.DeleteFile Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.DeleteFile Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.GetFile Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[GetFile]
(
	@FileUniqueId nvarchar(32)
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE FileUniqueId = @FileUniqueId;
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetFile Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.GetFile Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.GetFiles Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[GetFiles]
(
	@OrganizationGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE 
		(@OrganizationGuid IS NULL)
		OR (OrganizationGuid = @OrganizationGuid);
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetFiles Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.GetFiles Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.GetTemporaryFiles Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[GetTemporaryFiles]
(	
	@TemporaryGuid uniqueidentifier
)
AS
BEGIN
	DECLARE @Date datetime;
	SET @Date = DATEADD(day, -1, GETDATE());

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE 
		((@TemporaryGuid IS NOT NULL) AND ([TemporaryGuid] = @TemporaryGuid))
		OR ((@TemporaryGuid IS NULL) AND (TemporaryGuid IS NOT NULL) AND (CreatedTime <= @Date) AND (Deleted = 0));
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetTemporaryFiles Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.GetTemporaryFiles Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.UpdateFile Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[UpdateFile]
(
	@FileUniqueId nvarchar(32),
	@ParentFileUniqueId nvarchar(32),
	@FileExtension nvarchar(255),
	@MimeType nvarchar(255),
	@ApplicationGuid uniqueidentifier,
	@StorageGuid uniqueidentifier,
	@OrganizationGuid uniqueidentifier,
	@DepartmentGuid uniqueidentifier,
	@Name nvarchar(255),
	@SizeInBytes int,
	@Width int,
	@Height int,
	@Align int,
	@ExpirationRequired bit,
	@Deleted bit,
	@UpdateTransferLog bit,
	@ErrorCode int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	SET @ErrorCode = 0;

	IF (NOT EXISTS(SELECT 0 FROM dbo.Application WHERE ApplicationGuid = @ApplicationGuid))
	BEGIN
		SET @ErrorCode = -1;
		RETURN;
	END

	IF (NOT EXISTS(SELECT 0 FROM dbo.Organization WHERE OrganizationGuid = @OrganizationGuid))
	BEGIN
		SET @ErrorCode = -2;
		RETURN;
	END

	IF (NOT EXISTS(SELECT 0 FROM dbo.Department WHERE (OrganizationGuid = @OrganizationGuid) AND (DepartmentGuid = @DepartmentGuid)))
	BEGIN
		SET @ErrorCode = -3;
		RETURN;
	END

	BEGIN TRANSACTION;

	DECLARE @FileExtensionGuid uniqueidentifier, @CurrentDate datetime, @PreviousStorageGuid uniqueidentifier;

	SET @CurrentDate = GETDATE();
	SET @FileExtension = LOWER(@FileExtension);
	
	SELECT @FileExtensionGuid = FileExtensionGuid 
	FROM dbo.FileExtension 
	WHERE LOWER(FileExtension) = @FileExtension;

	IF (@FileExtensionGuid IS NULL)
	BEGIN
		SET @FileExtensionGuid = NEWID();

		INSERT INTO dbo.FileExtension (FileExtensionGuid, FileExtension, MimeType) 
		VALUES (@FileExtensionGuid, @FileExtension, @MimeType);
	END
	
	SELECT @PreviousStorageGuid = StorageGuid
	FROM dbo.[File] WHERE FileUniqueId = @FileUniqueId;

	IF (@PreviousStorageGuid IS NOT NULL)
	BEGIN
		IF (@PreviousStorageGuid <> @StorageGuid)
			UPDATE dbo.Storage
			SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) - @SizeInBytes,
				CurrentFileCount = ISNULL(CurrentFileCount, 0) - 1
			WHERE StorageGuid = @PreviousStorageGuid;
	
		UPDATE dbo.[File]
		SET ParentFileUniqueId = @ParentFileUniqueId
			, FileExtensionGuid = @FileExtensionGuid
			, ApplicationGuid = @ApplicationGuid
			, StorageGuid = @StorageGuid
			, DepartmentGuid = @DepartmentGuid
			, [Name] = @Name
			, SizeInBytes = @SizeInBytes
			, Height = @Height
			, Width = @Width
			, Align = @Align
			, UpdatedTime = @CurrentDate
			, ExpirationRequired = (CASE WHEN @ExpirationRequired IS NOT NULL THEN @ExpirationRequired ELSE ExpirationRequired END)
			, Deleted = @Deleted
		WHERE FileUniqueId = @FileUniqueId;
	END
	ELSE
		INSERT INTO dbo.[File] 
			(FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align
			, ExpirationRequired, CreatedTime, UpdatedTime, TemporaryGuid, Deleted)
		VALUES 
			(@FileUniqueId, @ParentFileUniqueId, @FileExtensionGuid, @ApplicationGuid, @StorageGuid, @DepartmentGuid, @Name, @SizeInBytes, @Height, @Width, @Align
			, ISNULL(@ExpirationRequired, 0), @CurrentDate, @CurrentDate, NULL, @Deleted);
-- TODO: Should be changed from ISNULL(@ExpirationRequired, 0) to ISNULL(@ExpirationRequired, 1) when the FS-client library will be updated in all appllications.

	UPDATE dbo.Storage
	SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) + @SizeInBytes,
		CurrentFileCount = ISNULL(CurrentFileCount, 0) + 1
	WHERE StorageGuid = @StorageGuid;

	IF (@UpdateTransferLog = 1) EXEC dbo.UpdateTransferLog @FileUniqueId, 0;

	COMMIT;
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.UpdateFile Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.UpdateFile Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Creating dbo.UpdateFileExpirationRequired Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('CREATE PROCEDURE [dbo].[UpdateFileExpirationRequired]
(
	@FileUniqueId nvarchar(32),
	@ExpirationRequired bit
)
AS
BEGIN
	SET NOCOUNT ON;

-- TODO: Should be uncommented when the FS-client library will be updated in all appllications.
/*	UPDATE [dbo].[File] 
	SET ExpirationRequired = @ExpirationRequired 
	WHERE FileUniqueId IN (
		SELECT c.FileUniqueId
		FROM dbo.GetChildFiles(@FileUniqueId) AS c
	);
*/
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.UpdateFileExpirationRequired Procedure Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.UpdateFileExpirationRequired Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.File Table'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Application')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Application]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Application')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_Application] FOREIGN KEY ([ApplicationGuid]) REFERENCES [dbo].[Application] ([ApplicationGuid])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Department')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Department]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Department')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_Department] FOREIGN KEY ([DepartmentGuid]) REFERENCES [dbo].[Department] ([DepartmentGuid])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_File')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_File]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_File')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_File] FOREIGN KEY ([ParentFileUniqueId]) REFERENCES [dbo].[File] ([FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_FileExtension')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_FileExtension]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_FileExtension')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_FileExtension] FOREIGN KEY ([FileExtensionGuid]) REFERENCES [dbo].[FileExtension] ([FileExtensionGuid])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Storage')
      ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Storage]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_Storage')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_Storage] FOREIGN KEY ([StorageGuid]) REFERENCES [dbo].[Storage] ([StorageGuid])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_File_File')
      ALTER TABLE [dbo].[File] ADD CONSTRAINT [FK_File_File] FOREIGN KEY ([ParentFileUniqueId]) REFERENCES [dbo].[File] ([FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_TransferLog_File')
      ALTER TABLE [dbo].[TransferLog] ADD CONSTRAINT [FK_TransferLog_File] FOREIGN KEY ([FileUniqueId]) REFERENCES [dbo].[File] ([FileUniqueId])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.File Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.File Table'
END
GO
