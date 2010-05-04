using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;

namespace FSNEP.Controllers
{
    [HandleErrorWithELMAH]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
