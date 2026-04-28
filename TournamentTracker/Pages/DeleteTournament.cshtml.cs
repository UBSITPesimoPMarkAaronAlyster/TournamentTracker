using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace TournamentTracker.Pages
{
    public class DeleteTournamentModel : PageModel
    {
        public IActionResult OnGet(int id)
        {
            string connStr = "server=localhost;port=1108;database=TournamentTracker_db;user=root;password=;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string deleteMatchesSql = "DELETE FROM matches WHERE tournament_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(deleteMatchesSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                string deleteTeamsSql = "DELETE FROM teams WHERE tournament_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(deleteTeamsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                string deleteTournamentSql = "DELETE FROM tournaments WHERE tournament_id = @id";

                using (MySqlCommand cmd = new MySqlCommand(deleteTournamentSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Tournament");
        }
    }
}