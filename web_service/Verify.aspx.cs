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

using System.Text.RegularExpressions;

public partial class Verify : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
        //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log";

        if (!Page.User.Identity.IsAuthenticated)
        {
            Label info_label = (Label)LoginView1.FindControl("InformationLabel");

            if (String.IsNullOrEmpty(Request.QueryString["ID"]) ||
                !Regex.IsMatch(Request.QueryString["ID"], "[0-9a-f]{8}-([0-9a-f]{4}-){3}[0-9a-f]{12}"))
            {
                info_label.Text = "An invalid ID was supplied.";
            }
            else
            {
                Guid user_id = new Guid(Request.QueryString["ID"]);
                MembershipUser user = Membership.GetUser(user_id);

                if (user == null)
                {
                    /* Couldn't find user */
                    info_label.Text = "The ID was not recognized.";
                }
                else
                {
                    user.IsApproved = true;
                    Membership.UpdateUser(user);

                    info_label.Text = "Your account has been verified and you may now log on to the site.";
                }
            }
        }
    }
}
