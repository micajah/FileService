CREATE TABLE [dbo].[TransferLog] (
    [TransferLogGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_TransferLog_TransferLogGuid] DEFAULT (newid()) NOT NULL,
    [FileUniqueId]    NVARCHAR (32)    NOT NULL,
    [DownloadCount]   BIGINT           CONSTRAINT [DF_TransferLog_intDownloadCount] DEFAULT ((0)) NOT NULL,
    [SizeInBytes]     INT              NOT NULL,
    [IsDownload]      BIT              CONSTRAINT [DF_TransferLog_IsDownload] DEFAULT ((1)) NOT NULL,
    [MonthAudit]      DATETIME         NOT NULL,
    CONSTRAINT [PK_TransferLog] PRIMARY KEY CLUSTERED ([TransferLogGuid] ASC),
    CONSTRAINT [FK_TransferLog_File] FOREIGN KEY ([FileUniqueId]) REFERENCES [dbo].[File] ([FileUniqueId])
);

GO

CREATE INDEX [IX_TransferLog_FileUniqueId] ON [dbo].[TransferLog] ([FileUniqueId] ASC)
GO

CREATE INDEX [IX_TransferLog_FileUniqueId_SizeInBytes_IsDownload_MonthAudit] ON [dbo].[TransferLog] ([FileUniqueId], [SizeInBytes], [IsDownload], [MonthAudit])
