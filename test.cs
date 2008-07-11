using System;
using System.Reflection;
using MomaTool.Database;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;

namespace MomaTool
{
	public class Tester
	{
		public static void Main ()
		{
			XmlConfigurationSource source = new XmlConfigurationSource ("test_castle_config.xml");
			Assembly ass = Assembly.Load ("MomaTool.Database");
			
			ActiveRecordStarter.Initialize (ass, source);
			ActiveRecordStarter.CreateSchema ();
		}
	}
}
