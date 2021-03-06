﻿using System;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Application;
using Micajah.Common.WebControls;
using Micajah.FileService.Dal;
using Micajah.FileService.Dal.MainDataSetTableAdapters;

namespace Micajah.FileService.Management
{
    public partial class OrganizationsPage : Page
    {
        #region Private Methods

        private void SwitchToGrid()
        {
            Grid.Visible = true;
            EditFormTable.Visible = false;
        }

        private void SwitchToEditForm()
        {
            Grid.Visible = false;
            EditFormTable.Visible = true;
        }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Grid.ColorScheme = WebApplicationSettings.DefaultColorScheme;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (EditFormTable.Visible)
            {
                MagicForm.ApplyColorScheme(EditFormTable, WebApplicationSettings.DefaultColorScheme);
                AutoGeneratedButtonsField.InsertButtonSeparator(ButtonsSeparator);
            }
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                System.Web.UI.WebControls.CheckBox checkBox = e.Row.FindControl("HasPrivateStorageCheckBox") as System.Web.UI.WebControls.CheckBox;
                if (checkBox != null)
                {
                    bool hasPrivateStorageCheckBox = (bool)DataBinder.Eval(e.Row.DataItem, "HasPrivateStorage");
                    checkBox.Checked = hasPrivateStorageCheckBox;
                    checkBox.Enabled = (!hasPrivateStorageCheckBox);
                    checkBox.ToolTip = (hasPrivateStorageCheckBox ? string.Empty : (string)this.GetLocalResourceObject("HasPrivateStorageCheckBox.ToolTip"));
                }
            }
        }

        protected void HasPrivateStorageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            System.Web.UI.WebControls.CheckBox checkBox = sender as System.Web.UI.WebControls.CheckBox;
            if (checkBox != null)
            {
                checkBox.Checked = false;
                this.SwitchToEditForm();
                GridViewRow row = checkBox.Parent.Parent as GridViewRow;
                if (row != null)
                {
                    Guid organizationGuid = (Guid)Grid.DataKeys[row.RowIndex]["OrganizationGuid"];
                    OrganizationNameTextBox.Text = row.Cells[0].Text;
                    OrganizationGuidTextBox.Text = organizationGuid.ToString("D");
                }
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            Guid organizationGuid = Helper.CreateGuid(OrganizationGuidTextBox.Text);
            Guid storageGuid = Helper.CreateGuid(StorageList.SelectedValue);
            string storagePath = null;

            using (StorageTableAdapter storageAdapter = new StorageTableAdapter())
            {
                MainDataSet.StorageDataTable storageTable = storageAdapter.GetStorage(storageGuid);
                if (storageTable.Count > 0)
                {
                    MainDataSet.StorageRow storageRow = storageTable[0];

                    storagePath = storageRow.Path;

                    storageRow.OrganizationId = organizationGuid;
                    storageAdapter.Update(storageRow);
                }
            }

            Thread thread = new Thread(new ParameterizedThreadStart(MasterPage.MoveOrganizationToPrivateStorage));
            thread.Start(new object[] { organizationGuid, storageGuid, storagePath });

            this.SwitchToGrid();
            Grid.DataBind();
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            this.SwitchToGrid();
        }

        #endregion
    }
}