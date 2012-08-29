<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Applications.aspx.cs" Inherits="Micajah.FileService.Management.ApplicationsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <mits:CommonGridView ID="Grid" runat="server" DataKeyNames="ApplicationGuid" DataSourceID="GridDataSource"
                Width="780px" AutoGenerateColumns="False" AutoGenerateEditButton="True" ShowAddLink="True"
                OnAction="Grid_Action" meta:resourcekey="Grid">
                <Columns>
                    <mits:TextBoxField DataField="Name" meta:resourcekey="NameColumn" />
                    <mits:TextBoxField DataField="ApplicationGuid" meta:resourcekey="ApplicationGuidColumn">
                        <ItemStyle Width="210px" Wrap="False" />
                    </mits:TextBoxField>
                </Columns>
            </mits:CommonGridView>
            <mits:MagicForm ID="EditForm" runat="server" AutoGenerateRows="False" DataSourceID="EditFormDataSource"
                DataKeyNames="ApplicationGuid" Width="490px" Visible="False" OnItemInserted="EditForm_ItemInserted"
                OnItemUpdated="EditForm_ItemUpdated" OnItemCommand="EditForm_ItemCommand" meta:resourcekey="EditForm"
                AutoGenerateEditButton="True" AutoGenerateInsertButton="True">
                <Fields>
                    <mits:TextBoxField DataField="Name" MaxLength="255" Columns="70" Required="True"
                        meta:resourcekey="NameField" />
                </Fields>
            </mits:MagicForm>
            <asp:ObjectDataSource ID="GridDataSource" runat="server" SelectMethod="GetVendorApplications"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.ApplicationTableAdapter">
                <SelectParameters>
                    <asp:QueryStringParameter Type="Object" Name="VendorGuid" QueryStringField="VendorGuid" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="EditFormDataSource" runat="server" InsertMethod="Insert"
                SelectMethod="GetApplication" TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.ApplicationTableAdapter"
                UpdateMethod="Update">
                <UpdateParameters>
                    <asp:Parameter Name="ApplicationGuid" Type="Object" />
                    <asp:Parameter Name="Name" Type="String" />
                </UpdateParameters>
                <SelectParameters>
                    <asp:ControlParameter ControlID="Grid" Name="ApplicationGuid" Type="Object" PropertyName="SelectedValue" />
                </SelectParameters>
                <InsertParameters>
                    <asp:Parameter Name="ApplicationGuid" Type="Object" />
                    <asp:QueryStringParameter Type="Object" Name="VendorGuid" QueryStringField="VendorGuid" />
                    <asp:Parameter Name="Name" Type="String" />
                </InsertParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
