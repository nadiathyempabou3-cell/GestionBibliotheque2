using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class ReservationController : Controller
    {
        // ============================================
        // LISTE DES RESERVATIONS
        // ============================================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Reservation> reservations = new List<Reservation>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"SELECT r.*, 
                                    u.nom, u.prenom,
                                    l.titre
                                    FROM RESERVATION r
                                    INNER JOIN MEMBRE m ON r.id_membre = m.id_membre
                                    INNER JOIN UTILISATEUR u ON m.id_utilisateur = u.id_utilisateur
                                    INNER JOIN LIVRE l ON r.id_livre = l.id_livre
                                    ORDER BY r.date_reservation DESC";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        reservations.Add(new Reservation
                        {
                            Id_reservation = reader.GetInt32("id_reservation"),
                            Date_reservation = reader.GetDateTime("date_reservation"),
                            Statut_reservation = reader.GetString("statut_reservation"),
                            Id_membre = reader.GetInt32("id_membre"),
                            Id_livre = reader.GetInt32("id_livre"),
                            Membre = new Membre
                            {
                                Utilisateur = new Utilisateur
                                {
                                    Nom = reader.GetString("nom"),
                                    Prenom = reader.GetString("prenom")
                                }
                            },
                            Livre = new Livre { Titre = reader.GetString("titre") }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(reservations);
        }

        // ============================================
        // AFFICHER FORMULAIRE AJOUT
        // ============================================
        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Membres = ObtenirMembres();
            ViewBag.Livres = ObtenirLivres();
            return View();
        }

        // ============================================
        // TRAITER AJOUT
        // ============================================
        [HttpPost]
        public IActionResult Ajouter(int id_membre, int id_livre)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"INSERT INTO RESERVATION 
                                    (date_reservation, statut_reservation, id_membre, id_livre)
                                    VALUES (NOW(), 'en_attente', @idMembre, @idLivre)";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@idMembre", id_membre);
                    cmd.Parameters.AddWithValue("@idLivre", id_livre);
                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Reservation ajoutee avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                ViewBag.Membres = ObtenirMembres();
                ViewBag.Livres = ObtenirLivres();
                return View();
            }
        }

        // ============================================
        // CONFIRMER UNE RESERVATION
        // ============================================
        public IActionResult Confirmer(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    var cmd = new MySqlCommand(
                        "UPDATE RESERVATION SET statut_reservation = 'confirmée' WHERE id_reservation = @id",
                        connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                TempData["Succes"] = "Reservation confirmee !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // ============================================
        // ANNULER UNE RESERVATION
        // ============================================
        public IActionResult Annuler(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    var cmd = new MySqlCommand(
                        "UPDATE RESERVATION SET statut_reservation = 'annulée' WHERE id_reservation = @id",
                        connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                TempData["Succes"] = "Reservation annulee !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // ============================================
        // METHODES PRIVEES
        // ============================================
        private List<Membre> ObtenirMembres()
        {
            List<Membre> membres = new List<Membre>();
            using (var connexion = ConnexionDB.ObtenirConnexion())
            {
                string query = @"SELECT m.id_membre, u.nom, u.prenom 
                                FROM MEMBRE m 
                                INNER JOIN UTILISATEUR u ON m.id_utilisateur = u.id_utilisateur";
                var cmd = new MySqlCommand(query, connexion);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    membres.Add(new Membre
                    {
                        Id_membre = reader.GetInt32("id_membre"),
                        Utilisateur = new Utilisateur
                        {
                            Nom = reader.GetString("nom"),
                            Prenom = reader.GetString("prenom")
                        }
                    });
                }
            }
            return membres;
        }

        private List<Livre> ObtenirLivres()
        {
            List<Livre> livres = new List<Livre>();
            using (var connexion = ConnexionDB.ObtenirConnexion())
            {
                var cmd = new MySqlCommand("SELECT id_livre, titre FROM LIVRE", connexion);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    livres.Add(new Livre
                    {
                        Id_livre = reader.GetInt32("id_livre"),
                        Titre = reader.GetString("titre")
                    });
                }
            }
            return livres;
        }
    }
}