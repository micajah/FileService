using System;
using System.Text;
using Micajah.FileService.Client;
using Micajah.FileService.Client.Dal;
using Micajah.FileService.Client.Dal.MetaDataSetTableAdapters;

namespace Micajah.FileService.Tools.CopyFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Tool to copy the files from one organization to another.\r\n");

            // Help
            if (args.Length == 0)
            {
                System.Console.WriteLine(@"-a  <ApplicationId>             The unique identifier of the application. 
                                If it's not specified gets the Micajah.FileService.ApplicationGuid 
                                key's value from configuration file.
-o  <OrganizationId>            The unique identifier of the source organization.
-d  <DepartmentId>              The unique identifier of the source department.
-do <DestinationOrganizationId> The unique identifier of the destination organization.
-dd <DestinationDepartmentId>   The unique identifier of the destination department.");
                return;
            }

            Guid applicationId = Guid.Empty;
            Guid organizationId = Guid.Empty;
            Guid departmentId = Guid.Empty;
            Guid destinationOrganizationId = Guid.Empty;
            Guid destinationDepartmentId = Guid.Empty;

            // Parses the arguments
            int length = args.Length;
            for (int x = 0; x < length; x++)
            {
                string arg = args[x].ToLowerInvariant();
                string val = null;
                if (x < (length - 1)) val = args[x + 1];

                switch (arg)
                {
                    case "-a":
                        applicationId = Support.CreateGuid(val);
                        break;
                    case "-o":
                        organizationId = Support.CreateGuid(val);
                        break;
                    case "-d":
                        departmentId = Support.CreateGuid(val);
                        break;
                    case "-do":
                        destinationOrganizationId = Support.CreateGuid(val);
                        break;
                    case "-dd":
                        destinationDepartmentId = Support.CreateGuid(val);
                        break;
                }
            }

            // Gets from config
            if (applicationId == Guid.Empty)
                applicationId = Micajah.FileService.Client.Properties.Settings.Default.ApplicationId;

            StringBuilder sb = new StringBuilder();

            // Validates arguments
            if (applicationId == Guid.Empty)
                sb.AppendLine("ApplicationId is not valid or not specified.");

            if (organizationId == Guid.Empty)
                sb.AppendLine("OrganizationId is not valid or not specified.");

            if (departmentId == Guid.Empty)
                sb.AppendLine("DepartmentId is not valid or not specified.");

            if (destinationOrganizationId == Guid.Empty)
                sb.AppendLine("DestinationOrganizationId is not valid or not specified.");

            if (destinationDepartmentId == Guid.Empty)
                sb.AppendLine("DestinationDepartmentId is not valid or not specified.");

            if (sb.Length > 0)
            {
                System.Console.WriteLine(sb.ToString());
                return;
            }

            string connectionString = Micajah.FileService.Client.Properties.Settings.Default.MetaDataConnectionString;

            int totalCount = 0;
            int successCount = 0;

            // Copy the files
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                using (MetaDataSet.FileDataTable table = adapter.GetFiles(organizationId, departmentId))
                {
                    foreach (MetaDataSet.FileRow row in table)
                    {
                        System.Console.Write("\"{0}\" (fileUniqueId = {1}) file is copying...", row.Name, row.FileUniqueId);

                        string result = Access.CopyFile(applicationId, organizationId, departmentId, row.FileUniqueId, destinationOrganizationId, destinationDepartmentId, true, connectionString);

                        if (Access.StringIsFileUniqueId(result))
                        {
                            System.Console.WriteLine(@" Done. 
The fileUniqueId of the copied file is {0}.
", result);
                            successCount++;
                        }
                        else
                            System.Console.WriteLine(@" Failed.
{0}
"
                                , result);

                        totalCount++;
                    }
                }
            }

            System.Console.WriteLine(@"
Total files found: {0}
Copied: {1}
Failed: {2}"
                , totalCount, successCount, totalCount - successCount);
        }
    }
}
