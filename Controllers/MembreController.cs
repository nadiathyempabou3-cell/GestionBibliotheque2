using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class MembreController : Controller
    {
        // ============================================
        // LISTE DES MEMBRES
        // ============================================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Membre> membres = new List<Membre>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"SELECT m.*, u.nom, u.prenom, u.email, u.telephone, u.statut_compte
                                    FROM MEMBRE m
                                    INNER JOIN UTILISATEUR u ON m.id_utilisateur = u.id_utilisateur
                                    ORDER BY u.nom";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        membres.Add(new Membre
                        {
                            Id_membre = reader.GetInt32("id_membre"),
                            Adresse = reader.IsDBNull(reader.GetOrdinal("adresse")) ? "" : reader.GetString("adresse"),
                            Date_inscription = reader.GetDateTime("date_inscription"),
                            Id_utilisateur = reader.GetInt32("id_utilisateur"),
                            Utilisateur = new Utilisateur
                            {
                                Nom = reader.GetString("nom"),
                                Prenom = reader.GetString("prenom"),
                                Email = reader.GetString("email"),
                                Telephone = reader.IsDBNull(reader.GetOrdinal("telephone")) ? "" : reader.GetString("telephone"),
                                Statut_compte = reader.GetString("statut_compte")
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(membres);
        }

        // ============================================
        // AFFICHER FORMULAIRE AJOUT
        // ============================================
        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // ============================================
        // TRAITER AJOUT
        // ============================================
        [HttpPost]
        public IActionResult Ajouter(Utilisateur utilisateur, string adresse)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    // Creer l'utilisateur
                    string queryUser = @"INSERT INTO UTILISATEUR 
                                        (nom, prenom, email, mot_de_passe, telephone, role, statut_compte)
                                        VALUES (@nom, @prenom, @email, @mdp, @tel, 'membre', 'actif')";

                    var cmdUser = new MySqlCommand(queryUser, connexion);
                    cmdUser.Parameters.AddWithValue("@nom", utilisateur.Nom);
                    cmdUser.Parameters.AddWithValue("@prenom", utilisateur.Prenom);
                    cmdUser.Parameters.AddWithValue("@email", utilisateur.Email);
                    cmdUser.Parameters.AddWithValue("@mdp", utilisateur.Mot_de_passe);
                    cmdUser.Parameters.AddWithValue("@tel", utilisateur.Telephone ?? "");
                    cmdUser.ExecuteNonQuery();

                    // Recuperer l'id du nouvel utilisateur
                    long newId = cmdUser.LastInsertedId;

                    // Creer le membre
                    string queryMembre = @"INSERT INTO MEMBRE (adresse, date_inscription, id_utilisateur)
                                          VALUES (@adresse, CURDATE(), @idUser)";

                    var cmdMembre = new MySqlCommand(queryMembre, connexion);
                    cmdMembre.Parameters.AddWithValue("@adresse", adresse ?? "");
                    cmdMembre.Parameters.AddWithValue("@idUser", newId);
                    cmdMembre.ExecuteNonQuery();
                }

                TempData["Succes"] = "Membre ajoute avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                return View();
            }
        }

        // ============================================
        // SUPPRIMER UN MEMBRE
        // ============================================
        public IActionResult Supprimer(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    // Recuperer id_utilisateur
                    var cmdGet = new MySqlCommand(
                        "SELECT id_utilisateur FROM MEMBRE WHERE id_membre = @id", connexion);
                    cmdGet.Parameters.AddWithValue("@id", id);
                    int idUser = Convert.ToInt32(cmdGet.ExecuteScalar());

                    // Supprimer le membre
                    var cmdMembre = new MySqlCommand(
                        "DELETE FROM MEMBRE WHERE id_membre = @id", connexion);
                    cmdMembre.Parameters.AddWithValue("@id", id);
                    cmdMembre.ExecuteNonQuery();

                    // Supprimer l'utilisateur
                    var cmdUser = new MySqlCommand(
                        "DELETE FROM UTILISATEUR WHERE id_utilisateur = @id", connexion);
                    cmdUser.Parameters.AddWithValue("@id", idUser);
                    cmdUser.ExecuteNonQuery();
                }

                TempData["Succes"] = "Membre supprime avec succes !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
