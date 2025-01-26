using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        [MaxLength(255)]
        public string ImagenUrl { get; set; }

        [ForeignKey("Categoria")]
        public int? IdCategoria { get; set; }

        public Categoria Categoria { get; set; }
    }
}