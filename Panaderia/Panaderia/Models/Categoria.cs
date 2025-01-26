using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }
    }
}