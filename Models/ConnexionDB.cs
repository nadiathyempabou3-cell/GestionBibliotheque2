using System;
using MySql.Data.MySqlClient;

namespace GestionBibliotheque.Models
{
    public class ConnexionDB
    {
        private static string ObtenirChaineConnexion()
        {
            string host = Environment.GetEnvironmentVariable("MYSQLHOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
            string database = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? "bibliotheque";
            string user = Environment.GetEnvironmentVariable("MYSQLUSER") ?? "root";
            string password = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? "";

            return $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};";
        }

        public static MySqlConnection ObtenirConnexion()
        {
            return new MySqlConnection(ObtenirChaineConnexion());
        }
    }
}
                 
