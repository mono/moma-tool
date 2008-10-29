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

            // If nothing selected, the empty string will filter nothing
            IssuesSqlDataSource.FilterExpression = filter;
        }
    }
    protected void IssueTypeFilterButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            IssuesGridView.DataBind();
        }
    }
}
