using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Panaderia.Models;
namespace Panaderia.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Producto> productos = new List<Producto>();

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT id_producto, nombre, precio FROM Productos LIMIT 6";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productos.Add(new Producto
                                {
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),
                                    Nombre = reader["nombre"].ToString(),
                                    Precio = Convert.ToDecimal(reader["precio"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al obtener los productos: " + ex.Message);
                }
            }

            return View(productos);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}