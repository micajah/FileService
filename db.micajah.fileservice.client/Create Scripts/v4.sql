IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'mfs')
DROP SCHEMA [mfs]
GO

CREATE SCHEMA [mfs] AUTHORIZATION [dbo]
GO

EXEC sys.sp_addextendedproperty @name=N'Version', @value=N'4' , @level0type=N'SCHEMA',@level0name=N'mfs'
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
