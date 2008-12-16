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

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Login1_LoginError(object sender, EventArgs e)
    {
        System.Web.UI.WebControls.Login login_ctrl = (System.Web.UI.WebControls.Login)LoginView1.FindControl("Login1");
        MembershipUser user = Membership.GetUser(login_ctrl.UserName);

        Label login_error_details = (Label)LoginView1.FindControl("LoginErrorDetails");
        if (user != null)
        {
            if (!user.IsApproved)
            {
                login_error_details.Text = "Your account has not yet been activated.";
            }
            else if (user.IsLockedOut)
            {
                login_error_details.Text = "Your account has been locked due to excessive incorrect login attempts.";
            }
            else
            {
                /* Login control handles other cases */
                login_error_details.Text = "";
            }
        }
    }
    protected void Login1_LoggedIn(object sender, EventArgs e)
    {
        /* Override the ReturnURL for specific pages like Verify.
         */
        string return_url = Request.QueryString["ReturnUrl"];
        if (return_url == null || return_url.Contains("Verify.aspx"))
        {
            Response.Redirect("~/Overview.aspx");
        }
    }
}
