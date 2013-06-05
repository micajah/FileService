-- Private Storages
BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Storage Table'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   ALTER TABLE [dbo].[Storage]
      ADD [OrganizationId] [uniqueidentifier] NULL
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Storage Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Storage Table'
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

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align
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

PRINT 'Creating dbo.GetOrganizations Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('CREATE PROCEDURE [dbo].[GetOrganizations]
(
	@HasPrivateStorage bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT o.OrganizationGuid, o.ApplicationGuid, o.[Name], o.BWAAccountId, CASE WHEN s.StorageGuid IS NULL THEN 0 ELSE 1 END AS HasPrivateStorage
	FROM dbo.Organization AS o
	LEFT JOIN dbo.Storage AS s
		ON	o.OrganizationGuid = s.OrganizationId
	WHERE 
		(@HasPrivateStorage IS NULL)
		OR ((@HasPrivateStorage = 0) AND (s.StorageGuid IS NULL))
		OR ((@HasPrivateStorage = 1) AND (s.StorageGuid IS NOT NULL))
	ORDER BY [Name];
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetOrganizations Procedure Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.GetOrganizations Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Creating dbo.GetStorage Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('CREATE PROCEDURE [dbo].[GetStorage]
(
	@StorageGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = ''\'' THEN '''' ELSE ''\'' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE (StorageGuid = @StorageGuid);
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetStorage Procedure Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.GetStorage Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.GetStorages Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[GetStorages]
(
	@Private bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = ''\'' THEN '''' ELSE ''\'' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE 
		(@Private IS NULL)
		OR ((@Private = 0) AND (OrganizationId IS NULL))
		OR ((@Private = 1) AND (OrganizationId IS NOT NULL))
	ORDER BY 2;
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.GetStorages Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.GetStorages Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Creating dbo.InsertStorage Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('CREATE PROCEDURE [dbo].[InsertStorage]
(
	@StorageGuid uniqueidentifier,
	@Path nvarchar(3000),
	@MaxSizeInMB decimal(18,4),
	@MaxFileCount int,
	@OrganizationId uniqueidentifier,
	@Active bit
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@StorageGuid IS NULL)
		SET @StorageGuid = NEWID();

	INSERT INTO dbo.Storage (StorageGuid, [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active)
	VALUES (@StorageGuid, @Path, @MaxSizeInMB, 0, @MaxFileCount, 0, @OrganizationId, @Active);

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = ''\'' THEN '''' ELSE ''\'' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.InsertStorage Procedure Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.InsertStorage Procedure'
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
	@ParentFileUniqueId nvarchar(32) = NULL,
	@FileExtension nvarchar(255),
	@MimeType nvarchar(255),
	@ApplicationGuid uniqueidentifier,
	@StorageGuid uniqueidentifier,
	@OrganizationGuid uniqueidentifier,
	@DepartmentGuid uniqueidentifier,
	@Name nvarchar(255),
	@SizeInBytes int,
	@Width int = NULL,
	@Height int = NULL,
	@Align int = NULL,
	@Deleted bit = 0,
	@UpdateTransferLog bit = 1,
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
			, Deleted = @Deleted
		WHERE FileUniqueId = @FileUniqueId;
	END
	ELSE
		INSERT INTO dbo.[File] 
			(FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align
			, CreatedTime, UpdatedTime, TemporaryGuid, Deleted)
		VALUES 
			(@FileUniqueId, @ParentFileUniqueId, @FileExtensionGuid, @ApplicationGuid, @StorageGuid, @DepartmentGuid, @Name, @SizeInBytes, @Height, @Width, @Align
			, @CurrentDate, @CurrentDate, NULL, @Deleted);

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

PRINT 'Creating dbo.UpdateStorage Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('CREATE PROCEDURE [dbo].[UpdateStorage]
(
	@StorageGuid uniqueidentifier,
	@MaxSizeInMB decimal(18,4),
	@MaxFileCount int,
	@OrganizationId uniqueidentifier,
	@Active bit
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE dbo.Storage
	SET MaxSizeInMB = @MaxSizeInMB, MaxFileCount = @MaxFileCount, OrganizationId = @OrganizationId, Active = @Active
	WHERE StorageGuid = @StorageGuid;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = ''\'' THEN '''' ELSE ''\'' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.UpdateStorage Procedure Added Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Add dbo.UpdateStorage Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.ValidateStorage Procedure'
GO

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, QUOTED_IDENTIFIER, CONCAT_NULL_YIELDS_NULL ON
GO

SET NUMERIC_ROUNDABORT OFF
GO


exec('ALTER PROCEDURE [dbo].[ValidateStorage]
(
	@ApplicationGuid uniqueidentifier,
	@OrganizationGuid uniqueidentifier,
	@DepartmentGuid uniqueidentifier,
	@FileUniqueId nvarchar(32),
	@SizeInMB numeric(18,4),
	@FilesCount int,
	@NextFreeSpaceInMBIn numeric(18,4),
	@NextFreeSpaceInMBOut numeric(18,4) OUTPUT,
	@StorageGuid uniqueidentifier OUTPUT,
	@StoragePath nvarchar(3000) OUTPUT,
	@ErrorCode int OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	SET @ErrorCode = 0;

	IF EXISTS(SELECT 0 FROM dbo.[File] WHERE FileUniqueId = @FileUniqueId)
	BEGIN
		SET @ErrorCode = -6;
		RETURN;
	END

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

	IF (@SizeInMB <= 0)
	Begin
		SET @ErrorCode = -4;
		RETURN;
	End

	IF (@FilesCount <= 0)
	BEGIN
		SET @ErrorCode = -5;
		RETURN;
	END

	SELECT TOP 1
		@StorageGuid = StorageGuid
		, @StoragePath = ([Path] + CASE WHEN RIGHT([Path], 1) = ''\'' THEN '''' ELSE ''\'' END) 
		, @NextFreeSpaceInMbOut = NextFreeSpaceInMB
	FROM (	SELECT  
				StorageGuid
				, [Path]
				, (	ISNULL(MaxSizeInMB, 100000)
					- (CASE WHEN CurrentSizeInBytes IS NULL THEN 0 ELSE (CONVERT(numeric(18,4), CurrentSizeInBytes) / 1048576) END) 
					- @SizeInMB
					) AS NextFreeSpaceInMB
				, OrganizationId
			FROM dbo.Storage
			WHERE (Active = 1) AND ((ISNULL(MaxFileCount, 1000000) - ISNULL(CurrentFileCount, 0)) >= @FilesCount)
				AND ((OrganizationId IS NULL) OR (OrganizationId = @OrganizationGuid))
		) AS rs
	WHERE
		(NextFreeSpaceInMB > 0)
		AND ((@NextFreeSpaceInMBIn = 0) OR ((@NextFreeSpaceInMBIn > 0) AND (NextFreeSpaceInMB < @NextFreeSpaceInMbIn)))
	ORDER BY OrganizationId DESC, NextFreeSpaceInMB DESC;

	IF (@StorageGuid IS NULL)
		SET @ErrorCode = -7;
END')
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.ValidateStorage Procedure Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.ValidateStorage Procedure'
END
GO

BEGIN TRANSACTION
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO

PRINT 'Updating dbo.Storage Table'
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Storage_Organization')
      ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [FK_Storage_Organization]
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
   IF NOT EXISTS (SELECT name FROM sysobjects WHERE name = N'FK_Storage_Organization')
      ALTER TABLE [dbo].[Storage] ADD CONSTRAINT [FK_Storage_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationGuid])
GO

IF @@ERROR <> 0
   IF @@TRANCOUNT = 1 ROLLBACK TRANSACTION
GO

IF @@TRANCOUNT = 1
BEGIN
   PRINT 'dbo.Storage Table Updated Successfully'
   COMMIT TRANSACTION
END ELSE
BEGIN
   PRINT 'Failed To Update dbo.Storage Table'
END
GO
 