<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FileList.aspx.cs" Inherits="Micajah.FileService.Web.FileListPage" %>

<%@ Register Src="Controls/Navigator.ascx" TagName="Navigator" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FileList</title>
    <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 12px;
        }
        p
        {
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:Navigator ID="Navigator1" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <p>
        The file list with the deleting function</p>
    <mfs:FileList ID="FileList1" runat="server" LocalObjectType="test object type" LocalObjectId="1"
        DateTimeToolTipFormatString="{0:g} (UTC-5)" RenderingMode="CommonGridView" EnableUploading="true"
        Width="100%" OrganizationId="f53995a0-2b8c-4c16-8be4-476495220ea6" OrganizationName="test organization"
        DepartmentId="96ca7b84-bdf7-4af2-b78c-29e60f6569fe" DepartmentName="test department">
    </mfs:FileList>
    <p>
        <br />
        <br />
        The files list with the deleting function and thumbnails
    </p>
    <table>
        <tr>
            <td>
                Please select the icon size:
            </td>
            <td>
                <asp:DropDownList ID="IconSizeList" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
            </td>
        </tr>
    </table>
    <br />
    <mfs:FileList ID="FileList2" runat="server" LocalObjectType="test object type" LocalObjectId="1"
        DateTimeToolTipFormatString="{0:g} (UTC-5)" ShowIcons="True" RenderingMode="GridView"
        OrganizationId="f53995a0-2b8c-4c16-8be4-476495220ea6" OrganizationName="test organization"
        DepartmentId="96ca7b84-bdf7-4af2-b78c-29e60f6569fe" DepartmentName="test department">
    </mfs:FileList>
    <p>
        <br />
        <br />
        The thumbnails list
    </p>
    <table>
        <tr>
            <td>
                Please input the files extensions separated by comma to display in this list:
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="FilterTextBox" runat="server" Text="video" />&nbsp;
                <asp:CheckBox ID="NegateCheckBox" runat="server" Text="Negate filter" />&nbsp;
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
            </td>
        </tr>
    </table>
    <br />
    <mfs:FileList ID="FileList3" runat="server" LocalObjectType="test object type" LocalObjectId="1"
        DateTimeToolTipFormatString="{0:g} (UTC-5)" RenderingMode="ThumbnailsList" ShowViewAllAtOnceLink="false"
        OrganizationId="f53995a0-2b8c-4c16-8be4-476495220ea6" OrganizationName="test organization"
        DepartmentId="96ca7b84-bdf7-4af2-b78c-29e60f6569fe" DepartmentName="test department" />
    </form>
</body>
</html>
