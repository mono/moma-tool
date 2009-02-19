using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using System.Net;
using System.IO;
using System.Text;
using System.Configuration;

/// <summary>
/// This service proxies calls from the client to the disqus service, to get around cross-domain
/// restrictions in the browser
/// </summary>
[WebService(Namespace = "http://mono-project.com/moma")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class DisqusProxy : System.Web.Services.WebService {

    public DisqusProxy () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool EnsureThreadCreated(string title, string message, string thread_identifier) {
        try
        {
            string forum_api_key = ConfigurationManager.ConnectionStrings["DisqusForumAPI"].ConnectionString;
            string post_data = "forum_api_key=" + HttpUtility.UrlEncode(forum_api_key) +
                "&title=" + HttpUtility.UrlEncode(title) +
                "&identifier=" + HttpUtility.UrlEncode(thread_identifier);
            UTF8Encoding enc = new UTF8Encoding();

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://disqus.com/api/thread_by_identifier/");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = enc.GetByteCount(post_data);

            using (Stream req_stream = req.GetRequestStream())
            {
                req_stream.Write(enc.GetBytes(post_data), 0, enc.GetByteCount(post_data));
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            string resp_str = "";

            // We don't really care what the response is, apart from success/fail
            // And no need to parse the JSON response, so long as it says '"succeeded": true' somewhere

            using (Stream resp_stream = resp.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(resp_stream))
                {
                    resp_str = reader.ReadToEnd();
                }
            }

            return resp_str.Contains("\"succeeded\": true");
        }
        catch
        {
            return false;
        }
    }
}

