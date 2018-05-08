using System.Web.Mvc;

namespace DPO.Web.Controllers
{
    public class TermsController : Controller
    {
        //example: redirUrl='http://bim.daikincity.com/category/vrv'
        public ActionResult BimTOS(string redirUrl)
        {
            ViewBag.RedirectUrl = redirUrl;
            return View("BimTOS");
        }
    }
}