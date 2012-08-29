using System;
using System.Data.SqlClient;

namespace Micajah.FileService.Client.Dal
{
    partial class MetaDataSet
    {
        partial class FileDataTable
        {
            #region Members

            public const string FileUniqueIdColumnName = "FileUniqueId";
            public const string NameColumnName = "Name";
            public const string UpdatedTimeColumnName = "UpdatedTime";
            public const string UpdatedDateColumnName = "UpdatedDate";
            public const int FileUniqueIdColumnMaxLength = 32;

            #endregion
        }

        partial class FileRow
        {
            #region Members

            private bool m_IsTemporary;

            #endregion

            #region Public Properties

            public bool IsTemporary
            {
                get { return m_IsTemporary; }
                set { m_IsTemporary = value; }
            }

            #endregion
        }
    }
}

namespace Micajah.FileService.Client.Dal.MetaDataSetTableAdapters
{
    partial class FileTableAdapter
    {
        #region Constructors

        public FileTableAdapter(string connectionString)
        {
            this.Connection = new SqlConnection(connectionString);
        }

        #endregion

        #region Public Methods

        public MetaDataSet.FileDataTable GetFiles(Guid organizationId, Guid departmentId)
        {
            return this.GetFiles(organizationId, departmentId, null, null, false);
        }

        public MetaDataSet.FileRow GetFile(Guid organizationId, Guid departmentId, string localObjectType, string localObjectId)
        {
            MetaDataSet.FileDataTable table = this.GetFiles(organizationId, departmentId, localObjectType, localObjectId, false);
            return ((table.Count > 0) ? table[0] : null);
        }

        public void MarkFileAsDeleted(string fileUniqueId)
        {
            MarkFileAsDeleted(fileUniqueId, null);
        }

        public void MarkFileAsDeleted(string fileUniqueId, string updatedBy)
        {
            using (MetaDataSet.FileDataTable table = this.GetFile(fileUniqueId))
            {
                if (table.Count > 0)
                {
                    MetaDataSet.FileRow row = table[0];

                    if (updatedBy == null)
                        row.SetUpdatedByNull();
                    else
                        row.UpdatedBy = updatedBy;
                    row.Deleted = true;

                    this.Update(row);
                }
            }
        }

        public void Insert(string fileUniqueId, string organizationId, string departmentId, string localObjectType, string localObjectId
            , string name, int sizeInBytes, string updatedBy)
        {
            this.Insert(fileUniqueId, Support.CreateGuid(organizationId), Support.CreateGuid(departmentId), localObjectType, localObjectId
                , name, sizeInBytes, updatedBy, false);
        }

        #endregion
    }
}
