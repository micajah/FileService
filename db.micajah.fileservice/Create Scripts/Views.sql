IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FilesView]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[FilesView]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[FilesView]
AS
	SELECT f.FileUniqueId, f.ParentFileUniqueId, f.FileExtensionGuid, f.ApplicationGuid, f.StorageGuid, f.DepartmentGuid, f.[Name], f.SizeInBytes, f.Height, f.Width, f.Align, f.ExpirationRequired
		, f.CreatedTime, f.UpdatedTime, f.TemporaryGuid, f.Deleted, fe.FileExtension, fe.MimeType, (s.[Path] + CASE WHEN RIGHT(s.[Path], 1) = '\' THEN '' ELSE '\' END) AS StoragePath, d.OrganizationGuid
	FROM dbo.[File] AS f
	INNER JOIN dbo.FileExtension AS fe
		ON	f.FileExtensionGuid = fe.FileExtensionGuid
	INNER JOIN dbo.Storage AS s
		ON	f.StorageGuid = s.StorageGuid
	INNER JOIN dbo.Department AS d
		ON	f.DepartmentGuid = d.DepartmentGuid
GO
