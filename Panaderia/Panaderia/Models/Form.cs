using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panaderia.Models
{
    public class Form
    {
        public int IdFormulario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Mensaje { get; set; }
        public DateTime? FechaEnvio { get; set; }
    }
}