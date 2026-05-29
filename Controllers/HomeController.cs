using Microsoft.AspNetCore.Mvc;
using GestionBibliotheque.Models;

namespace GestionBibliotheque.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
