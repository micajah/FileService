CREATE TABLE [dbo].[FileExtension] (
    [FileExtensionGuid] UNIQUEIDENTIFIER NOT NULL,
    [FileExtension]     NVARCHAR (255)   NOT NULL,
    [MimeType]          NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_FileExtension] PRIMARY KEY CLUSTERED ([FileExtensionGuid] ASC)
);

