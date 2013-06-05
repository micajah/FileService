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