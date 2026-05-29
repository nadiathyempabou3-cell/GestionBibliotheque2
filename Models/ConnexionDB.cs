using System;
using MySql.Data.MySqlClient;

namespace GestionBibliotheque.Models
{
    public class ConnexionDB
    {
        private static string ObtenirChaineConnexion()
        {
            string mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL");
            if (!string.IsNullOrEmpty(mysqlUrl))
            {
                try
                {
                    Uri uri = new Uri(mysqlUrl);
                    string host = uri.Host;
                    int port = uri.Port > 0 ? uri.Port : 3306;
                    string database = uri.AbsolutePath.TrimStart('/');
                    string userInfo = uri.UserInfo;
                    string user = userInfo.Split(':')[0];
                    string password = userInfo.Split(':').Length > 1 ? userInfo.Split(':')[1] : "";
                    return $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};SslMode=None;AllowPublicKeyRetrieval=true;";
                }
                catch { }
            }
            string h = Environment.GetEnvironmentVariable("MYSQLHOST") ?? "127.0.0.1";
            string p = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3307";
            string db = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? "bibliotheque";
            string u = Environment.GetEnvironmentVariable("MYSQLUSER") ?? "root";
            string pwd = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? "";
            return $"Server={h};Port={p};Database={db};Uid={u};Pwd={pwd};SslMode=None;AllowPublicKeyRetrieval=true;";
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
                 
