<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Error.aspx.cs" Inherits="CIMS_V2.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dnxalignCentre" style="padding-top:100px">
        <table style="text-align: center; margin: 0 auto; border-collapse: separate; border-spacing: 0px; padding: 0px; border: 0px">
            <tr style="vertical-align: top;">
                <td style="width: 100%">
                    <table style="text-align: center; margin: 0 auto; border-collapse: separate; border-spacing: 0px; padding: 0px; border: 0px">
                        <tr style="vertical-align: top;">
                            <td style="width: 100%">
                                <asp:Panel ID="header_here" runat="server"></asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="vertical-align: top">
                <td style="width: 100%">
                    <table class="blue_border" style="align-content: center">
                        <tr>
                            <td class="align_center" style="font-size: x-large; font-weight: bold">SYSTEM ERROR</td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="align_center" style="font-size: large; font-weight: bold">Oops! an unexpected error has occurred in the system. Please try again and if the error 
                                persists then inform the technical team.</td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="style2"></td>
                        </tr>
                        <tr>
                            <td class="align_center">
                                <asp:LinkButton ID="LinkBackToLogin" runat="server" Font-Bold="True">Back To Login</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr style="vertical-align: top;">
                <td style="width: 100%">
                    <table style="text-align: center; margin: 0 auto; border-collapse: separate; border-spacing: 0px; padding: 0px; border: 0px">
                        <tr style="vertical-align: top; text-align: center">
                            <td style="width: 100%; text-align: center">
                                <asp:Panel ID="footer_here" runat="server"></asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
