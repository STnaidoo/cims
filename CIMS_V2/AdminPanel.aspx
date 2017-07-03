<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPanel.aspx.cs" Inherits="CIMS_V2.AdminPanel" %>

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
<a href="AdminPanel.aspx">AdminPanel.aspx</a>
        .style25 {
            width: 206px;
        }
        .auto-style2 {
            width: 496px;
        }
        .auto-style3 {
            width: 496px;
            height: 32px;
        }
        .auto-style4 {
            height: 32px;
        }
    </style>

    <div style="width:100%" id="changeViewDiv" runat="server">
        <div style="width:50%; display:table">
        <table id="tblChangeViewButtons">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button Enabled="false" runat="server" ID="btnViewInstructionTypesManagement" Text="Instruction Type Management" Width="300px"  CssClass="btn btn-default" BackColor="LightGreen" OnClick="btnViewInstructionTypesManagement_Click"/>
                </td>
                <td>
                    <asp:Button Enabled="false" runat="server" ID="btnViewUserManagement" Text="User Management" Width="300px"  CssClass="btn btn-default" BackColor="LightGreen" OnClick="btnViewUserManagement_Click"/>
                </td>
                <td>
                    <asp:Button Enabled="false" runat="server" ID="btnViewUserTypesManagement" Text="User Type Management" Width="300px"  CssClass="btn btn-default" BackColor="LightGreen" OnClick="btnViewUserTypesManagement_Click"/>

                </td>

            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
        </div>
    </div>

    <div id="instructionTypesDiv" runat="server">
    <table class="style15" id="tblInstructionTypes">
        <tr>
            <td class="auto-style2">
                <h3>Instruction Type Management</h3>
            </td>
            <td>
                
            </td>
        </tr>
        <tr>
            <td class="auto-style2">
                <asp:CheckBox ID="chkViewInactiveInstructions" AutoPostBack="true" Text="View Inactive Instructions" runat="server" OnCheckedChanged="chkViewInactiveInstructions_CheckedChanged" />
            </td>
        </tr>

        <tr>
            <td class="auto-style2">
                Instructions</td>
            <td>
                <asp:DropDownList runat="server" ID="drpInstructionTypes" AutoPostBack="true" OnSelectedIndexChanged="drpInstructionTypes_SelectedIndexChanged" Width="300px">

                </asp:DropDownList>
            </td>
        </tr>

        <tr>
            <td class="auto-style2">
                Cutoff Time
            </td>
            <td>
                              
                <asp:DropDownList ID="drpCutOffHour" runat="server">
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

                </asp:DropDownList>
                <asp:DropDownList ID="drpCutOffMinute" runat="server">
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
                                                    <asp:ListItem>AM</asp:ListItem>
                                                    <asp:ListItem>PM</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <p>Enable Supporting Documents</p>
            </td>
            <td>
                <asp:DropDownList runat="server" ID="drpSupportingDocumentsEnable" Width="300px"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="auto-style3">
                <p>Instruction Active Status</p>
            </td>
            <td class="auto-style4">
                <asp:DropDownList runat="server" ID="drpActiveStatus" Width="300px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">
            </td>
            <td>
                <asp:Button runat="server" text="Save" ID="SaveButton" OnClick="SaveButton_Click" CssClass="btn btn-default" BackColor="LightGreen" Width="300px"/>
            </td>
        </tr>

    </table>
    </div>  

    <div id="userManagementDiv" runat="server">
    <table class="style15" id="tblUserManagement" runat="server">
        <tr>
            <td class="auto-style2">
                <h3>User Management</h3>
            </td>
            <td>
                
            </td>
        </tr>
        <tr>
            <td>
                <p>Search For User</p>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtSearchUser" Width="300px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                
            </td>
            <td>
                <asp:Button runat="server" ID="btnSearchUser" BackColor="LightGreen" Text="Search" CssClass="btn btn-default" OnClick="btnSearchUser_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <p>Search Results</p>
            </td>
            <td>
                <asp:DropDownList runat="server" ID="drpUserList" Enabled="false" AutoPostBack="true" Width="300px" OnSelectedIndexChanged="drpUserList_SelectedIndexChanged"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <p>User Login</p>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtUserLogin" Width="300px" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <p>User Active</p>
            </td>
            <td>
                <asp:DropDownList runat="server" ID="drpUserActive" Width="300px" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">
            </td>
            <td>
                <asp:Button runat="server" text="Save" ID="btnSaveUser" OnClick="btnSaveUser_Click" CssClass="btn btn-default" BackColor="LightGreen" Width="300px" Enabled="false"/>
            </td>
        </tr>
    </table>
    </div>

    <div id="userTypeManagementDiv" runat="server">
        <table class="style15" id="tblUserTypeManagment">
            <tr>
                <td class="auto-style2">
                    <h3>User Type Management</h3>
                </td>
                <td>

                </td>
            </tr>
            <tr>
                <td>
                <asp:CheckBox ID="chkViewInactiveUserTypes" AutoPostBack="true" Text="View Inactive User Types" runat="server" OnCheckedChanged="chkViewInactiveUserTypes_CheckedChanged"/>
                </td>
            </tr>
            <tr>
                <td>
                    User Type
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="drpUserTypes" Width="300px" OnSelectedIndexChanged="drpUserTypes_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Name
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtUserTypeName" Enabled="false" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Can
                    Get Next
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="drpCanGetNext" Enabled="false" Width="300px"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Can Originate Transactions</td>
                <td>
                    <asp:DropDownList runat="server" ID="drpCanOriginate" Enabled="false" Width="300px"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Visible
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="drpUserTypeVisible" Enabled="false" Width="300px"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>

                </td>
                <td>
                    <asp:Button runat="server" text="Save" ID="btnSaveUserType" OnClick="btnSaveUserType_Click" CssClass="btn btn-default" BackColor="LightGreen" Width="300px"/>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
