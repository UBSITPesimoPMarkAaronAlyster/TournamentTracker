using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class TournamentModel : PageModel
    {
        public List<Tournament> TournamentList { get; set; }
            = new List<Tournament>();

        public void OnGet()
        {
            string connStr =
            "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn =
                   new MySqlConnection(connStr))
            {
                conn.Open();

                string sql =
                @"SELECT tournament_id,
                         tournament_name,
                         start_date,
                         status,
                         bracket_size
                  FROM tournaments";

                using (MySqlCommand cmd =
                       new MySqlCommand(sql, conn))
                {
                    using (var reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TournamentList.Add(
                                new Tournament
                                {
                                    TournamentId =
                                        reader.GetInt32("tournament_id"),

                                    TournamentName =
                                        reader.GetString("tournament_name"),

                                    StartDate =
                                        reader.GetDateTime("start_date"),

                                    Status =
                                        reader.GetString("status"),

                                    BracketSize =
                                        reader.GetInt32("bracket_size")
                                });
                        }
                    }
                }
            }
        }
    }

    public class Tournament
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; }

        public DateTime StartDate { get; set; }

        public string Status { get; set; }

        public int BracketSize { get; set; }
    }
}