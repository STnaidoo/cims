<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Action_Setup.aspx.cs" Inherits="CIMS_V2.Unknown_Views.Action_Setup" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <link href="../Content/bootstrap.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style15
        {
            width: 100%;
        }
    .style16
    {
        width: 186px;
    }
    .style17
    {
        width: 186px;
        height: 14px;
    }
    .style18
    {
        height: 14px;
    }
    .style19
    {
        height: 30px;
    }
        .style20
        {
            width: 186px;
            height: 25px;
        }
        .style21
        {
            height: 25px;
        }
    </style>

    <%--Info needed to query the type of action to be done added here--%>


    <table class="style15">
        <tr>
            <td class="h1FB">
                Action Setup</td>
        </tr>
        <tr>
            <td class="h1FB">
                                    <table class="style15" __designer:mapid="143">
                                        <tr __designer:mapid="144">
                                            <td class="style16" __designer:mapid="145">
                                                Search by</td>
                                            <td class="style17" __designer:mapid="146">
                                                <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px" CssClass="form-control" Height="30px">
                                                </asp:DropDownList>
                                            </td>
<%--                                            <td class="style18" __designer:mapid="148">
                                                Value</td>
                                            <td class="style25" __designer:mapid="149">
                                                <asp:TextBox ID="txtValue" runat="server" Width="150px" CssClass="form-control" Height="30px"></asp:TextBox>
                                            </td>--%>
                                            <td class="style24" __designer:mapid="149">
                                                User Type</td>
                                            <td class="style19" __designer:mapid="149">
                        <asp:DropDownList ID="drpUserTypeSearch" runat="server" CssClass="form-control" Height="30px" Width="250px">
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
            <td class="section_navy_blue_header_small">
                &nbsp;</td>
        </tr>
        <tr>
            <td>

<%--Form to fill in action details --%>



              <table width="100%">
    <tr>
     <td class="style16">
         User Type Who Can Action
     </td>
     <td>
         <asp:DropDownList ID="drpdocument_status_user_type_who_can_action" 
             runat="server"  CssClass="form-control" Height="30px" Width="300px"></asp:DropDownList>
         <asp:TextBox ID="txtdocument_status_id" runat="server" Visible="False" CssClass="form-control" Height="30px" Width="250px"></asp:TextBox>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Action Text</td>
     <td>
         <asp:TextBox ID="txtdocument_status_action" runat="server"  CssClass="form-control" Height="30px" Width="330px"></asp:TextBox>
     </td>
    </tr>
    <tr>
     <td class="style20">
         Action Resultant Stage/Status
     </td>
     <td class="style21">
         <asp:DropDownList ID="drpdocument_status_stage" runat="server"  CssClass="form-control" Height="30px" Width="300px"></asp:DropDownList>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Action Resultant Text</td>
     <td>
         <asp:TextBox ID="txtdocument_status" runat="server"  CssClass="form-control" Height="30px" Width="300px"></asp:TextBox>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Is Action An Exception</td>
     <td>
         <asp:DropDownList ID="drpexception" runat="server" CssClass="form-control" Height="30px" Width="300px">
             <asp:ListItem Value="-1">.</asp:ListItem>
             <asp:ListItem Value="0">No</asp:ListItem>
             <asp:ListItem Value="1">Yes</asp:ListItem>
         </asp:DropDownList>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Is Document Held
     </td>
     <td>
         <asp:DropDownList ID="drpis_document_held" runat="server"  CssClass="form-control" Height="30px" Width="300px">
             <asp:ListItem Value="-1">.</asp:ListItem>
             <asp:ListItem Value="0">No</asp:ListItem>
             <asp:ListItem Value="1">Yes</asp:ListItem>
         </asp:DropDownList>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Is Action A Referral
     </td>
     <td>
         <asp:DropDownList ID="drpis_referral" runat="server" CssClass="form-control" Height="30px" Width="300px">
             <asp:ListItem Value="-1">.</asp:ListItem>
             <asp:ListItem Value="0">No</asp:ListItem>
             <asp:ListItem Value="1">Yes</asp:ListItem>
         </asp:DropDownList>
     </td>
    </tr>
    <tr>
     <td class="style16">
         Foward Back To After Refferal</td>
     <td>
         <asp:DropDownList ID="drpfoward_back_to_after_reversal" runat="server"  
             CssClass="form-control" Height="30px" Width="300px"></asp:DropDownList>
     </td>
    </tr>
    <tr>
        <td>
            For specific instruction type
        </td>
        <td>
            <asp:CheckBox ID="chkSpecificInstructionType" runat="server" AutoPostBack="true" OnCheckedChanged="chkSpecificInstructionType_CheckedChanged"/>
        </td>
    </tr>
    <tr>
        <td>
            Instruction Type
        </td>
        <td>
            <asp:DropDownList ID="drpInstructionTypes" runat="server" Enabled="false" CssClass="form-control" Height="30px" Width="300px">

            </asp:DropDownList>
        </td>
    </tr>

    <tr>
     <td class="style16">
         Include Amount In Checking
     </td>
     <td>
         <asp:CheckBox ID="chknclude_amount_in_checking1" runat="server" />
     </td>
    </tr>
    <tr>
     <td class="style16">
         Must Comment</td>
     <td>
         <asp:CheckBox ID="chkmust_comment" runat="server" />
     </td>
    </tr>
</table>
                
<%--    Grid view table starts here            --%>
                
            </td>
        </tr>
        <tr>
            <td class="style19">
                <asp:Button ID="btnSave" runat="server" Text="Save Action" onclick="btnSave_Click"/>
            </td>
        </tr>
        <tr>
            <td class="section_navy_blue_header_small">
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="dgvActions" runat="server" AutoGenerateColumns="False" 
                    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                    CellPadding="3" GridLines="Vertical" Width="100%" OnRowCommand="dgvActions_RowCommand">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <Columns>
                        <asp:BoundField DataField="document_status_id">
                            <ItemStyle Font-Size="0pt" Width="0px" />
                        </asp:BoundField>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField DataField="user_type" HeaderText="User Type Who Can Action" />
                        <asp:BoundField DataField="document_status_action" HeaderText="Action Text" />
                        <asp:BoundField DataField="document_status" 
                            HeaderText="Resultant Action Text" />
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



</asp:Content>
