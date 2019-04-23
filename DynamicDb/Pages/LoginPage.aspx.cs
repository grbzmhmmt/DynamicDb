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
        protected void Page_Load(object sender, EventArgs e)
        {


        }


        protected void LoginSystem(object sender, EventArgs e)
        {
            string userName = txtLoginUserName.Text;
            string pass = txtLoginPassword.Text;
            
            SqlConnection conn = new SqlConnection("Data Source= DESKTOP-6UQLI0L;Database=SqlDynamicDbMyUsers;Trusted_Connection=True;");
            string query = "select * from Users where UserName='" + userName + "' and Password='" + pass + "'";
            SqlCommand cmd = new SqlCommand(query, conn);
            //conn.Close();
            conn.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            
            List<string> myTables = new List<string>();

            if (dr.Read())
            {
                Response.Redirect("Dashboard.aspx?"+"user="+ txtLoginUserName.Text + "-" + txtLoginPassword.Text);

            }
            else
            {
                // Hatalar burada gösterilecek
            }

            conn.Close();


        }


    }
}