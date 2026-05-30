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
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();

                    var cmdLivres = new MySqlCommand(
                        "SELECT COUNT(*) FROM livre", connexion);
                    ViewBag.TotalLivres = cmdLivres.ExecuteScalar();

                    var cmdMembres = new MySqlCommand(
                        "SELECT COUNT(*) FROM membre", connexion);
                    ViewBag.TotalMembres = cmdMembres.ExecuteScalar();

                    var cmdEmprunts = new MySqlCommand(
                        "SELECT COUNT(*) FROM emprunt WHERE statut_emprunt = 'en_cours'", connexion);
                    ViewBag.EmpruntsEnCours = cmdEmprunts.ExecuteScalar();

                    var cmdReservations = new MySqlCommand(
                        "SELECT COUNT(*) FROM reservation WHERE statut_reservation = 'en_attente'", connexion);
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
