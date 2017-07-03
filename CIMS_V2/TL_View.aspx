<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="TL_View.aspx.cs" Inherits="CIMS_V2.Unknown_Views.TL_View" %>

<%@ Register Assembly="PdfViewer" Namespace="PdfViewer" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .style18 {
            height: 23px;
        }

        .style19 {
            width: 2px;
        }

        .style20 {
            width: 100%;
        }

        .style21 {
            width: 1011px;
        }

        .style22 {
            width: 350px;
        }
    </style>

    <link href="CSS/Q.css" rel="stylesheet" type="text/css" />

    <table class="fill_width">


        <tr>
            <td class="line_at_bottom" colspan="2">Users</td>
        </tr>


        <tr>
            <td colspan="2">
                <table class="style20">
                    <tr>
                        <td class="style22">
                            <asp:GridView ID="dgvUsers" runat="server" AutoGenerateColumns="False"
                                BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                                CellPadding="3" GridLines="Vertical" Width="550px">
                                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                <Columns>
                                    <asp:BoundField DataField="system_user_id">
                                        <ItemStyle Font-Size="0pt" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="officer" HeaderText="Officer" />
                                    <asp:TemplateField HeaderText="Allocate / Deallocate">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkAllocate" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="total_pending" HeaderText="Total Pending" />
                                    <asp:BoundField DataField="total_processed" HeaderText="Total Processed" />
                                    <asp:BoundField DataField="total" HeaderText="Total" />
                                </Columns>
                                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="#DCDCDC" />
                            </asp:GridView>
                        </td>
                        <td>
                            <table class="style20">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnAllocate" runat="server" Text="Allocate To Selected Users" OnClick="btnAllocate_Click"
                                            Width="280px" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkAllocateToMany" runat="server" Text="Allocate To Many" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnAllocate0" runat="server"
                                            Text="Deallocate From Selected Users" Width="280px" OnClick="btnAllocate0_Click1" />
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnDeallocateSelected" runat="server" OnClick="btnDeallocateSelected_Click"
                                            Text="Deallocate Selected Instructions" Width="280px" />
                                    </td>
                                    <td>&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td class="line_at_bottom" colspan="2">Instructions</td>
        </tr>

        <tr>
            <td class="line_at_bottom" colspan="2">
                <table class="style21">
                    <tr>
                        <td class="style25">&nbsp;</td>
                        <td class="style23">Search by</td>
                        <td class="style24">&nbsp;</td>
                        <td class="style19">&nbsp;</td>
                        <td class="style24">Instruction&nbsp; Type</td>
                        <td class="style24">&nbsp;</td>
                        <td class="style24">Processing Status </td>
                        <td class="style24">Allocation Status</td>
                        <td class="style24">Allocated To</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="style25">&nbsp;</td>
                        <td class="style23">
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td class="style24">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="txt"></asp:TextBox>
                        </td>
                        <td class="style19">&nbsp;</td>
                        <td class="style24">
                            <asp:DropDownList ID="drpTransType" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td class="style24">&nbsp;</td>
                        <td class="style24">
                            <asp:DropDownList ID="drpProcessingStatus" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td class="style24">
                            <asp:DropDownList ID="drpAllocationStatus" runat="server" Width="140px">
                                <asp:ListItem>All</asp:ListItem>
                                <asp:ListItem>Allocated</asp:ListItem>
                                <asp:ListItem>Not Allocated</asp:ListItem>
                            </asp:DropDownList>

                        </td>
                        <td class="style24">
                            <asp:DropDownList ID="drpAllocatedTo" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td></td>
                        <td>
                            <asp:Button ID="btnView" OnClick="btnView_Click" runat="server" Text="View" Width="80px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td>
                <asp:CheckBox ID="chkSelectAll" runat="server" Text="Select All To Allocate" OnCheckedChanged="chkSelectAll_CheckedChange"
                    AutoPostBack="True" />
            </td>
            <td class="align_right">
                <asp:CheckBox ID="chkSelectToDeallocate" runat="server" AutoPostBack="True" OnCheckedChanged="chkSelectToDeallocate_CheckedChange"
                    Text="Select All To Deallocate" />
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:GridView ID="dgvInstructions" runat="server" AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" GridLines="Vertical" Width="100%">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelectInstruction" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />
                        <asp:BoundField DataField="Amount" DataFormatString="{0:n}"
                            HeaderText="Amount" />
                        <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                        <asp:BoundField DataField="document_status_name"
                            HeaderText="Processing Status" />
                        <asp:BoundField DataField="allocated_to_name" HeaderText="Allocated To" />
                        <asp:BoundField DataField="branch_proccessed_date"
                            HeaderText="Submitted Date" />
                        <asp:BoundField DataField="branch_user" HeaderText="Branch" />
                        <asp:BoundField DataField="ftroa_user" HeaderText="FTRO (Maker)" />
                        <asp:BoundField DataField="ftrob_user" HeaderText="FTRO (Checker)" />
                        <asp:BoundField DataField="processor_user" HeaderText="Processor" />
                        <asp:BoundField DataField="Instruction_ID">
                            <ItemStyle Font-Size="0pt" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Client_ID">
                            <ItemStyle Font-Size="0pt" />
                        </asp:BoundField>
                        <asp:BoundField DataField="File_Name" HeaderText="File Name" Visible="False" />
                        <asp:TemplateField HeaderText="Deallocate">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkDeallocate" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

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
            <td class="style18" colspan="2">&nbsp;</td>
        </tr>
    </table>

</asp:Content>