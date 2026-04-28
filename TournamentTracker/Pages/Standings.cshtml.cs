using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class StandingsModel : PageModel
    {
        public List<Standing> StandingsList { get; set; } = new();

        public void OnGet()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                SELECT
                    t.team_name,

                    SUM(
                        CASE
                        WHEN m.winner_team_id=t.team_id THEN 1
                        ELSE 0
                        END
                    ) as wins,

                    SUM(
                        CASE
                        WHEN m.winner_team_id IS NOT NULL
                        AND m.winner_team_id<>t.team_id THEN 1
                        ELSE 0
                        END
                    ) as losses

                FROM teams t

                LEFT JOIN matches m
                ON t.team_id=m.team1_id OR t.team_id=m.team2_id

                GROUP BY t.team_id
                ORDER BY wins DESC;
                ";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        StandingsList.Add(new Standing
                        {
                            TeamName = reader.GetString("team_name"),
                            Wins = reader.GetInt32("wins"),
                            Losses = reader.GetInt32("losses")
                        });
                    }
                }
            }
        }
    }

    public class Standing
    {
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}