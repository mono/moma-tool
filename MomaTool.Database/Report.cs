using System;
using System.Collections;
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Expression;

namespace MomaTool.Database
{
	[ActiveRecord ("report")]
	public class Report : ActiveRecordBase<Report>
	{
		private int id;
		private string report_filename;
		private DateTime report_date;
		private string reporter_ip;
		private string reporter_name;
		private string reporter_email;
		private string reporter_organisation;
		private string reporter_homepage;
		private string reporter_comments;
		private DateTime create_date;
		private DateTime last_update_date;
		private bool is_active;
		
		private MomaDefinition moma_definition;
		
		[PrimaryKey (PrimaryKeyType.Sequence, SequenceName="report_id_seq")]
		public int Id
		{
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		[Property ("report_filename", NotNull=true, Length=50)]
		public String ReportFilename
		{
			get {
				return report_filename;
			}
			set {
				report_filename = value;
			}
		}
		
		[Property ("report_date", NotNull=true)]
		public DateTime ReportDate
		{
			get {
				return report_date;
			}
			set {
				report_date = value;
			}
		}
		
		[Property ("reporter_ip", Length=50)]
		public String ReporterIp
		{
			get {
				return reporter_ip;
			}
			set {
				reporter_ip = value;
			}
		}
		
		[Property ("reporter_name", Length=500)]
		public String ReporterName
		{
			get {
				return reporter_name;
			}
			set {
				reporter_name = value;
			}
		}
		
		[Property ("reporter_email", Length=500)]
		public String ReporterEmail
		{
			get {
				return reporter_email;
			}
			set {
				reporter_email = value;
			}
		}
		
		[Property ("reporter_organization", Length=500)]
		public String ReporterOrganisation
		{
			get {
				return reporter_organisation;
			}
			set {
				reporter_organisation = value;
			}
		}
		
		[Property ("reporter_homepage", Length=500)]
		public String ReporterHomepage
		{
			get {
				return reporter_homepage;
			}
			set {
				reporter_homepage = value;
			}
		}
		
		[Property ("reporter_comments", SqlType="Text")]
		public String ReporterComments
		{
			get {
				return reporter_comments;
			}
			set {
				reporter_comments = value;
			}
		}
		
		[Property ("create_date", NotNull=true)]
		public DateTime CreateDate
		{
			get {
				return create_date;
			}
			set {
				create_date = value;
			}
		}
		
		[Property ("last_update_date", NotNull=true)]
		public DateTime LastUpdateDate
		{
			get {
				return last_update_date;
			}
			set {
				last_update_date = value;
			}
		}
		
		[Property ("is_active", NotNull=true)]
		public bool IsActive
		{
			get {
				return is_active;
			}
			set {
				is_active = value;
			}
		}

		[BelongsTo ("moma_definition_id", ForeignKey="fk_report_moma_definition", Cascade=CascadeEnum.All)]
		public MomaDefinition MomaDefinition
		{
			get {
				return moma_definition;
			}
			set {
				moma_definition = value;
			}
		}

		/* Constructors */

		/* Need a 0-arg constructor for NHibernate */
		public Report ()
		{
			this.CreateDate = DateTime.UtcNow;
			this.IsActive = true;
		}

		/* Accessor functions */
		public static Report FindById (int id)
		{
			return(FindOne (Expression.Eq ("Id", id)));
		}
		
		public static Report FindByReportFilename (string file)
		{
			return(FindOne (Expression.Eq ("ReportFilename", file)));
		}

        public static Report[] FindMostRecent(int count)
        {
            return(Report[])Execute(
                delegate(ISession session, object instance) {
                    IQuery query = session.CreateQuery("from Report r order by create_date desc");

                    query.SetMaxResults(count);

                    IList results = query.List();
 
                    Report[] reports = new Report[results.Count];
                    results.CopyTo(reports, 0);
 
                    return reports;
                }, null);
        }
		
		public static Report CreateOrUpdate (string file,
						     MomaDefinition def,
						     DateTime date, string ip,
						     string name, string email,
						     string org,
						     string homepage,
						     string comments)
		{
			Report ret = FindByReportFilename (file);
			if (ret == null) {
				ret = new Report ();
			}
			
			ret.MomaDefinition = def;
			ret.ReportFilename = file;
			ret.ReportDate = date;
			ret.ReporterIp = ip;
			ret.ReporterName = name;
			ret.ReporterEmail = email;
			ret.ReporterOrganisation = org;
			ret.ReporterHomepage = homepage;
			ret.ReporterComments = comments;
			ret.LastUpdateDate = DateTime.UtcNow;
			
			ret.Save ();

			return ret;
		}
	}
}
