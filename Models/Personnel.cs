namespace GestionBibliotheque.Models
{
    public class Personnel
    {
        public int Id_personnel { get; set; }
        public string Poste { get; set; }
        public int Id_utilisateur { get; set; }

        public Utilisateur Utilisateur { get; set; }
    }
}
