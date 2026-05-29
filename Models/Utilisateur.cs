namespace GestionBibliotheque.Models
{
    public class Utilisateur
    {
        public int Id_utilisateur { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Mot_de_passe { get; set; }
        public string Telephone { get; set; }
        public string Role { get; set; }
        public string Statut_compte { get; set; }
        public DateTime Date_creation { get; set; }
    }
}
