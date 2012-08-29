<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="VendorsUsedSpace.aspx.cs" Inherits="Micajah.FileService.Management.VendorsUsedSpacePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:Table ID="VendorsUsedSpaceReportTable" runat="server" meta:resourcekey="VendorsUsedSpaceReportTable">
        <asp:TableRow runat="server" VerticalAlign="Top">
            <asp:TableCell runat="server" RowSpan="2">
                <mits:DateRange ID="VendorsUsedSpaceReportDatesRange" runat="server" DateRangeParts="None"
                    Required="true" />
            </asp:TableCell>
            <asp:TableCell runat="server">
                <div style="padding: 3px 0 4px 0;">
                    <asp:Label ID="VendorNameLabel" runat="server" meta:resourcekey="VendorNameLabel"></asp:Label></div>
                <mits:TextBox ID="VendorName" runat="server" MaxLength="255" Columns="50" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableFooterRow runat="server" TableSection="TableFooter">
            <asp:TableCell runat="server" ColumnSpan="2">
                <asp:Button ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" meta:resourcekey="SubmitButton" /></asp:TableCell>
        </asp:TableFooterRow>
    </asp:Table>
    <br />
    <rsweb:ReportViewer ID="VendorsUsedSpaceReportViewer" runat="server" Width="800px"
        Height="400px">
    </rsweb:ReportViewer>
</asp:Content>
