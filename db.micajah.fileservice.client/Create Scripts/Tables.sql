IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Mfs_File_Mfs_LocalObjectType]') AND type = 'F')
ALTER TABLE [dbo].[Mfs_File] DROP CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Mfs_LocalObjectType_LocalObjectTypeGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Mfs_LocalObjectType] DROP CONSTRAINT [DF_Mfs_LocalObjectType_LocalObjectTypeGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_File]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Mfs_File]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_LocalObjectType]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Mfs_LocalObjectType]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_Version]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Mfs_Version]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mfs_File](
	[FileUniqueId] [nvarchar](32) NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[DepartmentId] [uniqueidentifier] NOT NULL,
	[LocalObjectTypeId] [uniqueidentifier] NOT NULL,
	[LocalObjectId] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SizeInBytes] [int] NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Mfs_File] PRIMARY KEY CLUSTERED 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[FileUniqueId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE NONCLUSTERED INDEX [IX_Mfs_File_1] ON [dbo].[Mfs_File] 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[LocalObjectId] ASC
) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [IX_Mfs_File_2] ON [dbo].[Mfs_File] 
(
	[DepartmentId] ASC,
	[LocalObjectId] ASC
) ON [PRIMARY]
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_Mfs_File_3] ON [dbo].[Mfs_File] 
(
	[FileUniqueId] ASC
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mfs_LocalObjectType](
	[LocalObjectTypeId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Mfs_LocalObjectType] PRIMARY KEY CLUSTERED 
(
	[LocalObjectTypeId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mfs_Version](
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Mfs_Version] PRIMARY KEY CLUSTERED 
(
	[Version] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Mfs_File]  WITH CHECK ADD  CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType] FOREIGN KEY([LocalObjectTypeId])
REFERENCES [dbo].[Mfs_LocalObjectType] ([LocalObjectTypeId])
GO

ALTER TABLE [dbo].[Mfs_File] CHECK CONSTRAINT [FK_Mfs_File_Mfs_LocalObjectType]
GO

ALTER TABLE [dbo].[Mfs_LocalObjectType] ADD  CONSTRAINT [DF_Mfs_LocalObjectType_LocalObjectTypeGuid]  DEFAULT (newid()) FOR [LocalObjectTypeId]
GO
