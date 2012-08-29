using System;
using System.Globalization;
using System.IO;
using System.Text;
using Micajah.FileService.Dal;
using Micajah.FileService.Dal.MainDataSetTableAdapters;

namespace Micajah.FileService.Management
{
    public partial class MasterPage : Micajah.Common.Pages.MasterPage
    {
        #region Private Methods

        private static void DeleteDirectories(string storagePath, string fileUniqueId)
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 2; index >= 0; index--)
            {
                sb.Append(storagePath);

                int newIndex = 0;
                while (newIndex <= index)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}\\", fileUniqueId[newIndex]);
                    newIndex++;
                }

                string directoryPath = sb.ToString();
                sb.Remove(0, sb.Length);

                if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath);
            }
        }

        private static string GetPathFromUniqueName(string fileUniqueId)
        {
            StringBuilder sb = new StringBuilder();

            if (fileUniqueId.Length > 3)
            {
                for (int index = 0; index < 3; index++)
                {
                    sb.AppendFormat("{0}\\", fileUniqueId[index]);
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Public Methods

        public static void MoveOrganizationToPrivateStorage(object data)
        {
            object[] p = (object[])data;
            Guid organizationGuid = (Guid)p[0];
            Guid storageGuid = (Guid)p[1];
            string storagePath = (string)p[2];
            if (!storagePath.EndsWith("\\")) storagePath += "\\";

            using (FileTableAdapter fileAdapter = new FileTableAdapter())
            {
                foreach (MainDataSet.FileRow fileRow in fileAdapter.GetFiles(organizationGuid))
                {
                    string relativePath = GetPathFromUniqueName(fileRow.FileUniqueId);
                    string directoryPath = storagePath + relativePath;
                    string sourceFileName = fileRow.StoragePath + relativePath + fileRow.FileUniqueId + fileRow.FileExtension;
                    bool moved = false;

                    if (File.Exists(sourceFileName))
                    {
                        try
                        {
                            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                            File.Move(sourceFileName, directoryPath + fileRow.FileUniqueId + fileRow.FileExtension);

                            moved = true;

                            DeleteDirectories(fileRow.StoragePath, fileRow.FileUniqueId);

                        }
                        catch (IOException) { }
                        catch (ArgumentException) { }
                        catch (UnauthorizedAccessException) { }
                        catch (NotSupportedException) { }
                    }

                    if (moved)
                    {
                        fileRow.StorageGuid = storageGuid;

                        fileAdapter.Update(fileRow);
                    }
                }
            }
        }

        #endregion
    }
}