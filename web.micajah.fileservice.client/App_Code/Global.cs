using System;
using Micajah.FileService.Client;

namespace Micajah.FileService.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ResourceVirtualPathProvider.Register();
        }
    }
}