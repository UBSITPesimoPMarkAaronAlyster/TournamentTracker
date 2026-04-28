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
            = new TournamentInput();

        public IActionResult OnPost()
        {
            string connStr =
            "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn =
                   new MySqlConnection(connStr))
            {
                conn.Open();

                string sql =
                @"INSERT INTO tournaments
                (
                    tournament_name,
                    start_date,
                    status,
                    bracket_size
                )
                VALUES
                (
                    @name,
                    @date,
                    @status,
                    @bracket_size
                )";

                using (MySqlCommand cmd =
                       new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@name",
                        Tournament.TournamentName
                    );

                    cmd.Parameters.AddWithValue(
                        "@date",
                        Tournament.StartDate
                    );

                    cmd.Parameters.AddWithValue(
                        "@status",
                        Tournament.Status
                    );

                    cmd.Parameters.AddWithValue(
                        "@bracket_size",
                        Tournament.BracketSize
                    );

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Tournament");
        }
    }


    public class TournamentInput
    {
        public int TournamentId { get; set; }

        public string TournamentName { get; set; } = "";

        public DateTime StartDate { get; set; }

        public string Status { get; set; } = "";

        public int BracketSize { get; set; }
    }
}