using System;
using CAESArch.BLL;
using Castle.Windsor;
using MvcContrib.Castle;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.Controllers;
using MvcContrib.Services;
using System.Web.Mvc;
using CAESArch.IoC;

namespace FSNEP.Tests.Core
{
    [TestClass]
    public abstract class ControllerTestBase<CT> where CT : Controller
    {
        protected CT Controller { get; private set; }
        private TestControllerBuilder Builder { get; set; }
        protected IWindsorContainer container = new WindsorContainer();

        protected ControllerTestBase()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        [TestInitialize]
        public void Setup()
        {
            ServiceLocator.Clear();

            Builder = new TestControllerBuilder();
            container = ServiceLocator.Container;

            container.RegisterControllers(typeof(HomeController).Assembly); //Add all of the controllers

            AddComponents();

            DependencyResolver.InitializeWith(new WindsorDependencyResolver(container));

            Controller = Builder.CreateIoCController<CT>();
        }

        protected virtual void AddComponents()
        {
            
        }

        protected static INonStaticGenericBLLBase<T, IdT> GetStubRepository<T, IdT>()
        {
            return MockRepository.GenerateStub<INonStaticGenericBLLBase<T, IdT>>();
        }
    }
}
