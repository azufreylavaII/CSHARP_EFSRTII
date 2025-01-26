using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panaderia.Models;

namespace Panaderia.Controllers
{
        public class RolesController : Controller
        {
            // Simulación de una base de datos (esto debe reemplazarse con tu servicio o DbContext)
            private static List<Rol> roles = new List<Rol>
        {
            new Rol { IdRol = 1, NombreRol = "Administrador", Descripcion = "Acceso completo", Estado = true },
            new Rol { IdRol = 2, NombreRol = "Cliente", Descripcion = "Acceso limitado a compras", Estado = true }
        };

            // 1. Listar todos los roles
            public ActionResult Index()
            {
                return View(roles); // Envía la lista de roles a la vista
            }

            // 2. Crear un nuevo rol (GET)
            public ActionResult Create()
            {
                return View();
            }

            // 2. Crear un nuevo rol (POST)
            [HttpPost]
            public ActionResult Create(Rol nuevoRol)
            {
                if (ModelState.IsValid)
                {
                    nuevoRol.IdRol = roles.Count + 1; // Asignar un nuevo Id (simulado)
                    roles.Add(nuevoRol);
                    return RedirectToAction("Index");
                }
                return View(nuevoRol);
            }

            // 3. Editar un rol (GET)
            public ActionResult Edit(int id)
            {
                var rol = roles.FirstOrDefault(r => r.IdRol == id);
                if (rol == null) return HttpNotFound();
                return View(rol);
            }

            // 3. Editar un rol (POST)
            [HttpPost]
            public ActionResult Edit(Rol rolEditado)
            {
                if (ModelState.IsValid)
                {
                    var rol = roles.FirstOrDefault(r => r.IdRol == rolEditado.IdRol);
                    if (rol != null)
                    {
                        rol.NombreRol = rolEditado.NombreRol;
                        rol.Descripcion = rolEditado.Descripcion;
                        rol.Estado = rolEditado.Estado;
                    }
                    return RedirectToAction("Index");
                }
                return View(rolEditado);
            }

            // 4. Desactivar un rol
            public ActionResult Disable(int id)
            {
                var rol = roles.FirstOrDefault(r => r.IdRol == id);
                if (rol != null)
                {
                    rol.Estado = false; // Cambiar el estado a inactivo
                }
                return RedirectToAction("Index");
            }
        }
    

}