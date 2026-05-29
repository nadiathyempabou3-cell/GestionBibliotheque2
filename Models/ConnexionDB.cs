using System;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace GestionBibliotheque.Models
{
    public class ConnexionDB
    {
        private static string chaineConnexion =
            "Server=127.0.0.1;Port=3307;Database=bibliotheque;Uid=root;Pwd=;";

        // ============================================
        // OBTENIR UNE CONNEXION
        // ============================================
        public static MySqlConnection ObtenirConnexion()
        {
            try
            {
                MySqlConnection connexion = new MySqlConnection(chaineConnexion);
                connexion.Open();
                return connexion;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur de connexion : " + ex.Message);
            }
        }

        // ============================================
        // TESTER LA CONNEXION
        // ============================================
        public static bool TesterConnexion()
        {
            try
            {
                using (MySqlConnection connexion = ObtenirConnexion())
                {
                    return connexion.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
