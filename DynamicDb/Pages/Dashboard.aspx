<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="DynamicDb.Pages.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">

                <div class="col-4 bg-success" style="height:100px">

                </div>

                <div class="col-4 bg-danger"style="height:100px" >

                    <br />

                    <asp:HyperLink CssClass="btn btn-sm col-4 btn-primary" style="float:right" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateDbLink" runat="server" Text="Create Database" Width="88px" />
                    <br />
                    <asp:HyperLink CssClass="btn btn-sm col-4 btn-success" style="float:right" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateTableLink" runat="server" Text="Create Table" Width="88px" />
                    <br />
                    <asp:HyperLink CssClass="btn btn-sm col-4 btn-warning" style="float:right" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnEditTableLink" runat="server" Text="Edit Table" Width="88px" />
                    <br />
                    <asp:HyperLink CssClass="btn btn-sm col-4 btn-secondary" style="float:right" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnMyTablesLink" runat="server" Text="My Tables" Width="88px" />


                </div>

                <div class="col-4 bg-warning"style="height:100px">
                    Charts
                </div>

            </div>
        </div>
    </form>
</body>
</html>
