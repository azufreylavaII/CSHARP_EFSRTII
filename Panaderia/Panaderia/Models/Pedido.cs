using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
   

    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; }

        [Required]
        public decimal Total { get; set; }

        public String MetodoPago { get; set; }
        // Relaciones
        public Usuario Usuario { get; set; }
        public ICollection<DetallePedido> Detalles { get; set; }
    }

}