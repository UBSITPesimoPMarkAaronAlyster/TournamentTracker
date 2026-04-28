using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class CreateMatchModel : PageModel
    {
        [BindProperty]
        public Match Match { get; set; } = new Match();

        public List<CreateMatchTournamentItem> TournamentList { get; set; } = new List<CreateMatchTournamentItem>();

        public List<CreateMatchTeamItem> TeamList { get; set; } = new List<CreateMatchTeamItem>();

        public void OnGet()
        {
            LoadTournaments();
            LoadTeams();
        }

        public IActionResult OnPost()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            int winnerId = 0;

            if (Match.ScoreTeam1 > Match.ScoreTeam2)
            {
                winnerId = Match.Team1Id;
            }
            else if (Match.ScoreTeam2 > Match.ScoreTeam1)
            {
                winnerId = Match.Team2Id;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    INSERT INTO matches 
                    (tournament_id, team1_id, team2_id, score_team1, score_team2, winner_team_id)
                    VALUES 
                    (@tournament_id, @team1_id, @team2_id, @score_team1, @score_team2, @winner_team_id)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tournament_id", Match.TournamentId);
                    cmd.Parameters.AddWithValue("@team1_id", Match.Team1Id);
                    cmd.Parameters.AddWithValue("@team2_id", Match.Team2Id);
                    cmd.Parameters.AddWithValue("@score_team1", Match.ScoreTeam1);
                    cmd.Parameters.AddWithValue("@score_team2", Match.ScoreTeam2);

                    if (winnerId == 0)
                    {
                        cmd.Parameters.AddWithValue("@winner_team_id", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@winner_team_id", winnerId);
                    }

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Matches");
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
                            TournamentList.Add(new CreateMatchTournamentItem
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

        private void LoadTeams()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT team_id, team_name
                    FROM teams
                    WHERE team_name IS NOT NULL
                    ORDER BY team_id ASC";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TeamList.Add(new CreateMatchTeamItem
                            {
                                TeamId = reader.GetInt32("team_id"),
                                TeamName = reader.IsDBNull(reader.GetOrdinal("team_name"))
                                    ? "Unnamed Team"
                                    : reader.GetString("team_name")
                            });
                        }
                    }
                }
            }
        }
    }

    public class Match
    {
        public int MatchId { get; set; }

        public int TournamentId { get; set; }

        public int Team1Id { get; set; }

        public int Team2Id { get; set; }

        public int ScoreTeam1 { get; set; }

        public int ScoreTeam2 { get; set; }

        public int WinnerTeamId { get; set; }
    }

    public class CreateMatchTournamentItem
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";
    }

    public class CreateMatchTeamItem
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = "";
    }
}