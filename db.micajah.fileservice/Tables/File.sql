﻿CREATE TABLE [dbo].[File] (
    [FileUniqueId]       NVARCHAR (32)    NOT NULL,
    [ParentFileUniqueId] NVARCHAR (32)    NULL,
    [FileExtensionGuid]  UNIQUEIDENTIFIER NOT NULL,
    [ApplicationGuid]    UNIQUEIDENTIFIER NOT NULL,
    [StorageGuid]        UNIQUEIDENTIFIER NOT NULL,
    [DepartmentGuid]     UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (255)   NOT NULL,
    [SizeInBytes]        INT              NOT NULL,
    [Height]             INT              NULL,
    [Width]              INT              NULL,
    [Align]              INT              NULL,
    [ExpirationRequired] BIT              CONSTRAINT [DF_File_ExpirationRequired] DEFAULT ((1)) NOT NULL,
    [CreatedTime]        DATETIME         NOT NULL,
    [UpdatedTime]        DATETIME         NOT NULL,
    [TemporaryGuid]      UNIQUEIDENTIFIER NULL,
    [Deleted]            BIT              CONSTRAINT [DF_File_Deleted] DEFAULT ((0)) NOT NULL,
    [Checksum] VARCHAR(32) NULL, 
    CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([FileUniqueId] ASC),
    CONSTRAINT [FK_File_Application] FOREIGN KEY ([ApplicationGuid]) REFERENCES [dbo].[Application] ([ApplicationGuid]),
    CONSTRAINT [FK_File_Department] FOREIGN KEY ([DepartmentGuid]) REFERENCES [dbo].[Department] ([DepartmentGuid]),
    CONSTRAINT [FK_File_File] FOREIGN KEY ([ParentFileUniqueId]) REFERENCES [dbo].[File] ([FileUniqueId]),
    CONSTRAINT [FK_File_FileExtension] FOREIGN KEY ([FileExtensionGuid]) REFERENCES [dbo].[FileExtension] ([FileExtensionGuid]),
    CONSTRAINT [FK_File_Storage] FOREIGN KEY ([StorageGuid]) REFERENCES [dbo].[Storage] ([StorageGuid])
);


GO

CREATE INDEX [IX_File_ParentFileUniqueId] ON [dbo].[File] ([ParentFileUniqueId])
