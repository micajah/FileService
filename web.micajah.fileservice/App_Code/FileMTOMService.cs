using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Web.Services;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Micajah.FileService.Dal;
using Micajah.FileService.Dal.MainDataSetTableAdapters;

namespace Micajah.FileService.WebService
{
    [System.Web.Services.WebService(Namespace = "http://www.bigwebapps.com/webservices/filemanager/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public partial class FileMtomService : System.Web.Services.WebService
    {
        [WebMethod]
        public string CopyFile(string applicationGuid, string organizationGuid, string departmentGuid, string fileId, string destinationOrganizationGuid, string destinationDepartmentGuid)
        {
            return FileManager.CopyFile(applicationGuid, organizationGuid, departmentGuid, fileId, destinationOrganizationGuid, destinationDepartmentGuid);
        }

        [WebMethod]
        public bool GetFileInfo(string fileId, ref string fullFileName, ref long fileSize, ref int width, ref int height, ref int align, ref string fileMimeType)
        {
            MainDataSet.FileRow row = FileManager.GetFileInfo(fileId);
            if (row != null)
            {
                fullFileName = row.NameWithExtension;
                fileSize = (long)row.SizeInBytes;
                width = (row.IsWidthNull() ? 0 : row.Width);
                height = (row.IsHeightNull() ? 0 : row.Height);
                align = (row.IsAlignNull() ? 1 : row.Align);
                fileMimeType = row.MimeType;

                return true;
            }

            return false;
        }

        [WebMethod]
        public GetFileResponse GetFile(string fileId)
        {
            return FileManager.GetFile(fileId);
        }

        [WebMethod]
        public GetFileResponse GetThumbnail(string fileId, int width, int height, int align, ref string result)
        {
            return Thumbnail.GetThumbnail(fileId, width, height, align, ref result);
        }

        [WebMethod]
        public string DeleteFile(string fileId)
        {
            return FileManager.DeleteFile(fileId);
        }

        [WebMethod]
        public string GetTemporaryFiles(string fileGuid)
        {
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                return adapter.GetTemporaryFilesList(fileGuid);
            }
        }

        [WebMethod]
        public string PutFile(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid, GetFileRequestStreaming request)
        {
            return FileManager.CreateFile(applicationGuid, organizationName, ref organizationGuid, departmentName, ref departmentGuid, request);
        }

        [WebMethod]
        public string PutFileFromUrl(string applicationGuid, string organizationName, ref string organizationGuid, string departmentName, ref string departmentGuid, string fileUrl)
        {
            return FileManager.CreateFile(applicationGuid, organizationName, ref organizationGuid, departmentName, ref departmentGuid, fileUrl);
        }

        [WebMethod]
        public string SetTemporaryFile(string fileId, string fileGuid)
        {
            return FileManager.UpdateFileTemporaryGuid(fileId, fileGuid);
        }

        [WebMethod]
        public string UpdateFile(string fileId, GetFileRequestStreaming request)
        {
            string result = string.Empty;

            foreach (string fileName in request.FileContents.FileCollection)
            {
                if (string.Compare(Path.GetExtension(fileName), ".tmp", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    result = FileManager.UpdateFile(fileId, request.FileName, false);
                    break;
                }
            }

            return result;
        }

        [WebMethod]
        public string UpdateFileFromUrl(string fileId, string fileUrl)
        {
            return FileManager.UpdateFile(fileId, fileUrl, true);
        }

        [WebMethod]
        public string UpdateFileExpirationRequired(string fileId, bool expirationRequired)
        {
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                foreach (string fileUniqueId in fileId.Split(','))
                {
                    adapter.UpdateFileExpirationRequired(fileUniqueId, expirationRequired);
                }
            }
            return string.Empty;
        }
    }

    // Shared Web Method Return types for the FileMTOMService and FileMTOMSecureService.
    [XmlRoot("getFileResponse", Namespace = "http://www.bigwebapps.com/webservices/filemanager/")]
    public class GetFileResponse
    {
        [XmlElement("fileName", IsNullable = false)]
        public string FileName;

        [XmlElement("fileData", IsNullable = false)]
        public byte[] FileContents;
    }

    //The type to stream a response
    [XmlRoot("getFileResponseStreaming", Namespace = "http://www.bigwebapps.com/webservices/filemanager/")]
    public class GetFileResponseStreaming
    {
        [XmlElement("fileName", IsNullable = false)]
        public string FileName;

        [XmlElement("fileData", IsNullable = false)]
        public GetFileResponseWrapper FileContents;
    }

    //The type to stream a request
    [XmlRoot("getFileResponseStreaming", Namespace = "http://www.bigwebapps.com/webservices/filemanager/")]
    public class GetFileRequestStreaming
    {
        [XmlElement("fileName", IsNullable = false)]
        public string FileName;

        [XmlElement("fileData", IsNullable = false)]
        public GetFileResponseWrapper FileContents;
    }

    // This attribute tells the schema machinery to use the GetMysSchema method to get the schema for this class.
    [XmlSchemaProvider("GetMySchema")]
    public class GetFileResponseWrapper : IXmlSerializable, IDisposable
    {
        #region Members

        private string m_FileName;
        private TempFileCollection m_TempFiles;

        #endregion

        #region Constructors

        public GetFileResponseWrapper() : this(null) { }

        public GetFileResponseWrapper(string fileName)
        {
            m_FileName = fileName;
        }

        #endregion

        #region Destructors

        ~GetFileResponseWrapper()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public string FileName
        {
            get { return m_FileName; }
        }

        public TempFileCollection FileCollection
        {
            get { return m_TempFiles; }
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (m_TempFiles != null) m_TempFiles.Delete();
                m_TempFiles = null;
            }
        }

        protected void ReadContentsIntoFile(XmlReader reader, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
            {
                if (reader.CanReadBinaryContent)
                {
                    byte[] buf = new byte[1024];
                    int numRead = 0;
                    while ((numRead = reader.ReadContentAsBase64(buf, 0, 1024)) > 0)
                    {
                        fs.Write(buf, 0, numRead);
                    }
                }
                else
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The schema for the file contents node is actually just
        /// base 64 binary data so return the qname of the schema
        /// type directly.
        /// </summary>
        public static XmlQualifiedName GetMySchema(XmlSchemaSet xss)
        {
            return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
        }

        /// <summary>
        /// Always return null.
        /// </summary>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Deserializes state out of an XmlReader
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
            // Read the open tag of the encapsulating element
            reader.ReadStartElement();

            // Read the binary data that represents the file contents
            // into a temp file.
            m_TempFiles = new TempFileCollection();
            m_FileName = m_TempFiles.AddExtension("fileContents", false);
            ReadContentsIntoFile(reader, m_FileName);

            // Read the close tag of the encapsulating element
            reader.ReadEndElement();
        }

        /// <summary>
        /// Serializes state into an XmlWriter
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            using (FileStream fs = new FileStream(m_FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] buf = new byte[1024];
                int numRead = 0;
                while ((numRead = fs.Read(buf, 0, 1024)) > 0)
                {
                    writer.WriteBase64(buf, 0, numRead);
                }
            }
        }

        #endregion
    }
}