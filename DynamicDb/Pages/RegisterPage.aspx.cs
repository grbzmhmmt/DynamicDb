using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDb.Pages
{
    public partial class RegisterPage : System.Web.UI.Page
    {
        private string _dataFileSourceName = "C:\\Github\\DynamicDb\\DynamicDb\\App_Data\\DynamicDatabase.mdf";

        private string _sqlConnection;

        protected void Page_Load(object sender, EventArgs e)
        {
            _sqlConnection = "Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=" + _dataFileSourceName + ";" +
            "Initial Catalog=Alpha;" +
            "Integrated Security=True;" +
            "Connect Timeout=30;" +
            "Application Name=DynamicDb";
        }

        protected void RegisterSystem(object sender, EventArgs e)
        {
            //Kullanici adi, sifre al.
            string userName = txtRegisterUserName.Text.Trim();
            string userPassword = txtRegisterPassword.Text.Trim();

            //Kullanici adi, sifre bos mu?
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userPassword))
            {
                //Sql Baglantisini olustur.
                SqlConnection sqlConnection = new SqlConnection(_sqlConnection);
                //ilgili Kullanıcı onceden eklenmis mi? sorgusunu olustur.
                string query0 = "SELECT * FROM Users WHERE UserName='" + userName + "' AND Password='" + userPassword + "'";

                SqlCommand cmd = new SqlCommand(query0, sqlConnection);
                sqlConnection.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())//kullanıcı kontrolü
                {
                    //Sql Baglantisini kapat
                    sqlConnection.Close();
                    //Insert sorgusu
                    string query1 = "insert into Users(UserName, Password) values (" +
                        "'" + userName + "'," +
                        "'" + userPassword + "'" +
                    ")";

                    SqlCommand sqlCommand = new SqlCommand(query1, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    //Sql Baglantisini kapat
                    sqlConnection.Close();

                    try
                    {
                        Response.Redirect("Dashboard.aspx");
                    }
                    catch (Exception exc0)
                    {
                        Console.WriteLine(exc0);
                        try
                        {
                            Response.Redirect("Dashboard.aspx");
                        }
                        catch (Exception exc1)
                        {
                            Console.WriteLine(exc1);
                            //RegisterPage.aspx sayfasindaki toRedirect fonksiyonunu cagir
                            ClientScript.RegisterStartupScript(this.GetType(), "Redirect", "toRedirect('Dashboard.aspx')", true);
                        }
                    }
                } else
                {
                    Response.Write("<script>alert('Böyle bir kullanıcı zaten var.');</script>");
                }
            }
        }
    }
}