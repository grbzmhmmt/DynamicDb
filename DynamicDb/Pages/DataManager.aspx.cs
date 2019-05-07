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
        private List<string> columnNames;

        protected void Page_Load(object sender, EventArgs e)
        {
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

            if (!IsPostBack)
            {
                //string connectionString = GetConnectionString();
                //SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('TABLE_NAME')
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

        private void ConnectAndExecuteQuery(string connectionString, string query)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();

            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader dr = cmd.ExecuteReader();

            conn.Close();
        }

        private void BindGrid()
        {
            string connectionString = GetConnectionString();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Customers_CRUD"))
                {
                    cmd.Parameters.AddWithValue("@Action", "SELECT");
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            GridView1.DataSource = dt;
                            GridView1.DataBind();
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
            GridView1.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }

        protected void OnRowCancelingEdit(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            int customerId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
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
            GridView1.EditIndex = -1;
            this.BindGrid();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != GridView1.EditIndex)
            {
                (e.Row.Cells[2].Controls[2] as LinkButton).Attributes["onclick"] = "return confirm('Do you want to delete this row?');";
            }
        }

        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int customerId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

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