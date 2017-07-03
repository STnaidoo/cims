<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstructionTypeAllocations.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Instruction.InstructionTypeAllocations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .style15 {
            width: 100%;
        }

        .style16 {
            width: 77px;
        }

        .style17 {
            width: 212px;
        }

        .style18 {
            width: 68px;
        }

        .style19 {
            width: 230px;
        }

        .style20 {
            width: 492px;
        }

        .style21 {
            width: 79px;
        }

        .style22 {
            width: 164px;
        }

        .style23 {
            width: 131px;
        }

        .style24 {
            width: 31px;
        }

        .style25 {
            width: 206px;
        }
    </style>
    <table class="style15">
        <tr>
            <td class="h1FB">System Users Module</td>
        </tr>
        <tr>
            <td class="h1FB">
                <table class="style15">
                    <tr>
                        <td class="style16">Search by</td>
                        <td class="style17">
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="200px">
                            </asp:DropDownList>
                        </td>
                        <td class="style18">Value</td>
                        <td class="style25">
                            <asp:TextBox ID="txtValue" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td class="style24">Branch</td>
                        <td class="style19">
                            <asp:DropDownList ID="drpBranchs" runat="server" CssClass="drp">
                            </asp:DropDownList>
                        </td>
                        <td class="style21">
                            <asp:Button ID="btnSearch" OnClick="btnSearch_Click" runat="server" Text="Search" />
                        </td>
                        <td>
                            <asp:Button ID="btnAdd" runat="server" Text="Add New" Style="margin-left: 0px" Visible="False" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="ViewList" runat="server">

                        <table class="style15">
                            <tr>
                                <td class="h1FB">Users List</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="dgvNames" runat="server" AutoGenerateColumns="False"
                                        BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                                        CellPadding="3" GridLines="Vertical" Width="100%" OnRowCommand="dgvNames_RowCommand">
                                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                        <Columns>
                                            <asp:BoundField DataField="system_user_id">
                                                <ItemStyle Font-Size="0pt" Width="0px" />
                                            </asp:BoundField>
                                            <asp:CommandField ShowSelectButton="True" />
                                            <asp:BoundField DataField="system_user_login" HeaderText="User Name" />
                                            <asp:BoundField DataField="names" HeaderText="Name" />
                                            <asp:BoundField DataField="user_type" HeaderText="User Type" />
                                            <asp:BoundField DataField="system_user_email" HeaderText="User Email" />
                                            <asp:BoundField DataField="teamleader" HeaderText="Primary Team Leader" />
                                            <asp:BoundField DataField="branch_name" HeaderText="Branch" />
                                        </Columns>
                                        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                                        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                                        <AlternatingRowStyle BackColor="#DCDCDC" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </asp:View>

                    <asp:View ID="ViewDetails" runat="server">
                        <table class="style15">
                            <tr>
                                <td class="h1FB">Users Details</td>
                                <td class="h1FB">Instructions Allocations</td>
                            </tr>
                            <tr>
                                <td class="style20">
                                    <table style="width: 100%">
                                        <tr>
                                            <td class="style23">User Login
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_login" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">First Name
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_fname" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">Middle Name
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_mname" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">Last Name
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_lname" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                                <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Initials</td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_initials" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Title</td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_tittle" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                                <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Type</td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_type" runat="server" CssClass="drp"
                                                    AutoPostBack="True" Enabled="False" OnSelectedIndexChanged="drpsystem_user_type_SelectedIndexChanged">
                                                </asp:DropDownList>
                                                <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">Change Password</td>
                                            <td>
                                                <asp:CheckBox ID="Chkchange_password" runat="server" Enabled="False" />
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Email
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsystem_user_email" runat="server" CssClass="txt"
                                                    Enabled="False"></asp:TextBox>
                                                <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Active</td>
                                            <td>
                                                <asp:CheckBox ID="chksystem_user_active" runat="server" Enabled="False" />
                                                <asp:TextBox ID="txtsystem_user_id" runat="server" Visible="False" Width="18px"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Failed Login</td>
                                            <td>
                                                <asp:TextBox ID="txtFailedLoginCount" runat="server" CssClass="txt"
                                                    ReadOnly="True" Enabled="False"></asp:TextBox>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Branch</td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_branch" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                                <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Branch Code </td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_branch_code" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                                <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">Login Option</td>
                                            <td>
                                                <asp:DropDownList ID="drpLoginOptions" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>ldap</asp:ListItem>
                                                    <asp:ListItem>Non ldap</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Medium"
                                                    ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">User Status</td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_status" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="style23">Last Modified By</td>
                                            <td>
                                                <asp:DropDownList ID="drpsystem_user_modified_by" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="line_on_top" colspan="2">&nbsp; &nbsp;</td>
                                            <td class="line_on_top">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="vertical-align:top">
                                    <table class="style15">
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkEditInstructionsAlloscations" runat="server"
                                                    AutoPostBack="True" Font-Bold="True" Text="Edit Instructions Alloscations" oncheckedchanged="CheckBox1_CheckedChanged"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBoxList ID="chkBoxInstructions" runat="server" Enabled="False"
                                                    Width="100%" RepeatColumns="2">
                                                </asp:CheckBoxList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnSaveInstructionAllocations" runat="server" Enabled="False"
                                                    Text="Save Instructions Allocation" onclick="btnSaveInstructionAllocations_Click"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="h1FB">Team Leaders</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="dgvUsers" runat="server" AutoGenerateColumns="False"
                                                    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                                                    CellPadding="3" GridLines="Vertical" Width="75%" OnRowCommand="dgvUsers_RowCommand">
                                                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                                    <Columns>
                                                        <asp:BoundField DataField="teamleader" HeaderText="Team Leader" />
                                                        <asp:CommandField SelectText="Remove" ShowSelectButton="True" />
                                                        <asp:BoundField DataField="user_team_leaders_id">
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
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
        <tr>
            <td>
                <table class="style15">
                    <tr>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
