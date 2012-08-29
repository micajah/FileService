--  Use the SQL statements below to insert the required data for test application
INSERT INTO [dbo].[Vendor]([VendorGuid], [Name]) 
VALUES ('{E66C01E9-6528-4959-927D-966240E7F81F}', N'test vendor')
GO

INSERT INTO [dbo].[Application]([ApplicationGuid], [VendorGuid], [Name]) 
VALUES ('{9CE94487-0765-4AB8-B427-9CC3C5FB630A}', '{E66C01E9-6528-4959-927D-966240E7F81F}', N'test application')
GO

INSERT INTO [dbo].[Organization]([OrganizationGuid], [ApplicationGuid], [Name], [BWAAccountId]) 
VALUES ('{F53995A0-2B8C-4C16-8BE4-476495220EA6}', '{9CE94487-0765-4AB8-B427-9CC3C5FB630A}', N'test organization', NULL)
GO

INSERT INTO [dbo].[Department]([DepartmentGuid], [OrganizationGuid], [Name]) 
VALUES ('{96CA7B84-BDF7-4AF2-B78C-29E60F6569FE}', '{F53995A0-2B8C-4C16-8BE4-476495220EA6}', N'test organization')
GO

INSERT INTO [dbo].[Storage] ([StorageGuid], [Path], [MaxSizeInMB], [CurrentSizeInBytes], [MaxFileCount], [CurrentFileCount], OrganizationId, [Active])
VALUES ('{989B7E8F-41A2-4022-A1B6-2F8C702E585C}', 'C:\WebSites\MITS_FileService_Beta\Storages\Default\', NULL, 0, NULL, '{F53995A0-2B8C-4C16-8BE4-476495220EA6}' 0, 1)
GO
