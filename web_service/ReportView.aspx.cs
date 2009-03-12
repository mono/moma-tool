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

using System.Net.Mail;
using System.Text;
using System.Data.Common;

public partial class ReportView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
        //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log";

        // Content only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            SqlDataSource ds = (SqlDataSource)LoginView1.FindControl("MetadataSqlDataSource");
            DataView metadata_data = (DataView)ds.Select(DataSourceSelectArguments.Empty);

            if (metadata_data.Count == 0)
            {
                SendCommentCheckBox.Visible = false;
                SendCommentCheckBox.Checked = false;
                CommentButton.Visible = false;
                NewComment.Visible = false;
                Comments.Visible = false;
                CommentsLabel.Visible = false;
                ReportDetailsLabel.Visible = false;
                IssuesGridView.Visible = false;
                CurrentIssuesGridView.Visible = false;
                ReportIssuesLabel.Visible = false;
                ReportCurrentIssuesLabel.Visible = false;

                NoSuchReportLabel.Visible = true;
            }

#if DISABLE_FOR_NOW
            MembershipUser user = Membership.GetUser(Page.User.Identity.Name);
            Label email_content_label = (Label)LoginView1.FindControl("EmailContent");

            if (!Page.User.IsInRole("Novell") &&
                user.Email != email_content_label.Text)
            {
                /* Logged-in user, but not the owner of the report */
            }
#endif

            if (!Page.IsPostBack)
            {
                // Make the default text go away when the user clicks in the Comments box
                // (but ensure that it won't blow away a half-entered comment!)
                NewComment.Text = "Type your comment here...";
                NewComment.Attributes.Add("onFocus", "if (this.value == 'Type your comment here...') this.value='';");

                this.UpdateComments();
            }
        }
    }

    void UpdateComments()
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            DataView comments_data = (DataView)CommentsSqlDataSource.Select(DataSourceSelectArguments.Empty);
            Comments.Text = "";

            for (int i = 0; i < comments_data.Count; i++)
            {
                Comments.Text += comments_data[i]["comment"];
                Comments.Text += "\n";
                Comments.Text += comments_data[i]["commenter"];
                Comments.Text += " @ ";
                Comments.Text += comments_data[i]["comment_date"];
                Comments.Text += "\n\n\n";
            }
        }

        /* FIXME: figure out how to scroll to the end of the textbox */
    }

    int GetID()
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

        return id;
    }

    protected void CommentButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            Label email_content_label = (Label)LoginView1.FindControl("EmailContent");

            CommentsSqlDataSource.InsertParameters["id"].DefaultValue = id.ToString();
            CommentsSqlDataSource.InsertParameters["comment"].DefaultValue = NewComment.Text;
            CommentsSqlDataSource.InsertParameters["commenter"].DefaultValue = Page.User.Identity.Name;
            CommentsSqlDataSource.InsertParameters["comment_date"].DefaultValue = DateTime.Now.ToString("o"); // ISO format
            CommentsSqlDataSource.InsertParameters["emailed"].DefaultValue = SendCommentCheckBox.Checked.ToString();
            CommentsSqlDataSource.Insert();

            if (SendCommentCheckBox.Checked)
            {
                /* Email the comment (we know the address is available, as the checkbox can't be
                 * checked otherwise)
                 */
                StringBuilder email_text = new StringBuilder();

                email_text.AppendFormat("{0} added a comment to your MoMA report:\n\n", Page.User.Identity.Name);
                email_text.AppendFormat("{0}", NewComment.Text);
                email_text.AppendFormat("\n\nSee {0} for this report.\n", Page.Request.Url.ToString());

                string from_addr = Membership.GetUser().Email;

                try
                {
                    MailMessage mess = new MailMessage(from_addr, "dick@acm.org"/*email_content_label.Text*/);
                    mess.Subject = "New comment on MoMA report " + id.ToString();
                    mess.Body = email_text.ToString();
                    mess.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();
                    try
                    {
                        smtp.Send(mess);
                    }
                    catch (SmtpException ex)
                    {
                        /* Not sure what we can do here, except inform the commenter that the email didn't
                         * get sent
                         */
                    }
                }
                catch (Exception ex)
                {
                    /* MailMessage.ctor() can throw if the email address are malformed - again, not
                     * sure what can be done here
                     */
                }
            }

            /* This reads the entire set of comments - someone else might have added a comment to
             * this report simultaneously
             */
            this.UpdateComments();
            NewComment.Text = "";
        }
    }

    protected void ReportDetailsView_DataBound(object sender, EventArgs e)
    {
        DetailsView dv = (DetailsView)sender;

        if (Page.User.Identity.IsAuthenticated)
        {
            /* Might be null if report not found in DB */
            Label email_content_label = (Label)dv.FindControl("EmailContent");

            if (email_content_label == null || !email_content_label.Text.Contains("@"))
            {
                /* Hide the email checkbox, as we can't email the submitter anyway... */
                SendCommentCheckBox.Visible = false;
                SendCommentCheckBox.Checked = false;
            }
        }
    }
    protected void ReportedIssuesPagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        IssuesGridView.PageSize = int.Parse(ddl.SelectedValue);
    }
    protected void ReportedIssuesPagerGotoTextBox_TextChanged(object sender, EventArgs e)
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

            if (brace_start + 1 < brace_end)
            {
                /* Got some parameters */
                e.Row.Cells[4].Text = method.Substring(0, brace_start + 1) + "...)";
            }
            e.Row.Cells[4].ToolTip = method;
        }
    }
    protected void CurrentIssuesPagerPageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddl = (DropDownList)sender;
        CurrentIssuesGridView.PageSize = int.Parse(ddl.SelectedValue);
    }
    protected void CurrentIssuesPagerGotoTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)sender;

        int pagenum;
        if (int.TryParse(tb.Text.Trim(), out pagenum) &&
            pagenum > 0 &&
            pagenum <= CurrentIssuesGridView.PageCount)
        {
            CurrentIssuesGridView.PageIndex = pagenum - 1;
        }
        else
        {
            CurrentIssuesGridView.PageIndex = 0;
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
    protected void ReportIDTextBox_TextChanged(object sender, EventArgs e)
    {
        TextBox reportid_tb = (TextBox)LoginView1.FindControl("ReportIDTextBox");
        int new_id = Int32.Parse(reportid_tb.Text);

        if (new_id > 0)
        {
            UriBuilder redirect = new UriBuilder(Page.Request.Url);
            redirect.Query = "ReportID=" + new_id.ToString();

            Page.Response.Redirect(redirect.ToString(), true);
        }
    }
}
