using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Archivos.Controllers
{   [Authorize]
    public class PadronesController : Controller
    {
        // GET: Padrones
        [Authorize(Roles="Admin,Padrones")]
        public ActionResult Index()
        {
            return View();
        }
    }
}