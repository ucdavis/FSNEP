using System.Web.Mvc;

namespace FSNEP.Controllers
{
    public class HomeController : SuperController
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Error(string errorMessage)
        {
            return View("Error", errorMessage);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
