using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Micajah.FileService.WebControls;

namespace Micajah.FileService.Web
{
    public partial class SimpleUploadTestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("IsActive", typeof(bool));
                dt.Rows.Add(1, "Name1", "Description1", true);

                DetailsView1.DataSource = dt;
                DetailsView1.DataBind();
            }

            //Label1.Text = string.Empty;
        }
        //protected void Button1_Click(object sender, EventArgs e)
        //{
        //    //Response.Write(SimpleUpload1.UploadedFiles.Count.ToString());
        //    //Page.Response.Write("<br/>Button1_Click<br/>");
        //}
        //protected void LinkButton1_Click(object sender, EventArgs e)
        //{
        //    //Response.Write(SimpleUpload1.UploadedFiles.Count.ToString());
        //    //Page.Response.Write("<br/>LinkButton1_Click<br/>");
        //}

        protected void DetailsView1_ModeChanging(object sender, DetailsViewModeEventArgs e)
        {
            e.Cancel = true;
        }
        protected void DetailsView1_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            SimpleUpload ctl = DetailsView1.FindControl("SimpleUpload1") as SimpleUpload;
            if (ctl != null)
            {
                ctl.AcceptChanges();
                //Label1.Text = ctl.UploadedFiles.Count.ToString() + " files uploaded.";
            }
        }
        protected void DetailsView1_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                SimpleUpload ctl = DetailsView1.FindControl("SimpleUpload1") as SimpleUpload;
                if (ctl != null)
                {
                    ctl.RejectChanges();
                }
            }
        }
    }
}
