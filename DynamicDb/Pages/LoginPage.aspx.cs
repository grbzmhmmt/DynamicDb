using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace DynamicDb.Pages
{
    public partial class LoginPage : Page
    {
        private string userId;
        private string userName;
        private string userPassword;
        private string userDatabaseName;
        private string dataSourceName= "DESKTOP-6UQLI0L";
        private string databaseName = "SqlDynamicDbMyUsers";
        private string dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";

        //private string sqlConnection = "Data Source=(LocalDB)/MSSQLLocalDB;" +
        //    "AttachDbFilename=App_Data/DynamicDatabase.mdf;" +
        //    "Integrated Security=True;" +
        //    "Trusted_Connection=True;";

        private string sqlConnection;

        protected void Page_Load(object sender, EventArgs e)
        {
            //sqlConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            //"AttachDbFilename=" + dataFileSourceName + ";" +
            //"Initial Catalog=Alpha;" +
            //"Integrated Security=True;" +
            //"Connect Timeout=30;" +
            //"Application Name=DynamicDb";
        }


        protected void LoginSystem(object sender, EventArgs e)
        {
            try
            {
                userName = txtLoginUserName.Text.Replace("\'", "").ToString(); ;
                userPassword = txtLoginPassword.Text.Replace("\'", "").ToString();
                //Temel Database e bağlantı kuruluyor
                SqlConnection conn = new SqlConnection(
                    "Data Source=" + dataSourceName + "" +
                    " ;Database=" + databaseName +
                    " ;Trusted_Connection=True;"
                );
                //SqlConnection conn = new SqlConnection(sqlConnection);

                //ilgili Kullanıcı varmı diye sorgu atılıyor
                string query = "select * from Users where UserName='" + userName + "' and Password='" + userPassword + "'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

               if (dr.Read())//kullanıcı kontrolü
                {
                    userId = dr["UserId"].ToString();
                    //Kullanıcı nın Database adı için sorgu
                    string query1 = "select DatabaseName from UsersDatabase where UserId= '" + userId + "' and UserName='" + userName + "' and UserPass='" + userPassword + "'";
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    query = "SELECT user FROM " + "Users" + " WHERE UserId = '" + userId + "' AND UserName = '" + userName + "' AND Password = '" + userPassword + "'";

                    cmd1 = new SqlCommand(query1, conn);
                    //cmd1 = new SqlCommand(query, conn);

                    dr.Close();
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    //dr = cmd.ExecuteReader();

                    string directAddress="";

                    //Kullanıcı nın Database adı olmasına göre dashboarda parametre gönderiliyor
                    if (dr1.Read()) // (dr.Read())
                    {
                        userDatabaseName = dr1["DatabaseName"].ToString();
                        //string user = dr["user"].ToString();
                        directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword + "&databaseName= " + userDatabaseName;
                        //directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword + "&databaseName= " + user;
                    }
                    else
                    {
                        directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword;
                    }
                    Response.Redirect(directAddress);
                }
                else
                {
                    // Hatalar burada gösterilecek
                }
                conn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }




        }


    }
}