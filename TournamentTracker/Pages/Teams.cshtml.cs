using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class TeamsModel : PageModel
    {
        public List<TeamListItem> TeamList { get; set; } = new List<TeamListItem>();

        public void OnGet()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        teams.team_id,
                        teams.team_name,
                        teams.tournament_id,
                        tournaments.tournament_name
                    FROM teams
                    LEFT JOIN tournaments
                    ON teams.tournament_id = tournaments.tournament_id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TeamList.Add(new TeamListItem
                            {
                                TeamId = reader.GetInt32("team_id"),
                                TeamName = reader.IsDBNull(reader.GetOrdinal("team_name"))
                                    ? "Unnamed Team"
                                    : reader.GetString("team_name"),
                                TournamentId = reader.GetInt32("tournament_id"),
                                TournamentName = reader.IsDBNull(reader.GetOrdinal("tournament_name"))
                                    ? "No Tournament"
                                    : reader.GetString("tournament_name")
                            });
                        }
                    }
                }
            }
        }
    }

    public class TeamListItem
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = "";

        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";
    }
}