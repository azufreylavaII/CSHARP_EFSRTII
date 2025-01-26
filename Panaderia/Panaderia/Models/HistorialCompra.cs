using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
 
    public class HistorialCompra
    {
        [Key]
        public int IdHistorial { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [Required]
        public decimal MontoTotal { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; }
        public Pedido Pedido { get; set; }
    }

}