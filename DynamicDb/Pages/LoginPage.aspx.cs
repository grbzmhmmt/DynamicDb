using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDb.Pages
{
    public partial class LoginPage : System.Web.UI.Page
    {
        private static string userId;
        private static string userName;
        private static string userPassword;
        private static string userDatabaseName;
        private static string dataSourceName= "DESKTOP - 6UQLI0L";


        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void LoginSystem(object sender, EventArgs e)
        {
            try
            {
                userName = txtLoginUserName.Text.Replace("\'", "").ToString(); ;
                userPassword = txtLoginPassword.Text.Replace("\'", "").ToString(); 

                //Temel Database e bağlantı kuruluyor
                SqlConnection conn = new SqlConnection("Data Source="+dataSourceName+" ;Database=SqlDynamicDbMyUsers;Trusted_Connection=True;");

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

                    dr.Close();
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    string directAddress="";

                    //Kullanıcı nın Database adı olmasına göre dashboarda parametre gönderiliyor
                    if (dr1.Read())
                    {
                        userDatabaseName = dr1["DatabaseName"].ToString();
                        directAddress = "Dashboard.aspx?" + "userId=" + userId + "&userName=" + userName + "&password=" + userPassword + "&databaseName= " + userDatabaseName;
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
            catch (Exception)
            {


            }
        }
    }
}