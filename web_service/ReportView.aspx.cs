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

using MomaTool.Database.Linq;
using Npgsql;
using System.Data.Linq;
using System.Net.Mail;
using System.Text;

public partial class ReportView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Content only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            if (!Page.IsPostBack)
            {
                int id = this.GetID();
                MoMADB db = this.OpenDB();
                Report report = this.GetReport(db, id);
                MomADefinition def = this.GetMomaDefinition(db, report.MomADefinitionID);
                ReportMetadata report_meta = this.GetMetadata(db, id);

                // ... and we need to find it from inside the LoginView
                Label report_id_label = (Label)LoginView1.FindControl("ReportIDLabel");
                report_id_label.Text = "Report " + report.ID;

                DropDownList importance_ddl = (DropDownList)LoginView1.FindControl("ImportanceDropDownList");
                importance_ddl.SelectedValue = report_meta.Importance;

                TextBox application_content_textbox = (TextBox)LoginView1.FindControl("ApplicationContent");
                application_content_textbox.Text = report_meta.ApplicationName;

                Label author_content_label = (Label)LoginView1.FindControl("AuthorContent");
                author_content_label.Text = report.ReporterName;

                Label email_content_label = (Label)LoginView1.FindControl("EmailContent");
                email_content_label.Text = report.ReporterEmail;

                Label website_content_label = (Label)LoginView1.FindControl("WebsiteContent");
                website_content_label.Text = report.ReporterHomepage;

                Label framework_version_content_label = (Label)LoginView1.FindControl("FrameworkVersionContent");
                // I get NullReferenceException here if I try to use report.MomADefinition.DisplayName
                // with dblinq
                framework_version_content_label.Text = def.DisplayName;

                TextBox application_type_content_textbox = (TextBox)LoginView1.FindControl("ApplicationTypeContent");
                application_type_content_textbox.Text = report_meta.ApplicationType;

                var issues_q = (from iss in db.Issue
                                from type in db.IssueType
                                where iss.ReportID == id && iss.IssueTypeID == type.ID
                                select new
                                {
                                    Type = type.LookupName,
                                    Namespace = iss.MethodNamesPace,
                                    ClassName = iss.MethodClass,
                                    Method = iss.MethodName
                                });
                GridView grid1 = (GridView)LoginView1.FindControl("IssuesGridView");
                grid1.DataSource = issues_q;
                grid1.DataBind();

                // Make the default text go away when the user clicks in the Comments box
                // (but ensure that it won't blow away a half-entered comment!)
                TextBox new_comment_textbox = (TextBox)LoginView1.FindControl("NewComment");
                new_comment_textbox.Text = "Type your comment here...";
                new_comment_textbox.Attributes.Add("onFocus", "if (this.value == 'Type your comment here...') this.value='';");

                this.UpdateComments(db, id);

                if (!report.ReporterEmail.Contains("@"))
                {
                    CheckBox send_comment_checkbox = (CheckBox)LoginView1.FindControl("SendCommentCheckBox");

                    /* Hide the email checkbox, as we can't email the submitter anyway... */
                    send_comment_checkbox.Visible = false;
                    send_comment_checkbox.Checked = false;
                }
            }
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            MoMADB db = this.OpenDB();
            Report report = this.GetReport(db, id);
            ReportMetadata report_meta = this.GetMetadata(db, id);

            DropDownList importance_ddl = (DropDownList)LoginView1.FindControl("ImportanceDropDownList");
            report_meta.Importance = importance_ddl.SelectedValue;

            TextBox application_content_textbox = (TextBox)LoginView1.FindControl("ApplicationContent");
            report_meta.ApplicationName = application_content_textbox.Text;

            TextBox application_type_content_textbox = (TextBox)LoginView1.FindControl("ApplicationTypeContent");
            report_meta.ApplicationType = application_type_content_textbox.Text;

            db.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
    }

    Report GetReport(MoMADB db, int id)
    {
        Report report = (from rep in db.Report
                         where rep.ID == id
                         select rep).First();

        return report;
    }

    MomADefinition GetMomaDefinition(MoMADB db, int? def_id)
    {
        MomADefinition definition = (from def in db.MomADefinition
                                     where def.ID == def_id
                                     select def).First();

        return definition;
    }

    ReportMetadata GetMetadata(MoMADB db, int report_id)
    {
        // FirstOrDefault() doesn't work in dblinq, and this can fail if noone has created the
        // metadata entry yet, so we have to fart about with the array instead
        ReportMetadata report_meta;
        var report_meta_q = (from rep_meta in db.ReportMetadata
                             where rep_meta.ReportID == report_id
                             select rep_meta).ToArray();
        if (report_meta_q.Length == 0)
        {
            // Need to create a default metadata entry
            report_meta = new ReportMetadata();
            report_meta.ReportID = report_id;
            report_meta.Importance = "Useful";
            report_meta.ApplicationName = "";
            report_meta.ApplicationType = "";

            db.ReportMetadata.Add(report_meta);
            db.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
        else
        {
            report_meta = report_meta_q[0];
        }

        return report_meta;
    }

    ReportComment[] GetComments(MoMADB db, int report_id)
    {
        ReportComment[] comment_q = (from comm in db.ReportComment
                                     where comm.ReportID == report_id
                                     orderby comm.CommentDate ascending
                                     select comm).ToArray();

        return comment_q;
    }

    MoMADB OpenDB()
    {
        string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstr);
        MoMADB db = new MoMADB(conn);

        return db;
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

    void UpdateComments(MoMADB db, int report_id)
    {
        ReportComment[] comments = this.GetComments(db, report_id);
        TextBox comments_textbox = (TextBox)LoginView1.FindControl("Comments");
        comments_textbox.Text = "";

        foreach (ReportComment comm in comments)
        {
            comments_textbox.Text += comm.Comment;
            comments_textbox.Text += "\n";
            comments_textbox.Text += comm.CommentER;
            comments_textbox.Text += " @ ";
            comments_textbox.Text += comm.CommentDate;
            comments_textbox.Text += "\n\n\n";
        }

        /* FIXME: figure out how to scroll to the end of the textbox */
    }

    protected void ApplicationContent_TextChanged(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            MoMADB db = this.OpenDB();
            Report report = this.GetReport(db, id);
            ReportMetadata report_meta = this.GetMetadata(db, id);

            TextBox application_content_textbox = (TextBox)LoginView1.FindControl("ApplicationContent");
            report_meta.ApplicationName = application_content_textbox.Text;

            db.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
    }
    protected void ApplicationType_TextChanged(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            MoMADB db = this.OpenDB();
            Report report = this.GetReport(db, id);
            ReportMetadata report_meta = this.GetMetadata(db, id);

            TextBox application_type_content_textbox = (TextBox)LoginView1.FindControl("ApplicationTypeContent");
            report_meta.ApplicationType = application_type_content_textbox.Text;

            db.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
    }
    protected void Importance_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            MoMADB db = this.OpenDB();
            Report report = this.GetReport(db, id);
            ReportMetadata report_meta = this.GetMetadata(db, id);

            DropDownList importance_ddl = (DropDownList)LoginView1.FindControl("ImportanceDropDownList");
            report_meta.Importance = importance_ddl.SelectedValue;

            db.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
    }
    protected void CommentButton_Click(object sender, EventArgs e)
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            int id = this.GetID();
            MoMADB db = this.OpenDB();
            ReportComment comment = new ReportComment();
            CheckBox send_comment_checkbox = (CheckBox)LoginView1.FindControl("SendCommentCheckBox");
            TextBox newcomment_textbox = (TextBox)LoginView1.FindControl("NewComment");
            Label email_content_label = (Label)LoginView1.FindControl("EmailContent");

            comment.Comment = newcomment_textbox.Text;
            comment.ReportID = id;
            comment.CommentER = Page.User.Identity.Name;
            comment.CommentDate = DateTime.Now;
            comment.EmailEd = send_comment_checkbox.Checked;

            db.ReportComment.Add(comment);
            db.SubmitChanges(ConflictMode.FailOnFirstConflict);

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

            this.UpdateComments(db, id);
            newcomment_textbox.Text = "";
        }
    }
}
