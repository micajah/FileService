using System;
using System.IO;
using Micajah.FileService.Client;

namespace Micajah.FileService.Web
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        private Guid OrganizationId = new Guid("f53995a0-2b8c-4c16-8be4-476495220ea6");
        private Guid DepartmentId = new Guid("96ca7b84-bdf7-4af2-b78c-29e60f6569fe");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                lblMetaDataResult.Text = string.Empty;

            //FileServiceUpload1.ApplicationGUID = txtApplicationGUID.Text;
            //FileServiceUpload1.DepartmentName = txtDepartmentName.Text;
            //FileServiceUpload1.OrganizationName = txtOrganizationName.Text;
            //FileServiceUpload1.FileTypes = txtAccessTypes.Text;

            //if (txtMaxCountFiles.Text.Length > 0)
            //    FileServiceUpload1.CountFiles = Int32.Parse(txtMaxCountFiles.Text);

            //if (txtUploadFileId.Text != "< Enter file id >")
            //    FileServiceUpload1.UploadedFiles = txtUploadFileId.Text;

            //if (!IsPostBack)
            //{
            //    txtUploadFilesLinkCaption.Text = FileServiceUpload1.UploadFilesLinkCaption;
            //    txtAddFileLinkCaption.Text = FileServiceUpload1.AddFileLinkCaption;
            //    txtDelFileLinkCaption.Text = FileServiceUpload1.DelFileLinkCaption;
            //    txtFileListCaption.Text = FileServiceUpload1.FileListCaption;
            //    txtDelFileLinkFromFileListCaption.Text = FileServiceUpload1.DelFileLinkFromFileListCaption;
            //};
        }

        protected void btnGetFile_Click(object sender, EventArgs e)
        {
            if (txtGetFile.Text.Length > 0)
                Response.Redirect("GetFile.aspx?FileId=" + txtGetFile.Text);
        }

        protected void btnPutFile_Click(object sender, EventArgs e)
        {
            string ApplicationGUID = txtApplicationGUID.Text;
            string OrganizationName = txtOrganizationName.Text;
            string DepartmentName = txtDepartmentName.Text;

            string file_path = txtFilePath.Text;

            string _organization_guid = txtOrganizationGUID.Text;
            string _department_guid = txtDepartmentGUID.Text;
            string checksum = null;

            string _unique_id = Access.PutFile(ApplicationGUID, OrganizationName, ref _organization_guid, DepartmentName, ref _department_guid, file_path, ref checksum);

            txtOrganizationGUID.Text = _organization_guid;
            txtDepartmentGUID.Text = _department_guid;

            txtFilePath.Text = "Output: " + _unique_id;
        }

        protected void btnGetThumbnail_Click(object sender, EventArgs e)
        {
            if ((txtGetThumbnail.Text.Length > 0) && (txtGetThumbnailWidth.Text.Length > 0) && (txtGetThumbnailHeight.Text.Length > 0))
            {
                int _size_x = 0;
                int _size_y = 0;
                int _align = 0;

                string _url = "";
                string _old_id = txtGetThumbnail.Text;
                try
                {
                    _size_x = Int32.Parse(txtGetThumbnailWidth.Text);
                    _size_y = Int32.Parse(txtGetThumbnailHeight.Text);
                    _align = Int32.Parse(txtThumbnailAlign.Text);

                    string _new_id = "";
                    Access.GetThumbnail(txtGetThumbnail.Text, _size_x, _size_y, _align, ref _new_id);
                    txtGetThumbnail.Text = "Output: " + _new_id;

                    _url = Access.GetThumbnailUrl(_old_id, OrganizationId, DepartmentId, _size_x, _size_y, _align);
                }
                catch
                {

                };

                Response.Redirect(_url);
            };
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string _result = "";
                _result = Access.DeleteFile(txtDeleteFile.Text);

                if (_result.Length == 0)
                    txtDeleteFile.Text = "Output: file deleted.";
                else
                    txtDeleteFile.Text = "Output: " + _result;
            }
            catch
            {

            };
        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                string _id = "-1";
                Access.DeleteFile(_id);
            }
            catch
            {

            };
        }

        protected void btnUpdateFile_Click(object sender, EventArgs e)
        {
            string file_path = txtUpdateFilePath.Text;
            string checksum = null;
            string _unique_id = Access.UpdateFile(txtUpdateFile.Text, file_path, ref checksum);
            txtUpdateFilePath.Text = "Output: " + _unique_id;
        }

        protected void btnPutFileAsByteArray_Click(object sender, EventArgs e)
        {
            string ApplicationGUID = txtApplicationGUID.Text;
            string OrganizationName = txtOrganizationName.Text;
            string DepartmentName = txtDepartmentName.Text;

            string file_path = txtPutFileAsByteArray.Text;

            if (!File.Exists(file_path))
            {
                txtPutFileAsByteArray.Text = "Output: File not exist!";
                return;
            };

            byte[] _byte_array = File.ReadAllBytes(file_path);

            string _organization_guid = txtOrganizationGUID.Text;
            string _department_guid = txtDepartmentGUID.Text;
            string checksum = null;

            string _unique_id = Access.PutFileAsByteArray(ApplicationGUID, OrganizationName, ref _organization_guid, DepartmentName, ref _department_guid, file_path, ref _byte_array, ref checksum);

            txtPutFileAsByteArray.Text = "Output: " + _unique_id;

            txtOrganizationGUID.Text = _organization_guid;
            txtDepartmentGUID.Text = _department_guid;
        }

        protected void btnUpdateAsArray_Click(object sender, EventArgs e)
        {
            string file_path = txtUpdateAsArrayPath.Text;

            if (!File.Exists(file_path))
            {
                txtUpdateAsArrayPath.Text = "Output: File not exist!";
                return;
            };

            byte[] _byte_array = File.ReadAllBytes(file_path);
            string checksum = null;
            string _unique_id = Access.UpdateFileAsByteArray(txtUpdateAsArray.Text, file_path, ref _byte_array,ref checksum);
            txtUpdateAsArrayPath.Text = "Output: " + _unique_id;
        }

        protected void btnFileURL_Click(object sender, EventArgs e)
        {
            txtGetFile.Text = Access.GetFileUrl(txtGetFile.Text, OrganizationId, DepartmentId);
        }

        protected void btnThumbnailURL_Click(object sender, EventArgs e)
        {
            int _width = 0;
            int _height = 0;
            int _align = 0;

            try
            {
                _width = Int32.Parse(txtGetThumbnailWidth.Text);
                _height = Int32.Parse(txtGetThumbnailHeight.Text);
                _align = Int32.Parse(txtThumbnailAlign.Text);
            }
            catch
            {
                txtGetThumbnail.Text = "Error: Cant convert width or height from string to int.";
                return;
            };

            txtGetThumbnail.Text = Access.GetThumbnailUrl(txtGetThumbnail.Text, OrganizationId, DepartmentId, _width, _height, _align);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            /*
            string _url = "";

            int file_id=0;
            string str_file_id = "";
            int max_count=0;

            try
            {
                if (txtUploadFileId.Text != "< Enter file id >")
                {
                    file_id = Int32.Parse(txtUploadFileId.Text);
                    str_file_id = txtUploadFileId.Text;
                };

                max_count = Int32.Parse(txtMaxCountFiles.Text);
            }
            catch
            {
                return;
            }

            _url = Access.GetUploadURL(txtApplicationGUID.Text, txtOrganizationName.Text, txtDepartmentName.Text, txtAccessTypes.Text, max_count, str_file_id.ToString());

            hlUpload.NavigateUrl = _url;

            FileServiceUpload1.ApplicationGUID = txtApplicationGUID.Text;
            FileServiceUpload1.DepartmentName = txtDepartmentName.Text;
            FileServiceUpload1.OrganizationName = txtOrganizationName.Text;
            FileServiceUpload1.FileTypes = txtAccessTypes.Text;

            if (txtMaxCountFiles.Text.Length > 0)
                FileServiceUpload1.CountFiles = Int32.Parse(txtMaxCountFiles.Text);

            if (txtUploadFileId.Text != "< Enter file id >")
                FileServiceUpload1.UploadedFiles = txtUploadFileId.Text;
            */
        }

        //protected void btnUploadCommit_Click(object sender, EventArgs e)
        //{
        //    string my_files = FileServiceUpload1.UploadedFiles;
        //    FileServiceUpload1.Commit();
        //}

        protected void btnSetStyle_Click(object sender, EventArgs e)
        {
            //FileServiceUpload1.UploadFilesLinkCaption = txtUploadFilesLinkCaption.Text;
            //FileServiceUpload1.AddFileLinkCaption = txtAddFileLinkCaption.Text;
            //FileServiceUpload1.DelFileLinkCaption = txtDelFileLinkCaption.Text;
            //FileServiceUpload1.FileListCaption = txtFileListCaption.Text;
            //FileServiceUpload1.DelFileLinkFromFileListCaption = txtDelFileLinkFromFileListCaption.Text;
        }

        protected void btnPutFileFromURL_Click(object sender, EventArgs e)
        {
            string ApplicationGUID = txtApplicationGUID.Text;
            string OrganizationName = txtOrganizationName.Text;
            string DepartmentName = txtDepartmentName.Text;

            string file_path = txtFileURL.Text;

            string _organization_guid = txtOrganizationGUID.Text;
            string _department_guid = txtDepartmentGUID.Text;
            string checksum = null;

            string _unique_id = Access.PutFileFromUrl(ApplicationGUID, OrganizationName, ref _organization_guid, DepartmentName, ref _department_guid, file_path, ref checksum);

            txtOrganizationGUID.Text = _organization_guid;
            txtDepartmentGUID.Text = _department_guid;

            txtFileURL.Text = "Output: " + _unique_id;
        }

        protected void btnUpdateFromURL_Click(object sender, EventArgs e)
        {
            string file_path = txtUpdateFromURLPath.Text;
            string checksum = null;
            string _unique_id = Access.UpdateFileFromUrl(txtUpdateFromURL.Text, file_path, ref checksum);
            txtUpdateFromURLPath.Text = "Output: " + _unique_id;
        }

        void ShowMetaError(string error)
        {
            lblMetaDataResult.Text = error;
        }

        //void ReadMetaDataFromPage(ref FileMetaDataItem file_item)
        //{
        //    if (file_item != null)
        //    {
        //        file_item.FileId = txtMetaFileId.Text;
        //        file_item.OrganizationId = new Guid(txtMetaOrganizationId.Text);
        //        file_item.DepartmentId = new Guid(txtMetaDepartmentId.Text);

        //        file_item.FileName = txtMetaFileName.Text;

        //        int _file_size = 0;
        //        if (!Int32.TryParse(txtMetaFileSize.Text, out _file_size))
        //            ShowMetaError("Can't convert FileSize to int value.");
        //        else
        //            file_item.FileSize = _file_size;

        //        file_item.FileComment = txtMetaFileComment.Text;
        //        file_item.LocalObjectId = txtMetaLocalObjectId.Text;
        //        file_item.LocalObjectType = txtMetaLocalObjectType.Text;

        //        int _updated_by_user_id = 0;
        //        if (!Int32.TryParse(txtMetaUpdatedByUserId.Text, out _updated_by_user_id))
        //            ShowMetaError("Can't convert UpdatedByUserId to int value.");
        //        else
        //            file_item.UpdatedByUserId = _updated_by_user_id;

        //        file_item.IsDeleted = chkDeleted.Checked;
        //    }
        //}

        //void WriteMetaDataToPage(ref FileMetaDataItem file_item)
        //{
        //    if (file_item != null)
        //    {
        //        lblMetaInternalId.Text = file_item.InternalId.ToString();

        //        txtMetaFileId.Text = file_item.FileId;
        //        txtMetaOrganizationId.Text = file_item.OrganizationId.ToString();
        //        txtMetaDepartmentId.Text = file_item.DepartmentId.ToString();

        //        txtMetaFileName.Text = file_item.FileName;
        //        txtMetaFileSize.Text = file_item.FileSize.ToString();

        //        txtMetaFileComment.Text = file_item.FileComment;
        //        txtMetaLocalObjectId.Text = file_item.LocalObjectId;
        //        txtMetaLocalObjectType.Text = file_item.LocalObjectType;

        //        txtMetaUpdatedByUserId.Text = file_item.UpdatedByUserId.ToString();
        //        lblMetaUpdatedTime.Text = file_item.UpdatedTime.ToString();

        //        chkDeleted.Checked = file_item.IsDeleted;
        //    };
        //}

        protected void btnLoadMetaData_Click(object sender, EventArgs e)
        {
            lblMetaDataResult.Text = string.Empty;

            //if (txtMetaFileId.Text.Length > 0)
            //{
            //    FileMetaDataItem _item = new FileMetaDataItem(txtMetaFileId.Text);//auto loading...

            //    if (_item.InternalId > 0)
            //        WriteMetaDataToPage(ref _item);
            //    else
            //        ShowMetaError("Can't load file metadata info...");
            //}
            //else
            //    ShowMetaError("FileId require for loading file metadata info...");
        }

        protected void btnSaveMetaData_Click(object sender, EventArgs e)
        {
            lblMetaDataResult.Text = string.Empty;

            //FileMetaDataItem _item = new FileMetaDataItem();
            //ReadMetaDataFromPage(ref _item);
            //if (_item != null)
            //{
            //    if (!_item.Save())
            //        ShowMetaError("Can't save file metadata info...");
            //    else
            //        WriteMetaDataToPage(ref _item);
            //};
        }

        protected void btnMetaDefault_Click(object sender, EventArgs e)
        {
            lblMetaDataResult.Text = string.Empty;
            txtMetaFileId.Text = string.Empty;
            lblMetaInternalId.Text = string.Empty;
            txtMetaOrganizationId.Text = "0";
            txtMetaDepartmentId.Text = string.Empty;
            txtMetaFileName.Text = string.Empty;
            txtMetaFileSize.Text = "0";
            txtMetaFileComment.Text = string.Empty;
            txtMetaLocalObjectId.Text = string.Empty;
            txtMetaLocalObjectType.Text = string.Empty;
            txtMetaUpdatedByUserId.Text = string.Empty;
            lblMetaUpdatedTime.Text = string.Empty;
            chkDeleted.Checked = false;
        }
    }
}
