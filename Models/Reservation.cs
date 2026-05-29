namespace GestionBibliotheque.Models
{
    public class Reservation
    {
        public int Id_reservation { get; set; }
        public DateTime Date_reservation { get; set; }
        public string Statut_reservation { get; set; }
        public int Id_membre { get; set; }
        public int Id_livre { get; set; }

        public Membre Membre { get; set; }
        public Livre Livre { get; set; }
    }
}
