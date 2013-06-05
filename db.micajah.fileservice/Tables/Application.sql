CREATE TABLE [dbo].[Application] (
    [ApplicationGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_Application_ApplicationGuid] DEFAULT (newid()) NOT NULL,
    [VendorGuid]      UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ApplicationGuid] ASC),
    CONSTRAINT [FK_Application_Vendor] FOREIGN KEY ([VendorGuid]) REFERENCES [dbo].[Vendor] ([VendorGuid])
);

