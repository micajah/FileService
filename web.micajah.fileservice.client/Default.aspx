<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Micajah.FileService.Web.DefaultPage" %>

<%@ Register Src="Controls/Navigator.ascx" TagName="Navigator" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Navigator ID="Navigator1" runat="server" />
        <div>
            <table id="tblMain" runat="server" align="left" border="0" cellpadding="2" style="width: 524px;
                height: 1px">
                <tr>
                    <td align="center" colspan="3" style="height: 9px; font-weight: bold; font-size: 14pt;
                        color: green;">
                        &nbsp;- FileService -</td>
                </tr>
                <tr>
                    <td align="center" colspan="3" style="font-weight: bold; color: green; height: 9px">
                        test environment</td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 9px; font-weight: bold;" bgcolor="gainsboro">
                        General information:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        ApplicationGUID:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtApplicationGUID" runat="server" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        OrganizationName:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtOrganizationName" runat="server" Width="269px">micajah</asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        OrganizationGUID:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtOrganizationGUID" runat="server" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        DepartmentName:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtDepartmentName" runat="server" Width="269px">micajah</asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        DepartmentGUID:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtDepartmentGUID" runat="server" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="font-weight: bold; font-style: normal; height: 9px; font-variant: normal"
                        bgcolor="gainsboro">
                        Add operations:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        Put File:
                    </td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtFilePath" runat="server" Width="269px">&lt; Enter full path to file, ex.: C:\ibm.jpg &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                        <asp:Button ID="btnPutFile" runat="server" Text="Put..." Width="120px" OnClick="btnPutFile_Click" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        Put File As Byte[]:
                    </td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtPutFileAsByteArray" runat="server" Width="269px">&lt; Enter full path to file, ex.: C:\ibm.jpg &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                        <asp:Button ID="btnPutFileAsByteArray" runat="server" Text="Put as array..." Width="120px"
                            OnClick="btnPutFileAsByteArray_Click" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        Put File From URL:
                    </td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtFileURL" runat="server" Width="269px">&lt; Enter url to file, ex.: http://test.com/picture.jpg &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 9px">
                        <asp:Button ID="btnPutFileFromURL" runat="server" Text="Put from url..." Width="120px"
                            OnClick="btnPutFileFromURL_Click" /></td>
                </tr>
                <tr>
                    <td colspan="3" style="font-weight: bold; font-style: normal; height: 9px" bgcolor="gainsboro">
                        Get operations:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" nowrap="nowrap" style="width: 115px; height: 9px">
                        Get File:</td>
                    <td style="width: 280px; height: 9px">
                        <asp:TextBox ID="txtGetFile" runat="server" Width="269px">&lt; Enter file id &gt;</asp:TextBox></td>
                    <td style="width: 123px">
                        <asp:Button ID="btnGetFile" runat="server" OnClick="btnGetFile_Click" Text="Get..."
                            Width="75px" /><asp:Button ID="btnFileURL" runat="server" Text="URL" Width="45px"
                                OnClick="btnFileURL_Click" Enabled="False" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 11px">
                        Get Thumbnail:
                    </td>
                    <td nowrap="nowrap" style="width: 280px; height: 11px">
                        <asp:TextBox ID="txtGetThumbnail" runat="server" Width="269px">&lt; Enter file id &gt;</asp:TextBox></td>
                    <td nowrap="nowrap" style="width: 123px; height: 11px">
                        <asp:TextBox ID="txtThumbnailAlign" runat="server" Width="114px">1</asp:TextBox></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                    </td>
                    <td nowrap="nowrap" style="width: 280px">
                        &nbsp;Width:
                        <asp:TextBox ID="txtGetThumbnailWidth" runat="server" Width="78px">0</asp:TextBox>&nbsp;
                        &nbsp;Height:
                        <asp:TextBox ID="txtGetThumbnailHeight" runat="server" Width="79px">0</asp:TextBox></td>
                    <td style="width: 123px">
                        <asp:Button ID="btnGetThumbnail" runat="server" Text="Get..." Width="75px" OnClick="btnGetThumbnail_Click" /><asp:Button
                            ID="btnThumbnailURL" runat="server" Text="URL" Width="45px" OnClick="btnThumbnailURL_Click"
                            Enabled="False" /></td>
                </tr>
                <tr>
                    <td colspan="3" style="font-weight: bold; height: 12px" bgcolor="gainsboro">
                        Update operations:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        Update File:
                    </td>
                    <td nowrap="nowrap" style="width: 280px">
                        <asp:TextBox ID="txtUpdateFile" runat="server" Width="96px">&lt; Enter file id &gt;</asp:TextBox>
                        &nbsp;
                        <asp:TextBox ID="txtUpdateFilePath" runat="server" Width="155px">&lt; Enter full path to file &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        <asp:Button ID="btnUpdateFile" runat="server" OnClick="btnUpdateFile_Click" Text="Update..."
                            Width="120px" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        Update As Byte[]:</td>
                    <td nowrap="nowrap" style="width: 280px">
                        <asp:TextBox ID="txtUpdateAsArray" runat="server" Width="96px">&lt; Enter file id &gt;</asp:TextBox>&nbsp;
                        &nbsp;<asp:TextBox ID="txtUpdateAsArrayPath" runat="server" Width="155px">&lt; Enter full path to file &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        <asp:Button ID="btnUpdateAsArray" runat="server" OnClick="btnUpdateAsArray_Click"
                            Text="Update as array..." Width="120px" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        Update From URL:</td>
                    <td nowrap="nowrap" style="width: 280px">
                        <asp:TextBox ID="txtUpdateFromURL" runat="server" Width="96px">&lt; Enter file id &gt;</asp:TextBox>&nbsp;
                        &nbsp;<asp:TextBox ID="txtUpdateFromURLPath" runat="server" Width="155px">&lt; Enter full path to file &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        <asp:Button ID="btnUpdateFromURL" runat="server" Text="Update from url..." Width="120px"
                            OnClick="btnUpdateFromURL_Click" /></td>
                </tr>
                <tr>
                    <td colspan="3" style="font-weight: bold; height: 12px" bgcolor="gainsboro">
                        Delete operations:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        Delete File:</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtDeleteFile" runat="server" Width="269px">&lt; Enter file id &gt;</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        <asp:Button ID="btnDelete" runat="server" Text="Delete..." Width="120px" OnClick="btnDelete_Click" /></td>
                </tr>
                <tr>
                    <td align="left" bgcolor="gainsboro" class="text18" colspan="3" style="font-weight: bold;
                        height: 12px">
                        Upload screen:</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        <asp:TextBox ID="txtUploadFileId" runat="server" Width="109px">&lt; Enter file id &gt;</asp:TextBox></td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtAccessTypes" runat="server" Width="268px">graphic types</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        <asp:TextBox ID="txtMaxCountFiles" runat="server" Width="114px">0</asp:TextBox></td>
                </tr>
                <tr>
                    <td align="left" class="text18" style="width: 115px; height: 12px">
                        <asp:TextBox ID="txtUploadFilesLinkCaption" runat="server"></asp:TextBox>
                        <asp:TextBox ID="txtAddFileLinkCaption" runat="server"></asp:TextBox><br />
                        <asp:TextBox ID="txtDelFileLinkCaption" runat="server"></asp:TextBox>
                        <asp:TextBox ID="txtFileListCaption" runat="server"></asp:TextBox><br />
                        <asp:TextBox ID="txtDelFileLinkFromFileListCaption" runat="server"></asp:TextBox><br />
                        <asp:Button ID="btnSetStyle" runat="server" OnClick="btnSetStyle_Click" Text="Set style..."
                            Width="154px" /></td>
                    <td style="width: 280px; height: 12px">
                        &nbsp;&nbsp;
                    </td>
                    <td style="width: 123px; height: 12px">
                        <asp:HyperLink ID="hlUpload" runat="server" Width="42px">Get...</asp:HyperLink><br />
                        <asp:Button ID="btnUpload" runat="server" Text="Get..." Width="75px" OnClick="btnUpload_Click" /><asp:Button
                            ID="btnUploadURL" runat="server" Text="URL" Width="45px" OnClick="btnFileURL_Click" /></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        <asp:Button ID="btnClearAll" runat="server" Text="Clear All" Width="116px" OnClick="btnClearAll_Click"
                            Enabled="False" /></td>
                    <td colspan="2" style="height: 12px">
                        <asp:Label ID="lblClearInfo" runat="server" Font-Bold="True" Font-Size="8pt" ForeColor="Red"
                            Text="Note: This action delete all files, transfer log and clear usage storages info."
                            Width="398px"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 1px">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 9px; font-weight: bold; color: green;" align="center">
                        test file metadata</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        Results:</td>
                    <td colspan="2" style="height: 12px">
                        <asp:Label ID="lblMetaDataResult" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        FileId:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaFileId" runat="server" MaxLength="32" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        FS file_id</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        InternalId:</td>
                    <td style="width: 280px; height: 12px">
                        <asp:Label ID="lblMetaInternalId" runat="server"></asp:Label></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        OrganizationId:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaOrganizationId" runat="server" MaxLength="255" Width="269px"
                            EnableViewState="False"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        GUID</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        DepartmentId:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaDepartmentId" runat="server" MaxLength="255" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        GUID</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        FileName:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaFileName" runat="server" MaxLength="1024" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        FileSize:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaFileSize" runat="server" MaxLength="10" Width="269px">0</asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                        bytes</td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 16px">
                        FileComment:</td>
                    <td style="width: 280px; height: 16px">
                        <asp:TextBox ID="txtMetaFileComment" runat="server" MaxLength="200" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 16px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        LocalObjectId:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaLocalObjectId" runat="server" MaxLength="10" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        LocalObjectType:*</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaLocalObjectType" runat="server" MaxLength="50" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        UpdatedByUserId:</td>
                    <td style="width: 280px; height: 12px">
                        <asp:TextBox ID="txtMetaUpdatedByUserId" runat="server" MaxLength="10" Width="269px"></asp:TextBox></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        UpdatedTime:</td>
                    <td style="width: 280px; height: 12px">
                        <asp:Label ID="lblMetaUpdatedTime" runat="server"></asp:Label></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="right" class="text18" style="width: 115px; height: 12px">
                        IsDeleted:</td>
                    <td style="width: 280px; height: 12px">
                        <asp:CheckBox ID="chkDeleted" runat="server" /></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td align="left" class="text18" style="width: 115px; height: 12px">
                        <asp:Button ID="btnMetaDefault" runat="server" OnClick="btnMetaDefault_Click" Text="Default settings..." /></td>
                    <td style="width: 280px; height: 12px" align="center">
                        <asp:Button ID="btnLoadMetaData" runat="server" OnClick="btnLoadMetaData_Click" Text="LoadMetaData" />&nbsp;
                        &nbsp;<asp:Button ID="btnSaveMetaData" runat="server" OnClick="btnSaveMetaData_Click"
                            Text="SaveMetaData" /></td>
                    <td style="width: 123px; height: 12px">
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="height: 1px">
                    </td>
                </tr>
                <tr>
                </tr>
                <tr>
                </tr>
            </table>
            <br />
        </div>
    </form>
</body>
</html>
