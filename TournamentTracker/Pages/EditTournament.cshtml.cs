using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;

namespace TournamentTracker.Pages
{
    public class EditTournamentModel : PageModel
    {
        [BindProperty]
        public TournamentEditModel Tournament { get; set; } = new TournamentEditModel(); // Renamed class here

        public void OnGet(int id)
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM tournaments WHERE tournament_id=@id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Correctly populate the model with the data
                            Tournament.TournamentId = id;
                            Tournament.TournamentName = reader["tournament_name"].ToString();
                            Tournament.StartDate = Convert.ToDateTime(reader["start_date"]);
                            Tournament.Status = reader["status"].ToString();
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

                string sql = @"UPDATE tournaments 
                               SET tournament_name=@name,
                                   start_date=@date,
                                   status=@status
                               WHERE tournament_id=@id";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Tournament.TournamentName);
                    cmd.Parameters.AddWithValue("@date", Tournament.StartDate);
                    cmd.Parameters.AddWithValue("@status", Tournament.Status);
                    cmd.Parameters.AddWithValue("@id", Tournament.TournamentId);  // Ensure the ID is correctly passed

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Tournament");  // Redirect back to the list of tournaments
        }
    }

    public class TournamentEditModel  // Renamed class here
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }
}