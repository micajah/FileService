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
	@Checksum varchar(32),
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
			, [Checksum] = @Checksum
		WHERE FileUniqueId = @FileUniqueId;
	END
	ELSE
		INSERT INTO dbo.[File] 
			(FileUniqueId, ParentFileUniqueId, FileExtensionGuid, ApplicationGuid, StorageGuid, DepartmentGuid, [Name], SizeInBytes, Height, Width, Align
			, ExpirationRequired, CreatedTime, UpdatedTime, TemporaryGuid, Deleted, [Checksum])
		VALUES 
			(@FileUniqueId, @ParentFileUniqueId, @FileExtensionGuid, @ApplicationGuid, @StorageGuid, @DepartmentGuid, @Name, @SizeInBytes, @Height, @Width, @Align
			, ISNULL(@ExpirationRequired, 1), @CurrentDate, @CurrentDate, NULL, @Deleted, @Checksum);

	UPDATE dbo.Storage
	SET CurrentSizeInBytes = ISNULL(CurrentSizeInBytes, 0) + @SizeInBytes,
		CurrentFileCount = ISNULL(CurrentFileCount, 0) + 1
	WHERE StorageGuid = @StorageGuid;

	IF (@UpdateTransferLog = 1) EXEC dbo.UpdateTransferLog @FileUniqueId, 0;

	COMMIT;
END