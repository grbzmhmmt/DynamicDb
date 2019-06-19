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
        private string _userName;
        private string _userPassword;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region _dataSource degiskenini al
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
            //if (Request != null && Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["tableName"]))
            if (HttpContext.Current.Session != null &&
                HttpContext.Current.Session["tableName"] != null)
            {
                string newTableChangedName = HttpContext.Current.Session["tableName"]?.ToString();
                if (!string.IsNullOrEmpty(newTableChangedName))
                {
                    string isTableNameChanged = HttpContext.Current.Session["isTableNameChanged"]?.ToString();

                    if (isTableNameChanged == "true")
                    {
                        _tableName = newTableChangedName;
                        isTableNameChanged = "false";
                        lblTableName.Text = _tableName;
                        HttpContext.Current.Session["isTableNameChanged"] = "false";
                        HttpContext.Current.Session["tableName"] = newTableChangedName;
                        //string redirectUrl = Request.RawUrl;
                        //redirectUrl=redirectUrl.Replace("isNameChanged=true", "isNameChanged=false");
                        //Response.Redirect(redirectUrl, true);
                    }
                    else
                    {
                        _tableName = newTableChangedName;
                        lblTableName.Text = _tableName;
                    }
                }
                else
                {
                    _tableName = newTableChangedName;
                    lblTableName.Text = _tableName;
                }
            }
            #endregion

            #region User Kontrol
            if (HttpContext.Current.Session != null &&
                HttpContext.Current.Session["UserName"] != null &&
                HttpContext.Current.Session["Password"] != null &&
                !string.IsNullOrEmpty(HttpContext.Current.Session["UserName"]?.ToString()) &&
                !string.IsNullOrEmpty(HttpContext.Current.Session["Password"]?.ToString()))
            {
                _userName = HttpContext.Current.Session["UserName"] as string;
                _userPassword = HttpContext.Current.Session["Password"] as string;
            }

            string sqlConnection = "Data Source=" + _dataSourceName + " ;Database=SqlDynamicDbMyUsers;Trusted_Connection=True;";
            //Temel Database e bağlantı kuruluyor
            SqlConnection conn = new SqlConnection(sqlConnection);

            //ilgili Kullanıcı varmı diye sorgu atılıyor
            string query = "SELECT * FROM Users WHERE UserName='" + _userName + "' AND Password='" + _userPassword + "'";

            SqlCommand cmd = new SqlCommand(query, conn);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            if (!dr.Read())//kullanıcı kontrolü
            {
                Response.Redirect("NotFound.aspx", true);
            }

            dr.Close();
            conn.Close();
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

            ButtonSubmitUpdatingRow.Visible = false;
            ButtonCancelUpdatingRow.Visible = false;

            ButtonSubmitChangeColumnName.Visible = false;
            ButtonCancelChangeColumnName.Visible = false;
            TextBoxOldColumnName.Visible = false;

            PanelAddRow.Visible = false;
            PanelAddColumn.Visible = false;
            PanelDeleteColumn.Visible = false;
        }

        private void GenerateAddRowTextBoxes()
        {
            string[] tableColumnNames = GetColumnsName(_tableName);

            for (int i = 1; i < tableColumnNames.Length; i++)
            {
                string columnName = tableColumnNames[i].Trim();

                TextBox addNewRowTextBox = new TextBox
                {
                    ID = "AddRow" + columnName
                };

                addNewRowTextBox.Attributes.Add("placeholder", columnName);
                PanelAddRow.Controls.Add(addNewRowTextBox);

                if (i % 8 == 0)
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

            string[] tableColumnNames = GetColumnsName(_tableName);

            string selectCommand = "SELECT * FROM [" + _tableName + "] ";

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

            DataSource.ConnectionString = "Data Source=(Local);Initial Catalog=" + _dataBaseName + ";Integrated Security=True";
            DataSource.SelectCommand = selectCommand;
            DataSource.DeleteCommand = deleteQuery;

            #region DELETE PARAMETERS
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
            #endregion DELETE PARAMETERS
        }

        private void CreateSqlDataGridView()
        {
            DataGridView.Columns.Clear();

            string[] tableColumnNames = GetColumnsName(_tableName);

            string[] primaryKeys = new string[1];
            primaryKeys[0] = tableColumnNames[0];

            DataGridView.DataKeyNames = primaryKeys;

            CommandField commandButtons = new CommandField()
            {
                ShowSelectButton = true,
                ShowDeleteButton = true
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
            string newColumnType = "NVARCHAR(80)"; // "";

            if (string.IsNullOrEmpty(newColumnName))
            {
                PanelAddColumn.Controls.Clear();
                Response.Write("<script>alert('Sütun ad alani dolu olmalidir.')</script>");
            }
            else
            {
                string sqlQuery = $"ALTER TABLE [{_tableName}] ADD [{newColumnName}] {newColumnType} NULL";

                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    TextBoxNewColumnName.Text = "";
                    TextBoxDeleteColumn.Text = "";

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

                    TextBoxNewColumnName.Text = "";
                    TextBoxDeleteColumn.Text = "";
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
            return "Data Source=" + _dataSourceName + ";Database='" + _dataBaseName + "';Trusted_Connection=True;";
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

        protected void DataGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ButtonSubmitUpdatingRow.Visible = true;
            ButtonCancelUpdatingRow.Visible = true;
            PanelAddRow.Visible = true;

            GridViewRow gridViewRow = DataGridView.Rows[DataGridView.SelectedIndex];

            string[] cellValues = new string[gridViewRow.Cells.Count - 2];

            for (int i = 2; i < gridViewRow.Cells.Count; i++)
            {
                cellValues[i - 2] = gridViewRow.Cells[i].Text.Trim();
            }

            for (int i = 0; i < PanelAddRow.Controls.Count; i++)
            {
                Control control = PanelAddRow.Controls[i];

                if (control is TextBox)
                {
                    (control as TextBox).Text = cellValues[i].Trim();
                }
            }
        }

        protected void ButtonSubmitUpdatingRow_Click(object sender, EventArgs e)
        {
            string[] columnNames = GetColumnsName(_tableName);

            string updateQuery = "UPDATE [" + _tableName + "] SET ";

            for (int i = 0; i < PanelAddRow.Controls.Count; i++)
            {
                Control control = PanelAddRow.Controls[i];

                if (control is TextBox)
                {
                    if (i == 0)
                    {
                        string newColumnValue = (control as TextBox).Text.Trim();
                        updateQuery += columnNames[i + 1] + " = '" + newColumnValue + "' ";
                    }
                    else
                    {
                        string newColumnValue = (control as TextBox).Text.Trim();
                        updateQuery += ", " + columnNames[i + 1] + " = '" + newColumnValue + "'";
                    }
                }
            }

            GridViewRow gridViewRow = DataGridView.Rows[DataGridView.SelectedIndex];

            string pKey = gridViewRow.Cells[1].Text;

            updateQuery += " WHERE [" + columnNames[0] + "] = '" + pKey + "'";

            try
            {
                SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                SqlCommand sqlCommand = new SqlCommand(updateQuery, sqlConnection);
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

        protected void ButtonCancelUpdatingRow_Click(object sender, EventArgs e)
        {
            ButtonSubmitUpdatingRow.Visible = false;
            ButtonCancelUpdatingRow.Visible = false;
            PanelAddRow.Visible = false;
        }

        protected void ButtonChangeColumnName_Click(object sender, EventArgs e)
        {
            ButtonSubmitChangeColumnName.Visible = true;
            ButtonCancelChangeColumnName.Visible = true;

            TextBoxOldColumnName.Attributes.Add("placeholder", "Varolan sütun adı.");
            TextBoxNewColumnName.Attributes.Add("placeholder", "Yeni sütun adı.");
            TextBoxOldColumnName.Visible = true;

            PanelAddColumn.Visible = true;
        }

        protected void ButtonSubmitChangeColumnName_Click(object sender, EventArgs e)
        {
            string oldName = TextBoxOldColumnName.Text.Trim();
            string newName = TextBoxNewColumnName.Text.Trim();

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName))
            {
                Response.Write("<script>alert('Alanlar dolu olmalidir.')</script>");
            }
            else
            {
                string sqlChangeNameQuery = "EXEC sp_rename '" + _tableName + "." + oldName + "', '" + newName + "'";

                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(sqlChangeNameQuery, sqlConnection);
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

            TextBoxOldColumnName.Text = "";
            TextBoxNewColumnName.Text = "";
            TextBoxDeleteColumn.Text = "";
            TextBoxOldColumnName.Attributes.Add("placeholder", "");
            TextBoxNewColumnName.Attributes.Add("placeholder", "");
        }

        protected void ButtonCancelChangeColumnName_Click(object sender, EventArgs e)
        {
            PanelAddColumn.Visible = false;
            TextBoxOldColumnName.Visible = false;
            ButtonSubmitChangeColumnName.Visible = false;
            ButtonCancelChangeColumnName.Visible = false;

            TextBoxOldColumnName.Text = "";
            TextBoxNewColumnName.Text = "";
            TextBoxDeleteColumn.Text = "";
        }

        protected void ButtonChangeTableName_Click(object sender, EventArgs e)
        {
            string newTableName = TextBoxChangeTableName.Text.Trim();
            if (string.IsNullOrEmpty(newTableName))
            {
                Response.Write("<script>alert('Tablo adı giriniz.')</script>");
            }
            else
            {
                string sqlChangeNameQuery = "EXEC SP_RENAME '" + _tableName + "', '" + newTableName + "'";

                try
                {
                    SqlConnection sqlConnection = new SqlConnection(GetConnectionString());
                    SqlCommand sqlCommand = new SqlCommand(sqlChangeNameQuery, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();

                    HttpContext.Current.Session["isTableNameChanged"] = "true";
                    HttpContext.Current.Session["tableName"] = newTableName;

                    try
                    {
                        _tableName = newTableName;
                        lblTableName.Text = _tableName;
                        //Request.RawUrl.Replace(_tableName, newTableName);
                        //Response.Redirect(Request.RawUrl + "&isNameChanged=true&newTableName=" + newTableName, true);
                    }
                    catch (Exception exc)
                    {
                        //Response.Redirect(Request.RawUrl + "&isNameChanged=true&newTableName=" + newTableName, true);
                        Console.WriteLine("*** HATA BASLANGIC***");
                        Console.WriteLine(exc);
                        Console.WriteLine("*** HATA BITIS***");
                    }
                }
                catch (Exception exc)
                {
                    //Response.Redirect(Request.RawUrl + "&isNameChanged=true&newTableName=" + newTableName, true);
                    Console.WriteLine("*** HATA BASLANGIC***");
                    Console.WriteLine(exc);
                    Console.WriteLine("*** HATA BITIS***");
                }
            }
        }

        protected void ButtonDeleteTableName_Click(object sender, EventArgs e)
        {

        }
    }
}