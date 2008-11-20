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

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void CreateUserWizard1_SendingMail(object sender, MailMessageEventArgs e)
    {
        CreateUserWizard cuw = (CreateUserWizard)LoginView1.FindControl("CreateUserWizard1");
        MembershipUser user = Membership.GetUser(cuw.UserName);
        string verify_url = Request.Url.GetLeftPart(UriPartial.Authority) + Page.ResolveUrl("~/Verify.aspx?ID=" + user.ProviderUserKey.ToString());

        e.Message.Body = e.Message.Body.Replace("<%VerifyUrl%>", verify_url);
    }
}
