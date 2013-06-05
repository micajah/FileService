CREATE TABLE [dbo].[Organization] (
    [OrganizationGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_Organization_OrganizationGuid] DEFAULT (newid()) NOT NULL,
    [ApplicationGuid]  UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    [BWAAccountId]     INT              NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED ([OrganizationGuid] ASC),
    CONSTRAINT [FK_Organization_Application] FOREIGN KEY ([ApplicationGuid]) REFERENCES [dbo].[Application] ([ApplicationGuid])
);

