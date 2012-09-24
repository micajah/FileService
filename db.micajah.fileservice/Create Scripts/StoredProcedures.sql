IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DeleteFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteFile]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetApplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetApplication]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetFile]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetFiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetFiles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetOrganizations]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetOrganizations]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetStorage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetStorage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetStorages]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetStorages]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetTemporaryFiles]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetTemporaryFiles]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetThumbnailFileUniqueId]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetThumbnailFileUniqueId]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetVendor]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVendor]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetVendorApplications]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVendorApplications]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetVendors]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVendors]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetVendorsUsedSpace]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetVendorsUsedSpace]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertApplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertApplication]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertStorage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertStorage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[InsertVendor]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[InsertVendor]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateApplication]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateApplication]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateFile]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateFile]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateFileExpirationRequired]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateFileExpirationRequired]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateFileTemporaryGuid]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateFileTemporaryGuid]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateOrganizationDepartment]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateOrganizationDepartment]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateStorage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateStorage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateTransferLog]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateTransferLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[UpdateVendor]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[UpdateVendor]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[ValidateStorage]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[ValidateStorage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[DeleteFile]
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
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetFile]
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
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetFiles]
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
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetOrganizations]
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
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetStorage]
(
	@StorageGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE (StorageGuid = @StorageGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetStorages]
(
	@Private bit
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage
	WHERE 
		(@Private IS NULL)
		OR ((@Private = 0) AND (OrganizationId IS NULL))
		OR ((@Private = 1) AND (OrganizationId IS NOT NULL))
	ORDER BY 2;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetTemporaryFiles]
(	
	@TemporaryGuid uniqueidentifier
)
AS
BEGIN
	DECLARE @Date datetime;
	SET @Date = DATEADD(day, -1, GETUTCDATE());

	SELECT FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align, ExpirationRequired
		, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, FileExtension, MimeType, StoragePath, OrganizationGuid
	FROM dbo.FilesView
	WHERE 
		((@TemporaryGuid IS NOT NULL) AND ([TemporaryGuid] = @TemporaryGuid))
		OR ((@TemporaryGuid IS NULL) AND (TemporaryGuid IS NOT NULL) AND (CreatedTime <= @Date) AND (Deleted = 0));
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetVendor]
(
	@VendorGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VendorGuid, [Name] 
	FROM dbo.Vendor
	WHERE ([VendorGuid] = @VendorGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetVendorApplications]
(
	@VendorGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM dbo.Application
	WHERE (VendorGuid = @VendorGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetVendors]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VendorGuid, [Name] 
	FROM dbo.Vendor;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[GetVendorsUsedSpace]
(
	@StartDate datetime = NULL, 
	@EndDate datetime = NULL, 
	@VendorName nvarchar(255) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		v.VendorGuid
		, v.Name
		, a.Name AS ApplicationName
		, o.Name AS OrganizationName
		, d.Name AS DepartmentName
		, COUNT(DISTINCT f.FileUniqueId) AS CurrentFilesCount
		, (CAST(SUM(f.SizeInBytes) AS decimal) / 1048576) AS CurrentFilesSizeInMB
		, SUM((CAST(l.SizeInBytes AS decimal) * l.DownloadCount) / 1048576)  AS Transfer
	FROM dbo.Vendor v
	INNER JOIN dbo.Application a
		ON	v.VendorGuid = a.VendorGuid
			AND (	(@VendorName IS NULL)
					OR ((@VendorName IS NOT NULL) AND (v.Name = @VendorName))
				)
	INNER JOIN dbo.Organization o
		ON	a.ApplicationGuid = o.ApplicationGuid
	INNER JOIN dbo.Department d
		ON	o.OrganizationGuid = d.OrganizationGuid
	INNER JOIN dbo.[File] f
		ON	f.ApplicationGuid = a.ApplicationGuid
			AND f.DepartmentGuid = d.DepartmentGuid
			AND f.Deleted = 0
			AND (	(@StartDate IS NULL)
					OR ((@StartDate IS NOT NULL) AND (f.CreatedTime >= @StartDate))
				)
			AND (	(@EndDate IS NULL)
					OR ((@EndDate IS NOT NULL) AND (f.CreatedTime <= @EndDate))
				)
	INNER JOIN dbo.TransferLog l
		ON	f.FileUniqueId = l.FileUniqueId
	GROUP BY v.VendorGuid, v.Name, a.Name, o.Name, d.Name;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[InsertApplication]
(
	@ApplicationGuid uniqueidentifier,
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@ApplicationGuid IS NULL)
		SET @ApplicationGuid = NEWID();

	INSERT INTO [dbo].[Application] ([ApplicationGuid], [VendorGuid], [Name]) 
	VALUES (@ApplicationGuid, @VendorGuid, @Name);
	
	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM Application 
	WHERE (ApplicationGuid = @ApplicationGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertStorage]
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

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[InsertVendor]
(
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	IF (@VendorGuid IS NULL)
		SET @VendorGuid = NEWID();

	INSERT INTO [dbo].[Vendor] ([VendorGuid], [Name]) 
	VALUES (@VendorGuid, @Name);
	
	SELECT VendorGuid, [Name] 
	FROM Vendor 
	WHERE (VendorGuid = @VendorGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateApplication]
(
	@ApplicationGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE [dbo].[Application] 
	SET [Name] = @Name 
	WHERE ([ApplicationGuid] = @ApplicationGuid);
	
	SELECT ApplicationGuid, VendorGuid, [Name] 
	FROM Application 
	WHERE (ApplicationGuid = @ApplicationGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateFile]
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

	SET @CurrentDate = GETUTCDATE();
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
			, ISNULL(@ExpirationRequired, 1), @CurrentDate, @CurrentDate, NULL, @Deleted);

	UPDATE dbo.Storage
	SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) + @SizeInBytes,
		CurrentFileCount = ISNULL(CurrentFileCount, 0) + 1
	WHERE StorageGuid = @StorageGuid;

	IF (@UpdateTransferLog = 1) EXEC dbo.UpdateTransferLog @FileUniqueId, 0;

	COMMIT;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateOrganizationDepartment]
(
	@ApplicationGuid uniqueidentifier,
	@OrganizationName nvarchar(255),
	@DepartmentName nvarchar(255),
	@OrganizationGuid uniqueidentifier = NULL OUTPUT,
	@DepartmentGuid uniqueidentifier = NULL OUTPUT,
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

	DECLARE @OrgGuid uniqueidentifier;

	SELECT @OrgGuid = OrganizationGuid 
	FROM dbo.Organization 
	WHERE (ApplicationGuid = @ApplicationGuid) AND (OrganizationGuid = @OrganizationGuid);

	BEGIN TRANSACTION;

	IF (@OrgGuid IS NULL)
	BEGIN
		IF (@OrganizationGuid IS NULL)
			SET @OrgGuid = NEWID();
		ELSE
			SET @OrgGuid = @OrganizationGuid;

		INSERT INTO dbo.Organization (OrganizationGuid, ApplicationGuid, [Name], BWAAccountId)
		VALUES (@OrgGuid, @ApplicationGuid, LOWER(@OrganizationName), NULL);

		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION;
			SET @ErrorCode = -2;
			RETURN;
		END
	END
	ELSE
	BEGIN
		UPDATE [dbo].[Organization]
		SET [Name] = LOWER(@OrganizationName)
		WHERE [OrganizationGuid] = @OrgGuid;

		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION;
			SET @ErrorCode = -3;
			RETURN;
		END
	END
	
	SET @OrganizationGuid = @OrgGuid;

	DECLARE @DeptGuid uniqueidentifier;

	SELECT @DeptGuid = DepartmentGuid 
	FROM dbo.Department 
	WHERE (OrganizationGuid = @OrganizationGuid) AND (DepartmentGuid = @DepartmentGuid);

	IF (@DeptGuid IS NULL)
	BEGIN
		IF (@DepartmentGuid IS NULL)
			SET @OrgGuid = NEWID();
		ELSE
			SET @DeptGuid = @DepartmentGuid;

		INSERT INTO dbo.Department (DepartmentGuid, OrganizationGuid, [Name])
		VALUES (@DeptGuid, @OrganizationGuid, LOWER(@DepartmentName))

		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION;
			SET @ErrorCode = -4;
			RETURN;
		END
	END
	ELSE
	BEGIN
		UPDATE dbo.Department
		SET [Name] = LOWER(@DepartmentName)
		WHERE DepartmentGuid = @DeptGuid;

		IF (@@ERROR <> 0)
		BEGIN
			ROLLBACK TRANSACTION;
			SET @ErrorCode = -5;
			RETURN;
		END
	END
	
	SET @DepartmentGuid = @DeptGuid;

	IF (@@ERROR <> 0)		
	BEGIN
		ROLLBACK TRANSACTION;
		SET @ErrorCode = -6;
	END
	ELSE
		COMMIT;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateStorage]
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

	SELECT StorageGuid, ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) AS [Path], MaxSizeInMB, CurrentSizeInBytes, MaxFileCount, CurrentFileCount, OrganizationId, Active
	FROM dbo.Storage 
	WHERE (StorageGuid = @StorageGuid);
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateTransferLog]
(
	@FileUniqueId nvarchar(32),
	@IsDownload bit
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @FileSize int;

	SELECT @FileSize = SizeInBytes 
	FROM dbo.[File] 
	WHERE (@FileUniqueId = @FileUniqueId) AND (Deleted = 0);

	If (ISNULL(@FileSize, 0) <= 0) RETURN;

	DECLARE @CurrentDate datetime, @LogDate datetime, @TransferLogGuid uniqueidentifier;

	SET @CurrentDate = GETUTCDATE();
	SET @LogDate = @CurrentDate;

	SET @LogDate = DATEADD(dd, -DATEPART(dd, @CurrentDate) + 1, @LogDate);
	SET @LogDate = DATEADD(hh, -DATEPART(hh, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(mi, -DATEPART(mi, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(ss, -DATEPART(ss, @CurrentDate), @LogDate);
	SET @LogDate = DATEADD(ms, -DATEPART(ms, @CurrentDate), @LogDate);

	SELECT @TransferLogGuid = TransferLogGuid
	FROM dbo.TransferLog 
	WHERE (FileUniqueId = @FileUniqueId) AND (MonthAudit = @LogDate) AND (SizeInBytes = @FileSize) AND (IsDownload = @IsDownload);
	
	IF (@TransferLogGuid IS NULL)
		INSERT INTO dbo.TransferLog(FileUniqueId, DownloadCount, SizeInBytes, IsDownload, MonthAudit) 
		VALUES (@FileUniqueId, 1, @FileSize, @IsDownload, @LogDate);
	ELSE
		UPDATE dbo.TransferLog 
		SET DownloadCount = DownloadCount + 1
		WHERE TransferLogGuid = @TransferLogGuid;
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateVendor]
(
	@VendorGuid uniqueidentifier,
	@Name nvarchar(255)
)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE [dbo].[Vendor] 
	SET [Name] = @Name 
	WHERE ([VendorGuid] = @VendorGuid);
	
	SELECT VendorGuid, [Name] 
	FROM Vendor 
	WHERE (VendorGuid = @VendorGuid);
END


GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ValidateStorage]
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
		, @StoragePath = ([Path] + CASE WHEN RIGHT([Path], 1) = '\' THEN '' ELSE '\' END) 
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
END

GO
