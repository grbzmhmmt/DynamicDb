<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavBar.ascx.cs" Inherits="DynamicDb.Controls.NavBar" %>

<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
  <div class="navbar-collapse navbar-nav">
        <asp:HyperLink CssClass="nav-item nav-link" NavigateUrl="~/Pages/Dashboard.aspx" ID="btnCreateDbLink" runat="server" Text="Dashboard" />
        <asp:HyperLink CssClass="nav-item nav-link" NavigateUrl="~/Pages/CreateManager.aspx" ID="btnCreateTableLink" runat="server" Text="Create Table" />
        <asp:HyperLink CssClass="nav-item nav-link" NavigateUrl="~/Pages/DataManager.aspx" ID="btnEditTableLink" runat="server" Text="Edit Table" />
    </div>
</nav>
