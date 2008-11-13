using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Data;
using Npgsql;
using System.Globalization;
using MoMA.Analyzer;
using ICSharpCode.SharpZipLib.Zip;

namespace MoMAIssueAdd
{
	class Program
	{
		static int miss_id;
		static int niex_id;
		static int todo_id;

		static int GetIssueType(NpgsqlConnection conn, string lookup_name)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("SELECT id, lookup_name FROM issue_type WHERE lookup_name = :ln;", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("ln", DbType.String));
				command.Parameters[0].Value = lookup_name;

				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with id and lookup_name columns returned
						if (dr.FieldCount != 2)
						{
							throw new Exception("Got too many columns");
						}

						if (!dr.GetString(1).Equals(lookup_name))
						{
							throw new Exception(String.Format ("Got wrong name: {0}", dr[1]));
						}

						return (dr.GetInt32(0));
					}

					throw new Exception(String.Format("Couldn't find {0}", lookup_name));
				}
			}
		}

		/* Irritatingly, the ID is 32 bits when selecting it, but 64
		 * when inserting.
		 */
		static long GetDefinitionID(NpgsqlConnection conn, string lookup_name, DateTime date)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("SELECT id, lookup_name FROM moma_definition WHERE lookup_name = :ln;", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("ln", DbType.String));
				command.Parameters[0].Value = lookup_name;

				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with id and lookup_name columns returned
						if (dr.FieldCount != 2)
						{
							throw new Exception("Got too many columns");
						}

						if (!dr.GetString(1).Equals(lookup_name))
						{
							throw new Exception(String.Format ("Got wrong name: {0}", dr[1]));
						}

						/* Make sure the date is correct */
						Console.WriteLine ("Setting {0} date to {1}", lookup_name, date.ToString ());
						using (NpgsqlCommand date_command = new NpgsqlCommand("UPDATE moma_definition SET create_date = :create_date WHERE id = :def_id;", conn)) {
							date_command.Parameters.Add(new NpgsqlParameter("create_date", DbType.DateTime));
							date_command.Parameters[0].Value = date;
							date_command.Parameters.Add(new NpgsqlParameter("def_id", DbType.Int32));
							date_command.Parameters[1].Value = dr.GetInt32(0);
							date_command.ExecuteNonQuery ();
						}
						return (dr.GetInt32(0));
					}
				}
			}

			/* Not found, so insert it */
			using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO moma_definition (id, lookup_name, display_name, description, create_date) VALUES (nextval ('moma_definition_id_seq'), :lookup_name, :display_name, :description, :create_date); SELECT currval('moma_definition_id_seq');", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("lookup_name", DbType.String));
				command.Parameters[0].Value = lookup_name;
				command.Parameters.Add(new NpgsqlParameter("display_name", DbType.String));
				command.Parameters[1].Value = lookup_name;
				command.Parameters.Add(new NpgsqlParameter("description", DbType.String));
				command.Parameters[2].Value = lookup_name;
				command.Parameters.Add(new NpgsqlParameter("create_date", DbType.DateTime));
				command.Parameters[3].Value = date;

				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with the newly inserted id returned
						if (dr.FieldCount != 1)
						{
							throw new Exception("Got too many columns");
						}

						return (dr.GetInt64(0));
					}

					throw new Exception("Error inserting definition");
				}
			}
		}

		static int GetLatestDefinitionID(NpgsqlConnection conn)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("SELECT id, lookup_name FROM moma_definition ORDER BY create_date DESC LIMIT 1;", conn))
			{
				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with id and lookup_name columns returned
						if (dr.FieldCount != 2)
						{
							throw new Exception("Got too many columns");
						}

						Console.WriteLine ("Latest definitions: {0}", dr.GetString(1));

						return (dr.GetInt32(0));
					}
				}
			}
			throw new Exception("Error finding latest definition");
		}

		static int GetIssueID(NpgsqlConnection conn, int issue_type_id, string method_return_type, string method_namespace, string method_class, string method_name, string method_library)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("SELECT id FROM issue WHERE issue_type_id = :issue_type_id AND method_return_type = :method_return_type AND method_namespace = :method_namespace AND method_class = :method_class AND method_name = :method_name AND method_library = :method_library;", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("issue_type_id", DbType.Int32));
				command.Parameters[0].Value = issue_type_id;
				command.Parameters.Add(new NpgsqlParameter("method_return_type", DbType.String));
				command.Parameters[1].Value = method_return_type;
				command.Parameters.Add(new NpgsqlParameter("method_namespace", DbType.String));
				command.Parameters[2].Value = method_namespace;
				command.Parameters.Add(new NpgsqlParameter("method_class", DbType.String));
				command.Parameters[3].Value = method_class;
				command.Parameters.Add(new NpgsqlParameter("method_name", DbType.String));
				command.Parameters[4].Value = method_name;
				command.Parameters.Add(new NpgsqlParameter("method_library", DbType.String));
				command.Parameters[5].Value = method_library;

				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with id returned, or 0 rows at all
						if (dr.FieldCount != 1)
						{
							throw new Exception("Got too many columns");
						}
						return (dr.GetInt32(0));
					}

					return (0);
				}
			}
		}

		static long InsertIssue(NpgsqlConnection conn, int issue_type_id, string method_return_type, string method_namespace, string method_class, string method_name, string method_library)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO issue (id, issue_type_id, method_return_type, method_namespace, method_class, method_name, method_library) VALUES (nextval ('issue_id_seq'), :issue_type_id, :method_return_type, :method_namespace, :method_class, :method_name, :method_library); SELECT currval('issue_id_seq');", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("issue_type_id", DbType.Int32));
				command.Parameters[0].Value = issue_type_id;
				command.Parameters.Add(new NpgsqlParameter("method_return_type", DbType.String));
				command.Parameters[1].Value = method_return_type;
				command.Parameters.Add(new NpgsqlParameter("method_namespace", DbType.String));
				command.Parameters[2].Value = method_namespace;
				command.Parameters.Add(new NpgsqlParameter("method_class", DbType.String));
				command.Parameters[3].Value = method_class;
				command.Parameters.Add(new NpgsqlParameter("method_name", DbType.String));
				command.Parameters[4].Value = method_name;
				command.Parameters.Add(new NpgsqlParameter("method_library", DbType.String));
				command.Parameters[5].Value = method_library;

				using (NpgsqlDataReader dr = command.ExecuteReader())
				{
					while (dr.Read())
					{
						// Should just have the one row with the newly inserted id returned
						if (dr.FieldCount != 1)
						{
							throw new Exception("Got too many columns");
						}

						return (dr.GetInt64(0));
					}

					throw new Exception("Error inserting issue");
				}
			}
		}

		static void InsertIssueDefinition(NpgsqlConnection conn, long issue_id, long def_id)
		{
			using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO issue_definition (issue_id, def_id) VALUES (:issue_id, :def_id);", conn))
			{
				command.Parameters.Add(new NpgsqlParameter("issue_id", DbType.Int64));
				command.Parameters[0].Value = issue_id;
				command.Parameters.Add(new NpgsqlParameter("def_id", DbType.Int64));
				command.Parameters[1].Value = def_id;

				command.ExecuteNonQuery();
			}
		}

		static void MarkLatestDefinition (NpgsqlConnection conn)
		{
			NpgsqlTransaction trans = conn.BeginTransaction();

			int def_id = GetLatestDefinitionID (conn);

			using (NpgsqlCommand command = new NpgsqlCommand("UPDATE issue SET is_latest_definition = false;", conn)) {
				/* This can take a while, so stop Npgsql
				 * 'canceling statement due to user request'
				 */
				command.CommandTimeout = 600;
				command.ExecuteNonQuery ();
			}

			using (NpgsqlCommand command = new NpgsqlCommand("UPDATE issue SET is_latest_definition = true WHERE id IN (SELECT issue_id FROM issue_definition WHERE def_id = :def_id);", conn)) {
				/* This can take a while, so stop Npgsql
				 * 'canceling statement due to user request'
				 */
				command.CommandTimeout = 600;
				command.Parameters.Add(new NpgsqlParameter("def_id", DbType.Int32));
				command.Parameters[0].Value = def_id;
				int rows = command.ExecuteNonQuery ();
				Console.WriteLine ("Marked {0} rows as belonging to latest definitions", rows);
			}

			trans.Commit();
		}

		static void Main(string[] args)
		{
			//Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
			//Npgsql.NpgsqlEventLog.LogName = "/tmp/npgsql-debug-log";

			string connstr = "Server=hagbard.apathetic.discordia.org.uk;Database=moma;User ID=dick;Password=test";
			NpgsqlConnection conn = new NpgsqlConnection(connstr);
			conn.Open();

			miss_id = GetIssueType(conn, "MISS");
			niex_id = GetIssueType(conn, "NIEX");
			todo_id = GetIssueType(conn, "TODO");

			foreach (string f in args) {
				InsertDefinition (conn, f);
			}

			/* Mark the issues from the latest definition */
			MarkLatestDefinition (conn);

			conn.Close();
		}

		static long GetDefinitionIDFromFile (NpgsqlConnection conn, string file)
		{
			ZipInputStream zs = new ZipInputStream (File.OpenRead (file));
			ZipEntry ze;

			while ((ze = zs.GetNextEntry ()) != null) {
				if (ze.Name == "version.txt") {
					StreamReader sr = new StreamReader (zs);
					string version = sr.ReadLine ();
					string date = sr.ReadLine ();

					DateTime date_parsed = new DateTime(int.Parse(date.Substring(6, 2)) + 2000, int.Parse(date.Substring(0, 2)), int.Parse(date.Substring(3, 2)));

					long defid = GetDefinitionID(conn, version, date_parsed);

					Console.WriteLine ("Defs {0} date {1}", version, date_parsed.ToString ());
					sr.Close();
					zs.Close();

					return defid;
				}
			}

			throw new Exception(String.Format("Couldn't find version info in {0}", file));
		}

		static void InsertDefinition (NpgsqlConnection conn, string file)
		{
			Console.WriteLine ("Loading defs from {0}", file);

			long def_id = GetDefinitionIDFromFile (conn, file);
			string line;
			StreamReader sr;

			Stream mt;
			Stream ni;
			Stream mi;

			DefinitionHandler.GetDefinitionStreamsFromBundle (file, out mt, out ni, out mi);

			NpgsqlTransaction trans = conn.BeginTransaction();

			sr = new StreamReader (mt);
			while ((line = sr.ReadLine ()) != null) {
				int split = line.IndexOf ("-");
				string method = line.Substring (0, split);

				InsertIssueIfNew (conn, method, todo_id, def_id, file);
			}
			sr.Close ();
			Console.WriteLine ("Done TODO");

			sr = new StreamReader (ni);
			while ((line = sr.ReadLine ()) != null) {
				InsertIssueIfNew (conn, line, niex_id, def_id, file);
			}
			sr.Close ();
			Console.WriteLine ("Done NIEX");

			sr = new StreamReader (mi);
			while ((line = sr.ReadLine ()) != null) {
				InsertIssueIfNew (conn, line, miss_id, def_id, file);
			}
			sr.Close ();
			Console.WriteLine ("Done MISS");

			mt.Close ();
			ni.Close ();
			mi.Close ();

			trans.Commit();
		}

		static void InsertIssueIfNew (NpgsqlConnection conn, string rest, int iss_type, long def_id, string file)
		{
			string ns = "";
			string class_name = "";
			string method_name = "";
			string return_type = "";

			if (rest.Contains (" modopt(")) {
				int modopt_start = rest.IndexOf (" modopt(");
				int modopt_end = rest.IndexOf (")", modopt_start);

				rest = rest.Substring (0, modopt_start) + rest.Substring (modopt_end + 1);
			}

			while (rest.Contains (" modreq(")) {
				int modreq_start = rest.IndexOf (" modreq(");
				int modreq_end = rest.IndexOf (")", modreq_start);

				rest = rest.Substring (0, modreq_start) + rest.Substring (modreq_end + 1);
			}

			try {
				string[] method_parts = rest.Split(' ');
				return_type = method_parts[0];
				string details = method_parts[1];
				int colons = details.IndexOf("::");
				if (colons != -1)
				{
					/* "::" was found */
					method_name = details.Substring(colons + 2);
					string[] ns_parts = details.Substring(0, colons).Split('.');
					class_name = ns_parts[ns_parts.Length - 1];

					for (int i = 0; i < ns_parts.Length - 1; i++)
					{
						if (ns.Length > 0)
						{
							ns += ".";
						}
						ns += ns_parts[i];
					}
				}
				else
				{
					/* "::" not found */
					Console.WriteLine ("Error parsing [{0}] (no colons) in file {1}", rest, file);
					return;
				}
			} catch {
				Console.WriteLine ("Error parsing [{0}] in file {1}", rest, file);
				return;
			}

			long iss_id = GetIssueID(conn, iss_type, return_type, ns, class_name, method_name, "");
			if (iss_id == 0)
			{
				iss_id = InsertIssue(conn, iss_type, return_type, ns, class_name, method_name, "");
			} else {
			}

			InsertIssueDefinition(conn, iss_id, def_id);
		}
	}
}

