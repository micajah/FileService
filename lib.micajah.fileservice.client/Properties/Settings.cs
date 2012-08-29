using System.Collections.ObjectModel;
using System.Web.Configuration;

namespace Micajah.FileService.Client.Properties
{
    partial class Settings
    {
        #region Members

        private string m_FilePageUrl;
        private static ReadOnlyCollection<string> s_KnownFileExtensions;
        private static int? s_MaxRequestLengthInKB;

        #endregion

        #region Internal Properties

        internal static ReadOnlyCollection<string> KnownFileExtensions
        {
            get
            {
                if (s_KnownFileExtensions == null)
                    s_KnownFileExtensions = new ReadOnlyCollection<string>(new string[] { "generic", "avi", "bmp", "doc", "docx", "gif", "htm", "html", "jpg", "mov", "mp3", "mpg", "ogg", "pdf", "png", "ppt", "pptx", "txt", "xls", "xlsx", "wav", "wma", "wmv", "zip" });
                return s_KnownFileExtensions;
            }
        }

        #endregion

        #region Public Properties

        public string FilePageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(m_FilePageUrl))
                {
                    string[] parts = Default.WebServiceUrl.Split('/');
                    m_FilePageUrl = string.Join("/", parts, 0, parts.Length - 1) + "/File.ashx";
                }
                return m_FilePageUrl;
            }
        }

        public string ConnectionString
        {
            get { return this.MetaDataConnectionString; }
            set { this["MetaDataConnectionString"] = value; }
        }

        /// <summary>
        /// Gets the maximum request size in bytes for the web service.
        /// </summary>
        public static int WebServiceMaxRequestLength
        {
            get { return (WebServiceMaxRequestLengthInKB * 1024); }
        }

        /// <summary>
        ///  Gets the maximum request size in kylobytes for the web service.
        /// </summary>
        public static int WebServiceMaxRequestLengthInKB
        {
            get { return (WebServiceMaxRequestLengthInMB * 1024); }
        }

        /// <summary>
        ///  Gets the maximum request size in megabytes for the web service.
        /// </summary>
        public static int WebServiceMaxRequestLengthInMB
        {
            get { return 50; }
        }

        /// <summary>
        /// Gets the maximum request size in bytes.
        /// </summary>
        public static int MaxRequestLength
        {
            get { return (MaxRequestLengthInKB * 1024); }
        }

        /// <summary>
        /// Gets the maximum request size in kilobytes.
        /// </summary>
        public static int MaxRequestLengthInKB
        {
            get
            {
                if (!s_MaxRequestLengthInKB.HasValue)
                {
                    HttpRuntimeSection sect = (WebConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection);
                    s_MaxRequestLengthInKB = ((sect == null) ? 4096 : sect.MaxRequestLength);
                }
                return s_MaxRequestLengthInKB.Value;
            }
        }

        /// <summary>
        /// Gets the maximum request size in megabytes.
        /// </summary>
        public static int MaxRequestLengthInMB
        {
            get { return (MaxRequestLengthInKB / 1024); }
        }

        #endregion
    }
}
