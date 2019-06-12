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
        <div class="active">
            <asp:Button ID="ButtonAddNewRow" runat="server" Text="Add New Row" OnClick="ButtonAddNewRow_Click" />
            <asp:Button ID="ButtonAddNewColumn" runat="server" Text="Add New Column" OnClick="ButtonAddNewColumn_Click" />
            <asp:Button ID="SubmitNewRow" runat="server" Text="Add" OnClick="SubmitNewRow_Click" Visible="false"/>
            <asp:Button ID="CancelAddRow" runat="server" Text="Cancel" OnClick="CancelAddRow_Click" Visible="false" />
            <asp:Button ID="SubmitNewColumn" runat="server" Text="Add" OnClick="SubmitNewColumn_Click" Visible="false"/>
            <asp:Button ID="CancelAddColumn" runat="server" Text="Cancel" OnClick="CancelAddColumn_Click" Visible="false" />
            <div>                
                <asp:Panel ID="PanelAddNewRow" runat="server" Visible="false"></asp:Panel>
                <asp:Panel ID="PanelAddNewColumn" runat="server" Visible="false">
                    <asp:TextBox ID="TextBoxNewColumnName" runat="server"></asp:TextBox>
                    <asp:DropDownList ID="DropDownListNewColumnTypes" runat="server">
                        <asp:ListItem>number</asp:ListItem>
                        <asp:ListItem>string</asp:ListItem>
                        <asp:ListItem>date</asp:ListItem>
                        <asp:ListItem>image</asp:ListItem>
                    </asp:DropDownList>
                </asp:Panel>          
            </div>
        </div>
        <asp:SqlDataSource ID="DataSource" runat="server"
            ConflictDetection="CompareAllValues"
            ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
            OldValuesParameterFormatString="original_{0}"
            SelectCommand="SELECT * FROM [DefaultTable]"
            DeleteCommand="DELETE FROM [DefaultTable] WHERE [Id] = @original_Id AND [GenericNotNull] = @original_GenericNotNull AND (([GenericNull] = @original_GenericNull) OR ([GenericNull] IS NULL AND @original_GenericNull IS NULL))"
            UpdateCommand="UPDATE [DefaultTable] SET [GenericNotNull] = @GenericNotNull, [GenericNull] = @GenericNull WHERE [Id] = @original_Id AND [GenericNotNull] = @original_GenericNotNull AND (([GenericNull] = @original_GenericNull) OR ([GenericNull] IS NULL AND @original_GenericNull IS NULL))" InsertCommand="INSERT INTO [DefaultTable] ([GenericNotNull], [GenericNull]) VALUES (@GenericNotNull, @GenericNull)">
            <DeleteParameters>
                <asp:Parameter Name="original_Id" Type="Int32" />
                <asp:Parameter Name="original_GenericNotNull" Type="String" />
                <asp:Parameter Name="original_GenericNull" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="GenericNotNull" Type="String" />
                <asp:Parameter Name="GenericNull" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="GenericNotNull" Type="String" />
                <asp:Parameter Name="GenericNull" Type="String" />
                <asp:Parameter Name="original_Id" Type="Int32" />
                <asp:Parameter Name="original_GenericNotNull" Type="String" />
                <asp:Parameter Name="original_GenericNull" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:GridView ID="DataGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="Id" DataSourceID="DataSource" ShowHeaderWhenEmpty="True">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" InsertVisible="False" ReadOnly="True" SortExpression="Id" />
                <asp:BoundField DataField="GenericNotNull" HeaderText="GenericNotNull" SortExpression="GenericNotNull" />
                <asp:BoundField DataField="GenericNull" HeaderText="GenericNull" SortExpression="GenericNull" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
