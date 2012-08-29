<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SimpleUpload.aspx.cs" Inherits="Micajah.FileService.Web.SimpleUploadTestPage" %>

<%@ Register Src="Controls/Navigator.ascx" TagName="Navigator" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SimpleUpload</title>
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
    <%--<br />
        <asp:Button ID="Button1" runat="server" Text="Save" OnClick="Button1_Click" />
        <br />
        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Simple click</asp:LinkButton>--%>
    <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateEditButton="True" AutoGenerateRows="False"
        Height="50px" Caption="Simple Edit Form" DataKeyNames="Id" DefaultMode="Edit"
        OnModeChanging="DetailsView1_ModeChanging" OnItemUpdating="DetailsView1_ItemUpdating"
        CellPadding="4" ForeColor="Black" BackColor="#E9ECF1" BorderColor="Black" BorderWidth="1px"
        GridLines="None" OnItemCommand="DetailsView1_ItemCommand" Width="700px">
        <Fields>
            <asp:BoundField DataField="Id" HeaderText="Id" HeaderStyle-Width="80px" />
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:TemplateField HeaderText="Attaches">
                <ItemTemplate>
                    <mfs:SimpleUpload ID="SimpleUpload1" runat="server" LocalObjectType="test object type"
                        LocalObjectId="1" UploadControlsUniqueId="DetailsView1$ctl05" ShowProgressArea="true"
                        FileSelectorMode="SingleFile" OrganizationId="f53995a0-2b8c-4c16-8be4-476495220ea6"
                        OrganizationName="test organization" DepartmentId="96ca7b84-bdf7-4af2-b78c-29e60f6569fe"
                        DepartmentName="test department" ShowTitle="false" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
        </Fields>
    </asp:DetailsView>
    <br />
    <%--<asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>--%>
    </form>
</body>
</html>
