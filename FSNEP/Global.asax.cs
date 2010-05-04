using System.Web.Mvc;
using Castle.Windsor;
using FSNEP.Controllers;
using MvcContrib.Castle;
using CAESArch.Data.NHibernate;
using NHibernate;

namespace FSNEP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            #if DEBUG
            HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
            #endif

            //Register the routes for this site
            new RouteConfigurator().RegisterRoutes();

            var windsorContainer = InitializeDependencyLocator();

            //Configure the audit interceptor
            NHibernateSessionManager.Instance.RegisterInterceptor(windsorContainer.Resolve<IInterceptor>());
        }

        private static IWindsorContainer InitializeDependencyLocator()
        {
            var container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);

            ComponentRegistrar.AddComponentsTo(container);

            return container;
        }
    }
}