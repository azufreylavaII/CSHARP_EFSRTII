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

        public DateTime FechaPedido { get; set; }

        [MaxLength(100)]
        public string Estado { get; set; }

        [Required]
        public decimal Total { get; set; }

        [MaxLength(50)]
        public string MetodoPago { get; set; }

        public Usuario Usuario { get; set; }
    }
}