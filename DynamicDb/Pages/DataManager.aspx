<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataManager.aspx.cs" Inherits="DynamicDb.Pages.DataManager" %>

<!DOCTYPE html>

<%@ Register Src="~/Controls/NavBar.ascx" TagPrefix="uc" TagName="NavBar" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Manager</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
</head>
<body>
    <uc:NavBar runat="server" ID="NavBar" />

    <form id="form1" runat="server">
        <asp:TextBox CssClass="col-sm-4" ID="TextBoxTableName" placeholder="Table Name" runat="server"></asp:TextBox>
        <asp:Button CssClass="btn btn-info btn-sm m-1" ID="ButtonEditTable" runat="server" OnClick="ButtonEditTable_Click" Text="Create" Width="88px" />
        
        <asp:GridView ID="DataEditGridView" runat="server" AutoGenerateColumns="false"
            OnRowDataBound="OnRowDataBound" OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit"
            OnRowUpdating="OnRowUpdating" OnRowDeleting="OnRowDeleting" EmptyDataText="No records has been added.">
        <Columns>
            <asp:TemplateField HeaderText="Name" ItemStyle-Width="150">
                <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtName" runat="server" Text='<%# Eval("Name") %>'></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ButtonType="Link" ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="150" />
        </Columns>
    </asp:GridView>
    <table border="1" style="border-collapse: collapse">
        <tr>
            <td style="width: 150px">
                Name:<br />
                <asp:TextBox ID="txtName" runat="server" Width="140" />
            </td>
            <td style="width: 150px">
                Country:<br />
                <asp:TextBox ID="txtCountry" runat="server" Width="140" />
            </td>
            <td style="width: 100px">
                <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="Insert" />
            </td>
        </tr>
    </table>
    </form>

    <%--<asp:GridView runat="server" ID="GridView"  AutoGenerateColumns="false"
        DataKeyNames="<%# Eval("userPrimaryKey") %>"
        OnRowDataBound="OnRowDataBound"
        OnRowEditing="OnRowEditing"
        OnRowCancelingEdit="OnRowCancelingEdit"
        OnRowUpdating="OnRowUpdating" 
        OnRowDeleting="OnRowDeleting" 
        EmptyDataText="Veri yok.">
        <columns>
          <asp:boundfield datafield="CustomerID" headertext="Customer ID"/>
          <asp:boundfield datafield="CompanyName" headertext="Company Name"/>
          <asp:boundfield datafield="Address" headertext="Address"/>
          <asp:boundfield datafield="City" headertext="City"/>
          <asp:boundfield datafield="PostalCode" headertext="Postal Code"/>
          <asp:boundfield datafield="Country" headertext="Country"/>
        </columns>
    </asp:GridView>--%>
</body>
</html>
