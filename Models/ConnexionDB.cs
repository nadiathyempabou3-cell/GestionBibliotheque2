using System;
using MySql.Data.MySqlClient;

namespace GestionBibliotheque.Models
{
    public class ConnexionDB
    {
        private static string ObtenirChaineConnexion()
        {
            string host = Environment.GetEnvironmentVariable("MYSQLHOST");
            string port = Environment.GetEnvironmentVariable("MYSQLPORT");
            string database = Environment.GetEnvironmentVariable("MYSQLDATABASE");
            string user = Environment.GetEnvironmentVariable("MYSQLUSER");
            string password = Environment.GetEnvironmentVariable("MYSQLPASSWORD");

            if (!string.IsNullOrEmpty(host))
            {
                return $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};AllowPublicKeyRetrieval=true;SslMode=None;";
            }

            return "Server=127.0.0.1;Port=3307;Database=bibliotheque;Uid=root;Pwd=;";
        }

        public static MySqlConnection ObtenirConnexion()
        {
            try
            {
                MySqlConnection connexion = new MySqlConnection(ObtenirChaineConnexion());
                connexion.Open();
                return connexion;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur de connexion : " + ex.Message);
            }
        }

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