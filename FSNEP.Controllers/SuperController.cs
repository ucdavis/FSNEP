using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using MvcContrib;

namespace FSNEP.Controllers
{
    [HandleErrorWithELMAH]
    public class SuperController : UCDArch.Web.Controller.SuperController
    {
        protected RedirectToRouteResult RedirectToErrorPage(string message)
        {
            Message = message;
            //return RedirectToAction("Error", "Home");
            return this.RedirectToAction<HomeController>(a => a.Error(message));
        }
    }
}