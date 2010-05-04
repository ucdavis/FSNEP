using System.Linq;
using System.Security.Principal;
using Castle.Windsor;
using FSNEP.BLL.Impl;
using FSNEP.Core.Abstractions;

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

            AddBLLClassesTo(container);
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