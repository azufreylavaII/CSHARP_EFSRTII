using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panaderia.Controllers
{
    public class CarritosController : Controller
    {
        // GET: Carritos
        public ActionResult Index()
        {
            return View();
        }
    }
}