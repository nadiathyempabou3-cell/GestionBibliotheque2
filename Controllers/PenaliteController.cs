using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class PenaliteController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Penalite> penalites = new List<Penalite>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"SELECT p.*, u.nom, u.prenom, l.titre, e.date_emprunt
                                    FROM penalite p
                                    INNER JOIN emprunt e ON p.id_emprunt = e.id_emprunt
                                    INNER JOIN membre m ON e.id_membre = m.id_membre
                                    INNER JOIN utilisateur u ON m.id_utilisateur = u.id_utilisateur
                                    INNER JOIN livre l ON e.id_livre = l.id_livre
                                    ORDER BY p.id_penalite DESC";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        penalites.Add(new Penalite
                        {
                            Id_penalite = reader.GetInt32("id_penalite"),
                            Montant = reader.GetDecimal("montant"),
                            Motif = reader.GetString("motif"),
                            Statut_paiement = reader.GetString("statut_paiement"),
                            Id_emprunt = reader.GetInt32("id_emprunt"),
                            Emprunt = new Emprunt
                            {
                                Date_emprunt = reader.GetDateTime("date_emprunt"),
                                Membre = new Membre
                                {
                                    Utilisateur = new Utilisateur
                                    {
                                        Nom = reader.GetString("nom"),
                                        Prenom = reader.GetString("prenom")
                                    }
                                },
                                Livre = new Livre { Titre = reader.GetString("titre") }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(penalites);
        }

        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Emprunts = ObtenirEmprunts();
            return View();
        }

        [HttpPost]
        public IActionResult Ajouter(int id_emprunt, decimal montant, string motif)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"INSERT INTO penalite (montant, motif, statut_paiement, id_emprunt)
                                    VALUES (@montant, @motif, 'non_payé', @idEmprunt)";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@montant", montant);
                    cmd.Parameters.AddWithValue("@motif", motif);
                    cmd.Parameters.AddWithValue("@idEmprunt", id_emprunt);
                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Penalite ajoutee avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                ViewBag.Emprunts = ObtenirEmprunts();
                return View();
            }
        }

        public IActionResult MarquerPaye(int id)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    var cmd = new MySqlCommand(
                        "UPDATE penalite SET statut_paiement = 'payé' WHERE id_penalite = @id",
                        connexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                TempData["Succes"] = "Penalite marquee comme payee !";
            }
            catch (Exception ex)
            {
                TempData["Erreur"] = "Erreur : " + ex.Message;
            }
            return RedirectToAction("Index");
        }

        private List<Emprunt> ObtenirEmprunts()
        {
            List<Emprunt> emprunts = new List<Emprunt>();
            using (var connexion = ConnexionDB.ObtenirConnexion())
            {
                connexion.Open();
                string query = @"SELECT e.id_emprunt, u.nom, u.prenom, l.titre
                                FROM emprunt e
                                INNER JOIN membre m ON e.id_membre = m.id_membre
                                INNER JOIN utilisateur u ON m.id_utilisateur = u.id_utilisateur
                                INNER JOIN livre l ON e.id_livre = l.id_livre
                                WHERE e.statut_emprunt = 'en_cours'";
                var cmd = new MySqlCommand(query, connexion);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    emprunts.Add(new Emprunt
                    {
                        Id_emprunt = reader.GetInt32("id_emprunt"),
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
            return emprunts;
        }
    }
}
