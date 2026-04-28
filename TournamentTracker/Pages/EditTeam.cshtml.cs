using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class EditTeamModel : PageModel
    {
        [BindProperty]
        public EditTeamInput Team { get; set; } = new EditTeamInput();

        public List<EditTeamTournamentItem> TournamentList { get; set; } = new List<EditTeamTournamentItem>();

        public void OnGet(int id)
        {
            LoadTournaments();

            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM teams WHERE team_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Team.TeamId = reader.GetInt32("team_id");
                            Team.TeamName = reader.IsDBNull(reader.GetOrdinal("team_name"))
                                ? ""
                                : reader.GetString("team_name");
                            Team.TournamentId = reader.GetInt32("tournament_id");
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    UPDATE teams
                    SET team_name = @team_name,
                        tournament_id = @tournament_id
                    WHERE team_id = @team_id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@team_name", Team.TeamName);
                    cmd.Parameters.AddWithValue("@tournament_id", Team.TournamentId);
                    cmd.Parameters.AddWithValue("@team_id", Team.TeamId);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Teams");
        }

        private void LoadTournaments()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT tournament_id, tournament_name
                    FROM tournaments
                    WHERE tournament_name IS NOT NULL
                    ORDER BY tournament_id DESC";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TournamentList.Add(new EditTeamTournamentItem
                            {
                                TournamentId = reader.GetInt32("tournament_id"),
                                TournamentName = reader.IsDBNull(reader.GetOrdinal("tournament_name"))
                                    ? "Unnamed Tournament"
                                    : reader.GetString("tournament_name")
                            });
                        }
                    }
                }
            }
        }
    }

    public class EditTeamInput
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = "";

        public int TournamentId { get; set; }
    }

    public class EditTeamTournamentItem
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";
    }
}