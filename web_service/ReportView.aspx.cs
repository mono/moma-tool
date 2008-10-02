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

using MomaTool.Database.Linq;
using Npgsql;

public partial class ReportView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // GridView1 only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            string id_qs = Request.QueryString["ReportID"];
            int id;

            if (id_qs != null)
            {
                id = int.Parse(id_qs);
            }
            else
            {
                id = 1;
            }
            
            string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            MoMADB db = new MoMADB(conn);

            // ... and we need to find it from inside the LoginView
            GridView grid = (GridView)LoginView1.FindControl("GridView1");
            grid.DataSource = (from rep in db.Report
                               where rep.ID == id
                               select new {
                                   ID = rep.ID,
                                   ReportFileName = rep.ReportFilename,
                                   ReporterName = rep.ReporterName
                               });
            grid.DataBind();
        }
    }
}
