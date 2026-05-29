namespace GestionBibliotheque.Models
{
    public class Emprunt
    {
        public int Id_emprunt { get; set; }
        public DateTime Date_emprunt { get; set; }
        public DateTime Date_retour_prevue { get; set; }
        public DateTime? Date_retour_effective { get; set; }
        public string Statut_emprunt { get; set; }
        public int Id_membre { get; set; }
        public int Id_livre { get; set; }

        public Membre Membre { get; set; }
        public Livre Livre { get; set; }
    }
}
