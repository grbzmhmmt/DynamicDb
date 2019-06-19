using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDb.Pages
{
    public partial class CreateManager : Page
    {
        private static readonly string _dataSourceName = "DESKTOP-6UQLI0L";
        private static readonly string _myAppDBName = "SqlDynamicDbMyUsers";
        private static readonly string myDefaultTableName = "DefaultastasGrbzBrky";
        private static readonly string _myAppUsersTableName = "Users";
        private string _userName, _userPassword;
        private List<string> _userDbNames = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_userName) &&
                string.IsNullOrEmpty(_userPassword) &&
                HttpContext.Current.Session != null &&
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


            _userDbNames = GetUserDbNames();

            if (_userDbNames != null && _userDbNames.Count > 0)
            {
                string temp0 = DropDownDBNames.SelectedValue;
                string temp1 = DropDownGetTblDbNames.SelectedValue;
                DropDownDBNames.Items.Clear();
                DropDownGetTblDbNames.Items.Clear();
                foreach (var dbName in _userDbNames)
                {
                    DropDownDBNames.Items.Add(new ListItem(dbName.Trim(), dbName.Trim()));
                    DropDownGetTblDbNames.Items.Add(new ListItem(dbName.Trim(), dbName.Trim()));
                }
                if (!string.IsNullOrEmpty(temp0))
                {
                    DropDownDBNames.SelectedValue = temp0;
                }
                if (!string.IsNullOrEmpty(temp1))
                {
                    DropDownGetTblDbNames.SelectedValue = temp1;
                }
                DropDownDBNames.DataBind();
                DropDownGetTblDbNames.DataBind();
            }
        }

        protected List<string> GetUserDbNames()
        {

            string sqlConnection = "Data Source=" + _dataSourceName + " ;Database=SqlDynamicDbMyUsers;Trusted_Connection=True;";
            SqlConnection conn = new SqlConnection(sqlConnection);

            //ilgili Kullanıcı varmı diye sorgu atılıyor
            string query = "SELECT * FROM Users WHERE UserName='" + _userName + "' AND Password='" + _userPassword + "'";

            SqlCommand cmd = new SqlCommand(query, conn);

            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            string userDatabaseName = "";

            if (dr.Read())//kullanıcı kontrolü
            {
                userDatabaseName = dr["DatabaseName"].ToString();
            }
            dr.Close();
            conn.Close();

            return userDatabaseName.Split('$').ToList();

        }

        protected void BtnCreateDb_Click(object sender, EventArgs e)
        {
            string databaseName = txtDbDatabaseName.Text.Replace("\'", "").ToString().Trim();
            if (!string.IsNullOrEmpty(databaseName))
            {
                HttpContext.Current.Session["DataBaseName"] = databaseName;
                CreateDatabase(databaseName);
            }
            else
            {
                Response.Write("<script>alert('Database Name alanını boş bırakmayınız.');</script>");
            }
        }

        protected void BtnCreateTable_Click(object sender, EventArgs e)
        {
            string databaseName = DropDownDBNames.Text;
            string tableName = txtTblTableName.Text.Replace("\'", "").ToString().Trim();
            string primaryKey = txtTblPrimaryKeyName.Text.Replace("\'", "").ToString().Trim();
            string foreignKey = txtTblForeignKeyName.Text.Replace("\'", "").ToString().Trim();
            string foreingKeyRefTable = txtTblForeignKeyReferanceTable.Text.Replace("\'", "").ToString().Trim();
            string foreignKeyRefColumn = txtTblForeingKeyReferanceColumn.Text.Replace("\'", "").ToString().Trim();

            string[] myArray = hdnTblColumns.Value.Trim().Split(',');
            myArray = myArray.Reverse().Skip(1).Reverse().ToArray();

            if (!string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(tableName))
            {
                if (!string.IsNullOrEmpty(foreignKey) && !string.IsNullOrEmpty(foreingKeyRefTable) && !string.IsNullOrEmpty(foreignKeyRefColumn))
                {
                    try
                    {
                        CreateTableWithForeignKey(databaseName, tableName, myArray, primaryKey, foreignKey, foreingKeyRefTable, foreignKeyRefColumn);
                    }
                    catch (Exception)
                    {
                        Response.Write("<script>alert('Foreign Key Hatası');</script>");
                    }
                }
                else
                {
                    try
                    {
                        CreateTableWithPrimaryKey(databaseName, tableName, myArray, primaryKey);
                    }
                    catch (Exception)
                    {
                        Response.Write("<script>alert('Primary Key Hatası');</script>");
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('Database Name veya Table Name alanını boş bırakmayınız.');</script>");
            }
        }

        protected void GetTables_Click(object sender, EventArgs e)
        {
            var ss = DropDownGetTblDbNames.SelectedIndex;
            string dbName = DropDownGetTblDbNames.SelectedValue;
            if (!string.IsNullOrEmpty(dbName.Trim()))
            {
                HttpContext.Current.Session["DataBaseName"] = dbName;
                UpdateTables(dbName.Trim());
            }
        }

        private void ConnectAndExecuteQuery(string connectionString, string query)
        {
            try
            {
                if (HttpContext.Current.Session != null && (HttpContext.Current.Session["DataSourceName"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["DataSourceName"]?.ToString())))
                {
                    HttpContext.Current.Session["DataSourceName"] = _dataSourceName;
                }

                NavBar.DataSourceName = _dataSourceName;

                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Bu veritabanı ismi kullanılamıyor. Farklı bir veritabanı adı giriniz.');</script>");
            }
        }
        //Ok
        private void CreateDatabase(string dbName)
        {
            try
            {
                string connectionString = "Data Source=" + _dataSourceName + ";Database=master;Trusted_Connection=True;";
                string query = "CREATE DATABASE " + dbName.Trim();

                ConnectAndExecuteQuery(connectionString, query);
                string[] myDefaultArray = new string[] { "stn0", "stn1" };
                CreateTableWithPrimaryKey(dbName, myDefaultTableName, myDefaultArray, "pkey");

                _userDbNames.Add(dbName.Trim());
                DropDownDBNames.Items.Add(new ListItem(dbName.Trim(), dbName.Trim()));
                DropDownGetTblDbNames.Items.Add(new ListItem(dbName.Trim(), dbName.Trim()));
                AddUserDBNames(dbName.Trim());
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Lütfen Data Source kısmına doğru giriş yaptığınızdan emin olunuz.');</script>");
            }
        }

        private void AddUserDBNames(string dbName)
        {
            string connectionString = "Data Source=" + _dataSourceName + ";Database=" + _myAppDBName + ";Trusted_Connection=True;";

            string selectQuery = "SELECT DatabaseName FROM " + _myAppUsersTableName + " WHERE UserName = '" + _userName + "' AND Password = '" + _userPassword + "'";

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(selectQuery, conn);

                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                string userDatabaseName = "";
                if (dr.Read())
                {
                    if (dr["DatabaseName"] == null)
                    {
                        userDatabaseName = "";
                    }
                    else
                    {
                        userDatabaseName = dr["DatabaseName"]?.ToString();
                    }
                }

                dr.Close();
                conn.Close();

                string updateQuery = "";

                if (!string.IsNullOrEmpty(userDatabaseName))
                {
                    updateQuery = "UPDATE [" + _myAppUsersTableName + "] SET DatabaseName = '" + userDatabaseName + "$" + dbName + "'" +
                   "WHERE UserName = '" + _userName + "' AND Password = '" + _userPassword + "'";
                }
                else
                {
                    updateQuery = "UPDATE [" + _myAppUsersTableName + "] SET DatabaseName = '" + dbName + "'" +
                   "WHERE UserName = '" + _userName + "' AND Password = '" + _userPassword + "'";
                }

                ConnectAndExecuteQuery(connectionString, updateQuery);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void CreateTableWithPrimaryKey(string dbName, string tableName, string[] columns, string primaryKey)
        {
            string query = "";
            if (!string.IsNullOrEmpty(primaryKey))
            {
                query = "CREATE TABLE " + tableName + "(" + primaryKey + " int IDENTITY (1, 1) NOT NULL,";

                foreach (var column in columns)
                {
                    query += column + " char(250) NULL,";
                }

                query += " CONSTRAINT [PK_" + tableName + "] PRIMARY KEY (" + primaryKey + " ASC))";
            }
            else
            {
                query = "CREATE TABLE " + tableName + "( ";

                foreach (var column in columns)
                {
                    query += column + " char(250) NULL,";
                }

                query += " )";
            }
            try
            {
                string connectionString = "Data Source=" + _dataSourceName + ";Database=" + dbName + ";Trusted_Connection=True;";
                ConnectAndExecuteQuery(connectionString, query);
                UpdateTables(dbName.Trim());
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Veri Tabanına bağlanırken bir sorunla karşılaşıldı.');</script>");
            }
        }

        private void CreateTableWithForeignKey(string dbName, string tableName, string[] columns, string primaryKey, string foreignKey, string foreingKeyRefTable, string foreignKeyRefColumn)
        {
            string query = "";
            if (!string.IsNullOrEmpty(primaryKey))
            {
                query = "CREATE TABLE " + tableName +
                    "(" + primaryKey + " int IDENTITY (1,1) NOT NULL," +
                    "  [" + foreignKey + "] int NOT NULL,";

                foreach (var column in columns)
                {
                    query += column + " nvarchar(250) NULL,";
                }

                query += " CONSTRAINT [PK_" + tableName + "] PRIMARY KEY (" + primaryKey + " ASC)";

                query += "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF," +
                " ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";

                query += "ALTER TABLE[dbo].[" + tableName + "] WITH CHECK ADD CONSTRAINT[FK_" + tableName + "_" + foreingKeyRefTable + "]" +
                    " FOREIGN KEY([" + foreignKey + "]) REFERENCES[dbo].[" + foreingKeyRefTable + "] ([" + foreignKeyRefColumn + "])";
                query += " ALTER TABLE[dbo].[" + tableName + "] CHECK CONSTRAINT[FK_" + tableName + "_" + foreingKeyRefTable + "]";
            }
            else
            {
                Response.Write("<script>alert('Primary Key alanı boş bırakılamaz');</script>");
            }
            try
            {
                string connectionString = "Data Source=" + _dataSourceName + ";Database=" + dbName + ";Trusted_Connection=True;";
                ConnectAndExecuteQuery(connectionString, query);
                UpdateTables(dbName.Trim());
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Veri Tabanına bağlanırken bir sorunla karşılaşıldı.');</script>");
            }
        }

        private void UpdateTables(string dbName)
        {
            string query = "SELECT TABLE_NAME As Tablolar FROM " + dbName + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

            string connectionString = "Data Source=" + _dataSourceName + ";Database=master;Trusted_Connection=True;";

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();

            List<string> myTables = new List<string>();
            while (dr.Read())
            {
                string tableName = dr["Tablolar"].ToString();
                if (!tableName.Contains(myDefaultTableName))
                {
                    myTables.Add(tableName);
                }
            }
            dr.Close();
            conn.Close();

            TablesGridView.DataSource = myTables;
            TablesGridView.DataBind();
        }

        protected void TablesGridRow_Click(object sender, EventArgs e)
        {
            GridViewRow selectedRow = TablesGridView.SelectedRow;
            string tableName = selectedRow.Cells[1].Text.Trim();
            string databaseName = DropDownGetTblDbNames.Text;

            if (string.IsNullOrEmpty(tableName))
            {
                Response.Write("<script>alert('Tablo adı alanınıda doldurunuz.');</script>");
            }
            else
            {
                string directAddress = "DataManager.aspx?dataSourceName=" + _dataSourceName + "&dataBaseName=" + databaseName;
                HttpContext.Current.Session["tableName"] = tableName;
                Response.Redirect(directAddress);
            }
        }
    }
}
