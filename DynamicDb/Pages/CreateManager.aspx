<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateManager.aspx.cs" Inherits="DynamicDb.Pages.CreateManager" %>

<%@ Register Src="~/Controls/NavBar.ascx" TagPrefix="uc" TagName="NavBar" %>

<html>
<head>
    <title>Dynamic Database Software</title>
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css">
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <script type="text/javascript">
        var myColumns = [];
        myColumns.push("txtTblColumn0")
        $(window).ready(function () {
            var counter = 1;
            $("#addTblColumn").click(function () {

                if (counter > 10) {
                    alert("Only 10 textboxes allow");
                    return false;
                }

                var newTextBoxDiv = $(document.createElement('div'))
                    .attr("id", 'divTblColumn' + counter);
                newTextBoxDiv.attr("class", 'col-sm-6');

                newTextBoxDiv.after().html(
                    '<input class="col-sm-12" type="text" name="text' + counter +
                    '" id="txtTblColumn' + counter + '" placeholder="Column ' + counter + '">');

                newTextBoxDiv.appendTo("#TextBoxesGroup");

                var Id = "txtTblColumn" + counter;
                myColumns.push(Id);

                counter++;
            });

            $("#rmvTblColumn").click(function () {
                if (counter == -1) {
                    alert("No more textbox to remove");
                    return false;
                }

                counter--;
                $("#divTblColumn" + counter).remove();

            });

            $("#setValuesTbl").click(function () {

                var tblColumnValues = '';
                for (i = 0; i < counter; i++) {

                    tblColumnValues += $('#txtTblColumn' + i).val() + ",";
                    debugger;
                }

                $('#hdnTblColumns').val(tblColumnValues);
                debugger;
                alert(tblColumnValues);

            });
        });
    </script>
    <style>
        div {
            padding: 8px;
        }

        .app-grid {
            z-index: 1;
            width: 80%;
            border: solid 2px black;
            min-width: 80%;
        }

        .grid-header {
            background-color: #646464;
            font-family: Arial;
            color: White;
            border: none 0px transparent;
            height: 25px;
            text-align: center;
            font-size: 16px;
        }

        .grid-rows {
            background-color: #fff;
            font-family: Arial;
            font-size: 14px;
            color: #000;
            min-height: 25px;
            text-align: left;
            border: none 0px transparent;
        }

        .rows:hover {
            background-color: #ff8000;
            font-family: Arial;
            color: #fff;
            text-align: left;
        }
    </style>
</head>
<body>
    <uc:NavBar runat="server" ID="NavBar" />
    <div class="card-header text-center">
        <h1>Dynamic Database Software</h1>
    </div>
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-md-3 bg-danger p-0">
                <h3 class="bg-secondary">Create Database<br />
                </h3>
                <br />
                <asp:TextBox CssClass="col-sm-9" ID="txtDbDataSourceName" placeholder="DataSource Name" runat="server"></asp:TextBox>
                <br />
                <asp:TextBox CssClass="col-sm-9" ID="txtDbDatabaseName" placeholder="Database Name" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button class="btn btn-success btn-sm m-1" type='button' ID='BtnCreateDb' OnClick="BtnCreateDb_Click" Text="Create Database" runat="server" />
            </div>
            <div class="col-md-5 bg-info p-0" style="overflow-y: auto; max-height: 500px">
                <h3 class="bg-secondary text-center">Create Table</h3>
                <div class="col-12">
                    <div class="row">
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblDatabaseName" placeholder="Database Name" runat="server"></asp:TextBox>
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblTableName" placeholder="Table Name" runat="server"></asp:TextBox>
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblPrimaryKeyName" placeholder="Primary Key" runat="server"></asp:TextBox>
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblForeignKeyName" placeholder="Foreign Key" runat="server"></asp:TextBox>
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblForeignKeyReferanceTable" placeholder="ForeingKey Table" runat="server"></asp:TextBox>
                        <asp:TextBox CssClass="col-sm-4" ID="txtTblForeingKeyReferanceColumn" placeholder="ForeignKey Column" runat="server"></asp:TextBox>

                        <button class="btn btn-success btn-sm m-1" type='button' id='addTblColumn'><span class="glyphicon glyphicon-plus" aria-hidden="true"></span>Add</button>
                        <button class="btn btn-danger btn-sm m-1" type='button' id='rmvTblColumn'><span class="glyphicon glyphicon-trash" aria-hidden="true"></span>Remove</button>
                        <asp:Button CssClass="btn btn-info btn-sm m-1" ID="setValuesTbl" runat="server" OnClick="BtnCreateTable_Click" Text="Create" Width="88px" />
                    </div>
                </div>
                <div class="col-12" id='TextBoxesGroup'>
                    <div class="col-sm-6" id="divTblColumn0">
                        <input class="col-12" type='text' id='txtTblColumn0' placeholder="Column 0">
                    </div>
                </div>
                <asp:HiddenField ID="hdnTblColumns" runat="server" />
            </div>
            <div class="col-md-4 bg-warning p-0">
                <h3 class="bg-secondary text-center">Your Tables</h3>
                <div class="row">
                    <div class="col-12">
                        <asp:TextBox CssClass="col-sm-8" ID="txtGetTblDatabaseName" placeholder="Database Name" runat="server"></asp:TextBox>
                        <asp:Button class="btn btn-success btn-sm col-sm-3 m-1 " type='button' ID='BtnGetTables' OnClick="GetTables_Click" Text="Get Tables" runat="server" />
                        <hr />
                        <asp:GridView ID="TablesGridView" runat="server"
                            CssClass="app-grid"
                            HeaderStyle-CssClass="grid-header"
                            RowStyle-CssClass="grid-rows" 
                            AutoGenerateSelectButton="true"
                            OnSelectedIndexChanged="TablesGridRow_Click">
                            <%--OnRowClick="TablesGridRow_Click">--%>
                        </asp:GridView>
                        <%--<asp:ListView ID="tablesListView" runat="server" DataKeyNames="TableName">
                            <ItemTemplate>
                                <p><%#:Item.ProductName%></p>
                            </ItemTemplate>
                        </asp:ListView>--%>
                        <%--  <asp:GridView ID="GridViewTables" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSourceTables" AllowSorting="True">
                            <Columns>
                                <asp:BoundField DataField="Tablolar" HeaderText="Tablolar" SortExpression="Tablolar" />
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="Select"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <asp:SqlDataSource ID="SqlDataSourceTables" runat="server" ConnectionString="<%$ ConnectionStrings:masterConnectionString %>" SelectCommand="SELECT TABLE_NAME AS Tablolar
FROM deneme.INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'"></asp:SqlDataSource>--%>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
