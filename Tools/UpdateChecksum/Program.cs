using Micajah.FileService.Tools.UpdateChecksum.MetaDataSetTableAdapters;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Micajah.FileService.Tools.UpdateChecksum
{
    class Program
    {
        private static string CalculateChecksum(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
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

        static void Main(string[] args)
        {
            MainDataSetTableAdapters.FilesViewTableAdapter adapter = null;
            MainDataSet.FilesViewDataTable table = null;
            FileTableAdapter adapter2 = null;
            MetaDataSet.FileDataTable table2 = null;

            Console.WriteLine("Tool to update the checksum of the files.\r\n");

            try
            {
                Console.WriteLine("Step 1. Update checksum column in File table of FileService server database.\r\n");

                adapter = new MainDataSetTableAdapters.FilesViewTableAdapter();
                table = new MainDataSet.FilesViewDataTable();

                adapter.Fill(table);

                int totalCount = table.Count;
                int successCount = 0;

                foreach (MainDataSet.FilesViewRow row in table)
                {
                    string filePath = row.FilePath;
                    Console.Write("\"{0}\" file is fixing...", filePath);

                    try
                    {
                        row.Checksum = CalculateChecksum(filePath);
                        adapter.Update(row);

                        successCount++;

                        Console.WriteLine(" Done.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" Failed: {0}.", ex.ToString());
                    }
                }

                Console.WriteLine(@"
Total files found: {0}
Updated: {1}
Failed: {2}
"
                    , totalCount, successCount, totalCount - successCount);

                Console.WriteLine("Step 2. Update checksum column in Mfs_File table of metadata database.\r\n");

                totalCount = 0;
                successCount = 0;
                adapter2 = new FileTableAdapter();
                table2 = adapter2.GetData();

                foreach (MetaDataSet.FileRow row2 in table2)
                {
                    Console.Write("\"{0}\" (fileUniqueId = {1}) file is updating...", row2.Name, row2.FileUniqueId);

                    string fileNameWithExtension = null;
                    long sizeInBytes = 0;
                    int width = 0;
                    int height = 0;
                    int align = 0;
                    string mimeType = null;
                    string checksum = null;

                    if (Micajah.FileService.Client.Access.GetFileInfo(row2.FileUniqueId, ref  fileNameWithExtension, ref  sizeInBytes, ref  width, ref  height, ref  align, ref  mimeType, ref  checksum))
                    {
                        if (string.IsNullOrEmpty(checksum))
                            Console.WriteLine(" Failed: Checksum is empty.");
                        else
                        {
                            try
                            {
                                row2.Checksum = checksum;
                                adapter2.Update(row2);

                                successCount++;

                                Console.WriteLine(" Done.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(" Failed: {0}.", ex.ToString());
                            }
                        }
                    }
                    else
                        Console.WriteLine(" Failed.");

                    totalCount++;
                }

                Console.WriteLine(@"
Total files found: {0}
Updated: {1}
Failed: {2}"
                    , totalCount, successCount, totalCount - successCount);
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
                if (table != null) table.Dispose();
                if (adapter2 != null) adapter2.Dispose();
                if (table2 != null) table2.Dispose();
            }

            Console.WriteLine("\r\nPress any key to quit.");

            Console.ReadKey();
        }
    }
}
