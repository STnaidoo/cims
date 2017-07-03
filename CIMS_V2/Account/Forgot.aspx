<%@ Page Title="Forgot password" Language="C#"  AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="CIMS_V2.Account.Forgot" Async="true" %>

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
    <form runat="server" id="forgotPasswordForm">
    <div class="row">
        <div class="col-md-8">
            <asp:PlaceHolder id="loginForm" runat="server">
                <div class="form-horizontal">
                    <h4>Forgot your password?</h4>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">User Name</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" Width="300px" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The user name field is required." />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button runat="server" OnClick="ForgotPassword" Text="Email Link" CssClass="btn btn-default" BackColor="LightGreen" Width="300px" />
                        </div>
                       
                        
                    </div>
                    <div class="form-group">
                        <div class= "col-md-10">
                            <a href="Login.aspx" style="width:300px">Login Page</a>

                        </div>

                    </div>
                </div>


            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="DisplayEmail" Visible="false">
                <p class="text-info" style="color:black">
                    Please check your email for your password.
                </p>

            </asp:PlaceHolder>
        </div>

    </div>
        </form>
</body>
</html>
