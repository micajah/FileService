IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'mfs')
DROP SCHEMA [mfs]
GO

CREATE SCHEMA [mfs] AUTHORIZATION [dbo]
GO

EXEC sys.sp_addextendedproperty @name=N'Version', @value=N'43' , @level0type=N'SCHEMA',@level0name=N'mfs'
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mfs].[File]') AND type in (N'U'))
DROP TABLE [mfs].[File]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [mfs].[File](
	[FileUniqueId] [nvarchar](32) NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[DepartmentId] [uniqueidentifier] NOT NULL,
	[LocalObjectType] [nvarchar](50) NOT NULL,
	[LocalObjectId] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SizeInBytes] [int] NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[FileUniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE NONCLUSTERED INDEX [IX_File_1] ON [mfs].[File] 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[LocalObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [IX_File_2] ON [mfs].[File] 
(
	[DepartmentId] ASC,
	[LocalObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_File_3] ON [mfs].[File] 
(
	[FileUniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

INSERT INTO [mfs].[File] ([FileUniqueId], [OrganizationId], [DepartmentId], [LocalObjectType], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted])
SELECT [FileUniqueId],[OrganizationId], [DepartmentId], [LocalObjectTypeName], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted]
FROM [dbo].[Mfs_FilesView]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_DeleteFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Mfs_DeleteFile]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_GetDeletedFiles]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Mfs_GetDeletedFiles]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_GetFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Mfs_GetFile]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_GetFiles]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Mfs_GetFiles]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_UpdateFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Mfs_UpdateFile]
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_FilesView]'))
DROP VIEW [dbo].[Mfs_FilesView]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Mfs_File_Mfs_LocalObjectType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Mfs_File]'))
ALTER TABLE [dbo].[Mfs_File] DROP CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Mfs_LocalObjectType_LocalObjectTypeGuid]') AND type = 'D')
ALTER TABLE [dbo].[Mfs_LocalObjectType] DROP CONSTRAINT [DF_Mfs_LocalObjectType_LocalObjectTypeGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_File]') AND type in (N'U'))
DROP TABLE [dbo].[Mfs_File]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_LocalObjectType]') AND type in (N'U'))
DROP TABLE [dbo].[Mfs_LocalObjectType]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_Version]') AND type in (N'U'))
DROP TABLE [dbo].[Mfs_Version]
GO
