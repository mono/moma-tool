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
		static void UpdateReportTotals (NpgsqlConnection conn, long rep_id, long tot_todo, long tot_niex, long tot_miss, long tot_pinv, long tot_total)
		{
			Console.WriteLine ("Report {0} totals: todo {1} niex {2} miss {3} pinv {4} total {5}", rep_id, tot_todo, tot_niex, tot_miss, tot_pinv, tot_total);
			using (NpgsqlCommand command = new NpgsqlCommand ("UPDATE report SET todo = :tot_todo, niex = :tot_niex, miss = :tot_miss, pinv = :tot_pinv, total = :tot_total WHERE id = :rep_id;", conn)) {
				command.Parameters.Add (new NpgsqlParameter ("tot_todo", DbType.Int64));
				command.Parameters[0].Value = tot_todo;
				command.Parameters.Add (new NpgsqlParameter ("tot_niex", DbType.Int64));
				command.Parameters[1].Value = tot_niex;
				command.Parameters.Add (new NpgsqlParameter ("tot_miss", DbType.Int64));
				command.Parameters[2].Value = tot_miss;
				command.Parameters.Add (new NpgsqlParameter ("tot_pinv", DbType.Int64));
				command.Parameters[3].Value = tot_pinv;
				command.Parameters.Add (new NpgsqlParameter ("tot_total", DbType.Int64));
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

			NpgsqlTransaction trans = conn.BeginTransaction();

			using (NpgsqlCommand command = new NpgsqlCommand ("SELECT rep.id, todo.todo, niex.niex, miss.miss, pinv.pinv, total.total FROM report rep LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS miss FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id GROUP BY report_id) AS miss ON rep.id = miss.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS niex FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id GROUP BY report_id) AS niex ON rep.id = niex.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS pinv FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' AND issue_report.issue_id = issue.id GROUP BY report_id) AS pinv ON rep.id = pinv.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS todo FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id GROUP BY report_id) AS todo ON rep.id = todo.report_id LEFT JOIN (SELECT report_id, COUNT(report_id) AS total FROM issue_report GROUP BY report_id) AS total ON rep.id = total.report_id;", conn)) {
				using (NpgsqlDataReader dr = command.ExecuteReader ()) {
					while (dr.Read ()) {
						// Should have 6 columns
						if (dr.FieldCount != 6) {
							throw new Exception ("Didn't get 6 columns");
						}

						long todo = 0;
						long niex = 0;
						long miss = 0;
						long pinv = 0;
						long total = 0;

						if (!dr.IsDBNull(1)) {
							todo = dr.GetInt64(1);
						}
						if (!dr.IsDBNull(2)) {
							niex = dr.GetInt64(2);
						}
						if (!dr.IsDBNull(3)) {
							miss = dr.GetInt64(3);
						}
						if (!dr.IsDBNull(4)) {
							pinv = dr.GetInt64(4);
						}
						if (!dr.IsDBNull(5)) {
							total = dr.GetInt64(5);
						}
						UpdateReportTotals (conn, dr.GetInt32(0), todo, niex, miss, pinv, total);
					}
				}
			}

			trans.Commit();
			//trans.Rollback();

			conn.Close();
		}
	}
}

