using System;
using System.Configuration;

namespace Micajah.FileService.Dal
{
    public static class ConnectionStringProvider
    {
        #region Members

        private static string s_ConnectionString;

        #endregion

        #region Public Properties

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(s_ConnectionString)) s_ConnectionString = Properties.Settings.Default.FileServiceConnectionString;
                return s_ConnectionString;
            }
            set { Properties.Settings.Default["FileServiceConnectionString"] = s_ConnectionString = value; }
        }

        #endregion
    }
}
