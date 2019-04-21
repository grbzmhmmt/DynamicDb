<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomForm.aspx.cs" Inherits="DynamicDb.Pages.CustomForm.CustomForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Data Input Form</title>
    <link type="text/css" href="CustomForm.css" rel="stylesheet" />
</head>
<body class="app-body">
    <div class="app-page">
        <div class="app-menu">
            <div class="app-menu-item">Dosya</div>
            <div class="app-menu-item">Düzenle</div>
            <div class="app-menu-item">Ekle</div>
        </div>
        <div class="app-toolbar">
            <div class="app-toolbar-item">Geri Al</div>
            <div class="app-toolbar-item">İleri Al</div>
            <div class="app-toolbar-item">Hücre Türü</div>
        </div>
        <div class="app-cell-input" contenteditable="true"></div>
        <div class="app-table">
            <%--<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" DataKeyNames="CustomerId"
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
                    <asp:TemplateField HeaderText="Country" ItemStyle-Width="150">
                        <ItemTemplate>
                            <asp:Label ID="lblCountry" runat="server" Text='<%# Eval("Country") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCountry" runat="server" Text='<%# Eval("Country") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ButtonType="Link" ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="150" />
                </Columns>
            </asp:GridView>--%>
        </div>
    </div>
</body>
</html>
