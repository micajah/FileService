using System;

namespace Micajah.FileService.Tools.RestoreFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            MainDataSetTableAdapters.FileExtensionTableAdapter fileExtensionAdapter = null;
            MainDataSetTableAdapters.FileTableAdapter fileAdapter = null;
            MainDataSet.FileExtensionDataTable fileExtensionTable = null;
            MainDataSet.FileDataTable fileTable = null;

            Console.WriteLine("Tool to fix the data of the File Service after restoring it from metadata database.\r\n");

            int totalCount = 0;
            int successCount = 0;

            try
            {
                fileExtensionAdapter = new MainDataSetTableAdapters.FileExtensionTableAdapter();
                fileAdapter = new MainDataSetTableAdapters.FileTableAdapter();

                Console.Write("Step 1. Fills MimeType field of FileExtension table.");

                try
                {
                    fileExtensionTable = fileExtensionAdapter.GetData();
                    foreach (MainDataSet.FileExtensionRow fileExtensionRow in fileExtensionTable)
                    {
                        if (string.IsNullOrEmpty(fileExtensionRow.MimeType))
                            fileExtensionRow.MimeType = MimeType.GetMimeType(fileExtensionRow.FileExtension);
                    }

                    fileExtensionAdapter.Update(fileExtensionTable);

                    Console.WriteLine(" Done.\r\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Failed.\r\n{0}\r\n", ex.ToString());
                }

                Console.WriteLine("Step 2. Restoring the width and height of the Flash files.\r\n");

                fileExtensionTable = fileExtensionAdapter.GetFileExtension(".swf");

                if (fileExtensionTable.Count > 0)
                {
                    fileTable = fileAdapter.GetFilesByFileExtensionGuid(fileExtensionTable[0].FileExtensionGuid);
                    totalCount = fileTable.Count;

                    foreach (MainDataSet.FileRow fileRow in fileTable)
                    {
                        if (fileRow.IsHeightNull() || fileRow.IsWidthNull())
                        {
                            string filePath = fileRow.FilePath;
                            Console.Write("\"{0}\" file is fixing...", filePath);

                            try
                            {
                                SwfFileInfo swfFileInfo = new SwfFileInfo(filePath);
                                if (swfFileInfo.IsValid)
                                {
                                    fileAdapter.UpdateFileWidthHeight(swfFileInfo.Height, swfFileInfo.Width, fileRow.FileUniqueId);
                                    successCount++;

                                    Console.WriteLine(" Done.\r\n");
                                }
                                else
                                    Console.WriteLine(" Failed. Invalid flash file.\r\n");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(" Failed.\r\n{0}\r\n", ex.ToString());
                            }
                        }
                    }


                    Console.WriteLine(@"Total files found: {0}
Fixed: {1}
Failed: {2}"
                        , totalCount, successCount, totalCount - successCount);
                }
            }
            finally
            {
                if (fileExtensionAdapter != null) fileExtensionAdapter.Dispose();
                if (fileAdapter != null) fileAdapter.Dispose();
                if (fileExtensionTable != null) fileExtensionTable.Dispose();
                if (fileTable != null) fileTable.Dispose();
            }

            Console.WriteLine("\r\nPress any key to quit.");

            Console.ReadKey();
        }
    }
}
