using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using Panaderia.Models;
namespace Panaderia.Controllers
{
    public class ListasController : Controller
    {
        private string connectionString = "Server=autorack.proxy.rlwy.net;Port=33770;Database=railway;User Id=root;Password=vLBpfgcfnFSkRxufujCBkvAhGmLKCtkw;";

        // Listar productos
        public ActionResult ListarProductos()
        {
            List<Producto> productos = new List<Producto>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_producto, nombre, descripcion, precio, stock FROM Productos;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    productos.Add(new Producto
                    {
                        IdProducto = reader.GetInt32("id_producto"),
                        Nombre = reader.GetString("nombre"),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString("descripcion"),
                        Precio = reader.IsDBNull(reader.GetOrdinal("precio")) ? 0 : reader.GetDecimal("precio"),
                        Stock = reader.IsDBNull(reader.GetOrdinal("stock")) ? 0 : reader.GetInt32("stock")
                    });
                }
            }

            return View(productos);
        }

        // Crear producto
        public ActionResult CrearProducto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Productos (id_categoria, Nombre, Descripcion, Precio, Stock) VALUES (@IdCategoria, @Nombre, @Descripcion, @Precio, @Stock)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria == 0 ? (object)DBNull.Value : producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? "");
                    cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@Stock", producto.Stock);

                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("ListarProductos");
            }
            return View(producto);
        }


        // Editar producto
        public ActionResult EditarProducto(int id)
        {
            Producto producto = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_producto, id_categoria, nombre, descripcion, precio, stock FROM Productos WHERE id_producto = @id_producto";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_producto", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    producto = new Producto
                    {
                        IdProducto = reader.GetInt32("id_producto"),
                        IdCategoria = reader.IsDBNull(reader.GetOrdinal("id_categoria")) ? 0 : reader.GetInt32("id_categoria"),
                        Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? "" : reader.GetString("nombre"),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString("descripcion"),
                        Precio = reader.IsDBNull(reader.GetOrdinal("precio")) ? 0 : reader.GetDecimal("precio"),
                        Stock = reader.IsDBNull(reader.GetOrdinal("stock")) ? 0 : reader.GetInt32("stock")
                    };
                }
            }

            if (producto == null)
                return HttpNotFound();

            return View(producto);
        }

        [HttpPost]
        public ActionResult EditarProducto(Producto producto)
        {
            if (ModelState.IsValid)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Productos SET id_categoria = @id_categoria, nombre = @nombre, descripcion = @descripcion, precio = @precio, stock = @stock WHERE id_producto = @id_producto";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id_producto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@id_categoria", producto.IdCategoria == 0 ? (object)DBNull.Value : producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(producto.Nombre) ? (object)DBNull.Value : producto.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(producto.Descripcion) ? (object)DBNull.Value : producto.Descripcion);
                    cmd.Parameters.AddWithValue("@precio", producto.Precio <= 0 ? (object)DBNull.Value : producto.Precio);
                    cmd.Parameters.AddWithValue("@stock", producto.Stock <= 0 ? (object)DBNull.Value : producto.Stock);

                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el producto.");
                        return View(producto);
                    }
                }

                return RedirectToAction("ListarProductos");
            }

            return View(producto);
        }

        // Eliminar producto
        public ActionResult EliminarProducto(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Verificar si el producto existe antes de eliminarlo
                string queryCheck = "SELECT COUNT(*) FROM Productos WHERE id_producto = @id_producto";
                using (MySqlCommand cmdCheck = new MySqlCommand(queryCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id_producto", id);
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count == 0)
                    {
                        ModelState.AddModelError("", "El producto no existe o ya ha sido eliminado.");
                        return RedirectToAction("ListarProductos"); // O podrías redirigir a una página de error
                    }
                }

                // Ejecutar la eliminación
                string query = "DELETE FROM Productos WHERE id_producto = @id_producto";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id_producto", id);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        ModelState.AddModelError("", "No se pudo eliminar el producto.");
                        return RedirectToAction("ListarProductos");
                    }
                }
            }

            return RedirectToAction("ListarProductos");
        }



        //Listar usuarios
        public ActionResult ListarUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_usuario, Nombre, Correo, direccion, telefono, Rol, fecha_registro FROM Usuarios;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        id_usuario = reader.GetInt32("id_Usuario"),
                        nombre = reader.GetString("Nombre"),
                        correo = reader.GetString("Correo"),
                        direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? "" : reader.GetString("direccion"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                        rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? "Sin rol" : reader.GetString("Rol"),
                        fecha_registro = reader.IsDBNull(reader.GetOrdinal("fecha_registro")) ? DateTime.MinValue : reader.GetDateTime("fecha_registro")
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

                    if (usuario.fecha_registro == default(DateTime))
                    {
                        usuario.fecha_registro = DateTime.Now;
                    }

                    string query = "INSERT INTO Usuarios (Nombre, Correo, contrasena, Direccion, Telefono, Rol, fecha_registro) " +
                                   "VALUES (@Nombre, @Correo, @contrasena, @Direccion, @Telefono, @Rol, @FechaRegistro)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@contrasena", string.IsNullOrEmpty(usuario.contrasena) ? "123456" : usuario.contrasena);
                    cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrEmpty(usuario.direccion) ? DBNull.Value : (object)usuario.direccion);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(usuario.telefono) ? DBNull.Value : (object)usuario.telefono);
                    cmd.Parameters.AddWithValue("@Rol", string.IsNullOrEmpty(usuario.rol) ? "Sin rol" : usuario.rol);
                    cmd.Parameters.AddWithValue("@FechaRegistro", usuario.fecha_registro);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("ListarUsuarios");
            }
            return View(usuario);
        }


        // Editar usuario














        // Eliminar usuario
        public ActionResult EliminarUsuario(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();


                string queryCheck = "SELECT COUNT(*) FROM Usuarios WHERE id_usuario = @id_usuario";
                using (MySqlCommand cmdCheck = new MySqlCommand(queryCheck, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@id_usuario", id);
                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count == 0)
                    {

                        ModelState.AddModelError("", "El usuario no existe o ya ha sido eliminado.");
                        return RedirectToAction("ListarUsuarios");
                    }
                }


                string query = "DELETE FROM Usuarios WHERE id_usuario = @id_usuario";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {

                        ModelState.AddModelError("", "No se pudo eliminar el usuario.");
                        return RedirectToAction("ListarUsuarios");
                    }
                }
            }


            return RedirectToAction("ListarUsuarios");
        }


        public ActionResult EditarUsuario(int id)
        {
            Usuario usuario = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_usuario, Nombre, Correo, contrasena, direccion, telefono, rol FROM Usuarios WHERE id_usuario = @id_usuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_usuario", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = reader.GetInt32("id_usuario"),
                        nombre = reader.GetString("Nombre"),
                        correo = reader.GetString("Correo"),
                        contrasena = reader.GetString("contrasena"),
                        direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? "" : reader.GetString("direccion"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? "" : reader.GetString("telefono"),
                        rol = reader.IsDBNull(reader.GetOrdinal("rol")) ? "Cliente" : reader.GetString("rol")
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
            // Depuración: Verificar si el modelo es válido
            Console.WriteLine("Validando el modelo...");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("El modelo no es válido. Errores de validación:");
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(usuario);
            }

            // Depuración: Verificar los datos recibidos
            Console.WriteLine("Datos recibidos del formulario:");
            Console.WriteLine($"ID Usuario: {usuario.id_usuario}");
            Console.WriteLine($"Nombre: {usuario.nombre}");
            Console.WriteLine($"Correo: {usuario.correo}");
            Console.WriteLine($"Contraseña: {usuario.contrasena}");
            Console.WriteLine($"Dirección: {usuario.direccion}");
            Console.WriteLine($"Teléfono: {usuario.telefono}");
            Console.WriteLine($"Rol: {usuario.rol}");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    // Depuración: Intentando abrir la conexión
                    Console.WriteLine("Abriendo conexión a la base de datos...");
                    conn.Open();

                    // Construir la consulta SQL
                    string query = "UPDATE Usuarios SET Nombre = @Nombre, Correo = @Correo, Direccion = @Direccion, Telefono = @Telefono, Rol = @Rol";

                    if (!string.IsNullOrEmpty(usuario.contrasena))
                    {
                        query += ", contrasena = @Contrasena";
                    }

                    query += " WHERE id_usuario = @IdUsuario";

                    // Depuración: Mostrar la consulta SQL generada
                    Console.WriteLine("Consulta SQL generada:");
                    Console.WriteLine(query);

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Agregar parámetros
                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.id_usuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@Direccion", string.IsNullOrEmpty(usuario.direccion) ? DBNull.Value : (object)usuario.direccion);
                    cmd.Parameters.AddWithValue("@Telefono", string.IsNullOrEmpty(usuario.telefono) ? DBNull.Value : (object)usuario.telefono);
                    cmd.Parameters.AddWithValue("@Rol", string.IsNullOrEmpty(usuario.rol) ? "Cliente" : usuario.rol);

                    if (!string.IsNullOrEmpty(usuario.contrasena))
                    {
                        cmd.Parameters.AddWithValue("@Contrasena", usuario.contrasena);
                    }

                    // Depuración: Mostrar los valores de los parámetros
                    Console.WriteLine("Valores de los parámetros:");
                    foreach (MySqlParameter param in cmd.Parameters)
                    {
                        Console.WriteLine($"{param.ParameterName}: {param.Value}");
                    }

                    // Ejecutar la consulta
                    Console.WriteLine("Ejecutando la consulta...");
                    int affectedRows = cmd.ExecuteNonQuery();

                    // Depuración: Verificar filas afectadas
                    Console.WriteLine($"Filas afectadas: {affectedRows}");

                    if (affectedRows == 0)
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el usuario.");
                        Console.WriteLine("No se pudo actualizar el usuario.");
                        return View(usuario);
                    }
                }

                // Depuración: Redireccionar después de la actualización
                Console.WriteLine("Usuario actualizado correctamente. Redirigiendo a ListarUsuarios...");
                return RedirectToAction("ListarUsuarios");
            }
            catch (Exception ex)
            {
                // Depuración: Capturar y mostrar excepciones
                Console.WriteLine("Error durante la actualización:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                ModelState.AddModelError("", "Error al actualizar el usuario: " + ex.Message);
                return View(usuario);
            }
        }

    }
}