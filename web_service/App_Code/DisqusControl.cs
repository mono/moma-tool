using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// A control to embed Disqus comments on a page, that copes with AJAX update panels
/// </summary>

namespace MoMATool
{
    public class DisqusControl : WebControl, IScriptControl
    {
        private string _disqus_forum;
        private bool _disqus_developer;
        private string _disqus_container_id;
        private string _disqus_url;
        private string _disqus_title;
        private string _disqus_message;
        private string _disqus_identifier;
        private string _width;
        private ScriptManager sm;

        public string DisqusForum
        {
            get
            {
                return _disqus_forum;
            }
            set
            {
                _disqus_forum = value;
            }
        }

        public bool DisqusDeveloper
        {
            get
            {
                return _disqus_developer;
            }
            set
            {
                _disqus_developer = value;
            }
        }

        public string DisqusContainerId
        {
            get
            {
                return _disqus_container_id;
            }
            set
            {
                _disqus_container_id = value;
            }
        }

        public string DisqusURL
        {
            get
            {
                return _disqus_url;
            }
            set
            {
                _disqus_url = value;
            }
        }

        public string DisqusTitle
        {
            get
            {
                return _disqus_title;
            }
            set
            {
                _disqus_title = value;
            }
        }

        public string DisqusMessage
        {
            get
            {
                return _disqus_message;
            }
            set
            {
                _disqus_message = value;
            }
        }

        public string DisqusIdentifier
        {
            get
            {
                return _disqus_identifier;
            }
            set
            {
                _disqus_identifier = value;
            }
        }

        public string xWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        private string DivID
        {
            get
            {
                return "disqus-comment-div-" + this.ClientID;
            }
        }

        private string IframeID
        {
            get
            {
                return "disqus-comment-iframe-" + this.ClientID;
            }
        }

        private string Update
        {
            get
            {
                return this.IframeID;
            }
            set
            {
            }
        }

        public void DoUpdate()
        {
            this.Update = this.IframeID;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                sm = ScriptManager.GetCurrent(Page);

                if (sm == null)
                {
                    throw new HttpException("A ScriptManager control must exist on the current page");
                }

                sm.RegisterScriptControl(this);
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                sm.RegisterScriptDescriptors(this);
            }

            base.Render(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute("id", this.DivID);
            writer.RenderBeginTag("div");

            StringBuilder sb = new StringBuilder();
            sb.Append("<iframe id='" + this.IframeID + "' name='" + this.IframeID + "' ");
            sb.Append("width='" + this.Width.ToString() + "' height='0px' ");
            sb.Append("style='border: none;' ");
            sb.Append(">");
            sb.Append("</iframe>");
            writer.Write(sb.ToString());
            writer.RenderEndTag();

            base.RenderContents(writer);
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();
            reference.Path = ResolveClientUrl("disqus.js");

            return new ScriptReference[] { reference };
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor("MoMATool.DisqusControl", this.ClientID);
            descriptor.AddProperty("disqusForum", this.DisqusForum);
            descriptor.AddProperty("disqusDeveloper", this.DisqusDeveloper);
            descriptor.AddProperty("disqusContainerId", this.DisqusContainerId);
            descriptor.AddProperty("disqusURL", this.DisqusURL);
            descriptor.AddProperty("disqusTitle", this.DisqusTitle);
            descriptor.AddProperty("disqusMessage", this.DisqusMessage);
            descriptor.AddProperty("disqusIdentifier", this.DisqusIdentifier);
            descriptor.AddProperty("update", this.Update);

            return new ScriptDescriptor[] { descriptor };
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            return GetScriptReferences();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            return GetScriptDescriptors();
        }
    }
}