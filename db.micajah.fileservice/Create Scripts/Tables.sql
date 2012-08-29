IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Application_Vendor]') AND type = 'F')
ALTER TABLE [dbo].[Application] DROP CONSTRAINT [FK_Application_Vendor]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Application_ApplicationGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Application] DROP CONSTRAINT [DF_Application_ApplicationGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Department_Organization]') AND type = 'F')
ALTER TABLE [dbo].[Department] DROP CONSTRAINT [FK_Department_Organization]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Department_DepartmentGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Department] DROP CONSTRAINT [DF_Department_DepartmentGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_File_Application]') AND type = 'F')
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Application]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_File_Department]') AND type = 'F')
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Department]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_File_File]') AND type = 'F')
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_File]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_File_FileExtension]') AND type = 'F')
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_FileExtension]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_File_Storage]') AND type = 'F')
ALTER TABLE [dbo].[File] DROP CONSTRAINT [FK_File_Storage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_File_ExpirationRequired]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[File] DROP CONSTRAINT [DF_File_ExpirationRequired]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_File_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[File] DROP CONSTRAINT [DF_File_Deleted]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Organization_Application]') AND type = 'F')
ALTER TABLE [dbo].[Organization] DROP CONSTRAINT [FK_Organization_Application]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Organization_OrganizationGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Organization] DROP CONSTRAINT [DF_Organization_OrganizationGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_Storage_Organization]') AND type = 'F')
ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [FK_Storage_Organization]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Storage_StorageGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [DF_Storage_StorageGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Storage_CurrentSizeInBytes]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [DF_Storage_CurrentSizeInBytes]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Storage_CurrentFileCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [DF_Storage_CurrentFileCount]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Storage_Active]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Storage] DROP CONSTRAINT [DF_Storage_Active]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_TransferLog_File]') AND type = 'F')
ALTER TABLE [dbo].[TransferLog] DROP CONSTRAINT [FK_TransferLog_File]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TransferLog_TransferLogGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TransferLog] DROP CONSTRAINT [DF_TransferLog_TransferLogGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TransferLog_intDownloadCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TransferLog] DROP CONSTRAINT [DF_TransferLog_intDownloadCount]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TransferLog_IsDownload]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TransferLog] DROP CONSTRAINT [DF_TransferLog_IsDownload]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Vendor_VendorGuid]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Vendor] DROP CONSTRAINT [DF_Vendor_VendorGuid]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Application]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Application]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Department]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Department]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[File]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[File]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FileExtension]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[FileExtension]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Organization]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Organization]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Storage]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Storage]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[TransferLog]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[TransferLog]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Vendor]') AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Vendor]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Application](
	[ApplicationGuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[VendorGuid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED 
(
	[ApplicationGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Department](
	[DepartmentGuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[OrganizationGuid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[DepartmentGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[File](
	[FileUniqueId] [nvarchar](32) NOT NULL,
	[ParentFileUniqueId] [nvarchar](32) NULL,
	[FileExtensionGuid] [uniqueidentifier] NOT NULL,
	[ApplicationGuid] [uniqueidentifier] NOT NULL,
	[StorageGuid] [uniqueidentifier] NOT NULL,
	[DepartmentGuid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SizeInBytes] [int] NOT NULL,
	[Height] [int] NULL,
	[Width] [int] NULL,
	[Align] [int] NULL,
	[ExpirationRequired] [bit] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
	[TemporaryGuid] [uniqueidentifier] NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[FileUniqueId] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FileExtension](
	[FileExtensionGuid] [uniqueidentifier] NOT NULL,
	[FileExtension] [nvarchar](255) NOT NULL,
	[MimeType] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_FileExtension] PRIMARY KEY CLUSTERED 
(
	[FileExtensionGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Organization](
	[OrganizationGuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ApplicationGuid] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[BWAAccountId] [int] NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[OrganizationGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Storage](
	[StorageGuid] [uniqueidentifier] NOT NULL,
	[Path] [nvarchar](3000) NOT NULL,
	[MaxSizeInMB] [decimal](18, 4) NULL,
	[CurrentSizeInBytes] [bigint] NOT NULL,
	[MaxFileCount] [int] NULL,
	[CurrentFileCount] [int] NOT NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED 
(
	[StorageGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TransferLog](
	[TransferLogGuid] [uniqueidentifier] NOT NULL,
	[FileUniqueId] [nvarchar](32) NOT NULL,
	[DownloadCount] [bigint] NOT NULL,
	[SizeInBytes] [int] NOT NULL,
	[IsDownload] [bit] NOT NULL,
	[MonthAudit] [datetime] NOT NULL,
 CONSTRAINT [PK_TransferLog] PRIMARY KEY CLUSTERED 
(
	[TransferLogGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Vendor](
	[VendorGuid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
	[VendorGuid] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Application]  WITH CHECK ADD  CONSTRAINT [FK_Application_Vendor] FOREIGN KEY([VendorGuid])
REFERENCES [dbo].[Vendor] ([VendorGuid])
GO

ALTER TABLE [dbo].[Application] CHECK CONSTRAINT [FK_Application_Vendor]
GO

ALTER TABLE [dbo].[Application] ADD  CONSTRAINT [DF_Application_ApplicationGuid]  DEFAULT (newid()) FOR [ApplicationGuid]
GO

ALTER TABLE [dbo].[Department]  WITH CHECK ADD  CONSTRAINT [FK_Department_Organization] FOREIGN KEY([OrganizationGuid])
REFERENCES [dbo].[Organization] ([OrganizationGuid])
GO

ALTER TABLE [dbo].[Department] CHECK CONSTRAINT [FK_Department_Organization]
GO

ALTER TABLE [dbo].[Department] ADD  CONSTRAINT [DF_Department_DepartmentGuid]  DEFAULT (newid()) FOR [DepartmentGuid]
GO

ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Application] FOREIGN KEY([ApplicationGuid])
REFERENCES [dbo].[Application] ([ApplicationGuid])
GO

ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Application]
GO

ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Department] FOREIGN KEY([DepartmentGuid])
REFERENCES [dbo].[Department] ([DepartmentGuid])
GO

ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Department]
GO

ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_File] FOREIGN KEY([ParentFileUniqueId])
REFERENCES [dbo].[File] ([FileUniqueId])
GO

ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_File]
GO

ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_FileExtension] FOREIGN KEY([FileExtensionGuid])
REFERENCES [dbo].[FileExtension] ([FileExtensionGuid])
GO

ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_FileExtension]
GO

ALTER TABLE [dbo].[File]  WITH CHECK ADD  CONSTRAINT [FK_File_Storage] FOREIGN KEY([StorageGuid])
REFERENCES [dbo].[Storage] ([StorageGuid])
GO

ALTER TABLE [dbo].[File] CHECK CONSTRAINT [FK_File_Storage]
GO

ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_ExpirationRequired]  DEFAULT ((1)) FOR [ExpirationRequired]
GO

ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [dbo].[Organization]  WITH CHECK ADD  CONSTRAINT [FK_Organization_Application] FOREIGN KEY([ApplicationGuid])
REFERENCES [dbo].[Application] ([ApplicationGuid])
GO

ALTER TABLE [dbo].[Organization] CHECK CONSTRAINT [FK_Organization_Application]
GO

ALTER TABLE [dbo].[Organization] ADD  CONSTRAINT [DF_Organization_OrganizationGuid]  DEFAULT (newid()) FOR [OrganizationGuid]
GO

ALTER TABLE [dbo].[Storage]  WITH CHECK ADD  CONSTRAINT [FK_Storage_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([OrganizationGuid])
GO

ALTER TABLE [dbo].[Storage] CHECK CONSTRAINT [FK_Storage_Organization]
GO

ALTER TABLE [dbo].[Storage] ADD  CONSTRAINT [DF_Storage_StorageGuid]  DEFAULT (newid()) FOR [StorageGuid]
GO

ALTER TABLE [dbo].[Storage] ADD  CONSTRAINT [DF_Storage_CurrentSizeInBytes]  DEFAULT ((0)) FOR [CurrentSizeInBytes]
GO

ALTER TABLE [dbo].[Storage] ADD  CONSTRAINT [DF_Storage_CurrentFileCount]  DEFAULT ((0)) FOR [CurrentFileCount]
GO

ALTER TABLE [dbo].[Storage] ADD  CONSTRAINT [DF_Storage_Active]  DEFAULT ((1)) FOR [Active]
GO

ALTER TABLE [dbo].[TransferLog]  WITH CHECK ADD  CONSTRAINT [FK_TransferLog_File] FOREIGN KEY([FileUniqueId])
REFERENCES [dbo].[File] ([FileUniqueId])
GO

ALTER TABLE [dbo].[TransferLog] CHECK CONSTRAINT [FK_TransferLog_File]
GO

ALTER TABLE [dbo].[TransferLog] ADD  CONSTRAINT [DF_TransferLog_TransferLogGuid]  DEFAULT (newid()) FOR [TransferLogGuid]
GO

ALTER TABLE [dbo].[TransferLog] ADD  CONSTRAINT [DF_TransferLog_intDownloadCount]  DEFAULT ((0)) FOR [DownloadCount]
GO

ALTER TABLE [dbo].[TransferLog] ADD  CONSTRAINT [DF_TransferLog_IsDownload]  DEFAULT ((1)) FOR [IsDownload]
GO

ALTER TABLE [dbo].[Vendor] ADD  CONSTRAINT [DF_Vendor_VendorGuid]  DEFAULT (newid()) FOR [VendorGuid]
GO
