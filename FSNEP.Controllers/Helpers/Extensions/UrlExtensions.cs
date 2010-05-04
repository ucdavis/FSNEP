using System;
using System.Web.Mvc;

namespace FSNEP.Controllers.Helpers.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}{1}",
                                                  requestUrl.GetLeftPart(UriPartial.Authority),
                                                  url.Action(action, controller, routeValues));

            return absoluteAction;
        }
    }
}
