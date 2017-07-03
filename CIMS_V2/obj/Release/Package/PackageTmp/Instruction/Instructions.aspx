<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeBehind="Instructions.aspx.cs" MasterPageFile="~/Site.Master" Inherits="CIMS_V2.Instructions" %>

<%@ Register Assembly="PdfViewer" Namespace="PdfViewer" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .style15 {
            width: 231%;
            height: 58px;
        }


        .style18 {
            height: 23px;
        }

        .style19 {
            width: 118px;
        }

        .style20 {
            width: 71px;
        }

        .style21 {
            width: 1044px;
        }

        .style22 {
            width: 48px;
        }

        .style23 {
            width: 87px;
        }

        .style24 {
            width: 130px;
        }

        .style25 {
            width: 129px;
        }

        .style26 {
            width: 91px;
        }

        .style27 {
            width: 32px;
        }

        .auto-style3 {
            width: 47px;
        }
        .auto-style8 {
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-weight: 500;
            line-height: 1.1;
            font-size: 30px;
            height: 35px;
        }
        .auto-style11 {
            width: 325px;
        }
        .auto-style14 {
            width: 467px;
        }
        .auto-style15 {
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-weight: 500;
            line-height: 1.1;
            font-size: 30px;
            width: 220px;
        }
        .auto-style16 {
            width: 220px;
        }
    </style>
    <link href="CSS/Q.css" rel="stylesheet" type="text/css" />

    <table style="width: 100%">
        <tr>
            <td class="line_at_bottom">
                <table class="auto-style1">
                    <tr>
                        <td class="style25">
                            <table class="style15">
                                <tr>
                                    <td class="style24">
                                        <asp:RadioButton ID="rbnSearchCustomer" runat="server" AutoPostBack="True"
                                            Checked="True" GroupName="search" Text="Search Customer" oncheckedchanged="rbnSearchCustomer_CheckedChanged"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style24">
                                        <asp:RadioButton ID="rbnSearchInstruction" runat="server" AutoPostBack="True"
                                            GroupName="search" Text="Search Instruction" oncheckedchanged="rbnSearchInstruction_CheckedChanged"/>
                                    </td>
                                </tr>
                            </table>
                            &nbsp;</td>
                        <td class="style23">
                            <asp:DropDownList ID="drpSearchBy" runat="server" Width="150px">
                            </asp:DropDownList>

                        </td>
                        <td class="style24">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="txt"></asp:TextBox>
                        </td>
                        <td class="style24">
                            <table class="style15">
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbnUnProcessed" runat="server" Checked="True"
                                            GroupName="process" Text="Not Submitted" AutoPostBack="True" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Label ID="lblFrom" runat="server" Text="From" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="dtmFrom" runat="server" Visible="False"></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender1" runat="server"
                                            Format="dd-MMM-yyyy"
                                            TargetControlID="dtmFrom" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                    <td rowspan="2">
                                        <asp:Button ID="btnView" OnClick="btnView_Click" runat="server" Text="View" Width="96px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rbnProcessed" runat="server" Text="Submitted"
                                            AutoPostBack="True" GroupName="process" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>
                                        <asp:Label ID="lblTo" runat="server" Text="To" Visible="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="dtmTo" runat="server" Visible="False"></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender2" runat="server"
                                            Format="dd-MMM-yyyy"
                                            TargetControlID="dtmTo" />
                                    </td>
                                    <td>&nbsp;</td>
                                    <td>&nbsp;</td>
                                </tr>

                            </table>

                        </td>
                        <td class="style27">&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                     <td>
                        <asp:Button runat="server" ID="btnOpenAccount" Text="Open Account" OnClick="btnOpenAccount_Click"/>
                    </td>
                   </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td class="style18">
                <asp:MultiView ID="MultiView1" runat="server">
                    <asp:View ID="ViewCustomerList" runat="server">
                        <asp:GridView ID="dgvClients" runat="server" BackColor="White"
                            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowCommand="dgvClients_RowCommand"
                            GridLines="Vertical" Width="100%" AutoGenerateColumns="False">
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <Columns>
                                <asp:CommandField ShowSelectButton="True" />
                                <asp:BoundField DataField="Client_Name" HeaderText="Client Name" />
                                <asp:BoundField DataField="Client_Customer_Number"
                                    HeaderText="Client Customer Number" />
                                <asp:BoundField DataField="RM_Name" HeaderText="Relationship Manager" />
                                <asp:BoundField DataField="Client_ID">
                                    <ItemStyle BorderStyle="None" Font-Size="0pt" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                <asp:BoundField DataField="Client_Account_Number"
                                    HeaderText="Client Account Number" />
                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                        </asp:GridView>
                        <asp:GridView ID="dgvInstructions" runat="server" AutoGenerateColumns="False"
                            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px"
                            CellPadding="3" GridLines="Vertical" Width="100%">
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
                                <asp:BoundField DataField="picked_by_name" HeaderText="Picked by" />
                                <asp:BoundField DataField="branch_name" HeaderText="Branch Name" />
                                <asp:BoundField DataField="branch_user" HeaderText="Originator" />
                                <asp:BoundField DataField="branch_approved_by_user"
                                    HeaderText="Originator TL" />
                                <asp:BoundField DataField="ftroa_user" HeaderText="PRMO" />
                                <asp:BoundField DataField="ftrob_user" HeaderText="PRMO (TL)" />
                                <asp:BoundField DataField="prmu_manager_proccessed_by_name"
                                    HeaderText="PRMU Manager" Visible="False" />
                                <asp:BoundField DataField="processor_user" HeaderText="Pay Officer" />
                                <asp:BoundField DataField="processor_approved_by_name"
                                    HeaderText="Pay Officer (TL)" />
                                <asp:BoundField DataField="prmu_manager_proccessed_by_name"
                                    HeaderText="Validation Officer" />
                                <asp:BoundField DataField="validation_manager"
                                    HeaderText="Validation Manager" />
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
                    </asp:View>

                    <asp:View ID="ViewInstructions" runat="server">
                        <table width="100%">
                            <tr>
                                <td class="separator" valign="top" colspan="5">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style15" valign="top">Customer Details<asp:TextBox ID="txtClientID" runat="server" CssClass="txt"
                                    Width="16px" Visible="False"></asp:TextBox>
                                </td>
                                <td rowspan="2" width="2px">&nbsp;</td>
                                <td id="tdDocument" runat="server" class="h2">
                                    <asp:Label ID="lblLockedBy" runat="server"></asp:Label>
                                    &nbsp;
                                    <asp:Label ID="lblLockedDate" runat="server" Text="lblLockedDate" Visible="false" Enabled="false"></asp:Label>
                                </td>
                                <td runat="server">
                                    <asp:FileUpload ID="FileUpload2" runat="server"
                                        Width="227px" />
                                </td>
                                <td id="tdDocument2" runat="server" class="style26">
                                    <table>
                                        <tr>
                                            <td class="align_right" width="140px">&nbsp;</td>
                                            <td class="align_right" width="140px">
                                                <asp:Button ID="btnAdd" runat="server" Style="height: 26px"
                                                    Text="Add Attachment" Width="140px" />
                                            </td>
                                            <td class="align_right" width="160PX">
                                                <asp:DropDownList ID="drpAttachments" runat="server" AutoPostBack="True"
                                                    Width="150px" Visible ="false" Enabled="false">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnOpen" runat="server" Text="Open" Visible="false" enabled="false"/>
                                                &nbsp;
                                            </td>
                                            <td>
                                                <asp:Button ID="btnDeleteAttachment" runat="server" Text="Delete" Visible="false" Enabled="false"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="trInstructionDetails" runat="server">
                                <td style = "vertical-align: top; " class="auto-style16">
                                    <table>
                                        <tr>
                                            <td class="auto-style14">Cust Name</td>
                                            <td>
                                                <asp:TextBox ID="txtClient_Name" runat="server" CssClass="txt" ReadOnly="True"
                                                    Font-Bold="True"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Cust No.</td>
                                            <td>
                                                <asp:TextBox ID="txtClient_Customer_Number" runat="server" CssClass="txt"
                                                    ReadOnly="True" Font-Bold="True"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">RM/RO</td>
                                            <td>
                                                <asp:DropDownList ID="drpRM" runat="server" CssClass="drp" Enabled="False">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="h2" colspan="2">Instruction Details<asp:Button ID="btnNew" runat="server" OnClick="btnNew_Click"
                                                Text="New Instruction" Visible="False" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <table class="">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkAllowDuplicates" runat="server" Text="Allow Duplicate" />
                                                        </td>
                                                        <td class="align_left">
                                                            <asp:CheckBox ID="chkProcessAtBranch" runat="server" Text="Process(ed) at Branch"
                                                                AutoPostBack="True" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Account</td>
                                            <asp:Label ID="Label3" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>

                                            <td>
                                                <asp:DropDownList ID="drpAccount" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Instruction</td>
                                            <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>

                                            <td>
                                                <asp:DropDownList ID="drpInstructions" runat="server" 
                                                    CssClass="drp" AutoPostBack="True" >

                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr id="trCurrency" runat="server">
                                            <td class="auto-style14">Currency
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpCurrency" runat="server" CssClass="drp">
                                                </asp:DropDownList>
                                            </td>

                                        </tr>
                                        <tr id="trCrossCurrency" runat="server">
                                            <td class="auto-style14">Cross Currency
                                            <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>

                                            </td>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td class="style20">
                                                            <asp:DropDownList ID="drpCrossCurrency" runat="server" Width="63px">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem Value="2">No</asp:ListItem>
                                                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                                            </asp:DropDownList>

                                                        </td>
                                                        <td class="auto-style3">Rate</td>
                                                        <td>
                                                            <asp:TextBox ID="txtCCRate" runat="server" Width="52px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Origin Branch
                                            <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>

                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpBranchs" runat="server" CssClass="drp" Width="198px">
                                                </asp:DropDownList>
                                               

                                            </td>
                                        </tr>
                                        <tr id="trAmount" runat="server">
                                            <td class="auto-style14">Amount
                                            <asp:Label ID="Label8" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="txt"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Delivery Date
                                            <asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                            <td style="width: 300px">
                                                <asp:TextBox ID="dtmDeliveryTime" runat="server" Width="86px" AutoPostBack="true"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender3" runat="server"
                                                    Format="dd-MMM-yyyy"
                                                    TargetControlID="dtmDeliveryTime"  />

                                                <asp:DropDownList ID="drpDeliveryHour" runat="server">
                                                    <asp:ListItem>H</asp:ListItem>
                                                    <asp:ListItem>08</asp:ListItem>
                                                    <asp:ListItem>09</asp:ListItem>
                                                    <asp:ListItem>10</asp:ListItem>
                                                    <asp:ListItem>11</asp:ListItem>
                                                    <asp:ListItem>12</asp:ListItem>
                                                    <asp:ListItem>01</asp:ListItem>
                                                    <asp:ListItem>02</asp:ListItem>
                                                    <asp:ListItem>03</asp:ListItem>
                                                    <asp:ListItem>04</asp:ListItem>
                                                    <asp:ListItem>05</asp:ListItem>
                                                    <asp:ListItem>06</asp:ListItem>
                                                    <asp:ListItem>07</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="drpDeliveryMinute" runat="server">
                                                    <asp:ListItem>M</asp:ListItem>
                                                    <asp:ListItem>00</asp:ListItem>
                                                    <asp:ListItem>01</asp:ListItem>
                                                    <asp:ListItem>02</asp:ListItem>
                                                    <asp:ListItem>03</asp:ListItem>
                                                    <asp:ListItem>04</asp:ListItem>
                                                    <asp:ListItem>05</asp:ListItem>
                                                    <asp:ListItem>06</asp:ListItem>
                                                    <asp:ListItem>07</asp:ListItem>
                                                    <asp:ListItem>08</asp:ListItem>
                                                    <asp:ListItem>09</asp:ListItem>
                                                    <asp:ListItem>10</asp:ListItem>
                                                    <asp:ListItem>11</asp:ListItem>
                                                    <asp:ListItem>12</asp:ListItem>
                                                    <asp:ListItem>13</asp:ListItem>
                                                    <asp:ListItem>14</asp:ListItem>
                                                    <asp:ListItem>15</asp:ListItem>
                                                    <asp:ListItem>16</asp:ListItem>
                                                    <asp:ListItem>17</asp:ListItem>
                                                    <asp:ListItem>18</asp:ListItem>
                                                    <asp:ListItem>19</asp:ListItem>
                                                    <asp:ListItem>20</asp:ListItem>
                                                    <asp:ListItem>21</asp:ListItem>
                                                    <asp:ListItem>22</asp:ListItem>
                                                    <asp:ListItem>23</asp:ListItem>
                                                    <asp:ListItem>24</asp:ListItem>
                                                    <asp:ListItem>25</asp:ListItem>
                                                    <asp:ListItem>26</asp:ListItem>
                                                    <asp:ListItem>27</asp:ListItem>
                                                    <asp:ListItem>28</asp:ListItem>
                                                    <asp:ListItem>29</asp:ListItem>
                                                    <asp:ListItem>30</asp:ListItem>
                                                    <asp:ListItem>31</asp:ListItem>
                                                    <asp:ListItem>32</asp:ListItem>
                                                    <asp:ListItem>33</asp:ListItem>
                                                    <asp:ListItem>34</asp:ListItem>
                                                    <asp:ListItem>35</asp:ListItem>
                                                    <asp:ListItem>36</asp:ListItem>
                                                    <asp:ListItem>37</asp:ListItem>
                                                    <asp:ListItem>38</asp:ListItem>
                                                    <asp:ListItem>39</asp:ListItem>
                                                    <asp:ListItem>40</asp:ListItem>
                                                    <asp:ListItem>41</asp:ListItem>
                                                    <asp:ListItem>42</asp:ListItem>
                                                    <asp:ListItem>43</asp:ListItem>
                                                    <asp:ListItem>44</asp:ListItem>
                                                    <asp:ListItem>45</asp:ListItem>
                                                    <asp:ListItem>46</asp:ListItem>
                                                    <asp:ListItem>47</asp:ListItem>
                                                    <asp:ListItem>48</asp:ListItem>
                                                    <asp:ListItem>49</asp:ListItem>
                                                    <asp:ListItem>50</asp:ListItem>
                                                    <asp:ListItem>51</asp:ListItem>
                                                    <asp:ListItem>52</asp:ListItem>
                                                    <asp:ListItem>53</asp:ListItem>
                                                    <asp:ListItem>54</asp:ListItem>
                                                    <asp:ListItem>55</asp:ListItem>
                                                    <asp:ListItem>56</asp:ListItem>
                                                    <asp:ListItem>57</asp:ListItem>
                                                    <asp:ListItem>58</asp:ListItem>
                                                    <asp:ListItem>59</asp:ListItem>
                                                    <asp:ListItem>60</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:DropDownList ID="drpAmPms" runat="server">
                                                    <asp:ListItem>M</asp:ListItem>
                                                    <asp:ListItem>AM</asp:ListItem>
                                                    <asp:ListItem>PM</asp:ListItem>
                                                </asp:DropDownList>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Comments
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                                <td>
                                                <asp:TextBox ID="txtComments" runat="server" CssClass="txt" BackColor="#FFFFCC"
                                                    Rows="3" TextMode="MultiLine"></asp:TextBox>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="auto-style14">Reference
                                            <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                            </td>
                                                <td>
                                                <asp:TextBox ID="txtTransactionReference" runat="server" CssClass="txt"
                                                    ReadOnly="True" Width="126px"></asp:TextBox>

                                                &nbsp;
                                                <asp:Button ID="btnGenerate" runat="server" Text="Get Ref" Width="68px" onclick="btnGenerate_Click"/>
                                            </td>
                                        </tr>
<%--                                        <tr>
                                            <td class="auto-style12">Related Ref</td>
                                            <td>
                                                <asp:TextBox ID="txtRelatedTransactionReference" runat="server" CssClass="txt"
                                                    ReadOnly="True" Width="126px"></asp:TextBox>
                                                &nbsp;
                                                 <asp:Button ID="btnGenerate0" runat="server" Enabled="False" Text="View" Width="55px" />
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td class="auto-style14">EE&nbsp; Ref</td>
                                            <td>
                                                <asp:TextBox ID="txtFTRef" runat="server" BackColor="#FFFFCC" CssClass="txt"
                                                    ReadOnly="True" Rows="1"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                &nbsp;</td>
                                        </tr>
                                 
                                        
                                        <tr>
                                        <td class="auto-style14" >
                                            <asp:TextBox ID="txtStatus" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="37px"></asp:TextBox>
<%--                                            <asp:TextBox ID="txtInstructionStatus" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="50px"></asp:TextBox>--%>
                                            <asp:TextBox ID="txtInstructionID" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="37px"></asp:TextBox>
                                            <asp:TextBox ID="txtDocumentStatusID" runat="server" CssClass="txt" ReadOnly="True" Visible="False" Width="37px"></asp:TextBox>
                                       </td>
                                         </tr>   
                                            <tr>
                                                <td class="line_on_top" colspan="2">
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
                                                                <asp:FileUpload ID="supportingDocUpload" runat="server" Width="327px" visible="false"/>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="btnSupportingAttach" runat="server" OnClick="btnSupportingAttach_Click" Text="Attach" Width="120px" Visible="false" />
                                                                <asp:TextBox ID="txtSupportingFileName" runat="server" CssClass="txt" ReadOnly="true" Visible="false" Width="120px" ></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="txtSupportingFilePath" runat="server" CssClass="txt" ReadOnly="true" Visible="false" Width="120px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblPDFDoc">PDF Document</asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="auto-style11">
                                                                 <%--CssClass="fill_width"--%>
                                                                <asp:FileUpload ID="FileUpload1" runat="server" Width="327px" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="auto-style11">
                                                                 <asp:Button ID="btnAttach" runat="server" OnClick="btnAttach_Click" Text="Attach" Width="120px" />
                                                                 <asp:TextBox ID="txtFileName" runat="server" CssClass="txt" ReadOnly="True" Visible="True" Width="120px"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                       
                                                    </table>
                                                </td>

                                            </tr>

                                            <tr>
                                                <td class="auto-style8" colspan="2">Comments</td>
                                            </tr>
                                            <tr>
                                                <td class="auto-style14">Action
                                                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="drpValidationStatus" runat="server" CssClass="drp" OnSelectedIndexChanged="drpValidationStatus_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="auto-style14">DOC
                                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red" Text="*"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="drpDOC" runat="server" AutoPostBack="True" CssClass="drp">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
<%--                                            <tr>
                                                <td class="auto-style14">Call Back</td>
                                                <td>
                                                    <asp:TextBox ID="txtCallBackComment" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="auto-style14">Call Back No</td>
                                                <td>
                                                    <asp:TextBox ID="txtCallBackNos" runat="server" CssClass="txt" ReadOnly="True"></asp:TextBox>
                                                </td>
                                            </tr>--%>
                                          <%--  <tr id="trPRMO1Comments" runat="server">
                                                <td class="auto-style14">PRMO 1</td>
                                                <td>
                                                    <asp:TextBox ID="txtPRMO1Comments" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr id="trPRMO2Comments" runat="server">
                                                <td class="auto-style14">PRMO 2</td>
                                                <td>
                                                    <asp:TextBox ID="txtPRMO2Comments" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr id="trPRMOTLComments" runat="server">
                                                <td class="auto-style14">PRMO TL</td>
                                                <td>
                                                    <asp:TextBox ID="txtPRMOTLComments" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>--%>
<%--                                            <tr id="trProcessorComments" runat="server">
                                                <td class="auto-style14">Processor</td>
                                                <td>
                                                    <asp:TextBox ID="txtProcessorComments" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr id="trRMComments" runat="server">
                                                <td class="auto-style14">RM/RO</td>
                                                <td>
                                                    <asp:TextBox ID="txtRMComments" runat="server" BackColor="#FFFFCC" CssClass="txt" ReadOnly="True" Rows="1" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr> --%>
                                            <tr id="trOtherComments1" runat="server">
                                                <td class="h2" colspan="2">Other Comments</td>
                                            </tr>
                                            <tr id="trOtherComments2" runat="server">
                                                <td colspan="2">
                                                    <asp:GridView ID="dgvComments" runat="server" AutoGenerateColumns="False" GridLines="Horizontal" Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="instruction_comment" HeaderText="Other Comments" />
                                                            <asp:BoundField DataField="instruction_comments_type" HeaderText="Status" Visible="False" />
                                                            <asp:BoundField DataField="instruction_comment_by_name" HeaderText="By">
                                                            <ItemStyle Width="80px" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="instruction_comment_date" HeaderText="Date" Visible="False">
                                                            <ItemStyle Width="20px" />
                                                            </asp:BoundField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="line_on_top" colspan="2">
                                                    <asp:Button ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="Cancel" />
                                                    &nbsp;<asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click1" Text="Save" Visible="false" />
                                                    &nbsp;<asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" Visible="False" Width="140px" />
                                                </td>
                                            </tr>
                                        
                                    </table>
                                </td>
                                <td style="vertical-align: top" colspan="3">
                                    <cc1:ShowPdf ID="ShowPdf1" runat="server" FilePath="Images/logo.png" Height="650px"
                                        Width="100%" />
                                 
                                    <asp:GridView runat="server" ID="excelGridView" Visible="false" BorderWidth="1px" Width="100%" GridLines="Horizontal" BackColor="White">
                                        <HeaderStyle BackColor=" #000084" Font-Bold="true" ForeColor="White"/>
                                    </asp:GridView>
                                   
                                    <asp:Button ID="downloadButton" runat="server" OnClick="downloadButton_Click" Text="Download" Visible="false" Width="76px" />
                                    
     
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




