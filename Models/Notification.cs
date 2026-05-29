namespace GestionBibliotheque.Models
{
    public class Notification
    {
        public int Id_notification { get; set; }
        public string Message { get; set; }
        public string Type_notification { get; set; }
        public DateTime Date_notification { get; set; }
        public string Statut_lecture { get; set; }
        public int Id_utilisateur { get; set; }
        public int? Id_livre { get; set; }

        public Utilisateur Utilisateur { get; set; }
        public Livre Livre { get; set; }
    }
}
