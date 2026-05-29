namespace GestionBibliotheque.Models
{
    public class Livre
    {
        public int Id_livre { get; set; }
        public string Titre { get; set; }
        public string Auteur { get; set; }
        public string Isbn { get; set; }
        public int Annee_publication { get; set; }
        public string Description { get; set; }
        public int Quantite_totale { get; set; }
        public int Quantite_disponible { get; set; }
        public string Etat { get; set; }
        public int Id_categorie { get; set; }

        public Categorie Categorie { get; set; }
    }
}
