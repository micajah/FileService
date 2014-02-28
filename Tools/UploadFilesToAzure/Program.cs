using Micajah.FileService.Tools.UploadFilesToAzure.FileService;
using Micajah.FileService.Tools.UploadFilesToAzure.MetaDataSetTableAdapters;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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

                // Read configuration.
                string cacheControl = string.Format(CultureInfo.InvariantCulture, "public, max-age={0}", ConfigurationManager.AppSettings["mafs:ClientCacheExpiryTime"]);
                int uploadSpeedLimit = Convert.ToInt32(ConfigurationManager.AppSettings["UploadSpeedLimit"]);
                int parallelOperationThreadCount = Convert.ToInt32(ConfigurationManager.AppSettings["ParallelOperationThreadCount"]);
                string storageConnectionString = ConfigurationManager.AppSettings["mafs:StorageConnectionString"];
                List<string> objectTypesWithPublicAccess = new List<string>(ConfigurationManager.AppSettings["ObjectTypesWithPublicAccess"].Split(','));

                // Initialize and configure the storage and client.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                blobClient.ParallelOperationThreadCount = parallelOperationThreadCount;
                blobClient.SingleBlobUploadThresholdInBytes = uploadSpeedLimit;

                CloudBlobContainer container = null;
                CloudBlobContainer publicContainer = null;
                Guid departmentId = Guid.Empty;
                int fileIndex = 0;
                int rowsCount = 0;

                // Get top 1000 files from the database.
                table = adapter.GetFiles();
                rowsCount = table.Count;

                while (rowsCount > 0)
                {
                    totalCount += rowsCount;

                    foreach (MetaDataSet.FileRow row in table)
                    {
                        int uploadStatus = 0; // 0 - Failed, 1 - Success, NULL - Not processed.
                        fileIndex++;

                        try
                        {
                            // Download the file from File Service.
                            Console.WriteLine("File #{0} \"{1}\" (fileUniqueId = {2}).", fileIndex, row.Name, row.FileUniqueId);
                            Console.Write("Getting from File Service...");

                            GetFileResponse fileResponse = client.GetFile(row.FileUniqueId);

                            Console.Write(" Done. ");
                            Console.Write("Uploading to Azure...");

                            // Get the corresponding container for the file and create it if not exists.
                            if (row.DepartmentId != departmentId)
                            {
                                publicContainer = null;
                                container = null;

                                departmentId = row.DepartmentId;
                            }

                            bool publicAccess = objectTypesWithPublicAccess.Contains(row.LocalObjectType);
                            CloudBlobContainer blobContainer = null;

                            if (publicAccess)
                            {
                                if (publicContainer == null)
                                {
                                    string containerName = string.Format(CultureInfo.InvariantCulture, "{0:N}p", row.DepartmentId);

                                    publicContainer = blobClient.GetContainerReference(containerName);
                                    publicContainer.CreateIfNotExists();

                                    BlobContainerPermissions p = new BlobContainerPermissions()
                                    {
                                        PublicAccess = BlobContainerPublicAccessType.Blob
                                    };
                                    publicContainer.SetPermissions(p);
                                }

                                blobContainer = publicContainer;
                            }
                            else
                            {
                                if (container == null)
                                {
                                    string containerName = row.DepartmentId.ToString("N");

                                    container = blobClient.GetContainerReference(containerName);
                                    container.CreateIfNotExists();
                                }

                                blobContainer = container;
                            }

                            string blobName = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", row.LocalObjectType, row.LocalObjectId, row.Name);
                            string mimeType = System.Web.MimeMapping.GetMimeMapping(row.Name);

                            // Upload to Azure.
                            using (Stream stream = new MemoryStream(fileResponse.fileData))
                            {
                                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
                                blob.Properties.ContentType = mimeType;
                                blob.Properties.CacheControl = cacheControl;
                                blob.UploadFromStream(stream);
                            }

                            uploadStatus = 1;
                            successCount++;

                            Console.WriteLine(" Done.\r\n");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(" Failed.\r\n{0}\r\n", ex.ToString());
                        }

                        // Update upload status of the file in the database.
                        adapter.UpdateUploadStatus(row.FileUniqueId, uploadStatus);
                    }

                    // Get next 1000 files from the database.
                    table = adapter.GetFiles();
                    rowsCount = table.Count;
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
