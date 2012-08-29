using System;
using System.IO;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Application;
using Micajah.Common.WebControls;
using Micajah.FileService.Dal;

namespace Micajah.FileService.Management
{
    public partial class StoragesPage : Page
    {
        #region Private Properties

        private Guid? OrganizationId
        {
            get
            {
                Guid? organizationId = null;
                ComboBox comboBox = EditForm.FindControl("OrganizationList") as ComboBox;
                if (comboBox != null) organizationId = Helper.CreateNullableGuid(comboBox.SelectedValue);
                return organizationId;
            }
        }

        private string StoragePath
        {
            get
            {
                Micajah.Common.WebControls.TextBox textBox = EditForm.FindControl("PathTextBox") as Micajah.Common.WebControls.TextBox;
                return ((textBox == null) ? null : textBox.Text);
            }
        }

        #endregion

        #region Private Methods

        private void SwitchToGrid()
        {
            Grid.SelectedIndex = -1;
            Grid.Visible = true;
            EditForm.Visible = false;
        }

        private void SwitchToEditForm()
        {
            Grid.Visible = false;
            EditForm.Visible = true;
        }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Grid.ColorScheme
                = EditForm.ColorScheme
                = WebApplicationSettings.DefaultColorScheme;
        }

        protected void Grid_Action(object sender, CommonGridViewActionEventArgs e)
        {
            switch (e.Action)
            {
                case CommandActions.Add:
                    EditForm.ChangeMode(DetailsViewMode.Insert);
                    this.SwitchToEditForm();
                    break;
                case CommandActions.Edit:
                    EditForm.ChangeMode(DetailsViewMode.Edit);
                    Grid.SelectedIndex = e.RowIndex;
                    this.SwitchToEditForm();
                    break;
            }
        }

        protected void GridDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e == null) return;

            MainDataSet.StorageDataTable table = e.ReturnValue as MainDataSet.StorageDataTable;
            if (table != null)
            {
                table.Columns.Add("CurrentSizeInMB", typeof(decimal), "CurrentSizeInBytes / 1048576");
                table.Columns.Add("CurrentSizeInPercent", typeof(decimal), "CurrentSizeInMB / MaxSizeInMB");
                table.Columns.Add("FreeSizeInMB", typeof(decimal), "MaxSizeInMB - CurrentSizeInMB");
                table.Columns.Add("FreeSizeInPercent", typeof(decimal), "FreeSizeInMB / MaxSizeInMB");
            }
        }

        protected void EditFormDataSource_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            e.InputParameters["Path"] = this.StoragePath;
            e.InputParameters["OrganizationId"] = this.OrganizationId;
        }

        protected void EditFormDataSource_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                string storagePath = this.StoragePath;

                if (!string.IsNullOrEmpty(storagePath))
                {
                    try
                    {
                        if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
                    }
                    catch (IOException) { }
                    catch (ArgumentException) { }
                    catch (UnauthorizedAccessException) { }
                    catch (NotSupportedException) { }
                }

                Guid? organizationGuid = this.OrganizationId;
                if (organizationGuid.HasValue)
                {
                    if (e.ReturnValue != null)
                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(MasterPage.MoveOrganizationToPrivateStorage));
                        thread.Start(new object[] { organizationGuid.Value, (Guid)e.ReturnValue, storagePath });
                    }
                }
            }
        }

        protected void EditFormDataSource_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            e.InputParameters["OrganizationId"] = this.OrganizationId;
        }

        protected void EditForm_DataBound(object sender, EventArgs e)
        {
            Micajah.Common.WebControls.TextBox textBox = EditForm.FindControl("PathTextBox") as Micajah.Common.WebControls.TextBox;
            if (textBox != null) textBox.Enabled = (EditForm.CurrentMode == DetailsViewMode.Insert);

            ComboBox comboBox = EditForm.FindControl("OrganizationList") as ComboBox;
            if (comboBox != null)
            {
                comboBox.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem((string)this.GetLocalResourceObject("AllOrganizationsText"), string.Empty));
                switch (EditForm.CurrentMode)
                {
                    case DetailsViewMode.Insert:
                        comboBox.Enabled = true;
                        break;
                    case DetailsViewMode.Edit:
                        comboBox.Enabled = false;
                        string organizationId = DataBinder.Eval(EditForm.DataItem, "OrganizationId", "{0:D}");
                        Telerik.Web.UI.RadComboBoxItem item = comboBox.FindItemByValue(organizationId);
                        if (item != null)
                        {
                            comboBox.ClearSelection();
                            item.Selected = true;
                        }
                        break;
                }
            }
        }

        protected void EditForm_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            this.SwitchToGrid();
            Grid.DataBind();
        }

        protected void EditForm_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            this.SwitchToGrid();
            Grid.DataBind();
        }

        protected void EditForm_ItemCommand(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
            {
                this.SwitchToGrid();
                EditForm.ChangeMode(DetailsViewMode.ReadOnly);
            }
        }

        #endregion
    }
}