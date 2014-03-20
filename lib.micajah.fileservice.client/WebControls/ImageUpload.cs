using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
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
    /// The control for uploading the single image.
    /// </summary>
    [ToolboxData("<{0}:ImageUpload runat=server></{0}:ImageUpload>")]
    [ParseChildren(true)]
    public class ImageUpload : ScriptControl, INamingContainer, IUploadControl
    {
        #region Members

        /// <summary>
        /// Defines script that creates an instance of a client class.
        /// </summary>
        private class ImageUploadScriptDescriptor : ScriptDescriptor
        {
            #region Members

            private ImageUpload m_ScriptControl;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="scriptControl">The associated control.</param>
            public ImageUploadScriptDescriptor(ImageUpload scriptControl)
                : base()
            {
                m_ScriptControl = scriptControl;
            }

            #endregion

            #region Overriden Methods

            /// <summary>
            /// Returns script to create a client class or object.
            /// </summary>
            /// <returns>The script to create a client class or object.</returns>
            protected override string GetScript()
            {
                if (m_ScriptControl.EnablePopupWindow)
                    return string.Format(CultureInfo.InvariantCulture, "$create(Micajah.FileService.ImageUpload, null, null, null, $get(\"{0}\"));", m_ScriptControl.ClientID);
                else
                {
                    return string.Format(CultureInfo.InvariantCulture
                        , "$create(Micajah.FileService.ImageUpload, {{\"allowedFileExtensions\":\"{0}\", \"isPostBack\":{1}, \"deletingConfirmationText\":\"{2}\", \"enablePopupWindow\":{3}, \"errorMessages\":\"[\\\"{4}\\\", \\\"{5}\\\", \\\"{6}\\\", \\\"{7}\\\"]\"}}, null, null, $get(\"{8}\"));"
                        , AllowedFileExtensions
                        , (m_ScriptControl.Page.IsPostBack ? "true" : "false")
                        , ((m_ScriptControl.EnableDeleting && m_ScriptControl.EnableDeletingConfirmation) ? Resources.FileList_DeletingConfirmationText.Replace("\"", "\\\"") : string.Empty)
                        , (m_ScriptControl.EnablePopupWindow ? "true" : "false")
                        , Resources.ImageUpload_FilePathIsEmpty.Replace("\"", "\\\"")
                        , Resources.ImageUpload_UrlIsEmpty.Replace("\"", "\\\"")
                        , Resources.ImageUpload_InvalidUrl.Replace("\"", "\\\"")
                        , Resources.ImageUpload_InvalidFileExtension.Replace("\"", "\\\"")
                        , m_ScriptControl.ClientID);
                }
            }

            #endregion
        }

        private const string UrlRegularExpression = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        private const string AllowedFileExtensions = "jpg,jpe,jpeg,gif,png,tif,tiff,bmp";
        private const int PopupWindowWidth = 430;
        private const int PopupWindowHeight = 235;

        private HyperLink OpenLink;
        private RadWindow PopupWindow;
        private HiddenField OriginalFileField;
        private HiddenField CurrentFileField;
        private HiddenField PreviousFilesField;
        private RadioButtonList UploadTypeList;
        private Panel FileFromMyComputerPanel;
        private FileUpload FileFromMyComputer;
        private Panel FileFromWebPanel;
        private TextBox FileFromWeb;
        private HtmlGenericControl ErrorDiv;
        private Button UploadButton;
        private Panel UploadedImageViewPanel;
        private Image UploadedImage;
        private HyperLink DeleteButton;
        private Label NoImageLabel;
        private HyperLink ClosePopupWindowButton;
        private Label ButtonSeparator;

        private string m_ErrorMessage;
        private bool m_ObjectChanged;

        #endregion

        #region Private Properties

        private string FileServiceSessionGuid
        {
            get
            {
                object obj = ViewState["FileServiceSessionGuid"];
                if (obj == null)
                {
                    String guid = Guid.NewGuid().ToString();
                    ViewState["FileServiceSessionGuid"] = guid;
                    return guid;
                }
                else
                    return (string)obj;
            }
        }

        private bool ObjectChanged
        {
            get { return (((this.Page != null) && (!this.Page.IsPostBack)) || m_ObjectChanged); }
            set { m_ObjectChanged = value; }
        }

        private string PopupWindowNavigateUrl
        {
            get
            {
                Hashtable table = new Hashtable();
                table["EnableDeleting"] = this.EnableDeleting;
                table["EnableDeletingConfirmation"] = this.EnableDeletingConfirmation;
                table["EnablePopupWindow"] = false;
                table["OrganizationId"] = this.OrganizationId;
                table["OrganizationName"] = this.OrganizationName;
                table["DepartmentId"] = this.DepartmentId;
                table["DepartmentName"] = this.DepartmentName;

                return ResourceVirtualPathProvider.VirtualPathToAbsolute(ResourceVirtualPathProvider.VirtualRootShortPath + "ImageUpload.aspx")
                    + "?p=" + HttpUtility.UrlEncode(SimpleUpload.Serialize(table));
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique identifier of the current image.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string CurrentFileUniqueId
        {
            get
            {
                this.EnsureChildControls();
                return CurrentFileField.Value;
            }
        }

        /// <summary>
        /// Gets or set value indicating that the deleting is enabled or disabled.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the deleting is enabled.")]
        [DefaultValue(true)]
        public bool EnableDeleting
        {
            get
            {
                object obj = ViewState["EnableDeleting"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["EnableDeleting"] = value; }
        }

        /// <summary>
        /// Gets or set value indicating that the deleting requires confirmation.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the deleting requires confirmation.")]
        [DefaultValue(true)]
        public bool EnableDeletingConfirmation
        {
            get
            {
                object obj = ViewState["EnableDeletingConfirmation"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["EnableDeletingConfirmation"] = value; }
        }

        /// <summary>
        /// Gets or set value indicating whether the control is displayed in the pop-up window.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the control is displayed in the pop-up window.")]
        [DefaultValue(true)]
        public bool EnablePopupWindow
        {
            get
            {
                object obj = ViewState["EnablePopupWindow"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { ViewState["EnablePopupWindow"] = value; }
        }

        /// <summary>
        /// Gets a message that describes the current error, if it occured.
        /// </summary>
        [Browsable(false)]
        public string ErrorMessage
        {
            get { return m_ErrorMessage; }
        }

        /// <summary>
        /// Gets a value indicating that an error occurred.
        /// </summary>
        [Browsable(false)]
        public bool ErrorOccurred
        {
            get { return (!string.IsNullOrEmpty(m_ErrorMessage)); }
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
                this.ChangeObject(this.LocalObjectType, value);
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
                this.ChangeObject(this.LocalObjectId, value);
                this.ViewState["LocalObjectId"] = value;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the original image.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string OriginalFileUniqueId
        {
            get
            {
                this.EnsureChildControls();
                return OriginalFileField.Value;
            }
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
        /// Gets or set a value indicating whether the expiration required for the link to the uploaded file.
        /// </summary>
        [Category("Data")]
        [Description("Whether the expiration required for the link to the uploaded file.")]
        [DefaultValue(true)]
        public bool ExpirationRequired
        {
            get
            {
                object obj = ViewState["ExpirationRequired"];
                return ((obj == null) ? Settings.Default.LinksExpiration : (bool)obj);
            }
            set { ViewState["ExpirationRequired"] = value; }
        }

        #endregion

        #region Private Methods

        private void ChangeObject(string currentValue, string newValue)
        {
            if ((this.Page != null) && this.Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(currentValue))
                {
                    if (!string.IsNullOrEmpty(newValue))
                        this.ObjectChanged = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(newValue))
                        this.ObjectChanged = true;
                    else
                        this.ObjectChanged = (string.Compare(newValue, currentValue, StringComparison.Ordinal) != 0);
                }
            }
        }

        private void CreateEmbeddedControl()
        {
            HtmlTable table = new HtmlTable();
            table.Style[HtmlTextWriterStyle.Width] = string.Concat((PopupWindowWidth - 20), "px");
            table.Style[HtmlTextWriterStyle.Height] = string.Concat((PopupWindowHeight - 110), "px");
            HtmlTableRow tr = new HtmlTableRow();
            tr.Style[HtmlTextWriterStyle.VerticalAlign] = "top";
            HtmlTableCell td = new HtmlTableCell();

            UploadTypeList = new RadioButtonList();
            UploadTypeList.ID = "UploadTypeList";
            UploadTypeList.CellPadding = 5;
            UploadTypeList.CellSpacing = 0;
            UploadTypeList.Style[HtmlTextWriterStyle.Position] = "relative";
            UploadTypeList.Style[HtmlTextWriterStyle.Left] = "-8px";
            ListItem listItem = new ListItem(Resources.ImageUpload_UploadTypeList_FromMyComputerItem_Text, "0");
            listItem.Selected = true;
            UploadTypeList.Items.Add(listItem);
            UploadTypeList.Items.Add(new ListItem(Resources.ImageUpload_UploadTypeList_FromWebItem_Text, "1"));
            td.Controls.Add(UploadTypeList);

            FileFromMyComputerPanel = new Panel();
            FileFromMyComputerPanel.ID = "Panel0";
            FileFromMyComputerPanel.CssClass = "iuPanel";
            td.Controls.Add(FileFromMyComputerPanel);

            FileFromMyComputer = new FileUpload();
            FileFromMyComputer.ID = "FileFromMyComputer";
            FileFromMyComputer.Attributes["size"] = "29";
            FileFromMyComputer.Height = Unit.Pixel(21);
            FileFromMyComputerPanel.Controls.Add(FileFromMyComputer);

            FileFromWebPanel = new Panel();
            FileFromWebPanel.ID = "Panel1";
            FileFromWebPanel.CssClass = "iuPanel";
            td.Controls.Add(FileFromWebPanel);

            FileFromWeb = new TextBox();
            FileFromWeb.ID = "FileFromWeb";
            FileFromWeb.Columns = 45;
            FileFromWeb.Width = Unit.Pixel(250);
            FileFromWebPanel.Controls.Add(FileFromWeb);

            ErrorDiv = SimpleUpload.CreateErrorDiv("iuError");
            td.Controls.Add(ErrorDiv);

            UploadButton = new Button();
            UploadButton.ID = "UploadButton";
            UploadButton.CssClass = "iuUploadButton";
            UploadButton.CausesValidation = false;
            UploadButton.Text = Resources.ImageUpload_UploadButton_Text;
            UploadButton.Click += new EventHandler(UploadButton_Click);
            td.Controls.Add(UploadButton);

            tr.Cells.Add(td);

            td = new HtmlTableCell();
            td.Style[HtmlTextWriterStyle.Width] = "135px";

            this.EnsureUploadedImageViewPanel();
            td.Controls.Add(UploadedImageViewPanel);

            if (this.EnableDeleting)
            {
                UploadedImageViewPanel.Controls.Add(new LiteralControl("<br />"));
                DeleteButton = new HyperLink();
                DeleteButton.ID = "DeleteButton";
                DeleteButton.CssClass = "iuRemove";
                DeleteButton.Style[HtmlTextWriterStyle.Display] = "none";
                DeleteButton.Text = Resources.FileList_DeleteText;
                DeleteButton.NavigateUrl = "javascript:void(0);";
                UploadedImageViewPanel.Controls.Add(DeleteButton);
            }

            tr.Cells.Add(td);
            table.Rows.Add(tr);

            tr = new HtmlTableRow();
            tr.Style[HtmlTextWriterStyle.VerticalAlign] = "top";

            td = new HtmlTableCell();
            td.ColSpan = 2;

            ButtonSeparator = new Label();
            ButtonSeparator.ID = "ButtonSeparator";
            ButtonSeparator.CssClass = "iuButtonSeparator";
            ButtonSeparator.Text = "&nbsp;&nbsp;" + Resources.ImageUpload_ButtonSeparator_Text + "&nbsp;&nbsp;";
            ButtonSeparator.Style[HtmlTextWriterStyle.Display] = "none";
            td.Controls.Add(ButtonSeparator);

            ClosePopupWindowButton = new HyperLink();
            ClosePopupWindowButton.ID = "ClosePopupWindowButton";
            ClosePopupWindowButton.NavigateUrl = "javascript:void(0);";
            ClosePopupWindowButton.CssClass = "iuCloseButton";
            ClosePopupWindowButton.Style[HtmlTextWriterStyle.Display] = "none";
            ClosePopupWindowButton.Text = Resources.ImageUpload_ClosePopupWindowButton_Text;
            td.Controls.Add(ClosePopupWindowButton);

            tr.Cells.Add(td);
            table.Rows.Add(tr);

            this.Controls.Add(table);
        }

        private void CreatePopupWindow()
        {
            OpenLink = new HyperLink();
            OpenLink.ID = "OpenLink";
            OpenLink.NavigateUrl = "javascript:void(0);";
            OpenLink.ToolTip = Resources.ImageUpload_OpenLink_ToolTip;
            OpenLink.Style[HtmlTextWriterStyle.Cursor] = "pointer";
            this.Controls.Add(OpenLink);

            this.EnsureUploadedImageViewPanel();
            OpenLink.Controls.Add(UploadedImageViewPanel);

            ErrorDiv = SimpleUpload.CreateErrorDiv("iuError");
            this.Controls.Add(ErrorDiv);

            PopupWindow = new RadWindow();
            PopupWindow.ID = "PopupWindow";
            PopupWindow.Behaviors = (WindowBehaviors.Close | WindowBehaviors.Move);
            PopupWindow.Width = Unit.Pixel(PopupWindowWidth);
            PopupWindow.Height = Unit.Pixel(PopupWindowHeight);
            PopupWindow.OpenerElementID = OpenLink.ClientID;
            PopupWindow.Title = Resources.ImageUpload_PopupWindow_Title;
            PopupWindow.VisibleStatusbar = false;
            this.Controls.Add(PopupWindow);
        }

        private void DeleteFiles(FileTableAdapter adapter, string fileUniqueIdList)
        {
            if (!string.IsNullOrEmpty(fileUniqueIdList))
            {
                foreach (string fileUniqueId in fileUniqueIdList.Split('|'))
                {
                    m_ErrorMessage = Access.DeleteFile(fileUniqueId);
                    if (!this.ErrorOccurred)
                        adapter.MarkFileAsDeleted(fileUniqueId, this.UpdatedBy);
                }
            }
        }

        private void EnsureUploadedImageViewPanel()
        {
            if (UploadedImageViewPanel != null) return;

            UploadedImageViewPanel = new Panel();
            UploadedImageViewPanel.ID = "UploadedImageViewPanel";
            UploadedImageViewPanel.CssClass = "iuImageView";
            UploadedImageViewPanel.Width = Unit.Pixel(131);
            UploadedImageViewPanel.Height = Unit.Pixel(150);

            NoImageLabel = new Label();
            NoImageLabel.ID = "NoImageLabel";
            NoImageLabel.Text = Resources.ImageUpload_NoImageLabel_Text;
            NoImageLabel.Style[HtmlTextWriterStyle.Position] = "relative";
            NoImageLabel.Style[HtmlTextWriterStyle.Top] = "65px";
            UploadedImageViewPanel.Controls.Add(NoImageLabel);

            UploadedImage = new Image();
            UploadedImage.ID = "UploadedImage";
            UploadedImage.Width = Unit.Pixel(130);
            UploadedImage.Height = Unit.Pixel(130);
            UploadedImage.Style[HtmlTextWriterStyle.Cursor] = "pointer";
            UploadedImage.Style[HtmlTextWriterStyle.Display] = "none";
            UploadedImageViewPanel.Controls.Add(UploadedImage);
        }

        private static string GetLastPartOfString(string str, string delimiter)
        {
            if (str.IndexOf(delimiter, StringComparison.OrdinalIgnoreCase) > -1)
            {
                string[] array1 = str.Split(new string[] { delimiter }, StringSplitOptions.None);
                return array1[array1.Length - 1];
            }
            return str;
        }

        private void UploadFile()
        {
            if (!this.ChildControlsCreated) return;

            string uniqueId = null;
            long fileSize = 0;

            switch (UploadTypeList.SelectedValue)
            {
                case "0":
                    if (!string.IsNullOrEmpty(FileFromMyComputer.FileName))
                    {
                        if (FileFromMyComputer.HasFile)
                        {
                            HttpPostedFile file = FileFromMyComputer.PostedFile;
                            if (ValidateExtension(Path.GetExtension(FileFromMyComputer.FileName).Replace(".", string.Empty)) && MimeType.IsImageType(file.ContentType))
                            {
                                fileSize = file.InputStream.Length;
                                if (fileSize <= Settings.MaxRequestLength)
                                {
                                    byte[] bytes = FileFromMyComputer.FileBytes;
                                    string organizationGuidString = this.OrganizationId.ToString();
                                    string departmentGuidString = this.DepartmentId.ToString();
                                    string checksum = null;
                                    uniqueId = Access.PutFileAsByteArray(Settings.Default.ApplicationId.ToString()
                                        , this.OrganizationName, ref organizationGuidString
                                        , this.DepartmentName, ref departmentGuidString
                                        , file.FileName, ref bytes, this.ExpirationRequired, ref checksum);
                                    if (!Access.StringIsFileUniqueId(uniqueId)) m_ErrorMessage = uniqueId;
                                }
                                else
                                    m_ErrorMessage = Resources.ImageUpload_InvalidFileSize;
                            }
                            else
                                m_ErrorMessage = Resources.ImageUpload_InvalidFileExtension;
                        }
                        else
                            m_ErrorMessage = Resources.ImageUpload_FileNotExists;
                    }
                    else
                        m_ErrorMessage = Resources.ImageUpload_FilePathIsEmpty;
                    break;
                case "1":
                    string url = FileFromWeb.Text;
                    if (!string.IsNullOrEmpty(url))
                    {
                        if (Regex.IsMatch(url, UrlRegularExpression, (RegexOptions.IgnoreCase | RegexOptions.Singleline)))
                        {
                            if (ValidateExtension(GetLastPartOfString(url, ".")))
                            {
                                string organizationGuidString = this.OrganizationId.ToString();
                                string departmentGuidString = this.DepartmentId.ToString();
                                string checksum = null;
                                uniqueId = Access.PutFileFromUrl(Settings.Default.ApplicationId.ToString()
                                    , this.OrganizationName, ref organizationGuidString
                                    , this.DepartmentName, ref departmentGuidString
                                    , url, this.ExpirationRequired, ref checksum);
                                if (!Access.StringIsFileUniqueId(uniqueId)) m_ErrorMessage = uniqueId;
                            }
                            else
                                m_ErrorMessage = Resources.ImageUpload_InvalidFileExtension;
                        }
                        else
                            m_ErrorMessage = Resources.ImageUpload_InvalidUrl;
                    }
                    else
                        m_ErrorMessage = Resources.ImageUpload_UrlIsEmpty;
                    break;
            }

            if (!this.ErrorOccurred)
            {
                FileFromWeb.Text = string.Empty;

                if (!string.IsNullOrEmpty(uniqueId)) Access.SetTemporaryFile(uniqueId, this.FileServiceSessionGuid);

                if (!CurrentFileField.Value.Equals(OriginalFileField.Value, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(PreviousFilesField.Value))
                        PreviousFilesField.Value = CurrentFileField.Value;
                    else
                        PreviousFilesField.Value += "|" + CurrentFileField.Value;
                }
                CurrentFileField.Value = uniqueId;
            }
        }

        private static bool ValidateExtension(string extension)
        {
            return (string.Concat(",", AllowedFileExtensions.ToUpperInvariant(), ",").Contains("," + extension.ToUpperInvariant() + ","));
        }

        private void OriginalFileField_PreRender(object sender, EventArgs e)
        {
            if (this.ObjectChanged && (!string.IsNullOrEmpty(this.LocalObjectType)) && (!string.IsNullOrEmpty(this.LocalObjectId)))
            {
                using (FileTableAdapter adapter = new FileTableAdapter(this.ConnectionString))
                {
                    MetaDataSet.FileRow originalFile = adapter.GetFile(this.OrganizationId, this.DepartmentId, this.LocalObjectType, this.LocalObjectId);
                    OriginalFileField.Value = CurrentFileField.Value = ((originalFile == null) ? string.Empty : originalFile.FileUniqueId);
                }
            }
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            this.UploadFile();
        }

        #endregion

        #region Overriden Methods

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

            OriginalFileField = new HiddenField();
            OriginalFileField.ID = "OriginalFileField";
            OriginalFileField.PreRender += new EventHandler(OriginalFileField_PreRender);
            this.Controls.Add(OriginalFileField);

            CurrentFileField = new HiddenField();
            CurrentFileField.ID = "CurrentFileField";
            this.Controls.Add(CurrentFileField);

            PreviousFilesField = new HiddenField();
            PreviousFilesField.ID = "PreviousFilesField";
            this.Controls.Add(PreviousFilesField);
        }

        /// <summary>
        /// Returns a list of components, behaviors, and client controls that are required for the control's client functionality.
        /// </summary>
        /// <returns>The list of components, behaviors, and client controls that are required for the control's client functionality.</returns>
        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            List<ScriptDescriptor> list = new List<ScriptDescriptor>();
            list.Add(new ImageUploadScriptDescriptor(this));
            return list;
        }

        /// <summary>
        /// Returns a list of client script library dependencies for the control.
        /// </summary>
        /// <returns>The list of client script library dependencies for the control.</returns>
        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            List<ScriptReference> list = new List<ScriptReference>();
            list.Add(new ScriptReference(ResourceHandler.GetWebResourceUrl("Scripts.ImageUpload.js", true)));
            return list;
        }

        /// <summary>
        /// Raises the System.Web.UI.Control.PreRender event and registers the style sheet of the control.
        /// </summary>
        /// <param name="e">An System.EventArgs object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.ShowErrorMessage && this.ErrorOccurred)
            {
                ErrorDiv.InnerHtml = m_ErrorMessage;
                ErrorDiv.Style.Remove(HtmlTextWriterStyle.Display);
            }

            SimpleUpload.RegisterControlStyleSheet(this, "Styles.ImageUpload.css");
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <param name="writer">A System.Web.UI.HtmlTextWriter that contains the output stream to render on the client.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (writer == null) return;

            if (this.DesignMode)
                writer.Write(string.Format(CultureInfo.InvariantCulture, "[{0} \"{1}\"]", this.GetType().Name, this.ID));
            else
            {
                if (UploadedImage == null) this.EnsureChildControls();

                if (OpenLink != null)
                {
                    OpenLink.MergeStyle(base.ControlStyle);
                    base.ControlStyle.Reset();
                }
                if (PopupWindow != null) PopupWindow.NavigateUrl = this.PopupWindowNavigateUrl;
                UploadedImage.ImageUrl = (string.IsNullOrEmpty(CurrentFileField.Value) ? string.Empty : Access.GetThumbnailUrl(CurrentFileField.Value, this.OrganizationId, this.DepartmentId, 130, 130, 0, false, this.ExpirationRequired));
                base.CssClass = "iuContainer";
                base.RenderControl(writer);
            }
        }

        /// <summary>
        /// Gets the System.Web.UI.HtmlTextWriterTag value that corresponds to this Web server control.
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Commits all the changes since the last time AcceptChanges was called.
        /// </summary>
        /// <returns>true, if the changes are commited successfully; otherwise, false.</returns>
        public bool AcceptChanges()
        {
            if ((!this.ChildControlsCreated) || CurrentFileField.Value.Equals(OriginalFileField.Value, StringComparison.OrdinalIgnoreCase))
                return true;

            m_ErrorMessage = string.Empty;
            FileTableAdapter adapter = null;

            try
            {
                adapter = new FileTableAdapter(this.ConnectionString);

                this.DeleteFiles(adapter, PreviousFilesField.Value);
                if (!this.ErrorOccurred) this.DeleteFiles(adapter, OriginalFileField.Value);
                if (!this.ErrorOccurred)
                {
                    string currentFileUniqueId = CurrentFileField.Value;
                    if (!string.IsNullOrEmpty(currentFileUniqueId))
                    {
                        m_ErrorMessage = Access.SetTemporaryFile(currentFileUniqueId, string.Empty);
                        if (!this.ErrorOccurred)
                        {
                            string fileNameWithExtension = string.Empty;
                            string mimeType = string.Empty;
                            long sizeInBytes = 0;
                            int width = 0;
                            int height = 0;
                            int align = 0;
                            string checksum = null;

                            if (Access.GetFileInfo(currentFileUniqueId, ref fileNameWithExtension, ref sizeInBytes, ref width, ref height, ref align, ref mimeType, ref checksum))
                                adapter.Insert(currentFileUniqueId, this.OrganizationId, this.DepartmentId, this.LocalObjectType, this.LocalObjectId, fileNameWithExtension, (int)sizeInBytes, DateTime.UtcNow, this.UpdatedBy, false, checksum);
                        }
                    }

                    if (!this.ErrorOccurred)
                    {
                        OriginalFileField.Value = currentFileUniqueId;
                        PreviousFilesField.Value = string.Empty;
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                m_ErrorMessage = ex.Message;
            }
            catch (SqlException ex)
            {
                m_ErrorMessage = ex.Message;
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
            }

            return (!this.ErrorOccurred);
        }

        /// <summary>
        /// Rolls back all changes that have been made to the control since it was loaded, or the last time AcceptChanges was called.
        /// </summary>
        /// <returns>true, if the changes are rolled back successfully; otherwise, false.</returns>
        public bool RejectChanges()
        {
            if ((!this.ChildControlsCreated) || CurrentFileField.Value.Equals(OriginalFileField.Value, StringComparison.OrdinalIgnoreCase))
                return true;

            m_ErrorMessage = string.Empty;
            FileTableAdapter adapter = null;

            try
            {
                adapter = new FileTableAdapter(this.ConnectionString);

                this.DeleteFiles(adapter, PreviousFilesField.Value);
                if (!this.ErrorOccurred) this.DeleteFiles(adapter, CurrentFileField.Value);
            }
            catch (DBConcurrencyException ex)
            {
                m_ErrorMessage = ex.Message;
            }
            finally
            {
                if (adapter != null) adapter.Dispose();
            }

            if (!this.ErrorOccurred)
            {
                CurrentFileField.Value = OriginalFileField.Value;
                PreviousFilesField.Value = string.Empty;
            }

            return (!this.ErrorOccurred);
        }

        public void LoadProperties(string value)
        {
            SimpleUpload.LoadProperties(this, value);
        }

        #endregion
    }
}