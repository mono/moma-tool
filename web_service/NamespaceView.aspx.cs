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

public partial class NamespaceView : System.Web.UI.Page
{
    string query_ns;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            query_ns = Request.QueryString["Namespace"];
            if (query_ns == null)
            {
                query_ns = "System";
            }

            PopulateTree();
            BindData(query_ns);
        }
    }

    private void PopulateTree()
    {
        DataView namespace_data = (DataView)IssueNamespacesSqlDataSource.Select(DataSourceSelectArguments.Empty);
        TreeView tv = NamespaceTreeView;

        tv.PathSeparator = '.';

        for (int i = 0; i < namespace_data.Count; i++)
        {
            string ns = (string)namespace_data[i]["method_namespace"];
            string[] ns_parts = ns.Split('.');
            string valuepath = string.Empty;
            TreeNodeCollection parent_nodes;

            for (int j = 0; j < ns_parts.Length; j++)
            {
                if (valuepath == string.Empty)
                {
                    valuepath = ns_parts[j];
                    parent_nodes = null;
                }
                else
                {
                    parent_nodes = tv.FindNode(valuepath).ChildNodes;
                    valuepath += tv.PathSeparator + ns_parts[j];
                }

                InsertNode(tv, ns_parts[j], ns_parts[j], valuepath, parent_nodes);
            }
        }
    }
    /* value should be empty for classname nodes, namespace name for namespace nodes */
    private void InsertNode(TreeView tv, string text, string value, string valuepath, TreeNodeCollection parent)
    {
        TreeNode node = tv.FindNode(valuepath);
        if (node == null)
        {
            node = new TreeNode(Server.HtmlEncode(text), value);
            if (parent == null)
            {
                tv.Nodes.Add(node);
            }
            else
            {
                parent.Add(node);
            }
            if (valuepath == query_ns)
            {
                node.Selected = true;
            }
            else if (!query_ns.StartsWith(valuepath))
            {
                node.Collapse();
            }
        }
    }

    protected void NamespaceTreeView_SelectedNodeChanged(object sender, EventArgs e)
    {
        TreeView tv = (TreeView)sender;
        TreeNode node = tv.SelectedNode;

        BindData(node.ValuePath);
    }

    private void BindData(string ns)
    {
        StatsLabel.Text = "<h3>Statistics for " + ns + "</h3>";

        NamespaceStatsSqlDataSource.SelectParameters["ns"].DefaultValue = ns;
        StatsDetailsView.DataBind();

        NamespaceIssuesSqlDataSource.SelectParameters["ns"].DefaultValue = ns;
        IssuesGridView.DataBind();

        if (Page.User.Identity.IsAuthenticated)
        {
            GridView gv;
            NamespaceReportsSqlDataSource.SelectParameters["ns"].DefaultValue = ns;

            if (Page.User.IsInRole("Novell"))
            {
                //Novell_ReportsGridView.DataBind();
                gv = (GridView)LoginView1.FindControl("Novell_ReportsGridView");
            }
            else
            {
                //LoggedIn_ReportsGridView.DataBind();
                gv = (GridView)LoginView1.FindControl("LoggedIn_ReportsGridView");
            }
            gv.DataBind();
        }
    }

    protected void ReportsPagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;

        if (Page.User.Identity.IsAuthenticated)
        {
            if (Page.User.IsInRole("Novell"))
            {
                Novell_ReportsGridView.PageSize = int.Parse(ddl.SelectedValue);
            }
            else
            {
                LoggedIn_ReportsGridView.PageSize = int.Parse(ddl.SelectedValue);
            }
        }
    }
    protected void ReportsPagerGotoTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;
        GridView gv;

        if (Page.User.Identity.IsAuthenticated)
        {
            if (Page.User.IsInRole("Novell"))
            {
                gv = Novell_ReportsGridView;
            }
            else
            {
                gv = LoggedIn_ReportsGridView;
            }

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
    protected void ReportsGridView_PreRender(object sender, EventArgs e)
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
    protected void IssuesPagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        IssuesGridView.PageSize = int.Parse(ddl.SelectedValue);
    }
    protected void IssuesPagerGotoTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;

        int pagenum;
        if (int.TryParse(tb.Text.Trim(), out pagenum) &&
            pagenum > 0 &&
            pagenum <= IssuesGridView.PageCount)
        {
            IssuesGridView.PageIndex = pagenum - 1;
        }
        else
        {
            IssuesGridView.PageIndex = 0;
        }
    }
    protected void IssuesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
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
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string method = DataBinder.Eval(e.Row.DataItem, "method_name").ToString();
            int brace_start, brace_end;

            brace_start = method.IndexOf('(');
            brace_end = method.IndexOf(')');

            /* This is all highly dependent on column order :( */
            if (brace_start + 1 < brace_end)
            {
                /* Got some parameters */
                e.Row.Cells[3].Text = method.Substring(0, brace_start + 1) + "...)";
            }
            e.Row.Cells[3].ToolTip = method;

            if (e.Row.Cells[1].Text.Length > 28)
            {
                e.Row.Cells[1].ToolTip = e.Row.Cells[1].Text;
                e.Row.Cells[1].Text = e.Row.Cells[1].Text.Substring(0, 25) + "...";
            }

            if (e.Row.Cells[2].Text.Length > 28)
            {
                e.Row.Cells[2].ToolTip = e.Row.Cells[2].Text;
                e.Row.Cells[2].Text = e.Row.Cells[2].Text.Substring(0, 25) + "...";
            }

            if (e.Row.Cells[3].Text.Length > 28)
            {
                /* Already done ToolTip for this column */
                e.Row.Cells[3].Text = e.Row.Cells[3].Text.Substring(0, 25) + "...";
            }
        }
    }
    protected void IssuesGridView_PreRender(object sender, EventArgs e)
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
