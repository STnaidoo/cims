<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EndOfDayReport.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Reports.EndOfDayReport" %>

    <%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .style15
        {
            width: 96%;
        }
        .style16
        {
            width: 100%;
        }
        .style17
        {
            width: 117px;
        }
        .style18
        {
            width: 230px;
        }
        .auto-style2 {
            height: 43px;
        }
    </style>
    <table style="width:100%">
        <tr>
            <td class="line_at_bottom">End Of Day Report</td>
        </tr>

        <tr>
            <td class="line_at_bottom">
                <table class="style15">
                    <tr>
                        <td class="auto-style2">Filter by</td>
                        <td class="auto-style2">Value</td>
                        <td class="auto-style2">Branch</td>
                        <td class="auto-style2"></td>
                        <td class="auto-style2">Instruction Type</td>
                        <td class="auto-style2"></td>
<%--                        <td class="auto-style2">Status&nbsp;
                        <asp:CheckBox ID="chkMultiple" runat="server" AutoPostBack="True"
                            Text="Show Multiple Status" OnCheckedChanged="chkMultiple_CheckedChanged" />
                        </td>--%>
                        <td class="auto-style2">From</td>
                        <td class="auto-style2">To</td>
                        <td class="auto-style2"></td>
                    </tr>

                    <tr>
                        <td>
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" Width="150px" MaxLength="20"></asp:TextBox>
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
                        <asp:BoundField DataField="instruction_id">
                            <ItemStyle Font-Size="0pt" Width="0px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="branch_manual_approved_by">
                            <ItemStyle Font-Size="0pt" Width="0px" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Verified">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkVerified" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="reference"
                            HeaderText="Reference" />
<%--                        <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />--%>
                        <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                        <asp:BoundField DataField="amount" HeaderText="Value"
                            DataFormatString="{0:n}" />
                        <asp:BoundField DataField="currency_name" HeaderText="Currency" />
                        <asp:BoundField DataField="branch_proccessed_date" HeaderText="Date Submitted"
                            DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="branch_user" HeaderText="Branch User" />
                        <asp:BoundField DataField="document_status_name" HeaderText="Status" />
                        <asp:BoundField DataField="comments" HeaderText="Branch Comments"
                            Visible="False" />
                        <asp:BoundField DataField="ftroa_comments" HeaderText="FTRO Comments"
                            Visible="False" />
                        <asp:BoundField DataField="processor_comments"
                            HeaderText="Processor Comments" Visible="False" />
                        <asp:BoundField DataField="document_status_stage">
                            <ItemStyle Font-Size="0pt" Width="0px" />
                        </asp:BoundField>
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
            <td class="line_on_top">
                <table class="style16">
                    <tr>
                        <td class="style17">
                            <asp:Button ID="btnCheckAll" runat="server" Text="Check All" Width="140px" OnClick="btnCheckAll_Click" />
                        </td>
                        <td class="style18">
                            <asp:Button ID="btnUncheck" runat="server" Style="margin-left: 43px"
                                Text="Uncheck All" Width="140px" OnClick="btnUncheck_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnApprove" runat="server" Text="Approve Selected"
                                Enabled="False" OnClick="btnApprove_Click" />
                        </td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                        <td></td>
<%--                        <td style="float:right" class="align_right">
                            <asp:Button ID="btnPrint" runat="server" Text="Print" Width="140px" OnClick="btnPrint_Click" />
                        </td>--%>
                    </tr>
                </table>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td class="line_on_top">&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>
