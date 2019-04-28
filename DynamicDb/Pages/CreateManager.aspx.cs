using System;
using System.Data.SqlClient;
using System.Linq;

namespace DynamicDb.Pages
{
    public partial class CreateManager : System.Web.UI.Page
    {
        protected string dataSourceNameG = "DESKTOP-6UQLI0L";
        protected string databaseNameG;
        protected string tableNameG;
        protected string primaryKeyG;
        protected string foreignKeyG;
        protected string foreingKeyRefTableG;
        protected string foreignKeyRefColumnG;


        protected void Page_Load(object sender, EventArgs e)
        {
            txtDbDataSourceName.Text = dataSourceNameG;
        }

        protected void BtnCreateTable(object sender, EventArgs e)
        {
            databaseNameG = txtTblDatabaseName.Text.Replace("\'", "").ToString();
            tableNameG = txtTblTableName.Text.Replace("\'", "").ToString();
            primaryKeyG = txtTblPrimaryKeyName.Text.Replace("\'", "").ToString();
            foreignKeyG = txtTblForeignKeyName.Text.Replace("\'", "").ToString();
            foreingKeyRefTableG = txtTblForeignKeyReferanceTable.Text.Replace("\'", "").ToString();
            foreignKeyRefColumnG = txtTblForeingKeyReferanceColumn.Text.Replace("\'", "").ToString();

            string[] myArray = hdnTblColumns.Value.Split(',');
            myArray = myArray.Reverse().Skip(1).Reverse().ToArray();
            bool isQuerySuccess = true;

            if (!string.IsNullOrEmpty(foreignKeyG) && !string.IsNullOrEmpty(foreingKeyRefTableG) && !string.IsNullOrEmpty(foreignKeyRefColumnG))
            {
                try
                {
                    CreateTableWithForeignKey(databaseNameG, tableNameG, myArray, primaryKeyG, foreignKeyG, foreingKeyRefTableG, foreignKeyRefColumnG);
                }
                catch (Exception)
                {

                    Response.Write("<script>alert('Foreign Hatası');</script>");
                }
            }
            else
            {
                try
                {
                    isQuerySuccess = CreateTableWithPrimaryKey(databaseNameG, tableNameG, myArray, primaryKeyG);

                }
                catch (Exception)
                {
                    Response.Write("<script>alert('PrimaryKey Hatası');</script>");
                }
            }


        }

        protected Boolean CreateTableWithPrimaryKey(string dbName, string tableName, string[] columns, string primaryKey)
        {
            string query = "";
            if (!string.IsNullOrEmpty(primaryKey))
            {
                query = "CREATE TABLE " + tableName +
                    "(" + primaryKey + " int NOT NULL,";

                foreach (var column in columns)
                {
                    query += column + " char(250) NULL,";
                }

                query += " CONSTRAINT [PK_" + tableName + "] PRIMARY KEY (" + primaryKey + " ASC))";

                /*"WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF," +
                " ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] GO";
                */
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

                SqlConnection conn = new SqlConnection("Data Source=" + dataSourceNameG + ";Database=" + dbName + ";Trusted_Connection=True;");

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected Boolean CreateTableWithForeignKey(string dbName, string tableName, string[] columns, string primaryKey, string foreignKey, string foreingKeyRefTable, string foreignKeyRefColumn)
        {
            string query = "";
            if (!string.IsNullOrEmpty(primaryKey))
            {
                query = "CREATE TABLE " + tableName +
                    "(" + primaryKey + " int NOT NULL," +
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
            else { return false; }
            try
            {
                SqlConnection conn = new SqlConnection("Data Source=" + dataSourceNameG + ";Database=" + dbName + ";Trusted_Connection=True;");

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Close();
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        protected void btnCreateDb_Click(object sender, EventArgs e)
        {
            string dbName = txtDbDatabaseName.Text.Replace("\'", "").ToString();
            string dataSourceName = txtDbDataSourceName.Text.Replace("\'", "").ToString();
            CreateDatabase(dbName, dataSourceName);

        }

        protected void CreateDatabase(string dbName, string dataSourceName)
        {
            string query = "";
            SqlConnection conn;
            if (!string.IsNullOrEmpty(dbName))
            {
                if (!string.IsNullOrEmpty(dataSourceName))
                {
                    conn = new SqlConnection("Data Source=" + dataSourceName + ";Database=master;Trusted_Connection=True;");
                }
                else
                {
                    conn = new SqlConnection("Data Source=" + dataSourceNameG + ";Database=master;Trusted_Connection=True;");
                }

                try
                {
                    query = "CREATE DATABASE " + dbName;

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception)
                {
                    Response.Write("<script>alert('Lütfen Database İsmi Giriniz');</script>");
                }
            }


        }

        protected void getTables_Click(object sender, EventArgs e)
        {
            string dbName= txtGetTblDatabaseName.Text.Replace("\'", "").ToString();
            if (!string.IsNullOrEmpty(dbName))
            {
                GetTables(dbName);
            }


        }

        protected void GetTables(string dbName)
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


