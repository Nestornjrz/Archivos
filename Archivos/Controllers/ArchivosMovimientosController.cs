using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Archivos.Controllers
{
    [Authorize]
    public class ArchivosMovimientosController : Controller
    {
        // GET: ArchivosMovimientos
        [Authorize(Roles="Operador")]
        public ActionResult Index()
        {
            return View();
        }
    }
}