namespace GestionBibliotheque.Models
{
    public class Membre
    {
        public int Id_membre { get; set; }
        public string Adresse { get; set; }
        public DateTime Date_inscription { get; set; }
        public int Id_utilisateur { get; set; }

        // Pour afficher les infos de l'utilisateur lié
        public Utilisateur Utilisateur { get; set; }
    }
}
