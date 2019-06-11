using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;
using System.Configuration;

namespace DynamicDb.Pages
{
    public partial class DataManager : Page
    {
        private string _dataSourceName;
        private string _dataBaseName;
        private string _tableName;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            #region _dataSource degiskenini al
            if (string.IsNullOrEmpty(_dataSourceName) &&
                Context != null &&
                Context.Items["DataSourceName"] != null &&
                !string.IsNullOrEmpty(Context.Items["DataSourceName"]?.ToString()))
            {
                _dataSourceName = Context.Items["DataSourceName"]?.ToString();
            }

            if (string.IsNullOrEmpty(_dataSourceName) &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["DataSourceName"] != null &&
                !string.IsNullOrEmpty(HttpContext.Current.Session["DataSourceName"]?.ToString()))
            {
                _dataSourceName = HttpContext.Current.Session["DataSourceName"] as string;
            }

            if (string.IsNullOrEmpty(_dataSourceName))
            {
                _dataSourceName = Request.QueryString["dataSourceName"];
            }

            if (string.IsNullOrEmpty(_dataSourceName))
            {
                Response.Write("<script>alert('Veri kaynağınız olmadığından Veritabanı yaratma sayfasına yönlendiriliyorsunuz.');</script>");
                Response.Redirect("CreateManager.aspx");
            }
            #endregion

            #region _DataBase değişkenini al
            if (string.IsNullOrEmpty(_dataBaseName) &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["DataBaseName"] != null &&
                !string.IsNullOrEmpty(HttpContext.Current.Session["DataBaseName"]?.ToString()))
            {
                _dataBaseName = HttpContext.Current.Session["DataBaseName"] as string;
            }

            if (string.IsNullOrEmpty(_dataBaseName))
            {
                _dataBaseName = Request.QueryString["dataBaseName"];
            }
            #endregion

            #region create managerde tiklanan tablo adini al
            if (Request != null && Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["tableName"]))
            {
                _tableName = Request.QueryString["tableName"];
            }
            #endregion

            SubmitNewRow.Visible = false;
            CancelAddRow.Visible = false;

            SubmitNewColumn.Visible = false;
            CancelAddColumn.Visible = false;

            PanelAddNewRow.Visible = false;

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private string GetConnectionString()
        {
            string connectionString = "";
            if (_dataSourceName == null)
            {
                _dataSourceName = Context.Items["DataSourceName"]?.ToString();

                if (!string.IsNullOrEmpty(_dataSourceName))
                {
                    if (_dataSourceName.Contains("(LocalDB)\\MSSQLLocalDB"))
                    {
                        string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";
                        connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
                            "AttachDbFilename=" + dataFileSourceName + ";" +
                            "Initial Catalog=Alpha;" +
                            "Integrated Security=True;" +
                            "Connect Timeout=30;" +
                            "Application Name=DynamicDb";
                    }
                    else
                    {
                        connectionString = "Data Source=" + _dataSourceName + ";Database='" + _dataBaseName + "';Trusted_Connection=True;";
                    }
                    return connectionString;
                }
                return "";
            }
            else if (!string.IsNullOrEmpty(_dataSourceName))
            {
                if (_dataSourceName.Contains("(LocalDB)\\MSSQLLocalDB"))
                {
                    string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";
                    connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
                        "AttachDbFilename=" + dataFileSourceName + ";" +
                        "Initial Catalog=Alpha;" +
                        "Integrated Security=True;" +
                        "Connect Timeout=30;" +
                        "Application Name=DynamicDb";
                }
                else
                {
                    connectionString = "Data Source=" + _dataSourceName + ";Database='" + _dataBaseName + "';Trusted_Connection=True;";
                }
                return connectionString;
            }
            else
            {
                return "";
            }
        }

        public string[] GetColumnsName(string tableName)
        {
            string connectionString = GetConnectionString();
            List<string> listacolumnas = new List<string>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tableName + "' and t.type = 'U'";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listacolumnas.Add(reader.GetString(0));
            }
            return listacolumnas.ToArray();
        }

        private void BindGrid()
        {
            CreateSqlDataSource();
            CreateSqlDataGridView();
            DataGridView.DataSourceID = "DataSource";
            DataGridView.DataBind();
        }

        private void CreateSqlDataSource()
        {
            DataSource.SelectParameters.Clear();
            DataSource.DeleteParameters.Clear();
            DataSource.UpdateParameters.Clear();

            string[] tableColumnNames = GetColumnsName(_tableName);

            string selectCommand = "SELECT * FROM [" + _tableName + "] ";

            #region UPDATE QUERY
            string updateQuery = "UPDATE [" + _tableName + "] SET ";

            for (int i = 0; i < tableColumnNames.Length; i++)
            {
                string item = tableColumnNames[i];
                if (i == 0)
                {
                    updateQuery += "[" + item + "] = @" + item;
                }
                else
                {
                    updateQuery += ", [" + item + "] = @" + item;
                }
            }

            for (int i = 0; i < tableColumnNames.Length; i++)
            {
                string item = tableColumnNames[i];
                if (i == 0)
                {
                    updateQuery += " WHERE[" + item + "] = @original_" + item;
                }
                else
                {
                    updateQuery += " AND (([" + item + "] = @original_" + item + ") OR ([" + item + "] IS NULL AND @original_" + item + " IS NULL)) ";
                }
            }
            #endregion

            #region DELETE QUERY
            string deleteQuery = "DELETE FROM [" + _tableName + "] ";

            for (int i = 0; i < tableColumnNames.Length; i++)
            {
                string item = tableColumnNames[i];
                if (i == 0)
                {
                    deleteQuery += " WHERE[" + item + "] = @original_" + item;
                }
                else
                {
                    deleteQuery += " AND (([" + item + "] = @original_" + item + ") OR ([" + item + "] IS NULL AND @original_" + item + " IS NULL)) ";
                }
            }
            #endregion
            string a0 = DataSource.SelectCommand;
            string a1 = DataSource.DeleteCommand;
            string a2 = DataSource.UpdateCommand;

            DataSource.SelectCommand = selectCommand;
            DataSource.DeleteCommand = deleteQuery;
            DataSource.UpdateCommand = updateQuery;

            #region UPDATE PARAMETERS
            for (int i = 1; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i];
                DataSource.UpdateParameters.Add(new Parameter(columnName, DbType.String));
            }

            for (int i = 0; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i];
                if (i == 0)
                {
                    DataSource.UpdateParameters.Add(new Parameter("original_" + columnName, DbType.Int32));
                }
                else
                {
                    DataSource.UpdateParameters.Add(new Parameter("original_" + columnName, DbType.String));
                }
            }
            #endregion

            for (int i = 0; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i];
                if (i == 0)
                {
                    DataSource.DeleteParameters.Add(new Parameter("original_" + columnName, DbType.Int32));
                }
                else
                {
                    DataSource.DeleteParameters.Add(new Parameter("original_" + columnName, DbType.String));
                }
            }
        }

        private void CreateSqlDataGridView()
        {
            string[] tableColumnNames = GetColumnsName(_tableName);
            string[] primaryKeys = new string[1];
            primaryKeys[0] = tableColumnNames[0];

            DataGridView.DataKeyNames = primaryKeys;

            DataGridView.Columns.Clear();

            CommandField commandButtons = new CommandField()
            {
                ShowEditButton = true,
                ShowDeleteButton = true,
                ShowCancelButton = true
            };

            DataGridView.Columns.Add(commandButtons);

            foreach (var columnName in tableColumnNames)
            {
                BoundField boundField = new BoundField
                {
                    DataField = columnName,
                    HeaderText = columnName,
                    ReadOnly = columnName == tableColumnNames[0],
                    SortExpression = columnName
                };

                DataGridView.Columns.Add(boundField);
            }
        }

        protected void ButtonAddNewRow_Click(object sender, EventArgs e)
        {
            string[] tableColumnNames = GetColumnsName(_tableName);

            for (int i = 1; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i];

                TextBox addNewRowTextBox = new TextBox
                {
                    ID = "AddNewRow" + columnName,
                    Text = columnName
                };

                addNewRowTextBox.Attributes.Add("placeholder", columnName);
                PanelAddNewRow.Controls.Add(addNewRowTextBox);

                if (i % 5 == 0)
                {
                    PanelAddNewRow.Controls.Add(new LiteralControl("<br />"));
                }
            }

            SubmitNewRow.Visible = true;
            CancelAddRow.Visible = true;
            PanelAddNewRow.Visible = true;
        }

        protected void SubmitNewRow_Click(object sender, EventArgs eventArgs)
        {
            string[] tableColumnNames = GetColumnsName(_tableName);

            //Null olmayan sutun adlarini queriye ekle
            string insertQueryColumns = $"INSERT INTO [{_tableName}] (";

            //Null olmayan degerleri queriye ekle
            string queryValues = "VALUES (";

            // Dinamik olusturulmus textboxlari gez
            foreach (Control control in PanelAddNewRow.Controls)
            {
                if (control is TextBox)
                {
                    string value = ((TextBox)control).Text;

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    } else
                    {
                        string columnName = Array.Find(tableColumnNames, tableColumnName => control.ID.Contains(tableColumnName));
                        insertQueryColumns += columnName + ", ";
                        queryValues += value + ", ";
                    }
                }
            }
            // Arkada son kalan virgulden kurtul
            insertQueryColumns = ReplaceLastOccurrenceOfChar(insertQueryColumns, ", ", "");
            queryValues = ReplaceLastOccurrenceOfChar(queryValues, ", ", "");

            insertQueryColumns += ") ";
            queryValues += ")";
            //VALUES ile Sutunlari birlestir.
            string strBetweenParantheses = GetStringBetween(insertQueryColumns, "(", ")");
            if (string.IsNullOrEmpty(strBetweenParantheses))
            {
                PanelAddNewRow.Controls.Clear();
                Response.Write("<script>alert('En az bir alan dolu olmalidir')</script>");
            } else
            {
                insertQueryColumns += queryValues;
                SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                SqlCommand sqlCommand = new SqlCommand(insertQueryColumns, sqlConnection);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        protected void CancelAddRow_Click(object sender, EventArgs eventArgs)
        {
            PanelAddNewRow.Controls.Clear();
        }

        protected void ButtonAddNewColumn_Click(object sender, EventArgs e)
        {
            SubmitNewColumn.Visible = true;
            CancelAddColumn.Visible = true;
        }

        protected void SubmitNewColumn_Click(object sender, EventArgs eventArgs)
        {
            string[] tableColumnNames = GetColumnsName(_tableName);
            string newColumn = TextBoxNewColumnName.Text;
        }

        protected void CancelAddColumn_Click(object sender, EventArgs eventArgs)
        {
            PanelAddNewColumn.Controls.Clear();
        }

        private string ReplaceLastOccurrenceOfChar(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
            {
                return Source;
            }

            return Source.Remove(place, Find.Length).Insert(place, Replace);
        }

        private string GetStringBetween(string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }
    }
}

//protected void Page_Load(object sender, EventArgs e)
//{
//    #region _dataSource degiskenini al
//    if (string.IsNullOrEmpty(_dataSourceName) &&
//        Context != null &&
//        Context.Items["DataSourceName"] != null &&
//        !string.IsNullOrEmpty(Context.Items["DataSourceName"]?.ToString()))
//    {
//        _dataSourceName = Context.Items["DataSourceName"]?.ToString();
//    }

//    if (string.IsNullOrEmpty(_dataSourceName) &&
//        HttpContext.Current.Session != null &&
//        HttpContext.Current.Session["DataSourceName"] != null &&
//        !string.IsNullOrEmpty(HttpContext.Current.Session["DataSourceName"]?.ToString()))
//    {
//        _dataSourceName = HttpContext.Current.Session["DataSourceName"] as string;
//    }

//    if (string.IsNullOrEmpty(_dataSourceName))
//    {
//        _dataSourceName = Request.QueryString["dataSourceName"];
//    }

//    if (string.IsNullOrEmpty(_dataSourceName))
//    {
//        Response.Write("<script>alert('Veri kaynağınız olmadığından Veritabanı yaratma sayfasına yönlendiriliyorsunuz.');</script>");
//        Response.Redirect("CreateManager.aspx");
//    }
//    #endregion

//    if (string.IsNullOrEmpty(_dataBaseName) &&
//        HttpContext.Current.Session != null &&
//        HttpContext.Current.Session["DataBaseName"] != null &&
//        !string.IsNullOrEmpty(HttpContext.Current.Session["DataBaseName"]?.ToString()))
//    {
//        _dataBaseName = HttpContext.Current.Session["DataBaseName"] as string;
//    }

//    if (string.IsNullOrEmpty(_dataBaseName))
//    {
//        _dataBaseName = Request.QueryString["dataBaseName"];
//    }

//    #region create managerde tiklanan tablo adini al
//    if (Request != null && Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["tableName"]))
//    {
//        _tableName = Request.QueryString["tableName"];
//    }
//    #endregion

//    if (!IsPostBack)
//    {
//        //BindGrid();
//    }
//}

//private string GetConnectionString()
//{
//    string connectionString = "";
//    if (_dataSourceName == null)
//    {
//        _dataSourceName = Context.Items["DataSourceName"]?.ToString();

//        if (!string.IsNullOrEmpty(_dataSourceName))
//        {
//            if (_dataSourceName.Contains("(LocalDB)\\MSSQLLocalDB"))
//            {
//                string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";
//                connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
//                    "AttachDbFilename=" + dataFileSourceName + ";" +
//                    "Initial Catalog=Alpha;" +
//                    "Integrated Security=True;" +
//                    "Connect Timeout=30;" +
//                    "Application Name=DynamicDb";
//            }
//            else
//            {
//                connectionString = "Data Source=" + _dataSourceName + ";Database='" + _dataBaseName + "';Trusted_Connection=True;";
//            }
//            return connectionString;
//        }
//        return "";
//    }
//    else if (!string.IsNullOrEmpty(_dataSourceName))
//    {
//        if (_dataSourceName.Contains("(LocalDB)\\MSSQLLocalDB"))
//        {
//            string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";
//            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
//                "AttachDbFilename=" + dataFileSourceName + ";" +
//                "Initial Catalog=Alpha;" +
//                "Integrated Security=True;" +
//                "Connect Timeout=30;" +
//                "Application Name=DynamicDb";
//        }
//        else
//        {
//            connectionString = "Data Source=" + _dataSourceName + ";Database='" + _dataBaseName + "';Trusted_Connection=True;";
//        }
//        return connectionString;
//    }
//    else
//    {
//        return "";
//    }
//}

//public string[] GetColumnsName(string tableName)
//{
//    string connectionString = GetConnectionString();
//    List<string> listacolumnas = new List<string>();
//    SqlConnection connection = new SqlConnection(connectionString);
//    SqlCommand command = connection.CreateCommand();
//    command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tableName + "' and t.type = 'U'";
//    connection.Open();
//    SqlDataReader reader = command.ExecuteReader();
//    while (reader.Read())
//    {
//        listacolumnas.Add(reader.GetString(0));
//    }
//    return listacolumnas.ToArray();
//}

//private void ConnectAndExecuteQuery(string connectionString, string query)
//{
//    SqlConnection conn = new SqlConnection(connectionString);

//    conn.Open();

//    SqlCommand cmd = new SqlCommand(query, conn);

//    SqlDataReader dr = cmd.ExecuteReader();

//    conn.Close();
//}

//private void BindGrid()
//{
//    CreateSqlDataSource();
//    CreateSqlDataGridView();
//    GridView1.DataSource = _SqlDataSource;
//    GridView1.DataBind();
//}

//private void CreateSqlDataSource()
//{
//    ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["ConnectionString"];
//    string connectionString = mySetting.ConnectionString;

//    string[] tableColumnNames = GetColumnsName(_tableName);

//    _SqlDataSource.ConflictDetection = ConflictOptions.CompareAllValues;
//    _SqlDataSource.ConnectionString = connectionString;
//    _SqlDataSource.OldValuesParameterFormatString = $"original_{0}";
//    _SqlDataSource.SelectCommandType = SqlDataSourceCommandType.Text;
//    _SqlDataSource.UpdateCommandType = SqlDataSourceCommandType.Text;
//    _SqlDataSource.DeleteCommandType = SqlDataSourceCommandType.Text;

//    string selectCommand = "SELECT * FROM [" + _tableName + "]";

//    #region UPDATE QUERY
//    string updateQuery = "UPDATE [" + _tableName + "] SET ";

//    for (int i = 0; i < tableColumnNames.Length; i++)
//    {
//        string item = tableColumnNames[i];
//        if (i == 0)
//        {
//            updateQuery += "[" + item + "] = @" + item;
//        }
//        else
//        {
//            updateQuery += ", [" + item + "] = @" + item;
//        }
//    }

//    for (int i = 0; i < tableColumnNames.Length; i++)
//    {
//        string item = tableColumnNames[i];
//        if (i == 0)
//        {
//            updateQuery += "WHERE[" + item + "] = @original_" + item;
//        }
//        else
//        {
//            updateQuery += "AND (([" + item + "] = @original_" + item + ") OR([" + item + "] IS NULL AND @original_" + item + " IS NULL)) ";
//        }
//    }
//    #endregion

//    #region DELETE QUERY
//    string deleteQuery = "DELETE FROM [" + _tableName + "] ";

//    for (int i = 0; i < tableColumnNames.Length; i++)
//    {
//        string item = tableColumnNames[i];
//        if (i == 0)
//        {
//            deleteQuery += "WHERE[" + item + "] = @original_" + item;
//        }
//        else
//        {
//            deleteQuery += "AND (([" + item + "] = @original_" + item + ") OR([" + item + "] IS NULL AND @original_" + item + " IS NULL)) ";
//        }
//    }
//    #endregion

//    _SqlDataSource.SelectCommand = selectCommand;
//    _SqlDataSource.DeleteCommand = deleteQuery;
//    _SqlDataSource.UpdateCommand = updateQuery;

//    #region UPDATE PARAMETERS
//    for (int i = 1; i < tableColumnNames.Length; i++)
//    {
//        string columnName = tableColumnNames[i];
//        _SqlDataSource.UpdateParameters.Add(new Parameter(columnName, DbType.String));
//    }

//    for (int i = 0; i < tableColumnNames.Length; i++)
//    {
//        string columnName = tableColumnNames[i];
//        if (i == 0)
//        {
//            _SqlDataSource.UpdateParameters.Add(new Parameter("original_" + columnName, DbType.Int32));
//        }
//        else
//        {
//            _SqlDataSource.UpdateParameters.Add(new Parameter("original_" + columnName, DbType.String));
//        }
//    }
//    #endregion

//    for (int i = 0; i < tableColumnNames.Length; i++)
//    {
//        string columnName = tableColumnNames[i];
//        if (i == 0)
//        {
//            _SqlDataSource.DeleteParameters.Add(new Parameter("original_" + columnName, DbType.Int32));
//        }
//        else
//        {
//            _SqlDataSource.DeleteParameters.Add(new Parameter("original_" + columnName, DbType.String));
//        }
//    }

//    Page.Controls.Add(_SqlDataSource);
//}

//private void CreateSqlDataGridView()
//{
//    string[] tableColumnNames = GetColumnsName(_tableName);
//    string[] primaryKeys = new string[1];
//    primaryKeys[0] = tableColumnNames[0];

//    GridView1.DataKeyNames = primaryKeys;

//    foreach (var columnName in tableColumnNames)
//    {
//        BoundField boundField = new BoundField
//        {
//            DataField = columnName,
//            HeaderText = columnName,
//            ReadOnly = columnName == tableColumnNames[0],
//            SortExpression = columnName
//        };

//        GridView1.Columns.Add(boundField);
//    }
//}

//protected void BtnInsertRow_Click(object sender, EventArgs e)
//{
//    /*string[] tableColumnNames = GetColumnsName(_tableName);
//    string connectionString = GetConnectionString();
//    string commandString = $"INSERT INTO [{_tableName}] VALUES([{}])";
//    SqlConnection conn = new SqlConnection(connectionString);
//    conn.Open();
//    */
//}

//protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
//{
//    Console.Write("deneme");
//    string ali = "";
//    ////Retrieve the table from the session object.
//    //DataTable dt = (DataTable)Session["TaskTable"];

//    ////Update the values.
//    //GridViewRow row = TaskGridView.Rows[e.RowIndex];
//    //dt.Rows[row.DataItemIndex]["Id"] = ((TextBox)(row.Cells[1].Controls[0])).Text;
//    //dt.Rows[row.DataItemIndex]["Description"] = ((TextBox)(row.Cells[2].Controls[0])).Text;
//    //dt.Rows[row.DataItemIndex]["IsComplete"] = ((CheckBox)(row.Cells[3].Controls[0])).Checked;

//    ////Reset the edit index.
//    //TaskGridView.EditIndex = -1;

//    ////Bind data to the GridView control.
//    //BindData();
//}

//protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
//{

//}