using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class MatchesModel : PageModel
    {
        public List<MatchView> MatchList { get; set; } = new List<MatchView>();

        public void OnGet()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT 
                        m.match_id,
                        t.tournament_name,
                        team1.team_name AS team1_name,
                        team2.team_name AS team2_name,
                        m.score_team1,
                        m.score_team2,
                        winner.team_name AS winner_name
                    FROM matches m
                    LEFT JOIN tournaments t ON m.tournament_id = t.tournament_id
                    LEFT JOIN teams team1 ON m.team1_id = team1.team_id
                    LEFT JOIN teams team2 ON m.team2_id = team2.team_id
                    LEFT JOIN teams winner ON m.winner_team_id = winner.team_id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MatchList.Add(new MatchView
                            {
                                MatchId = reader.GetInt32("match_id"),
                                TournamentName = reader.IsDBNull(reader.GetOrdinal("tournament_name")) ? "No tournament" : reader.GetString("tournament_name"),
                                Team1Name = reader.IsDBNull(reader.GetOrdinal("team1_name")) ? "No team" : reader.GetString("team1_name"),
                                Team2Name = reader.IsDBNull(reader.GetOrdinal("team2_name")) ? "No team" : reader.GetString("team2_name"),
                                ScoreTeam1 = reader.GetInt32("score_team1"),
                                ScoreTeam2 = reader.GetInt32("score_team2"),
                                WinnerName = reader.IsDBNull(reader.GetOrdinal("winner_name")) ? "No winner yet" : reader.GetString("winner_name")
                            });
                        }
                    }
                }
            }
        }
    }

    public class MatchView
    {
        public int MatchId { get; set; }
        public string TournamentName { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public string WinnerName { get; set; }
    }
}