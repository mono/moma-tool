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

using System.Drawing;
using ZedGraph;
using ZedGraph.Web;

public partial class Overview : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
        //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log";
    }

    protected override void OnInit(EventArgs e)
    {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent()
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            ZedGraphWeb zg1 = (ZedGraphWeb)LoginView1.FindControl("IssuesPerAppGraph");
            if (zg1 != null)
            {
                zg1.RenderGraph += new ZedGraphWebControlEventHandler(this.OnRenderGraph1);
            }
        }
    }

    DataView graph_data;

    private void OnRenderGraph1(ZedGraphWeb zgw, Graphics g, MasterPane masterPane)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            SqlDataSource ds = (SqlDataSource)LoginView1.FindControl("IssuesPerAppSqlDataSource");
            graph_data = (DataView)ds.Select(DataSourceSelectArguments.Empty);

            GraphPane myPane = masterPane[0];
            PieItem segment1 = myPane.AddPieSlice(0, Color.FromArgb(0xf5, 0xeb, 0xcb), Color.White, 45f, 0, "1-5");
            PieItem segment2 = myPane.AddPieSlice(0, Color.FromArgb(0xec, 0xc5, 0x6b), Color.White, 45f, 0, "6-25");
            PieItem segment3 = myPane.AddPieSlice(0, Color.FromArgb(0xe2, 0x9f, 0x27), Color.White, 45f, 0, "26+");

            segment1.LabelDetail.FontSpec.Size = 20f;
            segment2.LabelDetail.FontSpec.Size = 20f;
            segment3.LabelDetail.FontSpec.Size = 20f;

            for (int i = 0; i < graph_data.Count; i++)
            {
                Int64 view_count = (Int64)graph_data[i]["Count"];

                if (view_count < 6)
                {
                    segment1.Value++;
                }
                else if (view_count < 26)
                {
                    segment2.Value++;
                }
                else
                {
                    segment3.Value++;
                }
            }

            masterPane.AxisChange(g);
        }
    }
}
