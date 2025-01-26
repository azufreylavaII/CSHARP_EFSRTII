using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Panaderia.Models
{


    public class Carrito
    {
        [Key]
        public int IdCarrito { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("Producto")]
        public int IdProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public DateTime FechaAgregado { get; set; }

        // Relación con Usuario
        public Usuario Usuario { get; set; }

        // Relación con Producto
        public Producto Producto { get; set; }
    }

}