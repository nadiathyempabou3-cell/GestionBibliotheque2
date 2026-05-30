using System;
using MySql.Data.MySqlClient;

namespace GestionBibliotheque.Models
{
    public class ConnexionDB
    {
        private static string ObtenirChaineConnexion()
        {
            return "Server=zephyr.proxy.rlwy.net;Port=50286;Database=railway;Uid=root;Pwd=PtBhOwaHFdRrvabWdvBNtGGChcgkMzFU;SslMode=Preferred;AllowPublicKeyRetrieval=true;";
        }

        public static MySqlConnection ObtenirConnexion()
        {
            return new MySqlConnection(ObtenirChaineConnexion());
        }
    }
}
