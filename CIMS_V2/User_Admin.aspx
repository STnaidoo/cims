<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="User_Admin.aspx.cs" Inherits="CIMS_V2.Unknown_Views.User_Admin" %>

<asp:content id="Content1" contentplaceholderid="MainContent" runat="server">
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
<a href="User_Admin.aspx">User_Admin.aspx</a>
        .style25 {
            width: 206px;
        }
    </style>

    <table class="style15">
        <tr>
            <td class="h1FB">
                                    System Users Module</td>
        </tr>
        <tr>
            <td class="h1FB">
                                    <table class="style15" __designer:mapid="143">
                                        <tr __designer:mapid="144">
                                            <td class="style16" __designer:mapid="145">
                                                Search by</td>
                                            <td class="style17" __designer:mapid="146">
                                                <asp:DropDownList ID="drpSearchBy" CssClass="form-control" runat="server" Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="style18" __designer:mapid="148">
                                                Value</td>
                                            <td class="style25" __designer:mapid="149">
                                                <asp:TextBox ID="txtValue" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
                                            </td>
                                            <td class="style24" __designer:mapid="149">
                                                Branch</td>
                                            <td class="style19" __designer:mapid="149">
                                                <asp:DropDownList ID="drpBranchs" CssClass="form-control" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td __designer:mapid="14b" class="style21">
                                                <asp:Button ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click"/>
                                            </td>
                                            <td __designer:mapid="14d">
                                                <asp:Button ID="btnAdd" runat="server" Text="Add New" style="margin-left: 0px" onclick="btnAdd_Click"/>
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
                                <td class="h1FB">
                                    Users List</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:GridView ID="dgvNames" runat="server" AutoGenerateColumns="False" 
                                        BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                                        CellPadding="3" GridLines="Vertical" Width="100%" OnRowCommand="dgvNames_RowCommand">
                                        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                        <Columns>
                                            <asp:BoundField DataField="system_user_id" >
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
     <td class="h1FB">
         Users Details</td>
        <td class="h1FB">
            Instructions Allocations</td>
    </tr>
    <tr>
     <td class="style20">
         <table style="width: 100%">
             <tr>
                 <td class="style23">
                     User Login
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_login" runat="server" CssClass="txt"></asp:TextBox>
                     <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     First Name
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_fname" runat="server" CssClass="txt"></asp:TextBox>
                     <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
<%--                 <td class="style23">
                     Middle Name
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_mname" runat="server" CssClass="txt" ReadOnly="true" Visible="false"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td> --%>
             </tr>
             <tr>
                 <td class="style23">
                     Last Name
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_lname" runat="server" CssClass="txt"></asp:TextBox>
                     <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
<%--                 <td class="style23">
                     User Initials</td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_initials" runat="server" CssClass="txt" ReadOnly="true" Visible="false"></asp:TextBox>

                 </td>
                 <td>
                     &nbsp;</td> --%>
             </tr>
             <tr>
                 <td class="style23">
                     User Title</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_tittle" runat="server" CssClass="drp">
                     </asp:DropDownList>
                     <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Type</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_type" runat="server" CssClass="drp" 
                         AutoPostBack="True" OnSelectedIndexChanged="drpsystem_user_type_SelectedIndexChanged">
                     </asp:DropDownList>
                     <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
        
             <tr>
                 <td class="style23">
                     Type / Reset Password</td>
                 <td>
                     <asp:CheckBox ID="chkResetPassword" runat="server" AutoPostBack="True" oncheckedchanged="chkResetPassword_CheckedChanged"/>
                     <asp:TextBox ID="txtcan_submit" runat="server" CssClass="txt" Visible="False" 
                         Width="24px"></asp:TextBox>
                     <asp:TextBox ID="txtcan_create" runat="server" CssClass="txt" Visible="False" 
                         Width="24px"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Password
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_password" runat="server" CssClass="txt" 
                         ReadOnly="True" TextMode="Password"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     Change Password</td>
                 <td>
                     <asp:CheckBox ID="Chkchange_password" runat="server" ReadOnly="true" />
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Email
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_email" runat="server" CssClass="txt"></asp:TextBox>
                     <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Active</td>
                 <td>
                     <asp:CheckBox ID="chksystem_user_active" runat="server" ReadOnly="true"/>
                     <asp:TextBox ID="txtsystem_user_id" runat="server" Visible="False" Width="18px"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
<%--                 <td class="style23">
                     User Failed Login</td>
                 <td>--%>
                     <asp:TextBox ID="txtFailedLoginCount" runat="server" CssClass="txt" 
                         ReadOnly="True" Visible="false"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
<%--                 <td class="style23">
                     User ID Number
                 </td>
                 <td>
                    <asp:TextBox ID="txtsystem_user_id_number" runat="server" CssClass="txt" ReadOnly="true" Visible="false"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                <td class="style23">
                     User Mobile
                 </td>
                 <td>
                     <asp:TextBox ID="txtsystem_user_mobile_number" runat="server" CssClass="txt" ReadOnly="true" Visible="false"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
           
                  <tr>
               <td class="style23">
                     User Landline
                 </td>
                 <td>
                  <asp:TextBox ID="txtsystem_user_land_line" runat="server" CssClass="txt" ReadOnly="true" Visible="false"></asp:TextBox>
                 </td>
                 <td>
                     &nbsp;</td>--%>
             </tr>
             <tr>
                 <td class="style23">
                     Is Team Leader </td>
                 <td>
                     <asp:DropDownList ID="drpis_team_leader" runat="server" 
                         CssClass="drp" AutoPostBack="True" OnSelectedIndexChanged="drpis_team_leader_SelectedIndexChanged">
                         <asp:ListItem></asp:ListItem>
                         <asp:ListItem Value="0">No</asp:ListItem>
                         <asp:ListItem Value="1">Yes</asp:ListItem>
                     </asp:DropDownList>
                     <asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     Is Team Leader To</td>
                 <td>
                     <asp:DropDownList ID="drpuser_can_allocate_what_document_status" runat="server" 
                         CssClass="drp">
                     </asp:DropDownList>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
               <tr>
                 <td class="style23">
                     Primary Team Leader</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_tl_1" runat="server" CssClass="drp">
                         <asp:ListItem></asp:ListItem>
                         <asp:ListItem Value="0">No</asp:ListItem>
                         <asp:ListItem Value="1">Yes</asp:ListItem>
                     </asp:DropDownList>
                     <asp:Label ID="Label14" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     Primary Manager</td>
                 <td>
                     <asp:DropDownList ID="drpManager" runat="server" CssClass="drp">
                         <asp:ListItem></asp:ListItem>
                         <asp:ListItem Value="0">No</asp:ListItem>
                         <asp:ListItem Value="1">Yes</asp:ListItem>
                     </asp:DropDownList>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Branch</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_branch" runat="server" CssClass="drp" 
                         AutoPostBack="True" OnSelectedIndexChanged="drpsystem_user_branch_SelectedIndexChanged">
                     </asp:DropDownList>
                     <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
<%--                 <td class="style23">
                     User Branch Code </td>--%>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_branch_code" runat="server" CssClass="drp" Visible="false" Enabled="false">
                     </asp:DropDownList>

                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     Login Option</td>
                 <td>
                     <asp:DropDownList ID="drpLoginOptions" runat="server" CssClass="drp">
                         <asp:ListItem></asp:ListItem>
                         <asp:ListItem>ldap</asp:ListItem>
                         <asp:ListItem>Non ldap</asp:ListItem>
                     </asp:DropDownList>
                     <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Medium" 
                         ForeColor="Red" Text="*"></asp:Label>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     User Status</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_status" runat="server" CssClass="drp" 
                         Enabled="False">
                     </asp:DropDownList>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="style23">
                     Last Modified By</td>
                 <td>
                     <asp:DropDownList ID="drpsystem_user_modified_by" runat="server" CssClass="drp" 
                         Enabled="False">
                     </asp:DropDownList>
                 </td>
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="line_on_top" colspan="2">
                     <asp:Button ID="btnSave" runat="server" Text="Add User" onclick="btnSave_Click"/>
                     &nbsp;
                     <asp:Button ID="btnApprove" runat="server" Text="Approve" onclick="btnApprove_Click"/>
                     &nbsp;<asp:Button ID="ResetFailedLogin" runat="server" Text="Reset Failed Logins" onclick="ResetFailedLogin_Click"/>
                     &nbsp;
                    <asp:Button ID="btnActivation" runat="server" Text="Change Active Status" onclick="btnActivation_Click"/>

                 </td>
        
  
                 <td class="line_on_top">
                     &nbsp;</td>
             </tr>
         </table>
     </td>
     <td valign="top">
         <table class="style15">
             <tr>
                 <td>
                     <asp:CheckBox ID="chkEditInstructionsAlloscations" runat="server" 
                         AutoPostBack="True" Font-Bold="True" Text="Edit Instructions Allocations" oncheckedchanged="CheckBox1_CheckedChanged"/>
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
                     <asp:CheckBoxList ID="CheckBoxInstructions2" runat="server" Enabled="False" 
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
                 <td>
                     &nbsp;</td>
             </tr>
             <tr>
                 <td class="h1FB">
                     Team Leaders</td>
             </tr>
             <tr>
                 <td>
                     <table class="style15">
                         <tr>
                             <td class="style22">
                                 <asp:DropDownList ID="drpTeamAddLeader" runat="server" AutoPostBack="True" 
                                     Width="250px" OnSelectedIndexChanged="drpTeamAddLeader_SelectedIndexChanged">
                                 </asp:DropDownList>
                             </td>
                             <td>
                                 <asp:Button ID="btnAddTL" runat="server" Text="Add Team Leader" onclick="btnAddTL_Click"/>
                             </td>
                         </tr>
                     </table>
                 </td>
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
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
              
                
                </td>
        </tr>
    </table>
</asp:content>
