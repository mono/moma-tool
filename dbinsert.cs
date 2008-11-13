using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Data;
using Npgsql;
using System.Globalization;

namespace MoMADBAdd
{
    class Program
    {
        static int miss_id;
        static int niex_id;
        static int pinv_id;
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

        static int GetDefinitionID(NpgsqlConnection conn, string lookup_name)
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

                        return (dr.GetInt32(0));
                    }

                    throw new Exception(String.Format("Couldn't find {0}", lookup_name));
                }
            }
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

        static void InsertReportMetadata(NpgsqlConnection conn, long report_id)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO report_metadata (report_id) VALUES (:report_id);", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("report_id", DbType.Int64));
                command.Parameters[1].Value = report_id;

                int rows = command.ExecuteNonQuery();
            }
	}

        static long InsertReport(NpgsqlConnection conn, int def_id, string report_filename, DateTime report_date,
            string reporter_ip, string reporter_name, string reporter_email,
            string reporter_organization, string reporter_homepage,
            string reporter_comments)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO report (id, moma_definition_id, report_filename, report_date, reporter_ip, reporter_name, reporter_email, reporter_organization, reporter_homepage, reporter_comments) VALUES (nextval ('report_id_seq'), :defid, :report_filename, :report_date, :reporter_ip, :reporter_name, :reporter_email, :reporter_organization, :reporter_homepage, :reporter_comments); SELECT currval('report_id_seq');", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("defid", DbType.Int32));
                command.Parameters[0].Value = def_id;
                command.Parameters.Add(new NpgsqlParameter("report_filename", DbType.String));
                command.Parameters[1].Value = report_filename;
                command.Parameters.Add(new NpgsqlParameter("report_date", DbType.DateTime));
                command.Parameters[2].Value = report_date;
                command.Parameters.Add(new NpgsqlParameter("reporter_ip", DbType.String));
                command.Parameters[3].Value = reporter_ip;
                command.Parameters.Add(new NpgsqlParameter("reporter_name", DbType.String));
                command.Parameters[4].Value = reporter_name;
                command.Parameters.Add(new NpgsqlParameter("reporter_email", DbType.String));
                command.Parameters[5].Value = reporter_email;
                command.Parameters.Add(new NpgsqlParameter("reporter_organization", DbType.String));
                command.Parameters[6].Value = reporter_organization;
                command.Parameters.Add(new NpgsqlParameter("reporter_homepage", DbType.String));
                command.Parameters[7].Value = reporter_homepage;
                command.Parameters.Add(new NpgsqlParameter("reporter_comments", DbType.String));
                command.Parameters[8].Value = reporter_comments;

                using (NpgsqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Should just have the one row with the newly inserted id returned
                        if (dr.FieldCount != 1)
                        {
                            throw new Exception("Got too many columns");
                        }

                        long rep_id = dr.GetInt64(0);

			InsertReportMetadata (conn, rep_id);

                        return (rep_id);
                    }

                    throw new Exception("Error inserting report");
                }
            }
        }

        static void InsertIssueReport(NpgsqlConnection conn, long issue_id, long report_id)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO issue_report (issue_id, report_id) VALUES (:issue_id, :report_id);", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("issue_id", DbType.Int64));
                command.Parameters[0].Value = issue_id;
                command.Parameters.Add(new NpgsqlParameter("report_id", DbType.Int64));
                command.Parameters[1].Value = report_id;

                int rows = command.ExecuteNonQuery();
            }
        }

        static void Main(string[] args)
        {
            //Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
            //Npgsql.NpgsqlEventLog.LogName = "c:\\cygwin\\tmp\\npgsql-debug-log2";

            string connstr = "Server=hagbard.apathetic.discordia.org.uk;Database=moma;User ID=dick;Password=test";
            NpgsqlConnection conn = new NpgsqlConnection(connstr);
            conn.Open();

            miss_id = GetIssueType(conn, "MISS");
            niex_id = GetIssueType(conn, "NIEX");
            pinv_id = GetIssueType(conn, "PINV");
            todo_id = GetIssueType(conn, "TODO");

	    foreach (string f in args) {
	    	ParseReport (conn, f);
	    }

	    conn.Close();
	}

	static void ParseReport (NpgsqlConnection conn, string file)
	{
	    using (FileStream fs = File.OpenRead (file)) {
		    Dictionary<string, string> meta = new Dictionary<string, string>();
		    StreamReader stream = new StreamReader(fs);
		    CultureInfo cult = CultureInfo.CreateSpecificCulture("en-US");
		    string date = stream.ReadLine();
		    DateTime date_parsed = DateTime.Parse(date, cult);
		    string ip = stream.ReadLine();
		    string r = ReadMeta(stream, meta);

		    int defid = GetDefinitionID(conn, meta["@Definitions"]);

		    NpgsqlTransaction trans = conn.BeginTransaction();

		    long rep_id = InsertReport(conn, defid, Path.GetFileName(file), date_parsed, ip, meta["@Name"], meta["@Email"], meta["@Organization"], meta["@HomePage"], meta["@Comments"]);

		    for (; r != null && r.Length > 6; r = stream.ReadLine())
		    {
			int iss_type;

			if (r[r.Length - 1] == '\r')
			{
			    r = r.Substring(0, r.Length - 1);
			}

			switch (r.Substring(0, 6))
			{
			    case "[TODO]":
				iss_type = todo_id;
				break;
			    case "[NIEX]":
				iss_type = niex_id;
				break;
			    case "[MISS]":
				iss_type = miss_id;
				break;
			    case "[PINV]":
				iss_type = pinv_id;
				break;
			    default:
				continue;
			}

			string rest = r.Substring(7);

			string ns = "";
			string class_name = "";
			string method_name = "";
			string lib = "";
			string return_type = "";

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
				    method_parts = rest.Split('-');
				    method_name = method_parts[0].Split(new char[] { ' ' }, 2)[1];
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

			long iss_id = GetIssueID(conn, iss_type, return_type, ns, class_name, method_name, lib);
			if (iss_id == 0)
			{
			    iss_id = InsertIssue(conn, iss_type, return_type, ns, class_name, method_name, lib);
			}

			InsertIssueReport(conn, iss_id, rep_id);
		    }

		    trans.Commit();
	    }
        }

        static string ReadMeta(StreamReader stream, Dictionary<string, string> meta)
        {
            string s;

            while ((s = stream.ReadLine()) != null)
            {
                if (s.Length == 0 || s[0] == '[')
                {
                    return s;
                }
                if (s[0] != '@')
                {
                    return s;
                }

                int p = s.IndexOf(':');
                string v = s.Substring(p + 2);
                meta[s.Substring(0, p)] = v;
            }

            return s;
        }
    }
}
