<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Vendors.aspx.cs" Inherits="Micajah.FileService.Management.VendorsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <mits:CommonGridView ID="Grid" runat="server" DataKeyNames="VendorGuid" DataSourceID="GridDataSource"
                Width="780px" AutoGenerateColumns="False" AllowSorting="true" AutoGenerateEditButton="True"
                ShowAddLink="True" OnAction="Grid_Action" meta:resourcekey="Grid">
                <Columns>
                    <mits:TextBoxField DataField="Name" SortExpression="Name" meta:resourcekey="NameColumn" />
                    <mits:TextBoxField DataField="VendorGuid" SortExpression="VendorGuid" meta:resourcekey="VendorGuidColumn">
                        <ItemStyle Width="210px" Wrap="False" />
                    </mits:TextBoxField>
                    <mits:HyperLinkField DataNavigateUrlFields="VendorGuid" DataNavigateUrlFormatString="~/Applications.aspx?VendorGuid={0}"
                        meta:resourcekey="ApplicationsColumn">
                        <ItemStyle Width="60px" />
                    </mits:HyperLinkField>
                </Columns>
            </mits:CommonGridView>
            <mits:MagicForm ID="EditForm" runat="server" AutoGenerateRows="False" DataSourceID="EditFormDataSource"
                DataKeyNames="VendorGuid" Width="490px" Visible="False" OnItemInserted="EditForm_ItemInserted"
                OnItemUpdated="EditForm_ItemUpdated" OnItemCommand="EditForm_ItemCommand" meta:resourcekey="EditForm"
                AutoGenerateEditButton="True" AutoGenerateInsertButton="True">
                <Fields>
                    <mits:TextBoxField DataField="Name" MaxLength="255" Columns="70" Required="True"
                        meta:resourcekey="NameField" />
                </Fields>
            </mits:MagicForm>
            <asp:ObjectDataSource ID="GridDataSource" runat="server" SelectMethod="GetVendors"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.VendorTableAdapter">
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="EditFormDataSource" runat="server" InsertMethod="Insert"
                SelectMethod="GetVendor" TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.VendorTableAdapter"
                UpdateMethod="Update">
                <UpdateParameters>
                    <asp:Parameter Name="VendorGuid" Type="Object" />
                    <asp:Parameter Name="name" Type="String" />
                </UpdateParameters>
                <SelectParameters>
                    <asp:ControlParameter Name="VendorGuid" Type="Object" ControlID="Grid" PropertyName="SelectedValue" />
                </SelectParameters>
                <InsertParameters>
                    <asp:Parameter Name="VendorGuid" Type="Object" />
                    <asp:Parameter Name="name" Type="String" />
                </InsertParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
