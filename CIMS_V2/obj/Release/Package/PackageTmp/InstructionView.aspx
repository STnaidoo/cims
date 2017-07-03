<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstructionView.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Instruction.InstructionView" %>

<%@ Register Assembly="PdfViewer" Namespace="PdfViewer" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .style15 {
            width: 100%;
        }

        .style17 {
            height: 26px;
        }

        .style18 {
            height: 23px;
        }

        .style20 {
            text-align: right;
            width: 37px;
        }

        .style21 {
            width: 192px;
        }

        .style22 {
            width: 82px;
        }

        .style24 {
            width: 293px;
        }

        .style25 {
            width: 91px;
        }

        .style27 {
            width: 161px;
        }

        .style28 {
            width: 72px;
        }

        .style29 {
            width: 66px;
        }

        .auto-style2 {
            width: 293px;
            height: 48px;
        }
        .auto-style3 {
            height: 48px;
        }

    </style>

    <table class="fill_width" style="width:100%">
        <tr>
            <td class="line_at_bottom">
                <table class="style21" style="width:100%">
                    <tr>
                        <td class="style19">Filter Option</td>
                        <td>Filter Value</td>
                        <td class="style24">Instruction Type</td>
                        <td class="style24">Status&nbsp;
                            <asp:CheckBox Visible="false" ID="chkMultiple" runat="server" AutoPostBack="True"
                            Text="Show Multiple" oncheckedchanged="chkMultiple_CheckedChanged"/>
                        </td>
                        <td class="style24">
                            <asp:Label ID="lblFrom" runat="server" Text="From"></asp:Label>
                        </td>
                        <td class="style24">&nbsp;</td>
                        <td class="style24">
                            <asp:Label ID="lblTo" runat="server" Text="To"></asp:Label>
                        </td>
                        <td class="style24">
                            <asp:Button ID="btnExport"  runat="server" Text="Export" 
                                Width="100px" onclick="btnExport_Click"/>
                        </td>
                        <td class="style24">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="style19">
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="txt" Width="150px"></asp:TextBox>
                        </td>
                        <td class="style24">
                            <asp:DropDownList ID="drpInstructions0" runat="server" Width="200px">
                            </asp:DropDownList>
                        </td>
                        <td class="style24">
                            <asp:DropDownList Visible="false" ID="drpStatus" runat="server" Width="210px">
                            </asp:DropDownList>
                        </td>
                        <td class="style24">
                            <asp:TextBox ID="dtmFrom" runat="server" Width="100px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="dtmFrom" />
                        </td>
                        <td class="style24">&nbsp;</td>
                        <td class="style24">
                            <asp:TextBox ID="dtmTo" runat="server" Width="100px"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender2" runat="server"
                                Format="dd-MMM-yyyy"
                                TargetControlID="dtmTo" />
                        </td>
                        <td class="style24">
                            <asp:Button ID="btnView" runat="server" Text="View" Width="96px" UseSubmitBehavior="false" onclick="btnView_Click"/>
                        </td>
                        <td class="style24">&nbsp;</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="chkShowComments" runat="server" AutoPostBack="True" Text="Show Comments" oncheckedchanged="chkShowComments_CheckedChanged"/>
            </td>
        </tr>
        <tr>
            <td class="line_at_bottom">
                <asp:CheckBoxList ID="chkBoxStatus" runat="server" RepeatColumns="5"
                    Visible="False">
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr>
            <td class="style18">
                
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="ViewCustomerList" runat="server">
                 
                        <asp:GridView ID="dgvInstructions" runat="server" AutoGenerateColumns="False"
                            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                            CellPadding="3" GridLines="Vertical" Width="100%" OnRowCommand="dgvInstructions_RowCommand">
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" />
                                <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />
                                <asp:BoundField DataField="currency_name" HeaderText="Currency" />
                                <asp:BoundField DataField="Amount" DataFormatString="{0:n}"
                                    HeaderText="Amount" />
                                <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                                <asp:BoundField DataField="Client_Account_Number" HeaderText="Client Account" />
                                <asp:BoundField DataField="document_status_name"
                                    HeaderText="Processing Status" />
                                <asp:BoundField DataField="delivery_date" HeaderText="Delivery Date" />
                                <asp:BoundField DataField="branch_proccessed_date"
                                    HeaderText="Submitted Date" />
                                <asp:BoundField DataField="allocated_to_name" HeaderText="Allocated To" />
                                <asp:BoundField DataField="picked_by_name" HeaderText="Picked By" />
                                <asp:BoundField DataField="branch_name" HeaderText="Branch Name" />
                                <asp:BoundField DataField="branch_user" HeaderText="Originator" />
                                <asp:BoundField DataField="branch_approved_by_user"
                                    HeaderText="Originator TL" />
                                <asp:BoundField DataField="ccc_officer_submitter" HeaderText="CCC Officer" />
                                <asp:BoundField DataField="ccc_tl_submitter" HeaderText="CCC TL" />
                                <asp:BoundField DataField="prmo_submitter" HeaderText="PRMO" />
                                <asp:BoundField DataField="prmo_tl_submitter" HeaderText="PRMO -TL" />
                                <asp:BoundField DataField="prmo_tl_submitter" HeaderText="PRMU TL"
                                    Visible="False" />
                                <asp:BoundField DataField="prmu_manager_proccessed_by_name"
                                    HeaderText="PRMU Manager " Visible="False" />
                                <asp:BoundField DataField="pay_officer_submitter" HeaderText="Pay Officer" />
                                <asp:BoundField DataField="pay_tl_submitter"
                                    HeaderText="Pay (TL)" />
                                <asp:BoundField DataField="validation_officer_submitter"
                                    HeaderText="Validation Officer" />
                                <asp:BoundField DataField="validation_manager_submitter"
                                    HeaderText="Validation Manager" />
                                <asp:BoundField DataField="trm_officer_submitter"
                                    HeaderText="TRMU Officer" />
                                <asp:BoundField DataField="trm_manager_submitter"
                                    HeaderText="TRMU TL/Manager" />
                                <asp:BoundField DataField="locked_by_name" HeaderText="Locked By" />
                                <asp:BoundField DataField="locked_date" HeaderText="Locked Date" />
                                <asp:BoundField DataField="Client_ID">
                                    <ItemStyle Font-Size="0pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Instruction_ID">
                                    <ItemStyle Font-Size="0pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="File_Name" HeaderText="File Name" Visible="False" />
                                <asp:BoundField DataField="related_reference" HeaderText="Rel Ref"
                                    Visible="False" />
                                <asp:BoundField DataField="status_calculation" HeaderText="Status_C" />
                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                        </asp:GridView>
                      
                    </asp:View>
                    <asp:View ID="ViewInstructions" runat="server">
                        <table style="width:100%">
                            <tr>
                                <td class="separator" style="vertical-align:top" colspan="4">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="h2" style="vertical-align:top">&nbsp;</td>
                                <td rowspan="3">&nbsp;</td>
                                <td id="tdDocument" runat="server" class="h2">
                                    <asp:Label ID="lblLockedBy" runat="server"></asp:Label>
                                    &nbsp;|
                                    <asp:Label ID="lblLockedDate" runat="server" Text="lblLockedDate"></asp:Label>
                                </td>
                                <td id="tdDocument2" runat="server" class="h2">
                                    <table class="style15">
                                        <tr>
                                            <td class="align_right" style="width:130px">&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;</td>
                                            <td class="align_right">Other Attachments</td>
                                            <td class="align_right" style="width:20px">
                                                <asp:DropDownList ID="drpAttachments" runat="server" AutoPostBack="True"
                                                    Width="200px" OnSelectedIndexChanged="drpAttachments_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnOpen" runat="server" Text="Open" Visible="False" onclick="btnOpen_Click"/>
                                            </td>
                                            <td class="align_right" style="width: 80px">
                                                <asp:Button ID="btnPreviewFull" runat="server" Text="Preview In Full Screen" onclick="btnPreviewFull_Click"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="h2" style="vertical-align:top">Customer Details<asp:TextBox ID="txtClientID" runat="server" CssClass="txt"
                                    Visible="False" Width="16px"></asp:TextBox>
                                </td>
                                <td id="tdDocument4" runat="server" class="h2" colspan="2">
                                    <table class="style15">
                                        <tr>
                                            <td class="style22">Priority</td>
                                            <td class="style25">
                                                <asp:DropDownList ID="drpPriority" runat="server" Width="60px" Enabled="False">
                                                    <asp:ListItem>1</asp:ListItem>
                                                    <asp:ListItem>0</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td class="style27">
                                                <asp:Button ID="btnChangePriority" runat="server" Text="Change Priority"
                                                    Enabled="False" onclick="btnChangePriority_Click"/>
                                            </td>
                                            <td class="style28">
                                                <asp:Button ID="btnUnlock" runat="server" Text="Unlock" onclick="btnUnlock_Click"/>
                                            </td>
                                            <td class="style29">
                                                <asp:Button ID="btnRecall" runat="server" Text="Recall" onclick="btnRecall_Click"/>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnUnpack" runat="server" Enabled="False" Text="Unpack" onclick="btnUnpack_Click"/>
                                            </td>
                                            <td>
                                                <table class="style15">
                                                    <tr>
                                                        <td class="align_right">
                                                            <asp:Button ID="btnUnpackAndAllocate" runat="server" Enabled="False"
                                                                Text="Unpack and Allocate To" onclick="btnUnpackAndAllocate_Click"/>
                                                        </td>
                                                        <td class="align_right">
                                                            <asp:Button ID="btnTakeOver" runat="server" Text="Take Over" Visible="False" onclick="btnTakeOver_Click"/>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="drpAllocatedTo" runat="server" Enabled="False"
                                                                Width="150px">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>Status</td>
                                                        <td>
                                                            <asp:DropDownList ID="drpUnpackStatuss" runat="server" Enabled="False"
                                                                Width="150px">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trInstructionDetails" runat="server">
                                <td style="vertical-align:top; width:220PX">
                                    <table>
                                        <tr>
                                            <td>Cust Name</td>
                                            <td>
                                                <asp:TextBox ID="txtClient_Name" runat="server" CssClass="txt" ReadOnly="True"
                                                    Font-Bold="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Cust No.</td>
                                            <td>
                                                <asp:TextBox ID="txtClient_Customer_Number" runat="server" CssClass="txt"
                                                    ReadOnly="True" Font-Bold="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>RM/RO</td>
                                            <td>
                                                <asp:DropDownList ID="drpRM" runat="server" CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">Instruction Details</td>
                                        </tr>
                                        <tr>
                                            <td>Account</td>
                                            <td>
                                                <asp:DropDownList ID="drpAccount" runat="server" CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Instruction</td>
                                            <td>
                                                <asp:DropDownList ID="drpInstructions" runat="server" AutoPostBack="True"
                                                    CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Currency</td>
                                            <td>
                                                <asp:DropDownList ID="drpCurrency" runat="server" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Cross Currency</td>
                                            <td>
                                                <table class="style15">
                                                    <tr>
                                                        <td class="style20">
                                                            <asp:DropDownList ID="drpCrossCurrency" runat="server" Enabled="False"
                                                                Width="63px">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value="2">No</asp:ListItem>
                                                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td class="style22">Rate</td>
                                                        <td>
                                                            <asp:TextBox ID="txtCCRate" runat="server" ReadOnly="True" Width="95px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Branch</td>
                                            <td>
                                                <asp:DropDownList ID="drpBranchs" runat="server" CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td>Amount</td>
                                            <td>
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="txt" Enabled="False"></asp:TextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="style19">Delivery Date</td>
                                            <td>
                                                <asp:TextBox ID="txtDeliveryDat" runat="server" CssClass="txt" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Comments</td>
                                            <td>
                                                <asp:TextBox ID="txtComments" runat="server" CssClass="txt" BackColor="#FFFFCC"
                                                    Enabled="False" Rows="3" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Reference</td>
                                            <td>
                                                <asp:TextBox ID="txtTransactionReference" runat="server" CssClass="txt"
                                                    ReadOnly="True"></asp:TextBox>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>FT&nbsp; Ref</td>
                                            <td>
                                                <asp:TextBox ID="txtFTRef" runat="server" BackColor="#FFFFCC" CssClass="txt"
                                                    ReadOnly="True" Rows="1"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">Comments</td>
                                        </tr>

                                        <tr>
                                            <td>Action</td>
                                            <td>
                                                <asp:DropDownList ID="drpValidationStatus" runat="server" AutoPostBack="True" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="style19">DOC</td>
                                            <td>
                                                <asp:DropDownList ID="drpDOC" runat="server" AutoPostBack="True" CssClass="drp"
                                                    Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Call Back</td>
                                            <td>
                                                <asp:TextBox ID="txtCallBackComment" runat="server"
                                                    CssClass="txt" ReadOnly="True" BackColor="#FFFFCC" Rows="1"
                                                    TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Call Back No</td>
                                            <td>
                                                <asp:TextBox ID="txtCallBackNos" runat="server"
                                                    CssClass="txt" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPRMO1Comments" runat="server">
                                            <td class="style19">PRMO 1</td>
                                            <td>
                                                <asp:TextBox ID="txtPRMO1Commentss" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPRMO2Comments" runat="server">
                                            <td class="style19">PRMO 2</td>
                                            <td>
                                                <asp:TextBox ID="txtPRMO2Commentss" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPRMOTLComments" runat="server">
                                            <td class="style19">PRMO TL</td>
                                            <td>
                                                <asp:TextBox ID="txtPRMOTLCommentss" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr id="trProcessorComments" runat="server">
                                            <td class="style19">Processor</td>
                                            <td>
                                                <asp:TextBox ID="txtProcessorComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trRMComments" runat="server">
                                            <td class="style19">RM/RO</td>
                                            <td>
                                                <asp:TextBox ID="txtRMComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr id="trOtherComments1" runat="server">
                                            <td class="h2" colspan="2">Other Comments</td>
                                        </tr>
                                        <tr id="trOtherComments2" runat="server">
                                            <td class="style19" colspan="2">
                                                <asp:GridView ID="dgvComments" runat="server" AutoGenerateColumns="False"
                                                    GridLines="Horizontal" Width="100%">
                                                    <Columns>
                                                        <asp:BoundField DataField="instruction_comment" HeaderText="Other Comments" />
                                                        <asp:BoundField DataField="instruction_comments_type" HeaderText="Status"
                                                            Visible="False" />
                                                        <asp:BoundField DataField="instruction_comment_by_name" HeaderText="By">
                                                            <ItemStyle Width="80px" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="instruction_comment_date" HeaderText="Date"
                                                            Visible="True">
                                                            <ItemStyle Width="20px" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="auto-style3">
                                                <asp:Label ID="AllocationLbl" runat="server" Visible="false">New Allocation</asp:Label>
                                            </td>
                                    
                                             <td class="auto-style3">
                                                <asp:DropDownList ID="drpUserList" runat="server" Visible="false" OnSelectedIndexChanged="drpUserList_SelectedIndexChanged" Width="150px"></asp:DropDownList>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="Allocation" runat="server" onclick="Allocation_Click" Text="Change Allocation" UseSubmitBehavior="false" Width="177px" Visible="false" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="line_on_top" colspan="2">&nbsp;&nbsp;<asp:TextBox ID="txtFileName" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtStatus" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtDocumentStatusID" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtInstructionStatus" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtInstructionID" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="18px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style17" colspan="2">
                                                &nbsp;

                                            </td>
                                        </tr>
                                    
                                    </table>
                                </td>
                                <td style="vertical-align:top" colspan="2">
                                    <cc1:ShowPdf ID="ShowPdf1" runat="server" FilePath="Images/logo.png" Height="650px"
                                        Width="567px" />
                                    <asp:GridView runat="server" ID="excelGridView" Visible="false" BorderWidth="1px" Width="100%" GridLines="Horizontal" BackColor="White">
                                        <HeaderStyle BackColor=" #000084" Font-Bold="true" ForeColor="White"/>
                                    </asp:GridView>
                                   
                                    <asp:Button ID="downloadButton" runat="server" OnClick="downloadButton_Click" Text="Download" Visible="false" Width="76px" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">&nbsp;</td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </table>
</asp:Content>

