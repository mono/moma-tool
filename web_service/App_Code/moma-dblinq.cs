#region Auto-generated classes for nhibernate database on 2008-09-03 16:20:12Z

//
//  ____  _     __  __      _        _
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from nhibernate on 2008-09-03 16:20:12Z
// Please visit http://linq.to/db for more information

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using DbLinq.Linq;
using DbLinq.Linq.Mapping;

namespace MomaTool.Database.Linq
{
	public partial class MoMADB : DbLinq.Linq.DataContext
	{
		public MoMADB(System.Data.IDbConnection connection)
		: base(connection, new DbLinq.PostgreSql.PgsqlVendor())
		{
		}

		public MoMADB(System.Data.IDbConnection connection, DbLinq.Vendor.IVendor vendor)
		: base(connection, vendor)
		{
		}

		public Table<Issue> Issue { get { return GetTable<Issue>(); } }
		public Table<IssueType> IssueType { get { return GetTable<IssueType>(); } }
		public Table<MomADefinition> MomADefinition { get { return GetTable<MomADefinition>(); } }
		public Table<Report> Report { get { return GetTable<Report>(); } }

	}

	[Table(Name = "public.issue")]
	public partial class Issue : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged handling

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region int ID

		[AutoGenId]
		private int id;
		[DebuggerNonUserCode]
		[Column(Storage = "id", Name = "id", DbType = "integer(32,0)", IsDbGenerated = true, CanBeNull = false, Expression = "nextval('issue_id_seq')")]
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				if (value != id)
				{
					id = value;
					OnPropertyChanged("ID");
				}
			}
		}

		#endregion

		#region int IssueTypeID

		private int issueTypeID;
		[DebuggerNonUserCode]
		[Column(Storage = "issueTypeID", Name = "issue_type_id", DbType = "integer(32,0)", CanBeNull = false, Expression = null)]
		public int IssueTypeID
		{
			get
			{
				return issueTypeID;
			}
			set
			{
				if (value != issueTypeID)
				{
					issueTypeID = value;
					OnPropertyChanged("IssueTypeID");
				}
			}
		}

		#endregion

		#region string MethodClass

		private string methodClass;
		[DebuggerNonUserCode]
		[Column(Storage = "methodClass", Name = "method_class", DbType = "character varying(200)", Expression = null)]
		public string MethodClass
		{
			get
			{
				return methodClass;
			}
			set
			{
				if (value != methodClass)
				{
					methodClass = value;
					OnPropertyChanged("MethodClass");
				}
			}
		}

		#endregion

		#region string MethodLibrary

		private string methodLibrary;
		[DebuggerNonUserCode]
		[Column(Storage = "methodLibrary", Name = "method_library", DbType = "character varying(500)", Expression = null)]
		public string MethodLibrary
		{
			get
			{
				return methodLibrary;
			}
			set
			{
				if (value != methodLibrary)
				{
					methodLibrary = value;
					OnPropertyChanged("MethodLibrary");
				}
			}
		}

		#endregion

		#region string MethodName

		private string methodName;
		[DebuggerNonUserCode]
		[Column(Storage = "methodName", Name = "method_name", DbType = "character varying(1000)", Expression = null)]
		public string MethodName
		{
			get
			{
				return methodName;
			}
			set
			{
				if (value != methodName)
				{
					methodName = value;
					OnPropertyChanged("MethodName");
				}
			}
		}

		#endregion

		#region string MethodNamesPace

		private string methodNamesPace;
		[DebuggerNonUserCode]
		[Column(Storage = "methodNamesPace", Name = "method_namespace", DbType = "character varying(200)", Expression = null)]
		public string MethodNamesPace
		{
			get
			{
				return methodNamesPace;
			}
			set
			{
				if (value != methodNamesPace)
				{
					methodNamesPace = value;
					OnPropertyChanged("MethodNamesPace");
				}
			}
		}

		#endregion

		#region string MethodReturnType

		private string methodReturnType;
		[DebuggerNonUserCode]
		[Column(Storage = "methodReturnType", Name = "method_return_type", DbType = "character varying(200)", Expression = null)]
		public string MethodReturnType
		{
			get
			{
				return methodReturnType;
			}
			set
			{
				if (value != methodReturnType)
				{
					methodReturnType = value;
					OnPropertyChanged("MethodReturnType");
				}
			}
		}

		#endregion

		#region int ReportID

		private int reportID;
		[DebuggerNonUserCode]
		[Column(Storage = "reportID", Name = "report_id", DbType = "integer(32,0)", CanBeNull = false, Expression = null)]
		public int ReportID
		{
			get
			{
				return reportID;
			}
			set
			{
				if (value != reportID)
				{
					reportID = value;
					OnPropertyChanged("ReportID");
				}
			}
		}

		#endregion

		#region Parents

		private System.Data.Linq.EntityRef<IssueType> issueType;
		[Association(Storage = "issueType", ThisKey = "IssueTypeID", Name = "fk_issue_issue_type")]
		[DebuggerNonUserCode]
		public IssueType IssueType
		{
			get
			{
				return issueType.Entity;
			}
			set
			{
				issueType.Entity = value;
			}
		}

		private System.Data.Linq.EntityRef<Report> report;
		[Association(Storage = "report", ThisKey = "ReportID", Name = "fk_issue_report")]
		[DebuggerNonUserCode]
		public Report Report
		{
			get
			{
				return report.Entity;
			}
			set
			{
				report.Entity = value;
			}
		}


		#endregion

	}

	[Table(Name = "public.issue_type")]
	public partial class IssueType : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged handling

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region string Description

		private string description;
		[DebuggerNonUserCode]
		[Column(Storage = "description", Name = "description", DbType = "text", CanBeNull = false, Expression = null)]
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				if (value != description)
				{
					description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		#endregion

		#region string DisplayName

		private string displayName;
		[DebuggerNonUserCode]
		[Column(Storage = "displayName", Name = "display_name", DbType = "character varying(100)", CanBeNull = false, Expression = null)]
		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				if (value != displayName)
				{
					displayName = value;
					OnPropertyChanged("DisplayName");
				}
			}
		}

		#endregion

		#region int ID

		[AutoGenId]
		private int id;
		[DebuggerNonUserCode]
		[Column(Storage = "id", Name = "id", DbType = "integer(32,0)", IsDbGenerated = true, CanBeNull = false, Expression = "nextval('issue_type_id_seq')")]
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				if (value != id)
				{
					id = value;
					OnPropertyChanged("ID");
				}
			}
		}

		#endregion

		#region bool IsActive

		private bool isActive;
		[DebuggerNonUserCode]
		[Column(Storage = "isActive", Name = "is_active", DbType = "boolean", CanBeNull = false, Expression = null)]
		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				if (value != isActive)
				{
					isActive = value;
					OnPropertyChanged("IsActive");
				}
			}
		}

		#endregion

		#region string LookupName

		private string lookupName;
		[DebuggerNonUserCode]
		[Column(Storage = "lookupName", Name = "lookup_name", DbType = "character varying(10)", CanBeNull = false, Expression = null)]
		public string LookupName
		{
			get
			{
				return lookupName;
			}
			set
			{
				if (value != lookupName)
				{
					lookupName = value;
					OnPropertyChanged("LookupName");
				}
			}
		}

		#endregion

		#region Children

		[Association(Storage = null, OtherKey = "IssueTypeID", Name = "fk_issue_issue_type")]
		[DebuggerNonUserCode]
		public EntityMSet<Issue> Issue
		{
			get
			{
				// L212 - child data available only when part of query
				return null;
			}
		}


		#endregion

	}

	[Table(Name = "public.moma_definition")]
	public partial class MomADefinition : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged handling

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region System.DateTime CreateDate

		private System.DateTime createDate;
		[DebuggerNonUserCode]
		[Column(Storage = "createDate", Name = "create_date", DbType = "timestamp without time zone", CanBeNull = false, Expression = null)]
		public System.DateTime CreateDate
		{
			get
			{
				return createDate;
			}
			set
			{
				if (value != createDate)
				{
					createDate = value;
					OnPropertyChanged("CreateDate");
				}
			}
		}

		#endregion

		#region string Description

		private string description;
		[DebuggerNonUserCode]
		[Column(Storage = "description", Name = "description", DbType = "text", CanBeNull = false, Expression = null)]
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				if (value != description)
				{
					description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		#endregion

		#region string DisplayName

		private string displayName;
		[DebuggerNonUserCode]
		[Column(Storage = "displayName", Name = "display_name", DbType = "character varying(100)", CanBeNull = false, Expression = null)]
		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				if (value != displayName)
				{
					displayName = value;
					OnPropertyChanged("DisplayName");
				}
			}
		}

		#endregion

		#region int ID

		[AutoGenId]
		private int id;
		[DebuggerNonUserCode]
		[Column(Storage = "id", Name = "id", DbType = "integer(32,0)", IsDbGenerated = true, CanBeNull = false, Expression = "nextval('moma_definition_id_seq')")]
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				if (value != id)
				{
					id = value;
					OnPropertyChanged("ID");
				}
			}
		}

		#endregion

		#region bool IsActive

		private bool isActive;
		[DebuggerNonUserCode]
		[Column(Storage = "isActive", Name = "is_active", DbType = "boolean", CanBeNull = false, Expression = null)]
		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				if (value != isActive)
				{
					isActive = value;
					OnPropertyChanged("IsActive");
				}
			}
		}

		#endregion

		#region string LookupName

		private string lookupName;
		[DebuggerNonUserCode]
		[Column(Storage = "lookupName", Name = "lookup_name", DbType = "character varying(100)", CanBeNull = false, Expression = null)]
		public string LookupName
		{
			get
			{
				return lookupName;
			}
			set
			{
				if (value != lookupName)
				{
					lookupName = value;
					OnPropertyChanged("LookupName");
				}
			}
		}

		#endregion

		#region Children

		[Association(Storage = null, OtherKey = "MomADefinitionID", Name = "fk_report_moma_definition")]
		[DebuggerNonUserCode]
		public EntityMSet<Report> Report
		{
			get
			{
				// L212 - child data available only when part of query
				return null;
			}
		}


		#endregion

	}

	[Table(Name = "public.report")]
	public partial class Report : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged handling

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region System.DateTime CreateDate

		private System.DateTime createDate;
		[DebuggerNonUserCode]
		[Column(Storage = "createDate", Name = "create_date", DbType = "timestamp without time zone", CanBeNull = false, Expression = null)]
		public System.DateTime CreateDate
		{
			get
			{
				return createDate;
			}
			set
			{
				if (value != createDate)
				{
					createDate = value;
					OnPropertyChanged("CreateDate");
				}
			}
		}

		#endregion

		#region int ID

		[AutoGenId]
		private int id;
		[DebuggerNonUserCode]
		[Column(Storage = "id", Name = "id", DbType = "integer(32,0)", IsDbGenerated = true, CanBeNull = false, Expression = "nextval('report_id_seq')")]
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				if (value != id)
				{
					id = value;
					OnPropertyChanged("ID");
				}
			}
		}

		#endregion

		#region bool IsActive

		private bool isActive;
		[DebuggerNonUserCode]
		[Column(Storage = "isActive", Name = "is_active", DbType = "boolean", CanBeNull = false, Expression = null)]
		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				if (value != isActive)
				{
					isActive = value;
					OnPropertyChanged("IsActive");
				}
			}
		}

		#endregion

		#region System.DateTime LastUpdateDate

		private System.DateTime lastUpdateDate;
		[DebuggerNonUserCode]
		[Column(Storage = "lastUpdateDate", Name = "last_update_date", DbType = "timestamp without time zone", CanBeNull = false, Expression = null)]
		public System.DateTime LastUpdateDate
		{
			get
			{
				return lastUpdateDate;
			}
			set
			{
				if (value != lastUpdateDate)
				{
					lastUpdateDate = value;
					OnPropertyChanged("LastUpdateDate");
				}
			}
		}

		#endregion

		#region int? MomADefinitionID

		private int? momAdEfinitionID;
		[DebuggerNonUserCode]
		[Column(Storage = "momAdEfinitionID", Name = "moma_definition_id", DbType = "integer(32,0)", Expression = null)]
		public int? MomADefinitionID
		{
			get
			{
				return momAdEfinitionID;
			}
			set
			{
				if (value != momAdEfinitionID)
				{
					momAdEfinitionID = value;
					OnPropertyChanged("MomADefinitionID");
				}
			}
		}

		#endregion

		#region System.DateTime ReportDate

		private System.DateTime reportDate;
		[DebuggerNonUserCode]
		[Column(Storage = "reportDate", Name = "report_date", DbType = "timestamp without time zone", CanBeNull = false, Expression = null)]
		public System.DateTime ReportDate
		{
			get
			{
				return reportDate;
			}
			set
			{
				if (value != reportDate)
				{
					reportDate = value;
					OnPropertyChanged("ReportDate");
				}
			}
		}

		#endregion

		#region string ReporterComments

		private string reporterComments;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterComments", Name = "reporter_comments", DbType = "text", Expression = null)]
		public string ReporterComments
		{
			get
			{
				return reporterComments;
			}
			set
			{
				if (value != reporterComments)
				{
					reporterComments = value;
					OnPropertyChanged("ReporterComments");
				}
			}
		}

		#endregion

		#region string ReporterEmail

		private string reporterEmail;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterEmail", Name = "reporter_email", DbType = "character varying(500)", Expression = null)]
		public string ReporterEmail
		{
			get
			{
				return reporterEmail;
			}
			set
			{
				if (value != reporterEmail)
				{
					reporterEmail = value;
					OnPropertyChanged("ReporterEmail");
				}
			}
		}

		#endregion

		#region string ReporterHomepage

		private string reporterHomepage;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterHomepage", Name = "reporter_homepage", DbType = "character varying(500)", Expression = null)]
		public string ReporterHomepage
		{
			get
			{
				return reporterHomepage;
			}
			set
			{
				if (value != reporterHomepage)
				{
					reporterHomepage = value;
					OnPropertyChanged("ReporterHomepage");
				}
			}
		}

		#endregion

		#region string ReporterIP

		private string reporterIp;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterIp", Name = "reporter_ip", DbType = "character varying(50)", Expression = null)]
		public string ReporterIP
		{
			get
			{
				return reporterIp;
			}
			set
			{
				if (value != reporterIp)
				{
					reporterIp = value;
					OnPropertyChanged("ReporterIP");
				}
			}
		}

		#endregion

		#region string ReporterName

		private string reporterName;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterName", Name = "reporter_name", DbType = "character varying(500)", Expression = null)]
		public string ReporterName
		{
			get
			{
				return reporterName;
			}
			set
			{
				if (value != reporterName)
				{
					reporterName = value;
					OnPropertyChanged("ReporterName");
				}
			}
		}

		#endregion

		#region string ReporterOrganization

		private string reporterOrganization;
		[DebuggerNonUserCode]
		[Column(Storage = "reporterOrganization", Name = "reporter_organization", DbType = "character varying(500)", Expression = null)]
		public string ReporterOrganization
		{
			get
			{
				return reporterOrganization;
			}
			set
			{
				if (value != reporterOrganization)
				{
					reporterOrganization = value;
					OnPropertyChanged("ReporterOrganization");
				}
			}
		}

		#endregion

		#region string ReportFilename

		private string reportFilename;
		[DebuggerNonUserCode]
		[Column(Storage = "reportFilename", Name = "report_filename", DbType = "character varying(50)", CanBeNull = false, Expression = null)]
		public string ReportFilename
		{
			get
			{
				return reportFilename;
			}
			set
			{
				if (value != reportFilename)
				{
					reportFilename = value;
					OnPropertyChanged("ReportFilename");
				}
			}
		}

		#endregion

		#region Children

		[Association(Storage = null, OtherKey = "ReportID", Name = "fk_issue_report")]
		[DebuggerNonUserCode]
		public EntityMSet<Issue> Issue
		{
			get
			{
				// L212 - child data available only when part of query
				return null;
			}
		}


		#endregion

		#region Parents

		private System.Data.Linq.EntityRef<MomADefinition> momAdEfinition;
		[Association(Storage = "momAdEfinition", ThisKey = "MomADefinitionID", Name = "fk_report_moma_definition")]
		[DebuggerNonUserCode]
		public MomADefinition MomADefinition
		{
			get
			{
				return momAdEfinition.Entity;
			}
			set
			{
				momAdEfinition.Entity = value;
			}
		}


		#endregion

	}
}
