using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPO.Web.Controllers
{
    public class Angular2Controller : Controller
    {
        // GET: Angular2
        public ActionResult Index(string path)
        {
            return Redirect($"/v2/#/{path}");
            //return View();
        }

     
    }
}