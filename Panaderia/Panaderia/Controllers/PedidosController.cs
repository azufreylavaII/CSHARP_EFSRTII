using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web.Mvc;
using Panaderia.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using System.Text;

using System.IO;

namespace Panaderia.Controllers
{
    public class PedidoController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        // LISTAR PEDIDOS PENDIENTES
        public ActionResult Index()
        {
            List<Pedido> pedidos = new List<Pedido>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Pedidos WHERE Estado = 'Pendiente'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pedidos.Add(new Pedido
                        {
                            IdPedido = Convert.ToInt32(reader["id_pedido"]),
                            IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                            FechaPedido = Convert.ToDateTime(reader["fecha_Pedido"]),
                            Estado = reader["estado"].ToString(),
                            Total = Convert.ToDecimal(reader["total"]),
                            MetodoPago = reader["metodo_pago"].ToString()
                        });
                    }
                }
            }
            return View(pedidos);
        }

        // EDITAR ESTADO DEL PEDIDO
        [HttpPost]
        public ActionResult EditarEstado(int id, string nuevoEstado)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Pedidos SET estado = @Estado WHERE id_Pedido = @IdPedido";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@IdPedido", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // DESCARGAR PDF DE PEDIDOS PENDIENTES
        public ActionResult DescargarPedidosPDF()
        {
            // Crear el documento PDF en memoria
            using (MemoryStream ms = new MemoryStream())
            {
                Document documento = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(documento, ms);

                documento.Open();

                // Agregar título
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph titulo = new Paragraph("Reporte de Pedidos Pendientes", titleFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20f;
                documento.Add(titulo);

                // Agregar fecha del reporte
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                Paragraph fecha = new Paragraph($"Fecha de generación: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}", normalFont);
                fecha.SpacingAfter = 20f;
                documento.Add(fecha);

                // Crear tabla
                PdfPTable tabla = new PdfPTable(6);
                tabla.WidthPercentage = 100;
                float[] anchos = new float[] { 0.5f, 0.8f, 1.2f, 0.8f, 0.8f, 1f };
                tabla.SetWidths(anchos);

                // Estilo para encabezados
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                // Agregar encabezados
                string[] headers = { "ID", "Usuario", "Fecha", "Estado", "Total", "Método Pago" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont));
                    cell.BackgroundColor = new BaseColor(240, 240, 240);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    tabla.AddCell(cell);
                }

                // Obtener datos de pedidos
                List<Pedido> pedidos = new List<Pedido>();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Pedidos WHERE Estado = 'Pendiente'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pedidos.Add(new Pedido
                            {
                                IdPedido = Convert.ToInt32(reader["id_pedido"]),
                                IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                FechaPedido = Convert.ToDateTime(reader["fecha_Pedido"]),
                                Estado = reader["estado"].ToString(),
                                Total = Convert.ToDecimal(reader["total"]),
                                MetodoPago = reader["metodo_pago"].ToString()
                            });
                        }
                    }
                }

                // Agregar datos a la tabla
                foreach (var pedido in pedidos)
                {
                    tabla.AddCell(new PdfPCell(new Phrase(pedido.IdPedido.ToString(), cellFont)));
                    tabla.AddCell(new PdfPCell(new Phrase(pedido.IdUsuario.ToString(), cellFont)));
                    tabla.AddCell(new PdfPCell(new Phrase(pedido.FechaPedido.ToString("dd/MM/yyyy HH:mm"), cellFont)));
                    tabla.AddCell(new PdfPCell(new Phrase(pedido.Estado, cellFont)));
                    tabla.AddCell(new PdfPCell(new Phrase($"${pedido.Total:N2}", cellFont)));
                    tabla.AddCell(new PdfPCell(new Phrase(pedido.MetodoPago, cellFont)));
                }

                documento.Add(tabla);

                // Agregar resumen
                

                documento.Close();

                // Retornar el PDF
                byte[] archivoPDF = ms.ToArray();
                return File(archivoPDF, "application/pdf", $"PedidosPendientes_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

    }
}
