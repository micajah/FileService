using System;
using System.Globalization;

namespace Micajah.FileService.Tools.UpdateChecksum
{
    public partial class MainDataSet
    {
        partial class FilesViewRow
        {
            #region Public Properties

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
