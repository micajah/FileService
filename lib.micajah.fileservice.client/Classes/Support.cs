using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Micajah.FileService.Client
{
    /// <summary>
    /// The helper class.
    /// </summary>
    public static class Support
    {
        #region Internal Methods

        internal static Guid GetGuidValue(string name)
        {
            object obj = Support.GetObjectValue(name);
            return ((obj == null) ? Guid.Empty : (Guid)obj);
        }

        internal static object GetObjectValue(string name)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpSessionState session = context.Session;
                if (session != null) return session[name];
            }
            return null;
        }

        internal static void SetObjectValue(string name, object value)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpSessionState session = context.Session;
                if (session != null) session[name] = value;
            }
        }

        #endregion

        #region Public Methods

        public static Guid CreateGuid(string value)
        {
            Guid guid = Guid.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    guid = new Guid(value);
                }
                catch (ArgumentNullException) { }
                catch (FormatException) { }
                catch (OverflowException) { }
            }

            return guid;
        }

        /// <summary>
        /// Converts a string to an array of bytes.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The array of bytes.</returns>
        public static byte[] GetBytes(string value)
        {
            byte[] bytes = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(value);
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Converts an array of bytes to a string.
        /// </summary>
        /// <param name="bytes">The array of bytes to convert.</param>
        /// <returns>The string.</returns>
        public static string GetString(byte[] value)
        {
            if (value == null) return null;
            if (value.Length == 0) return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

        #endregion
    }
}