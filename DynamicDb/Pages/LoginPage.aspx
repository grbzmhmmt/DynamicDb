<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="DynamicDb.Pages.LoginPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Page</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>


</head>
<body class="text-center">
    <form class="form-signin" runat="server">
        <div class="offset-4 col-4">
            <h1 class="h3 mb-3 font-weight-normal">Please sign in</h1>
            <br />
            <label for="inputEmail" class="sr-only">User Name</label>
            <asp:TextBox CssClass="form-control" ID="txtLoginUserName" placeholder="UserName" runat="server" required autofocus></asp:TextBox>
            <br />
            <label for="inputPassword" class="sr-only">Password</label>
            <br />
            <asp:TextBox CssClass="form-control" ID="txtLoginPassword" placeholder="Password" runat="server" required></asp:TextBox>
            <br />
            <asp:Button CssClass="btn btn-lg col-4 btn-primary btn-block" style="float:right" ID="btnLoginSubmit" runat="server" OnClick="LoginSystem" Text="Login" Width="88px" />

            <p class="mt-5 mb-3 text-muted">&copy; 2017-2019</p>
        </div>
    </form>
</body>
</html>
