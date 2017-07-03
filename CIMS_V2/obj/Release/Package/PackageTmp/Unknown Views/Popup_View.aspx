<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Popup_View.aspx.cs" Inherits="CIMS_V2.Unknown_Views.Popup_View" %>

<%@ Register assembly="PdfViewer" namespace="PdfViewer" tagprefix="cc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Popup View</title>
    <link href="CSS/Q.css" rel="stylesheet" type="text/css" />
</head>
<body>   
       <form id="form1" runat="server">                  
       <asp:ScriptManager ID="ScriptManager1" runat="server">
       </asp:ScriptManager>
    <table class="fill_width" width="100%">
    <tr>
        <td class="line_at_bottom">
            &nbsp;</td>
    </tr>

    <tr>
        <td class="style18">
            <asp:MultiView ID="MultiView1" runat="server" >
                <asp:View ID="ViewCustomerList" runat="server">
                    <table class="style21" width="100%">
                        <tr>
                            <td class="style19">
                                Filte Option</td>
                            <td>
                                Filter Value</td>
                            <td class="style24">
                                Instruction Type</td>
                            <td class="style24">
                                Status&nbsp;
                                <asp:CheckBox ID="chkMultiple" runat="server" AutoPostBack="True" OnCheckedChanged="chkMultiple_CheckedChanged"
                                    Text="Show Multiple" />
                            </td>
                            <td class="style24">
                                <asp:Label ID="lblFrom" runat="server" Text="From"></asp:Label>
                            </td>
                            <td class="style24">
                                &nbsp;</td>
                            <td class="style24">
                                <asp:Label ID="lblTo" runat="server" Text="To"></asp:Label>
                            </td>
                            <td class="style24">
                                &nbsp;</td>
                            <td class="style24">
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td class="style19">
                                <asp:DropDownList ID="drpSearchBy" runat="server" Enabled="False" Width="150px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="txt" Enabled="False" 
                                    Width="150px"></asp:TextBox>
                            </td>
                            <td class="style24">
                                <asp:DropDownList ID="drpInstructions0" runat="server" Enabled="False" 
                                    Width="150px">
                                </asp:DropDownList>
                            </td>
                            <td class="style24">
                                <asp:DropDownList ID="drpStatus" runat="server" Enabled="False" Width="150px">
                                </asp:DropDownList>
                            </td>
                            <td class="style24">
                                <asp:TextBox ID="dtmFrom" runat="server" Enabled="False" Width="100px"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" 
                                    Format="dd-MMM-yyyy" TargetControlID="dtmFrom" />
                            </td>
                            <td class="style24">
                                &nbsp;</td>
                            <td class="style24">
                                <asp:TextBox ID="dtmTo" runat="server" Enabled="False" Width="100px"></asp:TextBox>
                                <asp:CalendarExtender ID="CalendarExtender2" runat="server" 
                                    Format="dd-MMM-yyyy" TargetControlID="dtmTo" />
                            </td>
                            <td class="style24">
                                <asp:Button ID="btnView" runat="server" Text="View" Width="96px" OnClick="btnView_Click1" />
                            </td>
                            <td class="style24">
                                <asp:Button ID="btnExport" runat="server" Text="Export To Excel" OnClick="btnExport_Click" 
                                    Width="100px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="style23" colspan="9">
                                <asp:CheckBoxList ID="chkBoxStatus" runat="server" RepeatColumns="5" 
                                    Visible="False">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <asp:View ID="ViewInstructions" runat="server">
                    <table width="100%">
                                             <tr>
                                                 <td class=" " colspan="4" valign="top">
                                                     <asp:GridView ID="dgvInstructions" runat="server" AutoGenerateColumns="False" 
                                                         BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                                                         CellPadding="3" GridLines="Vertical" Width="100%">
                                                         <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                                                         <Columns>
                                                             <asp:CommandField ShowSelectButton="True" />
                                                             <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />
                                                             <asp:BoundField DataField="Amount" DataFormatString="{0:n}" 
                                                                 HeaderText="Amount" />
                                                             <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                                                             <asp:BoundField DataField="document_status_name" 
                                                                 HeaderText="Processing Status" />
                                                             <asp:BoundField DataField="branch_proccessed_date" 
                                                                 HeaderText="Submitted Date" />
                                                             <asp:BoundField DataField="branch_user" HeaderText="Branch" />
                                                             <asp:BoundField DataField="branch_approved_by_user" HeaderText="Branch (TL)" />
                                                             <asp:BoundField DataField="ftroa_user" HeaderText="FTRO" />
                                                             <asp:BoundField DataField="ftrob_user" HeaderText="FTRO (TL)" />
                                                             <asp:BoundField DataField="processor_user" HeaderText="Processor" />
                                                             <asp:BoundField DataField="processor_approved_by_name" 
                                                                 HeaderText="Processor (TL)" />
                                                             <asp:BoundField DataField="locked_by_name" HeaderText="Locked By" />
                                                             <asp:BoundField DataField="locked_date" HeaderText="Locked Date" />
                                                             <asp:BoundField DataField="Client_ID">
                                                                 <ItemStyle Font-Size="0pt" />
                                                             </asp:BoundField>
                                                             <asp:BoundField DataField="Instruction_ID">
                                                                 <ItemStyle Font-Size="0pt" />
                                                             </asp:BoundField>
                                                             <asp:BoundField DataField="File_Name" HeaderText="File Name" Visible="False" />
                                                             <asp:BoundField DataField="related_reference" HeaderText="Rel Ref" />
                                                             <asp:BoundField DataField="status_calculation" HeaderText="Status_C" />
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
                                                 <td class="separator" colspan="4" valign="top">
                                                     &nbsp;</td>
                                             </tr>
                                             <tr>
                                                 <td class="h2" valign="top">
                                                     Customer Details<asp:TextBox ID="txtClientID" runat="server" CssClass="txt" 
                                                         Visible="False" Width="16px"></asp:TextBox>
                                                 </td>
                                                 <td rowspan="2">
                                                     &nbsp;</td>
                                                 <td ID="tdDocument" runat="server" class="h2">
                                                     <asp:Label ID="lblLockedBy" runat="server"></asp:Label>
                                                     &nbsp;|
                                                     <asp:Label ID="lblLockedDate" runat="server" Text="lblLockedDate"></asp:Label>
                                                 </td>
                                                 <td ID="tdDocument2" runat="server" class="style26">
                                                     <table class="style15">
                                                         <tr>
                                                             <td class="align_right" width="130px">
                                                                 &nbsp;&nbsp;&nbsp;&nbsp;
                                                             </td>
                                                             <td class="style20">
                                                                 Attachments</td>
                                                             <td class="align_right" width="160PX">
                                                                 <asp:DropDownList ID="drpAttachments" runat="server" AutoPostBack="True" 
                                                                     Width="200px">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                     </table>
                                                 </td>
                                             </tr>
                                             <tr ID="trInstructionDetails" runat="server">
                                                 <td valign="top" width="220PX">
                                                     <table>
                                                         <tr>
                                                             <td>
                                                                 Cust Name</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtClient_Name" runat="server" CssClass="txt" Font-Bold="True" 
                                                                     ReadOnly="True"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Cust No.</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtClient_Customer_Number" runat="server" CssClass="txt" 
                                                                     Font-Bold="True" ReadOnly="True"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 RM/RO</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpRM" runat="server" CssClass="drp" Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td class="h2" colspan="2">
                                                                 &nbsp;</td>
                                                         </tr>
                                                         <tr>
                                                             <td class="h2" colspan="2">
                                                                 Instruction Details</td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Account</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpAccount" runat="server" CssClass="drp" Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Instruction</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpInstructions" runat="server" AutoPostBack="True" 
                                                                     CssClass="drp" Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Currency</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpCurrency" runat="server" CssClass="drp" 
                                                                     Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Branch</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpBranchs" runat="server" CssClass="drp" Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Amount</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtAmount" runat="server" CssClass="txt" Enabled="False"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Comments</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtComments" runat="server" BackColor="#FFFFCC" CssClass="txt" 
                                                                     Enabled="False" Rows="2" TextMode="MultiLine"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Reference</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtTransactionReference" runat="server" CssClass="txt" 
                                                                     ReadOnly="True"></asp:TextBox>
                                                                 &nbsp;
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 FT&nbsp; Ref</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtFTRef" runat="server" BackColor="#FFFFCC" CssClass="txt" 
                                                                     ReadOnly="True" Rows="1"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td class="h2" colspan="2">
                                                                 &nbsp;</td>
                                                         </tr>
                                                         <tr>
                                                             <td class="h2" colspan="2">
                                                                 Comments</td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Action</td>
                                                             <td>
                                                                 <asp:DropDownList ID="drpValidationStatus" runat="server" AutoPostBack="True" 
                                                                     CssClass="drp" Enabled="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 FTROA</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtFTROAComments" runat="server" BackColor="#FFFFCC" 
                                                                     CssClass="txt" Height="30px" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 FTROB</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtFTROBComments" runat="server" BackColor="#FFFFCC" 
                                                                     CssClass="txt" Height="30px" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 Processor</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtProcessorComments" runat="server" BackColor="#FFFFCC" 
                                                                     CssClass="txt" Height="30px" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td>
                                                                 RM/RO</td>
                                                             <td>
                                                                 <asp:TextBox ID="txtRMComments" runat="server" BackColor="#FFFFCC" 
                                                                     CssClass="txt" Height="30px" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td class="line_on_top" colspan="2">
                                                                 &nbsp;&nbsp;<asp:TextBox ID="txtFileName" runat="server" CssClass="txt" ReadOnly="True" 
                                                                     Visible="False" Width="50px"></asp:TextBox>
                                                                 <asp:TextBox ID="txtStatus" runat="server" CssClass="txt" ReadOnly="True" 
                                                                     Visible="False" Width="50px"></asp:TextBox>
                                                                 <asp:TextBox ID="txtDocumentStatusID" runat="server" CssClass="txt" 
                                                                     ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                                 <asp:TextBox ID="txtInstructionStatus" runat="server" CssClass="txt" 
                                                                     ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                                 <asp:TextBox ID="txtInstructionID" runat="server" CssClass="txt" 
                                                                     ReadOnly="True" Visible="False" Width="18px"></asp:TextBox>
                                                             </td>
                                                         </tr>
                                                         <tr>
                                                             <td class="style17" colspan="2">
                                                                 &nbsp;</td>
                                                         </tr>
                                                     </table>
                                                 </td>
                                                 <td colspan="2" valign="top">
                                                     <cc1:ShowPdf ID="ShowPdf1" runat="server" FilePath="Images/logo.png" 
                                                         Height="600px" Width="100%" />
                                                 </td>
                                             </tr>
                                             <tr>
                                                 <td colspan="4">
                                                     &nbsp;</td>
                                             </tr>
                    </table>
                </asp:View>
            </asp:MultiView>
        </td>
    </tr>
</table>

    </form>
</body>
</html>
