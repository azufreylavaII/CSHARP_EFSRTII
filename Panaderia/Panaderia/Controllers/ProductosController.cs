using MySql.Data.MySqlClient;
using Panaderia.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panaderia.Controllers
{
    public class ProductosController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        
        public ActionResult Index()
        {
            List<Producto> productos = new List<Producto>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id_producto, nombre, precio FROM Productos";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
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
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al obtener productos: " + ex.Message;
                }
            }

            return View(productos);
        }

        // 2. Agregar producto al carrito
        [HttpPost]
        public ActionResult AgregarAlCarrito(int idProducto, int cantidad)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Verificar si el producto ya está en el carrito
                    string checkQuery = "SELECT id_carrito, cantidad FROM Carrito WHERE id_usuario = @id_usuario AND id_producto = @id_producto";

                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                        checkCommand.Parameters.AddWithValue("@id_producto", idProducto);

                        using (MySqlDataReader reader = checkCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Si el producto ya existe en el carrito, actualizar la cantidad
                                int idCarrito = Convert.ToInt32(reader["id_carrito"]);
                                int cantidadExistente = Convert.ToInt32(reader["cantidad"]);
                                int nuevaCantidad = cantidadExistente + cantidad;

                                reader.Close();

                                string updateQuery = "UPDATE Carrito SET cantidad = @cantidad WHERE id_carrito = @id_carrito";

                                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@cantidad", nuevaCantidad);
                                    updateCommand.Parameters.AddWithValue("@id_carrito", idCarrito);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Si el producto no está en el carrito, insertarlo
                                reader.Close();

                                string insertQuery = "INSERT INTO Carrito (id_usuario, id_producto, cantidad, fecha_agregado) VALUES (@id_usuario, @id_producto, @cantidad, NOW())";

                                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                                    insertCommand.Parameters.AddWithValue("@id_producto", idProducto);
                                    insertCommand.Parameters.AddWithValue("@cantidad", cantidad);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al agregar al carrito: " + ex.Message;
                }
            }

            return RedirectToAction("VerCarrito");
        }


        // 3. Ver carrito del usuario
        public ActionResult VerCarrito()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            List<CarritoItem> carrito = new List<CarritoItem>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT c.id_carrito, c.id_producto, p.nombre, p.precio, c.cantidad, 
                        (p.precio * c.cantidad) AS subtotal 
                 FROM Carrito c 
                 JOIN Productos p ON c.id_producto = p.id_producto 
                 WHERE c.id_usuario = @id_usuario";


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_usuario", idUsuario);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                carrito.Add(new CarritoItem
                                {
                                    IdCarrito = Convert.ToInt32(reader["id_carrito"]),
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),  
                                    NombreProducto = reader["nombre"].ToString(),
                                    PrecioUnitario = Convert.ToDecimal(reader["precio"]),
                                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                                    Subtotal = Convert.ToDecimal(reader["subtotal"])
                                });

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al cargar el carrito: " + ex.Message;
                }
            }

            return View(carrito);
        }

        [HttpPost]
        public ActionResult EliminarDelCarrito(int idProducto)
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Carrito WHERE id_usuario = @id_usuario AND id_producto = @id_producto";

                    using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                        deleteCommand.Parameters.AddWithValue("@id_producto", idProducto);
                        deleteCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al eliminar del carrito: " + ex.Message;
                }
            }

            return RedirectToAction("VerCarrito");
        }

        public ActionResult ConfirmacionPedido(int idPedido)
        {
            Pedido pedido = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT id_pedido, id_usuario, fecha_pedido, estado, total, metodo_pago FROM Pedidos WHERE id_pedido = @id_pedido";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_pedido", idPedido);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pedido = new Pedido
                                {
                                    IdPedido = Convert.ToInt32(reader["id_pedido"]),
                                    IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                    FechaPedido = Convert.ToDateTime(reader["fecha_pedido"]),
                                    Estado = reader["estado"].ToString(),
                                    Total = Convert.ToDecimal(reader["total"]),
                                    MetodoPago = reader["metodo_pago"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al recuperar el pedido: " + ex.Message;
                }
            }

            if (pedido == null)
            {
                ViewBag.Error = "No se encontró el pedido.";
                return RedirectToAction("VerCarrito");
            }

            return View(pedido);
        }



        [HttpPost]
        public ActionResult TerminarPedido()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            int idUsuario = Convert.ToInt32(Session["UsuarioID"]);
            int idPedido = 0;
            decimal total = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Obtener productos del carrito
                    List<CarritoItem> carrito = new List<CarritoItem>();
                    string carritoQuery = @"SELECT c.id_producto, c.cantidad, p.precio 
                                    FROM Carrito c
                                    JOIN Productos p ON c.id_producto = p.id_producto
                                    WHERE c.id_usuario = @id_usuario";

                    using (MySqlCommand carritoCommand = new MySqlCommand(carritoQuery, connection))
                    {
                        carritoCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                        using (MySqlDataReader reader = carritoCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                carrito.Add(new CarritoItem
                                {
                                    IdProducto = Convert.ToInt32(reader["id_producto"]),
                                    Cantidad = Convert.ToInt32(reader["cantidad"]),
                                    PrecioUnitario = Convert.ToDecimal(reader["precio"])
                                });
                            }
                        }
                    }

                    if (carrito.Count == 0)
                    {
                        ViewBag.Error = "El carrito está vacío.";
                        return RedirectToAction("VerCarrito");
                    }

                    total = carrito.Sum(item => item.Cantidad * item.PrecioUnitario);

                    // Insertar el pedido
                    string insertPedidoQuery = @"INSERT INTO Pedidos (id_usuario, fecha_pedido, estado, total, metodo_pago) 
                                         VALUES (@id_usuario, NOW(), 'Pendiente', @total, 'Efectivo')";
                    using (MySqlCommand insertPedidoCommand = new MySqlCommand(insertPedidoQuery, connection))
                    {
                        insertPedidoCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                        insertPedidoCommand.Parameters.AddWithValue("@total", total);
                        insertPedidoCommand.ExecuteNonQuery();
                        idPedido = (int)insertPedidoCommand.LastInsertedId;
                    }

                    

                    // Insertar detalles del pedido
                    string insertDetalleQuery = @"INSERT INTO Detalles_Pedido (id_pedido, id_producto, cantidad, precio_unitario, subtotal) 
                                          VALUES (@id_pedido, @id_producto, @cantidad, @precio_unitario, @subtotal)";
                    foreach (var item in carrito)
                    {
                        using (MySqlCommand insertDetalleCommand = new MySqlCommand(insertDetalleQuery, connection))
                        {
                            insertDetalleCommand.Parameters.AddWithValue("@id_pedido", idPedido);
                            insertDetalleCommand.Parameters.AddWithValue("@id_producto", item.IdProducto);
                            insertDetalleCommand.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            insertDetalleCommand.Parameters.AddWithValue("@precio_unitario", item.PrecioUnitario);
                            insertDetalleCommand.Parameters.AddWithValue("@subtotal", item.Cantidad * item.PrecioUnitario);
                            insertDetalleCommand.ExecuteNonQuery();
                        }
                    }

                    // Vaciar el carrito
                    string deleteCarritoQuery = "DELETE FROM Carrito WHERE id_usuario = @id_usuario";
                    using (MySqlCommand deleteCarritoCommand = new MySqlCommand(deleteCarritoQuery, connection))
                    {
                        deleteCarritoCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                        deleteCarritoCommand.ExecuteNonQuery();
                    }

                    Pedido pedido = null;
                    string selectPedidoQuery = "SELECT id_pedido, id_usuario, fecha_pedido, estado, total, metodo_pago FROM Pedidos WHERE id_pedido = @id_pedido";

                    using (MySqlCommand selectPedidoCommand = new MySqlCommand(selectPedidoQuery, connection))
                    {
                        selectPedidoCommand.Parameters.AddWithValue("@id_pedido", idPedido);

                        using (MySqlDataReader reader = selectPedidoCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pedido = new Pedido
                                {
                                    IdPedido = Convert.ToInt32(reader["id_pedido"]),
                                    IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                    FechaPedido = Convert.ToDateTime(reader["fecha_pedido"]),
                                    Estado = reader["estado"].ToString(),
                                    Total = Convert.ToDecimal(reader["total"]),
                                    MetodoPago = reader["metodo_pago"].ToString()
                                };
                            }
                        }
                    }

                    // Verificar que se obtuvo el pedido antes de redirigir
                    if (pedido == null)
                    {
                        ViewBag.Error = "Error al recuperar el pedido.";
                        return RedirectToAction("VerCarrito");
                    }

                    return RedirectToAction("ConfirmacionPedido", new { idPedido = pedido.IdPedido });
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al finalizar el pedido: " + ex.Message;
                    return RedirectToAction("VerCarrito");
                }
            }
        }


    }
}