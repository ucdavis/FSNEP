using System.Collections.Specialized;
using System.Web.Mvc;
using System;

namespace FSNEP.Controllers.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MenuLocationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Maps Controller names to the proper menu location
        /// </summary>
        public static readonly StringDictionary MenuLocationMap = new StringDictionary
                                                                      {
                                                                          {"Home", "home"},
                                                                          {"TimeRecord", "time"},
                                                                          {"CostShare", "cost"},
                                                                          {"Supervisor", "super"},
                                                                          {"Association", "admin"},
                                                                          {"Audit", "admin"},
                                                                          {"Lookup", "admin"},
                                                                          {"UserAdministration", "admin"},
                                                                          {"Report", "reports"}
                                                                      };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"] as string;

            if (!string.IsNullOrEmpty(controllerName))
            {
                filterContext.Controller.ViewData["MenuLocation"] = MenuLocationMap[controllerName];
            }

            base.OnActionExecuting(filterContext);
        }
    }
}