namespace GestionBibliotheque.Models
{
    public class Abonnement
    {
        public int Id_abonnement { get; set; }
        public string Type_abonnement { get; set; }
        public DateTime Date_debut { get; set; }
        public DateTime Date_fin { get; set; }
        public decimal Montant { get; set; }
        public string Statut_paiement { get; set; }
        public int Id_membre { get; set; }

        public Membre Membre { get; set; }
    }
}
