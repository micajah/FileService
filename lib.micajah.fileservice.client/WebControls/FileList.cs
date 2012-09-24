using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.FileService.Client;
using Micajah.FileService.Client.Dal;
using Micajah.FileService.Client.Dal.MetaDataSetTableAdapters;
using Micajah.FileService.Client.Properties;
using Telerik.Web.UI;

namespace Micajah.FileService.WebControls
{
    /// <summary>
    /// Displays the files list.
    /// </summary>
    [ToolboxData("<{0}:FileList runat=server></{0}:FileList>")]
    [ParseChildren(true)]
    [PersistChildren(false)]
    public class FileList : Control, INamingContainer, IPostBackEventHandler, IUploadControl
    {
        #region Members

        private class FilesListItemTemplate : ITemplate, IDisposable
        {
            #region Members

            private ListItemType m_ItemType;
            private FileList m_FileList;
            private LinkButton DeleteLink;

            #endregion

            #region Constructors

            public FilesListItemTemplate(ListItemType itemType, FileList fileList)
            {
                m_ItemType = itemType;
                m_FileList = fileList;
            }

            #endregion

            #region Private Methods

            private void Image_DataBinding(object sender, EventArgs e)
            {
                Image img = (sender as Image);
                object dataItem = DataBinder.GetDataItem(img.NamingContainer);
                string url = null;
                url = GetNonImageFileTypeIconUrl(DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.NameColumnName).ToString(), IconSize.Bigger);
                img.ImageUrl = ((url == null) ? Access.GetFileUrl(DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.FileUniqueIdColumnName).ToString(), m_FileList.OrganizationId, m_FileList.DepartmentId) : url);
            }

            private void HyperLink_DataBinding(object sender, EventArgs e)
            {
                HyperLink link = (sender as HyperLink);
                object dataItem = DataBinder.GetDataItem(link.NamingContainer);
                string fileName = (string)DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.NameColumnName);
                string extension = Path.GetExtension(fileName);
                link.Text = fileName;
                link.NavigateUrl = Access.GetFileUrl((string)DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.FileUniqueIdColumnName), m_FileList.OrganizationId, m_FileList.DepartmentId);
                if ((string.Compare(extension, ".swf", StringComparison.OrdinalIgnoreCase) == 0) || MimeType.IsImageType(MimeType.GetMimeType(extension)))
                    link.Target = "_blank";
            }

            #endregion

            #region Public Methods

            public void InstantiateIn(Control container)
            {
                if (container == null) return;
                if ((m_ItemType != ListItemType.Item) && (m_ItemType != ListItemType.AlternatingItem)) return;

                Image img = new Image();
                img.DataBinding += new EventHandler(Image_DataBinding);

                Panel panel = new Panel();
                HyperLink link = new HyperLink();
                link.DataBinding += new EventHandler(HyperLink_DataBinding);
                link.CssClass = "flFileName";
                panel.Controls.Add(link);

                if (m_FileList.EnableDeleting)
                {
                    panel.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;"));
                    DeleteLink = new LinkButton();
                    DeleteLink.ID = "DeleteLink";
                    DeleteLink.CommandName = DataList.DeleteCommandName;
                    DeleteLink.CausesValidation = false;
                    DeleteLink.CssClass = "flRemove";
                    DeleteLink.Text = Resources.FileList_DeleteText;
                    if (m_FileList.EnableDeletingConfirmation) DeleteLink.OnClientClick = FileList.OnClientDeleting;
                    panel.Controls.Add(DeleteLink);
                }

                container.Controls.Add(img);
                container.Controls.Add(panel);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                if (DeleteLink != null) DeleteLink.Dispose();
            }

            #endregion
        }

        private class ThumbnailsListItemTemplate : ITemplate, IDisposable
        {
            #region Members

            private ListItemType m_ItemType;
            private FileList m_FileList;
            private LinkButton DeleteLink;

            #endregion

            #region Constructors

            public ThumbnailsListItemTemplate(ListItemType itemType, FileList fileList)
            {
                m_ItemType = itemType;
                m_FileList = fileList;
            }

            #endregion

            #region Private Methods

            private void Image_DataBinding(object sender, EventArgs e)
            {
                Image img = (sender as Image);
                object dataItem = DataBinder.GetDataItem(img.NamingContainer);
                string url = null;
                IconSize iconSize = IconSize.Bigger;
                url = (m_FileList.ShowVideoOnly
                    ? ResourceHandler.GetWebResourceUrl("Images.Video.gif", true)
                    : GetNonImageFileTypeIconUrl(DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.NameColumnName).ToString(), iconSize));
                img.ImageUrl = ((url == null) ? Access.GetThumbnailUrl(DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.FileUniqueIdColumnName).ToString(), m_FileList.OrganizationId, m_FileList.DepartmentId, (int)iconSize, (int)iconSize) : url);
            }

            private void HyperLink_DataBinding(object sender, EventArgs e)
            {
                HyperLink link = (sender as HyperLink);
                object dataItem = DataBinder.GetDataItem(link.NamingContainer);
                string extension = Path.GetExtension((string)DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.NameColumnName));
                link.NavigateUrl = Access.GetFileUrl((string)DataBinder.Eval(dataItem, MetaDataSet.FileDataTable.FileUniqueIdColumnName), m_FileList.OrganizationId, m_FileList.DepartmentId);
                if ((string.Compare(extension, ".swf", StringComparison.OrdinalIgnoreCase) == 0) || MimeType.IsImageType(MimeType.GetMimeType(extension)))
                    link.Target = "_blank";
            }

            private void Panel_DataBinding(object sender, EventArgs e)
            {
                Panel panel = (sender as Panel);
                DataRowView drv = DataBinder.GetDataItem(panel.NamingContainer) as DataRowView;
                if (drv != null)
                {
                    MetaDataSet.FileRow file = drv.Row as MetaDataSet.FileRow;
                    if (file != null)
                    {
                        string value = string.Format(m_FileList.Culture, m_FileList.DateTimeToolTipFormatString + ", {1:N0} KB|{2}", TimeZoneInfo.ConvertTimeFromUtc(file.UpdatedTime, m_FileList.TimeZone), file.SizeInKB, file.Name);
                        m_FileList.ToolTipManager.TargetControls.Add(panel.ClientID, value, true);
                    }
                }
            }

            #endregion

            #region Public Methods

            public void InstantiateIn(Control container)
            {
                if (container == null) return;
                if ((m_ItemType != ListItemType.Item) && (m_ItemType != ListItemType.AlternatingItem)) return;

                Image img = new Image();
                img.DataBinding += new EventHandler(Image_DataBinding);
                img.Width = img.Height = Unit.Pixel(m_FileList.ShowVideoOnly ? 148 : 128);

                HyperLink link = new HyperLink();
                link.DataBinding += new EventHandler(HyperLink_DataBinding);
                link.Controls.Add(img);

                Panel panel = new Panel();
                panel.ID = "ThumbPanel";
                panel.Width = panel.Height = Unit.Pixel(m_FileList.ShowVideoOnly ? 148 : 128);
                panel.Style[HtmlTextWriterStyle.BackgroundColor] = "White";
                panel.DataBinding += new EventHandler(Panel_DataBinding);
                panel.Controls.Add(link);

                if (m_FileList.EnableDeleting)
                {
                    DeleteLink = new LinkButton();
                    DeleteLink.ID = "DeleteLink";
                    DeleteLink.CommandName = DataList.DeleteCommandName;
                    DeleteLink.CausesValidation = false;
                    DeleteLink.CssClass = "flRemove";
                    DeleteLink.Text = Resources.FileList_DeleteText;
                    DeleteLink.Style[HtmlTextWriterStyle.Display] = "none";
                    if (m_FileList.EnableDeletingConfirmation) DeleteLink.OnClientClick = FileList.OnClientDeleting;
                    panel.Controls.Add(DeleteLink);
                }

                container.Controls.Add(panel);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                if (DeleteLink != null) DeleteLink.Dispose();
            }

            #endregion
        }

        private WebControl Grid;
        private ObjectDataSource GridDataSource;
        private RadToolTipManager ToolTipManager;
        private HyperLink ViewAllAtOnceLink;
        private Panel CaptionPanel;
        private LiteralControl Separator;
        private SimpleUpload m_FilesUpload;
        private DateTime m_UpdatedDate = DateTime.MinValue;
        private TimeZoneInfo m_TimeZone;

        #endregion

        #region Events

        /// <summary>
        /// Occurs after a file is deleted.
        /// </summary>
        public event CommandEventHandler FileDeleted;

        #endregion

        #region Private Properties

        private List<string> FileExtensionsFilterInternal
        {
            get
            {
                List<string> extensions = new List<string>(this.FileExtensionsFilter);
                if (extensions.Count > 0)
                {
                    switch (extensions[0].ToUpperInvariant())
                    {
                        case "VIDEO":
                            extensions = new List<string>(MimeType.VideoExtensions);
                            extensions.Add(".swf");
                            break;
                        case "IMAGE":
                            extensions = new List<string>(MimeType.ImageExtensions);
                            break;
                    }
                }
                return extensions;
            }
        }

        private string ViewAllAtOnceLinkNavigateUrl
        {
            get
            {
                Hashtable table = new Hashtable();
                table["RenderingMode"] = FileListRenderingMode.FilesList;
                table["LocalObjectId"] = this.LocalObjectId;
                table["LocalObjectType"] = this.LocalObjectType;
                table["FileExtensionsFilter"] = new string[] { "image" };
                table["NegateFileExtensionsFilter"] = this.NegateFileExtensionsFilter;
                table["ShowViewAllAtOnceLink"] = false;
                table["EnableDeleting"] = false;
                table["OrganizationId"] = this.OrganizationId;
                table["OrganizationName"] = this.OrganizationName;
                table["DepartmentId"] = this.DepartmentId;
                table["DepartmentName"] = this.DepartmentName;
                table["ConnectionString"] = this.ConnectionString;

                return ResourceVirtualPathProvider.VirtualPathToAbsolute(ResourceVirtualPathProvider.VirtualRootShortPath + "FileList.aspx")
                    + "?p=" + HttpUtility.UrlEncodeUnicode(SimpleUpload.Serialize(table));
            }
        }

        #endregion

        #region Internal Properties

        internal static string OnClientDeleting
        {
            get { return string.Format(CultureInfo.CurrentCulture, "return window.confirm(\"{0}\");", Resources.FileList_DeletingConfirmationText); }
        }

        internal bool ShowVideoOnly
        {
            get
            {
                string[] extensions = this.FileExtensionsFilter;
                if (extensions.Length > 0)
                {
                    if (string.Compare(extensions[0], "video", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        foreach (string ext in extensions)
                        {
                            string mimeType = MimeType.GetMimeType(ext);
                            if (!(MimeType.IsVideoType(mimeType) || MimeType.IsFlash(mimeType)))
                                return false;
                        }
                    }
                    return (!this.NegateFileExtensionsFilter);
                }
                return false;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the file extensions for displaying or not, if the NegateFileExtensionsFilter is true.
        /// </summary>
        [Category("Appearance")]
        [Description("The the file extensions for displaying.")]
        [DefaultValue(typeof(string[]), "")]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] FileExtensionsFilter
        {
            get
            {
                object obj = this.ViewState["FileExtensionsFilter"];
                return ((obj == null) ? new string[0] : (string[])obj);
            }
            set { this.ViewState["FileExtensionsFilter"] = value; }
        }

        /// <summary>
        /// Gets or sets the culture in which the date and time will be formatted.
        /// </summary>
        [Category("Appearance")]
        [Description("The culture in which the date and time will be formatted.")]
        [DefaultValue(typeof(CultureInfo), "en-US")]
        public CultureInfo Culture
        {
            get
            {
                object obj = this.ViewState["Culture"];
                return ((obj == null) ? CultureInfo.CurrentCulture : (CultureInfo)obj);
            }
            set { this.ViewState["Culture"] = value; }
        }

        /// <summary>
        /// Gets or sets the string that specifies the display format for the tool tip of the date column.
        /// </summary>
        [Category("Appearance")]
        [Description("The string that specifies the display format for the tool tip of the date column.")]
        [DefaultValue("{0:MMM d, yyyy H:mm}")]
        public string DateTimeToolTipFormatString
        {
            get
            {
                object obj = this.ViewState["DateTimeToolTipFormatString"];
                return ((obj == null) ? "{0:MMM d, yyyy H:mm}" : (string)obj);
            }
            set { this.ViewState["DateTimeToolTipFormatString"] = value; }
        }

        /// <summary>
        /// Gets or sets the string that specifies the display format for the date column.
        /// </summary>
        [Category("Appearance")]
        [Description("The string that specifies the display format for the date column.")]
        [DefaultValue("{0:d-MMM-yyyy}")]
        public string DateTimeFormatString
        {
            get
            {
                object obj = this.ViewState["DateTimeFormatString"];
                return ((obj == null) ? "{0:d-MMM-yyyy}" : (string)obj);
            }
            set { this.ViewState["DateTimeFormatString"] = value; }
        }

        /// <summary>
        /// Gets or set time zone identifier.
        /// </summary>
        [Category("Appearance")]
        [Description("The time zone identifier.")]
        [DefaultValue("Eastern Standard Time")]
        public string TimeZoneId
        {
            get
            {
                string str = (string)ViewState["TimeZoneId"];
                return (string.IsNullOrEmpty(str) ? "Eastern Standard Time" : str);
            }
            set { ViewState["TimeZoneId"] = value; }
        }

        /// <summary>
        /// Gets or set the time zone.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(typeof(TimeZoneInfo), "Eastern Standard Time")]
        public TimeZoneInfo TimeZone
        {
            get
            {
                if (m_TimeZone == null)
                    m_TimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId);
                return m_TimeZone;
            }
            set
            {
                m_TimeZone = value;
                if (value == null)
                    this.TimeZoneId = null;
                else
                    this.TimeZoneId = value.Id;
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
        /// Gets or set value indicating that the files uploading is enabled or disabled.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the deleting is enabled.")]
        [DefaultValue(false)]
        public bool EnableUploading
        {
            get
            {
                object obj = ViewState["EnableUploading"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["EnableUploading"] = value; }
        }

        /// <summary>
        /// Gets the files count in the control.
        /// </summary>
        [Browsable(false)]
        public int FilesCount
        {
            get
            {
                object obj = ViewState["FilesCount"];
                return ((obj == null) ? 0 : (int)obj);
            }
            private set { ViewState["FilesCount"] = value; }
        }

        /// <summary>
        /// Gets the instance of the SimpleUpload control that used to upload files.
        /// </summary>
        [Category("Behavior")]
        [Description("The instance of the SimpleUpload control that used to upload files.")]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleUpload FilesUpload
        {
            get
            {
                this.EnsureFilesUpload();
                return m_FilesUpload;
            }
        }

        /// <summary>
        /// Gets or sets the size of pictures in icons column.
        /// </summary>
        [Category("Appearance")]
        [Description("The size of pictures in icons column.")]
        [DefaultValue(IconSize.Smaller)]
        public IconSize IconSize
        {
            get
            {
                object obj = this.ViewState["IconSize"];
                return ((obj == null) ? IconSize.Smaller : (IconSize)obj);
            }
            set { this.ViewState["IconSize"] = value; }
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
        /// Gets or sets the type of the object which the files are associated with.
        /// </summary>
        [Category("Data")]
        [Description("The type of the object which the files are associated with.")]
        [DefaultValue("")]
        public string LocalObjectType
        {
            get { return (string)this.ViewState["LocalObjectType"]; }
            set { this.ViewState["LocalObjectType"] = value; }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the object which the files are associated with.
        /// </summary>
        [Category("Data")]
        [Description("The unique identifier of the object which the files are associated with.")]
        [DefaultValue("")]
        public string LocalObjectId
        {
            get { return (string)this.ViewState["LocalObjectId"]; }
            set { this.ViewState["LocalObjectId"] = value; }
        }

        /// <summary>
        /// Gets or set a value indicating that the FileExtensionsFilter be negated.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether the FileExtensionsFilter be negated.")]
        [DefaultValue(false)]
        public bool NegateFileExtensionsFilter
        {
            get
            {
                object obj = ViewState["NegateFileExtensionsFilter"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { ViewState["NegateFileExtensionsFilter"] = value; }
        }

        /// <summary>
        /// Gets or sets the rendering mode for the control.
        /// </summary>
        [Category("Appearance")]
        [Description("The rendering mode.")]
        [DefaultValue(FileListRenderingMode.CommonGridView)]
        public FileListRenderingMode RenderingMode
        {
            get
            {
                object obj = this.ViewState["RenderingMode"];
                return ((obj == null) ? FileListRenderingMode.CommonGridView : (FileListRenderingMode)obj);
            }
            set { this.ViewState["RenderingMode"] = value; }
        }

        /// <summary>
        /// Gets or sets the number of columns to display in the thumbnails list.
        /// </summary>
        [Category("Appearance")]
        [Description("The number of columns to display in the thumbnails list.")]
        [DefaultValue(4)]
        public int RepeatColumns
        {
            get
            {
                object obj = this.ViewState["RepeatColumns"];
                return ((obj == null) ? 4 : (int)obj);
            }
            set { this.ViewState["RepeatColumns"] = value; }
        }

        /// <summary>
        /// Gets or sets whether the thumbnails list displays vertically or horizontally.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the thumbnails list displays vertically or horizontally.")]
        [DefaultValue(RepeatDirection.Horizontal)]
        public RepeatDirection RepeatDirection
        {
            get
            {
                object obj = this.ViewState["RepeatDirection"];
                return ((obj == null) ? RepeatDirection.Horizontal : (RepeatDirection)obj);
            }
            set { this.ViewState["RepeatDirection"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tool tip for the file is displayed.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the tool tip for the file is displayed.")]
        [DefaultValue(true)]
        public bool ShowFileToolTip
        {
            get
            {
                object obj = this.ViewState["ShowFileToolTip"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { this.ViewState["ShowFileToolTip"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the icons column are displayed in control.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the icons column are displayed in control.")]
        [DefaultValue(false)]
        public bool ShowIcons
        {
            get
            {
                object obj = this.ViewState["ShowIcons"];
                return ((obj == null) ? false : (bool)obj);
            }
            set { this.ViewState["ShowIcons"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view at once hyperlink is displayed in control.
        /// </summary>
        [Category("Appearance")]
        [Description("Whether the view at once hyperlink is displayed in control.")]
        [DefaultValue(true)]
        public bool ShowViewAllAtOnceLink
        {
            get
            {
                object obj = this.ViewState["ShowViewAllAtOnceLink"];
                return ((obj == null) ? true : (bool)obj);
            }
            set { this.ViewState["ShowViewAllAtOnceLink"] = value; }
        }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        [Category("Layout")]
        [Description("The width of the control.")]
        [DefaultValue(typeof(Unit), "")]
        public Unit Width
        {
            get
            {
                object obj = this.ViewState["Width"];
                return ((obj == null) ? Unit.Empty : (Unit)obj);
            }
            set { this.ViewState["Width"] = value; }
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

        #endregion

        #region Private Methods

        private void ApplyStyle()
        {
            GridView grid = Grid as GridView;
            if (grid != null)
            {
                if (!this.Width.IsEmpty)
                    grid.Width = this.Width;

                if (this.RenderingMode == FileListRenderingMode.CommonGridView)
                {
                    if (!this.Width.IsEmpty)
                        grid.Columns[(this.ShowIcons ? 1 : 0)].ItemStyle.Width = Unit.Percentage(100);

                    grid.ShowHeader = true;
                    grid.BorderWidth = Unit.Pixel(1);
                    grid.CellPadding = 0;
                    grid.CellSpacing = 0;
                    grid.GridLines = GridLines.Both;
                    grid.CssClass = "flGrid_T";
                    grid.HeaderStyle.CssClass = "flGrid_H";
                    grid.EmptyDataRowStyle.CssClass = "flGrid_Er";
                    grid.Attributes["bordercolor"] = "#666666";
                    grid.BorderColor = System.Drawing.ColorTranslator.FromHtml("#666666");
                    System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#F0F0F0");

                    bool enableDeleting = this.EnableDeleting;
                    string cssClass = null;

                    foreach (GridViewRow row in grid.Rows)
                    {
                        cssClass = "flGrid_C";
                        if ((row.RowState & DataControlRowState.Alternate) == DataControlRowState.Alternate)
                        {
                            cssClass = "flGrid_A";
                            row.BackColor = color;
                        }

                        int cellsCount = row.Cells.Count - 1;
                        int index = 0;
                        foreach (TableCell cell in row.Cells)
                        {
                            if (string.IsNullOrEmpty(cell.CssClass)) cell.CssClass = cssClass;
                            if (enableDeleting && (index == cellsCount)) cell.CssClass = "flGrid_Db";
                            index++;
                        }
                    }
                }
                else if (!this.Width.IsEmpty)
                {
                    grid.Columns[(this.ShowIcons ? 2 : 1)].ItemStyle.Width = Unit.Percentage(100);
                }
            }
        }

        private GridView CreateGridView()
        {
            GridView grid = new GridView();
            grid.ID = "Grid";
            grid.CssClass = "flGrid";
            grid.DataKeyNames = new string[] { MetaDataSet.FileDataTable.FileUniqueIdColumnName };
            grid.DataSourceID = GridDataSource.ID;
            grid.AutoGenerateColumns = false;
            grid.GridLines = GridLines.None;
            grid.ShowHeader = false;
            grid.RowDataBound += new GridViewRowEventHandler(Grid_RowDataBound);
            grid.RowDeleting += new GridViewDeleteEventHandler(Grid_RowDeleting);
            grid.DataBinding += new EventHandler(Grid_DataBinding);

            BoundField updatedTimeField = new BoundField();
            updatedTimeField.DataField = MetaDataSet.FileDataTable.UpdatedTimeColumnName;
            updatedTimeField.HeaderStyle.Wrap = false;
            updatedTimeField.HeaderText = Resources.FileList_UpdatedWhenText;
            updatedTimeField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            updatedTimeField.ItemStyle.Wrap = false;

            if (this.ShowIcons)
            {
                ImageField imageField = new ImageField();
                imageField.DataImageUrlField = MetaDataSet.FileDataTable.FileUniqueIdColumnName;
                grid.Columns.Add(imageField);
            }

            HyperLinkField linkField = new HyperLinkField();
            linkField.DataNavigateUrlFields = new string[] { MetaDataSet.FileDataTable.FileUniqueIdColumnName };
            linkField.DataTextField = MetaDataSet.FileDataTable.NameColumnName;
            linkField.HeaderText = Resources.FileList_FileNameText;
            linkField.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
            linkField.ControlStyle.CssClass = "flFileName";
            linkField.Target = "_blank";
            grid.Columns.Add(linkField);

            BoundField boundField = new BoundField();
            boundField.DataField = "SizeInKB";
            boundField.DataFormatString = "{0:N0} KB";
            boundField.HeaderStyle.Wrap = false;
            boundField.HeaderText = Resources.FileList_SizeText;
            boundField.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
            boundField.ItemStyle.Wrap = false;
            grid.Columns.Add(boundField);

            if (this.RenderingMode != FileListRenderingMode.GridView)
                grid.Columns.Add(updatedTimeField);

            if (this.EnableDeleting)
            {
                ButtonField buttonField = new ButtonField();
                buttonField.CausesValidation = false;
                buttonField.CommandName = DataList.DeleteCommandName;
                buttonField.Text = Resources.FileList_DeleteText;
                buttonField.ControlStyle.CssClass = "flRemove";
                buttonField.ItemStyle.Wrap = false;
                grid.Columns.Add(buttonField);
            }

            if (this.RenderingMode == FileListRenderingMode.GridView)
                grid.Columns.Add(updatedTimeField);

            return grid;
        }

        private DataList CreateDataList()
        {
            DataList dataList = new DataList();
            dataList.ID = "Grid";
            dataList.DataSourceID = GridDataSource.ID;
            dataList.CellSpacing = 0;
            dataList.CellPadding = 0;
            dataList.DataKeyField = MetaDataSet.FileDataTable.FileUniqueIdColumnName;
            dataList.DataBinding += new EventHandler(Grid_DataBinding);
            dataList.ItemDataBound += new DataListItemEventHandler(DataList_ItemDataBound);
            dataList.DeleteCommand += new DataListCommandEventHandler(DataList_DeleteCommand);

            switch (this.RenderingMode)
            {
                case FileListRenderingMode.ThumbnailsList:
                    dataList.CssClass = "flThumbs";
                    dataList.ItemTemplate = new ThumbnailsListItemTemplate(ListItemType.Item, this);
                    dataList.RepeatColumns = this.RepeatColumns;
                    dataList.RepeatDirection = this.RepeatDirection;
                    break;
                case FileListRenderingMode.FilesList:
                    dataList.CssClass = "flFiles";
                    dataList.ItemTemplate = new FilesListItemTemplate(ListItemType.Item, this);
                    break;
            }

            return dataList;
        }

        private void CreateFilesUpload()
        {
            m_FilesUpload.ID = "FilesUpload";
            m_FilesUpload.EnablePopupWindow = true;
            m_FilesUpload.SkipUploadControlsValidation = true;
            m_FilesUpload.UploadButtonAcceptChanges = true;
            m_FilesUpload.CssClass = "flUploadFiles";
            if (this.RenderingMode == FileListRenderingMode.GridView)
                m_FilesUpload.CssClass += " flLink";
            m_FilesUpload.LocalObjectId = this.LocalObjectId;
            m_FilesUpload.LocalObjectType = this.LocalObjectType;
            m_FilesUpload.UpdatedBy = this.UpdatedBy;
            m_FilesUpload.OrganizationId = this.OrganizationId;
            m_FilesUpload.OrganizationName = this.OrganizationName;
            m_FilesUpload.DepartmentId = this.DepartmentId;
            m_FilesUpload.DepartmentName = this.DepartmentName;
            m_FilesUpload.ConnectionString = this.ConnectionString;
            m_FilesUpload.OnClientAfterAcceptChanges = string.Format(CultureInfo.InvariantCulture
                , @"Sys.Application.add_load(function() {{ 
    var su = $find('SimpleUpload1'); 
    if (su && su._popupWindow && su._popupWindow.BrowserWindow && su._popupWindow.BrowserWindow.__doPostBack) 
        su._popupWindow.BrowserWindow.__doPostBack('{0}', 'Refresh');
}});"
                , this.ClientID);
        }

        private void DeleteFile(string fileUniqueId)
        {
            if (Access.DeleteFile(fileUniqueId, true, this.UpdatedBy, this.ConnectionString).Length == 0)
            {
                if (this.FileDeleted != null)
                {
                    MetaDataSet.FileRow row = null;
                    using (FileTableAdapter adapter = new FileTableAdapter(this.ConnectionString))
                    {
                        using (MetaDataSet.FileDataTable table = adapter.GetFile(fileUniqueId))
                        {
                            if (table.Count > 0) row = table[0];
                        }
                    }
                    this.FileDeleted(this, new CommandEventArgs("Delete", row));
                }

                Grid.DataBind();
                if (m_FilesUpload != null) m_FilesUpload.Reset();
            }
        }

        private void EnsureGrid()
        {
            if (Grid != null) return;

            if (this.RenderingMode != FileListRenderingMode.FilesList)
            {
                ToolTipManager = new RadToolTipManager();
                ToolTipManager.ID = "ToolTipManager";
                ToolTipManager.Position = ToolTipPosition.MiddleRight;
                ToolTipManager.HideEvent = ToolTipHideEvent.LeaveToolTip;
                if (this.RenderingMode != FileListRenderingMode.FilesList)
                    ToolTipManager.OnClientBeforeShow = "FileList_ToolTipBeforeShow";
            }

            if ((this.RenderingMode == FileListRenderingMode.ThumbnailsList) || (this.RenderingMode == FileListRenderingMode.FilesList))
                Grid = this.CreateDataList();
            else
                Grid = this.CreateGridView();
        }

        private void EnsureGridDataSource()
        {
            if (GridDataSource != null) return;

            GridDataSource = new ObjectDataSource();
            GridDataSource.ID = "GridDataSource";
            GridDataSource.TypeName = typeof(FileTableAdapter).FullName;
            GridDataSource.ObjectCreating += new ObjectDataSourceObjectEventHandler(GridDataSource_ObjectCreating);

            GridDataSource.SelectMethod = "GetFiles";
            GridDataSource.SelectParameters.Add("OrganizationId", TypeCode.Object, string.Empty);
            GridDataSource.SelectParameters.Add("DepartmentId", TypeCode.Object, string.Empty);
            GridDataSource.SelectParameters.Add("LocalObjectId", TypeCode.String, string.Empty);
            GridDataSource.SelectParameters.Add("LocalObjectType", TypeCode.String, string.Empty);
            GridDataSource.SelectParameters.Add("Deleted", TypeCode.Boolean, bool.FalseString);
            GridDataSource.Selecting += new ObjectDataSourceSelectingEventHandler(GridDataSource_Selecting);
        }

        private void EnsureGridCaptionPanel()
        {
            CaptionPanel = new Panel();
            CaptionPanel.ID = "CaptionPanel";

            if (this.ShowViewAllAtOnceLink)
            {
                ViewAllAtOnceLink = new HyperLink();
                ViewAllAtOnceLink.ID = "ViewAllAtOnceLink";
                ViewAllAtOnceLink.CssClass = "flCptCtrl";
                ViewAllAtOnceLink.Text = Resources.FileList_ViewAllAtOnceLink_Text;
                ViewAllAtOnceLink.Target = "_blank";
                ViewAllAtOnceLink.NavigateUrl = "#";

                if (this.RenderingMode == FileListRenderingMode.CommonGridView)
                    CaptionPanel.Controls.Add(ViewAllAtOnceLink);
                else if (this.RenderingMode == FileListRenderingMode.GridView)
                    ViewAllAtOnceLink.CssClass += " flLink";
            }

            this.EnsureFilesUpload();

            if (m_FilesUpload != null)
            {
                Separator = new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;");
                if (ViewAllAtOnceLink != null)
                {
                    if (this.RenderingMode == FileListRenderingMode.CommonGridView)
                        CaptionPanel.Controls.Add(Separator);
                }
                CaptionPanel.Controls.Add(m_FilesUpload);
                if (ViewAllAtOnceLink != null)
                {
                    if (this.RenderingMode != FileListRenderingMode.CommonGridView)
                        CaptionPanel.Controls.Add(Separator);
                }
            }

            if (ViewAllAtOnceLink != null)
            {
                if (this.RenderingMode != FileListRenderingMode.CommonGridView)
                    CaptionPanel.Controls.Add(ViewAllAtOnceLink);
            }

            if (CaptionPanel.HasControls()) this.Controls.Add(CaptionPanel);
        }

        private void EnsureFilesUpload()
        {
            if ((m_FilesUpload == null) && this.EnableUploading)
            {
                m_FilesUpload = new SimpleUpload();
                m_FilesUpload.ID = "FilesUpload";
            }
        }

        private static string GetFileTypeIconUrl(string fileName, IconSize iconSize)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant().TrimStart('.');
            string webResourceNameFormatString = string.Format(CultureInfo.InvariantCulture, "Images.Icons{0}x{0}.{{0}}.gif", (int)iconSize);
            return ResourceHandler.GetWebResourceUrl(string.Format(CultureInfo.InvariantCulture, webResourceNameFormatString, (Settings.KnownFileExtensions.Contains(extension) ? extension : Settings.KnownFileExtensions[0])), true);
        }

        private static string GetNonImageFileTypeIconUrl(string fileName, IconSize iconSize)
        {
            return (MimeType.IsImageType(MimeType.GetMimeType(Path.GetExtension(fileName))) ? null : GetFileTypeIconUrl(fileName, iconSize));
        }

        private void DataList_DeleteCommand(object source, DataListCommandEventArgs e)
        {
            DeleteFile((string)((source as DataList).DataKeys[e.Item.ItemIndex]));
        }

        private void DataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e == null) return;
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    this.FilesCount = this.FilesCount + 1;
                    break;
            }
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            this.FilesCount = 0;
            GridView grid = sender as GridView;
            if (grid != null)
            {
                grid.CellPadding = -1;
                grid.CellSpacing = 0;
                if (this.ShowIcons)
                {
                    if (this.IconSize != IconSize.Smaller)
                        grid.CellPadding = 0;
                    else
                        grid.CellSpacing = -1;
                    grid.Columns[0].ControlStyle.Width = grid.Columns[0].ControlStyle.Height = Unit.Pixel((int)this.IconSize);
                }
                else
                    grid.CellSpacing = -1;
            }
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e == null) return;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                this.FilesCount = this.FilesCount + 1;
                int count = e.Row.Cells.Count;
                if (count > 0)
                {
                    string fileUniqueId = DataBinder.Eval(e.Row.DataItem, MetaDataSet.FileDataTable.FileUniqueIdColumnName).ToString();
                    string fileName = DataBinder.Eval(e.Row.DataItem, MetaDataSet.FileDataTable.NameColumnName).ToString();

                    if (this.ShowIcons)
                    {
                        Image img = e.Row.Cells[0].Controls[0] as Image;
                        if (img != null)
                            img.ImageUrl = GetFileTypeIconUrl(fileName, this.IconSize);
                    }

                    HyperLink link = e.Row.Cells[this.ShowIcons ? ((this.RenderingMode == FileListRenderingMode.GridView) ? 1 : 0) : 0].Controls[0] as HyperLink;
                    if (link != null)
                    {
                        link.Attributes["id"] = link.ClientID;
                        link.NavigateUrl = Access.GetFileUrl(fileUniqueId, this.OrganizationId, this.DepartmentId);
                        if (MimeType.IsImageType(MimeType.GetMimeType(Path.GetExtension(fileName))))
                        {
                            if (ToolTipManager != null) ToolTipManager.TargetControls.Add(link.ClientID, string.Empty, true);
                        }
                    }

                    if (this.RenderingMode == FileListRenderingMode.GridView)
                    {
                        e.Row.Attributes["onmouseover"] = "this.className += ' flHover';";
                        e.Row.Attributes["onmouseout"] = "this.className = this.className.replace(' flHover', '');";
                    }

                    DateTime updatedTime = (DateTime)DataBinder.Eval(e.Row.DataItem, MetaDataSet.FileDataTable.UpdatedTimeColumnName);
                    updatedTime = TimeZoneInfo.ConvertTimeFromUtc(updatedTime, this.TimeZone);

                    TableCell cell = e.Row.Cells[((this.RenderingMode == FileListRenderingMode.GridView) ? (this.ShowIcons ? 4 : 3) + (!this.EnableDeleting ? -1 : 0) : (this.ShowIcons ? 3 : 2))];
                    cell.Text = string.Format(this.Culture, this.DateTimeFormatString, updatedTime);

                    TableCell deleteCell = e.Row.Cells[count - (this.RenderingMode == FileListRenderingMode.GridView ? 2 : 1)];

                    if (this.RenderingMode == FileListRenderingMode.GridView)
                    {
                        DateTime updatedDate = (DateTime)DataBinder.Eval(e.Row.DataItem, MetaDataSet.FileDataTable.UpdatedDateColumnName);
                        if (m_UpdatedDate == updatedDate)
                            cell.Text = string.Empty;
                        else
                        {
                            if (m_UpdatedDate != DateTime.MinValue) e.Row.CssClass += " flPt";
                            cell.CssClass = "flDate";
                        }
                        m_UpdatedDate = updatedDate;
                    }

                    cell.ToolTip = string.Format(this.Culture, this.DateTimeToolTipFormatString, updatedTime);

                    if (this.EnableDeleting && this.EnableDeletingConfirmation)
                    {
                        if (deleteCell.Controls.Count > 0)
                        {
                            WebControl control = e.Row.Cells[count - (this.RenderingMode == FileListRenderingMode.GridView ? 2 : 1)].Controls[0] as WebControl;
                            if (control != null) control.Attributes.Add("onclick", OnClientDeleting);
                        }
                    }
                }
            }
        }

        protected void Grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (e == null) return;
            e.Cancel = true;

            DeleteFile(e.Keys[0].ToString());
        }

        private void GridDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            if (e == null) return;

            e.ObjectInstance = new FileTableAdapter(this.ConnectionString);
        }

        private void GridDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (e == null) return;

            if (e.InputParameters.Contains("OrganizationId")) e.InputParameters["OrganizationId"] = this.OrganizationId;
            if (e.InputParameters.Contains("DepartmentId")) e.InputParameters["DepartmentId"] = this.DepartmentId;
            if (e.InputParameters.Contains("LocalObjectId")) e.InputParameters["LocalObjectId"] = this.LocalObjectId;
            if (e.InputParameters.Contains("LocalObjectType")) e.InputParameters["LocalObjectType"] = this.LocalObjectType;

            if (this.FileExtensionsFilter.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                string format = (this.NegateFileExtensionsFilter ? " AND Name NOT LIKE '%{0}'" : " OR Name LIKE '%{0}'");
                foreach (string ext in this.FileExtensionsFilterInternal)
                {
                    sb.AppendFormat(format, ext);
                }
                sb.Remove(0, 4);
                GridDataSource.FilterExpression = sb.ToString();
            }
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Creates the child controls.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (this.RenderingMode != FileListRenderingMode.GridView)
                this.EnsureGridCaptionPanel();

            this.EnsureGridDataSource();
            this.Controls.Add(GridDataSource);

            this.EnsureGrid();
            this.Controls.Add(Grid);

            if (this.RenderingMode == FileListRenderingMode.GridView)
                this.EnsureGridCaptionPanel();

            if (ToolTipManager != null)
                this.Controls.Add(ToolTipManager);
        }

        /// <summary>
        /// Raises the System.Web.UI.Control.PreRender event and registers the style sheet of the control.
        /// </summary>
        /// <param name="e">An System.EventArgs object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.FilesUpload != null) this.CreateFilesUpload();

            bool visibleToolTip = (this.ShowFileToolTip || (this.RenderingMode == FileListRenderingMode.ThumbnailsList));

            if (ToolTipManager != null)
                ToolTipManager.Visible = visibleToolTip;

            if ((this.RenderingMode != FileListRenderingMode.FilesList) && visibleToolTip)
                ScriptManager.RegisterClientScriptInclude(this.Page, this.Page.GetType(), "Scripts.FileList.js", ResourceHandler.GetWebResourceUrl("Scripts.FileList.js", true));

            this.ApplyStyle();
            SimpleUpload.RegisterControlStyleSheet(this, "Styles.FileList.css");
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            this.EnsureChildControls();
            if (ToolTipManager != null) ToolTipManager.TargetControls.Clear();
            base.DataBind();
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
                if (this.FilesUpload != null)
                {
                    if (this.FilesUpload.PopupWindow != null)
                    {
                        this.FilesUpload.PopupWindow.ShowContentDuringLoad = false;
                        this.FilesUpload.PopupWindow.ReloadOnShow = true;
                    }
                }

                if (Separator != null)
                    Separator.Visible = (!this.IsEmpty);

                if ((this.RenderingMode == FileListRenderingMode.CommonGridView) && (!this.IsEmpty))
                {
                    CaptionPanel.HorizontalAlign = HorizontalAlign.Right;
                    if (!this.Width.IsEmpty) writer.AddStyleAttribute(HtmlTextWriterStyle.Width, this.Width.ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                }

                if (ViewAllAtOnceLink != null)
                {
                    ViewAllAtOnceLink.NavigateUrl = this.ViewAllAtOnceLinkNavigateUrl;
                    ViewAllAtOnceLink.Visible = (!this.IsEmpty);
                }

                base.RenderControl(writer);

                if (this.IsEmpty)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "flGrid");
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, Grid.ClientID);
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(Resources.FileList_EmptyDataText);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                if ((this.RenderingMode == FileListRenderingMode.CommonGridView) && (!this.IsEmpty))
                    writer.RenderEndTag();
            }
        }

        #endregion

        #region Public Methods

        public void LoadProperties(string value)
        {
            SimpleUpload.LoadProperties(this, value);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "Refresh") this.DataBind();
        }

        #endregion
    }
}
