using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Panaderia.Models
{
   

    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreRol { get; set; }

        [MaxLength(255)]
        public string Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }
    }

}