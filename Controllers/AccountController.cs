using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using GestionBibliotheque.Models;

namespace GestionBibliotheque.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    string query = "SELECT id_utilisateur, nom, prenom, role FROM utilisateur " +
                                   "WHERE email = @email AND mot_de_passe = @mdp AND statut_compte = 'actif'";

                    MySqlCommand cmd = new MySqlCommand(query, connexion);
                    cmd.Parameters.AddWithValue("@email", model.Email);
                    cmd.Parameters.AddWithValue("@mdp", model.Mot_de_passe);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        HttpContext.Session.SetInt32("UserId", reader.GetInt32("id_utilisateur"));
                        HttpContext.Session.SetString("UserNom", reader.GetString("nom"));
                        HttpContext.Session.SetString("UserPrenom", reader.GetString("prenom"));
                        HttpContext.Session.SetString("UserRole", reader.GetString("role"));

                        string role = reader.GetString("role");
                        reader.Close();
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        reader.Close();
                        ViewBag.Erreur = "Email ou mot de passe incorrect.";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                return View(model);
            }
        }

        public IActionResult Inscription()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Inscription(string nom, string prenom, string email,
                                          string mot_de_passe, string telephone, string adresse)
        {
            try
            {
                using (var connexion = ConnexionDB.ObtenirConnexion())
                {
                    connexion.Open();
                    var cmdCheck = new MySqlCommand(
                        "SELECT COUNT(*) FROM utilisateur WHERE email = @email", connexion);
                    cmdCheck.Parameters.AddWithValue("@email", email);
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        ViewBag.Erreur = "Cet email est deja utilise.";
                        return View();
                    }

                    string queryUser = @"INSERT INTO utilisateur 
                                        (nom, prenom, email, mot_de_passe, telephone, role, statut_compte)
                                        VALUES (@nom, @prenom, @email, @mdp, @tel, 'membre', 'actif')";

                    var cmdUser = new MySqlCommand(queryUser, connexion);
                    cmdUser.Parameters.AddWithValue("@nom", nom);
                    cmdUser.Parameters.AddWithValue("@prenom", prenom);
                    cmdUser.Parameters.AddWithValue("@email", email);
                    cmdUser.Parameters.AddWithValue("@mdp", mot_de_passe);
                    cmdUser.Parameters.AddWithValue("@tel", telephone ?? "");
                    cmdUser.ExecuteNonQuery();

                    long newId = cmdUser.LastInsertedId;

                    string queryMembre = @"INSERT INTO membre 
                                          (adresse, date_inscription, id_utilisateur)
                                          VALUES (@adresse, CURDATE(), @idUser)";

                    var cmdMembre = new MySqlCommand(queryMembre, connexion);
                    cmdMembre.Parameters.AddWithValue("@adresse", adresse ?? "");
                    cmdMembre.Parameters.AddWithValue("@idUser", newId);
                    cmdMembre.ExecuteNonQuery();
                }

                TempData["Succes"] = "Compte cree avec succes ! Connectez-vous.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
