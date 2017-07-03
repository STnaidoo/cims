﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransactionReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Reports.TransactionReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table style="width: 100%">
        
        <tr>
            <td class="line_at_bottom">Transaction Listing Report</td>
        </tr>

        <tr>
            <td class="line_at_bottom">
                <table class="style15">
                    <tr>
                        <td>Filter by</td>
                        <td>&nbsp;</td>

                        <td>Value</td>
                        <td>&nbsp;</td>

                        <td>Instruction Type</td>
                        <td>&nbsp;</td>
                        <td>Status</td>
                        <td>&nbsp;</td>
                        <td>From</td>
                        <td>&nbsp;</td>
                        <td>To</td>

                    </tr>

                    <tr>
                        <td>
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="120px">
                            </asp:DropDownList>
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="120px"></asp:TextBox>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="drpInstructions" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:DropDownList ID="drpStatus" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtFrom" runat="server" Width="100px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="txtFrom" />
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:TextBox ID="txtTo" runat="server" Width="100px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender2" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="txtTo" />
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnView" runat="server" Text="View" Width="100px" OnClick="btnView_Click" />
                        </td>
                                                <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnExport" runat="server" Text="Export" Width="100px" OnClick="btnExport_Click" />
                        </td>
                    </tr>
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
                        <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                        <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />
                        <asp:BoundField DataField="amount" HeaderText="Value"
                            DataFormatString="{0:n}" />
                        <asp:BoundField DataField="currency_name" HeaderText="Currency" />
                        <asp:BoundField DataField="inserted_date" HeaderText="Date"
                            DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="Branch_duration"
                            HeaderText="B. Dur (In Min)" />
                        <asp:BoundField DataField="FTRO_duration" HeaderText="F. Dur (In Min)" />
                        <asp:BoundField DataField="Processor_duration" HeaderText="P. Dur (In Min)" />
                        <asp:BoundField DataField="document_status_name" HeaderText="Status" />
                        <asp:BoundField DataField="comments" HeaderText="Branch Comments" />
                        <asp:BoundField DataField="ftroa_comments" HeaderText="FTRO Comments" />
                        <asp:BoundField DataField="processor_comments"
                            HeaderText="Processor Comments" />
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
    </table>
</asp:Content>