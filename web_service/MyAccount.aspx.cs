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

public partial class MyAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
        //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log";

        // Content only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            SqlDataSource ds = (SqlDataSource)LoginView1.FindControl("MyReportsSqlDataSource");

            MembershipUser user = Membership.GetUser(Page.User.Identity.Name);
            ds.SelectParameters["email"].DefaultValue = user.Email;
            DataView reports_data = (DataView)ds.Select(DataSourceSelectArguments.Empty);

            GridView reports_gv = (GridView)LoginView1.FindControl("MyReportsGridView");
            reports_gv.DataSource = ds;
            reports_gv.DataBind();
        }
    }
    protected void PagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;

        if (Page.User.Identity.IsAuthenticated)
        {
            GridView gv = (GridView)LoginView1.FindControl("MyReportsGridView");
            gv.PageSize = int.Parse(ddl.SelectedValue);
        }
    }
    protected void PagerGotoTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;

        if (Page.User.Identity.IsAuthenticated)
        {
            GridView gv = (GridView)LoginView1.FindControl("MyReportsGridView");

            int pagenum;
            if (int.TryParse(tb.Text.Trim(), out pagenum) &&
                pagenum > 0 &&
                pagenum <= gv.PageCount)
            {
                gv.PageIndex = pagenum - 1;
            }
            else
            {
                gv.PageIndex = 0;
            }
        }
    }
    protected void ReportsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        GridView gv = (GridView)sender;

        if (gv.SortExpression.Length > 0)
        {
            int cellIndex = -1;

            foreach (DataControlField field in gv.Columns)
            {
                if (field.SortExpression == gv.SortExpression)
                {
                    cellIndex = gv.Columns.IndexOf(field);
                    break;
                }
            }

            if (cellIndex > -1)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // Set alternating row style
                    e.Row.Cells[cellIndex].CssClass = e.Row.RowIndex % 2 == 0 ? "gv_col_sort_alternating" : "gv_col_sort";
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.Pager)
        {
            Label pager_count_label = (Label)e.Row.FindControl("PagerCountLabel");
            pager_count_label.Text = gv.PageCount.ToString();

            TextBox pager_goto_textbox = (TextBox)e.Row.FindControl("PagerGotoTextBox");
            pager_goto_textbox.Text = (gv.PageIndex + 1).ToString();

            DropDownList pager_page_size_ddl = (DropDownList)e.Row.FindControl("PagerPageSizeDropDownList");
            pager_page_size_ddl.SelectedValue = gv.PageSize.ToString();
        }
    }

    public string FormatIssueCount(string count)
    {
        /* count should be either blank (representing zero) or hold an integer */
        if (count == "")
        {
            return "0";
        }
        else
        {
            return count;
        }
    }
    protected void MyReportsGridView_PreRender(object sender, EventArgs e)
    {
        GridView grid = (GridView)sender;
        if (grid != null)
        {
            GridViewRow pagerRow = (GridViewRow)grid.BottomPagerRow;
            if (pagerRow != null)
            {
                pagerRow.Visible = true;
            }
        }
    }
}
