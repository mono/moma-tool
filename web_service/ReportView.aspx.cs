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
                /* Need to create this metadata entry */
                int id = this.GetID();
                ds.InsertParameters["id"].DefaultValue = id.ToString();
                ds.Insert();
            }

            if (!Page.IsPostBack)
            {
                // Make the default text go away when the user clicks in the Comments box
                // (but ensure that it won't blow away a half-entered comment!)
                TextBox new_comment_textbox = (TextBox)LoginView1.FindControl("NewComment");
                new_comment_textbox.Text = "Type your comment here...";
                new_comment_textbox.Attributes.Add("onFocus", "if (this.value == 'Type your comment here...') this.value='';");

                this.UpdateComments();
            }
        }
    }

    void UpdateComments()
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            SqlDataSource ds = (SqlDataSource)LoginView1.FindControl("CommentsSqlDataSource");
            DataView comments_data = (DataView)ds.Select(DataSourceSelectArguments.Empty);
            TextBox comments_textbox = (TextBox)LoginView1.FindControl("Comments");
            comments_textbox.Text = "";

            for (int i = 0; i < comments_data.Count; i++)
            {
                comments_textbox.Text += comments_data[i]["comment"];
                comments_textbox.Text += "\n";
                comments_textbox.Text += comments_data[i]["commenter"];
                comments_textbox.Text += " @ ";
                comments_textbox.Text += comments_data[i]["comment_date"];
                comments_textbox.Text += "\n\n\n";
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
            CheckBox send_comment_checkbox = (CheckBox)LoginView1.FindControl("SendCommentCheckBox");
            TextBox newcomment_textbox = (TextBox)LoginView1.FindControl("NewComment");
            Label email_content_label = (Label)LoginView1.FindControl("EmailContent");

            SqlDataSource ds = (SqlDataSource)LoginView1.FindControl("CommentsSqlDataSource");
            ds.InsertParameters["id"].DefaultValue = id.ToString();
            ds.InsertParameters["comment"].DefaultValue = newcomment_textbox.Text;
            ds.InsertParameters["commenter"].DefaultValue = Page.User.Identity.Name;
            ds.InsertParameters["comment_date"].DefaultValue = DateTime.Now.ToString("o"); // ISO format
            ds.InsertParameters["emailed"].DefaultValue = send_comment_checkbox.Checked.ToString();
            ds.Insert();

            if (send_comment_checkbox.Checked)
            {
                /* Email the comment (we know the address is available, as the checkbox can't be
                 * checked otherwise)
                 */
                StringBuilder email_text = new StringBuilder();

                email_text.AppendFormat("{0} added a comment to your MoMA report:\n\n", Page.User.Identity.Name);
                email_text.AppendFormat("{0}", newcomment_textbox.Text);
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
            newcomment_textbox.Text = "";
        }
    }

    protected void ReportDetailsView_DataBound(object sender, EventArgs e)
    {
        DetailsView dv = (DetailsView)sender;

        if (Page.User.Identity.IsAuthenticated)
        {
            Label email_content_label = (Label)dv.FindControl("EmailContent");

            if (!email_content_label.Text.Contains("@"))
            {
                CheckBox send_comment_checkbox = (CheckBox)LoginView1.FindControl("SendCommentCheckBox");

                /* Hide the email checkbox, as we can't email the submitter anyway... */
                send_comment_checkbox.Visible = false;
                send_comment_checkbox.Checked = false;
            }
        }
    }
}
