using System.Linq;
using System.Security.Principal;
using Castle.Windsor;
using FSNEP.BLL.Impl;
using FSNEP.Core.Abstractions;
using System.Web.Security;
using CAESArch.Core.DataInterfaces;
using CAESArch.BLL.Repositories;

namespace FSNEP
{
    public static class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            container.AddComponent("messageGateway", typeof(IMessageGateway), typeof(MessageGateway));

            container.AddComponent("membershipService", typeof(IMembershipService), typeof(AccountMembershipService));
            container.AddComponent("formsAuth", typeof(IFormsAuthentication), typeof(FormsAuthenticationService));

            container.AddComponent("principal", typeof (IPrincipal), typeof (WebPrincipal));

            container.AddComponent("roleProvider", typeof (RoleProvider), typeof (RoleProviderService));

            AddRepositoriesTo(container);

            AddBLLClassesTo(container);
        }

        private static void AddRepositoriesTo(IWindsorContainer container)
        {
            container.AddComponent("repository", typeof (IRepository), typeof (Repository));
        }

        /// <summary>
        /// Adds all of the implemented classes within the BLL assembly and their first interfaces to the windsor container.
        /// </summary>
        private static void AddBLLClassesTo(IWindsorContainer container)
        {
            var types = typeof(GenericBLL<,>).Assembly.GetTypes().Where(t => t.IsInterface == false && t.IsAbstract == false); //.Where(a => a.BaseType == typeof (GenericBLL<,>));

            foreach (var type in types)
            {
                var matchingInterface = type.GetInterfaces().FirstOrDefault();

                if (matchingInterface != null)
                {
                    container.AddComponent(type.Name, matchingInterface, type);
                }
            }
        }
    }
}