<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImageUpload.aspx.cs" Inherits="Micajah.FileService.Web.ImageUploadPage" %>

<%@ Register Src="Controls/Navigator.ascx" TagName="Navigator" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ImageUpload</title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:Navigator ID="Navigator1" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <mfs:ImageUpload ID="ImageUpload2" runat="server" LocalObjectType="test object type"
                LocalObjectId="4" OrganizationId="f53995a0-2b8c-4c16-8be4-476495220ea6" OrganizationName="test organization"
                DepartmentId="96ca7b84-bdf7-4af2-b78c-29e60f6569fe" DepartmentName="test department" />
            <br />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Accept Changes" />&nbsp;<asp:Button
                ID="Button2" runat="server" OnClick="Button2_Click" Text="Reject Changes" />
            <asp:Button ID="Button3" runat="server" Text="Change Object" OnClick="Button3_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
