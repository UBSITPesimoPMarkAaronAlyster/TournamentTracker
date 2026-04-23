using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;

namespace TournamentTracker.Pages
{
    public class CreateTournamentModel : PageModel
    {
        [BindProperty]
        public TournamentInput Tournament { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"INSERT INTO tournaments 
                               (tournament_name, start_date, status) 
                               VALUES (@name, @date, @status)";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", Tournament.TournamentName);
                    cmd.Parameters.AddWithValue("@date", Tournament.StartDate);
                    cmd.Parameters.AddWithValue("@status", Tournament.Status);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Tournament");
        }
    }

    public class TournamentInput
    {
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }
}