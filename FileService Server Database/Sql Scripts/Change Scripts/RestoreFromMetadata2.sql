USE [FileService] -- Put the database name of FileService there.

DECLARE @ApplicationGuid uniqueidentifier, @StorageGuid uniqueidentifier
	, @OrganizationGuid uniqueidentifier, @DepartmentGuid uniqueidentifier, @DepartmentName nvarchar(255)
	, @FileUniqueId nvarchar(32), @Name nvarchar(255), @SizeInBytes int, @UpdatedTime datetime, @Deleted bit
	, @FileExtensionGuid uniqueidentifier, @FileExtension nvarchar(255)

SET @ApplicationGuid = '{E17AC694-1838-4ED2-940E-B6253FF86267}'

DECLARE FilesCursor CURSOR FOR 
SELECT f.FileUniqueId, f.OrganizationId, f.DepartmentId, f.Name, f.SizeInBytes, f.UpdatedTime, f.Deleted, i.Name AS InstanceName
FROM BWA_HelpDesk_1.mfs.[File] AS f -- Replace the database name: BWA_HelpDesk_1, BWA_HelpDesk_FQ.
INNER JOIN BWA_HelpDesk_1.dbo.Mc_Instance AS i
	ON f.DepartmentId = i.InstanceId AND f.OrganizationId = i.InstanceId

OPEN FilesCursor

FETCH NEXT FROM FilesCursor 
INTO @FileUniqueId, @OrganizationGuid, @DepartmentGuid, @Name, @SizeInBytes, @UpdatedTime, @Deleted, @DepartmentName

WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @ErrorCode int

	EXECUTE dbo.UpdateOrganizationDepartment @ApplicationGuid, @DepartmentName, @DepartmentName, @OrganizationGuid OUTPUT, @DepartmentGuid OUTPUT, @ErrorCode OUTPUT
	
	IF @ErrorCode = 0
	BEGIN
		SELECT @FileExtension = SUBSTRING(@Name, LEN(@Name) - CHARINDEX('.', REVERSE(@Name)) + 1, LEN(@Name))
		
		IF LEN(@FileExtension) < 9 -- Like ".config", ".sitemap", etc., icluding "." character.
			SELECT @Name = SUBSTRING(@Name, 0, LEN(@Name) - CHARINDEX('.', REVERSE(@Name)) + 1)
		ELSE
			SET @FileExtension = ''			

		SELECT @FileExtensionGuid = FileExtensionGuid FROM dbo.FileExtension WHERE FileExtension = @FileExtension
		IF @FileExtensionGuid IS NULL
		BEGIN
			SET @FileExtensionGuid = NEWID()
	
			INSERT INTO dbo.FileExtension (FileExtensionGuid, FileExtension, MimeType)
			VALUES (@FileExtensionGuid, @FileExtension, '') -- MimeType should be populated by console tool.
		END
		
		IF @OrganizationGuid = '{10000E66-A468-45EA-90DD-7FE892992613}' -- First Quality
			SET @StorageGuid = '{56684B69-DAEC-4DC5-BD1A-BADF4D96C810}'
		ELSE
			SET @StorageGuid = '{A9C76220-52F5-4277-B3FB-79ACBD078806}'
	
		EXECUTE dbo.UpdateFile @FileUniqueId, NULL, @FileExtension, '', @ApplicationGuid, @StorageGuid, @OrganizationGuid, @DepartmentGuid
			, @Name, @SizeInBytes, NULL, NULL, NULL, 1, @Deleted, 0, @ErrorCode OUTPUT
	END

	FETCH NEXT FROM FilesCursor 
	INTO @FileUniqueId, @OrganizationGuid, @DepartmentGuid, @Name, @SizeInBytes, @UpdatedTime, @Deleted, @DepartmentName
END

CLOSE FilesCursor
DEALLOCATE FilesCursor
GO
