﻿using System;
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
        int segment4 = 0;

        for (int i = 0; i < graph_data.Count; i++)
        {
            Int32 view_count = (Int32)graph_data[i]["Count"];

            if (view_count == 0)
            {
                segment4++;
            }
            else if (view_count < 6)
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
        int total = segment1 + segment2 + segment3 + segment4;
        int scale1 = segment1 * 100 / total;
        int scale2 = segment2 * 100 / total;
        int scale3 = segment3 * 100 / total;
        int scale4 = segment4 * 100 / total;

        /* %2B is '+' which otherwise doesn't show up */
        IssuesPerAppGraphImage.ImageUrl = "http://chart.apis.google.com/chart?chs=200x150&cht=p&chl=0|1-5|6-25|26%2B&chd=t:" + scale4 + "," + scale1 + "," + scale2 + "," + scale3;

        graph_data = (DataView)IssuesPerAppNowSqlDataSource.Select(DataSourceSelectArguments.Empty);
        segment1 = 0;
        segment2 = 0;
        segment3 = 0;
        segment4 = 0;

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

        /* This datasource doesn't give us the 0 items, so work it out from the total we counted
         * previously
         */
        segment4 = total - (segment1 + segment2 + segment3);

        /* Google's scaling option doesn't appear to work :( */
        scale1 = segment1 * 100 / total;
        scale2 = segment2 * 100 / total;
        scale3 = segment3 * 100 / total;
        scale4 = segment4 * 100 / total;

        /* %2B is '+' which otherwise doesn't show up */
        IssuesPerAppNowGraphImage.ImageUrl = "http://chart.apis.google.com/chart?chs=200x150&cht=p&chl=0|1-5|6-25|26%2B&chd=t:" + scale4 + "," + scale1 + "," + scale2 + "," + scale3;
    }
}
