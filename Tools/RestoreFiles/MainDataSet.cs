using System;
using System.Globalization;

namespace Micajah.FileService.Tools.RestoreFiles
{
    partial class MainDataSet
    {
        partial class FileRow
        {
            #region Public Properties

            // Copied from dal.micajah.fileservice project.
            public string FilePath
            {
                get
                {
                    string filePath = this.StoragePath;
                    if (!filePath.EndsWith("\\", StringComparison.OrdinalIgnoreCase)) filePath += "\\";
                    if (this.FileUniqueId.Length > 1)
                        filePath += string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\", this.FileUniqueId[0], this.FileUniqueId[1]);
                    filePath += this.FileUniqueId;
                    return filePath;
                }
            }

            #endregion
        }
    }
}
