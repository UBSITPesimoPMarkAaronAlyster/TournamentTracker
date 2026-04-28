using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class BracketModel : PageModel
    {
        public List<BracketTournamentOption> TournamentList { get; set; } = new List<BracketTournamentOption>();

        public List<string> TeamSlots { get; set; } = new List<string>();

        public int SelectedTournamentId { get; set; }

        public string SelectedTournamentName { get; set; } = "No Tournament Selected";

        public int BracketSize { get; set; } = 4;

        public void OnGet(int? tournamentId)
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                LoadTournaments(conn);

                if (TournamentList.Count == 0)
                {
                    return;
                }

                SelectedTournamentId = tournamentId ?? TournamentList[0].TournamentId;

                LoadSelectedTournament(conn);
                LoadTeams(conn);
                FillEmptySlots();
            }
        }

        private void LoadTournaments(MySqlConnection conn)
        {
            string sql = "SELECT tournament_id, tournament_name, bracket_size FROM tournaments";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TournamentList.Add(new BracketTournamentOption
                        {
                            TournamentId = reader.GetInt32("tournament_id"),
                            TournamentName = reader.GetString("tournament_name"),
                            BracketSize = reader.GetInt32("bracket_size")
                        });
                    }
                }
            }
        }

        private void LoadSelectedTournament(MySqlConnection conn)
        {
            string sql = "SELECT tournament_name, bracket_size FROM tournaments WHERE tournament_id = @id";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", SelectedTournamentId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        SelectedTournamentName = reader.GetString("tournament_name");
                        BracketSize = reader.GetInt32("bracket_size");
                    }
                }
            }
        }

        private void LoadTeams(MySqlConnection conn)
        {
            string sql = "SELECT team_name FROM teams WHERE tournament_id = @id ORDER BY team_id ASC";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", SelectedTournamentId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TeamSlots.Add(reader.GetString("team_name"));
                    }
                }
            }
        }

        private void FillEmptySlots()
        {
            while (TeamSlots.Count < BracketSize)
            {
                TeamSlots.Add("BYE");
            }

            if (TeamSlots.Count > BracketSize)
            {
                TeamSlots = TeamSlots.GetRange(0, BracketSize);
            }
        }
    }

    public class BracketTournamentOption
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";

        public int BracketSize { get; set; }
    }
}