<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataManager.aspx.cs" Inherits="DynamicDb.Pages.DataManager" %>

<!DOCTYPE html>

<%@ Register Src="~/Controls/NavBar.ascx" TagPrefix="uc" TagName="NavBar"%>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DataManager</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
</head>
<body>
    <uc:NavBar runat="server" id="NavBar" />

    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
