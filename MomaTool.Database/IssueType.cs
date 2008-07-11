using System;
using Castle.ActiveRecord;
using NHibernate.Expression;

namespace MomaTool.Database
{
	[ActiveRecord ("issue_type")]
	public class IssueType : ActiveRecordBase<IssueType>
	{
		private int id;
		private string lookup_name;
		private string display_name;
		private string description;
		private bool is_active;
		
		[PrimaryKey (PrimaryKeyType.Sequence, SequenceName="issue_type_id_seq")]
		public int Id
		{
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		[Property ("lookup_name", NotNull=true, Length=10)]
		public String LookupName
		{
			get {
				return lookup_name;
			}
			set {
				lookup_name = value;
			}
		}
		
		[Property ("display_name", NotNull=true, Length=100)]
		public String DisplayName
		{
			get {
				return display_name;
			}
			set {
				display_name = value;
			}
		}
		
		[Property ("description", SqlType="Text", NotNull=true)]
		public String Description
		{
			get {
				return description;
			}
			set {
				description = value;
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

		/* Accessor functions */
		public static IssueType FindById (int id)
		{
			return(FindOne (Expression.Eq ("Id",id)));
		}
		
		public static IssueType FindByLookupName (string name)
		{
			return(FindOne (Expression.Eq ("LookupName", name)));
		}
	}
}
