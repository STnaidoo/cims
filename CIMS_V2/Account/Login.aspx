<%@ Page Title="Log in" Language="C#" AutoEventWireup="false" CodeBehind="Login.aspx.cs" Inherits="CIMS_V2.Account.Login" Async="true" %>

<%--<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>--%>

<!DOCTYPE html>
<html>

<head runat="server">
    <title>Log In</title>
    <link href="../Content/bootstrap.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .auto-style1 {
            position: relative;
            min-height: 1px;
            float: left;
            width: 63%;
            left: 0px;
            top: 25px;
            height: 377px;
            padding-left: 15px;
            padding-right: 15px;
        }
        .auto-style2 {
            margin-left: 40px;
        }
        .auto-style3 {
            margin-bottom: 15px;
            margin-left: 25px;
        }
        .auto-style4 {
            margin-left: 80px;
        }

        pic { 

     background-size: cover;
}
    </style>
</head>
<body>

    <div class="navbar navbar-inverse navbar-fixed-top" style="left: 0; background: radial-gradient(at top left, cornflowerblue, darkblue); right: 0; top: 0; height: 100px; margin-bottom: 0px;">
        <asp:Image ID="logo" runat="server" Height="90px" Width="350px" ImageAlign="Left" ImageUrl="~/images/logo2.png" />
    </div>

<%--    <div <%--style="background-image: url(/images/kenya.jpg); height: 800px; width: 600px; background-size: contain";--%> >
<%--      <asp:Image ID="Image2" runat="server" Height="100%" Width="100%" ImageAlign="Middle" ImageUrl="~/images/kenya.jpg" CssClass="pic"/>
    </div>--%>

    <div style="width: 500px; margin: 200px auto 0 auto;">
    <form id="login_form" runat="server">
<%--        <h2 class="auto-style2">Login</h2>--%>
        <div class="row">
            <div class="auto-style1">
                <section id="loginForm">
                    <div class="form-horizontal">
                        <h3 class="auto-style2">Welcome to CIMS</h3>
                        <hr />
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtUserName">User Name</asp:Label>
                            <div>
<%--                                <input type="text" placeholder="User Name" />--%>
                                <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control"  Width="300px"/>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUserName"
                                    CssClass="text-danger" ErrorMessage="The Username field is required." />
                            </div>
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtPassword">Password</asp:Label>
                            <div>
                                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" Width="300px" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="text-danger" ErrorMessage="The password field is required." />
                            </div>
                        </div>
                        <%-- <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <div class="checkbox">
                                <asp:CheckBox runat="server" ID="RememberMe" />
                                <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                            </div>
                        </div>
                    </div>--%>
                        <div>
                            <div>
                                <asp:Button runat="server" OnClick="LogIn" Text="Log in" CssClass="btn btn-default" Width="300px" BackColor="LightGreen"/>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Label ID="lblError" runat="server"></asp:Label>
                            </div>
                        </div>

                    </div>
        <div>
            <a href="Forgot.aspx">Forgot Password</a>
        </div>
                </section>
            </div>

        </div>


    </form>
    </div>
</body>
</html>