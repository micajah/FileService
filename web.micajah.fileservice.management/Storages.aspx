<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Async="true" CodeFile="Storages.aspx.cs" Inherits="Micajah.FileService.Management.StoragesPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBody" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <mits:CommonGridView ID="Grid" runat="server" DataSourceID="GridDataSource" DataKeyNames="StorageGuid"
                Width="780px" AutoGenerateColumns="False" AllowSorting="true" AutoGenerateEditButton="True"
                ShowAddLink="True" meta:resourcekey="Grid" OnAction="Grid_Action">
                <Columns>
                    <mits:TextBoxField DataField="Path" SortExpression="Path" meta:resourcekey="PathColumn">
                        <ItemStyle Wrap="false" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="MaxSizeInMb" DataFormatString="{0:N4}" SortExpression="MaxSizeInMb"
                        meta:resourcekey="MaxSizeInMBColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="CurrentSizeInMB" DataFormatString="{0:N4}" SortExpression="CurrentSizeInMB"
                        meta:resourcekey="CurrentSizeInMBColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="CurrentSizeInPercent" DataFormatString="{0:P}" SortExpression="CurrentSizeInPercent"
                        meta:resourcekey="CurrentSizeInPercentColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="FreeSizeInMB" DataFormatString="{0:N4}" SortExpression="FreeSizeInMB"
                        meta:resourcekey="FreeSizeInMBColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="FreeSizeInPercent" DataFormatString="{0:P}" SortExpression="FreeSizeInPercent"
                        meta:resourcekey="FreeSizeInPercentColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="MaxFileCount" SortExpression="MaxFileCount" meta:resourcekey="MaxFileCountColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="CurrentFileCount" SortExpression="CurrentFileCount"
                        meta:resourcekey="CurrentFileCountColumn">
                        <ItemStyle HorizontalAlign="Right" />
                    </mits:TextBoxField>
                    <mits:CheckBoxField DataField="Active" SortExpression="Active" meta:resourcekey="ActiveColumn">
                        <HeaderStyle Width="30px" />
                        <ItemStyle Width="30px" />
                    </mits:CheckBoxField>
                </Columns>
            </mits:CommonGridView>
            <mits:MagicForm ID="EditForm" runat="server" AutoGenerateRows="False" DataSourceID="EditFormDataSource"
                DataKeyNames="StorageGuid" Visible="False" Width="490px" OnItemInserted="EditForm_ItemInserted"
                OnItemUpdated="EditForm_ItemUpdated" OnItemCommand="EditForm_ItemCommand" meta:resourcekey="EditForm"
                AutoGenerateEditButton="True" AutoGenerateInsertButton="True" OnDataBound="EditForm_DataBound">
                <Fields>
                    <mits:TemplateField PaddingLeft="false" meta:resourcekey="PathField">
                        <ItemTemplate>
                            <mits:TextBox ID="PathTextBox" runat="server" Required="true" MaxLength="3000" Columns="70"
                                ValidationGroup="<%# EditForm.ClientID %>" Text='<%# Eval("Path") %>' ValidationType="RegularExpression"
                                ValidationExpression="[a-zA-Z]:(\\[\w .-]*)+" />
                        </ItemTemplate>
                    </mits:TemplateField>
                    <mits:TextBoxField DataField="MaxSizeInMb" DataFormatString="{0:N4}" MaxLength="14"
                        Columns="10" ValidationType="Double" meta:resourcekey="MaxSizeInMBField">
                    </mits:TextBoxField>
                    <mits:TextBoxField DataField="MaxFileCount" MaxLength="9" Columns="10" ValidationType="Integer"
                        meta:resourcekey="MaxFileCountField">
                    </mits:TextBoxField>
                    <mits:TemplateField PaddingLeft="false" meta:resourcekey="OrganizationIdField">
                        <ItemTemplate>
                            <mits:ComboBox ID="OrganizationList" runat="server" DataSourceID="OrganizationDataSource"
                                DataTextField="Name" DataValueField="OrganizationGuid" Width="380px">
                            </mits:ComboBox>
                        </ItemTemplate>
                    </mits:TemplateField>
                    <mits:CheckBoxField DataField="Active" DefaultChecked="true" meta:resourcekey="ActiveField">
                    </mits:CheckBoxField>
                </Fields>
            </mits:MagicForm>
            <asp:ObjectDataSource ID="GridDataSource" runat="server" SelectMethod="GetStorages"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.StorageTableAdapter"
                OnSelected="GridDataSource_Selected"></asp:ObjectDataSource>
            <asp:ObjectDataSource ID="EditFormDataSource" runat="server" InsertMethod="InsertStorage"
                SelectMethod="GetStorage" TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.StorageTableAdapter"
                UpdateMethod="Update" OnInserting="EditFormDataSource_Inserting" OnUpdating="EditFormDataSource_Updating"
                OnInserted="EditFormDataSource_Inserted">
                <UpdateParameters>
                    <asp:Parameter Name="StorageGuid" DbType="Guid" />
                    <asp:Parameter Name="MaxSizeInMB" Type="Decimal" />
                    <asp:Parameter Name="MaxFileCount" Type="Int32" />
                    <asp:Parameter Name="OrganizationId" DbType="Guid" />
                    <asp:Parameter Name="Active" Type="Boolean" />
                </UpdateParameters>
                <SelectParameters>
                    <asp:ControlParameter Name="StorageGuid" ControlID="Grid" PropertyName="SelectedValue"
                        DbType="Guid" />
                </SelectParameters>
                <InsertParameters>
                    <asp:Parameter Name="Path" Type="String" />
                    <asp:Parameter Name="MaxSizeInMB" Type="Decimal" />
                    <asp:Parameter Name="MaxFileCount" Type="Int32" />
                    <asp:Parameter Name="OrganizationId" DbType="Guid" />
                    <asp:Parameter Name="Active" Type="Boolean" />
                </InsertParameters>
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="OrganizationDataSource" runat="server" SelectMethod="GetOrganizations"
                TypeName="Micajah.FileService.Dal.MainDataSetTableAdapters.OrganizationTableAdapter">
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
