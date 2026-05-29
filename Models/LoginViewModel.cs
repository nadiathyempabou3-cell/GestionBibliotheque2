using System.ComponentModel.DataAnnotations;

namespace GestionBibliotheque.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email est obligatoire")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        public string Mot_de_passe { get; set; }
    }
}
