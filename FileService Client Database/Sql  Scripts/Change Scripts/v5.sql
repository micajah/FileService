IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mfs_File]') AND type in (N'U'))
DROP TABLE [dbo].[Mfs_File]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mfs_File](
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
 CONSTRAINT [PK_Mfs_File] PRIMARY KEY CLUSTERED 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[FileUniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[Mfs_File] ([FileUniqueId], [OrganizationId], [DepartmentId], [LocalObjectType], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted])
SELECT [FileUniqueId], [OrganizationId], [DepartmentId], [LocalObjectType], [LocalObjectId], [Name], [SizeInBytes], [UpdatedTime], [UpdatedBy], [Deleted]
FROM [mfs].[File]
GO

CREATE NONCLUSTERED INDEX [IX_Mfs_File_1] ON [dbo].[Mfs_File] 
(
	[OrganizationId] ASC,
	[DepartmentId] ASC,
	[LocalObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Mfs_File_2] ON [dbo].[Mfs_File] 
(
	[DepartmentId] ASC,
	[LocalObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Mfs_File_3] ON [dbo].[Mfs_File] 
(
	[FileUniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[mfs].[File]') AND type in (N'U'))
DROP TABLE [mfs].[File]
GO

IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'mfs')
DROP SCHEMA [mfs]
GO
