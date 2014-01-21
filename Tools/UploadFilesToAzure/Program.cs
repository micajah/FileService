using Micajah.FileService.Tools.UploadFilesToAzure.FileService;
using Micajah.FileService.Tools.UploadFilesToAzure.MetaDataSetTableAdapters;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace Micajah.FileService.Tools.UploadFilesToAzure
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tool to upload the files from File Service to Windows Azure Blob Storage.\r\n");

            int totalCount = 0;
            int successCount = 0;

            FileMtomServiceSoapClient client = null;
            FileTableAdapter adapter = null;
            MetaDataSet.FileDataTable table = null;

            try
            {
                client = new FileMtomServiceSoapClient();
                adapter = new FileTableAdapter();

                table = adapter.GetFiles();
                totalCount = table.Count;

                Guid departmentId = Guid.Empty;
                string cacheControl = string.Format(CultureInfo.InvariantCulture, "public, max-age={0}", ConfigurationManager.AppSettings["mafs:ClientCacheExpiryTime"]);
                int uploadSpeedLimit = Convert.ToInt32(ConfigurationManager.AppSettings["UploadSpeedLimit"]);

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["mafs:StorageConnectionString"]);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                blobClient.ParallelOperationThreadCount = 1;
                blobClient.SingleBlobUploadThresholdInBytes = uploadSpeedLimit;
                CloudBlobContainer container = null;

                foreach (MetaDataSet.FileRow row in table)
                {
                    try
                    {
                        Console.WriteLine("File \"{0}\" (fileUniqueId = {1}).", row.Name, row.FileUniqueId);

                        Console.Write("Getting from File Service...");

                        GetFileResponse fileResponse = client.GetFile(row.FileUniqueId);

                        Console.Write(" Done. ");

                        Console.Write("Uploading to Azure...");

                        if (row.DepartmentId != departmentId)
                        {
                            container = blobClient.GetContainerReference(row.DepartmentId.ToString("N"));
                            container.CreateIfNotExists();

                            departmentId = row.DepartmentId;
                        }

                        string blobName = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", row.LocalObjectType, row.LocalObjectId, row.Name);
                        string mimeType = System.Web.MimeMapping.GetMimeMapping(row.Name);

                        using (Stream stream = new MemoryStream(fileResponse.fileData))
                        {
                            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);
                            blob.Properties.ContentType = mimeType;
                            blob.Properties.CacheControl = cacheControl;
                            blob.UploadFromStream(stream);
                        }

                        adapter.UpdateUploaded(true, row.FileUniqueId);

                        Console.WriteLine(" Done.\r\n");

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" Failed.\r\n{0}\r\n", ex.ToString());
                    }
                }

                Console.WriteLine(@"
Total files found: {0}
Uploaded: {1}
Failed: {2}"
                    , totalCount, successCount, totalCount - successCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\r\n{0}\r\n", ex.ToString());
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }

                if (adapter != null)
                {
                    adapter.Dispose();
                }

                if (table != null)
                {
                    table.Dispose();
                }
            }

            Console.WriteLine("\r\nPress any key to quit.");

            Console.ReadKey();
        }
    }
}
