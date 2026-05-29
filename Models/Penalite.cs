namespace GestionBibliotheque.Models
{
    public class Penalite
    {
        public int Id_penalite { get; set; }
        public decimal Montant { get; set; }
        public string Motif { get; set; }
        public string Statut_paiement { get; set; }
        public int Id_emprunt { get; set; }

        public Emprunt Emprunt { get; set; }
    }
}
