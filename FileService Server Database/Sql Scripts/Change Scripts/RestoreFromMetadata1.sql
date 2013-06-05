-- Registers bigWebApps vendor.
INSERT INTO [dbo].[Vendor]([VendorGuid], [Name]) 
VALUES ('{C801463A-D745-49DC-94D4-C5480957E5D6}', N'bigwebapps')
GO

-- Registers HelpDesk application.
INSERT INTO [dbo].[Application]([ApplicationGuid], [VendorGuid], [Name]) 
VALUES ('{E17AC694-1838-4ED2-940E-B6253FF86267}', '{C801463A-D745-49DC-94D4-C5480957E5D6}', N'helpdesk')
GO

-- Registers First Quality organization.
INSERT INTO [dbo].[Organization]([OrganizationGuid], [ApplicationGuid], [Name], [BWAAccountId]) 
VALUES ('{10000E66-A468-45EA-90DD-7FE892992613}', '{E17AC694-1838-4ED2-940E-B6253FF86267}', N'help desk', NULL)
GO

INSERT INTO [dbo].[Department]([DepartmentGuid], [OrganizationGuid], [Name]) 
VALUES ('{10000E66-A468-45EA-90DD-7FE892992613}', '{10000E66-A468-45EA-90DD-7FE892992613}', N'help desk')
GO

-- Registers private storage for First Quality organization.
INSERT INTO [dbo].[Storage] ([StorageGuid], [Path], [MaxSizeInMB], [CurrentSizeInBytes], [MaxFileCount], [CurrentFileCount], OrganizationId, [Active])
VALUES ('{56684B69-DAEC-4DC5-BD1A-BADF4D96C810}', 'D:\FileServiceRepositories\PrivateRepo1\', NULL, 0, NULL, 0, '{10000E66-A468-45EA-90DD-7FE892992613}', 1)
GO

-- Registers the storage for all the rest organizations.
INSERT INTO [dbo].[Storage] ([StorageGuid], [Path], [MaxSizeInMB], [CurrentSizeInBytes], [MaxFileCount], [CurrentFileCount], OrganizationId, [Active])
VALUES ('{A9C76220-52F5-4277-B3FB-79ACBD078806}', 'D:\FileServiceRepositories\Repo1\', NULL, 0, NULL, 0, NULL, 1)
GO

-- Registers the empty file extension.
INSERT INTO dbo.FileExtension (FileExtensionGuid, FileExtension, MimeType)
VALUES ('{9785AFC7-5A80-4556-A934-CD391D8F5B6A}', '', 'text/plain')
GO