<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReturnReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Reports.ReturnReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .style15 {
            width: 96%;
        }

        .style16 {
            width: 100%;
        }

        .style18 {
            width: 230px;
        }
    </style>

    <table style="width: 100%">
        <tr>
            <td class="line_at_bottom">Returns Report</td>
        </tr>

        <tr>
            <td class="line_at_bottom">
                <table>
                    <tr>
                        <td>Filter by</td>
                        <td>Value</td>
                        <td>Branch</td>
                        <td>&nbsp;</td>
                        <td>Instruction Type</td>
                        <td>&nbsp;</td>
<%--                        <td>Status&nbsp;
                    <asp:CheckBox ID="chkMultiple" runat="server" AutoPostBack="True"
                        Text="Show Multiple Status" />
                        </td>--%>
                        <td>From</td>
                        <td>To</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>

                    <tr>
                        <td>
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpBranchs" runat="server" Width="150px"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="drpInstructions" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>&nbsp;</td>
<%--                        <td>
                            <asp:DropDownList ID="drpStatus" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>--%>
                        <td>
                            <asp:TextBox ID="txtFrom" runat="server" Width="110px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="txtFrom" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtTo" runat="server" Width="110px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender2" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="txtTo" />
                        </td>
                        <td>
                            <asp:Button ID="btnView" runat="server" Text="View" Width="70px" OnClick="btnView_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnExport" runat="server" Text="Export"
                                Width="100px" OnClick="btnExport_Click" />
                        </td>
                    </tr>

                    <tr>
                        <td colspan="10">
                            <asp:CheckBoxList ID="chkBoxStatus" runat="server" RepeatColumns="5"
                                Visible="False">
                            </asp:CheckBoxList>
                        </td>
                        <td>&nbsp;</td>
                    </tr>

                </table>
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:GridView ID="dgvReturnsReport" runat="server"
                    AutoGenerateColumns="False" BackColor="White" BorderColor="#999999"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical"
                    Width="100%">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="reference" HeaderText="Reference"/>
                        <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                        <asp:BoundField DataField="amount" HeaderText="Amount" />
                        <asp:BoundField DataField="currency_name" HeaderText="Currency" />
                        <asp:BoundField DataField="document_status_name" HeaderText="Status" />
                        <asp:BoundField DataField="delivery_date" HeaderText="Delivery Date" />
                        <asp:BoundField DataField="allocated_to_name" HeaderText="Allocated User" />
                        <asp:BoundField DataField="allocated_date" HeaderText="Allocated Date" />
                        <asp:BoundField DataField="branch_proccessed_date" HeaderText="Branch Processed Date" />
                        <asp:BoundField DataField="branch_user" HeaderText="Branch User" />
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
            <td class="line_on_top">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="line_on_top">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>
