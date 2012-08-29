using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.Application;
using Micajah.Common.WebControls;

namespace Micajah.FileService.Management
{
    public partial class VendorsPage : Page
    {
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