using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class CategorieController : Controller
    {
        // ============================================
        // LISTE DES CATEGORIES
        // ============================================
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Categorie> categories = new List<Categorie>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    var cmd = new MySqlCommand(
                        "SELECT * FROM CATEGORIE ORDER BY nom_categorie", connexion);
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
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(categories);
        }

        // ============================================
        // AJOUTER
        // ============================================
        [HttpPost]
        public IActionResult Ajouter(string nom_categorie)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    var cmd = new MySqlCommand(
                        "INSERT INTO CATEGORIE (nom_categorie) VALUES (@nom)", connexion);
                    cmd.Parameters.AddWithValue("@nom", nom_categorie);
                    cmd.ExecuteNonQuery();
                }
                TempData["Succes"] = "Categorie ajoutee avec succes !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        // ============================================
        // SUPPRIMER
        // ============================================
        public IActionResult Supprimer(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    var cmd = new MySqlCommand(
                        "DELETE FROM CATEGORIE WHERE id_categorie = @id", connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                TempData["Succes"] = "Categorie supprimee !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Impossible de supprimer : " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}