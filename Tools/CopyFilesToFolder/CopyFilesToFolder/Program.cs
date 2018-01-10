using Micajah.FileService.Tools.CopyFilesToFolder.MetaDataSetTableAdapters;
using Micajah.FileService.Tools.CopyFilesToFolder.Properties;
using System;
using System.IO;
using System.Text;

namespace Micajah.FileService.Tools.CopyFilesToFolder
{
    class Program
    {
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

        static void Main(string[] args)
        {
            Console.WriteLine("Tool to copy the files of department filtered by local object types to output folder.\r\n\r\n");

            int totalCount = 0;
            int successCount = 0;
            int fileIndex = 0;

            FileTableAdapter adapter = null;
            MetaDataSet.FileDataTable table = null;

            try
            {
                adapter = new FileTableAdapter();

                table = adapter.GetFiles(Settings.Default.OrganizationId, Settings.Default.DepartmentId, Settings.Default.LocalObjectTypes);

                totalCount = table.Count;

                if (totalCount > 0)
                {
                    Console.WriteLine("Total files found: {0}\r\n", totalCount);
                }

                foreach (MetaDataSet.FileRow row in table)
                {
                    fileIndex++;

                    try
                    {
                        Console.Write("Copying file #{0} \"{1}\" (fileUniqueId = {2})...", fileIndex, row.Name, row.FileUniqueId);

                        string sourcePath = Settings.Default.StoragePath + GetPathFromUniqueName(row.FileUniqueId);
                        string sourceFileName = sourcePath + row.FileUniqueId;

                        if (!File.Exists(sourceFileName))
                        {
                            Console.WriteLine(" Failed.\r\nFile not found {0}\r\n", sourceFileName);

                            continue;
                        }

                        string outputPath = $"{Settings.Default.OutputPath}{row.LocalObjectType}\\{row.LocalObjectId}";
                        string destFileName = $"{outputPath}\\{row.Name}";

                        if (!Directory.Exists(outputPath))
                        {
                            Directory.CreateDirectory(outputPath);
                        }

                        File.Copy(sourceFileName, destFileName, true);

                        successCount++;

                        Console.WriteLine(" Done.\r\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" Failed.\r\n{0}\r\n", ex.ToString());
                    }
                }

                Console.WriteLine(@"
Total files found: {0}
Copied: {1}
Failed: {2}"
                    , totalCount, successCount, totalCount - successCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\n{0}\r\n", ex.ToString());
            }
            finally
            {
                adapter?.Dispose();
                table?.Dispose();
            }

            Console.WriteLine("\r\nPress any key to quit.");

            Console.ReadKey();
        }
    }
}
