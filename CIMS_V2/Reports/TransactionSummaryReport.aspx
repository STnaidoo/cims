<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionSummaryReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Reports.TransactionSummaryReport" %>

   
    <%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width:100%">
        <tr>
            <td class="line_at_bottom">
            Transaction Summary Report
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>From</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtFrom" runat="server" Width="110px"></asp:TextBox>
                             <asp:CalendarExtender ID="CalendarExtender1" runat="server"
                                 Format="dd-MMM-yyyy" TargetControlID="txtFrom" />
                        </td>

                        <td>To</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtTo" runat="server" Width="110px"></asp:TextBox>
                             <asp:CalendarExtender ID="CalendarExtender2" runat="server" 
                                 Format="dd-MMM-yyyy" TargetControlID="txtTo" />
                        </td>


                        <td>Status</td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="drpStatus" runat="server" Width="250px">
                            </asp:DropDownList>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnView" runat="server" Text="View" Width="100px" OnClick="btnView_Click" />
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnExport" runat="server" Text="Export"
                                Width="100px" OnClick="btnExport_Click" />
                        </td>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="dgvTransactionsReport" runat="server"
                    AutoGenerateColumns="False" BackColor="White" BorderColor="#999999"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical"
                    Width="100%">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="instruction_type" HeaderText="Instruction Type" />
                        <asp:BoundField DataField="volume" HeaderText="Volume" />
                        <asp:BoundField DataField="amount" HeaderText="Amount"/>

                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                </asp:GridView>
            </td>
        </tr>
    <tr>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
    </table>
</asp:Content>
