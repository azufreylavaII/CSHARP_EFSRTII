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
            public int id_usuario { get; set; } // ID autoincremental

            [Required]
            public string nombre { get; set; } // Nombre del usuario

            [Required, EmailAddress]
            public string correo { get; set; } // Correo único

            [Required]
            public string contrasena { get; set; } // Contraseña (debería estar hasheada)

            public string direccion { get; set; } // Dirección del usuario

            public string telefono { get; set; } // Teléfono del usuario

           // public string rol { get; set; } // Rol (admin, cliente, etc.)
        public string rol { get; set; } = "Cliente"; // Valor por defecto


        public string imagen_url { get; set; } // URL de la imagen de perfil

            public DateTime fecha_registro { get; set; } = DateTime.Now; // Fecha de registro
        }
    }

