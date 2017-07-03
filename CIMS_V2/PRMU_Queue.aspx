<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="PRMU_Queue.aspx.cs" Inherits="CIMS_V2.Unknown_Views.PRMU_Queue" %>

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
            width: 840px;
        }

        .style23 {
            width: 155px;
        }

        .style24 {
            width: 219px;
        }

        .style25 {
            width: 167px;
        }

        .style26 {
            width: 216px;
        }

        .style27 {
            width: 145px;
        }
    </style>
    <link href="CSS/Q.css" rel="stylesheet" type="text/css" />


    <table class="fill_width" width="100%">
        <tr>
            <td class="line_at_bottom">
                <table class="style21">
                    <tr>
                        <td class="style25">User Type</td>
                        <td class="style27">Status</td>
                        <td class="style27">&nbsp;</td>
                        <td class="style27">Specify Cutoff Range</td>
                        <td class="style23">Minutes To Cutoff From</td>
                        <td class="style23">Minutes To Cutoff To</td>
                        <td class="style25">&nbsp;</td>
                    </tr>
                    <tr>
                        <td class="style25">
                            <asp:DropDownList ID="drpUserType" runat="server" Width="150px">
                            </asp:DropDownList>
                        </td>
                        <td class="style27">
                            <asp:DropDownList ID="drpStatus" runat="server" Width="150px" AutoPostBack="True">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>Referrals</asp:ListItem>
                                <asp:ListItem>Packed</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="style27">&nbsp;</td>
                        <td class="style27">
                            <asp:CheckBox ID="chkCutoffRange" runat="server" AutoPostBack="True" />
                        </td>
                        <td class="style23">
                            <asp:TextBox ID="txtMinutesToCuttoffFrom" runat="server" Visible="False"></asp:TextBox>
                        </td>
                        <td class="style23">
                            <asp:TextBox ID="txtMinutesToCuttoffTo" runat="server" Visible="False"></asp:TextBox>
                        </td>
                        <td class="style25">
                            <asp:Button ID="btnView" runat="server" Text="View" Width="96px" />
                        </td>
                        <td class="style24">
                            <asp:Button ID="btnExport" runat="server" Text="Export To Excel"
                                Width="100px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td class="style18">
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="ViewCustomerList" runat="server">
                        <asp:GridView ID="dgvInstructions" runat="server" AutoGenerateColumns="False"
                            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                            CellPadding="3" GridLines="Vertical" Width="100%">
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" />
                                <asp:BoundField DataField="Client_ID">
                                    <ItemStyle Font-Size="0pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Instruction_ID">
                                    <ItemStyle Font-Size="0pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Instruction_Type" HeaderText="Instruction Type" />
                                <asp:BoundField DataField="Amount" DataFormatString="{0:n}"
                                    HeaderText="Amount" />
                                <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                                <asp:BoundField DataField="document_status_name"
                                    HeaderText="Processing Status" />
                                <asp:BoundField DataField="branch_proccessed_date"
                                    HeaderText="Submitted Date" />
                                <asp:BoundField DataField="branch_user" HeaderText="Branch" />
                                <asp:BoundField DataField="File_Name" HeaderText="File Name" Visible="False" />
                                <asp:BoundField DataField="comments" HeaderText="Branch Comments" />
                                <asp:BoundField DataField="ftroa_comments" HeaderText="PRMO 1 comments" />
                                <asp:BoundField DataField="ftrob_comments" HeaderText="PRMO 2 comments" />
                                <asp:BoundField DataField="prmo2_comments" HeaderText="PRMU TL Comments" />
                                <asp:BoundField DataField="processor_comments"
                                    HeaderText="Processor comments" />
                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                        </asp:GridView>
                    </asp:View>
                    <asp:View ID="ViewInstructions" runat="server">
                        <table width="100%">
                            <tr>
                                <td class="separator" valign="top" colspan="4">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="h2" valign="top">Customer Details<asp:TextBox ID="txtClientID" runat="server" CssClass="txt"
                                    Width="16px" Visible="False"></asp:TextBox>
                                </td>
                                <td rowspan="2">&nbsp;</td>
                                <td id="tdDocument" runat="server" class="h2">
                                    <asp:Label ID="lblLockedBy" runat="server"></asp:Label>
                                    &nbsp;|
                                    <asp:Label ID="lblLockedDate" runat="server" Text="lblLockedDate"></asp:Label>
                                </td>
                                <td id="tdDocument2" runat="server" class="style26">
                                    <table class="style15">
                                        <tr>
                                            <td class="align_right" width="130px">
                                                <asp:Button ID="btnUnlock" runat="server" Text="Unlock" />
                                                &nbsp;&nbsp;<asp:Button ID="btnRecall" runat="server" Text="Recall" />
                                                &nbsp;&nbsp;
                                                <asp:Button ID="btnUnpack" runat="server" Enabled="False" Text="Unpack" />
                                            </td>
                                            <td class="style20">Attachments</td>
                                            <td class="align_right" width="160PX">
                                                <asp:DropDownList ID="drpAttachments" runat="server" AutoPostBack="True"
                                                    Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trInstructionDetails" runat="server">
                                <td valign="top" width="220PX">
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
                                            <td>Comments</td>
                                            <td>
                                                <asp:TextBox ID="txtComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" Rows="2" TextMode="MultiLine" Enabled="False"></asp:TextBox>
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
                                                <asp:DropDownList ID="drpValidationStatus" runat="server" CssClass="drp"
                                                    AutoPostBack="True" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>FTROA</td>
                                            <td>
                                                <asp:TextBox ID="txtFTROAComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine" Height="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>FTROB</td>
                                            <td>
                                                <asp:TextBox ID="txtFTROBComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine" Height="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Processor</td>
                                            <td>
                                                <asp:TextBox ID="txtProcessorComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine" Height="30px"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>RM/RO</td>
                                            <td>
                                                <asp:TextBox ID="txtRMComments" runat="server" BackColor="#FFFFCC"
                                                    CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine" Height="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" class="line_on_top">&nbsp;&nbsp;<asp:TextBox ID="txtFileName" runat="server" CssClass="txt" ReadOnly="True"
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
                                            <td class="style17" colspan="2">&nbsp;</td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top" colspan="2">
                                    <cc1:ShowPdf ID="ShowPdf1" runat="server" FilePath="Images/logo.png" Height="600px"
                                        Width="100%" />
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
