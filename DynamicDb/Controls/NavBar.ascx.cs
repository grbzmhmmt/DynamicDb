using System;
using System.Web;
using System.Web.UI;

namespace DynamicDb.Controls
{
    public partial class NavBar : UserControl
    {
        public string DataSourceName { get; set; }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            btnEditTableLink.NavigateUrl = ("~/Pages/DataManager.aspx?dataSourceName=" + DataSourceName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DataSourceName) &&
                HttpContext.Current.Session != null &&
                HttpContext.Current.Session["DataSourceName"] != null &&
                !string.IsNullOrEmpty(HttpContext.Current.Session["DataSourceName"]?.ToString()))
            {
                DataSourceName = HttpContext.Current.Session["DataSourceName"] as string;
            }
        }
    }
}