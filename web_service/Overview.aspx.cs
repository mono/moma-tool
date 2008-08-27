using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Overview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // GridView1 only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            // ... and we need to find it from inside the LoginView
            GridView grid = (GridView)LoginView1.FindControl("GridView1");
            grid.DataSource = MomaTool.Database.Report.FindMostRecent(20);
            grid.DataBind();
        }
    }
}
