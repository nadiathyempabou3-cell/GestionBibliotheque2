using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;
using System.Collections.Generic;

namespace GestionBibliotheque.Controllers
{
    public class AbonnementController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            List<Abonnement> abonnements = new List<Abonnement>();

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"SELECT a.*, u.nom, u.prenom
                                    FROM abonnement a
                                    INNER JOIN membre m ON a.id_membre = m.id_membre
                                    INNER JOIN utilisateur u ON m.id_utilisateur = u.id_utilisateur
                                    ORDER BY a.date_debut DESC";

                    var cmd = new MySqlCommand(query, connexion);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        abonnements.Add(new Abonnement
                        {
                            Id_abonnement = reader.GetInt32("id_abonnement"),
                            Type_abonnement = reader.GetString("type_abonnement"),
                            Date_debut = reader.GetDateTime("date_debut"),
                            Date_fin = reader.GetDateTime("date_fin"),
                            Montant = reader.GetDecimal("montant"),
                            Statut_paiement = reader.GetString("statut_paiement"),
                            Id_membre = reader.GetInt32("id_membre"),
                            Membre = new Membre
                            {
                                Utilisateur = new Utilisateur
                                {
                                    Nom = reader.GetString("nom"),
                                    Prenom = reader.GetString("prenom")
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
            }

            return View(abonnements);
        }

        public IActionResult Ajouter()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Membres = ObtenirMembres();
            return View();
        }

        [HttpPost]
        public IActionResult Ajouter(int id_membre, string type_abonnement,
                                     DateTime date_debut, decimal montant)
        {
            try
            {
                DateTime date_fin;
                if (type_abonnement == "mensuel")
                    date_fin = date_debut.AddMonths(1);
                else if (type_abonnement == "trimestriel")
                    date_fin = date_debut.AddMonths(3);
                else
                    date_fin = date_debut.AddYears(1);

                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = @"INSERT INTO abonnement 
                                    (type_abonnement, date_debut, date_fin, montant, 
                                     statut_paiement, id_membre)
                                    VALUES (@type, @debut, @fin, @montant, 'payé', @idMembre)";

                    var cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@type", type_abonnement);
                    cmd.Parameters.AddWithValue("@debut", date_debut);
                    cmd.Parameters.AddWithValue("@fin", date_fin);
                    cmd.Parameters.AddWithValue("@montant", montant);
                    cmd.Parameters.AddWithValue("@idMembre", id_membre);
                    cmd.ExecuteNonQuery();
                }

                TempData["Succes"] = "Abonnement ajoute avec succes !";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                ViewBag.Membres = ObtenirMembres();
                return View();
            }
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
    }
}
