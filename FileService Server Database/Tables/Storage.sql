CREATE TABLE [dbo].[Storage] (
    [StorageGuid]        UNIQUEIDENTIFIER CONSTRAINT [DF_Storage_StorageGuid] DEFAULT (newid()) NOT NULL,
    [Path]               NVARCHAR (3000)  NOT NULL,
    [MaxSizeInMB]        DECIMAL (18, 4)  NULL,
    [CurrentSizeInBytes] BIGINT           CONSTRAINT [DF_Storage_CurrentSizeInBytes] DEFAULT ((0)) NOT NULL,
    [MaxFileCount]       INT              NULL,
    [CurrentFileCount]   INT              CONSTRAINT [DF_Storage_CurrentFileCount] DEFAULT ((0)) NOT NULL,
    [OrganizationId]     UNIQUEIDENTIFIER NULL,
    [Active]             BIT              CONSTRAINT [DF_Storage_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED ([StorageGuid] ASC),
    CONSTRAINT [FK_Storage_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([OrganizationGuid])
);

