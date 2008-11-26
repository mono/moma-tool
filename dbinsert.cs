using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Data;
using Npgsql;
using System.Globalization;

using System.Xml;

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
                command.Parameters[0].Value = report_id;

                int rows = command.ExecuteNonQuery();
            }
	}

        static long InsertReport(NpgsqlConnection conn, int def_id, string report_filename, DateTime report_date,
            string reporter_ip, string reporter_name, string reporter_email,
            string reporter_organization, string reporter_homepage,
            string reporter_comments, string apptype)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO report (id, moma_definition_id, report_filename, report_date, reporter_ip, reporter_name, reporter_email, reporter_organization, reporter_homepage, reporter_comments, application_type) VALUES (nextval ('report_id_seq'), :defid, :report_filename, :report_date, :reporter_ip, :reporter_name, :reporter_email, :reporter_organization, :reporter_homepage, :reporter_comments, :apptype); SELECT currval('report_id_seq');", conn))
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
                command.Parameters.Add(new NpgsqlParameter("apptype", DbType.String));
                command.Parameters[9].Value = apptype;

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

        static long InsertReportAssembly(NpgsqlConnection conn, long rep_id,
		string ass_name, string ass_version, string ass_runtime,
		int ass_todo, int ass_niex, int ass_miss, int ass_pinv)
	{
		using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO report_assembly (id, report_id, assembly_name, assembly_version, assembly_runtime, todo, niex, miss, pinv) VALUES (nextval ('report_assembly_id_seq'), :rep_id, :ass_name, :ass_version, :ass_runtime, :ass_todo, :ass_niex, :ass_miss, :ass_pinv); SELECT currval('report_assembly_id_seq');", conn))
		{
			command.Parameters.Add(new NpgsqlParameter("rep_id", DbType.Int64));
			command.Parameters[0].Value = rep_id;
			command.Parameters.Add(new NpgsqlParameter("ass_name", DbType.String));
			command.Parameters[1].Value = ass_name;
			command.Parameters.Add(new NpgsqlParameter("ass_version", DbType.String));
			command.Parameters[2].Value = ass_version;
			command.Parameters.Add(new NpgsqlParameter("ass_runtime", DbType.String));
			command.Parameters[3].Value = ass_runtime;
			command.Parameters.Add(new NpgsqlParameter("ass_todo", DbType.Int32));
			command.Parameters[4].Value = ass_todo;
			command.Parameters.Add(new NpgsqlParameter("ass_niex", DbType.Int32));
			command.Parameters[5].Value = ass_niex;
			command.Parameters.Add(new NpgsqlParameter("ass_miss", DbType.Int32));
			command.Parameters[6].Value = ass_miss;
			command.Parameters.Add(new NpgsqlParameter("ass_pinv", DbType.Int32));
			command.Parameters[7].Value = ass_pinv;

			using (NpgsqlDataReader dr = command.ExecuteReader())
			{
				while (dr.Read())
				{
					// Should just have the one row with the newly inserted id returned
					if (dr.FieldCount != 1)
					{
						throw new Exception("Got too many columns");
					}

					long ass_id = dr.GetInt64(0);

					return (ass_id);
				}

				throw new Exception("Error inserting report_assembly");
			}
		}
	}

        static void InsertIssueReport(NpgsqlConnection conn, long issue_id, long report_id, long ass_id, string caller_class, string caller_method)
        {
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO issue_report (issue_id, report_id, assembly_id, caller_class, caller_method) VALUES (:issue_id, :report_id, :ass_id, :caller_class, :caller_method);", conn))
            {
                command.Parameters.Add(new NpgsqlParameter("issue_id", DbType.Int64));
                command.Parameters[0].Value = issue_id;
                command.Parameters.Add(new NpgsqlParameter("report_id", DbType.Int64));
                command.Parameters[1].Value = report_id;
		if (ass_id == -1) {
			command.Parameters.Add(new NpgsqlParameter("ass_id", DbType.Int64));
			command.Parameters[2].Value = null;
		} else {
			command.Parameters.Add(new NpgsqlParameter("ass_id", DbType.Int64));
			command.Parameters[2].Value = ass_id;
		}
                command.Parameters.Add(new NpgsqlParameter("caller_class", DbType.String));
                command.Parameters[3].Value = caller_class;
                command.Parameters.Add(new NpgsqlParameter("caller_method", DbType.String));
                command.Parameters[4].Value = caller_method;

                int rows = command.ExecuteNonQuery();
            }
        }

	static void UpdateReportTotals (NpgsqlConnection conn, long rep_id, int tot_todo, int tot_niex, int tot_miss, int tot_pinv, int tot_total)
	{
		//Console.WriteLine ("Report {0} totals: todo {1} niex {2} miss {3} pinv {4} total {5}", rep_id, tot_todo, tot_niex, tot_miss, tot_pinv, tot_total);
		using (NpgsqlCommand command = new NpgsqlCommand ("UPDATE report SET todo = :tot_todo, niex = :tot_niex, miss = :tot_miss, pinv = :tot_pinv, total = :tot_total WHERE id = :rep_id;", conn)) {
			command.Parameters.Add (new NpgsqlParameter ("tot_todo", DbType.Int32));
			command.Parameters[0].Value = tot_todo;
			command.Parameters.Add (new NpgsqlParameter ("tot_niex", DbType.Int32));
			command.Parameters[1].Value = tot_niex;
			command.Parameters.Add (new NpgsqlParameter ("tot_miss", DbType.Int32));
			command.Parameters[2].Value = tot_miss;
			command.Parameters.Add (new NpgsqlParameter ("tot_pinv", DbType.Int32));
			command.Parameters[3].Value = tot_pinv;
			command.Parameters.Add (new NpgsqlParameter ("tot_total", DbType.Int32));
			command.Parameters[4].Value = tot_total;
			command.Parameters.Add (new NpgsqlParameter ("rep_id", DbType.Int64));
			command.Parameters[5].Value = rep_id;

			command.ExecuteNonQuery ();
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

	    NpgsqlTransaction trans = conn.BeginTransaction();

	    foreach (string f in args) {
		Console.WriteLine ("Inserting [{0}]", f);
	    	ParseReport (conn, f);
	    }

	    trans.Commit();
	    //trans.Rollback();

	    conn.Close();
	}

	static void ParseReport (NpgsqlConnection conn, string file)
	{
	    using (FileStream fs = File.OpenRead (file)) {
		    StreamReader stream = new StreamReader(fs);
		    CultureInfo cult = CultureInfo.CreateSpecificCulture("en-US");
		    string date = stream.ReadLine();
		    DateTime date_parsed = DateTime.Parse(date, cult);
		    string ip = stream.ReadLine();

		    int peek = stream.Peek ();
		    if (peek == -1) {
		    	Console.WriteLine ("Can't tell which report format this is!");
			return;
		    }
		    char next_char = (char)peek;
		    if (next_char == '<') {
		    	ParseReport_xml (conn, stream, file, date_parsed, ip);
		    } else {
		    	ParseReport_text (conn, stream, file, date_parsed, ip);
		    }
	    }
	}

	static void ParseReport_xml (NpgsqlConnection conn, StreamReader stream, string file, DateTime date_parsed, string ip)
	{
		string def_ver = null;
		string moma_ver = null;
		string date = null;
		string rep_name = null;
		string rep_email = null;
		string rep_homepage = null;
		string rep_org = null;
		string rep_comments = null;
		string apptype = null;
		long rep_id = -1;
		long ass_id = -1;
		int tot_todo = 0;
		int tot_niex = 0;
		int tot_miss = 0;
		int tot_pinv = 0;
		int tot_total = 0;


		XmlReaderSettings settings = new XmlReaderSettings ();
		XmlReader reader = XmlReader.Create (stream, settings);

		reader.Read (); // xml tag
		while (reader.Read ()) {
			if (reader.NodeType == XmlNodeType.Whitespace) {
				continue;
			}

			switch (reader.Name) {
			case "report":
				string rep_version = reader.GetAttribute ("version");
				if (rep_version == null) {
					Console.WriteLine ("No version!");
					return;
				} else if (rep_version != "1.0") {
					Console.WriteLine ("Don't know how to parse v{0} reports!", rep_version);
					return;
				} else {
					//Console.WriteLine ("Report version is v{0}", rep_version);
				}

				XmlReader report = reader.ReadSubtree ();
				report.Read (); // report tag
				while (report.Read ()) {
					if (report.NodeType == XmlNodeType.EndElement) {
						/* Must be the closing </report> tag */
						break;
					}

					switch (report.Name) {
					case "metadata":
						XmlReader metadata = reader.ReadSubtree ();
						metadata.Read (); // metadata tag
						while (metadata.Read ()) {
							if (metadata.NodeType == XmlNodeType.EndElement) {
								/* Must be the closing </metadata> tag */
								break;
							}

							switch (metadata.Name) {
							case "definitions":
								def_ver = metadata.ReadString ();
								break;
							case "momaversion":
								moma_ver = metadata.ReadString ();
								break;
							case "date":
								date = metadata.ReadString ();
								break;
							case "name":
								rep_name = metadata.ReadString ();
								break;
							case "email":
								rep_email = metadata.ReadString ();
								break;
							case "homepage":
								rep_homepage = metadata.ReadString ();
								break;
							case "organization":
								rep_org = metadata.ReadString ();
								break;
							case "comments":
								rep_comments = metadata.ReadString ();
								break;
							case "apptype":
								apptype = metadata.ReadString ();
								break;
							default:
								Console.WriteLine ("Don't know how to handle this report/metadata element: [{0}]", metadata.Name);
								return;
							}
						}
						metadata.Close ();

						int defid = GetDefinitionID(conn, def_ver);

						rep_id = InsertReport(conn, defid, Path.GetFileName(file), date_parsed, ip, rep_name, rep_email, rep_org, rep_homepage, rep_comments, apptype);
						break;
					case "assemblies":
						XmlReader assemblies = reader.ReadSubtree ();
						assemblies.Read (); // assemblies tag
						while (assemblies.Read ()) {
							if (assemblies.NodeType == XmlNodeType.EndElement) {
								/* Must be the closing </assemblies> tag */
								break;
							}

							switch (assemblies.Name) {
							case "assembly":
								string ass_name = reader.GetAttribute ("name");
								string ass_version = reader.GetAttribute ("version");
								string ass_runtime = reader.GetAttribute ("runtime");
								int ass_todo = int.Parse (reader.GetAttribute ("todo"));
								int ass_niex = int.Parse (reader.GetAttribute ("niex"));
								int ass_miss = int.Parse (reader.GetAttribute ("miss"));
								int ass_pinv = int.Parse (reader.GetAttribute ("pinv"));

								if (rep_id == -1) {
									Console.WriteLine ("Read error: Assembly but no report!");
									return;
								}

								tot_todo += ass_todo;
								tot_niex += ass_niex;
								tot_miss += ass_miss;
								tot_pinv += ass_pinv;
								tot_total += (ass_todo + ass_niex + ass_miss + ass_pinv);

								ass_id = InsertReportAssembly (conn, rep_id, ass_name, ass_version, ass_runtime, ass_todo, ass_niex, ass_miss, ass_pinv);
								//Console.WriteLine ("ass_id is {0}", ass_id);

								XmlReader assembly = assemblies.ReadSubtree ();
								assembly.Read (); // assembly tag
								while (assembly.Read ()) {
									if (assembly.NodeType == XmlNodeType.EndElement) {
										/* Must be the closing </assembly> tag */
										break;
									}

									switch (assembly.Name) {
									case "issue":
										string iss_type_string = reader.GetAttribute ("type");
										string iss_class = null;
										string iss_caller = null;
										string iss_method = null;
										string iss_raw = null;
										string iss_data = null;
										int iss_type = -1;
										string return_type = "";
										string ns = "";
										string class_name = "";
										string method_name = "";
										string lib = "";

										XmlReader issue = assembly.ReadSubtree ();
										issue.Read (); // assembly tag
										while (issue.Read ()) {
											if (issue.NodeType == XmlNodeType.EndElement) {
												/* Must be the closing </assembly> tag */
												break;
											}

											switch (issue.Name) {
											case "class":
												iss_class = issue.ReadString ();
												break;
											case "caller":
												iss_caller = issue.ReadString ();
												break;
											case "method":
												iss_method = issue.ReadString ();
												break;
											case "raw":
												iss_raw = issue.ReadString ();
												break;
											case "data":
												iss_data = issue.ReadString ();
												break;
											default:
												Console.WriteLine ("Don't know how to handle this report/assemblies/assembly/issue element: [{0}]", issue.Name);
												return;
											}
										}
										issue.Close ();

										switch (iss_type_string) {
										case "todo":
											iss_type = todo_id;
											break;
										case "niex":
											iss_type = niex_id;
											break;
										case "miss":
											iss_type = miss_id;
											break;
										case "pinv":
											iss_type = pinv_id;
											break;
										default:
											Console.WriteLine ("Don't know how to handle issue type [{0}]", iss_type_string);
											return;
										}

										string[] method_parts = iss_raw.Split(' ');
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
										} else {
											Console.WriteLine ("Parse error!");
											return;
										}

										if (iss_type_string == "pinv") {
											lib = iss_data;
										}

										//Console.WriteLine ("iss_type: {0}, return_type [{1}], ns [{2}], class_name [{3}], method_name [{4}], lib [{5}]", iss_type, return_type, ns, class_name, method_name, lib);
										long iss_id = GetIssueID(conn, iss_type, return_type, ns, class_name, method_name, lib);
										if (iss_id == 0)
										{
											iss_id = InsertIssue(conn, iss_type, return_type, ns, class_name, method_name, lib);
										}

										InsertIssueReport(conn, iss_id, rep_id, ass_id, iss_class, iss_caller);
										break;
									default:
										Console.WriteLine ("Don't know how to handle this report/assemblies/assembly element: [{0}]", assembly.Name);
										return;
									}
								}
								assembly.Close ();
								break;
							default:
								Console.WriteLine ("Don't know how to handle this report/assemblies element: [{0}]", assemblies.Name);
								return;
							}
						}
						assemblies.Close ();
						break;
					default:
						Console.WriteLine ("Don't know how to handle this report element: [{0}]", report.Name);
						break;
					}
				}
				report.Close ();
				break;
			default:
				Console.WriteLine ("Don't know how to handle this element: [{0}]", reader.Name);
				break;
			}
		}
		reader.Close ();

		if (rep_id != -1) {
			UpdateReportTotals (conn, rep_id, tot_todo, tot_niex, tot_miss, tot_pinv, tot_total);
		}
	}

	static void ParseReport_text (NpgsqlConnection conn, StreamReader stream, string file, DateTime date_parsed, string ip)
	{
		Dictionary<string, string> meta = new Dictionary<string, string>();
		string r = ReadMeta(stream, meta);

		int defid = GetDefinitionID(conn, meta["@Definitions"]);

		long rep_id = InsertReport(conn, defid, Path.GetFileName(file), date_parsed, ip, meta["@Name"], meta["@Email"], meta["@Organization"], meta["@HomePage"], meta["@Comments"], null);

		int tot_todo = 0;
		int tot_niex = 0;
		int tot_miss = 0;
		int tot_pinv = 0;
		int tot_total = 0;

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

			InsertIssueReport(conn, iss_id, rep_id, -1, null, null);

			if (iss_type == todo_id) {
				tot_todo++;
			} else if (iss_type == niex_id) {
				tot_niex++;
			} else if (iss_type == miss_id) {
				tot_miss++;
			} else if (iss_type == pinv_id) {
				tot_pinv++;
			}
			tot_total++;
		}
		UpdateReportTotals (conn, rep_id, tot_todo, tot_niex, tot_miss, tot_pinv, tot_total);
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
