using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.Routing;

namespace FSNEP
{
    public class RouteConfigurator
    {
        public virtual void RegisterRoutes()
        {
            RouteCollection routes = RouteTable.Routes;
            routes.Clear();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            /*
            MvcRoute.MappUrl("{controller}/{action}/{username}")
                .WithDefaults(new {controller = "Account", action = "LogOn", username = ""})
                .AddWithName("Account", routes);
            */

            MvcRoute.MappUrl("Administration/Users/{action}/{id}")
                .WithDefaults(new {controller = "UserAdministration", action = "List", id = ""})
                .AddWithName("UserAdministration", routes);

            MvcRoute.MappUrl("Administration/Lookups/{action}/{id}")
                .WithDefaults(new {controller = "Lookup", action = "Index", id=""})
                .AddWithName("Lookups", routes);

            MvcRoute.MappUrl("Administration/Association/{action}/{id}")
                .WithDefaults(new { controller = "Association", action = "Index", id = "" })
                .AddWithName("Associations", routes);
            
            MvcRoute.MappUrl("{controller}/{action}/{id}")
                .WithDefaults(new { controller = "Home", action = "Index", id = "" })
                .AddWithName("Default", routes);
        }
    }
}