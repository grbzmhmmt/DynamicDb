<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="DynamicDb.Pages.Dashboard" %>

<!DOCTYPE html>

<<<<<<< HEAD
<%@ Register Src="~/Controls/NavBar.ascx" TagPrefix="uc" TagName="NavBar"%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
</head>
<body>
    <uc:NavBar runat="server" id="NavBar1" />
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
=======
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

>>>>>>> master
                <div class="col-4 bg-success">
                </div>

                <div class="col-4 bg-danger">
                    <div class="row">
                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-primary offset-2 mt-1 mb-1"  NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateDbLink" runat="server" Text="Create Database" />
<<<<<<< HEAD
                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-success offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateTableLink" runat="server" Text="Create Table"/>
                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-warning offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnEditTableLink" runat="server" Text="Edit Table" />
                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-secondary offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnMyTablesLink" runat="server" Text="My Tables" />
                    </div>
=======

                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-success offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateTableLink" runat="server" Text="Create Table"/>

                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-warning offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnEditTableLink" runat="server" Text="Edit Table" />

                        <asp:HyperLink CssClass="btn btn-sm col-8 btn-secondary offset-2 mt-1 mb-1" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnMyTablesLink" runat="server" Text="My Tables" />

                    </div>

>>>>>>> master
                </div>

                <div class="col-4 bg-warning">
                    Charts
                </div>
<<<<<<< HEAD
=======

>>>>>>> master
            </div>
        </div>
    </form>
</body>
</html>
