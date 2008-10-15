using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Drawing;
using ZedGraph;
using ZedGraph.Web;

public partial class Overview : System.Web.UI.Page
{
    public class OverviewData
    {
        public OverviewData(int id, DateTime create_date, string reporter_name, string profile,
            int miss, int niex, int pinv, int todo, int total)
        {
            this._ID = id;
            this._CreateDate = create_date;
            this._ReporterName = reporter_name;
            this._Profile = profile;
            this._Miss = miss;
            this._Niex = niex;
            this._Pinv = pinv;
            this._Todo = todo;
            this._Total = total;
        }

        /* For some barking reason, DataSource requires this to be a property not a field,
         * even though the exception states "A field or property with the name 'foo' was
         * not found on the selected data source.
         */
        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        private DateTime _CreateDate;
        public DateTime CreateDate
        {
            get
            {
                return _CreateDate;
            }
            set
            {
                _CreateDate = value;
            }
        }

        private string _ReporterName;
        public string ReporterName
        {
            get
            {
                return _ReporterName;
            }
            set
            {
                _ReporterName = value;
            }
        }

        private string _Profile;
        public string Profile
        {
            get
            {
                return _Profile;
            }
            set
            {
                _Profile = value;
            }
        }

        private int _Miss;
        public int Miss
        {
            get
            {
                return _Miss;
            }
            set
            {
                _Miss = value;
            }
        }

        private int _Niex;
        public int Niex
        {
            get
            {
                return _Niex;
            }
            set
            {
                _Niex = value;
            }
        }

        private int _Pinv;
        public int Pinv
        {
            get
            {
                return _Pinv;
            }
            set
            {
                _Pinv = value;
            }
        }

        private int _Todo;
        public int Todo
        {
            get
            {
                return _Todo;
            }
            set
            {
                _Todo = value;
            }
        }

        private int _Total;
        public int Total
        {
            get
            {
                return _Total;
            }
            set
            {
                _Total = value;
            }
        }
    }

    public class OverviewMostNeeded
    {
        public OverviewMostNeeded()
        {
            this._Namespace = "unknown namespace";
            this._Class = "unknown class";
            this._Method = "unknown method";
            this._Type = "unknown type";
            this._Apps = 0;
        }

        public OverviewMostNeeded(string ns, string cls, string meth, string type, int apps)
        {
            this._Namespace = ns;
            this._Class = cls;
            this._Method = meth;
            this._Type = type;
            this._Apps = apps;
        }

        private string _Namespace;
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
            set
            {
                _Namespace = value;
            }
        }

        private string _Class;
        public string Class
        {
            get
            {
                return _Class;
            }
            set
            {
                _Class = value;
            }
        }

        private string _Method;
        public string Method
        {
            get
            {
                return _Method;
            }
            set
            {
                _Method = value;
            }
        }

        private string _Type;
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }

        private int _Apps;
        public int Apps
        {
            get
            {
                return _Apps;
            }
            set
            {
                _Apps = value;
            }
        }
    }

    public class OverviewIssueCount
    {
        public OverviewIssueCount()
        {
            this._Count = 0;
        }

        public OverviewIssueCount(int count)
        {
            this._Count = count;
        }

        private int _Count;
        public int Count
        {
            get
            {
                return _Count;
            }
            set
            {
                _Count = value;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // GridView1 only available to logged-in users
        if (Page.User.Identity.IsAuthenticated)
        {
            // ... and we need to find it from inside the LoginView
            GridView grid1 = (GridView)LoginView1.FindControl("Latest20GridView");

            string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            MoMADB db = new MoMADB(conn);
            //NpgsqlEventLog.Level = LogLevel.Debug;
            //NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-logx";

            int miss_id = (from type in db.IssueType where type.LookupName == "MISS" select type.ID).ToList()[0];
            int niex_id = (from type in db.IssueType where type.LookupName == "NIEX" select type.ID).ToList()[0];
            int pinv_id = (from type in db.IssueType where type.LookupName == "PINV" select type.ID).ToList()[0];
            int todo_id = (from type in db.IssueType where type.LookupName == "TODO" select type.ID).ToList()[0];

            List<OverviewData> grid1_q = (from rep in db.Report
                                          from prof in db.MomADefinition
                                          where rep.MomADefinitionID == prof.ID
                                          orderby rep.CreateDate descending
                                          select new OverviewData(rep.ID, rep.CreateDate,
                                              rep.ReporterName, prof.DisplayName,
                                              0, 0, 0, 0, 0)
                                         ).Take(20).ToList();

            /* This ought to be incorporated into the select above, but I can't seem to make
             * that work with dblinq (not sure whether the problem is dblinq itself, npgsql, or just
             * me.)
             */
            foreach (OverviewData ov in grid1_q)
            {
                /* This works with MS LINQ, but not with DBLINQ - the ToArray() blows up */
#if NAUGHT
                var q = (from iss in db.Issue where iss.ReportID == ov.ID group iss by iss.IssueTypeID into g orderby g.Key select new { issue_type_id = g.Key, Count = g.Count() }).ToArray();

                ov.Miss = q[miss_id].Count;
                ov.Niex = q[niex_id].Count;
                ov.Pinv = q[pinv_id].Count;
                ov.Todo = q[todo_id].Count;
#endif

                ov.Miss = (from issue in db.Issue where issue.ReportID == ov.ID && issue.IssueTypeID == miss_id select issue.ID).ToList().Count;
                ov.Niex = (from issue in db.Issue where issue.ReportID == ov.ID && issue.IssueTypeID == niex_id select issue.ID).ToList().Count;
                ov.Pinv = (from issue in db.Issue where issue.ReportID == ov.ID && issue.IssueTypeID == pinv_id select issue.ID).ToList().Count;
                ov.Todo = (from issue in db.Issue where issue.ReportID == ov.ID && issue.IssueTypeID == todo_id select issue.ID).ToList().Count;
                ov.Total = ov.Miss + ov.Niex + ov.Pinv + ov.Todo;
            }

            grid1.DataSource = grid1_q;
            grid1.DataBind();

            GridView grid2 = (GridView)LoginView1.FindControl("MostNeededGridView");

            /* FIXME: figure out the LINQ version of the count(distinct)... */
            var grid2_q = db.ExecuteQuery<OverviewMostNeeded>(@"select count(distinct(issue.report_id)) as Apps, issue.method_namespace as Namespace, issue.method_class as Class, issue.method_name as Method, issue_type.display_name as Type from issue, issue_type where issue_type.id = issue.issue_type_id group by method_namespace, method_class, method_name, display_name order by Apps desc limit 20;");

            grid2.DataSource = grid2_q;
            grid2.DataBind();
        }
    }

    protected override void OnInit(EventArgs e)
    {
        InitializeComponent();
        base.OnInit(e);
    }

    private void InitializeComponent()
    {
        if (Page.User.Identity.IsAuthenticated)
        {
            ZedGraphWeb zg1 = (ZedGraphWeb)LoginView1.FindControl("IssuesPerAppGraph");
            zg1.RenderGraph += new ZedGraph.Web.ZedGraphWebControlEventHandler(this.OnRenderGraph1);
        }
    }

    private void OnRenderGraph1(ZedGraphWeb zgw, Graphics g, MasterPane masterPane)
    {
        string connstr = ConfigurationManager.ConnectionStrings["MomaDB"].ConnectionString;
        NpgsqlConnection conn = new NpgsqlConnection(connstr);
        MoMADB db = new MoMADB(conn);
        var issue_count_q = db.ExecuteQuery<OverviewIssueCount>(@"select count(issue.report_id) as Count from issue group by issue.report_id;").ToList();

        GraphPane myPane = masterPane[0];
        PieItem segment1 = myPane.AddPieSlice(0, Color.FromArgb(0xf5, 0xeb, 0xcb), Color.White, 45f, 0, "1-5");
        PieItem segment2 = myPane.AddPieSlice(0, Color.FromArgb(0xec, 0xc5, 0x6b), Color.White, 45f, 0, "6-25");
        PieItem segment3 = myPane.AddPieSlice(0, Color.FromArgb(0xe2, 0x9f, 0x27), Color.White, 45f, 0, "26+");

        segment1.LabelDetail.FontSpec.Size = 20f;
        segment2.LabelDetail.FontSpec.Size = 20f;
        segment3.LabelDetail.FontSpec.Size = 20f;

        foreach (OverviewIssueCount count in issue_count_q)
        {
            if (count.Count < 6)
            {
                segment1.Value++;
            }
            else if (count.Count < 26)
            {
                segment2.Value++;
            }
            else
            {
                segment3.Value++;
            }
        }

        masterPane.AxisChange(g);
    }
}
