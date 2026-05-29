using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;

namespace GestionBibliotheque.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Verifier si l'utilisateur est connecte
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    // Nombre total de livres
                    var cmdLivres = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT COUNT(*) FROM LIVRE", connexion);
                    ViewBag.TotalLivres = cmdLivres.ExecuteScalar();

                    // Nombre de membres
                    var cmdMembres = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT COUNT(*) FROM MEMBRE", connexion);
                    ViewBag.TotalMembres = cmdMembres.ExecuteScalar();

                    // Emprunts en cours
                    var cmdEmprunts = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT COUNT(*) FROM EMPRUNT WHERE statut_emprunt = 'en_cours'", connexion);
                    ViewBag.EmpruntsEnCours = cmdEmprunts.ExecuteScalar();

                    // Reservations en attente
                    var cmdReservations = new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT COUNT(*) FROM RESERVATION WHERE statut_reservation = 'en_attente'", connexion);
                    ViewBag.ReservationsEnAttente = cmdReservations.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            ViewBag.Nom = HttpContext.Session.GetString("UserNom");
            ViewBag.Prenom = HttpContext.Session.GetString("UserPrenom");
            ViewBag.Role = HttpContext.Session.GetString("UserRole");

            return View();
        }
    }
}
