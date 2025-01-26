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

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nombre { get; set; }

        [MaxLength(255)]
        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Relación con Categoría
        public Categoria Categoria { get; set; }
    }

}