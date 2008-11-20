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

public partial class Submissions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void SubmissionsSqlDataSource_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            if (Page.User.IsInRole("Novell"))
            {
                string filter = string.Empty;

                foreach (ListItem item in Novell_ImportanceCheckBoxList.Items)
                {
                    if (item.Selected)
                    {
                        if (filter != string.Empty)
                        {
                            filter += " OR ";
                        }
                        filter += "importance='" + item.Value + "'";
                    }
                }

                if (Novell_AppNameFilterTextBox.Text != string.Empty)
                {
                    if (filter != string.Empty)
                    {
                        filter += " AND ";
                    }

                    filter += "application_name='" + Novell_AppNameFilterTextBox.Text + "'";
                }

                if (Novell_AppTypeFilterTextBox.Text != string.Empty)
                {
                    if (filter != string.Empty)
                    {
                        filter += " AND ";
                    }

                    filter += "application_type='" + Novell_AppTypeFilterTextBox.Text + "'";
                }

                if (Novell_ProfileFilterDropDownList.SelectedItem != null &&
                    Novell_ProfileFilterDropDownList.SelectedItem.Value != "[All]")
                {
                    if (filter != string.Empty)
                    {
                        filter += " AND ";
                    }

                    filter += "display_name='" + Novell_ProfileFilterDropDownList.SelectedItem.Value + "'";
                }

                // If nothing selected, the empty string will filter nothing
                SubmissionsSqlDataSource.FilterExpression = filter;
            }
            else
            {
                string filter = string.Empty;

                if (LoggedIn_AppTypeFilterTextBox.Text != string.Empty)
                {
                    if (filter != string.Empty)
                    {
                        filter += " AND ";
                    }

                    filter += "application_type='" + LoggedIn_AppTypeFilterTextBox.Text + "'";
                }

                if (LoggedIn_ProfileFilterDropDownList.SelectedItem != null &&
                    LoggedIn_ProfileFilterDropDownList.SelectedItem.Value != "[All]")
                {
                    if (filter != string.Empty)
                    {
                        filter += " AND ";
                    }

                    filter += "display_name='" + LoggedIn_ProfileFilterDropDownList.SelectedItem.Value + "'";
                }

                // If nothing selected, the empty string will filter nothing
                SubmissionsSqlDataSource.FilterExpression = filter;
            }
        }
    }
    protected void FilterButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            if (Page.User.IsInRole("Novell"))
            {
                Novell_ReportsGridView.DataBind();
            }
            else
            {
                LoggedIn_ReportsGridView.DataBind();
            }
        }
    }
    protected void ProfileFilterDropDownList_DataBound(object sender, EventArgs e)
    {
        /* Add the 'All' option to the top */
        if (Page.User.Identity.IsAuthenticated)
        {
            if (Page.User.IsInRole("Novell"))
            {
                Novell_ProfileFilterDropDownList.Items.Insert(0, new ListItem("[All]", "[All]"));
            }
            else
            {
                LoggedIn_ProfileFilterDropDownList.Items.Insert(0, new ListItem("[All]", "[All]"));
            }
        }
    }
    protected void PagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
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
    protected void PagerGotoTextBox_TextChanged(object sender, EventArgs e)
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
}
