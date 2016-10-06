using Micajah.FileService.Dal;
using Micajah.FileService.Dal.MainDataSetTableAdapters;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Micajah.FileService.WebService
{
    public static class FileManager
    {
        #region Private Methods

        private static string CalculateChecksum(Stream stream)
        {
            using (BufferedStream buffStream = new BufferedStream(stream))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] checksum = md5.ComputeHash(buffStream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLowerInvariant();
                }
            }
        }

        private static string CheckFileUrl(string fileUrl, ref string fileName)
        {
            if (!Uri.IsWellFormedUriString(fileUrl, UriKind.Absolute))
                return "Error: FileURL not well formed.";

            if (!fileUrl.ToUpperInvariant().Contains("HTTP://"))
                return "Error: FileURL support only http protocol.";

            if (fileUrl.Length > 0)
            {
                string _url = fileUrl;
                _url = _url.Substring(_url.IndexOf("://", StringComparison.OrdinalIgnoreCase) + 3);
                if (_url.Contains("/"))
                    fileName = _url.Substring(_url.LastIndexOf('/') + 1);
            }
            else
                return "Error: FileURL undefined.";

            if (fileName.Length == 0)
                return "Error: FileURL not contain file name.";

            if (Path.GetExtension(fileName).Length == 0)
                return "Error: FileURL not contain file extension.";

            return string.Empty;
        }

        private static string CreateFile(string applicationGuid, string organizationGuid, string departmentGuid, string fileName, string tempFileName, bool fileNameIsUrl, bool? expirationRequired, HttpPostedFile file, ref string checksum)
        {
            string result = string.Empty;

            try
            {
                string fileNameWithExtension = string.Empty;
                if (fileNameIsUrl)
                {
                    result = CheckFileUrl(fileName, ref fileNameWithExtension);
                    if (result.Length > 0) return result;
                }
                //else if (file != null)
                //    fileNameWithExtension = Path.GetFileName(file.FileName);
                else
                    fileNameWithExtension = fileName;

                string destinationFileName = string.Empty;
                string destinationFolder = string.Empty;
                string fileUniqueId = string.Empty;
                int sizeInBytes = 0;
                Guid? storageGuid = null;
                FileInfo fileInfo = null;

                if (fileNameIsUrl)
                    sizeInBytes = 1048576; // file size before unknown: by default request 1Mb
                else if (file != null)
                    sizeInBytes = file.ContentLength;
                else
                {
                    fileInfo = new FileInfo(tempFileName);
                    sizeInBytes = (int)fileInfo.Length;
                }

                int errorCode = GetAccess(applicationGuid, organizationGuid, departmentGuid, sizeInBytes
                    , ref destinationFolder, ref destinationFileName, ref fileUniqueId, ref storageGuid);

                if (errorCode == 0)
                {
                    if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                    if (fileNameIsUrl)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(fileName, destinationFileName);

                        fileInfo = new FileInfo(destinationFileName);
                        sizeInBytes = (int)fileInfo.Length;
                    }
                    else if (file != null)
                        file.SaveAs(destinationFileName);
                    else
                        File.Move(tempFileName, destinationFileName); // Move from temporary to real location

                    int? width = null;
                    int? height = null;
                    string fileExtension = Path.GetExtension(fileNameWithExtension);
                    string fileMimeType = MimeType.GetMimeType(fileExtension);

                    if (MimeType.IsFlash(fileMimeType))
                    {
                        SwfFileInfo swfFileInfo = new SwfFileInfo(destinationFileName);
                        if (swfFileInfo.IsValid)
                        {
                            width = swfFileInfo.Width;
                            height = swfFileInfo.Height;
                        }
                    }
                    else if (MimeType.IsImageType(fileMimeType))
                    {
                        RotateFlipImageByOrientation(destinationFileName, fileMimeType);
                    }

                    checksum = CalculateChecksum(destinationFileName);

                    using (FileTableAdapter adapter = new FileTableAdapter())
                    {
                        adapter.UpdateFile(fileUniqueId, null, fileExtension, fileMimeType
                            , applicationGuid, storageGuid.Value.ToString(), organizationGuid, departmentGuid
                            , Path.GetFileNameWithoutExtension(fileNameWithExtension), sizeInBytes, width, height, null, expirationRequired, checksum, ref errorCode);
                    }

                    if (errorCode != 0)
                    {
                        result = GetCreateError(errorCode);

                        if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                        DeleteDirectories(destinationFolder, fileUniqueId);
                    }
                    else result = fileUniqueId;
                }
                else
                    result = GetAccessError(errorCode);
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }

            return result;
        }

        private static bool StorageHasAvailableFreeSpace(string storagePath, decimal sizeInMB)
        {
            if (EnsureDirectoryExists(storagePath))
            {
                DriveInfo storageDrive = new DriveInfo(storagePath[0].ToString());
                if (storageDrive != null)
                    return ((storageDrive.AvailableFreeSpace / 1024 / 1024) >= sizeInMB);
            }
            return false;
        }

        private static void DeleteFile(string fileUniqueId, FileTableAdapter adapter)
        {
            adapter.DeleteFile(fileUniqueId);
            //using (MainDataSet.FileDataTable table = adapter.DeleteFile(fileUniqueId))
            //{
            //    foreach (MainDataSet.FileRow row in table)
            //    {
            //        if (File.Exists(row.FilePath)) File.Delete(row.FilePath);
            //        DeleteDirectories(row.StoragePath, row.FileUniqueId);
            //    }
            //}
        }

        public static void DeleteTemporaryFiles(FileTableAdapter adapter)
        {
            using (MainDataSet.FileDataTable table = adapter.GetTemporaryFilesForDelete())
            {
                foreach (MainDataSet.FileRow row in table)
                {
                    DeleteFile(row.FileUniqueId, adapter);
                }
            }
        }

        private static string GetPathFromUniqueName(string fileUniqueId)
        {
            StringBuilder sb = new StringBuilder();

            if (fileUniqueId.Length > 2)
            {
                for (int index = 0; index < 2; index++)
                {
                    sb.AppendFormat("{0}\\", fileUniqueId[index]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines if the character needs to be encoded.
        /// </summary>
        /// <param name="value">A Unicode character.</param>
        /// <returns>true if value needs to be converted; otherwise, false.</returns>
        private static bool NeedToEncode(char value)
        {
            if (value <= 127)
            {
                string reservedChars = "$-_.+!*'(),@=&";
                if (char.IsLetterOrDigit(value) || reservedChars.IndexOf(value) >= 0)
                    return false;
            }
            return true;
        }

        private static void RotateFlipImageByOrientation(string fileName, string mimeType)
        {
            Image image = null;

            try
            {
                image = Bitmap.FromFile(fileName);

                if (image.RotateFlipByOrientation())
                {
                    ImageFormat imageFormat = MimeType.GetImageFormat(mimeType) ?? ImageFormat.Jpeg;

                    image.Save(fileName, imageFormat);
                }
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }
        }

        #endregion

        #region Public Methods

        public static string CalculateChecksum(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return CalculateChecksum(stream);
            }
        }

        public static string CopyFile(string applicationGuid, string organizationGuid, string departmentGuid, string fileUniqueId, string destinationOrganizationGuid, string destinationDepartmentGuid)
        {
            MainDataSet.FileRow row = GetFileInfo(fileUniqueId);
            if (row != null)
            {
                if (row.ApplicationGuid != Helper.CreateGuid(applicationGuid))
                    return "Error: Application is not valid.";

                if (row.OrganizationGuid != Helper.CreateGuid(organizationGuid))
                    return "Error: Organization is not valid.";

                if (row.DepartmentGuid != Helper.CreateGuid(departmentGuid))
                    return "Error: Department is not valid.";

                string destinationFileName = string.Empty;
                string destinationFolder = string.Empty;
                Guid? storageGuid = null;
                string destFileUniqueId = string.Empty;

                int errorCode = GetAccess(applicationGuid, destinationOrganizationGuid, destinationDepartmentGuid, row.SizeInBytes
                    , ref destinationFolder, ref destinationFileName, ref destFileUniqueId, ref storageGuid);

                if (errorCode == 0)
                {
                    if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                    string sourceFileName = row.StoragePath + GetPathFromUniqueName(fileUniqueId) + fileUniqueId;

                    File.Copy(sourceFileName, destinationFileName);

                    using (FileTableAdapter adapter = new FileTableAdapter())
                    {
                        adapter.UpdateFile(destFileUniqueId, null, row.FileExtension, row.MimeType
                            , applicationGuid, storageGuid.Value.ToString(), destinationOrganizationGuid, destinationDepartmentGuid
                            , row.Name, row.SizeInBytes, (row.IsWidthNull() ? null : new int?(row.Width)), (row.IsHeightNull() ? null : new int?(row.Height))
                            , (row.IsAlignNull() ? null : new int?(row.Align)), row.ExpirationRequired, (row.IsChecksumNull() ? null : row.Checksum), ref errorCode);
                    }

                    if (errorCode != 0)
                    {
                        if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                        DeleteDirectories(destinationFolder, destFileUniqueId);

                        return GetCreateError(errorCode);
                    }
                    else
                        return destFileUniqueId;
                }
                else
                    return GetAccessError(errorCode);
            }
            else
                return "Error: File not exist.";
        }

        public static string CreateAppInfo(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid)
        {
            string result = string.Empty;

            try
            {
                int errorCode = 0;

                using (DepartmentTableAdapter adapter = new DepartmentTableAdapter())
                {
                    adapter.UpdateOrganizationDepartment(applicationGuid, organizationName, departmentName, ref organizationGuid, ref departmentGuid, ref errorCode);
                }

                switch (errorCode)
                {
                    case -1:
                        result = "Can't find application with GUID: " + applicationGuid;
                        break;
                    case -2:
                        result = "Can't create organization: " + organizationName + " Please check input parameters, maybe OrganizationGUID not unique or have incorrect format.";
                        break;
                    case -3:
                        result = "Can't update organization: " + organizationName + " Please check input parameters, maybe OrganizationGUID not unique or have incorrect format.";
                        break;
                    case -4:
                        result = "Can't create department: " + departmentName + " Please check input parameters, maybe DepartmentGUID not unique or have incorrect format.";
                        break;
                    case -5:
                        result = "Can't update department: " + departmentName + " Please check input parameters, maybe DepartmentGUID not unique or have incorrect format.";
                        break;
                    case -6:
                        result = "Unknown error in CreateAppInfo, where ApplicationGUID: " + applicationGuid + " OrganizationName:" + organizationName + " OrganizationGUID:" + organizationGuid + " DepartmentName:" + departmentName + " DepartmentGUID:" + departmentGuid;
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }

            return result;
        }

        public static string CreateFile(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid
            , GetFileRequestStreaming request, ref string checksum)
        {
            string result = CreateAppInfo(applicationGuid, organizationName, ref organizationGuid, departmentName, ref departmentGuid);
            if (result.Length > 0) return result;

            foreach (string tempFileName in request.FileContents.FileCollection)
            {
                if (string.Compare(Path.GetExtension(tempFileName), ".tmp", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    result = CreateFile(applicationGuid, organizationGuid, departmentGuid, request.FileName, tempFileName, false, null, null, ref checksum);
                    break;
                }
            }

            return result;
        }

        public static string CreateFile(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid
            , string fileUrl, ref string checksum)
        {
            string result = CreateAppInfo(applicationGuid, organizationName, ref organizationGuid, departmentName, ref departmentGuid);
            if (result.Length > 0) return result;

            result = CreateFile(applicationGuid, organizationGuid, departmentGuid, fileUrl, null, true, null, null, ref checksum);

            return result;
        }

        public static object[] CreateFile(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid
            , string temporaryGuid, bool expirationRequired, HttpPostedFile file)
        {
            object[] objs = new object[5];
            string result = CreateAppInfo(applicationGuid, organizationName, ref organizationGuid, departmentName, ref departmentGuid);
            if (result.Length > 0)
                objs[4] = result;
            else
            {
                string fileName = Path.GetFileName(file.FileName);
                string checksum = null;
                string fileUniqueId = CreateFile(applicationGuid, organizationGuid, departmentGuid, fileName, null, false, expirationRequired, file, ref checksum);
                if (StringIsFileUniqueId(fileUniqueId))
                {
                    result = UpdateFileTemporaryGuid(fileUniqueId, temporaryGuid);
                    if (string.IsNullOrEmpty(result))
                    {
                        objs[0] = fileUniqueId;
                        objs[1] = fileName;
                        objs[2] = file.ContentLength;
                        objs[3] = checksum;
                    }
                    else
                        objs[4] = result;
                }
            }
            return objs;
        }

        // If directories are empty then delete
        public static void DeleteDirectories(string storagePath, string fileUniqueId)
        {
            if (string.IsNullOrEmpty(storagePath) || string.IsNullOrEmpty(fileUniqueId)) return;
            if (fileUniqueId.Length < 2) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 1; index >= 0; index--)
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

                if (Directory.Exists(storagePath)) Directory.Delete(storagePath);

            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (ArgumentException) { }
        }

        // Delete temporary files
        public static void DeleteTemporaryFiles()
        {
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                DeleteTemporaryFiles(adapter);
            }
        }

        public static string DeleteFile(string fileUniqueId)
        {
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                if (string.Compare(fileUniqueId, "-1", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (MainDataSet.FileDataTable table = adapter.GetFiles())
                    {
                        foreach (MainDataSet.FileRow row in table)
                        {
                            DeleteFile(row.FileUniqueId, adapter);
                        }
                    }
                }
                else
                    DeleteFile(fileUniqueId, adapter);

                DeleteTemporaryFiles(adapter);
            }

            return string.Empty;
        }

        public static object Deserialize(string value)
        {
            object obj = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    LosFormatter formatter = new LosFormatter();
                    obj = formatter.Deserialize(value);
                }
                catch (HttpException) { }
            }
            return obj;
        }

        public static bool EnsureDirectoryExists(string directoryPath)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(directoryPath))
            {
                try
                {
                    if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                    result = true;
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (ArgumentException) { }
                catch (NotSupportedException) { }
            }

            return result;
        }

        public static string GenerateUniqueFileId()
        {
            StringBuilder sb = new StringBuilder();

            Random idGenerator = new Random();

            string ALPHABET = "0123456789abcdefghijklmnopqrstuvwxyz"; //Note: In windows file names not case sensitive: 'A' and 'a' is equal

            for (int i = 0; i < 32; i++)
            {
                sb.Append(ALPHABET.ToCharArray(idGenerator.Next(35), 1)[0]);
            }

            return sb.ToString();
        }

        public static int GetAccess(string applicationGuid, string organizationGuid, string departmentGuid, long sizeInBytes
            , ref string storagePath, ref string fullFileName, ref string fileUniqueId, ref Guid? storageGuid)
        {
            fullFileName = string.Empty;
            storagePath = string.Empty;
            fileUniqueId = GenerateUniqueFileId();

            int filesCount = 1;
            int errorCode = 0;
            bool spaceIsAvailable = false;
            decimal? nextFreeSizeInMB = decimal.Zero;
            decimal sizeInMB = (sizeInBytes / 1024 / 1024);

            if (sizeInMB <= decimal.Zero) sizeInMB = 0.0001m;

            StorageTableAdapter adapter = new StorageTableAdapter();
            {
                do
                {
                    storagePath = null;

                    adapter.ValidateStorage(applicationGuid, organizationGuid, departmentGuid, fileUniqueId, sizeInMB, filesCount, nextFreeSizeInMB.Value, ref nextFreeSizeInMB, ref storageGuid, ref storagePath, ref errorCode);
                    while (errorCode == -6) // While not unique key
                    {
                        fileUniqueId = GenerateUniqueFileId();
                        adapter.ValidateStorage(applicationGuid, organizationGuid, departmentGuid, fileUniqueId, sizeInMB, filesCount, nextFreeSizeInMB.Value, ref nextFreeSizeInMB, ref storageGuid, ref storagePath, ref errorCode);
                    }

                    if ((errorCode == 0) && storageGuid.HasValue)
                    {
                        spaceIsAvailable = StorageHasAvailableFreeSpace(storagePath, sizeInMB);
                        if (!spaceIsAvailable) storagePath = null;
                    }
                    else
                        break;
                }
                while (!spaceIsAvailable);
            }

            if ((errorCode == 0) && storageGuid.HasValue && (!string.IsNullOrEmpty(storagePath)))
            {
                string destinationFolder = storagePath + GetPathFromUniqueName(fileUniqueId);

                if (EnsureDirectoryExists(destinationFolder))
                    fullFileName = destinationFolder + fileUniqueId;
                else
                    errorCode = -7;
            }

            return errorCode;
        }

        public static string GetAccessError(int errorCode)
        {
            string result = string.Empty;
            switch (errorCode)
            {
                case -1:
                    result = "Error: Application not defined.";
                    break;
                case -2:
                    result = "Error: Organization not defined.";
                    break;
                case -3:
                    result = "Error: Department not defined.";
                    break;
                case -4:
                    result = "Error: Require size not defined.";
                    break;
                case -5:
                    result = "Error: Require file count not defined.";
                    break;
                case -6:
                    result = "Error: Identifier of the file is not unique.";
                    break;
                case -7:
                    result = "Error: Storages is full or not defined.";
                    break;
            }
            return result;
        }

        public static string GetCreateError(int errorCode)
        {
            string result = string.Empty;
            switch (errorCode)
            {
                case -1:
                    result = "Error: Application not defined.";
                    break;
                case -2:
                    result = "Error: Organization not defined.";
                    break;
                case -3:
                    result = "Error: Department not defined.";
                    break;
            }
            return result;
        }

        public static GetFileResponse GetFile(string fileUniqueId)
        {
            return GetFile(GetFileInfo(fileUniqueId));
        }

        public static GetFileResponse GetFile(MainDataSet.FileRow row)
        {
            GetFileResponse response = new GetFileResponse();

            if (row != null)
            {
                response.FileName = row.NameWithExtension;
                response.FileContents = File.ReadAllBytes(row.FilePath);

                using (TransferLogTableAdapter adapter = new TransferLogTableAdapter())
                {
                    adapter.UpdateTransferLog(row.FileUniqueId, true);
                }
            }

            return response;
        }

        public static MainDataSet.FileRow GetFileInfo(string fileUniqueId)
        {
            MainDataSet.FileRow row = null;

            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                using (MainDataSet.FileDataTable table = adapter.GetFile(fileUniqueId))
                {
                    if (table.Count > 0)
                    {
                        if (!table[0].Deleted) row = table[0];
                    }
                }
            }

            if (row != null)
            {
                if (File.Exists(row.FilePath)) return row;
            }

            return null;
        }

        public static string Serialize(object value)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                LosFormatter formatter = new LosFormatter();
                formatter.Serialize(writer, value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the value indicating whether the specified string is file's unique identifier.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <returns>true, if the string is file's unique identifier; otherwise, false.</returns>
        public static bool StringIsFileUniqueId(string value)
        {
            if (value != null)
                return (value.Length == Micajah.FileService.Dal.MainDataSet.FileDataTable.FileUniqueIdColumnMaxLength);
            return false;
        }

        /// <summary>
        /// Encodes non-US-ASCII characters in a string.
        /// </summary>
        /// <param name="value">A string to encode.</param>
        /// <returns>Encoded string.</returns>
        public static string ToHexString(string value)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            StringBuilder sb = new StringBuilder();
            foreach (char chr in value)
            {
                if (NeedToEncode(chr))
                {
                    byte[] encodedBytes = utf8.GetBytes(chr.ToString());
                    for (int index = 0; index < encodedBytes.Length; index++)
                    {
                        sb.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
                    }
                }
                else
                    sb.Append(chr);
            }
            return sb.ToString();
        }

        public static string UpdateFile(string fileUniqueId, string fileName, bool fileNameIsUrl, ref string checksum)
        {
            string result = string.Empty;

            try
            {
                string applicationGuid = string.Empty;
                string organizationGuid = string.Empty;
                string departmentGuid = string.Empty;
                string fileNameWithExtension = string.Empty;
                int? width = null;
                int? height = null;

                MainDataSet.FileRow row = GetFileInfo(fileUniqueId);
                if (row != null)
                {
                    if (MimeType.IsImageType(row.MimeType))
                    {
                        width = (row.IsWidthNull() ? 0 : row.Width);
                        height = (row.IsHeightNull() ? 0 : row.Height);

                        if ((width.GetValueOrDefault(0) != 0) || (height.GetValueOrDefault(0) != 0))
                            return "Error: Do not update nested files. Update only original files.";
                    }

                    applicationGuid = row.ApplicationGuid.ToString();
                    organizationGuid = row.OrganizationGuid.ToString();
                    departmentGuid = row.DepartmentGuid.ToString();
                    fileNameWithExtension = row.NameWithExtension;
                }
                else
                    return "Error: File not exist.";

                if (fileNameIsUrl)
                {
                    fileNameWithExtension = string.Empty;
                    result = CheckFileUrl(fileName, ref fileNameWithExtension);
                    if (result.Length > 0) return result;
                }
                else
                    fileNameWithExtension = fileName;

                string storagePath = row.StoragePath;
                string destinationFileName = storagePath + GetPathFromUniqueName(fileUniqueId) + fileUniqueId;
                int errorCode = 0;
                long sizeInBytes = 0;
                FileInfo fileInfo = null;

                if (fileNameIsUrl)
                    sizeInBytes = 1048576; // File size before unknown: by default request 1Mb
                else
                {
                    fileInfo = new FileInfo(fileName);
                    sizeInBytes = fileInfo.Length;
                }

                decimal sizeInMB = (sizeInBytes / 1024 / 1024);

                if (sizeInMB <= decimal.Zero) sizeInMB = 0.0001m;

                if (!StorageHasAvailableFreeSpace(storagePath, sizeInMB))
                    errorCode = -7;

                if (errorCode == 0)
                {
                    result = FileManager.DeleteFile(fileUniqueId);
                    if (result.Length > 0) return result;

                    if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                    if (fileNameIsUrl)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(fileName, destinationFileName);

                        fileInfo = new FileInfo(destinationFileName);
                    }
                    else
                        File.Move(fileName, destinationFileName); // Move from temporary to real location

                    width = null;
                    height = null;
                    string fileExtension = Path.GetExtension(fileNameWithExtension);
                    string fileMimeType = MimeType.GetMimeType(fileExtension);

                    if (MimeType.IsFlash(fileMimeType))
                    {
                        SwfFileInfo swfFileInfo = new SwfFileInfo(destinationFileName);
                        if (swfFileInfo.IsValid)
                        {
                            width = swfFileInfo.Width;
                            height = swfFileInfo.Height;
                        }
                    }
                    else if (MimeType.IsImageType(fileMimeType))
                    {
                        RotateFlipImageByOrientation(destinationFileName, fileMimeType);
                    }

                    checksum = CalculateChecksum(destinationFileName);

                    using (FileTableAdapter adapter = new FileTableAdapter())
                    {
                        adapter.UpdateFile(fileUniqueId, null, fileExtension, fileMimeType
                            , applicationGuid, row.StorageGuid.ToString(), organizationGuid, departmentGuid
                            , Path.GetFileNameWithoutExtension(fileNameWithExtension), (int)fileInfo.Length, width, height, null, row.ExpirationRequired, checksum, ref errorCode);
                    }

                    if (errorCode != 0)
                    {
                        result = GetCreateError(errorCode);

                        if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                        DeleteDirectories(storagePath, fileUniqueId);
                    }
                }
                else
                    result = GetAccessError(errorCode);
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }

            return result;
        }

        public static string UpdateFileTemporaryGuid(string fileUniqueId, string temporaryGuid)
        {
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                adapter.UpdateFileTemporaryGuid(fileUniqueId, temporaryGuid);
            }
            return string.Empty;
        }

        #endregion
    }
}