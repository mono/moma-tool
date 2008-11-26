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
        //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
        //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log";

        DataView graph_data = (DataView)IssuesPerAppSqlDataSource.Select(DataSourceSelectArguments.Empty);
        int segment1 = 0;
        int segment2 = 0;
        int segment3 = 0;

        for (int i = 0; i < graph_data.Count; i++)
        {
            Int64 view_count = (Int64)graph_data[i]["Count"];

            if (view_count < 6)
            {
                segment1++;
            }
            else if (view_count < 26)
            {
                segment2++;
            }
            else
            {
                segment3++;
            }
        }

        /* Google's scaling option doesn't appear to work :( */
        int total = segment1 + segment2 + segment3;
        int scale1 = segment1 * 100 / total;
        int scale2 = segment2 * 100 / total;
        int scale3 = segment3 * 100 / total;

        /* %2B is '+' which otherwise doesn't show up */
        IssuesPerAppGraphImage.ImageUrl = "http://chart.apis.google.com/chart?chs=200x150&cht=p&chl=1-5|6-25|26%2B&chd=t:" + scale1 + "," + scale2 + "," + scale3;
    }
    protected void MostNeededGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string method = DataBinder.Eval(e.Row.DataItem, "method_name").ToString();
            int brace_start, brace_end;

            brace_start = method.IndexOf('(');
            brace_end = method.IndexOf(')');

            if (brace_start + 1 < brace_end)
            {
                /* Got some parameters */
                e.Row.Cells[2].Text = method.Substring(0, brace_start + 1) + "...)";
            }
            e.Row.Cells[2].ToolTip = method;
        }
    }
}
