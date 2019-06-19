<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegisterPage.aspx.cs" Inherits="DynamicDb.Pages.RegisterPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Register Page</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
    <script type="text/javascript">
        function toRedirect(page) {
            window.location.href = page;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="offset-4 col-4">
            <h1 class="h3 mb-3 font-weight-normal">Tüm Alanları Doldurunuz.</h1>
            <br />
            <label for="inputEmail" class="sr-only">User Name</label>
            <asp:TextBox CssClass="form-control" ID="txtRegisterUserName" placeholder="UserName" runat="server" required="required" autofocus="autofocus"></asp:TextBox>
            <br />
            <label for="inputPassword" class="sr-only">Password</label>
            <br />
            <asp:TextBox CssClass="form-control" ID="txtRegisterPassword" placeholder="Password" runat="server" required="required"></asp:TextBox>
            <br />
            <asp:Button CssClass="btn btn-lg col-4 btn-primary btn-block" style="float:left" ID="BtnRegisterSubmit" runat="server" OnClick="RegisterSystem" Text="Register" Width="88px" />
            <p class="mt-5 mb-3 text-muted">&copy; 2017-2019</p>
        </div>
    </form>
</body>
</html>
