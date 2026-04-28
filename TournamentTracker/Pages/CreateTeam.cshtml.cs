using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class CreateTeamModel : PageModel
    {
        [BindProperty]
        public CreateTeamInput Team { get; set; } = new CreateTeamInput();

        public List<CreateTeamTournamentItem> TournamentList { get; set; } = new List<CreateTeamTournamentItem>();

        public void OnGet()
        {
            LoadTournaments();
        }

        public IActionResult OnPost()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "INSERT INTO teams (team_name, tournament_id) VALUES (@team_name, @tournament_id)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@team_name", Team.TeamName);
                    cmd.Parameters.AddWithValue("@tournament_id", Team.TournamentId);

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
                            TournamentList.Add(new CreateTeamTournamentItem
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

    public class CreateTeamInput
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = "";

        public int TournamentId { get; set; }
    }

    public class CreateTeamTournamentItem
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";
    }
}