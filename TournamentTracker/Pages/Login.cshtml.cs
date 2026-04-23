using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TournamentTracker.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet() { }

        public IActionResult OnPost()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];

            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM users WHERE username=@u AND password=@p";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return RedirectToPage("/Index");
                        }
                    }
                }
            }

            return Page();
        }
    }
}