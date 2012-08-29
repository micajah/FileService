using System;
using System.Globalization;
using System.Text;
using System.ComponentModel;

namespace Micajah.FileService.Dal
{
    partial class MainDataSet
    {
        partial class FileDataTable
        {
            #region Members

            public const int FileUniqueIdColumnMaxLength = 32;

            #endregion
        }

        partial class FileRow
        {
            #region Private Properties

            private string FileExtensionInternal
            {
                get { return (this.IsThumbnail ? ".jpg" : this.FileExtension); }
            }

            private string FileType
            {
                get { return (this.IsThumbnail ? "thumbnail" : "file"); }
            }

            private bool IsThumbnail
            {
                get
                {
                    int width = (this.IsWidthNull() ? 0 : this.Width);
                    int height = (this.IsHeightNull() ? 0 : this.Height);
                    return ((!((width == 0) && (height == 0))) && this.IsImage);
                }
            }

            private bool IsImage
            {
                get { return this.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase); }
            }

            #endregion

            #region Public Properties

            public string NameWithExtension
            {
                get { return (this.Name + this.FileExtensionInternal); }
            }

            public string FilePath
            {
                get
                {
                    string filePath = this.StoragePath;
                    if (!filePath.EndsWith("\\", StringComparison.OrdinalIgnoreCase)) filePath += "\\";
                    if (this.FileUniqueId.Length > 1)
                        filePath += string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\", this.FileUniqueId[0], this.FileUniqueId[1]);
                    int width = (this.IsWidthNull() ? 0 : this.Width);
                    int height = (this.IsHeightNull() ? 0 : this.Height);
                    filePath += this.FileUniqueId;
                    return filePath;
                }
            }

            #endregion
        }
    }
}

namespace Micajah.FileService.Dal.MainDataSetTableAdapters
{
    partial class DepartmentTableAdapter
    {
        #region Public Methods

        public void UpdateOrganizationDepartment(string applicationGuid, string organizationName, string departmentName, ref string organizationGuid, ref string departmentGuid, ref int errorCode)
        {
            Guid? orgGuid = Helper.CreateNullableGuid(organizationGuid);
            Guid? deptGuid = Helper.CreateNullableGuid(departmentGuid);

            this.UpdateOrganizationDepartment(Helper.CreateGuid(applicationGuid), organizationName, departmentName, ref orgGuid, ref deptGuid, ref errorCode);

            if (errorCode == 0)
            {
                if (orgGuid.HasValue) organizationGuid = orgGuid.Value.ToString();
                if (deptGuid.HasValue) departmentGuid = deptGuid.Value.ToString();
            }
        }

        #endregion
    }

    partial class FileTableAdapter
    {
        #region Public Methods

        public MainDataSet.FileDataTable GetFiles()
        {
            return this.GetFiles(null);
        }

        public string GetTemporaryFilesList(string temporaryGuid)
        {
            return this.GetTemporaryFilesList(Helper.CreateGuid(temporaryGuid));
        }

        public string GetTemporaryFilesList(Guid temporaryGuid)
        {
            if (temporaryGuid != Guid.Empty)
            {
                StringBuilder sb = new StringBuilder();

                using (MainDataSet.FileDataTable table = this.GetTemporaryFiles(temporaryGuid))
                {
                    foreach (MainDataSet.FileRow row in table)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0};", row.FileUniqueId);
                    }
                }

                return sb.ToString();
            }

            return string.Empty;
        }

        public string GetThumbnailFileUniqueId(string fileUniqueId, int? width, int? height, int align)
        {
            string thumbnailFileUniqueId = null;
            this.GetThumbnailFileUniqueId(fileUniqueId, width, height, align, ref thumbnailFileUniqueId);
            return thumbnailFileUniqueId;
        }

        public MainDataSet.FileDataTable GetTemporaryFilesForDelete()
        {
            return this.GetTemporaryFiles(null);
        }

        public int Update(MainDataSet.FileRow row)
        {
            int errorCode = 0;
            this.UpdateFile(row.FileUniqueId, (row.IsParentFileUniqueIdNull() ? null : row.ParentFileUniqueId), row.FileExtension, row.MimeType, row.ApplicationGuid, row.StorageGuid
                , row.OrganizationGuid, row.DepartmentGuid, row.Name, row.SizeInBytes
                , (row.IsWidthNull() ? null : new int?(row.Width)), (row.IsHeightNull() ? null : new int?(row.Height)), (row.IsAlignNull() ? null : new int?(row.Align))
                , row.ExpirationRequired, row.Deleted, false, ref errorCode);
            return errorCode;
        }

        public void UpdateFile(string fileUniqueId, string parentFileUniqueId, string fileExtension, string mimeType, string applicationGuid, string storageGuid
            , string organizationGuid, string departmentGuid, string name, int sizeInBytes, int? width, int? height, int? align, bool? expirationRequired, ref int errorCode)
        {
            this.UpdateFile(fileUniqueId, parentFileUniqueId, fileExtension, mimeType, Helper.CreateGuid(applicationGuid), Helper.CreateGuid(storageGuid)
            , Helper.CreateGuid(organizationGuid), Helper.CreateGuid(departmentGuid), name, sizeInBytes, width, height, align, expirationRequired, false, true, ref errorCode);
        }

        public void UpdateFileTemporaryGuid(string fileUniqueId, string temporaryGuid)
        {
            this.UpdateFileTemporaryGuid(fileUniqueId, Helper.CreateNullableGuid(temporaryGuid));
        }

        #endregion
    }

    partial class OrganizationTableAdapter
    {
        #region Public Methods

        public MainDataSet.OrganizationDataTable GetOrganizations()
        {
            return this.GetOrganizations(null);
        }

        #endregion
    }

    partial class StorageTableAdapter
    {
        #region Public Methods

        public MainDataSet.StorageDataTable GetStorages()
        {
            return this.GetStorages(null);
        }

        [DataObjectMethod(DataObjectMethodType.Insert)]
        public Guid InsertStorage(string path, decimal? maxSizeInMB, int? maxFileCount, Guid? organizationId, bool active)
        {
            MainDataSet.StorageDataTable table = new MainDataSet.StorageDataTable();
            table.StorageGuidColumn.AllowDBNull = true;
            MainDataSet.StorageRow row = table.NewStorageRow();

            row.Path = path;
            if (maxSizeInMB.HasValue)
                row.MaxSizeInMB = maxSizeInMB.Value;
            else
                row.SetMaxSizeInMBNull();
            if (maxFileCount.HasValue)
                row.MaxFileCount = maxFileCount.Value;
            else
                row.SetMaxFileCountNull();
            if (organizationId.HasValue)
                row.OrganizationId = organizationId.Value;
            else
                row.SetOrganizationIdNull();
            row.Active = active;

            table.AddStorageRow(row);

            this.Update(row);

            return row.StorageGuid;
        }

        public void ValidateStorage(string applicationGuid, string organizationGuid, string departmentGuid, string fileUniqueId, decimal sizeInMB, int filesCount
            , decimal nextFreeSpaceInMBIn, ref decimal? nextFreeSpaceInMBOut, ref Guid? storageGuid, ref string storagePath, ref int errorCode)
        {
            this.ValidateStorage(Helper.CreateGuid(applicationGuid), Helper.CreateGuid(organizationGuid), Helper.CreateGuid(departmentGuid), fileUniqueId, sizeInMB, filesCount
                , nextFreeSpaceInMBIn, ref nextFreeSpaceInMBOut, ref storageGuid, ref storagePath, ref errorCode);
        }

        #endregion
    }
}
