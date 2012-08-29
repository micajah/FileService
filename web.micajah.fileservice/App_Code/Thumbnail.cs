using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using Micajah.FileService.Dal;
using Micajah.FileService.Dal.MainDataSetTableAdapters;

namespace Micajah.FileService.WebService
{
    public static class Thumbnail
    {
        //   Align
        //
        //2 -- 3 -- 4 
        //|         |
        //|         |
        //9    1    5
        //|         |
        //|         |
        //8 -- 7 -- 6
        //0 - The original image will be returned as thumbnail if its width and height are not greater than specified values.

        #region Private Methods

        private static void DrawImage(Image image, int x, int y, int width, int height, ref Bitmap bitmap)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, x, y, width, height);
                graphics.Flush();
            }
        }

        private static void GetAlignPosition(int align, int maxWidth, int maxHeight, int localSizeX, int localSizeY, ref int left, ref int top)
        {
            left = 0;
            top = 0;

            switch (align)
            {
                case 1:
                    left = (maxWidth - localSizeX) / 2;
                    top = (maxHeight - localSizeY) / 2;
                    break;
                case 2:
                    left = 0;
                    top = 0;
                    break;
                case 3:
                    left = (maxWidth - localSizeX) / 2;
                    top = 0;
                    break;
                case 4:
                    left = maxWidth - localSizeX;
                    top = 0;
                    break;
                case 5:
                    left = maxWidth - localSizeX;
                    top = (maxHeight - localSizeY) / 2;
                    break;
                case 6:
                    left = maxWidth - localSizeX;
                    top = maxHeight - localSizeY;
                    break;
                case 7:
                    left = (maxWidth - localSizeX) / 2;
                    top = maxHeight - localSizeY;
                    break;
                case 8:
                    left = 0;
                    top = maxHeight - localSizeY;
                    break;
                case 9:
                    left = 0;
                    top = (maxHeight - localSizeY) / 2;
                    break;
            }
        }

        private static void GetProportionalSize(int originalWidth, int originalHeight, ref int width, ref int height)
        {
            double multiplier = (double)originalHeight / (double)originalWidth;

            if (height <= 0)
            {
                height = Convert.ToInt32(((multiplier > 1) ? (width / multiplier) : (width * multiplier)), CultureInfo.InvariantCulture);
                return;
            }
            else if (width <= 0)
            {
                width = Convert.ToInt32(((multiplier > 1) ? (height * multiplier) : (height / multiplier)), CultureInfo.InvariantCulture);
                return;
            }

            double widthPercent = ((originalWidth > 0) ? 100 * width / originalWidth : 100);
            double heightPercent = ((originalHeight > 0) ? 100 * height / originalHeight : 100);

            double currentPercent = ((widthPercent > heightPercent) ? heightPercent : widthPercent);
            if (currentPercent == 0)
            {
                if (heightPercent != 0)
                    currentPercent = heightPercent;
                else
                    currentPercent = widthPercent;
            }
            currentPercent = currentPercent / 100;

            width = Convert.ToInt32(originalWidth * currentPercent, CultureInfo.InvariantCulture);
            height = Convert.ToInt32(originalHeight * currentPercent, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Public Methods

        public static string Create(string sourceFileName, string sourceFileExtension, string destinationFileName, int width, int height, int align)
        {
            string outputExtension = string.Empty;

            if (!File.Exists(sourceFileName))
                return "Internal error: Source file not exist.";

            if (destinationFileName.Length == 0)
                return "Internal error: Destination path not defined.";

            if ((string.Compare(sourceFileExtension, ".bmp", StringComparison.OrdinalIgnoreCase) != 0)
                && (string.Compare(sourceFileExtension, ".tif", StringComparison.OrdinalIgnoreCase) != 0)
                && (string.Compare(sourceFileExtension, ".gif", StringComparison.OrdinalIgnoreCase) != 0)
                && (string.Compare(sourceFileExtension, ".png", StringComparison.OrdinalIgnoreCase) != 0)
                && (string.Compare(sourceFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase) != 0)
                && (string.Compare(sourceFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase) != 0))
            {
                return "Internal error: " + sourceFileExtension + " format not supported.";
            }

            Image originalImage = Image.FromFile(sourceFileName);

            if (align == 0)
            {
                if ((((width > 0) && (originalImage.Width <= width)) || (width <= 0))
                    && (((height > 0) && (originalImage.Height <= height)) || (height <= 0)))
                {
                    return string.Empty;
                }
            }

            outputExtension = ".jpg";
            int outputWidth = width;
            int outputHeight = height;
            GetProportionalSize(originalImage.Width, originalImage.Height, ref outputWidth, ref outputHeight);

            Bitmap scaledImage = new Bitmap(outputWidth, outputHeight);
            DrawImage(originalImage, 0, 0, outputWidth, outputHeight, ref scaledImage);

            if (align > 0)
            {
                if (width == 0) width = outputWidth;
                if (height == 0) height = outputHeight;
                int maxWidth = ((outputWidth > width) ? outputWidth : width);
                int maxHeight = ((outputHeight > height) ? outputHeight : height);

                int x = 0;
                int y = 0;
                GetAlignPosition(align, maxWidth, maxHeight, outputWidth, outputHeight, ref x, ref y);

                Bitmap outputImage = new Bitmap(maxWidth, maxHeight);
                DrawImage((Image)scaledImage, x, y, outputWidth, outputHeight, ref outputImage);
                outputImage.Save(destinationFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                outputImage.Dispose();
            }
            else
                scaledImage.Save(destinationFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            originalImage.Dispose();
            scaledImage.Dispose();

            return outputExtension;
        }

        public static GetFileResponse GetThumbnail(string fileId, int width, int height, int align, ref string result)
        {
            result = string.Empty;

            if ((width > 2000) || (height > 2000))
            {
                result = "Error: Width or Height greater 2000.";
                return FileManager.GetFile(fileId);
            }

            string thumbnailFileUniqueId = null;
            using (FileTableAdapter adapter = new FileTableAdapter())
            {
                thumbnailFileUniqueId = adapter.GetThumbnailFileUniqueId(fileId, width, height, align);
            }

            if (!string.IsNullOrEmpty(thumbnailFileUniqueId))
            {
                result = thumbnailFileUniqueId;
            }
            else
            {
                MainDataSet.FileRow row = FileManager.GetFileInfo(fileId);
                if (row != null)
                {
                    string applicationGuid = row.ApplicationGuid.ToString();
                    string organizationGuid = row.OrganizationGuid.ToString();
                    string departmentGuid = row.DepartmentGuid.ToString();
                    string fileNameWithExtension = row.NameWithExtension;
                    long sizeInBytes = (long)row.SizeInBytes;
                    string filePath = row.FilePath;
                    string storagePath = row.StoragePath;
                    string tempPath = storagePath + "Temp\\";

                    if (FileManager.EnsureDirectoryExists(tempPath))
                    {
                        string tempFileName = tempPath + Path.GetRandomFileName();
                        string fileExtension = string.Empty;

                        try
                        {
                            fileExtension = Create(filePath, row.FileExtension, tempFileName, width, height, align);
                        }
                        catch (Exception ex)
                        {
                            result = ex.Message.ToString();
                        }

                        if (fileExtension.Length == 0)
                            thumbnailFileUniqueId = fileId;
                        else if (result.Length == 0)
                        {
                            string tempFileNameNew = Path.GetFileNameWithoutExtension(tempFileName) + fileExtension;
                            string destinationFileName = string.Empty;
                            string destinationFolder = string.Empty;
                            Guid? storageGuid = null;

                            FileInfo fileInfo = new FileInfo(tempFileName);
                            if ((fileInfo != null) && (fileInfo.Exists)) sizeInBytes = fileInfo.Length;

                            int errorCode = FileManager.GetAccess(applicationGuid, organizationGuid, departmentGuid, sizeInBytes, ref destinationFolder, ref destinationFileName, ref thumbnailFileUniqueId, ref storageGuid);
                            if (errorCode == 0)
                            {
                                if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                                File.Move(tempFileName, destinationFileName); // Move from temporary to real location

                                tempFileNameNew = fileNameWithExtension;

                                using (FileTableAdapter adapter = new FileTableAdapter())
                                {
                                    adapter.UpdateFile(thumbnailFileUniqueId, fileId, fileExtension, MimeType.GetMimeType(fileExtension)
                                        , applicationGuid, storageGuid.Value.ToString(), organizationGuid, departmentGuid
                                        , Path.GetFileNameWithoutExtension(tempFileNameNew), (int)fileInfo.Length, width, height, align, row.ExpirationRequired, ref errorCode);
                                }

                                if (errorCode == 0)
                                    result = thumbnailFileUniqueId;
                                else
                                {
                                    result = FileManager.GetCreateError(errorCode);

                                    if (File.Exists(destinationFileName)) File.Delete(destinationFileName);

                                    FileManager.DeleteDirectories(destinationFolder, thumbnailFileUniqueId);
                                }
                            }
                            else
                            {
                                if (File.Exists(tempFileName)) File.Delete(tempFileName);

                                result = FileManager.GetAccessError(errorCode);
                            }
                        }
                    }
                    else
                        result = "Error: Temporary directory not exists.";
                }
                else
                    result = "Error: File not exists.";
            }

            return FileManager.GetFile(thumbnailFileUniqueId);
        }

        #endregion
    }
}