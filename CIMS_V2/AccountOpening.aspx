<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AccountOpening.aspx.cs" Inherits="CIMS_V2.AccountOpening" Title="Instructions | Open Account" %>

<%@ Register Assembly="PdfViewer" Namespace="PdfViewer" TagPrefix="cc1" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .style15 {
            width: 100%;
        }

        .style16 {
            width: 228px;
        }

        .style17 {
            width: 64px;
        }
    </style>
    <div class="upload_clients" style="padding-top: 50px">
        <table class="style15">
            <tr>
                <td>Account Opening</td>
            </tr>
            <tr>
                <td>
                    <table>

                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblBranches">Branch</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="drpBranches" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:FileUpload runat="server" ID="pdfUpload" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnAttach" OnClick="btnAttach_Click" Text="Attach" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblUploadedPDF">Uploaded PDF</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtUploadedPDF" Width="100%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblReference">Reference</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtReference" ReadOnly="true" Width="100%"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblAction">Action</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="drpDocumentStatus" Width="150px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                               <asp:TextBox ID="txtComments" runat="server" CssClass="txt" Rows="3" TextMode="MultiLine"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            <asp:Button runat="server" ID="btnSubmit" OnClick="btnSubmit_Click" text="Submit"/>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                   <cc1:ShowPdf ID="pdfViewer" runat="server" FilePath="Images/logo.png" Height="650px" Width="567px" />
                </td>
            </tr>
            <tr>
                <td>

                </td>
            </tr>
        </table>
    </div>
</asp:Content>
