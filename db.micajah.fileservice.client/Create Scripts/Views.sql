IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[Mfs_FilesView]') AND OBJECTPROPERTY(id, N'IsView') = 1)
DROP VIEW [dbo].[Mfs_FilesView]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Mfs_FilesView]
AS
	SELECT f.FileUniqueId, f.OrganizationId, f.DepartmentId, f.LocalObjectTypeId, f.LocalObjectId, f.[Name], f.SizeInBytes, f.UpdatedTime, f.UpdatedBy, f.Deleted, t.[Name] AS LocalObjectTypeName
	FROM dbo.Mfs_File AS f
	INNER JOIN Mfs_LocalObjectType AS t
		ON	f.LocalObjectTypeId = t.LocalObjectTypeId
GO