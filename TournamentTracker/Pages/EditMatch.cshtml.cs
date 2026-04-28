using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class EditMatchModel : PageModel
    {
        [BindProperty]
        public Match Match { get; set; } = new Match();

        public List<EditMatchTournamentItem> TournamentList { get; set; } = new List<EditMatchTournamentItem>();

        public List<EditMatchTeamItem> TeamList { get; set; } = new List<EditMatchTeamItem>();

        public void OnGet(int id)
        {
            LoadTournaments();
            LoadTeams();

            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM matches WHERE match_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Match.MatchId = reader.GetInt32("match_id");
                            Match.TournamentId = reader.GetInt32("tournament_id");
                            Match.Team1Id = reader.GetInt32("team1_id");
                            Match.Team2Id = reader.GetInt32("team2_id");
                            Match.ScoreTeam1 = reader.GetInt32("score_team1");
                            Match.ScoreTeam2 = reader.GetInt32("score_team2");
                        }
                    }
                }
            }
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
                    UPDATE matches
                    SET tournament_id = @tournament_id,
                        team1_id = @team1_id,
                        team2_id = @team2_id,
                        score_team1 = @score_team1,
                        score_team2 = @score_team2,
                        winner_team_id = @winner_team_id
                    WHERE match_id = @match_id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tournament_id", Match.TournamentId);
                    cmd.Parameters.AddWithValue("@team1_id", Match.Team1Id);
                    cmd.Parameters.AddWithValue("@team2_id", Match.Team2Id);
                    cmd.Parameters.AddWithValue("@score_team1", Match.ScoreTeam1);
                    cmd.Parameters.AddWithValue("@score_team2", Match.ScoreTeam2);
                    cmd.Parameters.AddWithValue("@match_id", Match.MatchId);

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
                            TournamentList.Add(new EditMatchTournamentItem
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
                            TeamList.Add(new EditMatchTeamItem
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

    public class EditMatchTournamentItem
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";
    }

    public class EditMatchTeamItem
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = "";
    }
}