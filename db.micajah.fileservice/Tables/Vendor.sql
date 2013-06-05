CREATE TABLE [dbo].[Vendor] (
    [VendorGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_Vendor_VendorGuid] DEFAULT (newid()) NOT NULL,
    [Name]       NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED ([VendorGuid] ASC)
);

