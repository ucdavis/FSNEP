using System.Security.Principal;
using Castle.Windsor;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Impl;
using FSNEP.Core.Abstractions;
using System.Web.Security;
using FSNEP.BLL.Interfaces;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Core.CommonValidator;
using FSNEP.Core.Calendar;

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

            container.AddComponent("userAuth", typeof (IUserAuth), typeof (UserAuth));

            container.AddComponent("signatureFactory", typeof (ISignatureFactory), typeof (SignatureFactory));

            container.AddComponent("timeRecordCalendar", typeof (ITimeRecordCalendarGenerator),
                                   typeof (TimeRecordCalendarGenerator));

            container.AddComponent("auditInterceptor", typeof (NHibernate.IInterceptor), typeof (AuditInterceptor));
            
            AddRepositoriesTo(container);

            AddBLLClassesTo(container);

            container.AddComponent("validator", typeof (IValidator), typeof (Validator));
            container.AddComponent("dbContext", typeof (IDbContext), typeof (DbContext));
        }

        private static void AddRepositoriesTo(IWindsorContainer container)
        {
            container.AddComponent("repository", typeof (IRepository), typeof (Repository));
            container.AddComponent("genericRepository", typeof (IRepository<>), typeof (Repository<>));
            container.AddComponent("typedRepository", typeof (IRepositoryWithTypedId<,>),
                                   typeof (RepositoryWithTypedId<,>));

        }

        /// <summary>
        /// Adds all of the implemented classes within the BLL assembly and their first interfaces to the windsor container.
        /// </summary>
        private static void AddBLLClassesTo(IWindsorContainer container)
        {
            container.AddComponent("userBLL", typeof (IUserBLL), typeof (UserBLL));
            container.AddComponent("reportBLL", typeof (IReportBLL), typeof (ReportBLL));

            #if DEBUG

            container.AddComponent("devTimeRecordBLL", typeof(ITimeRecordBLL), typeof(DevTimeRecordBLL));
            container.AddComponent("devCostShareBLL", typeof(ICostShareBLL), typeof(DevCostShareBLL));
            container.AddComponent("devDelegateBLL", typeof (IDelegateBLL), typeof (DevDelegateBLL));

            #else

            container.AddComponent("TimeRecordBLL", typeof(ITimeRecordBLL), typeof(TimeRecordBLL));
            container.AddComponent("CostShareBLL", typeof(ICostShareBLL), typeof(CostShareBLL));
            container.AddComponent("DelegateBLL", typeof (IDelegateBLL), typeof (DelegateBLL));

            #endif

            /*
            var types = typeof(GenericBLL<,>).Assembly.GetTypes().Where(t => t.IsInterface == false && t.IsAbstract == false); //.Where(a => a.BaseType == typeof (GenericBLL<,>));

            foreach (var type in types)
            {
                var matchingInterface = type.GetInterfaces().FirstOrDefault();

                if (matchingInterface != null)
                {
                    container.AddComponent(type.Name, matchingInterface, type);
                }
            }
             */
        }
    }
}