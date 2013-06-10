using Micajah.FileService.Client.Dal;
using Micajah.FileService.Client.Dal.MetaDataSetTableAdapters;
using Micajah.FileService.Client.Properties;
using Micajah.FileService.Client.WebService;
using Microsoft.Web.Services3;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Protocols;

namespace Micajah.FileService.Client
{
    public static class Access
    {
        #region Private Properties

        private static FileMtomService ServiceProxy
        {
            get
            {
                FileMtomService serviceproxy = new FileMtomService();
                if (string.IsNullOrEmpty(Settings.Default.WebServiceInternalUrl))
                    serviceproxy.Url = Settings.Default.WebServiceUrl;
                else
                    serviceproxy.Url = Settings.Default.WebServiceInternalUrl;
                serviceproxy.Credentials = CredentialCache.DefaultCredentials;
                return serviceproxy;
            }
        }

        #endregion

        #region Private Methods

        private static string CalculateChecksum(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return CalculateChecksum(stream);
            }
        }

        private static string CalculateChecksum(byte[] fileContent)
        {
            using (MemoryStream stream = new MemoryStream(fileContent))
            {
                return CalculateChecksum(stream);
            }
        }

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

        private static bool ChecksumExists(string checksum, Guid organizationId, Guid departmentId, string localObjectId, string localObjectType, string connectionString)
        {
            using (FileTableAdapter adapter = new FileTableAdapter(string.IsNullOrEmpty(connectionString) ? Settings.Default.MetaDataConnectionString : connectionString))
            {
                using (MetaDataSet.FileDataTable table = adapter.GetFiles(organizationId, departmentId, localObjectType, localObjectId, false))
                {
                    if (table.Count > 0)
                    {
                        foreach (MetaDataSet.FileRow row in table)
                        {
                            if (!row.IsChecksumNull())
                            {
                                if (string.Compare(checksum, row.Checksum, StringComparison.OrdinalIgnoreCase) == 0)
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static string ReadFile(string fullFileName, ref byte[] fileContent)
        {
            if (File.Exists(fullFileName))
                fileContent = File.ReadAllBytes(fullFileName);
            else
                return "Error: File " + fullFileName + " not exist.";
            return string.Empty;
        }

        private static void ProcessException(Exception e, StringBuilder sb)
        {
            if (e != null)
            {
                WebException webExcep = e as WebException;
                if (webExcep != null)
                {
                    // Process the Web Exception.
                    if (webExcep.Response != null)
                    {
                        WebResponse response = webExcep.Response;
                        Stream responseStream = response.GetResponseStream();

                        if (responseStream.CanRead)
                        {
                            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                            string excepStr = reader.ReadToEnd();
                            sb.Append("Web Exception Occured: " + excepStr);
                        }
                        else
                        {
                            sb.Append("Web Exception Occured: " + e.ToString());
                        }
                    }
                    else
                    {
                        sb.Append("Web Exception Occured: " + e.ToString());
                    }
                }
                else
                {
                    SoapException se = e as SoapException;
                    if (se != null)
                    {
                        sb.Append("System.Web.Services.Protocols.SoapException:");
                        sb.Append(Environment.NewLine);
                        sb.Append("SOAP-Fault code: " + se.Code.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append("Message: ");
                    }
                    else
                    {
                        sb.Append(e.GetType().FullName);
                        sb.Append(": ");
                    }

                    sb.Append(e.Message);

                    if (e.InnerException != null)
                    {
                        sb.Append(" ---> ");
                        ProcessException(e.InnerException, sb);
                        sb.Append(Environment.NewLine);
                        sb.Append("--- End of Inner Exception ---");
                    }

                    if (e.StackTrace != null)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(e.StackTrace);
                    }
                }
            }
        }

        private static void Error(Exception exception)
        {
            Console.WriteLine("****** Exception Raised ******");

            ResponseProcessingException ex = exception as ResponseProcessingException;
            if (ex != null)
                Console.WriteLine(ex.Response.OuterXml);

            StringBuilder sb = new StringBuilder();
            ProcessException(exception, sb);
            Console.WriteLine(sb.ToString());
            Console.WriteLine("******************************");
        }

        #endregion

        #region Public Methods

        public static string CopyFile(Guid applicationId, Guid organizationId, Guid departmentId, string fileUniqueId
            , Guid destinationOrganizationId, Guid destinationDepartmentId)
        {
            return CopyFile(applicationId, organizationId, departmentId, fileUniqueId, destinationOrganizationId, destinationDepartmentId, false, null, null);
        }

        public static string CopyFile(Guid applicationId, Guid organizationId, Guid departmentId, string fileUniqueId
            , Guid destinationOrganizationId, Guid destinationDepartmentId, bool copyMetadata, string connectionString)
        {
            return CopyFile(applicationId, organizationId, departmentId, fileUniqueId, destinationOrganizationId, destinationDepartmentId, copyMetadata, null, connectionString);
        }

        public static string CopyFile(Guid applicationId, Guid organizationId, Guid departmentId, string fileUniqueId
            , Guid destinationOrganizationId, Guid destinationDepartmentId
            , bool copyMetadata, string updatedBy, string connectionString)
        {
            string result = string.Empty;
            FileTableAdapter adapter = null;

            try
            {
                result = ServiceProxy.CopyFile(applicationId.ToString("N"), organizationId.ToString("N"), departmentId.ToString("N"), fileUniqueId
                    , destinationOrganizationId.ToString("N"), destinationDepartmentId.ToString("N"));
                if (StringIsFileUniqueId(result) && copyMetadata)
                {
                    adapter = new FileTableAdapter(connectionString);
                    MetaDataSet.FileDataTable table = adapter.GetFile(fileUniqueId);
                    if (table.Count > 0)
                    {
                        MetaDataSet.FileRow row = table[0];
                        adapter.Insert(result, destinationOrganizationId, destinationDepartmentId, row.LocalObjectType, row.LocalObjectId, row.Name, row.SizeInBytes, DateTime.UtcNow, updatedBy, row.Deleted, (row.IsChecksumNull() ? null : row.Checksum));
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
            }

            return result;
        }

        public static bool GetFileInfo(string fileUniqueId, ref string fileNameWithExtension, ref long sizeInBytes, ref int width, ref int height, ref int align, ref string mimeType, ref string checksum)
        {
            bool result = false;

            try
            {
                result = ServiceProxy.GetFileInfo(fileUniqueId, ref fileNameWithExtension, ref sizeInBytes, ref width, ref height, ref align, ref mimeType, ref checksum);
            }
            catch (Exception ex)
            {
                result = false;
                Error(ex);
                fileNameWithExtension = ex.Message.ToString();
            }

            return result;
        }

        public static byte[] GetFile(string fileUniqueId)
        {
            try
            {
                GetFileResponse response = ServiceProxy.GetFile(fileUniqueId);
                if (response.fileData.Length > 0) return response.fileData;
            }
            catch (Exception ex)
            {
                Error(ex);
            }

            return null;
        }

        public static string DeleteFile(string fileUniqueId)
        {
            return DeleteFile(fileUniqueId, false, null, null);
        }

        public static string DeleteFile(string fileUniqueId, bool deleteMetadata, string connectionString)
        {
            return DeleteFiles(new string[] { fileUniqueId }, deleteMetadata, null, connectionString);
        }

        public static string DeleteFile(string fileUniqueId, bool deleteMetadata, string updatedBy, string connectionString)
        {
            return DeleteFiles(new string[] { fileUniqueId }, deleteMetadata, updatedBy, connectionString);
        }

        public static string DeleteFiles(string[] fileUniqueId)
        {
            return DeleteFiles(fileUniqueId, false, null, null);
        }

        public static string DeleteFiles(string[] fileUniqueId, bool deleteMetadata, string connectionString)
        {
            return DeleteFiles(fileUniqueId, deleteMetadata, null, connectionString);
        }

        public static string DeleteFiles(string[] fileUniqueId, bool deleteMetadata, string updatedBy, string connectionString)
        {
            if ((fileUniqueId == null) || (fileUniqueId.Length == 0)) return string.Empty;

            FileTableAdapter adapter = null;
            StringBuilder sb = new StringBuilder();

            try
            {
                FileMtomService serviceproxy = ServiceProxy;

                if (deleteMetadata) adapter = new FileTableAdapter(connectionString);

                foreach (string id in fileUniqueId)
                {
                    string result = serviceproxy.DeleteFile(id);
                    if (result.Length > 0)
                        sb.AppendLine(result);
                    else if (adapter != null)
                        adapter.MarkFileAsDeleted(id, updatedBy);
                }
            }
            catch (Exception ex)
            {
                Error(ex);
                sb.AppendLine(ex.Message);
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
            }

            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public static string DeleteFiles(Guid organizationId, Guid departmentId, string localObjectId, string localObjectType, string updatedBy, string connectionString)
        {
            List<string> list = new List<string>();
            using (FileTableAdapter adapter = new FileTableAdapter(connectionString))
            {
                using (MetaDataSet.FileDataTable table = adapter.GetFiles(organizationId, departmentId, localObjectType, localObjectId, false))
                {
                    foreach (MetaDataSet.FileRow row in table)
                    {
                        list.Add(row.FileUniqueId);
                    }
                }
            }
            return DeleteFiles(list.ToArray(), true, updatedBy);
        }

        public static string SetTemporaryFile(string fileUniqueId, string temporaryId)
        {
            string result = string.Empty;
            try
            {
                result = ServiceProxy.SetTemporaryFile(fileUniqueId, temporaryId);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        public static string GetTemporaryFiles(string temporaryId)
        {
            string result = string.Empty;
            try
            {
                result = ServiceProxy.GetTemporaryFiles(temporaryId);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        public static byte[] GetThumbnail(string fileUniqueId, int width, int height, ref string newFileUniqueId)
        {
            return GetThumbnail(fileUniqueId, width, height, 1, ref newFileUniqueId);
        }

        public static byte[] GetThumbnail(string fileUniqueId, int width, int height, int align, ref string newFileUniqueId)
        {
            try
            {
                GetFileResponse response = ServiceProxy.GetThumbnail(fileUniqueId, width, height, align, ref newFileUniqueId);
                if (response.fileData.Length > 0)
                    return response.fileData;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Error(ex);
                newFileUniqueId = ex.Message.ToString();
            }

            return null;
        }

        public static bool FileExists(byte[] fileContent, Guid organizationId, Guid departmentId, string localObjectId, string localObjectType, string connectionString)
        {
            return ChecksumExists(CalculateChecksum(fileContent), organizationId, departmentId, localObjectId, localObjectType, connectionString);
        }

        public static bool FileExists(string fileName, Guid organizationId, Guid departmentId, string localObjectId, string localObjectType, string connectionString)
        {
            return ChecksumExists(CalculateChecksum(fileName), organizationId, departmentId, localObjectId, localObjectType, connectionString);
        }

        public static string PutFile(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fullFileName, ref string checksum)
        {
            return PutFile(applicationId, organizationName, ref  organizationId, departmentName, ref  departmentId, fullFileName, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string PutFile(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fullFileName, bool expirationRequired, ref string checksum)
        {
            byte[] fileContent = null;
            string result = ReadFile(fullFileName, ref fileContent);
            if (result.Length == 0)
                result = PutFileAsByteArray(applicationId, organizationName, ref organizationId, departmentName, ref departmentId, fullFileName, ref fileContent, expirationRequired, ref checksum);
            return result;
        }

        public static string PutFileFromUrl(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fileUrl, ref string checksum)
        {
            return PutFileFromUrl(applicationId, organizationName, ref organizationId, departmentName, ref departmentId, fileUrl, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string PutFileFromUrl(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fileUrl, bool expirationRequired, ref string checksum)
        {
            string result = "Error: Unknown error.";

            try
            {
                FileMtomService serviceproxy = ServiceProxy;

                result = serviceproxy.PutFileFromUrl(applicationId, organizationName, ref organizationId, departmentName, ref departmentId, fileUrl, ref checksum);
                if ((!expirationRequired) && StringIsFileUniqueId(result))
                    serviceproxy.UpdateFileExpirationRequired(result, expirationRequired);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        public static string PutFileAsByteArray(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId,
            string fileNameWithExtension, ref byte[] fileContent, ref string checksum)
        {
            return PutFileAsByteArray(applicationId, organizationName, ref organizationId, departmentName, ref departmentId, fileNameWithExtension, ref fileContent, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string PutFileAsByteArray(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId,
            string fileNameWithExtension, ref byte[] fileContent, bool expirationRequired, ref string checksum)
        {
            string result = "Error: Unknown error.";

            try
            {
                FileMtomService serviceproxy = ServiceProxy;

                GetFileRequestStreaming request = new GetFileRequestStreaming();
                request.fileName = fileNameWithExtension;
                request.fileData = fileContent;

                result = serviceproxy.PutFile(applicationId, organizationName, ref organizationId, departmentName, ref departmentId, request, ref checksum);
                if ((!expirationRequired) && StringIsFileUniqueId(result))
                    serviceproxy.UpdateFileExpirationRequired(result, expirationRequired);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        public static string PutFileAsByteArray(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fileNameWithExtension, ref byte[] fileContent, ref string checksum, string localObjectId, string localObjectType, string updatedBy, string connectionString)
        {
            return PutFileAsByteArray(applicationId, organizationName, ref  organizationId, departmentName, ref  departmentId
            , fileNameWithExtension, ref fileContent, Settings.Default.LinksExpiration, ref checksum, localObjectId, localObjectType, updatedBy, connectionString);
        }

        public static string PutFileAsByteArray(string applicationId, string organizationName, ref string organizationId, string departmentName, ref string departmentId
            , string fileNameWithExtension, ref byte[] fileContent, bool expirationRequired, ref string checksum, string localObjectId, string localObjectType, string updatedBy, string connectionString)
        {
            string result = PutFileAsByteArray(applicationId, organizationName, ref  organizationId, departmentName, ref  departmentId, fileNameWithExtension, ref fileContent, expirationRequired, ref checksum);
            if (StringIsFileUniqueId(result))
            {
                FileTableAdapter adapter = null;
                try
                {
                    adapter = new FileTableAdapter(connectionString);
                    adapter.Insert(result, Support.CreateGuid(organizationId), Support.CreateGuid(departmentId), localObjectType, localObjectId, fileNameWithExtension, fileContent.Length, DateTime.UtcNow, updatedBy, false, checksum);
                }
                catch (DBConcurrencyException ex)
                {
                    result = ex.Message;
                }
                catch (SqlException ex)
                {
                    result = ex.Message;
                }
                finally
                {
                    if (adapter != null) adapter.Dispose();
                }
            }
            return result;
        }

        public static string UpdateFile(string fileUniqueId, string nameWithExtension, ref string checksum)
        {
            return UpdateFile(fileUniqueId, nameWithExtension, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string UpdateFile(string fileUniqueId, string nameWithExtension, bool expirationRequired, ref string checksum)
        {
            string result = "Error: Unknown error.";

            byte[] fileContent = null;
            result = ReadFile(nameWithExtension, ref fileContent);
            if (result.Length == 0)
            {
                try
                {
                    FileMtomService serviceproxy = ServiceProxy;

                    GetFileRequestStreaming request = new GetFileRequestStreaming();
                    request.fileName = nameWithExtension;
                    request.fileData = fileContent;

                    result = serviceproxy.UpdateFile(fileUniqueId, request, ref checksum);
                    if ((!expirationRequired) && (result.Length == 0))
                        serviceproxy.UpdateFileExpirationRequired(fileUniqueId, expirationRequired);
                }
                catch (Exception ex)
                {
                    Error(ex);
                    result = ex.Message.ToString();
                }
            }

            return result;
        }

        public static string UpdateFileFromUrl(string fileUniqueId, string url, ref string checksum)
        {
            return UpdateFileFromUrl(fileUniqueId, url, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string UpdateFileFromUrl(string fileUniqueId, string url, bool expirationRequired, ref string checksum)
        {
            string result = "Error: Unknown error.";

            try
            {
                FileMtomService serviceproxy = ServiceProxy;

                result = serviceproxy.UpdateFileFromUrl(fileUniqueId, url, ref checksum);
                if ((!expirationRequired) && (result.Length == 0))
                    result = serviceproxy.UpdateFileExpirationRequired(fileUniqueId, expirationRequired);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        public static string UpdateFileAsByteArray(string fileUniqueId, string fileNameWithExtension, ref byte[] content, ref string checksum)
        {
            return UpdateFileAsByteArray(fileUniqueId, fileNameWithExtension, ref  content, Settings.Default.LinksExpiration, ref checksum);
        }

        public static string UpdateFileAsByteArray(string fileUniqueId, string fileNameWithExtension, ref byte[] content, bool expirationRequired, ref string checksum)
        {
            string result = "Error: Unknown error.";

            try
            {
                FileMtomService serviceproxy = ServiceProxy;

                GetFileRequestStreaming request = new GetFileRequestStreaming();
                request.fileName = fileNameWithExtension;
                request.fileData = content;

                result = serviceproxy.UpdateFile(fileUniqueId, request, ref checksum);
                if ((!expirationRequired) && (result.Length == 0))
                    result = serviceproxy.UpdateFileExpirationRequired(fileUniqueId, expirationRequired);
            }
            catch (Exception ex)
            {
                Error(ex);
                result = ex.Message.ToString();
            }

            return result;
        }

        /// <summary>
        /// Returns the direct URL to the Flash file.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <returns>The string that contains the direct URL to the Flash file.</returns>
        public static string GetFlashUrl(string fileUniqueId, Guid organizationId, Guid departmentId)
        {
            return GetFileUrl(fileUniqueId, organizationId, departmentId, false) + "&Flash=1";
        }

        /// <summary>
        /// Returns the URL to the file.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <returns>The string that contains the URL to the file.</returns>
        public static string GetFileUrl(string fileUniqueId, Guid organizationId, Guid departmentId)
        {
            return GetFileUrl(fileUniqueId, organizationId, departmentId, false);
        }

        /// <summary>
        /// Returns the URL to the file.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <param name="download">The value indicating the URL is for download the file.</param>
        /// <returns>The string that contains the URL to the file.</returns>
        public static string GetFileUrl(string fileUniqueId, Guid organizationId, Guid departmentId, bool download)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}?FileId={1}", Settings.Default.FilePageUrl, fileUniqueId);
            if (Settings.Default.LinksExpiration)
            {
                int expirationTimeout = Settings.Default.LinksExpirationTimeout;
                using (SHA256Managed sha = new SHA256Managed())
                {
                    long ticks = DateTime.UtcNow.Ticks;
                    string hash = Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}"
                        , Settings.Default.ApplicationId, organizationId, departmentId, fileUniqueId, ticks, expirationTimeout))));

                    sb.AppendFormat(CultureInfo.InvariantCulture, "&E={0}.{1}&H={2}", ticks, expirationTimeout, HttpUtility.UrlEncodeUnicode(hash));
                }
            }
            if (download) sb.Append("&D=1");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the URL to the thumbnail of the picture.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the picture file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="height">The height of the thumbnail.</param>
        /// <returns>The string that contains the URL to the thumbnail of the picture.</returns>
        public static string GetThumbnailUrl(string fileUniqueId, Guid organizationId, Guid departmentId, int width, int height)
        {
            return GetThumbnailUrl(fileUniqueId, organizationId, departmentId, width, height, 1);
        }

        /// <summary>
        /// Returns the URL to the thumbnail of the picture.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the picture file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="height">The height of the thumbnail.</param>
        /// <param name="align">Specifies the align of the original picture in the thumbnail. The possible values are 1-9.</param>
        /// <returns>The string that contains the URL to the thumbnail of the picture.</returns>
        public static string GetThumbnailUrl(string fileUniqueId, Guid organizationId, Guid departmentId, int width, int height, int align)
        {
            return GetThumbnailUrl(fileUniqueId, organizationId, departmentId, width, height, align, false);
        }

        /// <summary>
        /// Returns the URL to the thumbnail of the picture.
        /// </summary>
        /// <param name="fileUniqueId">The unique identifier of the picture file.</param>
        /// <param name="organizationId">The organization's identifier that the file is belong to.</param>
        /// <param name="departmentId">The department's identifier that the file is belong to.</param>
        /// <param name="width">The width of the thumbnail.</param>
        /// <param name="height">The height of the thumbnail.</param>
        /// <param name="align">Specifies the align of the original picture in the thumbnail. The possible values are 1-9.</param>
        /// <param name="download">The value indicating the URL is for download the file.</param>
        /// <returns>The string that contains the URL to the thumbnail of the picture.</returns>
        public static string GetThumbnailUrl(string fileUniqueId, Guid organizationId, Guid departmentId, int width, int height, int align, bool download)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}&Width={1}&Height={2}&Align={3}", GetFileUrl(fileUniqueId, organizationId, departmentId, download), width, height, align);
        }

        /// <summary>
        /// Returns the value indicating whether the specified string is file's unique identifier.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <returns>true, if the string is file's unique identifier; otherwise, false.</returns>
        public static bool StringIsFileUniqueId(string value)
        {
            if (value != null)
            {
                return ((value.Length == Micajah.FileService.Client.Dal.MetaDataSet.FileDataTable.FileUniqueIdColumnMaxLength)
                    && (value.IndexOf("Error:", StringComparison.OrdinalIgnoreCase) == -1));
            }
            return false;
        }

        #endregion
    }
}
