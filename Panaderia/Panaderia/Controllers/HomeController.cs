using System.Web.Mvc;
using Panaderia.Models;
namespace Panaderia.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataBaseConnection db = new DataBaseConnection();
            bool resultado = db.ProbarConexion();

            if (resultado)
            {
                ViewBag.Mensaje = "Conexión exitosa a MySQL.";
            }
            else
            {
                ViewBag.Mensaje = "Fallo en la conexión a MySQL.";
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}