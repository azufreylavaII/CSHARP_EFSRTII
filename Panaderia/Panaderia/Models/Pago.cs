using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panaderia.Models
{
   

    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [ForeignKey("Pedido")]
        public int IdPedido { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [MaxLength(50)]
        public string MetodoPago { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; }

        // Relación con Pedido
        public Pedido Pedido { get; set; }
    }

}