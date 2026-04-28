using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TournamentTracker.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT * FROM users
                    WHERE username = @username
                    AND password = @password";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", Username);
                    cmd.Parameters.AddWithValue("@password", Password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetString("Username", Username);
                            return RedirectToPage("/Index");
                        }
                    }
                }
            }

            ErrorMessage = "Invalid username or password.";
            return Page();
        }
    }
}