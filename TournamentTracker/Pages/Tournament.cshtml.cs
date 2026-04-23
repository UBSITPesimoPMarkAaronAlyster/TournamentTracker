using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace TournamentTracker.Pages
{
    public class TournamentModel : PageModel
    {
        public List<Tournament> Tournaments { get; set; }

        public void OnGet()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM tournaments";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Tournaments = new List<Tournament>();
                        while (reader.Read())
                        {
                            Tournaments.Add(new Tournament
                            {
                                TournamentId = reader.GetInt32(0),
                                TournamentName = reader.GetString(1),
                                StartDate = reader.GetDateTime(2),
                                Status = reader.GetString(3)
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
    }
}