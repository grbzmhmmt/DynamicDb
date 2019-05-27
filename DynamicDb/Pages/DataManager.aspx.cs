using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;

namespace DynamicDb.Pages
{
    public partial class DataManager : Page
    {
        private string _dataSourceName;
        private string _tableName;
        private List<string> columnNames;

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
            #region create managerde tiklanan tablo adini al
            if (Request != null && Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["tableName"]))
            {
                _tableName = Request.QueryString["tableName"];
            }
            #endregion

            if (!IsPostBack)
            {
                BindGrid();
                //string connectionString = "Data Source=" + _dataSourceName + ";Database=master;Trusted_Connection=True;";

                //SqlConnection conn = new SqlConnection(connectionString);

                //conn.Open();
                //DataTable dataBaseTables = conn.GetSchema("Tables");
                //conn.Close();
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
                        connectionString = "Data Source=" + _dataSourceName + ";Database=master;Trusted_Connection=True;";
                    }
                    return connectionString;
                }
                return "";
            } else if (!string.IsNullOrEmpty(_dataSourceName))
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
                    connectionString = "Data Source=" + _dataSourceName + ";Database=master;Trusted_Connection=True;";
                }
                return connectionString;
            } else
            {
                return "";
            }
        }

        //private void ConnectAndExecuteQuery(string connectionString, string query)
        //{
        //    SqlConnection conn = new SqlConnection(connectionString);

        //    conn.Open();

        //    SqlCommand cmd = new SqlCommand(query, conn);

        //    SqlDataReader dr = cmd.ExecuteReader();

        //    conn.Close();
        //}

        public string[] GetColumnsName(string tableName)
        {
            string connectionString = GetConnectionString();
            List<string> listacolumnas = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + tableName + "' and t.type = 'U'";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listacolumnas.Add(reader.GetString(0));
                    }
                }
            }
            return listacolumnas.ToArray();
        }

        protected void ButtonEditTable_Click(object sender, EventArgs e)
        {
            string tableName = TextBoxTableName.Text.Trim();
            if (!string.IsNullOrEmpty(tableName))
            {
                string[] columns = GetColumnsName(tableName);
                if (columns.Length > 0)
                {
                    foreach(string column in columns)
                    {
                        BoundField test = new BoundField
                        {
                            DataField = column,
                            HeaderText = column
                        };

                        DataEditGridView.Columns.Add(test);
                    }
                }
            } else
            {
                Response.Write("<script>alert('Tablo adı giriniz.');</script>");
            }
        }

        private void BindGrid()
        {
            if (!string.IsNullOrEmpty(_tableName))
            {
                string connectionString = GetConnectionString();
                string commandString = "select c.name from sys.columns c inner join sys.tables t on t.object_id = c.object_id and t.name = '" + _tableName + "' and t.type = 'U'";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(commandString, conn))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            // cmd.CommandType = CommandType.StoredProcedure;
                            sda.SelectCommand = cmd;
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                DataEditGridView.DataSource = dt;
                                DataEditGridView.DataBind();
                            }
                        }
                    }
                }
            }
        }

        protected void Insert(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string country = txtCountry.Text;

            string connectionString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO t(column_list) " +
                    "VALUES(value_list)";
                //string querystr = "INSERT INTO Users SET User_FirstName=@User_FirstName, User_LastName=@User_LastName WHERE User_ID=@User_ID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "INSERT");
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Country", country);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                //using (SqlCommand cmd = new SqlCommand(query, con))
                //{
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.Parameters.AddWithValue("@Action", "INSERT");
                //    cmd.Parameters.AddWithValue("@Name", name);
                //    cmd.Parameters.AddWithValue("@Country", country);
                //    cmd.Connection = con;
                //    con.Open();
                //    cmd.ExecuteNonQuery();
                //    con.Close();
                //}
            }
            BindGrid();
        }

        protected void OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            DataEditGridView.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            DataEditGridView.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = DataEditGridView.Rows[e.RowIndex];
            int customerId = Convert.ToInt32(DataEditGridView.DataKeys[e.RowIndex].Values[0]);
            string name = (row.FindControl("txtName") as TextBox).Text;
            string country = (row.FindControl("txtCountry") as TextBox).Text;

            string connectionString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customers_CRUD"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "UPDATE");
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Country", country);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            DataEditGridView.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != DataEditGridView.EditIndex)
            //{
            //    (e.Row.Cells[2].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
            //}
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int customerId = Convert.ToInt32(DataEditGridView.DataKeys[e.RowIndex].Values[0]);

            string connectionString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customers_CRUD"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "DELETE");
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            this.BindGrid();
        }
    }
}