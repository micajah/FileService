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
		, @NextFreeSpaceInMBOut = NextFreeSpaceInMB
	FROM (	SELECT  
				StorageGuid
				, [Path]
				, (	ISNULL(MaxSizeInMB, 330752) -- Max available space on the HDD for the file storages.
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