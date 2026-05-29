using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class LivreController : Controller
    {
        // ============================================
        // LISTE DES LIVRES
        // ============================================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Livre> livres = new List<Livre>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"SELECT l.*, c.nom_categorie 
                                    FROM LIVRE l 
                                    INNER JOIN CATEGORIE c ON l.id_categorie = c.id_categorie
                                    ORDER BY l.titre";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        livres.Add(new Livre
                        {
                            Id_livre = reader.GetInt32("id_livre"),
                            Titre = reader.GetString("titre"),
                            Auteur = reader.GetString("auteur"),
                            Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? "" : reader.GetString("isbn"),
                            Annee_publication = reader.IsDBNull(reader.GetOrdinal("annee_publication")) ? 0 : reader.GetInt32("annee_publication"),
                            Quantite_totale = reader.GetInt32("quantite_totale"),
                            Quantite_disponible = reader.GetInt32("quantite_disponible"),
                            Etat = reader.GetString("etat"),
                            Id_categorie = reader.GetInt32("id_categorie"),
                            Categorie = new Categorie
                            {
                                Nom_categorie = reader.GetString("nom_categorie")
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(livres);
        }

        // ============================================
        // AFFICHER FORMULAIRE AJOUT
        // ============================================
        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Categories = ObtenirCategories();
            return View();
        }

        // ============================================
        // TRAITER AJOUT
        // ============================================
        [HttpPost]
        public IActionResult Ajouter(Livre livre)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"INSERT INTO LIVRE 
                                    (titre, auteur, isbn, annee_publication, description, 
                                     quantite_totale, quantite_disponible, etat, id_categorie)
                                    VALUES 
                                    (@titre, @auteur, @isbn, @annee, @description, 
                                     @qtotal, @qdispo, @etat, @categorie)";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@titre", livre.Titre);
                    cmd.Parameters.AddWithValue("@auteur", livre.Auteur);
                    cmd.Parameters.AddWithValue("@isbn", livre.Isbn);
                    cmd.Parameters.AddWithValue("@annee", livre.Annee_publication);
                    cmd.Parameters.AddWithValue("@description", livre.Description ?? "");
                    cmd.Parameters.AddWithValue("@qtotal", livre.Quantite_totale);
                    cmd.Parameters.AddWithValue("@qdispo", livre.Quantite_totale);
                    cmd.Parameters.AddWithValue("@etat", "disponible");
                    cmd.Parameters.AddWithValue("@categorie", livre.Id_categorie);

                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Livre ajoute avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                ViewBag.Categories = ObtenirCategories();
                return View(livre);
            }
        }

        // ============================================
        // AFFICHER FORMULAIRE MODIFICATION
        // ============================================
        public IActionResult Modifier(int id)
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            Livre livre = null;

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = "SELECT * FROM LIVRE WHERE id_livre = @id";
                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        livre = new Livre
                        {
                            Id_livre = reader.GetInt32("id_livre"),
                            Titre = reader.GetString("titre"),
                            Auteur = reader.GetString("auteur"),
                            Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? "" : reader.GetString("isbn"),
                            Annee_publication = reader.IsDBNull(reader.GetOrdinal("annee_publication")) ? 0 : reader.GetInt32("annee_publication"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                            Quantite_totale = reader.GetInt32("quantite_totale"),
                            Quantite_disponible = reader.GetInt32("quantite_disponible"),
                            Etat = reader.GetString("etat"),
                            Id_categorie = reader.GetInt32("id_categorie")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            ViewBag.Categories = ObtenirCategories();
            return View(livre);
        }

        // ============================================
        // TRAITER MODIFICATION
        // ============================================
        [HttpPost]
        public IActionResult Modifier(Livre livre)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = @"UPDATE LIVRE SET 
                                    titre = @titre, auteur = @auteur, isbn = @isbn,
                                    annee_publication = @annee, description = @description,
                                    quantite_totale = @qtotal, etat = @etat,
                                    id_categorie = @categorie
                                    WHERE id_livre = @id";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@titre", livre.Titre);
                    cmd.Parameters.AddWithValue("@auteur", livre.Auteur);
                    cmd.Parameters.AddWithValue("@isbn", livre.Isbn);
                    cmd.Parameters.AddWithValue("@annee", livre.Annee_publication);
                    cmd.Parameters.AddWithValue("@description", livre.Description ?? "");
                    cmd.Parameters.AddWithValue("@qtotal", livre.Quantite_totale);
                    cmd.Parameters.AddWithValue("@etat", livre.Etat);
                    cmd.Parameters.AddWithValue("@categorie", livre.Id_categorie);
                    cmd.Parameters.AddWithValue("@id", livre.Id_livre);

                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Livre modifie avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                ViewBag.Categories = ObtenirCategories();
                return View(livre);
            }
        }

        // ============================================
        // SUPPRIMER UN LIVRE
        // ============================================
        public IActionResult Supprimer(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    string query = "DELETE FROM LIVRE WHERE id_livre = @id";
                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Livre supprime avec succes !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // ============================================
        // METHODE PRIVEE : OBTENIR CATEGORIES
        // ============================================
        private List<Categorie> ObtenirCategories()
        {
            List<Categorie> categories = new List<Categorie>();

            using (var connexion = ConnexionDB.ObtenirConnexion())
            {
                var cmd = new MySqlCommand("SELECT * FROM CATEGORIE ORDER BY nom_categorie", connexion);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Categorie
                    {
                        Id_categorie = reader.GetInt32("id_categorie"),
                        Nom_categorie = reader.GetString("nom_categorie")
                    });
                }
            }

            return categories;
        }
    }
}
