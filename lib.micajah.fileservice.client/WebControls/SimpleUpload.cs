using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Micajah.FileService.Client;
using Micajah.FileService.Client.Dal;
using Micajah.FileService.Client.Dal.MetaDataSetTableAdapters;
using Micajah.FileService.Client.Properties;
using Telerik.Web.UI;

namespace Micajah.FileService.WebControls
{
    /// <summary>
    /// The control for single- and multi-file uploads.
    /// </summary>
    [Designer(typeof(System.Web.UI.Design.ControlDesigner))]
    [ToolboxData("<{0}:SimpleUpload runat=server></{0}:SimpleUpload>")]
    public class SimpleUpload : RadUpload, INamingContainer, IUploadControl
    {
        #region Members

        private RadProgressManager ProgressManager;
        private RadProgressArea ProgressArea;
        private Control UploadControl;
        private Image PaperClipImage;
        private CustomValidator Validator;
        private HiddenField DeletedFilesField;
        private HiddenField UploadedFilesField;
        private HiddenField UploadedByFlashFilesField;
        private HtmlGenericControl PreloadDiv;
        private HyperLink OpenLink;
        private Button UploadButton;
        private HtmlGenericControl UploadHolder;
        private HtmlGenericControl FlashUploadHolder;
        private HtmlGenericControl FlashUpload;
        private HtmlGenericControl ChangeModeMessageHolder;
        private HiddenField FilesMetaDataField;
        private HiddenField CurrentModeField;

        private List<string> m_ErrorMessages;
        private List<string> m_DeletedFileNames;
        private List<string> m_UploadedFileFullNames;
        private List<string> m_InvalidFileFullNames;
        private bool m_ChangesAcceptedOrRejected;
        private bool m_Reseted;
        private MetaDataSet.FileDataTable m_FilesMetaData;

        internal RadWindow PopupWindow;

        #endregion

        #region Private Properties

        private string[] AllowedFileExtensionsInternal
        {
            get
            {
                object obj = this.ViewState["AllowedFileExtensionsInternal"];
                return ((obj == null) ? null : (string[])obj);
            }
            set { this.ViewState["AllowedFileExtensionsInternal"] = value; }
        }

        private string[] AllowedMimeTypesInternal
        {
            get
            {
                object obj = this.ViewState["AllowedMimeTypesInternal"];
                return ((obj == null) ? null : (string[])obj);
            }
            set { this.ViewState["AllowedMimeTypesInternal"] = value; }
        }

        private string SessionId
        {
            get
            {
                object obj = ViewState["SessionId"];
                if (obj == null)
                    ViewState["SessionId"] = obj = Guid.NewGuid().ToString();
                return (string)obj;
            }
            set { this.ViewState["SessionId"] = value; }
        }

        private MetaDataSet.FileDataTable FilesMetaData
        {
            get
            {
                if (m_FilesMetaData == null)
                {
                    using (FileTableAdapter adapter = new FileTableAdapter(this.ConnectionString))
                    {
                        m_FilesMetaData = adapter.GetFiles(this.OrganizationId, this.DepartmentId, this.LocalObjectType, this.LocalObjectId, false);
                    }
                }
                return m_FilesMetaData;
            }
        }

        private string PopupWindowNavigateUrl
        {
            get
            {
                Hashtable table = new Hashtable();
                foreach (PropertyInfo p in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (p.CanWrite && (string.Compare(p.Name, "EnablePopupWindow", StringComparison.Ordinal) != 0)
                        && (string.Compare(p.Name, "UploadControlsUniqueId", StringComparison.Ordinal) != 0))
                    {
                        table[p.Name] = p.GetValue(this, null);
                    }
                }
                table["UploadButtonAcceptChanges"] = this.UploadButtonAcceptChanges;
                table["OnClientAfterAcceptChanges"] = this.OnClientAfterAcceptChanges;
                table["SessionId"] = this.SessionId;

                return ResourceVirtualPathProvider.VirtualPathToAbsolute(ResourceVirtualPathProvider.VirtualRootShortPath + "SimpleUpload.aspx")
                    + "?p=" + HttpUtility.UrlEncode(Serialize(table));
            }
        }

        private string FlashFileUploadUrl
        {
            get
            {
                object[] objs = new object[7];
                objs[0] = Settings.Default.ApplicationId.ToString();
                objs[1] = this.OrganizationId.ToString();
                objs[2] = this.OrganizationName;
                objs[3] = this.DepartmentId.ToString();
                objs[4] = this.DepartmentName;
                objs[5] = this.SessionId;
                objs[6] = Settings.Default.LinksExpiration;
                return Settings.Default.FilePageUrl + "?P=" + HttpUtility.UrlEncode(Serialize(objs));
            }
        }

        #endregion

        #region Internal Properties

        internal bool SkipUploadControlsValidation
        {
            get
            {
                object obj = this.ViewState["SkipUploadControlsValidation"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { this.ViewState["SkipUploadControlsValidation"] = value; }
        }

        internal bool UploadButtonAcceptChanges
        {
            get
            {
                object obj = this.ViewState["UploadButtonAcceptChanges"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { this.ViewState["UploadButtonAcceptChanges"] = value; }
        }

        internal string OnClientAfterAcceptChanges
        {
            get
            {
                object obj = this.ViewState["OnClientAfterAcceptChanges"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set { this.ViewState["OnClientAfterAcceptChanges"] = value; }
        }

        #endregion

        #region Overriden Properties

        public new string[] AllowedFileExtensions
        {
            get
            {
                string[] array = this.AllowedFileExtensionsInternal;
                return ((array == null) ? base.AllowedFileExtensions : array);
            }
            set
            {
                this.AllowedFileExtensionsInternal = null;
                if (value != null)
                {
                    List<string> list = new List<string>(value);
                    if (list.Count > 0)
                    {
                        switch (list[0].ToUpperInvariant())
                        {
                            case "VIDEO":
                                list = new List<string>(MimeType.VideoExtensions);
                                list.Add(".swf");

                                this.AllowedFileExtensionsInternal = value;
                                value = list.ToArray();
                                break;
                            case "IMAGE":
                                list = new List<string>(MimeType.ImageExtensions);

                                this.AllowedFileExtensionsInternal = value;
                                value = list.ToArray();
                                break;
                        }
                    }
                }
                base.AllowedFileExtensions = value;
            }
        }

        public new string[] AllowedMimeTypes
        {
            get
            {
                string[] array = this.AllowedFileExtensionsInternal;
                return ((array == null) ? base.AllowedMimeTypes : array);
            }
            set
            {
                this.AllowedMimeTypesInternal = null;
                if (value != null)
                {
                    List<string> list = new List<string>(value);
                    if (list.Count > 0)
                    {
                        switch (list[0].ToUpperInvariant())
                        {
                            case "VIDEO":
                                list = new List<string>(MimeType.VideoExtensions);
                                list.Add(".swf");

                                this.AllowedMimeTypesInternal = value;
                                value = MimeType.GetMimeTypes(list);
                                break;
                            case "IMAGE":
                                list = new List<string>(MimeType.ImageExtensions);

                                this.AllowedMimeTypesInternal = value;
                                value = MimeType.GetMimeTypes(list);
                                break;
                        }
                    }
                }
                base.AllowedMimeTypes = value;
            }
        }

        [Browsable(false)]
        [DefaultValue(ControlObjectsVisibility.None)]
        public new ControlObjectsVisibility ControlObjectsVisibility
        {
            get { return ControlObjectsVisibility.None; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public new bool EnableEmbeddedSkins
        {
            get { return false; }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        public new bool EnableFileInputSkinning
        {
            get { return false; }
        }

        [Browsable(false)]
        [DefaultValue(1)]
        public new int InitialFileInputsCount
        {
            get { return 1; }
        }

        [Browsable(false)]
        [DefaultValue(true)]
        public new bool ReadOnlyFileInputs
        {
            get { return true; }
        }

        [Browsable(false)]
        public new bool IsUploadModuleRegistered
        {
            get { return base.IsUploadModuleRegistered; }
        }

        [Browsable(false)]
        public new int MaxFileInputsCount
        {
            get { return 0; }
        }

        [Browsable(false)]
        public new int MaxFileSize
        {
            get { return 0; }
        }

        [Browsable(false)]
        [DefaultValue("SimpleUpload")]
        public new string Skin
        {
            get { return "SimpleUpload"; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the mode of the file selector.
        /// </summary>
        [Category("Behavior")]
        [Description("The mode of the file selector.")]
        [DefaultValue(UploadFileSelectorMode.SingleFile)]
        public UploadFileSelectorMode FileSelectorMode
        {
            get
            {
                UploadFileSelectorMode mode = UploadFileSelectorMode.SingleFile;
                this.EnsureChildControls();
                if (CurrentModeField != null)
                {
                    int im = 0;
                    if (int.TryParse(CurrentModeField.Value, out im))
                        mode = (UploadFileSelectorMode)im;
                }
                return mode;
            }
            set
            {
                this.EnsureChildControls();
                if (CurrentModeField != null)
                    CurrentModeField.Value = Convert.ToString((int)value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the names of the deleted files.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> DeletedFileNames
        {
            get
            {
                this.EnsureDeletedFileNames();
                return m_DeletedFileNames.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or set a value indicating whether the file service is used for managing the files.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the file service is used for managing the files.")]
        [DefaultValue(true)]
        public bool EnableFileService
        {
            get
            {
                object obj = ViewState["EnableFileService"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["EnableFileService"] = value; }
        }

        /// <summary>
        /// Gets or set value indicating whether the control is displayed in the pop-up window.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the control is displayed in the pop-up window.")]
        [DefaultValue(false)]
        public bool EnablePopupWindow
        {
            get
            {
                object obj = ViewState["EnablePopupWindow"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["EnablePopupWindow"] = value; }
        }

        /// <summary>
        /// Gets the messages that describes the errors, if it occured.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> ErrorMessages
        {
            get { return m_ErrorMessages.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a value indicating that an error occurred.
        /// </summary>
        [Browsable(false)]
        public bool ErrorOccurred
        {
            get { return (this.ErrorMessages.Count > 0); }
        }

        /// <summary>
        /// Gets the files count in the control.
        /// </summary>
        [Browsable(false)]
        public int FilesCount
        {
            get { return (this.EnableFileService ? this.GetFilesCount() : 0); }
        }

        /// <summary>
        /// Gets or sets the tab index of the file input field.
        /// </summary>
        [Category("Behavior")]
        [Description("The tab index of the file input field.")]
        [DefaultValue(0)]
        public virtual short InputTabIndex
        {
            get
            {
                object obj = this.ViewState["InputTabIndex"];
                return ((obj == null) ? (short)0 : (short)obj);
            }
            set { this.ViewState["InputTabIndex"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the file input field.
        /// </summary>
        [Category("Behavior")]
        [Description("The width of the file input field.")]
        [DefaultValue(0)]
        public virtual Unit InputWidth
        {
            get
            {
                object obj = this.ViewState["InputWidth"];
                return ((obj == null) ? Unit.Empty : (Unit)obj);
            }
            set { this.ViewState["InputWidth"] = value; }
        }

        /// <summary>
        /// Gets a value indicating that the control is empty.
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty
        {
            get { return (this.FilesCount == 0); }
        }

        /// <summary>
        /// Gets or sets the type of the object which the uploaded files are associated with.
        /// </summary>
        [Category("Data")]
        [Description("The type of the object which the uploaded files are associated with.")]
        [DefaultValue("")]
        public string LocalObjectType
        {
            get
            {
                object obj = this.ViewState["LocalObjectType"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set
            {
                if ((!string.IsNullOrEmpty(this.LocalObjectType)) && (string.Compare(this.LocalObjectType, value, StringComparison.Ordinal) != 0))
                    this.Reset();
                this.ViewState["LocalObjectType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the object which the uploaded files are associated with.
        /// </summary>
        [Category("Data")]
        [Description("The unique identifier of the object which the uploaded files are associated with.")]
        [DefaultValue("")]
        public string LocalObjectId
        {
            get
            {
                object obj = this.ViewState["LocalObjectId"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set
            {
                if ((!string.IsNullOrEmpty(this.LocalObjectId)) && (string.Compare(this.LocalObjectId, value, StringComparison.Ordinal) != 0))
                    this.Reset();
                this.ViewState["LocalObjectId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum files count that can be selected by user in the control.
        /// The default value is 0 that indicates the maximum files count is not set.
        /// </summary>
        [Description("The maximum files count that can be selected by user in the control.")]
        [Category("Behavior")]
        [DefaultValue(0)]
        public int MaxFilesCount
        {
            get
            {
                object obj = this.ViewState["MaxFilesCount"];
                return ((obj == null) ? 0 : (int)obj);
            }
            set { this.ViewState["MaxFilesCount"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether the error message is displayed in the control.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the error message is displayed in the control.")]
        [DefaultValue(true)]
        public bool ShowErrorMessage
        {
            get
            {
                object obj = ViewState["ShowErrorMessage"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["ShowErrorMessage"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether the paperclip image is displayed in the control.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the paperclip image is displayed in the control.")]
        [DefaultValue(true)]
        public bool ShowPaperclipImage
        {
            get
            {
                object obj = ViewState["ShowPaperclipImage"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["ShowPaperclipImage"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether the progress dialog is displayed when files are uploaded.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the progress dialog is displayed when files are uploaded.")]
        [DefaultValue(false)]
        public bool ShowProgressArea
        {
            get
            {
                object obj = ViewState["ShowProgressArea"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["ShowProgressArea"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether the title is displayed.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the title is displayed.")]
        [DefaultValue(true)]
        public bool ShowTitle
        {
            get
            {
                object obj = ViewState["ShowTitle"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["ShowTitle"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating whether the uploaded files are displayed in the control.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the uploaded files are displayed in the control.")]
        [DefaultValue(true)]
        public bool ShowUploadedFiles
        {
            get
            {
                object obj = ViewState["ShowUploadedFiles"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["ShowUploadedFiles"] = value; }
        }

        /// <summary>
        /// Gets or sets the group of controls for which the control causes validation when it posts back to the server.
        /// </summary>
        [Category("Behavior")]
        [Description("The group of controls for which the control causes validation when it posts back to the server.")]
        [DefaultValue("")]
        [Themeable(false)]
        public string ValidationGroup
        {
            get
            {
                object obj = this.ViewState["ValidationGroup"];
                return ((obj == null) ? string.Empty : (string)obj);
            }
            set
            {
                this.ViewState["ValidationGroup"] = value;
                this.EnsureChildControls();
                if (Validator != null) Validator.ValidationGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique identifiers of the controls which upload the selected files.
        /// </summary>
        [Category("Behavior")]
        [Description("The unique identifiers of the controls which upload the selected files.")]
        [DefaultValue(typeof(string[]), "")]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] UploadControlsUniqueId
        {
            get
            {
                object obj = this.ViewState["UploadControlsUniqueId"];
                return ((obj == null) ? new string[0] : (string[])obj);
            }
            set { this.ViewState["UploadControlsUniqueId"] = value; }
        }

        /// <summary>
        /// Gets or sets the identifier of the user.
        /// </summary>
        [Category("Data")]
        [Description("The identifier of the user.")]
        [DefaultValue("")]
        public string UpdatedBy
        {
            get
            {
                object obj = this.ViewState["UpdatedBy"];
                return ((obj == null) ? null : (string)obj);
            }
            set { this.ViewState["UpdatedBy"] = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the organization.
        /// </summary>
        [Category("Data")]
        [Description("The unique identifier of the organization.")]
        [DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
        public Guid OrganizationId
        {
            get
            {
                object obj = this.ViewState["OrganizationId"];
                return ((obj == null) ? Guid.Empty : (Guid)obj);
            }
            set { this.ViewState["OrganizationId"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        [Category("Data")]
        [Description("The name of the organization.")]
        [DefaultValue("")]
        public string OrganizationName
        {
            get
            {
                object obj = this.ViewState["OrganizationName"];
                return ((obj == null) ? null : (string)obj);
            }
            set { this.ViewState["OrganizationName"] = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the department.
        /// </summary>
        [Category("Data")]
        [Description("The unique identifier of the department.")]
        [DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
        public Guid DepartmentId
        {
            get
            {
                object obj = this.ViewState["DepartmentId"];
                return ((obj == null) ? Guid.Empty : (Guid)obj);
            }
            set { this.ViewState["DepartmentId"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the department.
        /// </summary>
        [Category("Data")]
        [Description("The name of the department.")]
        [DefaultValue("")]
        public string DepartmentName
        {
            get
            {
                object obj = this.ViewState["DepartmentName"];
                return ((obj == null) ? null : (string)obj);
            }
            set { this.ViewState["DepartmentName"] = value; }
        }

        /// <summary>
        /// Gets or sets the connection string to the metadata database.
        /// </summary>
        [Category("Data")]
        [Description("The connection string to the metadata database.")]
        [DefaultValue("")]
        public string ConnectionString
        {
            get
            {
                object obj = this.ViewState["ConnectionString"];
                return ((obj == null) ? Settings.Default.MetaDataConnectionString : (string)obj);
            }
            set { this.ViewState["ConnectionString"] = value; }
        }

        /// <summary>
        /// Gets the fully qualified names on the client of the successfully uploaded files.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> UploadedFileFullNames
        {
            get { return m_UploadedFileFullNames.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the names of the successfully uploaded files.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> UploadedFileNames
        {
            get { return GetFileNames(this.UploadedFileFullNames); }
        }

        /// <summary>
        /// Gets the fully qualified names on the client of the invalid files.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> InvalidFileFullNames
        {
            get { return m_InvalidFileFullNames.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the fully qualified names on the client of the invalid files.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<string> InvalidFileNames
        {
            get { return GetFileNames(this.InvalidFileFullNames); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Preloads images to fix the Google Chrome and Safari bug with background images.
        /// </summary>
        private void AddPreloadDiv()
        {
            PreloadDiv = new HtmlGenericControl("div");
            PreloadDiv.ID = "PreloadDiv";
            PreloadDiv.Style[HtmlTextWriterStyle.Display] = "none";

            HtmlImage img = new HtmlImage();
            img.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(RadProgressArea), "Telerik.Web.UI.Skins.Default.Upload.ruSprite.png");
            PreloadDiv.Controls.Add(img);

            img = new HtmlImage();
            img.Src = this.Page.ClientScript.GetWebResourceUrl(typeof(RadProgressArea), "Telerik.Web.UI.Skins.Default.Upload.ruProgress.gif");
            PreloadDiv.Controls.Add(img);

            this.Controls.Add(PreloadDiv);
        }

        private void CreateEmbeddedControl()
        {
            PaperClipImage = new Image();
            PaperClipImage.ID = "PaperClipImage";
            PaperClipImage.ImageUrl = ResourceHandler.GetWebResourceUrl("Images.PaperClip.gif", true);
            PaperClipImage.CssClass = "suPaperClip";
            this.Controls.Add(PaperClipImage);

            UploadHolder = new HtmlGenericControl("div");
            UploadHolder.ID = "UploadHolder";
            UploadHolder.Style[HtmlTextWriterStyle.Height] = "24px";
            UploadHolder.Style[HtmlTextWriterStyle.Display] = "inline";

            FlashUploadHolder = new HtmlGenericControl("div");
            FlashUploadHolder.ID = "FlashUploadHolder";
            FlashUploadHolder.Style[HtmlTextWriterStyle.Position] = "absolute";
            FlashUploadHolder.Style[HtmlTextWriterStyle.Overflow] = "hidden";
            FlashUploadHolder.Style[HtmlTextWriterStyle.Width] = "1px";
            FlashUploadHolder.Style[HtmlTextWriterStyle.Height] = "1px";

            FlashUpload = new HtmlGenericControl("span");
            FlashUpload.ID = "FlashUpload";

            FlashUploadHolder.Controls.Add(FlashUpload);
            UploadHolder.Controls.Add(FlashUploadHolder);
            this.Controls.Add(UploadHolder);

            ChangeModeMessageHolder = new HtmlGenericControl("div");
            ChangeModeMessageHolder.ID = "ChangeModeMessageHolder";
            ChangeModeMessageHolder.Attributes["class"] = "suChangeModeMessage";
            this.Controls.Add(ChangeModeMessageHolder);

            Validator = new CustomValidator();
            Validator.ID = "Validator";
            Validator.Display = ValidatorDisplay.Dynamic;
            Validator.CssClass = "suError";
            Validator.ServerValidate += new ServerValidateEventHandler(Validator_ServerValidate);
            this.Controls.Add(Validator);

            UploadButton = new Button();
            UploadButton.ID = "UploadButton";
            UploadButton.CssClass = "suUploadButton";
            UploadButton.Style[HtmlTextWriterStyle.Display] = "none";
            UploadButton.Text = Resources.SimpleUpload_UploadButton_Text;
            this.Controls.Add(UploadButton);
            UploadButton.Click += new EventHandler(UploadButton_Click);
        }

        private void CreatePopupWindow()
        {
            OpenLink = new HyperLink();
            OpenLink.ID = "OpenLink";
            OpenLink.NavigateUrl = "javascript:void(0);";
            OpenLink.Text = Resources.SimpleUpload_OpenLink_Text;
            this.Controls.Add(OpenLink);

            PopupWindow = new RadWindow();
            PopupWindow.ID = "PopupWindow";
            PopupWindow.Behaviors = (WindowBehaviors.Close | WindowBehaviors.Move);
            PopupWindow.Width = Unit.Pixel(530);
            PopupWindow.Height = Unit.Pixel(370);
            PopupWindow.OpenerElementID = OpenLink.ClientID;
            PopupWindow.Title = Resources.SimpleUpload_OpenLink_Text;
            PopupWindow.VisibleStatusbar = false;
            this.Controls.Add(PopupWindow);
        }

        private void DeleteFiles()
        {
            if (!string.IsNullOrEmpty(DeletedFilesField.Value))
            {
                foreach (string fileUniqueId in DeletedFilesField.Value.Split('|'))
                {
                    string result = Access.DeleteFile(fileUniqueId);
                    if (string.IsNullOrEmpty(result))
                    {
                        MetaDataSet.FileRow row = this.FilesMetaData.FindByFileUniqueIdOrganizationIdDepartmentId(fileUniqueId, this.OrganizationId, this.DepartmentId);
                        if (row != null)
                        {
                            if (this.UpdatedBy == null)
                                row.SetUpdatedByNull();
                            else
                                row.UpdatedBy = this.UpdatedBy;
                            row.Deleted = true;
                        }
                    }
                    else if (!m_ErrorMessages.Contains(result))
                        m_ErrorMessages.Add(result);
                }
            }
        }

        private void EnsureDeletedFileNames()
        {
            if (m_DeletedFileNames == null)
            {
                m_DeletedFileNames = new List<string>();
                foreach (string fileUniqueId in DeletedFilesField.Value.Split('|'))
                {
                    MetaDataSet.FileRow row = this.FilesMetaData.FindByFileUniqueIdOrganizationIdDepartmentId(fileUniqueId, this.OrganizationId, this.DepartmentId);
                    if (row != null)
                        m_DeletedFileNames.Add(row.Name);
                }
            }
        }

        private int GetFilesCount()
        {
            int filesCount = this.FilesMetaData.Count;
            if (this.Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(DeletedFilesField.Value))
                    filesCount -= DeletedFilesField.Value.Split('|').Length;
            }
            return filesCount;
        }

        private ReadOnlyCollection<string> GetFileNames(ReadOnlyCollection<string> fileFullNames)
        {
            List<string> list = new List<string>();
            foreach (string fullFileName in fileFullNames)
            {
                string fileName = Path.GetFileName(fullFileName);
                list.Add(fileName);
            }
            return list.AsReadOnly();
        }

        private void LoadFilesMetaData()
        {
            StringBuilder sb = new StringBuilder();
            List<string> deletedFiles = new List<string>(DeletedFilesField.Value.Split('|'));

            foreach (MetaDataSet.FileRow file in this.FilesMetaData)
            {
                if (!deletedFiles.Contains(file.FileUniqueId))
                    sb.AppendFormat("|{0}*{1}*{2}", file.FileUniqueId, file.Name, Access.GetFileUrl(file.FileUniqueId, this.OrganizationId, this.DepartmentId));
            }

            if (sb.Length > 0)
            {
                sb.Remove(0, 1);
                UploadedFilesField.Value = sb.ToString();
            }
        }

        private void LoadFilesMetaDataValue()
        {
            if (FilesMetaDataField == null) return;

            object obj = Deserialize(FilesMetaDataField.Value);
            if (obj != null)
            {
                m_FilesMetaData = new MetaDataSet.FileDataTable();
                LoadFilesMetaDataValue((obj as List<object>), false);
            }
        }

        private void LoadUploadedByFlashFilesMetaData()
        {
            if (UploadedByFlashFilesField == null) return;

            List<object> list = new List<object>();
            foreach (string part in UploadedByFlashFilesField.Value.Split('|'))
            {
                object obj = Deserialize(part);
                if (obj != null) list.Add(obj);
            }

            if (list.Count > 0)
            {
                if (m_FilesMetaData == null) m_FilesMetaData = new MetaDataSet.FileDataTable();
                LoadFilesMetaDataValue(list, true);
            }

            UploadedByFlashFilesField.Value = string.Empty;
        }

        private void LoadFilesMetaDataValue(List<object> list, bool byFlash)
        {
            foreach (object[] objs in list)
            {
                if (byFlash)
                {
                    string errorMessage = (string)objs[4];
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        if (!m_ErrorMessages.Contains(errorMessage))
                            m_ErrorMessages.Add(errorMessage);
                        continue;
                    }
                }

                MetaDataSet.FileRow row = m_FilesMetaData.NewFileRow();
                row.FileUniqueId = (string)objs[0];
                row.OrganizationId = this.OrganizationId;
                row.DepartmentId = this.DepartmentId;
                row.LocalObjectType = this.LocalObjectType;
                row.LocalObjectId = this.LocalObjectId;
                row.Name = (string)objs[1];
                row.SizeInBytes = (int)objs[2];
                row.SizeInKB = ((double)row.SizeInBytes / 1024);
                row.UpdatedBy = this.UpdatedBy;
                if (byFlash)
                {
                    row.Deleted = false;
                    row.IsTemporary = true;
                    row.Checksum = (string)objs[3];

                    m_UploadedFileFullNames.Add((string)objs[1]);
                }
                else
                {
                    row.Deleted = (bool)objs[3];
                    row.IsTemporary = (bool)objs[4];
                }
                row.UpdatedTime = DateTime.UtcNow;
                m_FilesMetaData.AddFileRow(row);
                if (!row.IsTemporary) row.AcceptChanges();
            }
        }

        private void RenderTitle(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "suTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.Div); // Div
            writer.Write(Resources.SimpleUpload_Title + " ");
            writer.RenderEndTag(); // Div
        }

        private void SaveFilesMetaDataValue()
        {
            List<object> objList = new List<object>();
            foreach (MetaDataSet.FileRow row in this.FilesMetaData)
            {
                objList.Add(new object[] { row.FileUniqueId, row.Name, row.SizeInBytes, row.Deleted, row.IsTemporary });
            }

            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                LosFormatter formatter = new LosFormatter();
                formatter.Serialize(writer, objList);
            }
            FilesMetaDataField.Value = sb.ToString();
        }

        private string UpdateFilesMetaData()
        {
            FileTableAdapter adapter = null;
            try
            {
                adapter = new FileTableAdapter(this.ConnectionString);
                adapter.Update(this.FilesMetaData);

                foreach (DataRow row in this.FilesMetaData.Select("Deleted = 1"))
                {
                    this.FilesMetaData.Rows.Remove(row);
                }
            }
            catch (DBConcurrencyException ex)
            {
                return ex.Message;
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
            }

            return string.Empty;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (this.UploadButtonAcceptChanges) this.AcceptChanges();
        }

        private void UploadFiles()
        {
            bool invalidFileExtension = false;
            bool invalidFileSize = false;
            bool invalidMimeType = false;

            foreach (UploadedFile file in this.UploadedFiles)
            {
                if (file.ContentLength > 0)
                {
                    int fileSize = (int)file.InputStream.Length;
                    byte[] bytes = new byte[fileSize];
                    string checksum = null;
                    file.InputStream.Read(bytes, 0, fileSize);

                    string organizationGuidString = this.OrganizationId.ToString();
                    string departmentGuidString = this.DepartmentId.ToString();
                    string uniqueId = Access.PutFileAsByteArray(Settings.Default.ApplicationId.ToString()
                        , this.OrganizationName, ref organizationGuidString
                        , this.DepartmentName, ref departmentGuidString
                        , file.FileName, ref bytes, ref checksum);

                    if (Access.StringIsFileUniqueId(uniqueId))
                    {
                        string result = Access.SetTemporaryFile(uniqueId, this.SessionId);
                        if (string.IsNullOrEmpty(result))
                        {
                            MetaDataSet.FileRow row = this.FilesMetaData.NewFileRow();
                            row.FileUniqueId = uniqueId;
                            row.OrganizationId = this.OrganizationId;
                            row.DepartmentId = this.DepartmentId;
                            row.LocalObjectType = this.LocalObjectType;
                            row.LocalObjectId = this.LocalObjectId;
                            row.Name = file.GetName();
                            row.SizeInBytes = fileSize;
                            row.SizeInKB = ((double)row.SizeInBytes / 1024);
                            row.UpdatedBy = this.UpdatedBy;
                            row.Deleted = false;
                            row.IsTemporary = true;
                            row.UpdatedTime = DateTime.UtcNow;
                            row.Checksum = checksum;
                            this.FilesMetaData.AddFileRow(row);

                            m_UploadedFileFullNames.Add(file.FileName);

                            continue;
                        }
                        else if (!m_ErrorMessages.Contains(result))
                            m_ErrorMessages.Add(result);
                    }
                    else if (!m_ErrorMessages.Contains(uniqueId))
                        m_ErrorMessages.Add(uniqueId);
                }
                else
                    invalidFileSize = true;

                if (!m_InvalidFileFullNames.Contains(file.FileName))
                    m_InvalidFileFullNames.Add(file.FileName);
            }

            foreach (UploadedFile file in this.InvalidFiles)
            {
                if (!this.IsValidExtension(file)) invalidFileExtension = true;
                if (!this.IsValidMimeType(file)) invalidMimeType = true;
                if (!this.IsValidSize(file)) invalidFileSize = true;

                m_InvalidFileFullNames.Add(file.FileName);
            }

            if (invalidFileExtension && (!m_ErrorMessages.Contains(Resources.SimpleUpload_InvalidFileExtension)))
                m_ErrorMessages.Add(Resources.SimpleUpload_InvalidFileExtension);
            if (invalidMimeType && (!m_ErrorMessages.Contains(Resources.SimpleUpload_InvalidMimeType)))
                m_ErrorMessages.Add(Resources.SimpleUpload_InvalidMimeType);
            if (invalidFileSize && (!m_ErrorMessages.Contains(Resources.SimpleUpload_InvalidFileSize)))
                m_ErrorMessages.Add(Resources.SimpleUpload_InvalidFileSize);
        }

        private void ValidateUploadControls()
        {
            if (!this.EnablePopupWindow)
            {
                ArrayList ctls = new ArrayList(this.UploadControlsUniqueId);
                if (!ctls.Contains(UploadButton.UniqueID))
                    ctls.Add(UploadButton.UniqueID);
                this.UploadControlsUniqueId = (string[])ctls.ToArray(typeof(string));
            }

            if ((!this.SkipUploadControlsValidation) && (this.UploadControlsUniqueId.Length == 0))
                throw new ArgumentNullException(Resources.SimpleUpload_UploadControlsUniqueIdNotSpecified);

            foreach (string uniqueId in this.UploadControlsUniqueId)
            {
                UploadControl = this.Page.FindControl(uniqueId);
                if ((UploadControl == null) && (!this.SkipUploadControlsValidation))
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.SimpleUpload_UploadControlUniqueIdNotFound, uniqueId));
                Button btn = UploadControl as Button;
                if (btn != null) btn.UseSubmitBehavior = false;
            }
        }

        private void Validator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (!this.ErrorOccurred);
            Validator.ErrorMessage = ((this.ErrorOccurred && this.ShowErrorMessage) ? string.Join("<br />", m_ErrorMessages.ToArray()) : string.Empty);
        }

        #endregion

        #region Internal Methods

        internal static HtmlGenericControl CreateErrorDiv(string cssClass)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "ErrorDiv";
            div.EnableViewState = false;
            div.Style[HtmlTextWriterStyle.Display] = "none";
            if (!string.IsNullOrEmpty(cssClass)) div.Attributes["class"] = cssClass;
            return div;
        }

        internal static void LoadProperties(object obj, string value)
        {
            if ((obj == null) || string.IsNullOrEmpty(value)) return;

            Hashtable table = Deserialize(value) as Hashtable;
            if (table != null)
            {
                foreach (PropertyInfo p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (table.ContainsKey(p.Name) && p.CanWrite)
                    {
                        try
                        {
                            p.SetValue(obj, table[p.Name], null);
                        }
                        catch (ArgumentException) { }
                        catch (TargetException) { }
                        catch (TargetParameterCountException) { }
                        catch (MethodAccessException) { }
                        catch (TargetInvocationException) { }
                    }
                }
            }
        }

        internal static object Deserialize(string value)
        {
            object obj = null;
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    LosFormatter formatter = new LosFormatter();
                    obj = formatter.Deserialize(value);
                }
                catch (HttpException) { }
            }
            return obj;
        }

        internal static string Serialize(object value)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                LosFormatter formatter = new LosFormatter();
                formatter.Serialize(writer, value);
            }
            return sb.ToString();
        }

        internal void Reset()
        {
            if (!m_Reseted)
            {
                m_FilesMetaData = null;
                UploadedFilesField.Value = string.Empty;
                UploadedByFlashFilesField.Value = string.Empty;
                DeletedFilesField.Value = string.Empty;

                m_Reseted = true;
            }
        }

        /// <summary>
        /// Registers the specified style sheet for the specified control.
        /// </summary>
        /// <param name="ctl">The control to register style sheet for.</param>
        /// <param name="styleSheetWebResourceName">The name of the server-side resource of the style sheet to register.</param>
        internal static void RegisterControlStyleSheet(Control ctl, string styleSheetWebResourceName)
        {
            Type pageType = ctl.Page.GetType();
            string webResourceUrl = ResourceHandler.GetWebResourceUrl(styleSheetWebResourceName, true);
            if (!ctl.Page.ClientScript.IsClientScriptBlockRegistered(pageType, webResourceUrl))
            {
                string script = string.Empty;
                if (ctl.Page.Header == null)
                    script = string.Format(CultureInfo.InvariantCulture, "<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\"></link>", webResourceUrl);
                else
                {
                    HtmlLink link = new HtmlLink();
                    link.Href = webResourceUrl;
                    link.Attributes.Add("type", "text/css");
                    link.Attributes.Add("rel", "stylesheet");
                    ctl.Page.Header.Controls.Add(link);
                }
                ctl.Page.ClientScript.RegisterClientScriptBlock(pageType, webResourceUrl, script, false);
            }
        }

        #endregion

        #region Overriden Methods

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            bool result = base.LoadPostData(postDataKey, postCollection);

            if (this.EnableFileService)
            {
                this.LoadFilesMetaDataValue();
                this.LoadUploadedByFlashFilesMetaData();
                this.UploadFiles();
            }
            else
                this.LoadUploadedByFlashFilesMetaData();

            return result;
        }

        /// <summary>
        /// Restores view-state information from a previous request that was saved with the System.Web.UI.WebControls.WebControl.SaveViewState() method.
        /// </summary>
        /// <param name="savedState">An object that represents the control state to restore.</param>
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            List<string> list = this.ViewState["UploadedFileFullNames"] as List<string>;
            if (list != null) m_UploadedFileFullNames = list;

            list = this.ViewState["InvalidFileFullNames"] as List<string>;
            if (list != null) m_InvalidFileFullNames = list;
        }

        /// <summary>
        /// Saves any state that was modified after the System.Web.UI.WebControls.Style.TrackViewState() method was invoked.
        /// </summary>
        /// <returns>An object that contains the current view state of the control; otherwise, if there is no view state associated with the control, null.</returns>
        protected override object SaveViewState()
        {
            if (!m_ChangesAcceptedOrRejected)
            {
                this.ViewState["UploadedFileFullNames"] = m_UploadedFileFullNames;
                this.ViewState["InvalidFileFullNames"] = m_InvalidFileFullNames;
            }
            return base.SaveViewState();
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// <param name="e">An System.EventArgs object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            base.ControlObjectsVisibility = ControlObjectsVisibility.None;
            base.EnableEmbeddedSkins = false;
            base.EnableFileInputSkinning = false;
            base.InitialFileInputsCount = 1;
            base.ReadOnlyFileInputs = true;
            base.Skin = "SimpleUpload";

            m_ErrorMessages = new List<string>();
            m_UploadedFileFullNames = new List<string>();
            m_InvalidFileFullNames = new List<string>();
        }

        /// <summary>
        /// Creates the child controls.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (this.EnablePopupWindow)
                this.CreatePopupWindow();
            else
                this.CreateEmbeddedControl();

            DeletedFilesField = new HiddenField();
            DeletedFilesField.ID = "DeletedFilesField";
            this.Controls.Add(DeletedFilesField);

            UploadedFilesField = new HiddenField();
            UploadedFilesField.ID = "UploadedFilesField";
            this.Controls.Add(UploadedFilesField);

            UploadedByFlashFilesField = new HiddenField();
            UploadedByFlashFilesField.ID = "UploadedByFlashFilesField";
            this.Controls.Add(UploadedByFlashFilesField);

            FilesMetaDataField = new HiddenField();
            FilesMetaDataField.ID = "FilesMetaDataField";
            this.Controls.Add(FilesMetaDataField);

            CurrentModeField = new HiddenField();
            CurrentModeField.ID = "CurrentModeField";
            this.Controls.Add(CurrentModeField);
        }

        protected override void DescribeComponent(IScriptDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);

            StringBuilder sb = new StringBuilder();
            foreach (string extension in Settings.KnownFileExtensions)
            {
                sb.AppendFormat(",[\"{0}\",\"{1}\"]", extension, ResourceHandler.GetWebResourceName(string.Format(CultureInfo.InvariantCulture, "Images.Icons16x16.{0}.gif", extension)));
            }
            if (sb.Length > 0) sb.Remove(0, 1);
            sb.Append("]");
            sb.Insert(0, "[");

            ScriptComponentDescriptor scd = descriptor as ScriptComponentDescriptor;
            if (scd != null) scd.Type = "Micajah.FileService.SimpleUpload";
            descriptor.AddProperty("enableFileService", this.EnableFileService);
            descriptor.AddProperty("inputTabIndex", this.InputTabIndex);
            if (!this.InputWidth.IsEmpty) descriptor.AddProperty("inputWidth", this.InputWidth.ToString());
            descriptor.AddProperty("fileTypeIcons", sb.ToString());
            descriptor.AddProperty("fileTypeIconUrlFormat", ResourceHandler.GetWebResourceUrlFormat(true));
            descriptor.AddProperty("maxFileCount", this.MaxFilesCount);
            descriptor.AddProperty("swfUploadFileSizeLimit", Settings.WebServiceMaxRequestLength);

            descriptor.AddProperty("messages", string.Format(CultureInfo.CurrentCulture, "[\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\"]"
                , Resources.SimpleUpload_InvalidFileExtension.Replace("\"", "\\\"")
                , Resources.SimpleUpload_FileIsNotUploadedYet.Replace("\"", "\\\"")
                , string.Format(CultureInfo.InvariantCulture, Resources.SimpleUpload_ChangeModeToAdvanced, Settings.MaxRequestLengthInMB).Replace("\"", "\\\"")
                , string.Format(CultureInfo.InvariantCulture, Resources.SimpleUpload_ChangeModeToBasic, Settings.WebServiceMaxRequestLengthInMB).Replace("\"", "\\\"")
                , Resources.SimpleUpload_TooManyFilesSelected.Replace("\"", "\\\"")
                , Resources.SimpleUpload_InvalidFileSize.Replace("\"", "\\\"")
            ));
            descriptor.AddProperty("uploadControlsUniqueId", "[\"" + string.Join("\",\"", this.UploadControlsUniqueId) + "\"]");
            descriptor.AddProperty("isPostBack", this.Page.IsPostBack);
            descriptor.AddProperty("enableFileInputSkinning", false);
            descriptor.AddProperty("showUploadedFiles", this.ShowUploadedFiles);
            descriptor.AddProperty("showProgressArea", this.ShowProgressArea);
            descriptor.AddProperty("swfUploadFlashUrl", ResourceHandler.GetWebResourceUrl("Swf.SwfUpload.swf", true));
            descriptor.AddProperty("swfUploadFileUploadUrl", this.FlashFileUploadUrl);
            descriptor.AddProperty("uploadButtonAcceptChanges", this.UploadButtonAcceptChanges);
        }

        /// <summary>
        /// Returns a list of client script library dependencies for the control.
        /// </summary>
        /// <returns>The list of client script library dependencies for the control.</returns>
        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            List<ScriptReference> list = new List<ScriptReference>();
            list.AddRange(base.GetScriptReferences());
            list.Add(new ScriptReference(ResourceHandler.GetWebResourceUrl("Scripts.FlashDetection.js", true)));
            list.Add(new ScriptReference(ResourceHandler.GetWebResourceUrl("Scripts.SwfUpload.js", true)));
            if (this.ShowProgressArea && (!this.EnablePopupWindow))
                list.Add(new ScriptReference(ResourceHandler.GetWebResourceUrl("Scripts.SwfUpload.Speed.js", true)));
            list.Add(new ScriptReference(ResourceHandler.GetWebResourceUrl("Scripts.SimpleUpload.js", true)));
            return list;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.ShowProgressArea && (!this.EnablePopupWindow))
            {
                if (!RadProgressManager.IsRegisteredOnPage(this.Page))
                {
                    ProgressManager = new RadProgressManager();
                    ProgressManager.ID = "ProgressManager";
                    this.Controls.Add(ProgressManager);
                }

                if (!this.Page.ClientScript.IsStartupScriptRegistered(this.Page.GetType(), "SimpleUpload_ProgressArea"))
                {
                    this.AddPreloadDiv();

                    ProgressArea = new RadProgressArea();
                    ProgressArea.ID = "ProgressArea";
                    ProgressArea.CssClass = "suProgressArea";
                    ProgressArea.Skin = "Vista";
                    ProgressArea.OnClientProgressUpdating = "Micajah.FileService.SimpleUpload._handleProgressAreaProgressUpdating";
                    ProgressArea.HeaderText = Resources.SimpleUpload_ProgressArea_Title;
                    this.Controls.Add(ProgressArea);

                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SimpleUpload_ProgressArea"
                        , string.Format(CultureInfo.InvariantCulture, "Micajah.FileService.SimpleUpload._progressAreaId = \"{0}\";\r\n", ProgressArea.ClientID)
                        , true);
                }
            }

            this.ValidateUploadControls();

            if (this.EnableFileService)
            {
                this.LoadFilesMetaData();
                this.SaveFilesMetaDataValue();
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Registers the style sheet of the skin.
        /// </summary>
        protected override void RegisterCssReferences()
        {
            base.RegisterCssReferences();

            RegisterControlStyleSheet(this, "Styles.SimpleUpload.css");
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="writer">A System.Web.UI.HtmlTextWriter that contains the output stream to render on the client.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!this.Visible) return;
            if (writer == null) return;

            if (this.DesignMode)
                writer.Write(string.Format(CultureInfo.InvariantCulture, "[{0} \"{1}\"]", this.GetType().Name, this.ID));
            else
            {
                if (OpenLink != null)
                {
                    OpenLink.MergeStyle(base.ControlStyle);
                    OpenLink.RenderControl(writer);

                    PopupWindow.NavigateUrl = this.PopupWindowNavigateUrl;
                    PopupWindow.RenderControl(writer);
                }
                else
                {
                    if (PreloadDiv != null) PreloadDiv.RenderControl(writer);
                    if (ProgressManager != null) ProgressManager.RenderControl(writer);

                    writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                    writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table); // Table
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr); // Tr
                    if (this.ShowPaperclipImage)
                    {
                        writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
                        writer.RenderBeginTag(HtmlTextWriterTag.Td); // Td1
                        if (PaperClipImage != null) PaperClipImage.RenderControl(writer);
                        writer.RenderEndTag(); // Td1
                    }
                    writer.AddStyleAttribute(HtmlTextWriterStyle.VerticalAlign, "top");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td); // Td2

                    if (this.ShowTitle) this.RenderTitle(writer);
                    base.RenderControl(writer);

                    UploadHolder.RenderControl(writer);
                    ChangeModeMessageHolder.RenderControl(writer);
                    Validator.RenderControl(writer);
                    UploadButton.RenderControl(writer);

                    writer.RenderEndTag(); // Td2
                    writer.RenderEndTag(); // Tr
                    writer.RenderEndTag(); // Table

                    if (ProgressArea != null) ProgressArea.RenderControl(writer);
                }
                DeletedFilesField.RenderControl(writer);
                UploadedFilesField.RenderControl(writer);
                UploadedByFlashFilesField.RenderControl(writer);
                FilesMetaDataField.RenderControl(writer);
                CurrentModeField.RenderControl(writer);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Commits all the changes since the last time AcceptChanges was called.
        /// </summary>
        /// <returns>true, if the changes are commited successfully; otherwise, false.</returns>
        public bool AcceptChanges()
        {
            if ((!this.ChildControlsCreated) && (!this.EnableFileService)) return true;

            this.DeleteFiles();
            this.EnsureDeletedFileNames();
            if (!this.ErrorOccurred) DeletedFilesField.Value = string.Empty;

            string result = string.Empty;

            foreach (MetaDataSet.FileRow file in this.FilesMetaData)
            {
                if (file.Deleted) continue;

                result = string.Empty;
                if (file.IsTemporary) result = Access.SetTemporaryFile(file.FileUniqueId, string.Empty);

                if (string.IsNullOrEmpty(result))
                {
                    if (file.RowState == DataRowState.Modified)
                        file.AcceptChanges();
                    else if (file.RowState == DataRowState.Added)
                    {
                        if ((this.OrganizationId != Guid.Empty) && (file.OrganizationId != this.OrganizationId))
                            file.OrganizationId = this.OrganizationId;
                        if ((this.DepartmentId != Guid.Empty) && (file.DepartmentId != this.DepartmentId))
                            file.DepartmentId = this.DepartmentId;
                        if (string.Compare(file.LocalObjectType, this.LocalObjectType, StringComparison.Ordinal) != 0)
                            file.LocalObjectType = this.LocalObjectType;
                        if (string.Compare(file.LocalObjectId, this.LocalObjectId, StringComparison.Ordinal) != 0)
                            file.LocalObjectId = this.LocalObjectId;
                        file.IsTemporary = false;
                    }
                }
                else if (!m_ErrorMessages.Contains(result))
                    m_ErrorMessages.Add(result);
            }

            result = this.UpdateFilesMetaData();
            if (string.IsNullOrEmpty(result))
            {
                UploadedFilesField.Value = string.Empty;
                UploadedByFlashFilesField.Value = string.Empty;
            }
            else
            {
                if (!m_ErrorMessages.Contains(result))
                    m_ErrorMessages.Add(result);
            }

            m_ChangesAcceptedOrRejected = true;

            if ((!string.IsNullOrEmpty(this.OnClientAfterAcceptChanges)) && (!this.EnablePopupWindow))
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), this.ClientID + "_OnClientAfterAcceptChanges", this.OnClientAfterAcceptChanges, true);

            return (!this.ErrorOccurred);
        }

        /// <summary>
        /// Rolls back all changes that have been made to the control since it was loaded, or the last time AcceptChanges was called.
        /// </summary>
        /// <returns>true, if the changes are rolled back successfully; otherwise, false.</returns>
        public bool RejectChanges()
        {
            if (this.ChildControlsCreated && this.EnableFileService)
            {
                this.EnsureDeletedFileNames();
                this.Reset();

                m_ChangesAcceptedOrRejected = true;
            }
            return true;
        }

        public void LoadProperties(string value)
        {
            LoadProperties(this, value);
        }

        #endregion
    }
}
