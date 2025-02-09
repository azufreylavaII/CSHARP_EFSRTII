using System;
using System.Configuration;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Panaderia.Models;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace Panaderia.Controllers
{
    public class UsuariosController : Controller
    {
        // Vista para el registro
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.contrasena);

                        // Verificar si el correo ya existe en la base de datos
                        string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE correo = @correo";
                        using (var checkCommand = new MySqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@correo", usuario.correo);
                            int existingUsers = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (existingUsers > 0)
                            {
                                ViewBag.Mensaje = "Este correo ya está registrado.";
                                return View(usuario);  // Retornar la vista con el mensaje
                            }
                        }

                        // Insertar el usuario en la base de datos
                        string query = "INSERT INTO Usuarios (nombre, correo, contrasena, direccion, telefono, rol, imagen_url, fecha_registro) VALUES (@nombre, @correo, @contrasena, @direccion, @telefono, @rol, @imagen_url, @fecha_registro)";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@nombre", usuario.nombre);
                            command.Parameters.AddWithValue("@correo", usuario.correo);
                            command.Parameters.AddWithValue("@contrasena", hashedPassword); // Usar la contraseña hasheada

                            command.Parameters.AddWithValue("@direccion", usuario.direccion);
                            command.Parameters.AddWithValue("@telefono", usuario.telefono);
                            command.Parameters.AddWithValue("@rol", usuario.rol);
                            command.Parameters.AddWithValue("@imagen_url", usuario.imagen_url ?? "default.png");
                            command.Parameters.AddWithValue("@fecha_registro", DateTime.Now);

                            int rowsAffected = command.ExecuteNonQuery();

                            // Verifica si la inserción fue exitosa
                            if (rowsAffected > 0)
                            {
                                ViewBag.Mensaje = "Usuario registrado correctamente.";
                            }
                            else
                            {
                                ViewBag.Mensaje = "Error al registrar el usuario.";
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al registrar usuario: " + ex.Message);
                        ViewBag.Mensaje = "Excepción en el proceso: " + ex.Message;
                    }
                }
            }
            else
            {
                ViewBag.Mensaje = "El modelo no es válido. Verifica los campos.";
            }

            return View(usuario);  // Regresamos la misma vista sin redirección
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string correo, string contrasena)
        {
            // Verificar si los campos no están vacíos
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Mensaje = "Por favor ingresa el correo y la contraseña.";
                return View();  // Regresamos la vista con el mensaje
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Buscar el usuario por correo
                    string query = "SELECT * FROM Usuarios WHERE correo = @correo";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@correo", correo);

                        var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // Verificar la contraseña con bcrypt
                            string storedPassword = reader["contrasena"].ToString();
                            if (BCrypt.Net.BCrypt.Verify(contrasena, storedPassword))
                            {
                                // Si la contraseña es correcta, guardar los datos en la sesión
                                Session["UsuarioID"] = reader["id_usuario"];
                                Session["UsuarioNombre"] = reader["nombre"];
                                Session["Rol"] = reader["rol"];

                                // Mensaje de éxito
                                ViewBag.Mensaje = "Login exitoso.";
                                return RedirectToAction("Dashboard");  // Redirigir al Dashboard
                            }
                            else
                            {
                                ViewBag.Mensaje = "Contraseña incorrecta.";
                            }
                        }
                        else
                        {
                            ViewBag.Mensaje = "Correo no encontrado.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al iniciar sesión: " + ex.Message);
                    ViewBag.Mensaje = "Error al iniciar sesión: " + ex.Message;
                }
            }

            return View();  // Regresar a la vista de login si ocurre algún error
        }


        // Dashboard del usuario
        public ActionResult Dashboard()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // Cerrar sesión
        public ActionResult Logout()
        {
            Session["UsuarioID"] = null;
            Session["UsuarioNombre"] = null;
            Session["Rol"] = null;
            return RedirectToAction("Login");
        }
    }
}
