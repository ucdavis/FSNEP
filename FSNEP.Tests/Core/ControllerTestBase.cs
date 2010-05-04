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
        
        protected ControllerTestBase()
        {
            new RouteConfigurator().RegisterRoutes();
        }

        [TestInitialize]
        public void Setup()
        {
            Builder = new TestControllerBuilder();

            SetupController();
        }

        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected virtual void SetupController()
        {
            Controller = Builder.CreateController<CT>();
        }

        protected void CreateController(params object[] constructorArgs)
        {
            Controller = Builder.CreateController<CT>(constructorArgs);
        }

        protected static INonStaticGenericBLLBase<T, IdT> GetStubRepository<T, IdT>()
        {
            return MockRepository.GenerateStub<INonStaticGenericBLLBase<T, IdT>>();
        }
    }
}
