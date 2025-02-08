using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using MySql.Data.MySqlClient;
using Panaderia.Models;


namespace Panaderia.Controllers
{
    public class FormController : Controller
    {
        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(Form formulario)
        {
            if (ModelState.IsValid)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = "INSERT INTO Formulario (Nombre, Correo, Mensaje, Fecha_envio) VALUES (@Nombre, @Correo, @Mensaje, @FechaEnvio)";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Nombre", formulario.Nombre);
                            command.Parameters.AddWithValue("@Correo", formulario.Correo);
                            command.Parameters.AddWithValue("@Mensaje", formulario.Mensaje);
                            command.Parameters.AddWithValue("@FechaEnvio", DateTime.Now); 

                            command.ExecuteNonQuery();
                        }

                        return RedirectToAction("Crear");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Ocurrió un error al enviar el formulario: " + ex.Message);
                    }
                }
            }

            return View(formulario);
        }

    }

}