using Microsoft.AspNetCore.Mvc;
using GestionBibliotheque.Models;

namespace GestionBibliotheque.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Connexion = ConnexionDB.TesterConnexion()
                ? "Connexion à la base de données réussie !"
                : "Erreur de connexion. Vérifiez que XAMPP est démarré.";

            return View();
        }
    }
}
