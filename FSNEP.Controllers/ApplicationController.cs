using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using MvcContrib;
using UCDArch.Web.Controller;

namespace FSNEP.Controllers
{
    [HandleErrorWithELMAH]
    [MenuLocationFilter]
    [Version(MajorVersion = 2)]
    public class ApplicationController : SuperController
    {
        protected RedirectToRouteResult RedirectToErrorPage(string message)
        {
            Message = message;
            //return RedirectToAction("Error", "Home");
            return this.RedirectToAction<HomeController>(a => a.Error(message));
        }
    }
}