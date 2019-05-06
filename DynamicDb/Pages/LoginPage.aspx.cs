using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace DynamicDb.Pages
{
    public partial class LoginPage : Page
    {
        private string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";

        private string sqlConnection;

        protected void Page_Load(object sender, EventArgs e)
        {
            sqlConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=" + dataFileSourceName + ";" +
            "Initial Catalog=Alpha;" +
            "Integrated Security=True;" +
            "Connect Timeout=30;" +
            "Application Name=DynamicDb";
        }
        
        protected void LoginSystem(object sender, EventArgs e)
        {
            string userId;
            string userName;
            string userPassword;
            string userDatabaseName;

            try
            {
                userName = txtLoginUserName.Text.Replace("\'", "").ToString(); ;
                userPassword = txtLoginPassword.Text.Replace("\'", "").ToString();

                //Temel Database e bağlantı kuruluyor
                SqlConnection conn = new SqlConnection(sqlConnection);

                //ilgili Kullanıcı varmı diye sorgu atılıyor
                string query = "SELECT * FROM Users WHERE UserName='" + userName + "' AND Password='" + userPassword + "'";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();                

                if (dr.Read())//kullanıcı kontrolü
                {
                    userId = dr["UserId"].ToString();

                    userDatabaseName = dr["DatabaseName"].ToString();

                    string directAddress = "";
                    //Kullanıcı veritabanu var mı? Varsa sayfaya onla yönlen.
                    if (userDatabaseName != null && userDatabaseName.Trim() != "")
                    {
                        directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword + "&databaseName= " + userDatabaseName;
                    } else
                    {
                        directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword;
                    }

                    dr.Close();
                    conn.Close();

                    Response.Redirect(directAddress);                   
                }
                else
                {
                    // Hatalar burada gösterilecek
                }
                dr.Close();
                conn.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}