using System;
using Castle.ActiveRecord;
using NHibernate.Expression;

namespace MomaTool.Database
{
	[ActiveRecord ("moma_definition")]
	public class MomaDefinition : ActiveRecordBase<MomaDefinition>
	{
		private int id;
		private string lookup_name;
		private string display_name;
		private string description;
		private DateTime create_date;
		private bool is_active;
		
		[PrimaryKey (PrimaryKeyType.Sequence, SequenceName="moma_definition_id_seq")]
		public int Id
		{
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		
		[Property ("lookup_name", NotNull=true, Length=100)]
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
		
		[Property ("description", NotNull=true, SqlType="Text")]
		public String Description
		{
			get {
				return description;
			}
			set {
				description = value;
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

		/* Constructors */

		/* Need a 0-arg constructor for NHibernate */
		public MomaDefinition () 
		{
			this.CreateDate = DateTime.UtcNow;
			this.IsActive = true;
		}
		
		public MomaDefinition (string name) : this()
		{
			this.LookupName = name;
			this.DisplayName = name;
			this.Description = name;
		}

		/* Accessor functions */
		public static MomaDefinition FindById (int id)
		{
			return(FindOne (Expression.Eq ("Id", id)));
		}

		public static MomaDefinition FindByLookupName (string name)
		{
			return(FindOne (Expression.Eq ("LookupName", name)));
		}

		public static MomaDefinition GetOrCreate (string name)
		{
			MomaDefinition ret = FindByLookupName (name);
			if (ret == null) {
				ret = new MomaDefinition (name);
				ret.Save ();
			}
			
			return ret;
		}
	}
}
