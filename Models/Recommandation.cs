namespace GestionBibliotheque.Models
{
    public class Recommandation
    {
        public int Id_recommandation { get; set; }
        public DateTime Date_recommandation { get; set; }
        public int Id_membre { get; set; }
        public int Id_livre { get; set; }

        public Membre Membre { get; set; }
        public Livre Livre { get; set; }
    }
}
