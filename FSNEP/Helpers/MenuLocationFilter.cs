using System.Web.Mvc;

namespace FSNEP.Helpers
{
    public class MenuLocationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewData["MenuLocation"] = filterContext.RouteData.Values["controller"];

            base.OnActionExecuting(filterContext);
        }
    }
}