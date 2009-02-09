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

using AjaxControlToolkit;
using Npgsql;
using System.Collections.Generic;
using System.Collections.Specialized;

public partial class NamespaceView : System.Web.UI.Page
{
    int issue_id = 0;
    string default_ns;
    string default_cls;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string id_qs = Request.QueryString["IssueID"];

            if (id_qs != null)
            {
                issue_id = int.Parse(id_qs);
            }

            if (issue_id != 0)
            {
                this.GetIssueDetails(issue_id);

                // Make the drop down lists show the selected items
                NamespaceDropDownList_CascadingDropDown.SelectedValue = default_ns;
                ClassDropDownList_CascadingDropDown.SelectedValue = default_cls;
                IssueDropDownList_CascadingDropDown.SelectedValue = issue_id.ToString();
            }
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static AjaxControlToolkit.CascadingDropDownNameValue[] GetNamespaceDropDownContents(string knownCategoryValues, string category)
    {
        List<CascadingDropDownNameValue> values = new List<CascadingDropDownNameValue>();

        // Would much rather use an SqlDataSource here, but not sure how to get from a page service
        // method to the instance of the page
        string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstr);
        conn.Open();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT DISTINCT method_namespace FROM issue, issue_type WHERE method_namespace != '' AND issue.issue_type_id = issue_type.id AND issue_type.lookup_name != 'PINV' ORDER BY method_namespace;", conn))
        {
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    string ns = dr.GetString(0);

                    // '<CrtImplementationDetails>' screws up the Ajax toolkit javascript, and HtmlEncoding it
                    // screws up the ParseKnownCategoryValuesString method
                    if (!ns.StartsWith("<"))
                    {
                        values.Add(new CascadingDropDownNameValue(ns, ns));
                    }
                }
            }
        }
        conn.Close();

        return values.ToArray();
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static AjaxControlToolkit.CascadingDropDownNameValue[] GetClassDropDownContents(string knownCategoryValues, string category)
    {
        List<CascadingDropDownNameValue> values = new List<CascadingDropDownNameValue>();
        StringDictionary kv = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
        string ns;

        if (kv.ContainsKey("namespace"))
        {
            ns = kv["namespace"];
        }
        else
        {
            return null;
        }

        string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstr);
        conn.Open();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT DISTINCT method_class FROM issue, issue_type WHERE method_namespace = :ns AND issue.issue_type_id = issue_type.id AND issue_type.lookup_name != 'PINV' ORDER BY method_class;", conn))
        {
            command.Parameters.Add(new NpgsqlParameter("ns", DbType.String));
            command.Parameters[0].Value = ns;

            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    string cls = dr.GetString(0);

                    // Nasty kludge to work around seeming length limits of returned data
                    // (Only System.Windows.Forms has too many classes, so weed these ones out for now)
                    if (!cls.StartsWith("NativeMethods/") && !cls.StartsWith("UnsafeNativeMethods/"))
                    {
                        values.Add(new CascadingDropDownNameValue(cls, cls));
                    }
                }
            }
        }
        conn.Close();

        return values.ToArray();
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static AjaxControlToolkit.CascadingDropDownNameValue[] GetIssueDropDownContents(string knownCategoryValues, string category)
    {
        List<CascadingDropDownNameValue> values = new List<CascadingDropDownNameValue>();
        StringDictionary kv = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues);
        string ns, cls;

        if (kv.ContainsKey("namespace") && kv.ContainsKey("class"))
        {
            ns = kv["namespace"];
            cls = kv["class"];
        }
        else
        {
            return null;
        }

        string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstr);
        conn.Open();

        using (NpgsqlCommand command = new NpgsqlCommand("SELECT issue.id, issue.method_name, issue_type.lookup_name FROM issue, issue_type WHERE issue.method_namespace = :ns AND issue.method_class = :cls AND issue.issue_type_id = issue_type.id AND issue_type.lookup_name != 'PINV' ORDER BY issue.method_name;", conn))
        {
            command.Parameters.Add(new NpgsqlParameter("ns", DbType.String));
            command.Parameters[0].Value = ns;
            command.Parameters.Add(new NpgsqlParameter("cls", DbType.String));
            command.Parameters[1].Value = cls;

            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    int id = dr.GetInt32(0);

                    values.Add(new CascadingDropDownNameValue(dr.GetString(1) + " [" + dr.GetString(2) + "]", id.ToString()));
                }
            }
        }
        conn.Close();

        return values.ToArray();
    }

    protected void IssueDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int id = int.Parse(IssueDropDownList.SelectedValue);

        this.GetIssueDetails(id);
    }

    void GetIssueDetails(int id)
    {
        IssueByQueryIDSqlDataSource.SelectParameters["id"].DefaultValue = id.ToString();
        DataView issue_data = (DataView)IssueByQueryIDSqlDataSource.Select(DataSourceSelectArguments.Empty);
        if (issue_data.Count == 1)
        {
            default_ns = (string)issue_data[0]["method_namespace"];
            default_cls = (string)issue_data[0]["method_class"];

            IssueDetailsView.DataBind();

            DetailsLabel.Visible = true;
            IssueDetailsView.Visible = true;

            if ((bool)issue_data[0]["is_latest_definition"] != true)
            {
                FixedLabel.Visible = true;
            }
            else
            {
                FixedLabel.Visible = false;
            }

            if (Page.User.Identity.IsAuthenticated)
            {
                IssueReportsSqlDataSource.SelectParameters["id"].DefaultValue = id.ToString();

                Label label;
                GridView gv;

                if (Page.User.IsInRole("Novell"))
                {
                    label = (Label)LoginView1.FindControl("Novell_ReportedByLabel");
                    gv = (GridView)LoginView1.FindControl("Novell_ReportsGridView");

                    // Can't reference these directly for some reason (same code works fine in NamespaceView.aspx.cs)
                    //Novell_ReportedByLabel.Visible = true;
                    //Novell_ReportsGridView.DataBind();
                }
                else
                {
                    label = (Label)LoginView1.FindControl("LoggedIn_ReportedByLabel");
                    gv = (GridView)LoginView1.FindControl("LoggedIn_ReportsGridView");
                    //LoggedIn_ReportedByLabel.Visible = true;
                    //LoggedIn_ReportsGridView.DataBind();
                }
                label.Visible = true;
                gv.DataBind();
                gv.Visible = true;
            }
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
}
