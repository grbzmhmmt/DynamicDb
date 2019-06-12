using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

            SetVisibilitiesToFalse();

            GenerateAddRowTextBoxes();

            BindGrid();
        }

        #region Baslatma ekran ayarlama fonksiyonlari
        private void SetVisibilitiesToFalse()
        {
            ButtonSubmitAddRow.Visible = false;
            ButtonCancelAddRow.Visible = false;

            ButtonSubmitAddColumn.Visible = false;
            ButtonCancelAddColumn.Visible = false;

            ButtonSubmitDeleteColumn.Visible = false;
            ButtonCancelDeleteColumn.Visible = false;

            PanelAddRow.Visible = false;
            PanelAddColumn.Visible = false;
            PanelDeleteColumn.Visible = false;
        }

        private void GenerateAddRowTextBoxes()
        {
            string[] tableColumnNames = GetColumnsName(_tableName);

            for (int i = 1; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i];

                TextBox addNewRowTextBox = new TextBox
                {
                    ID = "AddRow" + columnName,
                    Text = columnName
                };

                addNewRowTextBox.Attributes.Add("placeholder", columnName);
                PanelAddRow.Controls.Add(addNewRowTextBox);

                if (i % 5 == 0)
                {
                    PanelAddRow.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }

        private void BindGrid()
        {
            CreateSqlDataSource();
            CreateSqlDataGridView();
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
                ShowSelectButton = true
                //ShowEditButton = true,
                //ShowDeleteButton = true,
                //ShowCancelButton = true
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

            DataGridView.DataSourceID = "DataSource";
        }
        #endregion

        #region Satir ekleme ve silme fonksiyonlari
        protected void ButtonStartAddRow_Click(object sender, EventArgs e)
        {
            ButtonSubmitAddRow.Visible = true;
            ButtonCancelAddRow.Visible = true;
            PanelAddRow.Visible = true;
        }

        protected void ButtonSubmitAddRow_Click(object sender, EventArgs eventArgs)
        {
            string[] tableColumnNames = GetColumnsName(_tableName);

            //Null olmayan sutun adlarini queriye ekle
            string insertQueryColumns = $"INSERT INTO [{_tableName}] (";

            //Null olmayan degerleri queriye ekle
            string queryValues = "VALUES (";

            // Dinamik olusturulmus textboxlari gez
            foreach (Control control in PanelAddRow.Controls)
            {
                if (control is TextBox)
                {
                    string value = ((TextBox)control).Text;

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    else
                    {
                        string columnName = Array.Find(tableColumnNames, tableColumnName => control.ID.Contains("AddRow" + tableColumnName));
                        insertQueryColumns += columnName + ", ";
                        queryValues += "'" + value + "'" + ", ";
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
                PanelAddRow.Controls.Clear();
                Response.Write("<script>alert('En az bir alan dolu olmalidir.')</script>");
            }
            else
            {
                insertQueryColumns += queryValues;
                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(insertQueryColumns, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    DataGridView.DataBind();
                }
                catch (Exception exc)
                {
                    Console.WriteLine("*** HATA BASLANGIC***");
                    Console.WriteLine(exc);
                    Console.WriteLine("*** HATA BITIS***");
                }

                ButtonSubmitAddRow.Visible = false;
                ButtonCancelAddRow.Visible = false;

                PanelAddRow.Visible = false;
            }
        }

        protected void ButtonCancelAddRow_Click(object sender, EventArgs eventArgs)
        {
            PanelAddRow.Controls.Clear();
        }

        protected void ButtonDeleteSelectedRow_Click(object sender, EventArgs e)
        {
            // Satir secilimi kontrol et
            if (DataGridView.SelectedValue == null)
            {
                Response.Write("<script>alert('Silmek icin bir satir secmelisiniz.')</script>");
            }
            else
            {
                // Satir secilimi ikinci defa kontrol et
                if (DataGridView.SelectedIndex >= 0)
                {
                    string[] tableColumnNames = GetColumnsName(_tableName);

                    GridViewRow gridSelectedRow = DataGridView.Rows[DataGridView.SelectedIndex];
                    // Primary key i al.
                    string pKey = gridSelectedRow.Cells[1].Text;
                    string column1 = gridSelectedRow.Cells[2].Text;

                    string sqlDeleteRowQuery = $"DELETE FROM [{_tableName}] WHERE [{tableColumnNames[0]}] = '" + pKey + "'" + "AND" +
                        $"[{tableColumnNames[1]}] = '" + column1 + "'";

                    try
                    {
                        SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                        SqlCommand sqlCommand = new SqlCommand(sqlDeleteRowQuery, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();

                        DataGridView.DataBind();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("*** HATA BASLANGIC***");
                        Console.WriteLine(exc);
                        Console.WriteLine("*** HATA BITIS***");
                    }
                }
            }
        }
        #endregion

        #region Sütun ekleme ve silme fonksiyonlari
        protected void ButtonStartAddColumn_Click(object sender, EventArgs e)
        {
            ButtonSubmitAddColumn.Visible = true;
            ButtonCancelAddColumn.Visible = true;

            PanelAddColumn.Visible = true;
        }

        protected void ButtonSubmitAddColumn_Click(object sender, EventArgs eventArgs)
        {
            string[] tableColumnNames = GetColumnsName(_tableName);
            string newColumnName = TextBoxNewColumnName.Text;
            string newColumnType = "";

            if (string.IsNullOrEmpty(newColumnName))
            {
                PanelAddColumn.Controls.Clear();
                Response.Write("<script>alert('Sütun ad alani dolu olmalidir.')</script>");
            }
            else
            {
                string selectedType = DropDownListNewColumnTypes.SelectedValue;
                // Secilen tipe gore tipi ayarla
                if (selectedType == "number")
                {
                    newColumnType = "INT";
                }
                else if (selectedType == "string")
                {
                    newColumnType = "NVARCHAR(80)";
                }
                else if (selectedType == "date")
                {
                    newColumnType = "DATE";
                }
                else if (selectedType == "image")
                {
                    newColumnType = "IMAGE";
                }

                string sqlQuery = $"ALTER TABLE [{_tableName}] ADD [{newColumnName}] {newColumnType} NULL";

                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    DataGridView.DataBind();

                    try
                    {
                        Response.Redirect(Request.RawUrl, false);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("*** HATA BASLANGIC***");
                        Console.WriteLine(exc);
                        Console.WriteLine("*** HATA BITIS***");
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("*** HATA BASLANGIC***");
                    Console.WriteLine(exc);
                    Console.WriteLine("*** HATA BITIS***");
                }

                ButtonSubmitAddColumn.Visible = false;
                ButtonCancelAddColumn.Visible = false;

                PanelAddColumn.Visible = false;
            }
        }

        protected void ButtonCancelAddColumn_Click(object sender, EventArgs e)
        {
            PanelAddColumn.Controls.Clear();
        }

        protected void ButtonStartDeleteColumn_Click(object sender, EventArgs e)
        {
            ButtonSubmitDeleteColumn.Visible = true;
            ButtonCancelDeleteColumn.Visible = true;
            PanelDeleteColumn.Visible = true;
        }

        protected void ButtonSubmitDeleteColumn_Click(object sender, EventArgs e)
        {
            //Silinecek sutun adini al.
            string columnToBeDeleted = TextBoxDeleteColumn.Text.Trim();

            if (string.IsNullOrEmpty(columnToBeDeleted))
            {
                Response.Write("<script>alert('Sutun adi alanini girmelisiniz.')</script>");
            }
            else
            {
                string sqlDeleteColumnQuery = $"ALTER TABLE [{_tableName}] DROP COLUMN {columnToBeDeleted}";

                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(sqlDeleteColumnQuery, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    try
                    {
                        Response.Redirect(Request.RawUrl, false);
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("*** HATA BASLANGIC***");
                        Console.WriteLine(exc);
                        Console.WriteLine("*** HATA BITIS***");
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("*** HATA BASLANGIC***");
                    Console.WriteLine(exc);
                    Console.WriteLine("*** HATA BITIS***");
                }
            }
        }

        protected void ButtonCancelDeleteColumn_Click(object sender, EventArgs e)
        {
            TextBoxDeleteColumn.Text = "";
            ButtonSubmitDeleteColumn.Visible = false;
            ButtonCancelDeleteColumn.Visible = false;
            PanelDeleteColumn.Visible = false;
        }
        #endregion

        #region Yardimci fonksiyonlar
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

        private string ReplaceLastOccurrenceOfChar(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
            {
                return Source;
            }

            return Source.Remove(place, Find.Length).Insert(place, Replace);
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
        #endregion
    }
}