<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="GetNext.aspx.cs" Inherits="CIMS_V2.GetNext" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="PdfViewer" Namespace="PdfViewer" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .style20 {
            text-align: right;
            width: 216px;
        }

        .style22 {
            text-align: right;
            width: 253px;
        }

        .style26 {
            text-align: right;
            width: 285px;
        }

        .style31 {
            width: 128px;
        }

        .fill_width {
            width: 81%;
            margin-right: 0px;
        }

        .line_at_bottom {
            color: #000088;
            font-size: 14px;
            font-family: Arial;
            font-weight: bold;
            border-bottom-color: #DCE6F1;
            border-bottom-style: solid;
            border-bottom-width: 2px;
        }
        .auto-style2 {
            width: 6px;
        }
        .auto-style3 {
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-weight: 500;
            line-height: 1.1;
            font-size: 30px;
            width: 396px;
        }
        .auto-style4 {
            width: 396px;
        }
        .auto-style6 {
            width: 275px;
        }
        .auto-style7 {
            height: 54px;
        }
        .auto-style8 {
            height: 22px;
        }
        .auto-style9 {
            width: 275px;
            height: 22px;
        }
    </style>
    
    <table>
        <tr>
            <td class="line_at_bottom">
                
                <table>
                    <tr>
                        <td>
                            <table class="style15">
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbnGetNext" runat="server" AutoPostBack="True"
                                            Checked="True" GroupName="search" Text="Get Next" Visible="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbnSearchInstruction" runat="server" AutoPostBack="True"
                                            GroupName="search" Text="Search Instruction" Visible="False" />
                                    </td>
                                </tr>
                            </table>
                            &nbsp;</td>
                        <td>
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="txt"></asp:TextBox>
                        </td>
                        <td>
                            <table class="style15">
                                <tr>
                                    <td class="style31">
                                        <asp:RadioButton ID="rbnUnProcessed" runat="server" Checked="True"
                                            GroupName="process" Text="Not Submitted" AutoPostBack="True" OnCheckedChanged="rbnUnProcessed_CheckedChanged" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Label ID="lblFrom" runat="server" Text="From" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox  class="input-append date" ID="dtmFrom" runat="server" Visible="False"></asp:TextBox>
                                         <asp:CalendarExtender ID="CalendarExtender1" runat="server" 
                                             Format="dd-MMM-yyyy" TargetControlID="dtmFrom" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style31">
                                        <asp:RadioButton ID="rbnProcessed" runat="server" Text="Submitted"
                                            AutoPostBack="True" GroupName="process" OnCheckedChanged="rbnProcessed_CheckedChanged" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Label ID="lblTo" runat="server" Text="To" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox class="input-append date" ID="dtmTo" runat="server" Visible="False"></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd-MMM-yyyy" 
                                        TargetControlID="dtmTo" />
                                </td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="btnView" runat="server" Text="View" Width="96px" OnClick="btnView_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnGetNext" runat="server" Text="Get Next" Width="96px" OnClick="btnGetNext_Click" />
                        </td>
                    </tr>
         </table>
     
    <table>
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
                                <asp:BoundField DataField="document_status_name"
                                    HeaderText="Processing Status" />
                                <asp:BoundField DataField="delivery_date" HeaderText="Delivery Date" />
                                <asp:BoundField DataField="branch_proccessed_date"
                                    HeaderText="Submitted Date" />
                                <asp:BoundField DataField="allocated_to_name" HeaderText="Allocated To" />
                                <asp:BoundField DataField="picked_by_name" HeaderText="Picked By" />
                                <asp:BoundField DataField="branch_user" HeaderText="Originator" />
                                <asp:BoundField DataField="branch_name" HeaderText="Branch Name" />
                                <asp:BoundField DataField="branch_approved_by_user"
                                    HeaderText="Originator TL" />
                                <asp:BoundField DataField="prmo_submitter" HeaderText="PRMO" />
                                <asp:BoundField DataField="prmo_tl_submitter" HeaderText="PRMO (TL)" />
                                <asp:BoundField DataField="prmu_manager_proccessed_by_name"
                                    HeaderText="PRMU Manager" Visible="False" />
                                <asp:BoundField DataField="pay_officer_submitter" HeaderText="Pay Officer" />
                                <asp:BoundField DataField="pay_tl_submitter"
                                    HeaderText="Pay (TL)" />
                                <asp:BoundField DataField="locked_by_name" HeaderText="Locked By" />
                                <asp:BoundField DataField="locked_date" HeaderText="Locked Date" />
                                <asp:BoundField DataField="validation_officer_submitter"
                                    HeaderText="Validation Officer" />
                                <asp:BoundField DataField="validation_manager_submitter"
                                    HeaderText="Validation Manager" />
                                <asp:BoundField DataField="trm_officer_submitter" HeaderText="TRM Officer" />
                                <asp:BoundField DataField="trm_manager_submitter" HeaderText="TRM Manager/TL" />
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
                    </asp:View>
                    <asp:View ID="ViewInstructions" runat="server">
                        <table>
                            <tr>
                                <td class="separator" colspan="4">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style3">
                                    <img alt="" src="images/pointer.png" style="width: 16px; height: 16px" />
                                    Customer Details<asp:TextBox ID="txtClientID" runat="server" CssClass="txt"
                                        Width="16px" Visible="False"></asp:TextBox>
                                    <asp:Button ID="btnNew" runat="server" Text="New Instruction" Visible="False" OnClick="btnNew_Click" />
                                </td>
                                <td rowspan="2" class="auto-style2">&nbsp;</td>
<%--                                <td id="tdDocument" runat="server" class="h2">
                                    <asp:Label ID="lblLockedBy" runat="server"></asp:Label>
                                    &nbsp;
                                    <asp:Label ID="lblLockedDate" runat="server" Text="lblLockedDate"></asp:Label>
                                </td>--%>
                                <td id="tdDocument2" runat="server" class="style26">
                                    <table class="style15">
                                        <tr>
                                            <td class="auto-style7">
                                                <asp:FileUpload ID="FileUpload2" runat="server" CssClass="fill_width"
                                                    Visible="False" Width="221px" />
                                            </td>
                                            <td class="auto-style7"></td>
                                            <td class="auto-style7">
                                                <asp:Button ID="btnAdd" runat="server" Style="height: 26px"
                                                    Text="Add Attachment" Width="140px" Visible="False" OnClick="btnAdd_Click" />
                                            </td>
                                            <td class="auto-style7"></td>
                                            <td class="auto-style7">
                                                <asp:DropDownList ID="drpAttachments" runat="server" AutoPostBack="True"
                                                    Width="150px" Visible="false">
                                                </asp:DropDownList>
                                            </td>
                                            <td class="auto-style7">
                                                <asp:Button ID="btnOpen" runat="server" Text="Open" Visible="False" />
                                            </td>
                                            <td class="auto-style7">
                                                <asp:Button ID="btnDeleteAttachment" runat="server" Text="Delete" Visible="false" />
                                            </td>
                                            <td class="auto-style7">
                                                <asp:Button ID="Button1" runat="server" Text="Preview"  Visible="false"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trInstructionDetails" runat="server">
                                <td class="auto-style4">
                                    <table>
                                        <tr>
                                            <td class="style19">Cust Name</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtClient_Name" runat="server" CssClass="txt" ReadOnly="True"
                                                    Font-Bold="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Cust No.</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtClient_Customer_Number" runat="server" CssClass="txt"
                                                    ReadOnly="True" Font-Bold="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">RM/RO</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpRM" runat="server" CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">
                                                <img alt="" src="images/pointer.png" style="width: 16px; height: 16px" />
                                                Instruction Details</td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table class="style15">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkAllowDuplicates" runat="server" Text="Allow Duplicate" />
                                                        </td>
                                                        <td class="align_right">
                                                            <asp:CheckBox ID="chkProcessAtBranch" runat="server" AutoPostBack="True"
                                                                Text="Processed at Branch" Enabled="False" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Account</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpAccount" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Instruction</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpInstructions" runat="server" AutoPostBack="True"
                                                    CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trCurrency" runat="server">
                                            <td class="style19">Currency</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpCurrency" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trCrossCurrency" runat="server">
                                            <td class="style19">Cross Currency</td>
                                            <td class="auto-style6">
                                                <table class="style15">
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="drpCrossCurrency" runat="server" Enabled="False"
                                                                Width="63px">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value="2">No</asp:ListItem>
                                                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>Rate</td>
                                                        <td>
                                                            <asp:TextBox ID="txtCCRate" runat="server" ReadOnly="True" Width="95px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Origin Branch</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpBranchs" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trAmount" runat="server">
                                            <td class="style19">Amount</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="txt"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Delivery Date</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtDeliveryDat" runat="server" CssClass="txt" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trComments" runat="server">
                                            <td class="style19">Comments</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" Rows="3" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Reference</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtTransactionReference" runat="server" CssClass="txt"
                                                    ReadOnly="True" Width="126px"></asp:TextBox>
                                                &nbsp;
                        <asp:Button ID="btnGenerate" runat="server" Text="Get Ref" Width="70px" />
                                            </td>
                                        </tr>
                                        <tr id="trRelatedTransactionReferences" runat="server">
                                            <td class="style19">Related Ref</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtRelatedTransactionReference" runat="server" CssClass="txt"
                                                    ReadOnly="True" Width="126px"></asp:TextBox>
                                                &nbsp;
                        <asp:Button ID="btnGenerate0" runat="server" Enabled="False" Text="View"
                            Width="68px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">EE&nbsp; Ref</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtFTRef" runat="server" BackColor="#FFFFCC" CssClass="txt"
                                                    ReadOnly="True" Rows="1"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">

                                                &nbsp;<asp:TextBox ID="txtStatus" runat="server" CssClass="txt" ReadOnly="True"
                                                    Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtInstructionStatus" runat="server" CssClass="txt"
                                                    ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtDocumentStatusID" runat="server" CssClass="txt"
                                                    ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtInstructionID" runat="server" CssClass="txt"
                                                    ReadOnly="True" Visible="False" Width="18px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trUploads" runat="server">
                                            <td colspan="2" class="line_on_top">
                                                <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" Visible="false" ID="lblSupportingDoc">Excel Sheet</asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:FileUpload ID="supportingDocUpload" runat="server" Width="100%" visible="false"/>
                                                            </td>
                                                        </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="btnSupportingAttach" runat="server" OnClick="btnSupportingAttach_Click" Text="Attach Excel" Width="120px" Visible="false" />
                                                                <asp:TextBox ID="txtSupportingFileName" runat="server" CssClass="txt" ReadOnly="true" Visible="false" Width="258px" ></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>

                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                            <td>
                                                                <asp:Label runat="server" Visible="false" ID="lblPDFDoc">PDF Document</asp:Label>
                                                            </td>
                                                        </tr>

                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>

                                                    <tr>
                                                        <td>
                                                            <asp:FileUpload ID="FileUpload1" runat="server" Width="100%"/>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="btnAttach" runat="server" Text="Attach PDF"
                                                                Width="117px" OnClick="btnAttach_Click" />
                                                           <asp:TextBox ID="txtFileName" runat="server" CssClass="txt" ReadOnly="True"
                                                            Visible="False" Width="261px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="downloadButton" runat="server" OnClick="downloadButton_Click" Text="Download" Visible="false" Width="119px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">&nbsp;<img alt="" src="images/pointer.png" style="width: 16px; height: 16px" />
                                                Action and Comments</td>
                                        </tr>
                                        <tr>
                                            <td class="style19">Action</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpValidationStatus" runat="server"
                                                    CssClass="drp" OnSelectedIndexChanged="drpValidationStatus_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style19">DOC</td>
                                            <td class="auto-style6">
                                                <asp:DropDownList ID="drpDOC" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trCallBackComments" runat="server">
                                            <td class="style19">Call Back</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtCallBackComment" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr id="trCallBackNo" runat="server">
                                            <td class="style19">Call Back No</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtCallBackNos" runat="server" CssClass="txt" ReadOnly="True"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr id="trPRMO1Comments" runat="server">
                                            <td class="style19">PRMO 1</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtPRMO1Comments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPRMO2Comments" runat="server">
                                            <td class="style19">PRMO 2</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtPRMO2Comments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trPRMOTLComments" runat="server">
                                            <td class="style19">PRMO TL</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtPRMOTLComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trProcessorComments" runat="server">
                                            <td class="auto-style8">Processor</td>
                                            <td class="auto-style9">
                                                <asp:TextBox ID="txtProcessorComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trRMComments" runat="server">
                                            <td class="style19">RM/RO</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtRMComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="trOtherComments1" runat="server">
                                            <td class="style19">Other Comments</td>
                                            <td class="auto-style6">
                                                <asp:TextBox ID="txtOtherComments" runat="server" CssClass="txt"
                                                    TextMode="MultiLine"></asp:TextBox>
                                            </td>
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
                                            <td class="line_on_top" colspan="2">
                                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                                                &nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" Visible="false"/>
                                                &nbsp;<asp:Button ID="btnSubmit" runat="server" Text="Submit" Visible="False"
                                                    Width="140px" onclick="btnSubmit_Click"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="style17" colspan="2">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="vertical-align: top" colspan="3">
                                    <cc1:ShowPdf ID="ShowPdf1" runat="server" Height="650px"
                                        Width="567px" />
                                    <asp:GridView runat="server" ID="excelGridView" Visible="false" BorderWidth="1px" Width="100%" GridLines="Horizontal" BackColor="White">
                                        <HeaderStyle BackColor=" #000084" Font-Bold="true" ForeColor="White"/>
                                    </asp:GridView>
                                   
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">&nbsp;</td>
                            </tr>
                        </table>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </table>
</asp:Content>

