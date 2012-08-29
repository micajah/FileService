<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Navigator.ascx.cs" Inherits="Micajah.FileService.Web.Controls.Navigator" %>
<div style="font-family: Arial; font-size: 11px; text-align: right; border-bottom: solid 1px Gray;
    margin-bottom: 15px; padding-bottom: 5px;">
    <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/Default.aspx">Default</asp:HyperLink>&nbsp;|&nbsp;
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/SimpleUpload.aspx">SimpleUpload</asp:HyperLink>&nbsp;|&nbsp;
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/FileList.aspx">FileList</asp:HyperLink>&nbsp;|&nbsp;
    <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/ImageUpload.aspx">ImageUpload</asp:HyperLink>
</div>
