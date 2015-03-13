using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Micajah.FileService.Dal;

namespace Micajah.FileService.WebService
{
    /// <summary>
    /// Displays the file content.
    /// </summary>
    public class FileHandler : IHttpHandler
    {
        #region Members

        private string m_FileUniqueId;
        private DateTime m_Expires;
        private MainDataSet.FileRow m_FileRow;

        #endregion

        #region Public Properties

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion

        #region Private Methods

        private static void ConfigureResponse(HttpResponse response, DateTime expires)
        {
            response.Clear();
            response.ClearHeaders();
            response.Charset = Encoding.UTF8.WebName;
            response.ContentEncoding = Encoding.UTF8;
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetExpires(expires.ToLocalTime());
        }

        private static string GetObjectTag(int width, int height, string fileUrl, string fileName)
        {
            string widthAttribute = string.Empty;
            if (width > 0) widthAttribute = " width=\"" + width + "\"";
            string heightAttribute = string.Empty;
            if (height > 0) heightAttribute = " height=\"" + height + "\"";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>{3}</title>
</head>
<body>
    <object classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" codebase=""https://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=5,0,0,0""{0}{1}>
        <param name=""movie"" value=""{2}&Flash=1"">
        <param name=""quality"" value=""high"">
        <embed src=""{2}&Flash=1"" quality=""high""{0}{1} type=""application/x-shockwave-flash"" pluginspage=""https://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash"">
    </object>
</body>
</html>"
                , widthAttribute, heightAttribute, fileUrl, fileName);
            return sb.ToString();
        }

        private static void ReceiveFile(HttpContext context)
        {
            if (context.Request.Files.Count == 0)
                return;

            object[] p = FileManager.Deserialize(context.Request.Params["P"]) as object[];
            if ((p == null) || (p.Length < 6))
                return;

            string organizationId = (string)p[1];
            string departmentId = (string)p[3];
            bool expirationRequired = true;
            if (p.Length > 6) expirationRequired = (bool)p[6];

            object[] objs = FileManager.CreateFile((string)p[0], (string)p[2], ref organizationId, (string)p[4], ref departmentId, (string)p[5], expirationRequired, context.Request.Files[0]);

            context.Response.StatusCode = 200;
            context.Response.Write(HttpUtility.HtmlEncode(FileManager.Serialize(objs)));
            context.Response.End();
        }

        private void ValidateRequest(HttpContext context)
        {
            m_Expires = DateTime.UtcNow.AddHours(12);
            bool isValid = false;

            m_FileUniqueId = context.Request.QueryString["FileId"];
            if (!string.IsNullOrEmpty(m_FileUniqueId))
            {
                bool expirationRequired = true;
                m_FileRow = FileManager.GetFileInfo(m_FileUniqueId);
                if (m_FileRow != null)
                    expirationRequired = m_FileRow.ExpirationRequired;

                string hash = context.Request.QueryString["H"];
                string t = context.Request.QueryString["E"];

                if (expirationRequired)
                {
                    if ((t != null) && (hash != null))
                    {
                        string[] tp = t.Split('.');
                        long ticks = 0;
                        if (long.TryParse(tp[0], out ticks))
                        {
                            int expirationTimeout = 0;
                            if ((tp.Length > 1) && int.TryParse(tp[1], out expirationTimeout))
                            {
                                string expectedHash = null;

                                if (m_FileRow != null)
                                {
                                    using (SHA256Managed sha = new SHA256Managed())
                                    {
                                        expectedHash = Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}"
                                            , m_FileRow.ApplicationGuid, m_FileRow.OrganizationGuid, m_FileRow.DepartmentGuid, m_FileUniqueId, ticks, expirationTimeout))));
                                    }
                                }

                                if (string.Compare(hash, expectedHash, StringComparison.Ordinal) == 0)
                                {
                                    DateTime date = DateTime.MinValue.AddTicks(ticks);
                                    date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, DateTimeKind.Utc);

                                    m_Expires = date.AddMinutes(expirationTimeout);

                                    DateTime now = DateTime.UtcNow;
                                    now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

                                    isValid = (now >= date) && (now <= m_Expires);
                                    if (!isValid)
                                        context.Response.Write(Resources.Main.FileLinkIsExpired);
                                }
                            }
                        }
                    }
                }
                else
                    isValid = true;
            }

            if (!isValid)
                context.Response.End();
        }

        #endregion

        #region Public Methods

        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current.RewritePath(context.Server.HtmlDecode(context.Request.Url.PathAndQuery));

            ReceiveFile(context);
            this.ValidateRequest(context);

            string fileNameWithExtension = string.Empty;
            string fileMimeType = string.Empty;
            bool isFlash = false;
            bool isImage = false;
            bool download = false;
            GetFileResponse file = null;

            if (m_FileRow == null)
                m_FileRow = FileManager.GetFileInfo(m_FileUniqueId);

            if (m_FileRow != null)
            {
                fileNameWithExtension = m_FileRow.NameWithExtension;
                fileMimeType = m_FileRow.MimeType;
                isFlash = MimeType.IsFlash(fileMimeType);
                isImage = MimeType.IsImageType(fileMimeType);
                download = (string.Compare(context.Request.QueryString["D"], "1", StringComparison.OrdinalIgnoreCase) == 0);

                if (isImage)
                {
                    int width = 0;
                    int height = 0;
                    int align = 0;
                    Int32.TryParse(context.Request.QueryString["Width"], out width);
                    Int32.TryParse(context.Request.QueryString["Height"], out height);
                    Int32.TryParse(context.Request.QueryString["Align"], out align);
                    if ((width > 0) || (height > 0))
                    {
                        string thumbnailId = string.Empty;
                        file = Thumbnail.GetThumbnail(m_FileUniqueId, width, height, align, ref thumbnailId);
                        m_FileRow = FileManager.GetFileInfo(thumbnailId);
                        if (m_FileRow != null)
                        {
                            fileNameWithExtension = m_FileRow.NameWithExtension;
                            fileMimeType = m_FileRow.MimeType;
                        }
                    }
                }
                else if (isFlash && (!download))
                {
                    if (string.Compare(context.Request.QueryString["Flash"], "1", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        m_FileRow = FileManager.GetFileInfo(m_FileUniqueId);
                        if (m_FileRow != null)
                        {
                            ConfigureResponse(context.Response, m_Expires);
                            context.Response.Write(GetObjectTag((m_FileRow.IsWidthNull() ? 0 : m_FileRow.Width), (m_FileRow.IsHeightNull() ? 0 : m_FileRow.Height), context.Request.Url.PathAndQuery, fileNameWithExtension));
                            context.Response.End();
                        }
                    }
                }
            }

            if ((file == null) && (m_FileRow != null))
                file = FileManager.GetFile(m_FileRow);

            if ((file != null) && (file.FileContents != null) && (file.FileContents.Length > 0))
            {
                ConfigureResponse(context.Response, m_Expires);
                context.Response.ContentType = fileMimeType;

                string contentDisposition = string.Empty;
                if (context.Request.Browser.IsBrowser("IE") || (context.Request.UserAgent != null && context.Request.UserAgent.Contains("Chrome")))
                    contentDisposition = "filename=\"" + FileManager.ToHexString(fileNameWithExtension) + "\";";
                else if ((context.Request.UserAgent != null && context.Request.UserAgent.Contains("Safari")))
                    contentDisposition = "filename=\"" + fileNameWithExtension + "\";";
                else
                    contentDisposition = "filename*=utf-8''" + HttpUtility.UrlPathEncode(fileNameWithExtension) + ";";

                if ((!(isImage || isFlash)) || download)
                    contentDisposition = "attachment;" + contentDisposition;
                context.Response.AddHeader("Content-Disposition", contentDisposition);

                context.Response.BinaryWrite(file.FileContents);
                context.Response.End();
            }
        }

        #endregion
    }
}