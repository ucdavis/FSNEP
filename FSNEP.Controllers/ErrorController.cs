using UCDArch.Web.Attributes;
using System.Web.Mvc;

namespace FSNEP.Controllers
{
    [HandleTransactionsManually]
    public class ErrorController : ApplicationController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}