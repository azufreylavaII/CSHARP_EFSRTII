using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Correo { get; set; }

        [Required]
        [MaxLength(255)]
        public string Contraseña { get; set; }

        public string Direccion { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }

        [MaxLength(100)]
        public string Rol { get; set; }

        [MaxLength(255)]
        public string ImagenUrl { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}