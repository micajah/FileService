<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Async="true" CodeFile="Organizations.aspx.cs" Inherits="Micajah.FileService.Management.OrganizationsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <style type="text/css">
        .CheckBox input
        {
            margin: 0;
            height: 13px;
        }
    </style>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <mits:CommonGridView ID="Grid" runat="server" DataKeyNames="OrganizationGuid" DataSourceID="GridDataSource"
                Width="780px" AutoGenerateColumns="False" AllowSorting="true" AllowPaging="true"
                meta:resourcekey="Grid" OnRowDataBound="Grid_RowDataBound">
                <Columns>
                    <mits:TextBoxField DataField="Name" SortExpression="Name" meta:resourcekey="NameColumn" />
                    <mits:TextBoxField DataField="OrganizationGuid" SortExpression="OrganizationGuid"
                        meta:resourcekey="OrganizationGuidColumn">
                        <ItemStyle Width="210px" Wrap="False" />
                    </mits:TextBoxField>
                    <asp:TemplateField SortExpression="HasPrivateStorage" meta:resourcekey="HasPrivateStorageColumn">
                        <ItemTemplate>
                            <asp:CheckBox ID="HasPrivateStorageCheckBox" runat="server" CssClass="CheckBox" AutoPostBack="true"
                                OnCheckedChanged="HasPrivateStorageCheckBox_CheckedChanged" />
                        </ItemTemplate>
                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
            </mits:CommonGridView>
            <asp:Table ID="EditFormTable" runat="server" Visible="false" Width="490px" meta:resourcekey="EditFormTable">
                <asp:TableRow runat="server" VerticalAlign="Top">
                    <asp:TableCell runat="server">
                        <asp:Literal ID="OrganizationNameLiteral" runat="server" meta:resourcekey="OrganizationNameLiteral"></asp:Literal>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="OrganizationNameTextBox" runat="server" Enabled="false" Columns="70"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Literal ID="OrganizationGuidLiteral" runat="server" meta:resourcekey="OrganizationGuidLiteral"></asp:Literal>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="OrganizationGuidTextBox" runat="server" Enabled="false" Columns="70"></asp:TextBox>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Literal ID="StorageLiteral" runat="server" meta:resourcekey="StorageLiteral"></asp:Literal>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:DropDownList ID="StorageList" runat="server" DataTextField="Path" DataValueField="StorageGuid"
                            DataSourceID="StorageDataSource" Width="380px">
                        </asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow runat="server" TableSection="TableFooter">
                    <asp:TableCell runat="server" ColumnSpan="2">
                        <asp:Button ID="SubmitButton" runat="server" OnClick="SubmitButton_Click" meta:resourcekey="SubmitButton" />
                        <asp:PlaceHolder ID="ButtonsSeparator" runat="server" />
                        <asp:LinkButton ID="CancelButton" runat="server" CssClass="Mf_Cb" OnClick="CancelButton_Click"
                            meta:resourcekey="CancelButton" />
                    </asp:TableCell></asp:TableFooterRow>
            </asp:Table>
            <asp:ObjectDataSource ID="GridDataSource" runat="server" SelectMethod="GetOrganizations"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.OrganizationTableAdapter">
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="StorageDataSource" runat="server" SelectMethod="GetStorages"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.StorageTableAdapter"
                FilterExpression="Active = 1">
                <SelectParameters>
                    <asp:Parameter Name="Private" Type="Boolean" DefaultValue="false" />
                </SelectParameters>
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
