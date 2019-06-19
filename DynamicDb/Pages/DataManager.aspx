<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataManager.aspx.cs" Inherits="DynamicDb.Pages.DataManager" %>

<!DOCTYPE html>

<%@ Register Src="~/Controls/NavBar.ascx" TagPrefix="uc" TagName="NavBar" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Data Manager</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>

    <style>
        .btnClass {
            background-color: #f8f9fa;
            color: var(--blue);
            border-radius: 6px;
            border-width: thin;
            border-color: var(--blue);
        }
    </style>
</head>
<body>
    <uc:NavBar runat="server" ID="NavBar" />
    <form id="form1" runat="server">
        <div class="container p-0 m-0" style="width: 100%; max-width: 100vw;">
            <div class="row m-0">
                <div class="col-sm-8 alert alert-info">


                    <div class="active">
                        <asp:Button CssClass="btnClass" ID="ButtonAddRow" runat="server" Text="Satir Eklemeyi Başlatın" OnClick="ButtonStartAddRow_Click" />

                        <asp:Button CssClass="btnClass" ID="ButtonAddColumn" runat="server" Text="Sütun Eklemeyi Başlatın" OnClick="ButtonStartAddColumn_Click" />
                        <asp:Button CssClass="btnClass" ID="ButtonDeleteColumn" runat="server" Text="Sütun Silmeyi Başlatın" OnClick="ButtonStartDeleteColumn_Click" />
                        <asp:Button CssClass="btnClass" ID="ButtonChangeColumnName" runat="server" Text="Sütun Adı Değiştirmeyi Başlatın" OnClick="ButtonChangeColumnName_Click" />

                        <asp:Button CssClass="btnClass" ID="ButtonSubmitAddRow" runat="server" Text="Ekle" OnClick="ButtonSubmitAddRow_Click" Visible="false" />
                        <asp:Button CssClass="btnClass" ID="ButtonCancelAddRow" runat="server" Text="İptal" OnClick="ButtonCancelAddRow_Click" Visible="false" />

                        <asp:Button CssClass="btnClass" ID="ButtonSubmitUpdatingRow" runat="server" Text="Güncelle" OnClick="ButtonSubmitUpdatingRow_Click" Visible="false" />
                        <asp:Button CssClass="btnClass" ID="ButtonCancelUpdatingRow" runat="server" Text="İptal" OnClick="ButtonCancelUpdatingRow_Click" Visible="false" />

                        <asp:Button CssClass="btnClass" ID="ButtonSubmitAddColumn" runat="server" Text="Ekle" OnClick="ButtonSubmitAddColumn_Click" Visible="false" />
                        <asp:Button CssClass="btnClass" ID="ButtonCancelAddColumn" runat="server" Text="İptal" OnClick="ButtonCancelAddColumn_Click" Visible="false" />

                        <asp:Button CssClass="btnClass" ID="ButtonSubmitDeleteColumn" runat="server" Text="Sil" OnClick="ButtonSubmitDeleteColumn_Click" Visible="false" />
                        <asp:Button CssClass="btnClass" ID="ButtonCancelDeleteColumn" runat="server" Text="İptal" OnClick="ButtonCancelDeleteColumn_Click" Visible="false" />

                        <asp:Button CssClass="btnClass" ID="ButtonSubmitChangeColumnName" runat="server" Text="Güncelle" OnClick="ButtonSubmitChangeColumnName_Click" Visible="false" />
                        <asp:Button CssClass="btnClass" ID="ButtonCancelChangeColumnName" runat="server" Text="İptal" OnClick="ButtonCancelChangeColumnName_Click" Visible="false" />
                        <br />
                        <hr />
                        <div>
                            <asp:Panel ID="PanelAddRow" runat="server" Visible="false"></asp:Panel>
                            <asp:Panel ID="PanelAddColumn" runat="server" Visible="false">
                                <div>Yeni sütunun adini yaziniz.</div>
                                <asp:TextBox ID="TextBoxOldColumnName" runat="server" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TextBoxNewColumnName" runat="server"></asp:TextBox>
                            </asp:Panel>
                            <asp:Panel ID="PanelDeleteColumn" runat="server" Visible="false">
                                <div>Silmek istediginiz sütunun adini yaziniz.</div>
                                <asp:TextBox ID="TextBoxDeleteColumn" runat="server"></asp:TextBox>
                            </asp:Panel>
                        </div>
                    </div>
                    <asp:SqlDataSource ID="DataSource" runat="server"                        
                        OldValuesParameterFormatString="original_{0}"
                        SelectCommand="SELECT * FROM [DefaultastasGrbzBrky]" ConnectionString="<%$ ConnectionStrings:SqlDynamicDbMyUsersConnectionString %>">
                    </asp:SqlDataSource>
                    <asp:Label ID="lblTableNameText" runat="server" Text="Tablo Adı : "></asp:Label>

                    <asp:Label ID="lblTableName" runat="server" Text="Label"></asp:Label>
                    <br />
                    <asp:GridView ID="DataGridView" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="pkey" DataSourceID="DataSource" ShowHeaderWhenEmpty="True" CellPadding="4" ForeColor="#333333" GridLines="None"
                        OnSelectedIndexChanged="DataGridView_SelectedIndexChanged">
                        <AlternatingRowStyle BackColor="White" />
                        <EditRowStyle BackColor="#2461BF" />
                        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" Font-Bold="True" />
                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        <Columns>
                            <asp:BoundField DataField="pkey" HeaderText="pkey" InsertVisible="False" ReadOnly="True" SortExpression="pkey" />
                            <asp:BoundField DataField="stn0" HeaderText="stn0" SortExpression="stn0" />
                            <asp:BoundField DataField="stn1" HeaderText="stn1" SortExpression="stn1" />
                        </Columns>
                    </asp:GridView>


                </div>
                <div class="col-sm-4 alert alert-dark">

                    <asp:TextBox ID="TextBoxChangeTableName" runat="server" placeholder="Yeni tablo ismi giriniz."></asp:TextBox>
                    <br />
                    <asp:Button CssClass="btnClass" ID="ButtonChangeTableName" runat="server" Text="Tablo Adını Değiştir" OnClick="ButtonChangeTableName_Click" />
                    <hr />
                    <asp:Button CssClass="btnClass" ID="ButtonDeleteTableName" runat="server" Text="Tabloyu Sil" OnClick="ButtonDeleteTableName_Click" />
              
                    
                </div>
            </div>
        </div>
    </form>
</body>
</html>
