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
                string query = "SELECT id_Producto, nombre, descripcion, precio, stock FROM Productos;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    productos.Add(new Producto
                    {
                        IdProducto = reader.GetInt32("id_Producto"),
                        Nombre = reader.GetString("nombre"),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString("descripcion"),
                        Precio = reader.GetDecimal("precio"),
                        Stock = reader.GetInt32("stock"),
                    
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
                    string query = "INSERT INTO Productos (IdCategoria, Nombre, Descripcion, Precio, Stock) VALUES (@IdCategoria, @Nombre, @Descripcion, @Precio, @Stock)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
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
                string query = "SELECT id_Producto, nombre, descripcion, precio, stock FROM Productos WHERE id_Producto = @IdProducto";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdProducto", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    producto = new Producto
                    {
                        IdProducto = reader.GetInt32("id_Producto"),
              
                        Nombre = reader.GetString("nombre"),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? "" : reader.GetString("descripcion"),
                        Precio = reader.GetDecimal("precio"),
                        Stock = reader.GetInt32("stock"),
                      
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
                    string query = "UPDATE Productos SET nombre = @Nombre, descripcion = @Descripcion, precio = @Precio, stock = @Stock WHERE id_Producto = @IdProducto";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
                    cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                    cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                    cmd.Parameters.AddWithValue("@Stock", producto.Stock);
                 
                    cmd.ExecuteNonQuery();
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
                string query = "DELETE FROM Productos WHERE id_producto = @IdProducto";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdProducto", id);
                cmd.ExecuteNonQuery();
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
                string query = "SELECT id_usuario, Nombre, Correo, direccion, telefono, Rol FROM Usuarios;";
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
                    string query = "INSERT INTO usuarios (Nombre, Correo, Contraseña, Direccion, Telefono, Rol, FechaRegistro) VALUES (@Nombre, @Correo, @Contraseña, @Direccion, @Telefono, @Rol, @FechaRegistro)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@Direccion", usuario.direccion ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Rol", usuario.rol ?? "Sin rol");
                    cmd.Parameters.AddWithValue("@FechaRegistro", usuario.fecha_registro);
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
                string query = "SELECT IdUsuario, Nombre, Correo, Contraseña, Direccion, Telefono, Rol, FechaRegistro FROM usuarios WHERE IdUsuario = @IdUsuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", id);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario = new Usuario
                    {
                        id_usuario = reader.GetInt32("IdUsuario"),
                        nombre = reader.GetString("Nombre"),
                        correo = reader.GetString("Correo"),
                        contrasena = reader.GetString("Contraseña"),
                        direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? "" : reader.GetString("Direccion"),
                        telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "" : reader.GetString("Telefono"),
                        rol = reader.IsDBNull(reader.GetOrdinal("Rol")) ? "Sin rol" : reader.GetString("Rol"),
                        fecha_registro = reader.GetDateTime("FechaRegistro")
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
                    string query = "UPDATE usuarios SET Nombre = @Nombre, Correo = @Correo, Contraseña = @Contraseña, Direccion = @Direccion, Telefono = @Telefono, Rol = @Rol WHERE IdUsuario = @IdUsuario";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.id_usuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@Correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@Contraseña", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@Direccion", usuario.direccion ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.telefono ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Rol", usuario.rol ?? "Sin rol");
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
                string query = "DELETE FROM usuarios WHERE IdUsuario = @IdUsuario";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@IdUsuario", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("ListarUsuarios");
        }
    }
}