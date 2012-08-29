<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.UI.Page" %>

<%@ Register Assembly="Micajah.FileService" Namespace="Micajah.FileService.WebControls" TagPrefix="mfs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script runat="server" type="text/C#">
    private void Page_Load(object sender, EventArgs e)
    {
        if (!this.Page.IsPostBack) ImageUpload1.LoadProperties(this.Page.Request.QueryString["p"]);
    }
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <mfs:ImageUpload ID="ImageUpload1" runat="server" />
    </form>
</body>
</html>
