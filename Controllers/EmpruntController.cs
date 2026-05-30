using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class EmpruntController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Emprunt> emprunts = new List<Emprunt>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"SELECT e.*, u.nom, u.prenom, l.titre
                                    FROM emprunt e
                                    INNER JOIN membre m ON e.id_membre = m.id_membre
                                    INNER JOIN utilisateur u ON m.id_utilisateur = u.id_utilisateur
                                    INNER JOIN livre l ON e.id_livre = l.id_livre
                                    ORDER BY e.date_emprunt DESC";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        emprunts.Add(new Emprunt
                        {
                            Id_emprunt = reader.GetInt32("id_emprunt"),
                            Date_emprunt = reader.GetDateTime("date_emprunt"),
                            Date_retour_prevue = reader.GetDateTime("date_retour_prevue"),
                            Date_retour_effective = reader.IsDBNull(reader.GetOrdinal("date_retour_effective")) ? (DateTime?)null : reader.GetDateTime("date_retour_effective"),
                            Statut_emprunt = reader.GetString("statut_emprunt"),
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
                            Livre = new Livre
                            {
                                Titre = reader.GetString("titre")
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(emprunts);
        }

        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Membres = ObtenirMembres();
            ViewBag.Livres = ObtenirLivres();
            return View();
        }

        [HttpPost]
        public IActionResult Ajouter(int id_membre, int id_livre, DateTime date_retour_prevue)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"INSERT INTO emprunt 
                                    (date_emprunt, date_retour_prevue, statut_emprunt, id_membre, id_livre)
                                    VALUES (CURDATE(), @dateRetour, 'en_cours', @idMembre, @idLivre)";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@dateRetour", date_retour_prevue);
                    cmd.Parameters.AddWithValue("@idMembre", id_membre);
                    cmd.Parameters.AddWithValue("@idLivre", id_livre);
                    cmd.ExecuteNonQuery();

                    var cmdUpdate = new MySqlCommand(
                        "UPDATE livre SET quantite_disponible = quantite_disponible - 1 WHERE id_livre = @id",
                        connexion);
                    cmdUpdate.Parameters.AddWithValue("@id", id_livre);
                    cmdUpdate.ExecuteNonQuery();
                }

                TempData["Succes"] = "Emprunt enregistre avec succes !";
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

        public IActionResult Retourner(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    var cmdGet = new MySqlCommand(
                        "SELECT id_livre FROM emprunt WHERE id_emprunt = @id", connexion);
                    cmdGet.Parameters.AddWithValue("@id", id);
                    int idLivre = Convert.ToInt32(cmdGet.ExecuteScalar());

                    var cmdEmprunt = new MySqlCommand(
                        @"UPDATE emprunt SET 
                          date_retour_effective = CURDATE(), 
                          statut_emprunt = 'retourné'
                          WHERE id_emprunt = @id", connexion);
                    cmdEmprunt.Parameters.AddWithValue("@id", id);
                    cmdEmprunt.ExecuteNonQuery();

                    var cmdLivre = new MySqlCommand(
                        "UPDATE livre SET quantite_disponible = quantite_disponible + 1 WHERE id_livre = @id",
                        connexion);
                    cmdLivre.Parameters.AddWithValue("@id", idLivre);
                    cmdLivre.ExecuteNonQuery();
                }

                TempData["Succes"] = "Livre retourne avec succes !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        private List<Membre> ObtenirMembres()
        {
            List<Membre> membres = new List<Membre>();
            using (var connexion = ConnexionDB.ObtenirConnexion())
            {
                connexion.Open();
                string query = @"SELECT m.id_membre, u.nom, u.prenom 
                                FROM membre m 
                                INNER JOIN utilisateur u ON m.id_utilisateur = u.id_utilisateur";
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
                connexion.Open();
                string query = "SELECT id_livre, titre FROM livre WHERE quantite_disponible > 0";
                var cmd = new MySqlCommand(query, connexion);
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
