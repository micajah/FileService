CREATE TABLE [dbo].[Department] (
    [DepartmentGuid]   UNIQUEIDENTIFIER CONSTRAINT [DF_Department_DepartmentGuid] DEFAULT (newid()) NOT NULL,
    [OrganizationGuid] UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NOT NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentGuid] ASC),
    CONSTRAINT [FK_Department_Organization] FOREIGN KEY ([OrganizationGuid]) REFERENCES [dbo].[Organization] ([OrganizationGuid])
);

