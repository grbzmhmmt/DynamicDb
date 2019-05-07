using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace DynamicDb.Pages
{
    public partial class CreateManager : Page
    {
        private string _dataSourceName;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BtnCreateDb_Click(object sender, EventArgs e)
        {
            string dataSourceName = txtDbDataSourceName.Text.Replace("\'", "").ToString().Trim();
            string databaseName = txtDbDatabaseName.Text.Replace("\'", "").ToString().Trim();
            if (!string.IsNullOrEmpty(databaseName) && !string.IsNullOrEmpty(dataSourceName))
            {
                CreateDatabase(databaseName, dataSourceName);
            }
            else
            {
                Response.Write("<script>alert('Database Source ve Name alanını boş bırakmayınız.');</script>");
            }
        }

        protected void BtnCreateTable_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_dataSourceName))
            {
                _dataSourceName = txtDbDataSourceName.Text.Replace("\'", "").ToString().Trim();

                if (string.IsNullOrEmpty(_dataSourceName))
                {
                    Response.Write("<script>alert('Database Source alanını boş bırakmayınız.');</script>");
                    return;
                }
            }

            string databaseName = txtDbDatabaseName.Text.Replace("\'", "").ToString().Trim();
            string tableName = txtTblTableName.Text.Replace("\'", "").ToString().Trim();
            string primaryKey = txtTblPrimaryKeyName.Text.Replace("\'", "").ToString().Trim();
            string foreignKey = txtTblForeignKeyName.Text.Replace("\'", "").ToString().Trim();
            string foreingKeyRefTable = txtTblForeignKeyReferanceTable.Text.Replace("\'", "").ToString().Trim();
            string foreignKeyRefColumn = txtTblForeingKeyReferanceColumn.Text.Replace("\'", "").ToString().Trim();

            string[] myArray = hdnTblColumns.Value.Trim().Split(',');
            myArray = myArray.Reverse().Skip(1).Reverse().ToArray();

            if (!string.IsNullOrEmpty(databaseName) || !string.IsNullOrEmpty(tableName))
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
            string dbName = txtGetTblDatabaseName.Text.Replace("\'", "").ToString();
            if (!string.IsNullOrEmpty(dbName))
            {
                GetTables(dbName);
            }
        }

        private void ConnectAndExecuteQuery(string connectionString, string query)
        {
            if (string.IsNullOrEmpty(_dataSourceName))
            {
                _dataSourceName = txtDbDataSourceName.Text.Replace("\'", "").ToString().Trim();
                if (string.IsNullOrEmpty(_dataSourceName))
                {
                    Response.Write("<script>alert('Database Source alanını boş bırakmayınız.');</script>");
                    return;
                }
            }
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

            if (Context != null && (Context.Items["DataSourceName"] == null || string.IsNullOrEmpty(Context.Items["DataSourceName"]?.ToString())))
            {
                Context.Items["DataSourceName"] = _dataSourceName;
            }

            if (HttpContext.Current.Session != null && (HttpContext.Current.Session["DataSourceName"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["DataSourceName"]?.ToString())))
            {
                HttpContext.Current.Session["DataSourceName"] = _dataSourceName;
            }

            NavBar.DataSourceName = _dataSourceName;

            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        private void CreateDatabase(string dbName, string dataSourceName)
        {
            try
            {
                string connectionString = "Data Source=" + dataSourceName + ";Database=master;Trusted_Connection=True;";
                string query = "CREATE DATABASE " + dbName;

                ConnectAndExecuteQuery(connectionString, query);

                _dataSourceName = dataSourceName;
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Lütfen Data Source kısmına doğru giriş yaptığınızdan emin olunuz.');</script>");
            }
        }

        private void CreateTableWithPrimaryKey(string dbName, string tableName, string[] columns, string primaryKey)
        {
            string query = "";
            if (!string.IsNullOrEmpty(primaryKey))
            {
                query = "CREATE TABLE " + tableName + "(" + primaryKey + " int NOT NULL,";

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
                    "(" + primaryKey + " int NOT NULL," +
                    "  [" + foreignKey + "] int NOT NULL," +
                    "PRIMARY KEY CLUSTERED (" + primaryKey + " ASC)";

                foreach (var column in columns)
                {
                    query += column + " nvarchar(250) NULL,";
                }

                query += " CONSTRAINT [PK_" + tableName + "] PRIMARY KEY (" + primaryKey + " ASC)";

                query += "WITH(" +
                    "PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, " +
                    "IGNORE_DUP_KEY = OFF, " +
                    "ALLOW_ROW_LOCKS = ON, " +
                    "ALLOW_PAGE_LOCKS = ON" +
                    ") ON[PRIMARY]) ON[PRIMARY]";

                query += "ALTER TABLE[dbo].[" + tableName + "] WITH CHECK ADD " +
                    "CONSTRAINT[FK_" + tableName + "_" + foreingKeyRefTable + "] " +
                    "FOREIGN KEY([" + foreignKey + "]) " +
                    "REFERENCES[dbo].[" + foreingKeyRefTable + "] ([" + foreignKeyRefColumn + "])";

                query += " ALTER TABLE[dbo].[" + tableName + "] CHECK " +
                    "CONSTRAINT[FK_" + tableName + "_" + foreingKeyRefTable + "]";
            }
            else
            {
                Response.Write("<script>alert('Primary Key alanı boş bırakılamaz');</script>");
            }
            try
            {
                string connectionString = "Data Source=" + _dataSourceName + ";Database=" + dbName + ";Trusted_Connection=True;";
                ConnectAndExecuteQuery(connectionString, query);
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Veri Tabanına bağlanırken bir sorunla karşılaşıldı.');</script>");
            }
        }

        private void GetTables(string dbName)
        {
            /*
            try
            {
                string query = "SELECT TABLE_NAME As Tablolar FROM " + dbName + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

                SqlConnection conn = new SqlConnection("Data Source=" + dataSourceNameG + ";Database=master;Trusted_Connection=True;");
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Close();
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                
                List<string> myTables = new List<string>();

                while (dr.Read())
                {
                    myTables.Add(dr["Tablolar"].ToString());
                }

                SqlDataSourceTables.SelectCommand = query;
                //GridViewTables.DataSource = myTables;
                dr.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            */
        }
    }
}
