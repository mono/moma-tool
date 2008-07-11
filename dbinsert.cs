using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using MomaTool.Database;

namespace MomaTool
{
	public class DBInsert
	{
		static IssueType miss;
		static IssueType niex;
		static IssueType pinv;
		static IssueType todo;
		
		public static void Main (string[] args)
		{
			XmlConfigurationSource source = new XmlConfigurationSource ("test_castle_config.xml");
			Assembly ass = Assembly.Load ("MomaTool.Database");
			
			ActiveRecordStarter.Initialize (ass, source);
			//ActiveRecordStarter.CreateSchema ();

			miss = IssueType.FindByLookupName ("MISS");
			if(miss == null) {
				Console.WriteLine ("Can't find issue type: MISS");
				Environment.Exit (1);
			}
				
			niex = IssueType.FindByLookupName ("NIEX");
			if(niex == null) {
				Console.WriteLine ("Can't find issue type: NIEX");
				Environment.Exit (1);
			}

			pinv = IssueType.FindByLookupName ("PINV");
			if(pinv == null) {
				Console.WriteLine ("Can't find issue type: PINV");
				Environment.Exit (1);
			}

			todo = IssueType.FindByLookupName ("TODO");
			if(todo == null) {
				Console.WriteLine ("Can't find issue type: TODO");
				Environment.Exit (1);
			}

			foreach (string f in args) {
				/*Report dbrep = */ParseReport (f);
			}
		}

		static Report ParseReport (string file)
		{
			Report dbrep;

			using (FileStream fs = File.OpenRead (file)) {
				MomaDefinition dbdef;
				Dictionary<string,string> meta = new Dictionary<string,string>();

				StreamReader stream = new StreamReader (fs);
				CultureInfo cult = CultureInfo.CreateSpecificCulture ("en-US");
				
				string date = stream.ReadLine ();
				DateTime date_parsed = DateTime.Parse (date, cult);
				string ip = stream.ReadLine ();
				string r = ReadMeta (stream, meta);

				try {
					dbdef = MomaDefinition.GetOrCreate (meta["@Definitions"]);
				} catch {
					Console.WriteLine ("Exception when creating MomaDefinition: [{0}]", meta["@Definitions"]);

					throw;
				}
				
				try {
					dbrep = Report.CreateOrUpdate (Path.GetFileName (file), dbdef, date_parsed, ip, meta["@Name"], meta["@Email"], meta["@Organization"], meta["@HomePage"], meta["@Comments"]);
				} catch {
					Console.WriteLine ("Exception when creating Report: Date {0}, IP {1}, Name [{2}], Email [{3}], Organisation [{4}], Home page [{5}], Comments [{6}]", date_parsed.ToString (), ip, meta["@Name"], meta["@Email"], meta["@Organization"], meta["@HomePage"], meta["@Comments"]);

					throw;
				}

				
				for(; r != null && r.Length > 6; r = stream.ReadLine ()) {
					IssueType type;
					
					if (r[r.Length - 1] == '\r') {
						r = r.Substring (0, r.Length - 1);
					}
					
					switch(r.Substring (0, 6)) {
					case "[TODO]":
						type = todo;
						break;
					case "[NIEX]":
						type = niex;
						break;
					case "[MISS]":
						type = miss;
						break;
					case "[PINV]":
						type = pinv;
						break;
					default:
						continue;
					}
					
					string rest = r.Substring (7);
					
					string ns = "";
					string class_name = "";
					string method_name = "";
					string lib = "";
					string return_type = "";
					
					try {
						string[] method_parts = rest.Split (' ');
						return_type = method_parts[0];
						string details = method_parts[1];
					
						int colons = details.IndexOf ("::");
						if (colons != -1) {
							/* "::" was found */
							method_name = details.Substring (colons+2);
							string[] ns_parts = details.Substring (0, colons).Split ('.');
							class_name = ns_parts[ns_parts.Length - 1];
							
							for(int i = 0; i < ns_parts.Length - 1; i++) {
								if (ns.Length > 0) {
									ns += ".";
								}
								ns += ns_parts[i];
							}
						} else {
							/* "::" not found */
							method_parts = rest.Split ('-');
							method_name = method_parts[0].Split (new char[]{' '}, 2)[1];
							lib = method_parts[1];
						}
					} catch {
						Console.WriteLine ("Error parsing [{0}] in file {1}", r, file);
						/* Carry on parsing,
						 * no need to error
						 * out here
						 */
						continue;
					}

					try {
						Issue.Create (dbrep, type, return_type, ns, class_name, method_name, lib);
					} catch {
						Console.WriteLine ("Exception when creating Issue in report {0}: {1} Return type [{2}], Namespace [{3}], Class [{4}], Method [{5}], Library [{6}]", file, type.DisplayName, return_type, ns, class_name, method_name, lib);

						throw;
					}
				}
			}

			return(dbrep);
		}

		static string ReadMeta (StreamReader stream, Dictionary<string,string> meta)
		{
			string s;
			
			while ((s = stream.ReadLine ()) != null) {
				if (s.Length == 0 || s[0] == '[') {
					return s;
				}
				
				if (s[0] != '@') {
					return s;
				}
				
				int p = s.IndexOf (':');
				string v = s.Substring (p + 2);
				meta[s.Substring (0, p)] = v;
			}
			
			return s;
		}
	}
}
