CREATE TABLE [dbo].[Mfs_File] (
    [FileUniqueId]    NVARCHAR (32)    NOT NULL,
    [OrganizationId]  UNIQUEIDENTIFIER NOT NULL,
    [DepartmentId]    UNIQUEIDENTIFIER NOT NULL,
    [LocalObjectType] NVARCHAR (50)    NOT NULL,
    [LocalObjectId]   NVARCHAR (255)   NOT NULL,
    [Name]            NVARCHAR (255)   NOT NULL,
    [SizeInBytes]     INT              NOT NULL,
    [UpdatedTime]     DATETIME         NOT NULL,
    [UpdatedBy]       NVARCHAR (255)   NULL,
    [Deleted]         BIT              NOT NULL,
    CONSTRAINT [PK_Mfs_File] PRIMARY KEY CLUSTERED ([OrganizationId] ASC, [DepartmentId] ASC, [FileUniqueId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Mfs_File_1]
    ON [dbo].[Mfs_File]([OrganizationId] ASC, [DepartmentId] ASC, [LocalObjectId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Mfs_File_2]
    ON [dbo].[Mfs_File]([DepartmentId] ASC, [LocalObjectId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Mfs_File_3]
    ON [dbo].[Mfs_File]([FileUniqueId] ASC);

