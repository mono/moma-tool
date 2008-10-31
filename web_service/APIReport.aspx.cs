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

public partial class APIReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void IssuesSqlDataSource_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            string filter = string.Empty;

            foreach (ListItem item in IssueTypeFilterListBox.Items)
            {
                if (item.Selected)
                {
                    if (filter != string.Empty)
                    {
                        filter += " OR ";
                    }
                    filter += "lookup_name='" + item.Value + "'";
                }
            }

            if (IssueNamespaceFilterTextBox.Text != string.Empty)
            {
                if (filter != string.Empty)
                {
                    filter += " AND ";
                }

                filter += "method_namespace='" + IssueNamespaceFilterTextBox.Text + "'";
            }

            if (IssueClassFilterTextBox.Text != string.Empty)
            {
                if (filter != string.Empty)
                {
                    filter += " AND ";
                }

                filter += "method_class='" + IssueClassFilterTextBox.Text + "'";
            }

            // If nothing selected, the empty string will filter nothing
            IssuesSqlDataSource.FilterExpression = filter;
        }
    }
    protected void IssueFilterButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            IssuesGridView.DataBind();
        }
    }
    protected void PagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        IssuesGridView.PageSize = int.Parse(ddl.SelectedValue);
    }
    protected void PagerGotoTextBox_TextChanged(object sender, EventArgs e)
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
}
