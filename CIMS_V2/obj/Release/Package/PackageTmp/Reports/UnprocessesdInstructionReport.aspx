<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnprocessesdInstructionReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Reports.UnprocessesdInstructionReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width: 100%">
        <tr>
            <td class="h1FB">Unprocessed Instructions</td>
        </tr>
        <tr>
            <td class="h1FB">
                <table class="style15">
                    <tr>
                        <td class="style16">&nbsp;</td>
                        <td class="style17">User Type
                        </td>
                                                <td>&nbsp;</td>
                        <td class="style18">
                            <asp:DropDownList ID="drpUserTypes" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                                                <td>&nbsp;</td>
                        <td class="style25"></td>
                        <td class="style24">Report Type
                        </td>
                                                <td>&nbsp;</td>
                        <td class="style19">
                            <asp:DropDownList ID="drpReportType" runat="server" Width="250px">
                                <asp:ListItem>By User Type and Instruction Type</asp:ListItem>
                                <asp:ListItem>By User Type</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                                                <td>&nbsp;</td>     
                        <td class="style21">
                            <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" Width="100px" />
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnExport" runat="server" Text="Export" Width="100px" OnClick="btnExport_Click" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="dgvUnprocessedByUserType" runat="server" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#999999"
                    BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" GridLines="Vertical" Width="100%" Visible="False">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="user_type" HeaderText="User Type">
                            <ItemStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="number_of_instructions"
                            HeaderText="No of Instructions" />
                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                </asp:GridView>

        </tr>
        <tr>
            <td>

                <asp:GridView ID="dgvUnprocessedByUserTypeandInstructionType" runat="server" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#999999"
                    BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" GridLines="Vertical" Width="100%"
                    Visible="False">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="user_type" HeaderText="User Type">
                            <ItemStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="instruction_type" HeaderText="Instruction Type">
                            <ItemStyle Width="200px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="number_of_instructions"
                            HeaderText="No of Instructions" />
                    </Columns>
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                </asp:GridView>

        </tr>
    </table>
</asp:Content>
