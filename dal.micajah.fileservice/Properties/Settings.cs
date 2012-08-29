using System.Configuration;

namespace Micajah.FileService.Dal.Properties
{
    internal sealed partial class Settings
    {
        public Settings()
        {
            this["FileServiceConnectionString"] = ConfigurationManager.ConnectionStrings["Micajah.FileService.Server.ConnectionString"].ConnectionString;
        }
    }
}
