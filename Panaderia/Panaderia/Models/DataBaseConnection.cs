using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Panaderia.Models
{
    public class DataBaseConnection
    {

        private MySqlConnection connection;

        public DataBaseConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            connection = new MySqlConnection(connectionString);
        }

        public bool ProbarConexion()
        {
            try
            {
                connection.Open();
                Console.WriteLine("Conexión exitosa a la base de datos MySQL.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de conexión: " + ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}