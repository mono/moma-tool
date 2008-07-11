using System;
using Castle.ActiveRecord;
using NHibernate.Expression;

namespace MomaTool.Database
{
	[ActiveRecord ("issue")]
	public class Issue : ActiveRecordBase<Issue>
	{
		private int id;
		private string method_return_type;
		private string method_namespace;
		private string method_class;
		private string method_name;
		private string method_library;

		private Report report;
		private IssueType issue_type;
		
		[PrimaryKey (PrimaryKeyType.Sequence, SequenceName="issue_id_seq")]
		public int Id
		{
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		[Property ("method_return_type", Length=200)]
		public String MethodReturnType
		{
			get {
				return method_return_type;
			}
			set {
				method_return_type = value;
			}
		}
		
		[Property ("method_namespace", Length=200)]
		public String MethodNamespace
		{
			get {
				return method_namespace;
			}
			set {
				method_namespace = value;
			}
		}
		
		[Property ("method_class", Length=200)]
		public String MethodClass
		{
			get {
				return method_class;
			}
			set {
				method_class = value;
			}
		}
		
		[Property ("method_name", Length=1000)]
		public String MethodName
		{
			get {
				return method_name;
			}
			set {
				method_name = value;
			}
		}
		
		[Property ("method_library", Length=500)]
		public String MethodLibrary
		{
			get {
				return method_library;
			}
			set {
				method_library = value;
			}
		}

		[BelongsTo ("report_id", ForeignKey="fk_issue_report", Cascade=CascadeEnum.All, NotNull=true)]
		public Report Report
		{
			get {
				return report;
			}
			set {
				report = value;
			}
		}
		
		[BelongsTo ("issue_type_id", ForeignKey="fk_issue_issue_type", Cascade=CascadeEnum.All, NotNull=true)]
		public IssueType IssueType
		{
			get {
				return issue_type;
			}
			set {
				issue_type = value;
			}
		}

		/* Accessor functions */
		public static Issue FindById (int id)
		{
			return(FindOne (Expression.Eq ("Id", id)));
		}
		
		public static Issue Create (Report report, IssueType type, string return_type, string ns, string class_name, string method_name, string library)
		{
			Issue ret = new Issue ();
			
			ret.Report = report;
			ret.IssueType = type;
			ret.MethodReturnType = return_type;
			ret.MethodNamespace = ns;
			ret.MethodClass = class_name;
			ret.MethodName = method_name;
			ret.MethodLibrary = library;
			
			ret.Save ();
			
			return ret;
		}
	}
}
