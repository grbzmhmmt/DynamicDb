﻿using System;
using System.Web.UI;

namespace DynamicDb.Pages
{ 
    public partial class Dashboard : Page
    {
        private static string userId;
        private static string userName;
        private static string userPassword;
        private static string databaseName;

        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Request.QueryString["userId"];
            userName= Request.QueryString["userName"];
            userPassword= Request.QueryString["password"];
            databaseName = Request.QueryString["databaseName"];

            if ( string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
            {
                Response.Redirect("NotFound.aspx", true);
            }
        }
    }
}