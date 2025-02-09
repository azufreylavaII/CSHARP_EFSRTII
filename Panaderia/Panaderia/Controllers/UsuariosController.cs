using System;
using System.Configuration;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Panaderia.Models;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using System.Collections.Generic;
using System.Linq;

namespace Panaderia.Controllers
{
    public class UsuariosController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

       

        // Listar usuarios
        public ActionResult ListarUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_usuario, nombre, correo, telefono FROM Usuarios;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        id_usuario = reader.GetInt32("id_usuario"),
                        nombre = reader.GetString("nombre"),
                        correo = reader.GetString("correo"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                    });
                }
            }

            return View(usuarios);
        }

        // Crear usuario
        public ActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Usuarios (nombre, correo, contrasena, direccion, telefono, rol) VALUES (@Nombre, @Correo, @Contraseña,  @Direccion,  @Telefono, @Rol)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
               
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@Direccion", usuario.direccion);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono);
                    cmd.Parameters.AddWithValue("@Rol", usuario.rol);

                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("ListarUsuarios");
            }
            return View(usuario);
        }

        // Editar usuario
        public ActionResult EditarUsuario(int id)
        {
            Usuario usuario = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_usuario, nombre, correo, direccion, telefono FROM Usuarios WHERE id_usuario = @IdUsuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = reader.GetInt32("id_usuario"),
                        nombre = reader.GetString("nombre"),
                        correo = reader.GetString("correo"),
                        direccion = reader.GetString("direccion"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                  
                    };
                }
            }

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        [HttpPost]
        public ActionResult EditarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Usuarios SET nombre = @Nombre, correo = @Correo, direccion = @Direccion, telefono = @Telefono, rol = @Rol WHERE id_usuario = @IdUsuario";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.id_usuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@Direccion", usuario.direccion);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono);
                    cmd.Parameters.AddWithValue("@Rol", usuario.rol);

                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("ListarUsuarios");
            }
            return View(usuario);
        }

        // Eliminar usuario
        public ActionResult EliminarUsuario(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Usuarios WHERE id_usuario = @IdUsuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ListarUsuarios");
        }
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
                //string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

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
                                string rol = reader["rol"].ToString();
                                Console.WriteLine("Rol guardado en la sesión: " + rol);
                                // Si la contraseña es correcta, guardar los datos en la sesión
                                Session["UsuarioID"] = reader["id_usuario"];
                                Session["UsuarioNombre"] = reader["nombre"];
                                Session["Rol"] = reader["rol"];

                                // Mensaje de éxito
                                ViewBag.Mensaje = "Login exitoso.";
                                //return RedirectToAction("ListarProductos");  // Redirigir al Dashboard
                                System.Diagnostics.Debug.WriteLine("Rol guardado en sesión: " + Session["Rol"]);

                                return RedirectToAction("ListarProductos", "Productos");


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


        // Mostrar los datos del usuario actual
        public ActionResult MiPerfil()
        {
            // Verificar si el usuario está autenticado
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login");
            }

            int usuarioId = (int)Session["UsuarioID"];
            List<Usuario> usuarios = new List<Usuario>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_usuario, nombre, correo, direccion, telefono, rol FROM Usuarios WHERE id_usuario = @IdUsuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        id_usuario = reader.GetInt32("id_usuario"),
                        nombre = reader.GetString("nombre"),
                        correo = reader.GetString("correo"),
                        direccion = reader.GetString("direccion"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                       
                    });
                }
            }

            

            return View(usuarios);
        }
    }
}
