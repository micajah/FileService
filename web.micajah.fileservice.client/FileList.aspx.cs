using System;
using Micajah.FileService.WebControls;

namespace Micajah.FileService.Web
{
    public partial class FileListPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                FileList3.FileExtensionsFilter = FilterTextBox.Text.Split(',');
                FileList3.NegateFileExtensionsFilter = NegateCheckBox.Checked;

                foreach (string name in Enum.GetNames(typeof(IconSize)))
                {
                    IconSizeList.Items.Add(name);
                }
                IconSizeList.SelectedValue = IconSize.Smaller.ToString();
            }

        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            FileList3.FileExtensionsFilter = FilterTextBox.Text.Split(',');
            FileList3.NegateFileExtensionsFilter = NegateCheckBox.Checked;
            FileList3.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            FileList2.IconSize = (IconSize)Enum.Parse(typeof(IconSize), IconSizeList.SelectedValue);
            IconSizeList.SelectedValue = FileList2.IconSize.ToString();
            FileList2.DataBind();
        }
    }
}